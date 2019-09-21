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

let private getDeviceExtensions physicalDevice deviceExtensions =
    let extensionCount = 0u

    vkEnumerateDeviceExtensionProperties(physicalDevice, vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateDeviceExtensionProperties(physicalDevice, vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult

    deviceExtensions
    |> Array.iter (fun x ->
        if extensions |> Array.exists (fun y -> y.extensionName.ToString() = x) |> not then
            failwithf "Unable to find device extension, %s." x
    )

    extensions
    |> Array.filter (fun x -> deviceExtensions |> Array.exists (fun y -> x.extensionName.ToString() = y))

let private getDeviceLayers physicalDevice validationLayers =
    let layerCount = 0u

    vkEnumerateDeviceLayerProperties(physicalDevice, &&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateDeviceLayerProperties(physicalDevice, &&layerCount, layersPtr) |> checkResult

    validationLayers
    |> Array.iter (fun x ->
        if layers |> Array.exists (fun y -> y.layerName.ToString() = x) |> not then
            failwithf "Unable to find device layer, %s." x
    )

    layers
    |> Array.filter (fun x -> validationLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

type private QueueFamilyIndices =
    {
        graphicsFamily: uint32 option
        presentFamily: uint32 option
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
                        { indices with presentFamily = Some (uint32 i) }
                    else
                        indices

                let i = uint32 i
                { indices with graphicsFamily = Some (uint32 i) }
            else
                indices
        else
            indices) { graphicsFamily = None; presentFamily = None }

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

let private mkLogicalDevice physicalDevice indices validationLayers deviceExtensions =
    let queueCreateInfos = 
        [|indices.graphicsFamily.Value; indices.presentFamily.Value|]
        |> Array.distinct // we need to be distinct so we do not create duplicate create infos
        |> Array.map (fun familyIndex ->
            let queuePriority = 1.f
            mkDeviceQueueCreateInfo familyIndex &&queuePriority
        )
    let extensions = getDeviceExtensions physicalDevice deviceExtensions |> Array.map (fun x -> x.extensionName.ToString())
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

type SwapChainSupportDetails =
    {
        capabilities: VkSurfaceCapabilitiesKHR
        formats: VkSurfaceFormatKHR []
        presentModes: VkPresentModeKHR []
    }

let private querySwapChainSupport physicalDevice surface =
    let capabilities = VkSurfaceCapabilitiesKHR ()
    vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physicalDevice, surface, &&capabilities) |> checkResult

    let formatCount = 0u
    vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, &&formatCount, vkNullPtr) |> checkResult

    let formats = Array.zeroCreate (int formatCount)
    use pFormats = fixed formats
    vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, &&formatCount, pFormats) |> checkResult

    let presentModeCount = 0u
    vkGetPhysicalDeviceSurfacePresentModesKHR(physicalDevice, surface, &&presentModeCount, vkNullPtr) |> checkResult

    let presentModes = Array.zeroCreate (int presentModeCount)
    use pPresentModes = fixed presentModes
    vkGetPhysicalDeviceSurfacePresentModesKHR(physicalDevice, surface, &&presentModeCount, pPresentModes) |> checkResult

    {
        capabilities = capabilities
        formats = formats
        presentModes = presentModes
    }

let private findSwapSurfaceFormat (formats: VkSurfaceFormatKHR []) =
    formats
    |> Array.find (fun x -> x.format = VkFormat.VK_FORMAT_B8G8R8A8_UNORM && x.colorSpace = VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR)

let private getSwapPresentMode (formats: VkPresentModeKHR []) =
    formats
    |> Array.tryFind (fun x -> x = VkPresentModeKHR.VK_PRESENT_MODE_MAILBOX_KHR)
    |> Option.defaultValue VkPresentModeKHR.VK_PRESENT_MODE_FIFO_KHR // default is guaranteed

let private getSwapExtent (capabilities: VkSurfaceCapabilitiesKHR) =
    if capabilities.currentExtent.width <> UInt32.MaxValue then
        capabilities.currentExtent
    else
        let width = 
            [capabilities.minImageExtent.width; capabilities.maxImageExtent.width; 1280u]
            |> List.max

        let height =
            [capabilities.minImageExtent.width; capabilities.maxImageExtent.width; 720u]
            |> List.max

        VkExtent2D (
            width = width,
            height = height
        )

let private mkSwapChain physicalDevice device surface indices =
    let swapChainSupport = querySwapChainSupport physicalDevice surface

    let surfaceFormat = findSwapSurfaceFormat swapChainSupport.formats
    let presentMode = getSwapPresentMode swapChainSupport.presentModes
    let extent = getSwapExtent swapChainSupport.capabilities

    // Simply sticking to this minimum means that we may sometimes have to wait on the driver to complete internal operations before we can acquire another image to render to. 
    // Therefore it is recommended to request at least one more image than the minimum
    let imageCount = swapChainSupport.capabilities.minImageCount + 1u

    let imageCount =
        // We should also make sure to not exceed the maximum number of images while doing this, where 0 is a special value that means that there is no maximum
        if swapChainSupport.capabilities.maxImageCount > 0u && imageCount > swapChainSupport.capabilities.maxImageCount then
            swapChainSupport.capabilities.maxImageCount
        else
            imageCount

    let queueFamilyIndices = [|indices.graphicsFamily.Value;indices.presentFamily.Value|]
    let isConcurrent = indices.graphicsFamily.Value <> indices.presentFamily.Value
    use pQueueFamilyIndices = fixed queueFamilyIndices
    let createInfo = 
        VkSwapchainCreateInfoKHR (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SWAPCHAIN_CREATE_INFO_KHR,
            surface = surface,
            minImageCount = imageCount,
            imageFormat = surfaceFormat.format,
            imageColorSpace = surfaceFormat.colorSpace,
            imageExtent = extent,
            imageArrayLayers = 1u,
            imageUsage = VkImageUsageFlags.VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT,
            imageSharingMode = (if isConcurrent then VkSharingMode.VK_SHARING_MODE_CONCURRENT else VkSharingMode.VK_SHARING_MODE_EXCLUSIVE),
            queueFamilyIndexCount = (if isConcurrent then 2u else 0u (* optional *)),
            pQueueFamilyIndices = (if isConcurrent then pQueueFamilyIndices else vkNullPtr (* optional *)),
            preTransform = swapChainSupport.capabilities.currentTransform,
            compositeAlpha = VkCompositeAlphaFlagsKHR.VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR,
            presentMode = presentMode,
            clipped = VK_TRUE,
            oldSwapchain = VK_NULL_HANDLE
        )

    let swapChain = VkSwapchainKHR ()
    vkCreateSwapchainKHR(device, &&createInfo, vkNullPtr, &&swapChain) |> checkResult
    swapChain

[<Sealed>]
type VulkanInstance (instance: VkInstance, debugMessenger: VkDebugUtilsMessengerEXT, device: VkDevice, surface: VkSurfaceKHR, swapChain: VkSwapchainKHR, graphicsQueue: VkQueue, presentQueue: VkQueue, handles: GCHandle[]) =

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

                vkDestroySwapchainKHR(device, swapChain, vkNullPtr)
                vkDestroyDevice(device, vkNullPtr)

                if debugMessenger <> IntPtr.Zero then
                    let destroyDebugUtilsMessenger = getInstanceExtension<vkDestroyDebugUtilsMessengerEXT> instance
                    destroyDebugUtilsMessenger.Invoke(instance, debugMessenger, vkNullPtr)

                vkDestroySurfaceKHR(instance, surface, vkNullPtr)
                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

    static member Create(mkSurface, appName, engineName, validationLayers, deviceExtensions) =
        let validationLayers = validationLayers |> Array.ofList
        let deviceExtensions = deviceExtensions |> Array.ofList
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
        let device = mkLogicalDevice physicalDevice indices validationLayers deviceExtensions
        let swapChain = mkSwapChain physicalDevice device surface indices

        let graphicsQueue = mkQueue device indices.graphicsFamily.Value
        let presentQueue = mkQueue device indices.presentFamily.Value

        new VulkanInstance(instance, debugMessenger, device, surface, swapChain, graphicsQueue, presentQueue, [|debugCallbackHandle|])

    static member CreateWin32(hwnd, hinstance, appName, engineName, validationLayers, deviceExtensions) =
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

        VulkanInstance.Create(mkSurface, appName, engineName, validationLayers, deviceExtensions)
