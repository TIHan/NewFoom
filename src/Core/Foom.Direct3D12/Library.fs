namespace Foom.Direct3D12

open System
open System.Numerics
open System.Threading
open FSharp.NativeInterop
open SharpDX
open SharpDX.DXGI
open SharpDX.Direct3D
open SharpDX.Direct3D12
open SharpDX.D3DCompiler

#nowarn "9"
#nowarn "42"

[<AutoOpen>]
module private Helpers =

    [<Struct>]
    type Vertex =
        {
            Position: Vector3
            Color: Vector4
        }

    let createDevice () =
        let debugInterface = DebugInterface.Get()
        debugInterface.EnableDebugLayer()
       // let debugController = debugInterface.QueryInterface<Debug1>()
       // debugController.EnableGPUBasedValidation <- Mathematics.Interop.RawBool.op_Implicit(true)
        new Device(null, FeatureLevel.Level_11_0)

    let createCommandQueue (device: Device) =
        // Describe and create the command queue.
        let desc = CommandQueueDescription(CommandListType.Direct)
        device.CreateCommandQueue(desc)

    let createSwapChain (factory: Factory2) (cmdQueue: CommandQueue) frameCount width height (hwnd: nativeint) =
        // Describe and create the swap chain.
        let mutable swapChainDesc = 
            SwapChainDescription1(
                BufferCount = frameCount,
                Width = width,
                Height = height,
                Format = DXGI.Format.R8G8B8A8_UNorm,
                Usage = Usage.RenderTargetOutput,
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

        (renderTargets, rtvHeapDescriptorSize)

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

    let createRootSignature (device: Device) =
        let range = 
            DescriptorRange(
                RangeType = DescriptorRangeType.ShaderResourceView,
                DescriptorCount = 1,
                BaseShaderRegister = 0,
                RegisterSpace = 0
                //Flags = DescriptorRangeFlags.DataStatic
            )
        let rootParameter = RootParameter(ShaderVisibility.Pixel, [|range|])
        let samplerDesc = 
            StaticSamplerDescription(
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Border,
                AddressV = TextureAddressMode.Border,
                AddressW = TextureAddressMode.Border,
                MipLODBias = 0.f,
                MaxAnisotropy = 0,
                ComparisonFunc = Comparison.Never,
                BorderColor = StaticBorderColor.TransparentBlack,
                MinLOD = 0.f,
                MaxLOD = Single.MaxValue,
                ShaderRegister = 0,
                RegisterSpace = 0,
                ShaderVisibility = ShaderVisibility.Pixel
            )
        let rootSignatureDesc = RootSignatureDescription(RootSignatureFlags.AllowInputAssemblerInputLayout, [|rootParameter|], [|samplerDesc|])
        let rootSignatureBlob = rootSignatureDesc.Serialize()
        device.CreateRootSignature(Blob.op_Implicit(rootSignatureBlob))

    let createTexture (device: Device) (cmdList: GraphicsCommandList) =
        // Describe and create a Texture2D.
        let textureDesc =
            ResourceDescription(
                MipLevels = 1s,
                Format = Format.R8G8B8A8_UNorm,
                Width = 256L,
                Height = 256,
                Flags = ResourceFlags.None,
                DepthOrArraySize = 1s,
                Dimension = ResourceDimension.Texture2D
            )

        textureDesc.SampleDescription.Count <- 1
        textureDesc.SampleDescription.Quality <- 0
        
        let texture = device.CreateCommittedResource(HeapProperties(HeapType.Default), HeapFlags.None, textureDesc, ResourceStates.CopyDestination)

        let uploadBufferSize = 
            let mutable desc = texture.Description
            let mutable requiredSize = 0L
            device.GetCopyableFootprints(&desc, 0, 1, 0L, null, null, null, &requiredSize)
            requiredSize

        // Create the GPU upload buffer.
        let textureUploadHeap = device.CreateCommittedResource(HeapProperties(HeapType.Upload), HeapFlags.None, ResourceDescription.Buffer(int64 uploadBufferSize), ResourceStates.GenericRead) 

        let textureData =
            let rowPitch = 256 * 4
            let cellPitch = rowPitch >>> 3
            let cellHeight = 256 >>> 3
            let textureSize = rowPitch * 256
            
            let data = Array.zeroCreate textureSize
            let mutable n = 0
            while (n < textureSize) do
                let x = n % rowPitch
                let y = n % rowPitch
                let i = x / cellPitch
                let j = y / cellHeight

                if i % 2 = j % 2 then
                    data.[n] <- 0x00uy // r
                    data.[n + 1] <- 0x00uy // g
                    data.[n + 2] <- 0x00uy // b
                    data.[n + 3] <- 0xffuy // a
                else
                    data.[n] <- 0xffuy
                    data.[n + 1] <- 0xffuy
                    data.[n + 2] <- 0xffuy
                    data.[n + 3] <- 0xffuy

                n <- n + 4

            data
        
        // Copy data to the intermediate upload heap and then schedule a copy 
        // from the upload heap to the Texture2D.
        use ptr = fixed textureData
        textureUploadHeap.WriteToSubresource(0, Nullable(), NativePtr.toNativeInt ptr, 4 * 256, textureData.Length)

        cmdList.CopyTextureRegion(TextureCopyLocation(texture, 0), 0, 0, 0, TextureCopyLocation(textureUploadHeap, 0), Nullable())
        cmdList.ResourceBarrierTransition(texture, ResourceStates.CopyDestination, ResourceStates.PixelShaderResource)

        // TODO: Describe and create a srv for the texture
        texture

    let createPipelineState (device: Device) (rootSignature: RootSignature) =
        // Create the pipeline state, which includes compiling and loading shaders.
        let shaderFlags = ShaderFlags.Debug ||| ShaderFlags.SkipOptimization
        let vertexShader = ShaderBytecode.CompileFromFile("default.hlsl", "VSMain", "vs_5_0", shaderFlags)
        let pixelShader = ShaderBytecode.CompileFromFile("default.hlsl", "PSMain", "ps_5_0", shaderFlags)

        // Define the vertex input layout.
        let inputElements =
            [|
                InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0)
                InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, InputClassification.PerVertexData, 0)
            |]

        // Describe and create the graphics pipeline state object (PSO).
        let psoDesc =
            GraphicsPipelineStateDescription(
                InputLayout = InputLayoutDescription(inputElements),
                RootSignature = rootSignature,
                VertexShader = SharpDX.Direct3D12.ShaderBytecode.op_Implicit(vertexShader.Bytecode.Data),
                PixelShader = SharpDX.Direct3D12.ShaderBytecode.op_Implicit(pixelShader.Bytecode.Data),
                RasterizerState = RasterizerStateDescription.Default(),
                BlendState = BlendStateDescription.Default(),
                SampleMask = Int32.MaxValue,
                PrimitiveTopologyType = PrimitiveTopologyType.Triangle,
                RenderTargetCount = 1,
                DepthStencilFormat = Format.D32_Float
            )

        psoDesc.DepthStencilState.IsDepthEnabled <- Mathematics.Interop.RawBool.op_Implicit(false)
        psoDesc.DepthStencilState.IsStencilEnabled <- Mathematics.Interop.RawBool.op_Implicit(false)
        psoDesc.SampleDescription.Count <- 1
        psoDesc.RenderTargetFormats.[0] <- Format.R8G8B8A8_UNorm

        // Create pipeline state.
        device.CreateGraphicsPipelineState(psoDesc)

    let createCommandList (device: Device) (cmdAllocs: CommandAllocator []) frameIndex (pipelineState: PipelineState) =
        // Create the command list.
        let commandList = device.CreateCommandList(CommandListType.Direct, cmdAllocs.[frameIndex], pipelineState)

        // Command lists are created in the recording state, but there is nothing
        // to record yet. The main loop expects it to be closed, so close it now.
        commandList.Close()
        commandList

