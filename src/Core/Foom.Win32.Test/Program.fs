﻿open Foom.Game
open Foom.Win32
open Foom.Vulkan
open FSharp.Vulkan.Interop
open FSharp.Window
open FSharp.SpirV
open FSharp.SpirV.Quotations
open System.Numerics
    
let fragment = 
    <@ 
        fun (fragColor: Vector3) ->
            let doot = fragColor
            {| outColor = Vector4(doot, 1.f) |}
    @>


let spvFragment = SPVGen.GenFragment fragment

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

    let vertexBytes = System.IO.File.ReadAllBytes("triangle_vertex.spv")
    let fragmentBytes =
        use ms = new System.IO.MemoryStream 100
        SpirV.serialize ms spvFragment
        let bytes = Array.zeroCreate (int ms.Length)
        ms.Position <- 0L
        ms.Read(bytes, 0, bytes.Length) |> ignore
        bytes
   // let fragmentBytes = System.IO.File.ReadAllBytes("triangle_fragment.spv")

    instance.AddPipeline(vertexBytes, fragmentBytes)

    window.Start ()

    printfn "F# Vulkan ended...."
    0
