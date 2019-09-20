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

let private getSuitablePhysicalDevice instance =
    let deviceCount = 0u

    vkEnumeratePhysicalDevices(instance, &&deviceCount, vkNullPtr) |> checkResult

    if deviceCount = 0u then
        failwith "Unable to find GPUs with Vulkan support."

    let devices = Array.zeroCreate (int deviceCount)
    let pDevices = fixed devices
    vkEnumeratePhysicalDevices(instance, &&deviceCount, pDevices) |> checkResult

    let deviceOpt =
        devices
        |> Array.tryFind (fun device ->
            let deviceProperties = VkPhysicalDeviceProperties()
            vkGetPhysicalDeviceProperties(device, &&deviceProperties)

            let deviceFeatures = VkPhysicalDeviceFeatures()
            vkGetPhysicalDeviceFeatures(device, &&deviceFeatures)

            deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU &&
            deviceFeatures.geometryShader = VK_TRUE
        )

    match deviceOpt with
    | Some device -> device
    | _ -> failwith "Unable to find suitable GPU."

let private getInstanceExtension<'T when 'T :> Delegate> instance =
    use pName = fixed vkBytesOfString typeof<'T>.Name
    vkGetInstanceProcAddr(instance, pName) 
    |> vkDelegateOfFunctionPointer<'T>

let private mkInstance (appName: string) (engineName: string) (validationLayers: string[]) =
    use appNamePtr = fixed vkBytesOfString appName
    use engineNamePtr = fixed vkBytesOfString engineName
   
    let appInfo = mkApplicationInfo appNamePtr engineNamePtr
    let extensions = getInstanceExtensions () |> Array.map (fun x -> x.extensionName.ToString())
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
    createInfo.messageSeverity <- VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT |||
                                  VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT |||
                                  VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT
    createInfo.messageType <- VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT |||
                              VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT |||
                              VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT
    createInfo.pfnUserCallback <- debugCallback
    createInfo.pUserData <- IntPtr.Zero // optional

    let createDebugUtilsMessenger = getInstanceExtension<vkCreateDebugUtilsMessengerEXT> instance
    let debugMessenger = VkDebugUtilsMessengerEXT ()
    createDebugUtilsMessenger.Invoke(instance, &&createInfo, vkNullPtr, &&debugMessenger) |> checkResult
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
                    let destroyDebugUtilsMessenger = getInstanceExtension<vkDestroyDebugUtilsMessengerEXT> instance
                    destroyDebugUtilsMessenger.Invoke(instance, debugMessenger, vkNullPtr)

                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

let init appName engineName validationLayers =
    let instance = mkInstance appName engineName validationLayers

    let debugCallbackHandle, debugCallback = 
        PFN_vkDebugUtilsMessengerCallbackEXT.Create(fun messageSeverity messageType pCallbackData pUserData ->
            let callbackData = NativePtr.read pCallbackData
            let str = vkStringOfBytePtr callbackData.pMessage
            printfn "validation layer: %s" str

            VK_FALSE
        )
    let debugMessenger = mkDebugMessenger instance debugCallback
    let device = getSuitablePhysicalDevice instance

    new VulkanInstance(instance, debugMessenger, [|debugCallbackHandle|])