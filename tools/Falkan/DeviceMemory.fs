[<AutoOpen>]
module internal Falkan.DeviceMemory

open System
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

[<Struct;NoEquality;NoComparison>]
type DeviceMemory(bucket: DeviceMemoryBucket, offset: int, size: int) =

    member _.Bucket = bucket

    member _.Offset = offset

    member _.Size = size

    interface IDisposable with

        member this.Dispose() =
            bucket.Free this

and [<Sealed>] DeviceMemoryBucket private (device: VkDevice, raw: VkDeviceMemory, bucketSize: int) as this =

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
                let struct(block, _) = (blocks |> Seq.maxBy (fun struct(block, _) -> block.Size))
                let size = block.Size
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
            bucketSize - lastBlock.Offset + lastBlock.Size

    let getFreeFragmentBlock size =
        getFreeBlocks ()
        |> Seq.filter (fun struct(block, _) -> block.Size >= size)
        |> Seq.minBy (fun struct(block, _) -> block.Size)

    let calculateOffset (possibleOffset: int) (alignment: int) =
        if possibleOffset % alignment = 0 then
            possibleOffset
        else
            ((possibleOffset / alignment) + 1) * alignment

    let allocateBlock size alignment =
        let block =
            if Seq.isEmpty blocks then
                let block = new DeviceMemory(this, 0, size)
                blocks.Add struct(block, false)
                block
            elif getFreeSize () >= size then
                let lastBlock = getLastBlock ()
                let offset = calculateOffset (lastBlock.Offset + lastBlock.Size) alignment
                let block = new DeviceMemory(this, offset, size)
                blocks.Add struct(block, false)
                block
            elif getMaxFreeFragmentSize () >= size then
                let struct(block, index) = getFreeFragmentBlock size
                let newBlock = new DeviceMemory(this, block.Offset, size)
                blocks.[index] <- struct(newBlock, false)

                let remainingSize = block.Size - size
                let remainingIndex = index + 1
                if remainingSize > 0 && remainingIndex < blocks.Count then
                    let offset = calculateOffset (newBlock.Offset + newBlock.Size) alignment
                    let remainingBlock = new DeviceMemory(this, offset, remainingSize)
                    blocks.Insert(remainingIndex, struct(remainingBlock, true))
                newBlock
            else
                failwith "Invalid allocation on GPU memory block."

        clearCache ()
        block

    let freeBlock (block: DeviceMemory) =
        let index = blocks |> Seq.findIndex (fun struct(x, isFree) -> not isFree && x.Offset = block.Offset && x.Size = block.Size)
        blocks.[index] <- struct(block, true)

        let possibleNextBlockIndexToMerge = index + 1
        let possiblePrevBlockIndexToMerge = index - 1
        if possibleNextBlockIndexToMerge < blocks.Count && isBlockFree possibleNextBlockIndexToMerge then
            let struct(blockToMerge, _) = blocks.[possibleNextBlockIndexToMerge]
            blocks.[index] <- struct(new DeviceMemory(block.Bucket, block.Offset, block.Size + blockToMerge.Size), true)
            blocks.RemoveAt possibleNextBlockIndexToMerge
        elif possiblePrevBlockIndexToMerge < blocks.Count && possiblePrevBlockIndexToMerge >= 0 && isBlockFree possiblePrevBlockIndexToMerge then
            let struct(blockToMerge, _) = blocks.[possiblePrevBlockIndexToMerge]
            blocks.[index] <- struct(new DeviceMemory(block.Bucket, blockToMerge.Offset, block.Size + blockToMerge.Size), true)
            blocks.RemoveAt possiblePrevBlockIndexToMerge

        clearCache ()

    member _.VkDevice = device

    member _.VkDeviceMemory = raw

    member _.MaxAvailableFreeSize =
        // TODO: Add cache that doesn't lock
        lock gate <| fun _ ->
            let freeSize = getFreeSize ()
            let freeFragmentSize = getMaxFreeFragmentSize ()
            if freeSize > freeFragmentSize then freeSize
            else freeFragmentSize

    member _.Allocate(size, alignment) =
        lock gate <| fun _ ->
            if size > getFreeSize () && size > getMaxFreeFragmentSize () then
                failwith "Not enough available GPU memory in bucket."

            allocateBlock size alignment

    member _.Free block =
        lock gate <| fun _ ->
            if raw <> block.Bucket.VkDeviceMemory then
                failwith "Invalid GPU memory to free in bucket."

            freeBlock block

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

let buckets = System.Collections.Concurrent.ConcurrentDictionary<struct(uint32 * VkDevice), Lazy<DeviceMemoryBucket>>()

let allocateMemory physicalDevice device (memRequirements: VkMemoryRequirements) properties =
    let memTypeIndex = 
        getSuitableMemoryTypeIndex 
            physicalDevice memRequirements.memoryTypeBits 
            properties

    let factory = System.Func<_, _> (fun struct(memTypeIndex, device) -> lazy DeviceMemoryBucket.Create(device, memTypeIndex, 64 * 1024 * 1024)) // TODO: 64mb is arbitrary for now just to this working
    let bucket = buckets.GetOrAdd(struct(memTypeIndex, device), factory)

    bucket.Value.Allocate(int memRequirements.size, int memRequirements.alignment)