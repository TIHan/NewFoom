module Foom.InternalD3D12

open System
open System.Numerics
open System.Threading
open FSharp.NativeInterop
open SharpDX
open SharpDX.DXGI
open SharpDX.Direct3D
open SharpDX.Direct3D12
open SharpDX.D3DCompiler
open Foom.Win32Internal

#nowarn "9"
#nowarn "42"

[<Struct>]
type Vertex =
    {
        Position: Vector3
        Color: Vector4
    }

let tryCreateDevice (factory: Factory2) =
    factory.Adapters1
    |> Array.tryPick (fun adapter ->
        if not(adapter.Description1.Flags.HasFlag(AdapterFlags.Software)) then
            let device = new Device(adapter, FeatureLevel.Level_11_0)
            Some(device)
        else
            None 
    )

let createDevice factory =
    match tryCreateDevice factory with
    | Some(device) -> device
    | _ -> failwith "Can't find DirectX12 device."

let createCommandQueue (device: Device) =
    // Describe and create the command queue.
    let desc = CommandQueueDescription(CommandListType.Direct)
    device.CreateCommandQueue(desc)

let createSwapChain factory cmdQueue frameCount width height (hwnd: nativeint) =
    // Describe and create the swap chain.
    let swapChainDesc = 
        SwapChainDescription1(
            BufferCount = frameCount,
            Width = width,
            Height = height,
            Format = DXGI.Format.R8G8B8A8_UNorm,
            SwapEffect = DXGI.SwapEffect.FlipDiscard
        )

    swapChainDesc.SampleDescription.Count <- 1

    use swapChain = new SwapChain1(factory, cmdQueue (* Swap chain needs the queue so that it can force a flush on it. *), hwnd, &swapChainDesc)
    swapChain.QueryInterface<SwapChain3>()

let createDescriptorHeap (device: Device) frameCount =
    // Describe and create a render target view (RTV) descriptor heap.
    let rtvHeapDesc = 
        DescriptorHeapDescription(
            DescriptorCount = frameCount,
            Type = DescriptorHeapType.RenderTargetView,
            Flags = DescriptorHeapFlags.None
        )
        
    device.CreateDescriptorHeap(rtvHeapDesc)

let createRenderTargets (device: Device) (swapChain: SwapChain3) (rtvHeap: DescriptorHeap) frameCount =
    // Create frame resources.
    let renderTargets = Array.zeroCreate<Resource> frameCount

    let rtvHeapDescriptorSize = device.GetDescriptorHandleIncrementSize(DescriptorHeapType.RenderTargetView)
    let mutable rtvHandle = rtvHeap.CPUDescriptorHandleForHeapStart

    // Create a RTV for each frame.
    for i = 0 to frameCount - 1 do
        let rt = swapChain.GetBackBuffer(i)
        device.CreateRenderTargetView(rt, System.Nullable(), rtvHandle)

        renderTargets.[i] <- rt
        rtvHandle <- rtvHandle + rtvHeapDescriptorSize

    renderTargets

let createCommandAllocators (device: Device) frameCount =
    let cmdAllocs = Array.zeroCreate<CommandAllocator> frameCount

    // Create a command allocator for each frame.
    for i = 0 to frameCount - 1 do
        cmdAllocs.[i] <- device.CreateCommandAllocator(CommandListType.Direct)

    cmdAllocs

let createFence (device: Device) (fenceValues: int64 []) (frameIndex: int) =
    let fence = device.CreateFence(fenceValues.[frameIndex], FenceFlags.None)
    fenceValues.[frameIndex] <- fenceValues.[frameIndex] + 1L
    fence

let createNativeEvent () =
    let evt = CreateEventW(NativePtr.ofNativeInt IntPtr.Zero, 0uy, 0uy, NativePtr.ofNativeInt IntPtr.Zero)
    if (evt = IntPtr.Zero) then
        failwith "Unable to create event."
    evt

