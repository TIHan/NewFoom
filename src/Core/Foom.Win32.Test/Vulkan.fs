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

let private mkApplicationInfo appName engineName =
    let mutable appInfo = VkApplicationInfo()
    appInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO
    appInfo.pApplicationName <- appName
    appInfo.applicationVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.pEngineName <- engineName
    appInfo.engineVersion <- VK_MAKE_VERSION(1u, 0u, 0u)
    appInfo.apiVersion <- VK_API_VERSION_1_0
    appInfo

let private mkCreateInfo appInfo (extensionNames: NativeArray<nativeptr<byte>>) (layerNames: NativeArray<nativeptr<byte>>) =
    let mutable createInfo = VkInstanceCreateInfo()
    createInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO
    createInfo.pApplicationInfo <- appInfo
    createInfo.enabledExtensionCount <- uint32 extensionNames.Length
    createInfo.ppEnabledExtensionNames <- extensionNames.Buffer
    createInfo.enabledLayerCount <- uint32 layerNames.Length
    createInfo.ppEnabledLayerNames <- vkCast layerNames.Buffer
    createInfo

let createInstance (appName: string) (engineName: string) (customExtensions: string list) (validationLayers: string list) =
    let customExtensions = customExtensions |> Array.ofList
    let validationLayers = validationLayers |> Array.ofList

    // Extensions
    
    let extensionCount = 0u

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    use extensions = new NativeArray<VkExtensionProperties>(int extensionCount)

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, extensions.Buffer) |> checkResult

    let extensionCount = extensionCount + uint32 customExtensions.Length

    use customExtensionNames = new NativeArray<VkFixedArray_byte_256>(customExtensions.Length)
    for i = 0 to customExtensions.Length - 1 do
        vkMarshalString customExtensions.[i] (customExtensionNames.ToPointer<byte> i) |> ignore

    use extensionNames = new NativeArray<nativeptr<byte>>(int extensionCount)
    for i = 0 to extensions.Length - 1 do
        extensionNames.[i] <- extensions.ToPointer i
    for i = 0 to customExtensions.Length - 1 do
        extensionNames.[i + extensions.Length] <- customExtensionNames.ToPointer i

    // Layers

    let layerCount = 0u

    vkEnumerateInstanceLayerProperties(&&layerCount, vkNullPtr) |> checkResult

    use layers = new NativeArray<VkLayerProperties>(int layerCount)

    vkEnumerateInstanceLayerProperties(&&layerCount, layers.Buffer) |> checkResult

    let layers2 = 
        validationLayers
        |> Array.filter (fun x -> 
            let mutable exists = false
            let mutable i = 0
            while not exists && i < layers.Length do
                if layers.[i].layerName.ToString() = x then
                    exists <- true
                i <- i + 1
            exists
        )

    use layerNames = new NativeArray<VkFixedArray_byte_256>(layers2.Length)
    for i = 0 to layerNames.Length - 1 do
        vkMarshalString layers2.[i] (layerNames.ToPointer<byte> i) |> ignore
    use layerNames2 = new NativeArray<nativeptr<byte>>(layers2.Length)
    for i = 0 to layerNames2.Length - 1 do
        layerNames2.[i] <- layerNames.ToPointer i

    // Create instance

    let appName2 = NativePtr.stackalloc<byte> appName.Length
    vkMarshalString appName appName2 appName.Length

    let engineName2 = NativePtr.stackalloc<byte> engineName.Length
    vkMarshalString engineName engineName2 engineName.Length

    let appInfo = mkApplicationInfo appName2 engineName2
    let createInfo = mkCreateInfo &&appInfo extensionNames layerNames2

    let mutable instance = VkInstance()
    vkCreateInstance(&&createInfo, vkNullPtr, &&instance) |> checkResult
    instance