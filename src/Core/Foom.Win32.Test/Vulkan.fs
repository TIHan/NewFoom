module Foom.Vulkan

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

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

let private mkInstanceCreateInfo appInfo extensionCount extensionNamesPtr layerCount layerNamesPtr =
    let mutable createInfo = VkInstanceCreateInfo()
    createInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO
    createInfo.pApplicationInfo <- appInfo
    createInfo.enabledExtensionCount <- extensionCount
    createInfo.ppEnabledExtensionNames <- extensionNamesPtr
    createInfo.enabledLayerCount <- layerCount
    createInfo.ppEnabledLayerNames <- layerNamesPtr
    createInfo

let private getInstanceExtensions() =
    let extensionCount = 0u

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult
    extensions

let private getInstanceLayers(validationLayers: string[]) =
    let layerCount = 0u

    vkEnumerateInstanceLayerProperties(&&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateInstanceLayerProperties(&&layerCount, layersPtr) |> checkResult
    layers
    |> Array.filter (fun x -> validationLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

let private mkInstance (appName: string) (engineName: string) (customExtensions: string[]) (validationLayers: string[]) =
    use appNamePtr = fixed vkString appName
    use engineNamePtr = fixed vkString engineName
   
    let appInfo = mkApplicationInfo appNamePtr engineNamePtr
    let extensions = getInstanceExtensions() |> Array.map (fun x -> x.extensionName.ToString()) |> Array.append customExtensions
    let layers = getInstanceLayers validationLayers |> Array.map (fun x -> x.layerName.ToString())
    
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let createInfo = mkInstanceCreateInfo &&appInfo (uint32 extensions.Length) extensionsHandle.PtrPtr (uint32 layers.Length) layersHandle.PtrPtr

    let mutable instance = VkInstance()
    vkCreateInstance(&&createInfo, vkNullPtr, &&instance) |> checkResult
    instance

let private mkDebugMessenger instance debugCallback =
    let mutable createInfo = VkDebugUtilsMessengerCreateInfoEXT ()
    createInfo.sType <- VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT
    createInfo.messageSeverity <- VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT ||| 
                                  VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT |||
                                  VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT
    createInfo.messageType <- VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT |||
                              VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT |||
                              VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT
    createInfo.pfnUserCallback <- debugCallback
    createInfo.pUserData <- IntPtr.Zero // optional

    use pName = fixed vkString "vkCreateDebugUtilsMessengerEXT"
    vkGetInstanceProcAddr(instance, pName) |> ignore
    let debugMessenger = VkDebugUtilsMessengerEXT ()
    vkCreateDebugUtilsMessengerEXT(instance, &&createInfo, vkNullPtr, &&debugMessenger) |> checkResult
    debugMessenger

[<Sealed>]
type VulkanInstance (instance: VkInstance, debugMessenger: VkDebugUtilsMessengerEXT, handles: GCHandle[]) =

    let mutable isDisposed = 0

    member __.Instance = instance

    member __.DebugMessenger = debugMessenger

    override x.Finalize() =
        (x :> IDisposable).Dispose ()

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                failwith "VulkanInstance already disposed"
            else
                GC.SuppressFinalize x

                if debugMessenger <> IntPtr.Zero then
                    vkDestroyDebugUtilsMessengerEXT(instance, debugMessenger, vkNullPtr)

                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

let init appName engineName customExtensions validationLayers =
    let instance = mkInstance appName engineName customExtensions validationLayers

    let debugCallbackHandle, debugCallback = 
        PFN_vkDebugUtilsMessengerCallbackEXT.Create(fun messageSeverity messageType pCallbackData pUserData ->
            let callbackData = NativePtr.read pCallbackData
            let mutable length = 0
            let mutable pMessage = callbackData.pMessage
            let mutable isNullTerm = false
            while not isNullTerm do
                if NativePtr.read pMessage = 0uy then
                    isNullTerm <- true
                else
                    length <- length + 1
                    pMessage <- NativePtr.add pMessage 1
            let str = System.Text.Encoding.UTF8.GetString(callbackData.pMessage, length)
            printfn "validation layer: %s" str

            VK_TRUE
        )
    let debugMessenger = mkDebugMessenger instance debugCallback

    new VulkanInstance(instance, debugMessenger, [|debugCallbackHandle|])