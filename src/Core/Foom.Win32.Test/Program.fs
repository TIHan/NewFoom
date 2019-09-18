open System
open Foom.Game
open Foom.Win32
open FSharp.Vulkan.Interop
open FSharp.NativeInterop

#nowarn "9"
#nowarn "51"

let inline checkResult result =
    if result <> VkResult.VK_SUCCESS then
        failwithf "%A" result

let createApplicationInfo (appName: string) (engineName: string) =
    let applicationName = NativePtr.stackalloc appName.Length |> vkMarshalString appName
    let engineName = NativePtr.stackalloc engineName.Length |> vkMarshalString engineName
    let mutable appInfo = VkApplicationInfo()
    appInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO
    appInfo.pApplicationName <- applicationName
    appInfo.applicationVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.pEngineName <- engineName
    appInfo.engineVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.apiVersion <- VK_API_VERSION_1_0
    appInfo

let createInstance appName engineName (validationLayers: string list) =
    let appInfo = createApplicationInfo appName engineName
    
    let mutable extensionCount = 0u
    let extensionNames =
        vkEnumerateInstanceExtensionProperties(vkNull, &&extensionCount, vkNull) |> checkResult
        let extensions = NativePtr.stackalloc(int extensionCount)
        vkEnumerateInstanceExtensionProperties(vkNull, &&extensionCount, extensions) |> checkResult

        let extensionNames = NativePtr.stackalloc(int extensionCount)
        vkMapPointer extensions extensionCount extensionNames (fun x -> x)
        |> vkCast

    let mutable layerCount = 0u
    let layerNames =
        vkEnumerateInstanceLayerProperties(&&layerCount, vkNull) |> checkResult
        let layers = NativePtr.stackalloc(int layerCount)
        vkEnumerateInstanceLayerProperties(&&layerCount, layers) |> checkResult

        let foundLayers = 
            validationLayers 
            |> List.choose (fun x -> 
                vkTryPick layers layerCount (fun y -> 
                    if x = y.layerName.AsString then Some y.layerName 
                    else None))

        layerCount <- uint32 foundLayers.Length
        let layers =
            let layers = NativePtr.stackalloc foundLayers.Length
            foundLayers
            |> vkMapOfSeq layers (fun x -> x)
        let layerNames = NativePtr.stackalloc foundLayers.Length
        vkMapPointer layers layerCount layerNames (fun x -> x)
        |> vkCast
        
    let mutable createInfo = VkInstanceCreateInfo()
    createInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO
    createInfo.pApplicationInfo <- &&appInfo
    createInfo.enabledExtensionCount <- extensionCount
    createInfo.ppEnabledExtensionNames <- extensionNames
    createInfo.enabledLayerCount <- layerCount
    createInfo.ppEnabledLayerNames <- layerNames

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
        createInstance "App" "Engine" ["VK_LAYER_LUNARG_standard_validation"]
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
