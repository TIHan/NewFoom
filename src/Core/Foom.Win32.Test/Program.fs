open System
open Foom.Game
open Foom.Win32
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let inline checkResult result =
    if result <> VkResult.VK_SUCCESS then
        failwithf "%A" result

let createInstance() =
    let mutable appInfo = VkApplicationInfo()
    appInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO
    appInfo.pApplicationName <- vkMarshalString "Win32Game"
    appInfo.applicationVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.pEngineName <- vkMarshalString "Win32Engine"
    appInfo.engineVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.apiVersion <- VK_API_VERSION_1_0

    let mutable extensionCount = 0u
    let extensions =
        vkEnumerateInstanceExtensionProperties(vkNull, &&extensionCount, vkNull) |> checkResult
        let extensions = vkCreateUnmanagedArray extensionCount
        vkEnumerateInstanceExtensionProperties(vkNull, &&extensionCount, extensions) |> checkResult
        extensions

    let mutable createInfo = VkInstanceCreateInfo()
    createInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO
    createInfo.pApplicationInfo <- vkMarshal appInfo
    createInfo.enabledExtensionCount <- extensionCount
    createInfo.ppEnabledExtensionNames <- vkCast extensions
    createInfo.enabledLayerCount <- 0u

    let mutable instance = VkInstance()
    vkCreateInstance(&&createInfo, vkNull, &&instance) |> checkResult
    instance

type Win32ServerGame() =
    inherit AbstractServerGame()

    override __.Update(_, _) = false
    
type Win32ClientGame() =
    inherit AbstractClientGame()

    let mutable dx12 = None

    member __.Init(width, height, hwnd) =
        createInstance()
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
    clGame.Init(game.Width, game.Height, game.Hwnd)
    game.Start()
    0
