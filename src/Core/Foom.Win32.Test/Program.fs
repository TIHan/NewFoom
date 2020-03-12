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
open Falkan.FreeType

#nowarn "9"
#nowarn "51"

let loadMusic (wad: Wad) =    
    let fmodCheckResult res =
        if res <> FMOD.RESULT.OK then
            failwithf "FMOD error! (%A) %s\n" res (FMOD.Error.String res)
    

    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let music = wad.TryFindMusic "d_e1m2"
    
    let res, fmodSystem = FMOD.Factory.System_Create()
    fmodCheckResult res
    fmodSystem.init(512, FMOD.INITFLAGS.NORMAL, 0n) |> fmodCheckResult
    
    let res, soundGroup = fmodSystem.createSoundGroup("wad")
    fmodCheckResult res
    
    let mutable info = FMOD.CREATESOUNDEXINFO()
    info.format <- FMOD.SOUND_FORMAT.PCM16
    info.cbsize <- sizeof<FMOD.CREATESOUNDEXINFO>
    info.length <- uint32 music.Value.Length
    let res, sound = fmodSystem.createSound(music.Value, FMOD.MODE.OPENMEMORY ||| FMOD.MODE.LOOP_NORMAL, &info)
    Array.Clear(music.Value, 0, music.Value.Length)
    fmodCheckResult res
    
    sound.setSoundGroup soundGroup |> fmodCheckResult
    
    let res, channelGroup = fmodSystem.createChannelGroup("music")
    fmodCheckResult res
    
    let res, channel = fmodSystem.playSound(sound, channelGroup, false)
    fmodCheckResult res
    
    let res, isPlaying = channel.isPlaying()
    fmodCheckResult res
    
    fmodSystem.update() |> fmodCheckResult
    
    let res, v = channel.getVolume()
    ()

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
    let input1 = FalkanShaderVertexInput(PerVertex, 0u, 0u, typeof<Vector3>)
    let input2 = FalkanShaderVertexInput(PerVertex, 1u, 1u, typeof<Vector2>)

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
                let color = sampler.ImplicitLod fragTexCoord
                if color.W < 0.5f then
                    kill ()
                outColor <- color
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

    let layout = 
        Shader(0,true,
            [
                FalkanShaderDescriptorLayout(UniformBufferDescriptor, VertexStage, 0u)
                FalkanShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 1u)
            ],
            [input1; input2])

    instance.CreateShader(layout, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)

let textShader (instance: FalGraphics) =
    let input1 = FalkanShaderVertexInput(PerVertex, 0u, 0u, typeof<Vector2>)
    let input2 = FalkanShaderVertexInput(PerVertex, 1u, 1u, typeof<Vector2>)

    let vertex =
        <@
            let position = Variable<Vector2> [Decoration.Location 0u; Decoration.Binding 0u] StorageClass.Input
            let texCoord = Variable<Vector2> [Decoration.Location 1u; Decoration.Binding 1u] StorageClass.Input
            let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
            let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output

            fun () ->
                gl_Position <- Vector4(position, 0.f, 1.f)
                fragTexCoord <- texCoord
        @>
    let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], [])
    let spvVertex =
        Checker.Check vertex
        |> SpirvGen.GenModule spvVertexInfo

    let fragment =
        <@ 
            let sampler = Variable<Sampler2d> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.UniformConstant
            let fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input
            let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output

            fun () ->
                let color = sampler.ImplicitLod fragTexCoord
                if color.W < 0.5f then
                    kill ()
                outColor <- color
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

    let layout = 
        Shader(0,false,
            [
                FalkanShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 0u)
            ],
            [input1; input2])

    instance.CreateShader(layout, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)

