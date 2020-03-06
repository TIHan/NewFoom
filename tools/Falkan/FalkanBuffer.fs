[<AutoOpen>]
module Falkan.FalkanBuffer

open System
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

[<Struct;NoEquality;NoComparison>]
type DeviceMemory =
    {
        raw: VkDeviceMemory
        offset: int
        size: int
    }

[<Sealed>]
type DeviceMemoryBucket private (device: VkDevice, raw: VkDeviceMemory, bucketSize: int) =

    let gate = obj ()
    let blocks = ResizeArray<struct(DeviceMemory * bool)> 100 // TODO: We could do better here. Could allocate on the LOH if it gets big enough.

    let mutable freeBlocksCache = ValueNone
    let getFreeBlocks () =
        match freeBlocksCache with
        | ValueSome blocks -> blocks
        | _ ->
            let mutable index = 0
            let blocks =
                blocks 
                |> Seq.choose (fun struct(block, isFree) -> 
                    if isFree then
                        let i = index
                        index <- index + 1
                        Some struct(block, i)
                    else
                        None)
                |> Seq.cache
            freeBlocksCache <- ValueSome blocks
            blocks

    let mutable maxFreeFragmentSizeCache = ValueNone
    let getMaxFreeFragmentSize () =
        match maxFreeFragmentSizeCache with
        | ValueSome size -> size
        | _ ->
            let blocks = getFreeBlocks ()
            if Seq.isEmpty blocks then 0
            else
                let struct(block, _) = (blocks |> Seq.maxBy (fun struct(block, _) -> block.size))
                let size = block.size
                maxFreeFragmentSizeCache <- ValueSome size
                size

    let clearCache () =
        freeBlocksCache <- ValueNone
        maxFreeFragmentSizeCache <- ValueNone

    let isBlockFree index =
        let struct(_, isFree) = blocks.[index]
        isFree

    let getLastBlock () =
        let struct(lastBlock, _) = blocks.[blocks.Count - 1]
        lastBlock

    let getFreeSize () =
        if Seq.isEmpty blocks then
            bucketSize
        else
            let lastBlock = getLastBlock ()
            bucketSize - lastBlock.offset + lastBlock.size

    let getFreeFragmentBlock size =
        getFreeBlocks ()
        |> Seq.filter (fun struct(block, _) -> block.size >= size)
        |> Seq.minBy (fun struct(block, _) -> block.size)

    let allocateBlock size =
        let block =
            if Seq.isEmpty blocks then
                let block =
                    {
                        raw = raw
                        offset = 0
                        size = size
                    }
                blocks.Add struct(block, false)
                block
            elif getFreeSize () >= size then
                let lastBlock = getLastBlock ()
                let block =
                    {
                        raw = raw
                        offset = lastBlock.offset + lastBlock.size
                        size = size
                    }
                blocks.Add struct(block, false)
                block
            elif getMaxFreeFragmentSize () >= size then
                let struct(block, index) = getFreeFragmentBlock size
                let newBlock =
                    {
                        raw = raw
                        offset = block.offset
                        size = size
                    }
                blocks.[index] <- struct(newBlock, false)

                let remainingSize = block.size - size
                let remainingIndex = index + 1
                if remainingSize > 0 && remainingIndex < blocks.Count then
                    let remainingBlock =
                        {
                            raw = raw
                            offset = newBlock.offset + newBlock.size
                            size = remainingSize
                        }
                    blocks.Insert(remainingIndex, struct(remainingBlock, true))
                newBlock
            else
                failwith "Invalid allocation on GPU memory block."

        clearCache ()
        block

    let freeBlock block =
        let index = blocks |> Seq.findIndex (fun struct(x, isFree) -> not isFree && x.offset = block.offset && x.size = block.size)
        blocks.[index] <- struct(block, true)

        let possibleNextBlockIndexToMerge = index + 1
        let possiblePrevBlockIndexToMerge = index - 1
        if possibleNextBlockIndexToMerge < blocks.Count && isBlockFree possibleNextBlockIndexToMerge then
            let struct(blockToMerge, _) = blocks.[possibleNextBlockIndexToMerge]
            blocks.[index] <- struct({ block with size = block.size + blockToMerge.size }, true)
            blocks.RemoveAt possibleNextBlockIndexToMerge
        elif possiblePrevBlockIndexToMerge < blocks.Count && isBlockFree possiblePrevBlockIndexToMerge then
            let struct(blockToMerge, _) = blocks.[possiblePrevBlockIndexToMerge]
            blocks.[index] <- struct({ block with offset = blockToMerge.offset; size = block.size + blockToMerge.size }, true)
            blocks.RemoveAt possiblePrevBlockIndexToMerge

        clearCache ()

    member _.MaxAvailableFreeSize =
        // TODO: Add cache that doesn't lock
        lock gate <| fun _ ->
            let freeSize = getFreeSize ()
            let freeFragmentSize = getMaxFreeFragmentSize ()
            if freeSize > freeFragmentSize then freeSize
            else freeFragmentSize

    member _.Allocate size =
        lock gate <| fun _ ->
            if size > getFreeSize () && size > getMaxFreeFragmentSize () then
                failwith "Not enough available GPU memory in bucket."

            allocateBlock size

    member _.Free block =
        lock gate <| fun _ ->
            if raw <> block.raw then
                failwith "Invalid GPU memory to free in bucket."

            freeBlock

    interface IDisposable with

        member _.Dispose() =
            vkFreeMemory(device, raw, vkNullPtr)

    static member Create(device, memTypeIndex, bucketSize) =
        let mutable allocInfo =
            VkMemoryAllocateInfo (
                sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
                allocationSize = uint64 bucketSize,
                memoryTypeIndex = memTypeIndex
            )

        let mutable raw = VkDeviceMemory ()
        vkAllocateMemory(device, &&allocInfo, vkNullPtr, &&raw) |> checkResult
        new DeviceMemoryBucket(device, raw, bucketSize)