let moveToNextFrame (cmdQueue: CommandQueue) (swapChain: DXGI.SwapChain3) (fence: Fence) fenceEvent (fenceValues: int64 []) (frameIndex: byref<int>) =
    let fenceValue = fenceValues.[frameIndex]

    cmdQueue.Signal(fence, fenceValue)

    frameIndex <- swapChain.CurrentBackBufferIndex

    if (fence.CompletedValue < fenceValues.[frameIndex]) then
        fence.SetEventOnCompletion(fenceValues.[frameIndex], fenceEvent)
        WaitForSingleObjectEx(fenceEvent, 0xFFFFFFFFu, 0uy) |> ignore

    fenceValues.[frameIndex]

let createDebug () =
    let debugInterface = DebugInterface.Get()
    let debugController = debugInterface.QueryInterface<Debug1>()
    debugController.EnableGPUBasedValidation <- Mathematics.Interop.RawBool.op_Implicit(true)
    debugController


[<Sealed>]
type Direct3D12Pipeline(width, height, hwnd: nativeint) =

    let mutable isDisposing = 0
    let mutable isDisposed = false

    let frameCount = 2
    let fenceValues = Array.zeroCreate<int64> frameCount
    let factory = new Factory4()

    let device =                createDevice factory
    let debug =                 createDebug ()
    let cmdQueue =              createCommandQueue device
    let swapChain =             createSwapChain factory cmdQueue frameCount width height hwnd

    let mutable frameIndex =    swapChain.CurrentBackBufferIndex

    let rtvHeap =               createDescriptorHeap device frameCount
    let renderTargets =         createRenderTargets device swapChain rtvHeap frameCount
    let cmdAllocs =             createCommandAllocators device frameCount
    let fence =                 createFence device fenceValues frameIndex
    let fenceEvent =            createNativeEvent () // Create an event handle to use for frame synchronization.

    let mutable vertexBufferView = VertexBufferView()

    /// Prepare to render the next frame.
    member __.MoveToNextFrame () =
        // Schedule a Signal command in the queue.
        let fenceValue = fenceValues.[frameIndex]
        cmdQueue.Signal(fence, fenceValue)

        // Update the frame index.
        frameIndex <- swapChain.CurrentBackBufferIndex

        // If the next frame is not ready to be rendered yet, wait until it is ready.
        if (fence.CompletedValue < fenceValues.[frameIndex]) then
            fence.SetEventOnCompletion(fenceValues.[frameIndex], fenceEvent)
            WaitForSingleObjectEx(fenceEvent, 0xFFFFFFFFu (* INFINITE *), 0uy (* FALSE *)) |> ignore

        // Set the fence value for the next frame.
        fenceValues.[frameIndex] <- fenceValue + 1L

    /// Wait for pending GPU work to complete.
    member __.WaitForGpu() =
        // Schedule a Signal command in the queue.
        cmdQueue.Signal(fence, fenceValues.[frameIndex])

        // Wait until the fence has been processed.
        fence.SetEventOnCompletion(fenceValues.[frameIndex], fenceEvent)
        WaitForSingleObjectEx(fenceEvent, 0xFFFFFFFFu (* INFINITE *), 0uy (* FALSE *)) |> ignore

        // Increment the fence value for the current frame.
        fenceValues.[frameIndex] <- fenceValues.[frameIndex] + 1L


    member this.LoadAssets() =

        // Create an empty root signature.
        let rootSignatureDesc = RootSignatureDescription(Flags = RootSignatureFlags.AllowInputAssemblerInputLayout)
        let rootSignatureBlob = rootSignatureDesc.Serialize()
        let rootSignatureDataPointer = DataPointer(rootSignatureBlob.BufferPointer, int rootSignatureBlob.BufferSize)
        let rootSignature = device.CreateRootSignature(rootSignatureDataPointer)

        // Create the pipeline state, which includes compiling and loading shaders.
        let shaderFlags = ShaderFlags.Debug ||| ShaderFlags.SkipOptimization
        let vertexShader = ShaderBytecode.CompileFromFile("default.hlsl", "VSMain", "vs_5_0", shaderFlags)
        let pixelShader = ShaderBytecode.CompileFromFile("default.hlsl", "PSMain", "ps_5_0", shaderFlags)

        // Define the vertex input layout.
        let inputElements =
            [|
                InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0)
                InputElement("COLOR", 0, Format.R32G32B32A32_Float, 0, 12, InputClassification.PerVertexData, 0)
            |]

        // Describe and create the graphics pipeline state object (PSO).
        let psoDesc =
            GraphicsPipelineStateDescription(
                InputLayout = InputLayoutDescription(inputElements),
                RootSignature = rootSignature,
                VertexShader = SharpDX.Direct3D12.ShaderBytecode.op_Implicit(vertexShader.Bytecode.Data),
                PixelShader = SharpDX.Direct3D12.ShaderBytecode.op_Implicit(pixelShader.Bytecode.Data),
                RasterizerState = RasterizerStateDescription(),
                BlendState = BlendStateDescription(),
                SampleMask = Int32.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1
            )

        psoDesc.DepthStencilState.IsDepthEnabled <- Mathematics.Interop.RawBool.op_Implicit(false)
        psoDesc.DepthStencilState.IsStencilEnabled <- Mathematics.Interop.RawBool.op_Implicit(false)
        psoDesc.SampleDescription.Count <- 1
        psoDesc.RenderTargetFormats.[0] <- Format.R8G8B8A8_UNorm

        // Create pipeline state.
        let pipelineState = device.CreateGraphicsPipelineState(psoDesc)

        // Create the command list.
        let commandList = device.CreateCommandList(CommandListType.Direct, cmdAllocs.[frameIndex], pipelineState)

        // Command lists are created in the recording state, but there is nothing
        // to record yet. The main loop expects it to be closed, so close it now.
        commandList.Close()

        // Create the vertex buffer.
        let aspectRatio = float32 (width / height)
        let triangleVertices =
            [|
                { Position = Vector3(0.f, 0.25f * aspectRatio, 0.f); Color = Vector4(1.f, 0.f, 0.f, 1.f) }
                { Position = Vector3(0.25f, -0.25f * aspectRatio, 0.f); Color = Vector4(0.f, 1.f, 0.f, 1.f) }
                { Position = Vector3(-0.25f, -0.25f * aspectRatio, 0.f); Color = Vector4(0.f, 0.f, 1.f, 1.f) }
            |]

        let vertexBufferSize = Utilities.SizeOf(triangleVertices) |> int64

        // Note: using upload heaps to transfer static data like vert buffers is not 
        // recommended. Every time the GPU needs it, the upload heap will be marshalled 
        // over. Please read up on Default Heap usage. An upload heap is used here for 
        // code simplicity and because there are very few verts to actually transfer.
        let vertexBuffer =
            device.CreateCommittedResource(
                HeapProperties(HeapType.Upload),
                HeapFlags.None,
                ResourceDescription.Buffer(vertexBufferSize),
                ResourceStates.GenericRead
            )

        // Copy the triangle data to the vertex buffer.
        let mapped = Span<Vertex>(vertexBuffer.Map(0).ToPointer(), int vertexBufferSize)
        triangleVertices.CopyTo(mapped)
        vertexBuffer.Unmap(0)

        // Initialize the vertex buffer view.
        vertexBufferView.BufferLocation <- vertexBuffer.GPUVirtualAddress
        vertexBufferView.StrideInBytes <- sizeof<Vertex>
        vertexBufferView.SizeInBytes <- int vertexBufferSize

        // Wait for the command list to execute; we are reusing the same command 
        // list in our main loop but for now, we just want to wait for setup to 
        // complete before continuing.
        this.WaitForGpu();

    interface IDisposable with

        member __.Dispose() =
            let prevIsDisposing = Interlocked.Exchange(&isDisposing, 1)
            if prevIsDisposing = 0 then
                fence.Dispose()
                renderTargets |> Array.iter (fun x -> x.Dispose())
                rtvHeap.Dispose()
                swapChain.Dispose()
                cmdQueue.Dispose()
                device.Dispose()
                isDisposed <- true
