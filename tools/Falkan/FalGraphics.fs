﻿[<AutoOpen>]
module Falkan.Graphics

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let mkBuffer<'T when 'T : unmanaged> device count usage =
    let bufferInfo =
        VkBufferCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO,
            size = uint64 (sizeof<'T> * count),
            usage = usage,
            sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE
        )
    
    let vertexBuffer = VkBuffer ()
    vkCreateBuffer(device, &&bufferInfo, vkNullPtr, &&vertexBuffer) |> checkResult
    vertexBuffer

let getMemoryRequirements device buffer =
    let memRequirements = VkMemoryRequirements ()
    vkGetBufferMemoryRequirements(device, buffer, &&memRequirements)
    memRequirements

let getSuitableMemoryTypeIndex physicalDevice typeFilter properties =
    let memProperties = VkPhysicalDeviceMemoryProperties ()
    vkGetPhysicalDeviceMemoryProperties(physicalDevice, &&memProperties)

    [| for i = 0 to int memProperties.memoryTypeCount - 1 do
        if typeFilter &&& (1u <<< i) <> 0u then
            let memType = memProperties.memoryTypes.[i]
            if memType.propertyFlags &&& properties = properties then
                yield uint32 i |]
    |> Array.head

let allocateMemory physicalDevice device (memRequirements: VkMemoryRequirements) properties =
    let memTypeIndex = 
        getSuitableMemoryTypeIndex 
            physicalDevice memRequirements.memoryTypeBits 
            properties

    let allocInfo =
        VkMemoryAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
            allocationSize = memRequirements.size,
            memoryTypeIndex = memTypeIndex
        )

    let bufferMemory = VkDeviceMemory ()
    vkAllocateMemory(device, &&allocInfo, vkNullPtr, &&bufferMemory) |> checkResult
    bufferMemory

// TODO: We should not be allocating memory for every buffer. We need to allocate a chunk and just use a portion of it.
let bindMemory physicalDevice device buffer properties =
    let memRequirements = getMemoryRequirements device buffer
    let memory = allocateMemory physicalDevice device memRequirements properties
    vkBindBufferMemory(device, buffer, memory, 0UL) |> checkResult
    memory

let mapBuffer<'T when 'T : unmanaged> device memory (data: ReadOnlySpan<'T>) =
    let deviceData = nativeint 0
    let pDeviceData = &&deviceData |> NativePtr.toNativeInt

    vkMapMemory(device, memory, 0UL, uint64 (sizeof<'T> * data.Length), VkMemoryMapFlags.MinValue, pDeviceData) |> checkResult

    let deviceDataSpan = Span<'T>(deviceData |> NativePtr.ofNativeInt<'T> |> NativePtr.toVoidPtr, data.Length)
    data.CopyTo deviceDataSpan

    vkUnmapMemory(device, memory)

let recordCopyBuffer (commandBuffer: VkCommandBuffer) src dst size =
    let beginInfo =
        VkCommandBufferBeginInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
            flags = VkCommandBufferUsageFlags.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT,
            pInheritanceInfo = vkNullPtr
        )

    vkBeginCommandBuffer(commandBuffer, &&beginInfo) |> checkResult

    let copyRegion =
        VkBufferCopy (
            srcOffset = 0UL, // Optional
            dstOffset = 0UL, // Optional
            size = size
        )

    vkCmdCopyBuffer(commandBuffer, src, dst, 1u, &&copyRegion)

    vkEndCommandBuffer(commandBuffer) |> checkResult

let copyBuffer device commandPool srcBuffer dstBuffer size queue =
    let allocInfo =
        VkCommandBufferAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool = commandPool,
            commandBufferCount = 1u
        )

    let commandBuffer = VkCommandBuffer()
    vkAllocateCommandBuffers(device, &&allocInfo, &&commandBuffer) |> checkResult
    
    recordCopyBuffer commandBuffer srcBuffer dstBuffer size

    let submitInfo =
        VkSubmitInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            commandBufferCount = 1u,
            pCommandBuffers = &&commandBuffer
        )

    vkQueueSubmit(queue, 1u, &&submitInfo, VK_NULL_HANDLE) |> checkResult
    vkQueueWaitIdle(queue) |> checkResult

    vkFreeCommandBuffers(device, commandPool, 1u, &&commandBuffer)

