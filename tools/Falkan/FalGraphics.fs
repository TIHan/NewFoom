[<AutoOpen>]
module FsGame.Graphics.Vulkan.Graphics

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
    (fdevice: VulkanDevice,
     swapChain: SwapChain) =

    let device = fdevice.Device
    let gate = obj ()
    let buffers = Collections.Generic.Dictionary<VkBuffer, VulkanMemory>()
    let images = Collections.Generic.Dictionary<VkImage, FalkanImage> ()
    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "Vulkan instance is disposed."

    let destroyBuffer (buffer: VkBuffer) (memory: VulkanMemory) =
        vkDestroyBuffer(device, buffer, vkNullPtr)
        (memory :> IDisposable).Dispose()

    member _.SetupCommands() =
        swapChain.SetupCommands()

    member __.DrawFrame () =
        checkDispose ()
        swapChain.Run ()

    member __.WaitIdle () =
        checkDispose ()
        swapChain.WaitIdle ()

    [<RequiresExplicitTypeArguments>]
    member _.CreateBuffer<'T when 'T : unmanaged> (kind, flags, count)  =
        lock gate <| fun _ ->

        checkDispose ()   

        let buffer = fdevice.CreateBuffer<'T>(kind, flags, sizeof<'T> * count)
        buffers.Add(buffer.buffer, buffer.memory)
        buffer

    member this.CreateBuffer<'T when 'T : unmanaged> (kind, flags, data: ReadOnlySpan<'T>) =
        let buffer = this.CreateBuffer<'T>(kind, flags, sizeof<'T> * data.Length)
        buffer.Upload data
        buffer

    member this.CreateBuffer<'T when 'T : unmanaged> (kind, flags, data: 'T[]) =
        this.CreateBuffer<'T>(kind, flags, ReadOnlySpan data)

    member this.CreateBuffer<'T when 'T : unmanaged> (kind, flags, data: 'T) =
        let buffer = this.CreateBuffer<'T>(kind, flags, sizeof<'T>)
        buffer.Upload data
        buffer

    member _.DestroyBuffer (buffer: VulkanBuffer<_>) =
        lock gate <| fun _ ->

        checkDispose ()
        match buffers.Remove buffer.buffer with
        | true ->
            destroyBuffer buffer.buffer buffer.memory
        | _ ->
            failwith "Buffer is not in the vulkan instance."

    member _.CreateImage (width, height, data)  =
        let image =
            lock gate <| fun _ ->

            checkDispose ()   

            let image = fdevice.CreateImage(width, height)
            images.Add (image.vkImage, image)
            image
        fillImage fdevice fdevice.VkCommandPool fdevice.VkTransferQueue image.vkImage image.format image.width image.height data
        image

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

                    buffers
                    |> Seq.iter (fun pair -> destroyBuffer pair.Key pair.Value)

                    buffers.Clear()
                )

                lock gate (fun () ->
                    images.Values
                    |> Seq.iter (fun x -> x.Destroy())

                    images.Clear()
                )

    member this.CreateShader(shaderDesc: VulkanShaderDescription, vertexSpirvSource: ReadOnlySpan<byte>, fragmentSpirvSource: ReadOnlySpan<byte>) =
        swapChain.CreateShader(shaderDesc, vertexSpirvSource, fragmentSpirvSource)

    member this.CreateComputeShader(shaderDesc: VulkanShaderDescription, vertexSpirvSource: ReadOnlySpan<byte>) =
        swapChain.CreateComputeShader(shaderDesc, vertexSpirvSource)

    static member Create(falDevice: VulkanDevice, invalidate, renderSubpassDescs) =
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

    static member CreateCompute(falDevice: VulkanDevice, invalidate) =
        let indices = falDevice.Indices

        if not indices.HasCompute then
            failwith "Compute is not available."

        let swapChain = SwapChain.CreateCompute(falDevice, indices.computeFamily.Value, invalidate)

        new FalGraphics(falDevice, swapChain)
