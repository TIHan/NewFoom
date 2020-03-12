[<AutoOpen>]
module Falkan.Graphics

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

type PipelineIndex = int

[<Sealed>]
type FalGraphics
    private
    (fdevice: FalDevice,
     swapChain: SwapChain) =

    let device = fdevice.Device
    let physicalDevice = fdevice.PhysicalDevice
    let gate = obj ()
    let buffers = Collections.Generic.Dictionary<VkBuffer, FalkanBuffer> ()
    let images = Collections.Generic.Dictionary<VkImage, FalkanImage> ()
    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "Vulkan instance is disposed."

    member _.SetupCommands() =
        swapChain.SetupCommands()

    member __.DrawFrame () =
        checkDispose ()
        swapChain.DrawFrame ()

    member __.WaitIdle () =
        checkDispose ()
        swapChain.WaitIdle ()

    [<RequiresExplicitTypeArguments>]
    member _.CreateBuffer<'T when 'T : unmanaged> (size, flags, kind)  =
        lock gate <| fun _ ->

        checkDispose ()   

        let buffer = fdevice.CreateBuffer(kind, flags, sizeof<'T> * size)
        buffers.Add (buffer.buffer, buffer)
        buffer

    member _.FillBuffer<'T when 'T : unmanaged> (buffer: FalkanBuffer, data) =
        checkDispose ()

        buffer.SetData<'T>(data)

    member _.DestroyBuffer (buffer: FalkanBuffer) =
        lock gate <| fun _ ->

        checkDispose ()
        match buffers.Remove buffer.buffer with
        | true ->
            buffer.Destroy()
        | _ ->
            failwith "Buffer is not in the vulkan instance."

    member _.CreateImage (width, height)  =
        lock gate <| fun _ ->

        checkDispose ()   

        let image = fdevice.CreateImage(width, height)
        images.Add (image.vkImage, image)
        image

    member _.FillImage (buffer: FalkanImage, data) =
        checkDispose ()

        fillImage physicalDevice device fdevice.VkCommandPool fdevice.VkTransferQueue buffer.vkImage buffer.format buffer.width buffer.height data

    member _.AddRenderSubpass renderSubpassDesc =
        checkDispose ()

        swapChain.AddRenderSubpass renderSubpassDesc

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                failwith "VulkanInstance already disposed"
            else
                GC.SuppressFinalize x

                (swapChain :> IDisposable).Dispose ()

                lock gate (fun () ->
                    buffers.Values
                    |> Seq.iter (fun x -> x.Destroy())

                    buffers.Clear()
                )

                lock gate (fun () ->
                    images.Values
                    |> Seq.iter (fun x -> x.Destroy())

                    images.Clear()
                )

                buckets.Values
                |> Seq.iter (fun x -> 
                    if x.IsValueCreated then
                        (x.Value :> IDisposable).Dispose())

    member this.CreateShader(layout: FalkanShaderDescription, vertexSpirvSource: ReadOnlySpan<byte>, fragmentSpirvSource: ReadOnlySpan<byte>) =
        swapChain.CreateShader(layout, vertexSpirvSource, fragmentSpirvSource)

    static member Create(falDevice: FalDevice, invalidate, renderSubpassDescs) =
        let indices = falDevice.Indices
        let surface =
            match falDevice.Surface with
            | Some surface when indices.graphicsFamily.IsSome && indices.presentFamily.IsSome ->
                surface
            | _ ->
                failwith "Unable to get surface for graphics on device."

        let graphicsFamily = indices.graphicsFamily.Value
        let presentFamily = indices.presentFamily.Value

        if graphicsFamily <> presentFamily then
            failwith "Currently not able to handle concurrent graphics and present families."

        let swapChain = SwapChain.Create(falDevice, surface, graphicsFamily, presentFamily, invalidate, renderSubpassDescs)

        new FalGraphics(falDevice, swapChain)
