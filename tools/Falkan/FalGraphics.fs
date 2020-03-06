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
     commandPool: VkCommandPool,
     transferQueue: VkQueue,
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

    member __.AddShader (vertexBindings, vertexAttributes, vertexBytes, fragmentBytes) : PipelineIndex =
        checkDispose ()
        swapChain.AddShader (vertexBindings, vertexAttributes, vertexBytes, fragmentBytes)

    member _.RecordDraw (pipelineIndex, vertexBuffers, vertexCount, instanceCount) =
        checkDispose ()
        swapChain.RecordDraw (pipelineIndex, vertexBuffers, vertexCount, instanceCount)

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

        buffer.SetData<'T>(data, commandPool, transferQueue)

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

        fillImage physicalDevice device commandPool transferQueue buffer.vkImage buffer.width buffer.height data

    member _.SetUniformBuffer<'T when 'T : unmanaged>(buffer: FalkanBuffer) =
        lock gate <| fun _ ->
            
        checkDispose ()

        swapChain.SetUniformBuffer(buffer.buffer, sizeof<'T>)

    member _.SetSampler(image: FalkanImage) =
        lock gate <| fun _ ->
            
        checkDispose ()

        swapChain.SetSampler(image.vkImageView, image.vkSampler)

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

                vkDestroyCommandPool(device, commandPool, vkNullPtr)

    static member Create(falDevice: FalDevice, invalidate) =
        let device = falDevice.Device
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

        let commandPool = mkCommandPool device graphicsFamily
        // TODO: We should try to use a transfer queue instead of a graphics queue. This works for now.
        let transferQueue = mkQueue device graphicsFamily

        let swapChain = SwapChain.Create(falDevice, surface, graphicsFamily, presentFamily, commandPool, invalidate)

        new FalGraphics(falDevice, commandPool, transferQueue, swapChain)
