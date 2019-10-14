[<AutoOpen>]
module Falkan.Buffer

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

let fillBuffer<'T when 'T : unmanaged> device memory (data: ReadOnlySpan<'T>) =
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

[<AbstractClass>]
type FalBuffer () =

    abstract Buffer: VkBuffer
    abstract Memory: VkDeviceMemory
    abstract IsShared: bool

[<Sealed>]
type VertexBuffer<'T when 'T : unmanaged> private 
    (
        buffer: VkBuffer, 
        memory: VkDeviceMemory, 
        physicalDevice: VkPhysicalDevice,
        device: VkDevice, 
        commandPool: VkCommandPool,
        transferQueue: VkQueue,
        isShared
    ) =
    inherit FalBuffer ()

    override _.Buffer = buffer
    override _.Memory = memory
    override _.IsShared = isShared

    member _.Fill(data: ReadOnlySpan<'T>) =
        if isShared then
            fillBuffer device memory data
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

            fillBuffer device stagingMemory data

            let size = uint64 (sizeof<'T> * count)
            copyBuffer device commandPool stagingBuffer buffer size transferQueue

            vkDestroyBuffer(device, stagingBuffer, vkNullPtr)
            vkFreeMemory(device, stagingMemory, vkNullPtr)

    static member Create(physicalDevice, device, commandPool, transferQueue, count, usage) =
        let memProperties = 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT

        let buffer = mkBuffer<'T> device count usage
        if usage &&& VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_DST_BIT = VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_DST_BIT then
            // High-performance GPU memory
            let memory = bindMemory physicalDevice device buffer VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
            VertexBuffer<'T>(buffer, memory, physicalDevice, device, commandPool, transferQueue, false)
        else
            let memory = bindMemory physicalDevice device buffer memProperties
            VertexBuffer<'T>(buffer, memory, physicalDevice, device, commandPool, transferQueue, true)