let getMemoryRequirements device buffer =
    let mutable memRequirements = VkMemoryRequirements ()
    vkGetBufferMemoryRequirements(device, buffer, &&memRequirements)
    memRequirements

let getSuitableMemoryTypeIndex physicalDevice typeFilter properties =
    let mutable memProperties = VkPhysicalDeviceMemoryProperties ()
    vkGetPhysicalDeviceMemoryProperties(physicalDevice, &&memProperties)

    [| for i = 0 to int memProperties.memoryTypeCount - 1 do
        if typeFilter &&& (1u <<< i) <> 0u then
            let memType = memProperties.memoryTypes.[i]
            if memType.propertyFlags &&& properties = properties then
                yield uint32 i |]
    |> Array.head

// TODO: Let's not make this a global.
let buckets = System.Collections.Concurrent.ConcurrentDictionary<struct(uint32 * VkDevice), Lazy<DeviceMemoryBucket>>()

let allocateMemory physicalDevice device (memRequirements: VkMemoryRequirements) properties =
    let memTypeIndex = 
        getSuitableMemoryTypeIndex 
            physicalDevice memRequirements.memoryTypeBits 
            properties

    let factory = System.Func<_, _> (fun struct(memTypeIndex, device) -> lazy DeviceMemoryBucket.Create(device, memTypeIndex, 64 * 1024 * 1024)) // TODO: 64mb is arbitrary for now just to this working
    let bucket = buckets.GetOrAdd(struct(memTypeIndex, device), factory)

    bucket.Value.Allocate(int memRequirements.size)

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

let bindMemory physicalDevice device buffer properties =
    let memRequirements = getMemoryRequirements device buffer
    let memory = allocateMemory physicalDevice device memRequirements properties
    vkBindBufferMemory(device, buffer, memory.raw, uint64 memory.offset) |> checkResult
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
    {
        vkPhysicalDevice: VkPhysicalDevice
        vkDevice: VkDevice
        buffer: VkBuffer
        memory: DeviceMemory
        flags: FalkanBufferFlags
        kind: FalkanBufferKind
    }

    member x.IsShared = hasSharedMemoryFlag x.flags

    member internal x.Destroy() =
        vkDestroyBuffer(x.vkDevice, x.buffer, vkNullPtr)

type FalDevice with

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
            { vkPhysicalDevice = physicalDevice; vkDevice = device; buffer = buffer; memory = memory; flags = flags; kind = kind }
        else
            // High-performance GPU memory
            let memory = bindMemory physicalDevice device buffer VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
            { vkPhysicalDevice = physicalDevice; vkDevice = device; buffer = buffer; memory = memory; flags = flags; kind = kind }


type FalkanBuffer with

    member buffer.SetData<'T when 'T : unmanaged>(data, commandPool, transferQueue) =
        let physicalDevice = buffer.vkPhysicalDevice
        let device = buffer.vkDevice

        if buffer.IsShared then
            mapMemory<'T> device buffer.memory.raw buffer.memory.offset data
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
            let stagingMemory = bindMemory physicalDevice device stagingBuffer stagingProperties
        
            mapMemory device stagingMemory.raw stagingMemory.offset data
        
            let size = uint64 (sizeof<'T> * count)
            copyBuffer device commandPool stagingBuffer buffer.buffer size transferQueue
        
            vkDestroyBuffer(device, stagingBuffer, vkNullPtr)