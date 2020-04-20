[<AutoOpen>]
module FsGame.Graphics.Vulkan.FalkanBuffer

open System
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let mkBuffer device size usage =
    let mutable bufferInfo =
        VkBufferCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO,
            size = uint64 size,
            usage = usage,
            sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE
        )
    
    let mutable vertexBuffer = VkBuffer ()
    vkCreateBuffer(device, &&bufferInfo, vkNullPtr, &&vertexBuffer) |> checkResult
    vertexBuffer

let getMemoryRequirements device buffer =
    let mutable memRequirements = VkMemoryRequirements ()
    vkGetBufferMemoryRequirements(device, buffer, &&memRequirements)
    memRequirements

let internal bindMemory (vulkanDevice: VulkanDevice) buffer properties =
    let memRequirements = getMemoryRequirements vulkanDevice.Device buffer
    let memory = VulkanMemory.Allocate vulkanDevice memRequirements properties
    vkBindBufferMemory(vulkanDevice.Device, buffer, memory.NativeDeviceMemory, uint64 memory.Block.Offset) |> checkResult
    memory

let recordCopyBuffer (commandBuffer: VkCommandBuffer) src dst dstOffset size =
    let mutable beginInfo =
        VkCommandBufferBeginInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
            flags = VkCommandBufferUsageFlags.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT,
            pInheritanceInfo = vkNullPtr
        )

    vkBeginCommandBuffer(commandBuffer, &&beginInfo) |> checkResult

    let mutable copyRegion =
        VkBufferCopy (
            srcOffset = 0UL, // Optional
            dstOffset = dstOffset, // Optional
            size = size
        )

    vkCmdCopyBuffer(commandBuffer, src, dst, 1u, &&copyRegion)

    vkEndCommandBuffer(commandBuffer) |> checkResult

let copyBuffer device commandPool srcBuffer dstBuffer dstOffset size queue =
    let mutable allocInfo =
        VkCommandBufferAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool = commandPool,
            commandBufferCount = 1u
        )

    let mutable commandBuffer = VkCommandBuffer()
    vkAllocateCommandBuffers(device, &&allocInfo, &&commandBuffer) |> checkResult
    
    recordCopyBuffer commandBuffer srcBuffer dstBuffer dstOffset size

    let mutable submitInfo =
        VkSubmitInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            commandBufferCount = 1u,
            pCommandBuffers = &&commandBuffer
        )

    vkQueueSubmit(queue, 1u, &&submitInfo, VK_NULL_HANDLE) |> checkResult
    vkQueueWaitIdle(queue) |> checkResult

    vkFreeCommandBuffers(device, commandPool, 1u, &&commandBuffer)

//let fillBuffer<'T when 'T : unmanaged> physicalDevice device commandPool transferQueue (buffer: FalkanBuffer) data =
//    if buffer.IsShared then
//        mapMemory<'T> device buffer.memory.raw buffer.memory.offset data
//    else
//        // Memory that is not shared can not be written directly to from the CPU.
//        // In order to set it from the CPU, a temporary shared memory buffer is used as a staging buffer to transfer the data.
//        // This means that write times to local memory is more expensive but is highly-performant when read from the GPU.
//        // Effectively, local memory is great for static data and shared memory is great for dynamic data.
//        let count = data.Length
//        let stagingBuffer = mkBuffer<'T> device count VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_SRC_BIT
//        let stagingProperties = 
//                VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
//                VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT
//        let stagingMemory = bindMemory physicalDevice device stagingBuffer stagingProperties

//        mapMemory device stagingMemory.raw stagingMemory.offset data

//        let size = uint64 (sizeof<'T> * count)
//        copyBuffer device commandPool stagingBuffer buffer.buffer size transferQueue

//        vkDestroyBuffer(device, stagingBuffer, vkNullPtr)