[<Sealed>]
type Direct3D12Pipeline(width, height, hwnd: nativeint) =

    let mutable isDisposing = 0
    let mutable isDisposed = false

    let frameCount = 2
    let fenceValues = Array.zeroCreate<int64> frameCount
    let factory = new Factory4()
    let viewPort = Mathematics.Interop.RawViewportF(Width = float32 width, Height = float32 height)
    let scissorRect = Mathematics.Interop.RawRectangle(0, 0, width, height)

    let device =                                    createDevice ()
    let cmdQueue =                                  createCommandQueue device
    let swapChain =                                 createSwapChain factory cmdQueue frameCount width height hwnd

    let mutable frameIndex =                        swapChain.CurrentBackBufferIndex

    let rtvHeap =                                   createDescriptorHeap device frameCount
    let renderTargets, rtvDescriptorSize =          createRenderTargets device swapChain rtvHeap frameCount
    let cmdAllocs =                                 createCommandAllocators device frameCount
    let fence =                                     createFence device fenceValues frameIndex
    let fenceEvent =                                new AutoResetEvent(false) // Create an event handle to use for frame synchronization.
    let rootSignature =                             createRootSignature device
    let pipelineState =                             createPipelineState device rootSignature
    let cmdList =                                   createCommandList device cmdAllocs frameIndex pipelineState

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
            fence.SetEventOnCompletion(fenceValues.[frameIndex], fenceEvent.SafeWaitHandle.DangerousGetHandle())
            fenceEvent.WaitOne() |> ignore

        // Set the fence value for the next frame.
        fenceValues.[frameIndex] <- fenceValue + 1L

    /// Wait for pending GPU work to complete.
    member __.WaitForGpu() =
        // Schedule a Signal command in the queue.
        cmdQueue.Signal(fence, fenceValues.[frameIndex])

        // Wait until the fence has been processed.
        fence.SetEventOnCompletion(fenceValues.[frameIndex], fenceEvent.SafeWaitHandle.DangerousGetHandle())
        fenceEvent.WaitOne() |> ignore

        // Increment the fence value for the current frame.
        fenceValues.[frameIndex] <- fenceValues.[frameIndex] + 1L


    member this.LoadAssets() =
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
        vertexBufferView.StrideInBytes <- Utilities.SizeOf<Vertex>() //sizeof<Vertex>
        vertexBufferView.SizeInBytes <- int vertexBufferSize

        // Wait for the command list to execute; we are reusing the same command 
        // list in our main loop but for now, we just want to wait for setup to 
        // complete before continuing.
        this.WaitForGpu();

    member __.PopulateCommandList() =
        let cmdAlloc = cmdAllocs.[frameIndex]

        // Command list allocators can only be reset when the associated 
        // command lists have finished execution on the GPU; apps should use 
        // fences to determine GPU execution progress.
        cmdAlloc.Reset()

        // However, when ExecuteCommandList() is called on a particular command 
        // list, that command list can then be reset at any time and must be before 
        // re-recording.
        cmdList.Reset(cmdAlloc, pipelineState)

        // Set necessary state.
        cmdList.SetGraphicsRootSignature(rootSignature)
        cmdList.SetViewports([|viewPort|])
        cmdList.SetScissorRectangles([|scissorRect|])

        // Indicate that the back buffer will be used as a render target.
        cmdList.ResourceBarrierTransition(renderTargets.[frameIndex], ResourceStates.Present, ResourceStates.RenderTarget)

        let mutable rtvHandle = rtvHeap.CPUDescriptorHandleForHeapStart
        rtvHandle <- rtvHandle + (frameIndex * rtvDescriptorSize)
        cmdList.SetRenderTargets([|rtvHandle|], Nullable())

        // Record commands.
        cmdList.ClearRenderTargetView(rtvHandle, Mathematics.Interop.RawColor4(0.f, 0.2f, 0.4f, 1.f), 0, null)
        cmdList.PrimitiveTopology <- SharpDX.Direct3D.PrimitiveTopology.TriangleList
        cmdList.SetVertexBuffer(0, vertexBufferView)
        cmdList.DrawInstanced(3, 1, 0, 0)

        // Indicate that the back buffer will now be used to present.
        cmdList.ResourceBarrierTransition(renderTargets.[frameIndex], ResourceStates.RenderTarget, ResourceStates.Present)

        cmdList.Close()

    member this.Render() =
        // Record all the commands we need to render the scene into the command list.
        this.PopulateCommandList()

        // Execute the command list.
        cmdQueue.ExecuteCommandList(cmdList)

        // Present the frame.
        swapChain.Present(1, PresentFlags.None) |> ignore

        this.MoveToNextFrame()

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
