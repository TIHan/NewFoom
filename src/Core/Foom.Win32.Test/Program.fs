open Foom.Game
open Foom.Win32
open Foom.Vulkan
open FSharp.Vulkan.Interop
open FSharp.Window

//type Win32ServerGame() =
//    inherit AbstractServerGame()

//    override __.Update(_, _) = false
    
//type Win32ClientGame() =
//    inherit AbstractClientGame()

//    let mutable dx12 = None

//    member __.Init(width, height, hwnd, hinstance) =
//        use instance = VulkanInstance.CreateWin32(hwnd, hinstance, "App", "Engine", ["VK_LAYER_KHRONOS_validation"], [VK_KHR_SWAPCHAIN_EXTENSION_NAME])
//        ()

//    override __.PreUpdate(_, _, inputs) =
//        inputs
//        |> List.iter (printfn "%A")

//    override __.Update(_, _, _) = false

//    override __.Render(_, _) =
//        ()

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
    let fragmentBytes = System.IO.File.ReadAllBytes("triangle_fragment.spv")

    instance.AddPipeline(vertexBytes, fragmentBytes)

    window.Start ()

    printfn "F# Vulkan ended...."
    0
