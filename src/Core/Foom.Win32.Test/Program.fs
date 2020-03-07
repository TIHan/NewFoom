open Foom.Game
open Foom.Win32
open Falkan
open FSharp.Vulkan.Interop
open FSharp.Window
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open System
open System.Numerics
open System.Drawing
open System.Drawing.Imaging
open FSharp.NativeInterop
open Foom.Wad

#nowarn "9"

[<Struct>]
type ModelViewProjection =
    {
        model: Matrix4x4
        view: Matrix4x4
        proj: Matrix4x4
    }    

[<Struct>]
type Vertex =
    {
        position: Vector2
        color: Vector3
        texCoord: Vector2
    }

type Sampler2d = SampledImage<single, DimKind.Two, ImageDepthKind.NoDepth, ImageArrayedKind.NonArrayed, ImageMultisampleKind.Single, ImageSampleKind.Sampler, ImageFormatKind.Unknown, AccessQualifierKind.None>

let radians (degrees) = degrees * MathF.PI / 180.f


let createBitmap (pixels: Pixel [,]) =
    let mutable isTransparent = false

    let width = Array2D.length1 pixels
    let height = Array2D.length2 pixels

    pixels
    |> Array2D.iter (fun p ->
        if p.Equals Pixel.Cyan then
            isTransparent <- true
    )

    let bitmap = new Bitmap (width, height)
    for i = 0 to width - 1 do
        for j = 0 to height - 1 do
            let pixel = pixels.[i, j]
            if pixel = Pixel.Cyan then
                bitmap.SetPixel (i, j, Color.FromArgb (0, 0, 0, 0))
            else
                bitmap.SetPixel (i, j, Color.FromArgb (int pixel.R, int pixel.G, int pixel.B))

    bitmap

