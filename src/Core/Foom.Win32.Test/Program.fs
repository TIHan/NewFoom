open Foom.Game
open Foom.Win32
open Foom.Vulkan
open FSharp.Vulkan.Interop

type Win32ServerGame() =
    inherit AbstractServerGame()

    override __.Update(_, _) = false
    
type Win32ClientGame() =
    inherit AbstractClientGame()

    let mutable dx12 = None

    member __.Init(width, height, hwnd, hinstance) =
        use instance = VulkanInstance.CreateWin32(hwnd, hinstance, "App", "Engine", ["VK_LAYER_KHRONOS_validation"], [VK_KHR_SWAPCHAIN_EXTENSION_NAME])
        ()

    override __.PreUpdate(_, _, inputs) =
        inputs
        |> List.iter (printfn "%A")

    override __.Update(_, _, _) = false

    override __.Render(_, _) =
        ()

[<EntryPoint>]
let main argv =
    let clGame = Win32ClientGame()
    let game = Win32Game("F# Game", Win32ServerGame(), clGame, 30.)
    clGame.Init(game.Width, game.Height, game.Hwnd, game.Hinstance)
    game.Start()
    0
