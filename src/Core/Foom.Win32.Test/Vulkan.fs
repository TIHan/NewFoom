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
    VkApplicationInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
        pApplicationName = appNamePtr,
        applicationVersion = VK_MAKE_VERSION(1u, 0u, 0u),
        pEngineName = engineNamePtr,
        engineVersion = VK_MAKE_VERSION(1u, 0u, 0u),
        apiVersion = VK_API_VERSION_1_0
    )

let private mkInstanceCreateInfo appInfo extensionCount extensionNamesPtr layerCount layerNamesPtr =
    VkInstanceCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
        pApplicationInfo = appInfo,
        enabledExtensionCount = extensionCount,
        ppEnabledExtensionNames = extensionNamesPtr,
        enabledLayerCount = layerCount,
        ppEnabledLayerNames = layerNamesPtr
    )

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

let private getInstanceExtensions() =
    let extensionCount = 0u

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult
    extensions

let private getInstanceLayers validationLayers =
    let layerCount = 0u

    vkEnumerateInstanceLayerProperties(&&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateInstanceLayerProperties(&&layerCount, layersPtr) |> checkResult
    layers
    |> Array.filter (fun x -> validationLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

let private mkInstance appName engineName validationLayers =
    use appNamePtr = fixed vkBytesOfString appName
    use engineNamePtr = fixed vkBytesOfString engineName
   
    let appInfo = mkApplicationInfo appNamePtr engineNamePtr
    let extensions = getInstanceExtensions () |> Array.map (fun x -> x.extensionName.ToString())
    let layers = getInstanceLayers validationLayers |> Array.map (fun x -> x.layerName.ToString())
    
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let createInfo = mkInstanceCreateInfo &&appInfo (uint32 extensions.Length) extensionsHandle.PtrPtr (uint32 layers.Length) layersHandle.PtrPtr

    let instance = VkInstance()
    vkCreateInstance(&&createInfo, vkNullPtr, &&instance) |> checkResult
    instance

let private mkDebugMessengerCreateInfo debugCallback =
    VkDebugUtilsMessengerCreateInfoEXT (
        sType = VkStructureType.VK_STRUCTURE_TYPE_DEBUG_UTILS_MESSENGER_CREATE_INFO_EXT,
        messageSeverity = (VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT |||
                           VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT |||
                           VkDebugUtilsMessageSeverityFlagsEXT.VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT),
        messageType = (VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT |||
                       VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT |||
                       VkDebugUtilsMessageTypeFlagsEXT.VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT),
        pfnUserCallback = debugCallback,
        pUserData = IntPtr.Zero // optional
    )
let private mkDebugMessenger instance debugCallback =
    let createInfo = mkDebugMessengerCreateInfo debugCallback

    let createDebugUtilsMessenger = getInstanceExtension<vkCreateDebugUtilsMessengerEXT> instance
    let debugMessenger = VkDebugUtilsMessengerEXT ()
    createDebugUtilsMessenger.Invoke(instance, &&createInfo, vkNullPtr, &&debugMessenger) |> checkResult
    debugMessenger

let private getDeviceExtensions physicalDevice =
    let extensionCount = 0u

    vkEnumerateDeviceExtensionProperties(physicalDevice, vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateDeviceExtensionProperties(physicalDevice, vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult
    extensions
    |> Array.filter (fun x -> extensions |> Array.exists (fun y -> x.extensionName.ToString() <> "VK_NVX_binary_import"))

let private getDeviceLayers physicalDevice validationLayers =
    let layerCount = 0u

    vkEnumerateDeviceLayerProperties(physicalDevice, &&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateDeviceLayerProperties(physicalDevice, &&layerCount, layersPtr) |> checkResult
    layers
    |> Array.filter (fun x -> validationLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

[<Struct>]
type private QueueFamilyIndices =
    {
        graphicsFamily: uint32 voption
        presentFamily: uint32 voption
    }

    member this.IsComplete = this.graphicsFamily.IsSome && this.presentFamily.IsSome

let private getPhysicalDeviceQueueFamilies physicalDevice surface =
    let queueFamilyCount = 0u

    vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &&queueFamilyCount, vkNullPtr)

    let queueFamilies = Array.zeroCreate (int queueFamilyCount)
    use pQueueFamilies = fixed queueFamilies
    vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &&queueFamilyCount, pQueueFamilies)
    
    queueFamilies
    |> Array.mapi (fun i x -> (i, x))
    |> Array.fold (fun indices (i, x) ->
        if x.queueCount > 0u then
            if x.queueFlags &&& VkQueueFlags.VK_QUEUE_GRAPHICS_BIT = VkQueueFlags.VK_QUEUE_GRAPHICS_BIT then

                let indices =
                    let presentSupport = VK_FALSE      
                    vkGetPhysicalDeviceSurfaceSupportKHR(physicalDevice, uint32 i, surface, &&presentSupport) |> checkResult
                    if presentSupport = VK_TRUE then
                        { indices with presentFamily = ValueSome (uint32 i) }
                    else
                        indices

                let i = uint32 i
                { indices with graphicsFamily = ValueSome (uint32 i) }
            else
                indices
        else
            indices) { graphicsFamily = ValueNone; presentFamily = ValueNone }

let private mkDeviceQueueCreateInfo queueFamilyIndex pQueuePriorities =
    VkDeviceQueueCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
        queueFamilyIndex = queueFamilyIndex,
        queueCount = 1u,
        pQueuePriorities = pQueuePriorities
    )

let private mkDeviceCreateInfo pQueueCreateInfos queueCreateInfoCount pEnabledFeatures enabledExtensionCount ppEnabledExtensionNames enabledLayerCount ppEnabledLayerNames =
    VkDeviceCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO,
        pQueueCreateInfos = pQueueCreateInfos,
        queueCreateInfoCount = queueCreateInfoCount,
        pEnabledFeatures = pEnabledFeatures,
        enabledExtensionCount = enabledExtensionCount,
        ppEnabledExtensionNames = ppEnabledExtensionNames,
        enabledLayerCount = enabledLayerCount,
        ppEnabledLayerNames = ppEnabledLayerNames
    )

let private mkLogicalDevice physicalDevice indices validationLayers =
    let queueCreateInfos = 
        [|indices.graphicsFamily.Value; indices.presentFamily.Value|]
        |> Array.distinct // we need to be distinct so we do not create duplicate create infos
        |> Array.map (fun familyIndex ->
            let queuePriority = 1.f
            mkDeviceQueueCreateInfo familyIndex &&queuePriority
        )
    let extensions = getDeviceExtensions physicalDevice |> Array.map (fun x -> x.extensionName.ToString())
    let layers = getDeviceLayers physicalDevice validationLayers |> Array.map (fun x -> x.layerName.ToString())

    let deviceFeatures = VkPhysicalDeviceFeatures()
    vkGetPhysicalDeviceFeatures(physicalDevice, &&deviceFeatures)

    use pQueueCreateInfos = fixed queueCreateInfos
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let createInfo = 
        mkDeviceCreateInfo 
            pQueueCreateInfos (uint32 queueCreateInfos.Length)
            &&deviceFeatures 
            (uint32 extensions.Length) extensionsHandle.PtrPtr
            (uint32 layers.Length) layersHandle.PtrPtr

    let device = VkDevice()
    vkCreateDevice(physicalDevice, &&createInfo, vkNullPtr, &&device) |> checkResult
    device

let private mkQueue device familyIndex =
    let queue = VkQueue()
    vkGetDeviceQueue(device, familyIndex, 0u, &&queue)
    queue

[<Sealed>]
type VulkanInstance (instance: VkInstance, debugMessenger: VkDebugUtilsMessengerEXT, device: VkDevice, graphicsQueue: VkQueue, presentQueue: VkQueue, surface: VkSurfaceKHR, handles: GCHandle[]) =

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

                vkDestroyDevice(device, vkNullPtr)

                if debugMessenger <> IntPtr.Zero then
                    let destroyDebugUtilsMessenger = getInstanceExtension<vkDestroyDebugUtilsMessengerEXT> instance
                    destroyDebugUtilsMessenger.Invoke(instance, debugMessenger, vkNullPtr)

                vkDestroySurfaceKHR(instance, surface, vkNullPtr)
                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

    static member Create(mkSurface, appName, engineName, validationLayers) =
        let validationLayers = validationLayers |> Array.ofList
        let debugCallbackHandle, debugCallback = 
            PFN_vkDebugUtilsMessengerCallbackEXT.Create(fun messageSeverity messageType pCallbackData pUserData ->
                let callbackData = NativePtr.read pCallbackData
                let str = vkStringOfBytePtr callbackData.pMessage
                printfn "validation layer: %s" str

                VK_FALSE
            )

        let instance = mkInstance appName engineName validationLayers
        let surface = mkSurface instance // must create surface right after instance - influences device calls
        let debugMessenger = mkDebugMessenger instance debugCallback
        let physicalDevice = getSuitablePhysicalDevice instance
        let indices = getPhysicalDeviceQueueFamilies physicalDevice surface
        let device = mkLogicalDevice physicalDevice indices validationLayers

        let graphicsQueue = mkQueue device indices.graphicsFamily.Value
        let presentQueue = mkQueue device indices.presentFamily.Value

        new VulkanInstance(instance, debugMessenger, device, graphicsQueue, presentQueue, surface, [|debugCallbackHandle|])

    static member CreateWin32(hwnd, hinstance, appName, engineName, validationLayers) =
        let mkSurface =
            fun instance ->
                let createInfo = 
                    VkWin32SurfaceCreateInfoKHR (
                        sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
                        hwnd = hwnd,
                        hinstance = hinstance
                    )
                let surface = VkSurfaceKHR ()
                vkCreateWin32SurfaceKHR(instance, &&createInfo, vkNullPtr, &&surface) |> checkResult
                surface

        VulkanInstance.Create(mkSurface, appName, engineName, validationLayers)
