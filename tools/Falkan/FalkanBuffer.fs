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

let internal bindMemory physicalDevice device buffer properties =
    let memRequirements = getMemoryRequirements device buffer
    let memory = allocateMemory physicalDevice device memRequirements properties
    vkBindBufferMemory(device, buffer, memory.Bucket.VkDeviceMemory, uint64 memory.Offset) |> checkResult
    memory

let mapMemory<'T when 'T : unmanaged> device memory offset (data: ReadOnlySpan<'T>) =
    let mutable deviceData = nativeint 0
    let pDeviceData = &&deviceData |> NativePtr.toNativeInt

    vkMapMemory(device, memory, uint64 offset, uint64 (sizeof<'T> * data.Length), VkMemoryMapFlags.MinValue, pDeviceData) |> checkResult

    let deviceDataSpan = Span<'T>(deviceData |> NativePtr.ofNativeInt<'T> |> NativePtr.toVoidPtr, data.Length)
    data.CopyTo deviceDataSpan

    vkUnmapMemory(device, memory)

let recordCopyBuffer (commandBuffer: VkCommandBuffer) src dst size =
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
            dstOffset = 0UL, // Optional
            size = size
        )

    vkCmdCopyBuffer(commandBuffer, src, dst, 1u, &&copyRegion)

    vkEndCommandBuffer(commandBuffer) |> checkResult

let copyBuffer device commandPool srcBuffer dstBuffer size queue =
    let mutable allocInfo =
        VkCommandBufferAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool = commandPool,
            commandBufferCount = 1u
        )

    let mutable commandBuffer = VkCommandBuffer()
    vkAllocateCommandBuffers(device, &&allocInfo, &&commandBuffer) |> checkResult
    
    recordCopyBuffer commandBuffer srcBuffer dstBuffer size

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

[<Flags>]
type FalkanBufferFlags =
    | None = 0b0uy
    | SharedMemory = 0b1uy

let inline hasSharedMemoryFlag flags =
    flags &&& FalkanBufferFlags.SharedMemory = FalkanBufferFlags.SharedMemory

type FalkanBufferKind =
    | UnspecifiedBuffer
    | VertexBuffer
    | IndexBuffer
    | UniformBuffer

[<Struct;NoComparison>]
type FalkanBuffer =
    internal {
        device: VulkanDevice
        buffer: VkBuffer
        memory: DeviceMemory
        flags: FalkanBufferFlags
        kind: FalkanBufferKind
    }

    member x.IsShared = hasSharedMemoryFlag x.flags

    member internal x.Destroy() =
        vkDestroyBuffer(x.device.Device, x.buffer, vkNullPtr)
        (x.memory :> IDisposable).Dispose()

type VulkanDevice with

    member this.CreateBuffer(kind, flags, size) =
        let physicalDevice = this.PhysicalDevice
        let device = this.Device

        let isShared = hasSharedMemoryFlag flags
        let usage =
            match kind with
            | VertexBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT
            | IndexBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_INDEX_BUFFER_BIT
            | UniformBuffer -> VkBufferUsageFlags.VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT
            | _ -> Unchecked.defaultof<_>
        let memProperties = 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT

        let buffer = mkBuffer device size (usage ||| VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_DST_BIT)
        if isShared then
            let memory = bindMemory physicalDevice device buffer memProperties
            { device = this; buffer = buffer; memory = memory; flags = flags; kind = kind }
        else
            // High-performance GPU memory
            let memory = bindMemory physicalDevice device buffer VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
            { device = this; buffer = buffer; memory = memory; flags = flags; kind = kind }


type FalkanBuffer with

    member buffer.SetData<'T when 'T : unmanaged>(data) =
        let physicalDevice = buffer.device.PhysicalDevice
        let device = buffer.memory.Bucket.VkDevice
        let commandPool = buffer.device.VkCommandPool
        let transferQueue = buffer.device.VkTransferQueue

        if buffer.IsShared then
            mapMemory<'T> device buffer.memory.Bucket.VkDeviceMemory buffer.memory.Offset data
        else
            // Memory that is not shared can not be written directly to from the CPU.
            // In order to set it from the CPU, a temporary shared memory buffer is used as a staging buffer to transfer the data.
            // This means that write times to local memory is more expensive but is highly-performant when read from the GPU.
            // Effectively, local memory is great for static data and shared memory is great for dynamic data.
            let count = data.Length
            let stagingBuffer = mkBuffer device (sizeof<'T> * count) VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_SRC_BIT
            let stagingProperties = 
                    VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
                    VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT
            use stagingMemory = bindMemory physicalDevice device stagingBuffer stagingProperties
        
            mapMemory device stagingMemory.Bucket.VkDeviceMemory stagingMemory.Offset data
        
            let size = uint64 (sizeof<'T> * count)
            copyBuffer device commandPool stagingBuffer buffer.buffer size transferQueue

            vkDestroyBuffer(device, stagingBuffer, vkNullPtr)