[<Flags>]
type BufferFlags =
    | None = 0b0uy
    | SharedMemory = 0b1uy

let inline hasSharedMemoryFlag flags =
    flags &&& BufferFlags.SharedMemory = BufferFlags.SharedMemory

type BufferKind =
    | Unspecified = 0uy
    | Vertex = 1uy
    | Index = 2uy
    | Uniform = 3uy

[<Struct;NoComparison>]
type Buffer =
    {
        buffer: VkBuffer
        memory: VkDeviceMemory
        flags: BufferFlags
        kind: BufferKind
    }

    member x.IsShared = hasSharedMemoryFlag x.flags

let mkBoundBuffer<'T when 'T : unmanaged> physicalDevice device count flags kind =
    let isShared = hasSharedMemoryFlag flags
    let usage =
        match kind with
        | BufferKind.Vertex -> VkBufferUsageFlags.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT
        | BufferKind.Index -> VkBufferUsageFlags.VK_BUFFER_USAGE_INDEX_BUFFER_BIT
        | BufferKind.Uniform -> VkBufferUsageFlags.VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT
        | _ -> Unchecked.defaultof<_>
    let memProperties = 
         VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
         VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT

    let buffer = mkBuffer<'T> device count (usage ||| VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_DST_BIT)
    if isShared then
        let memory = bindMemory physicalDevice device buffer memProperties
        { buffer = buffer; memory = memory; flags = flags; kind = kind }
    else
        // High-performance GPU memory
        let memory = bindMemory physicalDevice device buffer VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
        { buffer = buffer; memory = memory; flags = flags; kind = kind }

let fillBuffer<'T when 'T : unmanaged> physicalDevice device commandPool transferQueue (buffer: Buffer) data =
    if buffer.IsShared then
        mapBuffer<'T> device buffer.memory data
    else
        // Memory that is not shared can not be written directly to from the CPU.
        // In order to set it from the CPU, a temporary shared memory buffer is used as a staging buffer to transfer the data.
        // This means that write times to local memory is more expensive but is highly-performant when read from the GPU.
        // Effectively, local memory is great for static data and shared memory is great for dynamic data.
        let count = data.Length
        let stagingBuffer = mkBuffer<'T> device count VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_SRC_BIT
        let stagingProperties = 
                VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
                VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT
        let stagingMemory = bindMemory physicalDevice device stagingBuffer stagingProperties

        mapBuffer device stagingMemory data

        let size = uint64 (sizeof<'T> * count)
        copyBuffer device commandPool stagingBuffer buffer.buffer size transferQueue

        vkDestroyBuffer(device, stagingBuffer, vkNullPtr)
        vkFreeMemory(device, stagingMemory, vkNullPtr)

type PipelineIndex = int

[<Struct;NoComparison>]
type FalBuffer<'T when 'T : unmanaged> = { buffer: Buffer } with

    member this.Buffer = this.buffer.buffer

    member this.IsShared = this.buffer.IsShared

[<Sealed>]
type FalGraphics
    private
    (physicalDevice: VkPhysicalDevice,
     device: VkDevice,
     commandPool: VkCommandPool,
     transferQueue: VkQueue,
     swapChain: SwapChain) =

    let gate = obj ()
    let buffers = Collections.Generic.Dictionary<VkBuffer, Buffer> ()
    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "Vulkan instance is disposed."

    let destroyBuffer (buffer: Buffer) =
        vkDestroyBuffer(device, buffer.buffer, vkNullPtr)
        vkFreeMemory(device, buffer.memory, vkNullPtr)

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

        let buffer = mkBoundBuffer<'T> physicalDevice device size flags kind
        buffers.Add (buffer.buffer, buffer)
        { buffer = buffer } : FalBuffer<'T>

    member _.FillBuffer<'T when 'T : unmanaged> (buffer: FalBuffer<'T>, data) =
        checkDispose ()

        fillBuffer<'T> physicalDevice device commandPool transferQueue buffer.buffer data

    member _.DestroyBuffer (buffer: FalBuffer<_>) =
        lock gate <| fun _ ->

        checkDispose ()
        match buffers.Remove buffer.buffer.buffer : bool * Buffer with
        | true, buffer ->
            destroyBuffer buffer
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