let setRender (instance: FalGraphics) =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let e1m1 = wad.FindMap "e1m1"
    loadMusic wad
    let start = e1m1.TryFindPlayer1Start().Value
    let start = Vector3(float32 start.X, float32 start.Y, 28.f)

    let mvpUniform = instance.CreateBuffer<ModelViewProjection>(1, FalkanBufferFlags.None, UniformBuffer)
    let mutable quat =  Matrix4x4.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
    quat.Translation <- Vector3(start.X, start.Y, 28.f)
    let mvp =
        {
            model = Matrix4x4.Identity
            view = quat
            proj = Matrix4x4.CreatePerspectiveFieldOfView(radians 45.f, 1280.f / 720.f, 0.1f, 1000000.f)
        }

    instance.FillBuffer(mvpUniform, ReadOnlySpan [|mvp.InvertedView|])
    let shader = meshShader instance

    let getImage name =
        let texture = 
            match wad.TryFindFlatTexture name with
            | None ->
                (wad.TryFindTexture name).Value
            | Some texture -> texture
       
        let data = texture.Data
        use bmp = createBitmap data
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let image = instance.CreateImage(bmp.Width, bmp.Height)
        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image

    let queueDraw (image: FalkanImage) (vertices: Vector3 []) (uv: Vector2 []) =      
        let verticesBuffer = instance.CreateBuffer<Vector3> (vertices.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)

        let uvBuffer = instance.CreateBuffer<Vector2> (uv.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(uvBuffer, ReadOnlySpan uv)

        let draw = shader.CreateDrawBuilder()
        let draw = draw.AddDescriptorBuffer(mvpUniform, sizeof<ModelViewProjection>)
        let draw = draw.AddDescriptorImage(image)
        let draw = draw.Next.AddVertexBuffer(verticesBuffer)
        let draw = draw.AddVertexBuffer(uvBuffer)
        shader.AddDraw(draw, uint32 vertices.Length, 1u)

    (e1m1.Sectors, e1m1.ComputeAllSectorGeometry())
    ||> Seq.iteri2 (fun i s geo ->
        let image = getImage s.FloorTextureName
        let uv = Map.CreateSectorUv(image.Width,image.Height, geo.FloorVertices)
        queueDraw image geo.FloorVertices uv

        let image = getImage s.CeilingTextureName
        let uv = Map.CreateSectorUv(image.Width,image.Height, geo.CeilingVertices)
        queueDraw image geo.CeilingVertices uv

        )


    let queueLinedef geo (ldef: Linedef) sdefIndex =
        match geo.Upper, sdefIndex with
        | Some vertices, Some i ->
            let sdef = e1m1.Sidedefs.[i]
            match sdef.UpperTextureName with
            | Some upperTex ->
                let image = getImage upperTex
                let uv = e1m1.CreateUpperWallUv(ldef, sdef, image.Width, image.Height, vertices)
                queueDraw image vertices uv
            | _ -> ()
        | _ -> ()
        
        match geo.Middle, sdefIndex with
        | Some vertices, Some i ->
            let sdef = e1m1.Sidedefs.[i]
            match sdef.MiddleTextureName with
            | Some upperTex ->
                let image = getImage upperTex
                let uv = e1m1.CreateMiddleWallUv(ldef, sdef, image.Width, image.Height, vertices)
                queueDraw image vertices uv
            | _ -> ()
        | _ -> ()
        

        match geo.Lower, sdefIndex with
        | Some vertices, Some i ->
            let sdef = e1m1.Sidedefs.[i]
            match sdef.LowerTextureName with
            | Some upperTex ->
                let image = getImage upperTex
                let uv = e1m1.CreateLowerWallUv(ldef, sdef, image.Width, image.Height, vertices)
                queueDraw image vertices uv
            | _ -> ()
        | _ -> ()

    e1m1.Linedefs
    |> Seq.iter (fun ldef ->
        let geo = e1m1.ComputeFrontWallGeometry ldef
        queueLinedef geo ldef ldef.FrontSidedefIndex        
        )

    let freeType = FreeType.Create()
    let face = freeType.Load("fonts/OpenSans/OpenSans-Regular.ttf")
   // face.SetCharSize(0, 16*64, 300u, 300u)
    use doot = face.GetCharBitmap('a', 48)
    let bmp = doot.Bitmap
    let shader = textShader instance

    let verticesBuffer = instance.CreateBuffer<Vector2>(3, FalkanBufferFlags.None, VertexBuffer)
    let vertices =
        [|
            Vector2(0.f, 0.f)
            Vector2(0.5f, 0.5f)
            Vector2(0.f, 1.f)
        |]
    instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)

    let uvBuffer = instance.CreateBuffer<Vector2>(3, FalkanBufferFlags.None, VertexBuffer)
    let uv =
        [|
            Vector2(0.f, 0.f)
            Vector2(0.5f, 0.5f)
            Vector2(0.f, 1.f)
        |]
    instance.FillBuffer(uvBuffer, ReadOnlySpan uv)

    let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
    let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
    let image = instance.CreateImage(bmp.Width, bmp.Height)
    let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
    instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
    bmp.UnlockBits(data)

    let draw = shader.CreateDrawBuilder()
    let draw = draw.AddDescriptorImage(image)
    let draw = draw.Next.AddVertexBuffer(verticesBuffer)
    let draw = draw.AddVertexBuffer(uvBuffer)
    shader.AddDraw(draw, uint32 vertices.Length, 1u)

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

    use device = FalDevice.CreateWin32(hwnd, hinstance, "App", "Engine", [VulkanDeviceLayer.LunarGStandardValidation], [])

    let subpasses =
        [RenderSubpass ColorDepthStencilSubpass]

    use instance = FalGraphics.Create (device, windowState.WindowResized, subpasses)
    let mvp, mvpUniform = setRender instance
    instance.SetupCommands()

    let mutable mvp = mvp

    let mutable yaw = 0.f
    let mutable pitch = 0.f

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
                let mutable acc = Vector3.Zero

                let mutable view = mvp.view

                let getRotation () =
                    Quaternion.CreateFromRotationMatrix view

                let setRotation quat =
                    let mutable m = Matrix4x4.CreateFromQuaternion quat
                    m.Translation <- view.Translation
                    view <- m

                let rotation = getRotation ()

                let mutable xrel = 0.f
                let mutable yrel = 0.f
                events
                |> List.iter (fun x ->
                    let v =
                        match x with
                        | InputEvent.KeyPressed 'W' -> Vector3.Transform (-Vector3.UnitZ, rotation)
                        | InputEvent.KeyPressed 'S' -> Vector3.Transform (Vector3.UnitZ, rotation)
                        | InputEvent.KeyPressed 'A' -> Vector3.Transform (-Vector3.UnitX, rotation)
                        | InputEvent.KeyPressed 'D' -> Vector3.Transform (Vector3.UnitX, rotation)
                        | InputEvent.MouseMoved(_, _, xrel2, yrel2) ->
                            xrel <- float32 xrel2
                            yrel <- float32 yrel2
                            Vector3.Zero
                        | _ -> Vector3.Zero
                    acc <- acc + (Vector3(v.X, v.Y, v.Z))
                )

                acc <-
                    if acc <> Vector3.Zero then
                        (acc |> Vector3.Normalize) * 50.f
                    else
                        acc

                if xrel <> 0.f || yrel <> 0.f then
                    yaw <- yaw + (xrel * -0.25f) * (MathF.PI / 180.f)
                    pitch <- pitch + (yrel * -0.25f) * (MathF.PI / 180.f)
                    let rotation = Quaternion.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
                    setRotation (rotation * Quaternion.CreateFromYawPitchRoll(yaw * 0.25f, pitch * 0.25f, 0.f))

                view.Translation <- view.Translation + acc
                mvp <-
                    { mvp with view = view }
                

            member __.OnUpdateFrame (time, interval) =
                instance.FillBuffer(mvpUniform, ReadOnlySpan[|mvp.InvertedView|])
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
