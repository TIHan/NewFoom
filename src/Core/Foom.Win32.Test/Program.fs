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
    
    member this.InvertedView =
        let mutable invertedView = Matrix4x4.Identity
        Matrix4x4.Invert(this.view, &invertedView) |> ignore
        { model = this.model; view = invertedView; proj = this.proj }

[<Struct>]
type Vertex =
    {
        position: Vector2
        color: Vector3
        texCoord: Vector2
    }

type Sampler2d = SampledImage<single, DimKind.Two, ImageDepthKind.NoDepth, ImageArrayedKind.NonArrayed, ImageMultisampleKind.Single, ImageSampleKind.Sampler, ImageFormatKind.Unknown, AccessQualifierKind.None>

let radians (degrees) = degrees * (MathF.PI / 180.f)


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

let meshShader (instance: FalGraphics) =
    let input1 = FalkanShaderInput.createVector3(PerVertex, 0u, 0u)
    let input2 = FalkanShaderInput.createVector2(PerVertex, 1u, 1u)

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

    instance.CreateShader(input1, input2, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)

let setRender (instance: FalGraphics) =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let e1m1 = wad.FindMap "e1m1"

    let start = e1m1.TryFindPlayer1Start().Value
    let start = Vector3(float32 start.X, float32 start.Y, 28.f)

    //let mvpBindings = [||]
    let mvpUniform = instance.CreateBuffer<ModelViewProjection>(1, FalkanBufferFlags.None, UniformBuffer)
    let mutable quat = Matrix4x4.Identity //Quaternion.CreateFromAxisAngle(Vector3.UnitX, radians 180.f) |> Matrix4x4.CreateFromQuaternion //Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, radians 180.f)
    quat.Translation <- Vector3(start.X, start.Y, 5000.f)
    let mutable invertedView = Matrix4x4.Identity
    Matrix4x4.Invert(quat, &invertedView) |> ignore
   // let mutable quat = Matrix4x4.CreateLookAt(Vector3.Zero, Vector3(start.X, start.Y, 0.f), Vector3.UnitZ)
    let mvp =
        {
            model = Matrix4x4.Identity
            view = quat //Matrix4x4.CreateLookAt(Vector3(0.0f, 0.0f, 5.0f), Vector3(1.f), Vector3(0.f, 0.f, 1.f))
            proj = Matrix4x4.CreatePerspectiveFieldOfView(radians 45.f, 1280.f / 720.f, 0.1f, 1000000.f)
        }

    instance.FillBuffer(mvpUniform, ReadOnlySpan [|mvp.InvertedView|])
    instance.SetUniformBuffer<ModelViewProjection>(mvpUniform)

    let shader = meshShader instance

    (e1m1.Sectors, e1m1.ComputeAllSectorGeometry())
    ||> Seq.iteri2 (fun i s geo ->
        let texture = wad.TryFindFlatTexture(s.FloorTextureName).Value
       
        let data = texture.Data
        use bmp = createBitmap data
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let image = instance.CreateImage(bmp.Width, bmp.Height)
        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)

        instance.FillBuffer(mvpUniform, ReadOnlySpan [|mvp.InvertedView|])
        let vertices = geo.FloorVertices
        let verticesBuffer = instance.CreateBuffer<Vector3> (vertices.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)

        let uv = Map.CreateSectorUv(texture.Width,texture.Height, geo.FloorVertices)
        let uvBuffer = instance.CreateBuffer<Vector2> (uv.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(uvBuffer, ReadOnlySpan uv)

        shader.AddDraw(image, verticesBuffer, uvBuffer, vertices.Length, 1))
    mvp, mvpUniform

[<EntryPoint>]
let main argv =
    let title = "F# Vulkan"
    let width = 1280
    let height = 720

    // Add dispose
    let windowState = Win32WindowState (title, width, height)
    windowState.Show ()

  //  Console.ReadLine() |> ignore

    let hwnd = windowState.Hwnd
    let hinstance = windowState.Hinstance

    use device = FalDevice.CreateWin32Surface(hwnd, hinstance, "App", "Engine", ["VK_LAYER_LUNARG_standard_validation"], [VK_KHR_SWAPCHAIN_EXTENSION_NAME])
    use instance = FalGraphics.Create (device, windowState.WindowResized)
    let mvp, mvpUniform = setRender instance
    instance.SetupCommands()

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
                events
                |> List.iter (fun x ->
                    let v =
                        match x with
                        | InputEvent.KeyPressed 'W' -> Vector3(0.f, 20.f, 0.f) +  mvp.view.Translation
                        | InputEvent.KeyPressed 'S' -> Vector3(0.f, -20.f, 0.f) +  mvp.view.Translation
                        | InputEvent.KeyPressed 'A' -> Vector3(-20.f, 0.f, 0.f) +  mvp.view.Translation
                        | InputEvent.KeyPressed 'D' -> Vector3(20.f, 0.f, 0.f) +  mvp.view.Translation
                        | _ -> mvp.view.Translation
                    let mutable view = mvp.view
                    view.Translation <- v
                    mvp <-
                        { mvp with view = view }
                    instance.FillBuffer(mvpUniform, ReadOnlySpan[|mvp.InvertedView|])
                )

            member __.OnUpdateFrame (time, interval) =
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