let setRender (instance: FalGraphics) =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let e1m1 = wad.FindMap "e1m1"

    let start = e1m1.TryFindPlayer1Start().Value
    let start = Vector3(float32 start.X, float32 start.Y, 28.f)

    //let mvpBindings = [||]
    let mvpUniform = instance.CreateBuffer<ModelViewProjection>(1, FalkanBufferFlags.None, UniformBuffer)
    let mvp =
        {
            model = Matrix4x4.CreateRotationY(radians 90.f)
            view = Matrix4x4.CreateLookAt(Vector3(2.0f, 2.0f, 2.0f), Vector3(0.f), Vector3(0.f, 0.f, 1.0f))
            proj = Matrix4x4.CreatePerspectiveFieldOfView(radians 45.f, 1280.f / 720.f, 0.1f, 1000.f)
        }
    instance.FillBuffer(mvpUniform, ReadOnlySpan [|mvp|])
    instance.SetUniformBuffer<ModelViewProjection>(mvpUniform)

    (e1m1.Sectors, e1m1.ComputeAllSectorGeometry())
    ||> Seq.iter2 (fun s geo ->
        let texture = wad.TryFindFlatTexture(s.FloorTextureName).Value
       
        let data = texture.Data
        use bmp = createBitmap data
        //use bmp = new Bitmap(Bitmap.FromFile("texture.jpg"))
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let image = instance.CreateImage(bmp.Width, bmp.Height)
        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)

        instance.SetSampler image
        instance.FillBuffer(mvpUniform, ReadOnlySpan [|mvp|])
        let vertices =
            [|
                Vector3 (-0.5f, -0.5f, 0.f)
                Vector3 (0.5f, -0.5f, 0.f)
                Vector3 (0.5f, 0.5f, 0.f)
                Vector3 (-0.5f, 0.5f, 0.f)
                Vector3 (-0.5f, -0.5f, 0.f)
                Vector3 (0.5f, 0.5f, 0.f)
            |]
        let verticesBindings = [|mkVertexInputBinding<Vector3> 0u VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX|]
        let verticesAttributes = mkVertexAttributeDescriptions<Vector3> 0u 0u
        let verticesBuffer = instance.CreateBuffer<Vector3> (vertices.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)


        //let uv = Map.CreateSectorUv(texture.Width,texture.Height, geo.FloorVertices)
        let uv =
            [|
                Vector2(1.f, 0.f)
                Vector2(0.f, 0.f)
                Vector2(0.f, 1.f)
                Vector2(1.f, 1.f)
                Vector2(1.f, 0.f)
                Vector2(0.f, 1.f)
            |]

        let uvBindings = [|mkVertexInputBinding<Vector2> 1u VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX|]
        let uvAttributes = mkVertexAttributeDescriptions<Vector2> 1u 1u
        let uvBuffer = instance.CreateBuffer<Vector2> (uv.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(uvBuffer, ReadOnlySpan uv)

        let vertex =
            <@
                let mvp = Variable<ModelViewProjection> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.Uniform
                let position = Variable<Vector3> [Decoration.Location 0u; Decoration.Binding 0u] StorageClass.Input
                let texCoord = Variable<Vector2> [Decoration.Location 1u; Decoration.Binding 1u] StorageClass.Input
                let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
                let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output

                fun () ->
                    gl_Position <- Vector4.Transform(Vector4(position, 1.f), mvp.model * mvp.view * mvp.proj)
                    fragTexCoord <- texCoord
            @>
        let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], [])
        let spvVertex =
            Checker.Check vertex
            |> SpirvGen.GenModule spvVertexInfo

        let fragment =
            <@ 
                let sampler = Variable<Sampler2d> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.UniformConstant
                let fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input
                let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output

                fun () ->
                  outColor <- sampler.ImplicitLod fragTexCoord
            @>
        let spvFragmentInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], ExecutionMode.OriginUpperLeft)
        let spvFragment = 
            Checker.Check fragment
            |> SpirvGen.GenModule spvFragmentInfo

        let vertexBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvVertex)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes
        let fragmentBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvFragment)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes

        let pipelineIndex = instance.AddShader(Array.append verticesBindings uvBindings, Array.append verticesAttributes uvAttributes, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)
        instance.RecordDraw(pipelineIndex, [verticesBuffer;uvBuffer], vertices.Length, 1))
    mvp, mvpUniform

[<EntryPoint>]
let main argv =
    let title = "F# Vulkan"
    let width = 1280
    let height = 720

    // Add dispose
    let windowState = Win32WindowState (title, width, height)
    windowState.Show ()

    let hwnd = windowState.Hwnd
    let hinstance = windowState.Hinstance

    use device = FalDevice.CreateWin32Surface(hwnd, hinstance, "App", "Engine", ["VK_LAYER_KHRONOS_validation"], [VK_KHR_SWAPCHAIN_EXTENSION_NAME])
    use instance = FalGraphics.Create (device, windowState.WindowResized)
    let mvp, mvpUniform = setRender instance

    let mutable mvp = mvp

    let windowEvents = 
        let gate = obj ()
        let mutable quit = false

        { new IWindowEvents with

            member __.OnWindowClosing () =
                lock gate (fun () ->
                    quit <- true
                    instance.WaitIdle ()
                )

            member __.OnInputEvents events = 
                if not events.IsEmpty then
                    printfn "%A" events

            member __.OnUpdateFrame (time, interval) =
                //mvp <-
                //    { mvp with model = Matrix4x4.CreateRotationY(radians (float32 time))}
                //instance.FillBuffer(mvpUniform, ReadOnlySpan[|mvp|])
              //  instance.SetUniformBuffer(mvpUniform)
                quit

            member __.OnRenderFrame (_, _, _, _) =
                lock gate (fun () ->
                    if not quit then
                        instance.DrawFrame ()
                ) }
    windowState.WindowClosing.Add(windowEvents.OnWindowClosing)

    let window = Window (title, 30., width, height, windowEvents, windowState)
    window.Start ()

    printfn "F# Vulkan ended...."
    0
