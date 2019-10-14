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
    (physicalDevice: VkPhysicalDevice,
     device: VkDevice,
     commandPool: VkCommandPool,
     transferQueue: VkQueue,
     swapChain: SwapChain) =

    let gate = obj ()
    let buffers = Collections.Generic.Dictionary<VkBuffer, FalBuffer> ()
    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "Vulkan instance is disposed."

    let destroyBuffer (falBuffer: FalBuffer) =
        vkDestroyBuffer(device, falBuffer.Buffer, vkNullPtr)
        vkFreeMemory(device, falBuffer.Memory, vkNullPtr)

    member __.AddShader (vertexBindings, vertexAttributes, vertexBytes, fragmentBytes) : PipelineIndex =
        checkDispose ()
        swapChain.AddShader (vertexBindings, vertexAttributes, vertexBytes, fragmentBytes)

    member _.RecordDraw (pipelineIndex, vertexBuffers: FalBuffer [], vertexCount, instanceCount) =
        checkDispose ()
        swapChain.RecordDraw (pipelineIndex, vertexBuffers |> Array.map (fun x -> x.Buffer), vertexCount, instanceCount)

    member __.DrawFrame () =
        checkDispose ()
        swapChain.DrawFrame ()

    member __.WaitIdle () =
        checkDispose ()
        swapChain.WaitIdle ()

    [<RequiresExplicitTypeArguments>]
    member _.CreateVertexBuffer<'T when 'T : unmanaged> count =
        lock gate <| fun _ ->

        checkDispose ()   

        let falBuffer = 
            VertexBuffer<'T>.Create(
                physicalDevice, device, commandPool, transferQueue, 
                count, VkBufferUsageFlags.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT ||| VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_DST_BIT)
        buffers.Add (falBuffer.Buffer, falBuffer)
        falBuffer

    member _.DestroyBuffer (buffer: FalBuffer) =
        lock gate <| fun _ ->

        checkDispose ()
        match buffers.Remove buffer.Buffer : bool * FalBuffer with
        | true, falBuffer ->
            destroyBuffer falBuffer
        | _ ->
            failwith "Buffer is not in the vulkan instance."

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                failwith "VulkanInstance already disposed"
            else
                GC.SuppressFinalize x

                (swapChain :> IDisposable).Dispose ()

                lock gate (fun () ->
                    buffers.Values
                    |> Seq.iter destroyBuffer

                    buffers.Clear()
                )

                vkDestroyCommandPool(device, commandPool, vkNullPtr)

    static member Create(falDevice: FalDevice) =
        let physicalDevice = falDevice.PhysicalDevice
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

        let swapChain = SwapChain.Create(physicalDevice, device, surface, graphicsFamily, presentFamily, commandPool)

        new FalGraphics(physicalDevice, device, commandPool, transferQueue, swapChain)
