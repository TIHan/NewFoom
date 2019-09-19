module Foom.Vulkan

open System
open Foom.Game
open Foom.Win32
open FSharp.Vulkan.Interop
open FSharp.NativeInterop
open Foom.NativeCollections
open System.Runtime.CompilerServices

#nowarn "9"
#nowarn "51"

let inline private checkResult result =
    if result <> VkResult.VK_SUCCESS then
        failwithf "%A" result

let private mkApplicationInfo appNamePtr engineNamePtr =
    let mutable appInfo = VkApplicationInfo()
    appInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO
    appInfo.pApplicationName <- appNamePtr
    appInfo.applicationVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.pEngineName <- engineNamePtr
    appInfo.engineVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.apiVersion <- VK_API_VERSION_1_0
    appInfo

let private mkCreateInfo appInfo extensionCount extensionNamesPtr layerCount layerNamesPtr =
    let mutable createInfo = VkInstanceCreateInfo()
    createInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO
    createInfo.pApplicationInfo <- appInfo
    createInfo.enabledExtensionCount <- extensionCount
    createInfo.ppEnabledExtensionNames <- extensionNamesPtr
    createInfo.enabledLayerCount <- layerCount
    createInfo.ppEnabledLayerNames <- layerNamesPtr
    createInfo

let getInstanceExtensions() =
    let extensionCount = 0u

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult
    extensions

let getInstanceLayers(validationLayers: string[]) =
    let layerCount = 0u

    vkEnumerateInstanceLayerProperties(&&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateInstanceLayerProperties(&&layerCount, layersPtr) |> checkResult
    layers
    |> Array.filter (fun x -> validationLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

let createInstance (appName: string) (engineName: string) (customExtensions: string[]) (validationLayers: string[]) =
    use appNamePtr = fixed vkString appName
    use engineNamePtr = fixed vkString engineName
   
    let appInfo = mkApplicationInfo appNamePtr engineNamePtr
    let extensions = getInstanceExtensions() |> Array.map (fun x -> x.extensionName.ToString()) |> Array.append customExtensions
    let layers = getInstanceLayers validationLayers |> Array.map (fun x -> x.ToString())
    
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let createInfo = mkCreateInfo &&appInfo (uint32 extensions.Length) extensionsHandle.PtrPtr (uint32 layers.Length) layersHandle.PtrPtr

    let mutable instance = VkInstance()
    vkCreateInstance(&&createInfo, vkNullPtr, &&instance) |> checkResult
    instance