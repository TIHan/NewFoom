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

let radians (degrees) = degrees * MathF.PI / 180.f

let setRender (instance: FalGraphics) =
    use bmp = new Bitmap(Bitmap.FromFile("texture.jpg"))
    let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
    let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat)

    let image = instance.CreateImage(bmp.Width, bmp.Height)
    let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
    instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
    bmp.UnlockBits(data)
    //let mvpBindings = [||]
    let mvpUniform = instance.CreateBuffer<ModelViewProjection>(1, BufferFlags.None, BufferKind.Uniform)
    let mvp =
        {
            model = Matrix4x4.CreateRotationY(radians 90.f)
            view = Matrix4x4.CreateLookAt(Vector3(2.0f, 2.0f, 2.0f), Vector3(0.f), Vector3(0.f, 0.f, 1.0f))
            proj = Matrix4x4.CreatePerspectiveFieldOfView(radians 45.f, 1280.f / 720.f, 0.1f, 10.f)
        }
    instance.FillBuffer(mvpUniform, ReadOnlySpan [|mvp|])
    instance.SetUniformBuffer(mvpUniform)
    instance.SetSampler image

    let vertices =
        [|
            { position = Vector2 (-0.5f, -0.5f); color = Vector3 (1.f, 0.f, 0.f); texCoord = Vector2(1.f, 0.f) }
            { position = Vector2 (0.5f, -0.5f); color = Vector3 (0.f, 1.f, 0.f); texCoord = Vector2(0.f, 0.f) }
            { position = Vector2 (0.5f, 0.5f); color = Vector3 (0.f, 0.f, 1.f); texCoord = Vector2(0.f, 1.f) }
            { position = Vector2 (-0.5f, 0.5f); color = Vector3 (1.f, 1.f, 1.f); texCoord = Vector2(1.f, 1.f) }
        |]
    let verticesBindings = [|mkVertexInputBinding<Vertex> 0u VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX|]
    let verticesAttributes = mkVertexAttributeDescriptions<Vertex> 0u 0u
    let verticesBuffer = instance.CreateBuffer<Vertex> (vertices.Length, BufferFlags.None, BufferKind.Vertex)
    instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)

    let vertex =
        <@
            let mvp = Variable<ModelViewProjection> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.Uniform
            let vertex = Variable<Vertex> [Decoration.Location 0u] StorageClass.Input
            let _position = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input
            let _color = Variable<Vector3> [Decoration.Location 1u] StorageClass.Input
            let _texCoord = Variable<Vector2> [Decoration.Location 2u] StorageClass.Input
            let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
            let mutable fragColor = Variable<Vector3> [Decoration.Location 0u] StorageClass.Output
            let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 1u] StorageClass.Output

            fun () ->
                gl_Position <- Vector4.Transform(Vector4(vertex.position, 0.f, 1.f), mvp.model * mvp.view * mvp.proj)
                fragColor <- vertex.color
                fragTexCoord <- vertex.texCoord
        @>
    let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], [])
    let spvVertex =
        Checker.Check vertex
        |> SpirvGen.GenModule spvVertexInfo

    let fragment =
        <@ 
            let sampler = Variable<Sampler2d> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.Image
            let fragColor = Variable<Vector3> [Decoration.Location 0u] StorageClass.Input
            let fragTexCoord = Variable<Vector2> [Decoration.Location 1u] StorageClass.Input
            let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output

            fun () -> outColor <- Vector4(fragTexCoord, 0.f, 1.f)
        @>
    let spvFragmentInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], ExecutionMode.OriginUpperLeft)
    let spvFragment = 
        Checker.Check fragment
        |> SpirvGen.GenModule spvFragmentInfo

   // let vertexBytes = System.IO.File.ReadAllBytes("triangle_vertex.spv")
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
   // let fragmentBytes = System.IO.File.ReadAllBytes("triangle_fragment.spv")
    let pipelineIndex = instance.AddShader(verticesBindings, verticesAttributes, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)
    instance.RecordDraw(pipelineIndex, [|verticesBuffer.Buffer|], vertices.Length, 1)
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
    use instance = FalGraphics.Create device
    let mvp, mvpUniform = setRender instance

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
               // let mvp =
               //     { mvp with model = Matrix4x4.CreateRotationY(radians (float32 time))}
             //   instance.FillBuffer(mvpUniform, ReadOnlySpan[|mvp|])
            //    instance.SetUniformBuffer(mvpUniform)
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
