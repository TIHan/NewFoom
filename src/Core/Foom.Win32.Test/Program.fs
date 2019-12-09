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

type EmptyWindowEvents (graphics: FalGraphics) =

    let gate = obj ()
    let mutable quit = false

    interface IWindowEvents with

        member __.OnWindowClosing () =
            lock gate (fun () ->
                quit <- true
                graphics.WaitIdle ()
            )

        member __.OnInputEvents events = 
            if not events.IsEmpty then
                printfn "%A" events

        member __.OnUpdateFrame (_, _) = 
            quit

        member __.OnRenderFrame (_, _, _, _) =
            lock gate (fun () ->
                if not quit then
                    graphics.DrawFrame ()
            )

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
    }

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
    let windowEvents = EmptyWindowEvents instance :> IWindowEvents
    windowState.WindowClosing.Add(windowEvents.OnWindowClosing)

    let window = Window (title, 30., width, height, windowEvents, windowState)

    let vertices =
        [|
            { position = Vector2 (0.f, -0.5f); color = Vector3 (1.f, 0.f, 0.f) }
            { position = Vector2 (0.5f, 0.5f); color = Vector3 (1.f, 0.f, 0.f) }
            { position = Vector2 (-0.5f, 0.5f); color = Vector3 (1.f, 0.f, 0.f) }
        |]
    let verticesBindings = [|mkVertexInputBinding<Vertex> 0u VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX|]
    let verticesAttributes = mkVertexAttributeDescriptions<Vertex> 0u 0u
    let verticesBuffer = instance.CreateBuffer<Vertex> (vertices.Length, BufferFlags.None, BufferKind.Vertex)
    instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)

    let vertex =
        <@
            let vertex = Variable<Vertex> [Decoration.Location 0u] StorageClass.Input
            let position = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input
            let color = Variable<Vector3> [Decoration.Location 1u] StorageClass.Input
            let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
            let mutable fragColor = Variable<Vector3> [Decoration.Location 0u] StorageClass.Output

            let mvp = Variable<ModelViewProjection> [Decoration.Uniform] StorageClass.Uniform

            fun () ->
                gl_Position <- Vector4(vertex.position, 0.f, 1.f)
                fragColor <- vertex.color
        @>
    let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], [])
    let spvVertex =
        Checker.Check vertex
        |> SpirvGen.GenModule spvVertexInfo

    let fragment =
        <@ 
            let fragColor = Variable<Vector3> [Decoration.Location 0u] StorageClass.Input
            let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output

            fun () -> outColor <- Vector4(fragColor, 1.f)
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

    window.Start ()

    printfn "F# Vulkan ended...."
    0
