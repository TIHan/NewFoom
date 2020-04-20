[<AutoOpen>]
module FsGame.Graphics.Vulkan.VulkanDevice

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.Vulkan.Interop
open InternalDeviceHelpers
open Microsoft.FSharp.NativeInterop

#nowarn "9"
#nowarn "51"

// ===============================================================

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
    let halfSize = block.Size / 2
    let newOrder = block.Order - 1

    struct({ Offset = block.Offset; Size = halfSize; Order = newOrder }, { Offset = block.Offset + halfSize; Size = halfSize; Order = newOrder })

let mergeBlocks block1 block2 =
    if block1.Order < 0 || block2.Order < 0 then
        failwith "Block order is less than zero."
    if block1.Order <> block2.Order then
        failwith "Merging two blocks requires they have the same order."
    if block1.Size <> block2.Size then
        failwith "Merging two blocks requires they have the same size."
    if (block1.Offset + block1.Size) <> block2.Offset && (block2.Offset + block2.Size) <> block1.Offset then
        failwith "Merging two blocks requires they be blocks right next to each other by offset and size."

    { Offset = (if block1.Offset < block2.Offset then block1.Offset else block2.Offset); Size = block1.Size + block2.Size; Order = block1.Order + 1 }

type ChunkTree =
    | ChunkBranch of block: Block * left: ChunkTree * right: ChunkTree
    | ChunkLeaf of block: Block * isFree: bool

    member this.Block =
        match this with
        | ChunkBranch(block=block)
        | ChunkLeaf(block=block) -> block

    member this.IsFree =
        match this with
        | ChunkLeaf(isFree=isFree) -> isFree
        | _ -> false

    member this.SplitLeaf() =
        match this with
        | ChunkLeaf(block, isFree) when isFree ->
            let struct(halfBlock1, halfBlock2) = splitBlock block
            let left = ChunkLeaf(halfBlock1, true)
            let right = ChunkLeaf(halfBlock2, true)
            ChunkBranch(block, left, right)
        | _ -> failwith "Cannot split a branch or a non-free leaf."

    member this.Remap(cond, f) : ChunkTree =
        if cond this then
            match f this with
            | ChunkBranch(block, left, right) -> 
                let left = left.Remap(cond, f)
                let right = right.Remap(cond, f)
                if left.IsFree && right.IsFree then
                    ChunkLeaf(mergeBlocks left.Block right.Block, true)
                else
                    ChunkBranch(block, left, right)
            | leaf -> leaf
        else
            this

    member this.Allocate(size) =
        let mutable allocated = None
        let cond = fun (x: ChunkTree) -> x.Block.Size >= size && allocated.IsNone
        let rec f =
            fun x ->
                match x with
                | ChunkLeaf(block, isFree) when isFree && block.Size >= size -> 
                    if block.Order > 0 && block.Size / 2 >= size then
                        x.SplitLeaf()
                    else
                        allocated <- Some { block with Size = size }
                        ChunkLeaf(block, false)
                | _ -> x
        let tree = this.Remap (cond, f)
        allocated, tree

    member this.Free(targetBlock: Block) =
        let mutable didFree = false
        let tree =
            this.Remap ((fun x -> targetBlock.Order <= x.Block.Order && targetBlock.Size <= x.Block.Size && not didFree), fun x ->
                match x with
                | ChunkLeaf(block, isFree) when not isFree && block.Offset = targetBlock.Offset && block.Order = targetBlock.Order -> 
                    didFree <- true
                    ChunkLeaf(block, true)
                | _ -> x)
        didFree, tree

[<Sealed>] 
type Chunk (alignment: int, upperLimit: int) =

    let gate = obj ()
    let size = pown 2 upperLimit * alignment
    let mutable tree = ChunkLeaf({ Offset = 0; Size = size; Order = upperLimit }, true)

    member _.TryRequest(size: int) =
        lock gate <| fun _ -> 
            let allocated, newTree = tree.Allocate size
            tree <- newTree
            allocated

    member _.Free(block: Block) =
        lock gate <| fun _ -> 
            let didFree, newTree = tree.Free block
            tree <- newTree
            didFree
        
