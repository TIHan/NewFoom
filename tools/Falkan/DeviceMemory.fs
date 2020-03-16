[<AutoOpen>]
module internal FsGame.Graphics.Vulkan.DeviceMemory

open System
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let allocateDeviceMemory device memTypeIndex size =
    let mutable allocInfo =
        VkMemoryAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
            allocationSize = uint64 size,
            memoryTypeIndex = memTypeIndex
        )

    let mutable deviceMemory = VkDeviceMemory ()
    vkAllocateMemory(device, &&allocInfo, vkNullPtr, &&deviceMemory) |> checkResult
    deviceMemory

[<Struct;NoEquality;NoComparison>]
type Block =
    {
        Offset: int
        Size: int
        Order: int
    }

let splitBlock block =
    if block.Order <= 0 then
        failwith "Block order is less than or equal to zero."
    let halfOffset = block.Offset / 2
    let halfSize = block.Size / 2
    let newOrder = block.Order - 1
    struct({ Offset = halfOffset; Size = halfSize; Order = newOrder }, { Offset = halfOffset + halfSize; Size = halfSize; Order = newOrder })

type ChunkTree =
    | ChunkBranch of order: int * size: int * offset: int * left: ChunkTree * right: ChunkTree
    | ChunkLeaf of order: int * size: int * offset: int * isFree: bool

    member this.Order =
        match this with
        | ChunkBranch(order=order)
        | ChunkLeaf(order=order) -> order

    member this.Size =
        match this with
        | ChunkBranch(size=size)
        | ChunkLeaf(size=size) -> size

    member this.Offset =
        match this with
        | ChunkBranch(offset=offset)
        | ChunkLeaf(offset=offset) -> offset

    member this.IsLeaf =
        match this with
        | ChunkLeaf(isFree=isFree) -> isFree
        | _ -> false

    member this.SplitLeaf() =
        match this with
        | ChunkLeaf(order, size, offset, isFree) when isFree ->
            let newSize = size / 2
            let left = ChunkLeaf(order + 1, newSize, offset, true)
            let right = ChunkLeaf(order + 1, newSize, offset + newSize, true)
            ChunkBranch(order, size, offset, left, right)
        | _ -> failwith "Cannot split a branch or a non-free leaf."

    member this.Remap(f) =
        match f this with
        | ChunkBranch(order, size, offset, left, right) -> ChunkBranch(order, size, offset, left.Remap f, right.Remap f)
        | leaf -> leaf

[<Sealed>] 
type Chunk (alignment: int, upperLimit: int) =

    let sizes = Array.init (upperLimit + 1) (fun i -> pown 2 i * alignment)
    let free = Array.init (upperLimit + 1) (fun _ -> ResizeArray<int>())

    let getSize order = sizes.[order]
    let getFreeOffsets order = free.[order]

    let tryGetBlock order =
        let offsets = getFreeOffsets order
        if offsets.Count > 0 then
            let offset = offsets.[0]
            { Offset = offset; Size = getSize order; Order = order }
            |> ValueSome
        else
            ValueNone

    let rec tryRequestBlock order =
        match tryGetBlock order with
        | ValueSome block -> ValueSome block
        | _ ->
            match tryRequestBlock (order + 1) with
            | ValueSome block ->
                free.[block.Order].RemoveAt 0
                let struct(halfBlock1, halfBlock2) = splitBlock block
                let offsets = getFreeOffsets halfBlock1.Order
                offsets.Add(halfBlock1.Offset)
                offsets.Add(halfBlock2.Offset)
                offsets.Sort()
                tryRequestBlock order
            | _ ->
                ValueNone

    let rec rebalance order =
        let size = getSize order
        let offsets = getFreeOffsets order
        offsets.Sort()
        for i = 0 to offsets.Count - 2 do
            let j = i + 1
            let offset1 = offsets.[i]
            let offset2 = offsets.[j]
            if (offset2 - offset1) = size then
                let nextOrder = order + 1
                (getFreeOffsets nextOrder).Add offset1
                rebalance nextOrder

    do free.[upperLimit].Add 0

    member _.TryRequest(size: int) =
        let requestOrder =
            sizes
            |> Array.findIndex (fun x -> size <= x)

        match tryRequestBlock requestOrder with
        | ValueSome block ->
            (getFreeOffsets block.Order).RemoveAt 0
            Some block
        | _ ->
            None

    member _.Free(block: Block) =
        let offsets = (getFreeOffsets block.Order)
        offsets.Add block.Offset
        rebalance block.Order
        
let chunks = System.Collections.Concurrent.ConcurrentDictionary<uint32 * VkDeviceSize, Lazy<ResizeArray<Chunk * VkDeviceMemory>>>()
       

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

    let factory = System.Func<_, _> (fun struct(memTypeIndex, device) -> lazy DeviceMemoryBucket.Create(device, memTypeIndex, 256 * 1024 * 1024)) // TODO: 64mb is arbitrary for now just to this working
    let bucket = buckets.GetOrAdd(struct(memTypeIndex, device), factory)

    bucket.Value.Allocate(int memRequirements.size, int memRequirements.alignment)

// 64mb, artbitrary
let DefaultChunkSize = 256 * 1024 * 1024

let gate = obj ()

[<Struct;NoEquality;NoComparison>]
type VulkanMemory =
    {
        DeviceMemory: VkDeviceMemory
        Block: Block
        Chunk: Chunk
        IsFree: bool ref
    }

    member this.Free() =
        let isFree = this.IsFree
        let chunk = this.Chunk
        let block = this.Block
        lock gate <| fun _ ->
            if not !isFree then
                isFree := true
                chunk.Free block

    interface IDisposable with

        member this.Dispose() =
            this.Free()

    static member Allocate physicalDevice device (memRequirements: VkMemoryRequirements) properties =
        lock gate <| fun _ ->
            let memTypeIndex = 
                getSuitableMemoryTypeIndex 
                    physicalDevice memRequirements.memoryTypeBits 
                    properties

            let factory = System.Func<_, _> (fun _ -> lazy ResizeArray())
            let chunks = chunks.GetOrAdd((memTypeIndex, memRequirements.alignment), factory).Value

            let requestSize = int memRequirements.size

            let blockOpt =
                chunks
                |> Seq.tryPick (fun (chunk, deviceMemory) -> 
                    match chunk.TryRequest (int memRequirements.size) with
                    | Some block -> Some (block, chunk, deviceMemory)
                    | _ -> None)

            match blockOpt with
            | Some (block, chunk, deviceMemory) -> { DeviceMemory = deviceMemory; Block = block; Chunk = chunk; IsFree = ref false }
            | _ ->
                let alignment = int memRequirements.alignment
                let size = requestSize
                let upperLimit = 20

                let chunk = Chunk(alignment, upperLimit)
                let block = (chunk.TryRequest size).Value
                let deviceMemory = allocateDeviceMemory device memTypeIndex (pown 2 20 * (int alignment))

                chunks.Add(chunk, deviceMemory)

                { DeviceMemory = deviceMemory; Block = block; Chunk = chunk; IsFree = ref false }
