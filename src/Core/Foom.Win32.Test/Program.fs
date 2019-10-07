﻿open Foom.Game
open Foom.Win32
open Foom.Vulkan
open FSharp.Vulkan.Interop
open FSharp.Window
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open System
open System.Numerics

type EmptyWindowEvents (instance: VulkanInstance) =

    let gate = obj ()
    let mutable quit = false

    interface IWindowEvents with

        member __.OnWindowClosing () =
            lock gate (fun () ->
                quit <- true
                instance.WaitIdle ()
            )

        member __.OnInputEvents events = 
            if not events.IsEmpty then
                printfn "%A" events

        member __.OnUpdateFrame (_, _) = 
            quit

        member __.OnRenderFrame (_, _, _, _) =
            lock gate (fun () ->
                if not quit then
                    instance.DrawFrame ()
            )

// The builder class.
[<ReflectedDefinition>]
type EventuallyBuilder() =
    member x.Bind(comp, func) = Eventually.bind func comp
    member x.Return(value) = Eventually.result value
    member x.ReturnFrom(value) = value
    member x.Combine(expr1, expr2) = Eventually.combine expr1 expr2

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

    use instance = VulkanInstance.CreateWin32(hwnd, hinstance, "App", "Engine", ["VK_LAYER_KHRONOS_validation"], [VK_KHR_SWAPCHAIN_EXTENSION_NAME])
    let windowEvents = EmptyWindowEvents instance :> IWindowEvents
    windowState.WindowClosing.Add(windowEvents.OnWindowClosing)

    let window = Window (title, 30., width, height, windowEvents, windowState)

    let positions =
        [|
            Vector2 (0.f, -0.5f)
            Vector2 (0.5f, 0.5f)
            Vector2 (-0.5f, 0.5f)
        |]
    let positionsBindings = [|mkVertexInputBinding<Vector2> 0u VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX|]
    let positionsAttributes = mkVertexAttributeDescriptions<Vector2> 0u 0u
    let positionsBuffer = instance.CreateVertexBuffer<Vector2> positions.Length
    let positionsMemory = instance.AllocateMemory positionsBuffer
    instance.CopyToMemory(ReadOnlySpan positions, positionsMemory)
    let vertex =
        <@
            //let positions =
            //    [|
            //        Vector2 (0.f, -0.5f)
            //        Vector2 (0.5f, 0.5f)
            //        Vector2 (-0.5f, 0.5f)
            //    |]

            let colors =
                [|
                    Vector3 (1.f, 0.f, 0.f)
                    Vector3 (0.f, 1.f, 0.f)
                    Vector3 (0.f, 0.f, 1.f)
                |]

            

            fun (gl_VertexIndex: int) (position: Vector2) ->              
                {| 
                    gl_Position = Vector4(position, 0.f, 1.f)
                    fragColor = colors.[gl_VertexIndex]
                |}
        @>
    let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], ["GLSL.std.450"])
    let spvVertex =
        Checker.Check vertex
        |> SpirvGen.GenModule spvVertexInfo

    let fragment = <@ fun fragColor -> {| outColor = Vector4(fragColor, 1.f) |} @>
    let spvFragmentInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], (ExecutionMode.OriginUpperLeft, []))
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

    let pipelineIndex = instance.AddShader(positionsBindings, positionsAttributes, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)
    instance.RecordDraw(pipelineIndex, [|positionsBuffer|], positions.Length, 1)

    window.Start ()

    printfn "F# Vulkan ended...."
    0