let chunks = System.Collections.Concurrent.ConcurrentDictionary<uint32 * VkDeviceSize, Lazy<ResizeArray<Chunk * VkDeviceMemory>>>()

let getSuitableMemoryTypeIndex physicalDevice typeFilter properties =
    let mutable memProperties = VkPhysicalDeviceMemoryProperties ()
    vkGetPhysicalDeviceMemoryProperties(physicalDevice, &&memProperties)

    [| for i = 0 to int memProperties.memoryTypeCount - 1 do
        if typeFilter &&& (1u <<< i) <> 0u then
            let memType = memProperties.memoryTypes.[i]
            if memType.propertyFlags &&& properties = properties then
                yield uint32 i |]
    |> Array.head

let mapMemory device deviceMemory offset size =
    let mutable deviceData = nativeint 0
    let data = &&deviceData |> NativePtr.toNativeInt

    vkMapMemory(device, deviceMemory, uint64 offset, uint64 size, VkMemoryMapFlags.MinValue, data) |> checkResult

    deviceData

let unmapMemory device deviceMemory =
    vkUnmapMemory(device, deviceMemory)

// ===============================================================

type VulkanDeviceLayer =
    | LunarGStandardValidation

    override this.ToString() =
        match this with
        | LunarGStandardValidation -> "VK_LAYER_LUNARG_standard_validation"

type VulkanDeviceExtension =
    | ShaderFloat16Int8

    override this.ToString() =
        match this with
        | ShaderFloat16Int8 -> VK_KHR_SHADER_FLOAT16_INT8_EXTENSION_NAME

[<Sealed>]
type VulkanDevice private 
    (
        instance: VkInstance,
        surfaceOpt: VkSurfaceKHR option,
        debugMessenger: VkDebugUtilsMessengerEXT,
        physicalDevice: VkPhysicalDevice,
        indices: QueueFamilyIndices,
        device: VkDevice,
        vkCommandPool: VkCommandPool,
        vkTransferQueue: VkQueue,
        handles: GCHandle []
    ) =

    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "FalDevice is disposed."

    member _.PhysicalDevice =
        checkDispose ()
        physicalDevice

    member _.Device =
        checkDispose ()
        device

    member _.Indices =
        checkDispose ()
        indices

    member _.Surface =
        checkDispose ()
        surfaceOpt

    member _.VkCommandPool =
        checkDispose ()
        vkCommandPool

    member _.VkTransferQueue =
        checkDispose ()
        vkTransferQueue

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                checkDispose ()
            else
                GC.SuppressFinalize x

                chunks.Values
                |> Seq.iter (fun chunks ->
                    if chunks.IsValueCreated then
                        chunks.Value
                        |> Seq.iter (fun (_, deviceMemory) -> vkFreeMemory(device, deviceMemory, vkNullPtr)))

                chunks.Clear()

                vkDestroyCommandPool(device, vkCommandPool, vkNullPtr)
                vkDestroyDevice(device, vkNullPtr)

                if debugMessenger <> IntPtr.Zero then
                    let destroyDebugUtilsMessenger = getInstanceExtension<vkDestroyDebugUtilsMessengerEXT> instance
                    destroyDebugUtilsMessenger.Invoke(instance, debugMessenger, vkNullPtr)

                surfaceOpt |> Option.iter (fun surface -> vkDestroySurfaceKHR(instance, surface, vkNullPtr))
                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

    static member Create (appName, engineName, deviceLayers: VulkanDeviceLayer list, deviceExtensions: VulkanDeviceExtension list, debugCallback, ?createVkSurface) =
        if sizeof<nativeint> <> 8 then
            failwith "This specific Vulkan API only runs on 64-bit."

        let deviceLayers = deviceLayers |> List.map (fun x -> x.ToString()) |> Array.ofList
        let deviceExtensions = (VK_KHR_SWAPCHAIN_EXTENSION_NAME :: (deviceExtensions |> List.map (fun x -> x.ToString()))) |> Array.ofList
        let debugCallbackHandle, debugCallback =
            createVkDebugUtilsMessengerCallback debugCallback

        let instance = mkInstance appName engineName deviceLayers
        // must create surface right after instance - influences device calls
        let surfaceOpt = match createVkSurface with Some mkSurface -> Some(mkSurface instance) | _ -> None
        let debugMessenger = mkDebugMessenger instance debugCallback
        let physicalDevice = getSuitablePhysicalDevice instance
        let indices = getPhysicalDeviceQueueFamilies physicalDevice surfaceOpt
        let device = mkLogicalDevice physicalDevice indices deviceLayers deviceExtensions

        let _transferFamily =
            match indices.transferFamily with
            | Some transferQueueFamily -> transferQueueFamily
            | _ -> failwith "Unable to create VulkanDevice: Transfer queue not available on physical device."

        let family =
            if createVkSurface.IsSome then
                match indices.graphicsFamily with
                | Some graphicsQueueFamily -> graphicsQueueFamily
                | _ -> failwith "Unable to create VulkanDevice: Graphics queue not available on physical device."
            else
                match indices.computeFamily with
                | Some computeQueueFamily -> computeQueueFamily
                | _ -> failwith "Unable to create VulkanDevice: Compute queue not available on physical device."

        // TODO: We should try to use a transfer queue instead of a graphics queue. This works for now.
        let transferQueue = mkQueue device family

        let commandPool = mkCommandPool device family

        new VulkanDevice (instance, surfaceOpt, debugMessenger, physicalDevice, indices, device, commandPool, transferQueue, [|debugCallbackHandle|])

    static member CreateWin32 (hwnd, hinstance, appName, engineName, deviceLayers, deviceExtensions, debugCallback) =
        VulkanDevice.Create (appName, engineName, deviceLayers, deviceExtensions, debugCallback, createVkSurface = createWin32Surface hwnd hinstance)

    static member CreateCompute (appName, engineName, deviceLayers, debugCallback, deviceExtensions) =
        VulkanDevice.Create (appName, engineName, deviceLayers, debugCallback, deviceExtensions)

