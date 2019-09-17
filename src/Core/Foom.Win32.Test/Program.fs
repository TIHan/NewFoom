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

[<Flags>]
type VulkanInstanceFlags =
    | Empty                     = 0x0
    | EnableValidationLayers    = 0x1

let createInstance appName engineName flags =
    let appInfo = createApplicationInfo appName engineName
    
    let mutable extensionCount = 0u
    let extensionNames =
        vkEnumerateInstanceExtensionProperties(vkNull, &&extensionCount, vkNull) |> checkResult
        let extensions = NativePtr.stackalloc(int extensionCount)
        vkEnumerateInstanceExtensionProperties(vkNull, &&extensionCount, extensions) |> checkResult

        let mutable extensionNames = NativePtr.stackalloc(int extensionCount)
        vkMap extensions extensionCount extensionNames (fun x -> x.extensionName.UnsafePtr)

    let validationLayers = ["VK_LAYER_KHRONOS_validation"]
    let mutable layerCount = 0u
    let layerNames =
        if flags &&& VulkanInstanceFlags.EnableValidationLayers = VulkanInstanceFlags.EnableValidationLayers then
            vkEnumerateInstanceLayerProperties(&&layerCount, vkNull) |> checkResult
            let layers = NativePtr.stackalloc(int layerCount)
            vkEnumerateInstanceLayerProperties(&&layerCount, layers) |> checkResult

            let mutable layerNames = NativePtr.stackalloc(int layerCount)
            vkMap layers layerCount layerNames (fun x -> x.layerName.UnsafePtr)
        else
            vkNull
        
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
        createInstance "App" "Engine" (VulkanInstanceFlags.EnableValidationLayers)
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