// TODO: Change this to VulkanUsage
[<Flags>]
type VulkanBufferFlags =
    | None =           0b0uy
    | SharedMemory =   0b1uy

let inline hasSharedMemoryFlag flags =
    flags &&& VulkanBufferFlags.SharedMemory = VulkanBufferFlags.SharedMemory

type VulkanBufferKind =
    | UnspecifiedBuffer
    | VertexBuffer
    | IndexBuffer
    | UniformBuffer
    | StorageBuffer

[<Struct;NoComparison>]
type VulkanBuffer<'T when 'T : unmanaged> =
    internal {
        buffer: VkBuffer
        memory: VulkanMemory
        flags: VulkanBufferFlags
        kind: VulkanBufferKind
        length: int
    }

    member x.IsShared = hasSharedMemoryFlag x.flags

    member x.Memory = x.memory

    member x.Length = x.length

type VulkanDevice with

    [<RequiresExplicitTypeArguments>]
    member this.CreateBuffer<'T when 'T : unmanaged>(kind, flags, size) : VulkanBuffer<'T> =
        let device = this.Device

        let length = size / sizeof<'T>
        let isShared = hasSharedMemoryFlag flags
        let usage =
            match kind with
            | VertexBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT
            | IndexBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_INDEX_BUFFER_BIT
            | UniformBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT
            | StorageBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_STORAGE_BUFFER_BIT
            | _ -> Unchecked.defaultof<_>
        let memProperties = 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT

        let buffer = mkBuffer device size (usage ||| VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_DST_BIT)
        if isShared then
            let memory = bindMemory this buffer memProperties
            { buffer = buffer; memory = memory; flags = flags; kind = kind; length = length }
        else
            // High-performance GPU memory
            let memory = bindMemory this buffer VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
            { buffer = buffer; memory = memory; flags = flags; kind = kind; length = length }

type VulkanBuffer<'T when 'T : unmanaged> with
    
    member buffer.Upload(offset, data: ReadOnlySpan<'T>) =
        let device = buffer.memory.Device.Device
        let commandPool = buffer.memory.Device.VkCommandPool
        let transferQueue = buffer.memory.Device.VkTransferQueue

        let count = data.Length

        if buffer.IsShared then
            let span = buffer.memory.MapAsSpan<'T>(offset, count)
            data.CopyTo span
            buffer.memory.Unmap()
        else
            // Memory that is not shared can not be written directly to from the CPU.
            // In order to set it from the CPU, a temporary shared memory buffer is used as a staging buffer to transfer the data.
            // This means that write times to local memory is more expensive but is highly-performant when read from the GPU.
            // Effectively, local memory is great for static data and shared memory is great for dynamic data.
            let size = sizeof<'T> * count
            let stagingBuffer = mkBuffer device size VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_SRC_BIT
            let stagingProperties = 
                    VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
                    VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT
            use stagingMemory = bindMemory buffer.memory.Device stagingBuffer stagingProperties
        
            let span = stagingMemory.MapAsSpan<'T>(count)
            data.CopyTo span
            stagingMemory.Unmap()
        
            copyBuffer device commandPool stagingBuffer buffer.buffer (uint64 (sizeof<'T> * offset)) (uint64 size) transferQueue

            vkDestroyBuffer(device, stagingBuffer, vkNullPtr)

    member buffer.Upload(data: ReadOnlySpan<'T>) =
        buffer.Upload(0, data)

    member buffer.Upload(offset, data: 'T[]) =
        buffer.Upload(offset, ReadOnlySpan data)

    member buffer.Upload(data: 'T[]) =
        buffer.Upload(ReadOnlySpan data)

    member buffer.Upload(data: inref<'T>) =
        let stack = NativePtr.stackalloc 1
        NativePtr.set stack 0 data
        buffer.Upload(ReadOnlySpan<'T>(NativePtr.toVoidPtr stack, 1))

    member inline buffer.Upload(data: 'T) =
        buffer.Upload(&data)