let gate = obj ()

[<Struct;NoEquality;NoComparison>]
type VulkanMemory =
    {
        Device: VulkanDevice
        NativeDeviceMemory: VkDeviceMemory
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
                assert (chunk.Free block)

    interface IDisposable with

        member this.Dispose() =
            this.Free()

    static member Allocate (vulkanDevice: VulkanDevice) (memRequirements: VkMemoryRequirements) properties =
        let physicalDevice = vulkanDevice.PhysicalDevice
        let device = vulkanDevice.Device

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
            | Some (block, chunk, deviceMemory) -> { Device = vulkanDevice; NativeDeviceMemory = deviceMemory; Block = block; Chunk = chunk; IsFree = ref false }
            | _ ->
                let alignment = int memRequirements.alignment
                let size = requestSize
                let upperLimit = 20

                let chunk = Chunk(alignment, upperLimit)
                let block = (chunk.TryRequest size).Value
                let deviceMemory = allocateDeviceMemory device memTypeIndex (pown 2 20 * (int alignment))

                chunks.Add(chunk, deviceMemory)

                { Device = vulkanDevice; NativeDeviceMemory = deviceMemory; Block = block; Chunk = chunk; IsFree = ref false }

type VulkanMemory with

    member this.Map(offset, size) =
        mapMemory this.Device.Device this.NativeDeviceMemory (this.Block.Offset + offset) size

    [<RequiresExplicitTypeArguments>]
    member this.MapAsSpan<'T when 'T : unmanaged>(offset, count) =
        let offset = sizeof<'T> * offset
        let size = sizeof<'T> * count
        let deviceData = this.Map(offset, size)
        Span<'T>(deviceData |> NativePtr.ofNativeInt<'T> |> NativePtr.toVoidPtr, count)

    [<RequiresExplicitTypeArguments>]
    member this.MapAsSpan<'T when 'T : unmanaged>(count) =
        this.MapAsSpan<'T>(0, count)

    member this.Unmap() =
        unmapMemory this.Device.Device this.NativeDeviceMemory