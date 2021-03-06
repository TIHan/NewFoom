﻿module internal FsGame.Graphics.Vulkan.InternalDeviceHelpers

open System
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let mkApplicationInfo appNamePtr engineNamePtr =
    VkApplicationInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_APPLICATION_INFO,
        pApplicationName = appNamePtr,
        applicationVersion = VK_MAKE_VERSION(1u, 0u, 0u),
        pEngineName = engineNamePtr,
        engineVersion = VK_MAKE_VERSION(1u, 0u, 0u),
        apiVersion = VK_MAKE_VERSION(1u, 1u, 0u)
    )

let getInstanceExtension<'T when 'T :> Delegate> instance =
    use pName = fixed vkBytesOfString typeof<'T>.Name
    vkGetInstanceProcAddr(instance, pName) 
    |> vkDelegateOfFunctionPointer<'T>

let getInstanceExtensions() =
    let mutable extensionCount = 0u

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult
    extensions

let getInstanceLayers validationLayers =
    let mutable layerCount = 0u

    vkEnumerateInstanceLayerProperties(&&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateInstanceLayerProperties(&&layerCount, layersPtr) |> checkResult
    layers
    |> Array.filter (fun x -> validationLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

let private mkInstanceCreateInfo appInfo extensionCount extensionNamesPtr layerCount layerNamesPtr =
    VkInstanceCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO,
        pApplicationInfo = appInfo,
        enabledExtensionCount = extensionCount,
        ppEnabledExtensionNames = extensionNamesPtr,
        enabledLayerCount = layerCount,
        ppEnabledLayerNames = layerNamesPtr
    )

let mkInstance appName engineName validationLayers =
    use appNamePtr = fixed vkBytesOfString appName
    use engineNamePtr = fixed vkBytesOfString engineName
   
    let mutable appInfo = mkApplicationInfo appNamePtr engineNamePtr
    let extensions = getInstanceExtensions () |> Array.map (fun x -> x.extensionName.ToString()) |> Array.filter (fun x -> x <> VK_NV_EXTERNAL_MEMORY_CAPABILITIES_EXTENSION_NAME)
    let layers = getInstanceLayers validationLayers |> Array.map (fun x -> x.layerName.ToString())
    
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let mutable createInfo = mkInstanceCreateInfo &&appInfo (uint32 extensions.Length) extensionsHandle.PtrPtr (uint32 layers.Length) layersHandle.PtrPtr

    let mutable instance = VkInstance()
    vkCreateInstance(&&createInfo, vkNullPtr, &&instance) |> checkResult
    instance

let mkDebugMessengerCreateInfo debugCallback =
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

let mkDebugMessenger instance debugCallback =
    let mutable createInfo = mkDebugMessengerCreateInfo debugCallback

    let createDebugUtilsMessenger = getInstanceExtension<vkCreateDebugUtilsMessengerEXT> instance
    let mutable debugMessenger = VkDebugUtilsMessengerEXT ()
    createDebugUtilsMessenger.Invoke(instance, &&createInfo, vkNullPtr, &&debugMessenger) |> checkResult
    debugMessenger

let isDiscreteGpu (deviceProperties: VkPhysicalDeviceProperties) =
    deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU

let isDiscreteGpuWithGeometryShader deviceProperties (deviceFeatures: VkPhysicalDeviceFeatures) =
    isDiscreteGpu deviceProperties && deviceFeatures.geometryShader = VK_TRUE

let isIntegratedGpu (deviceProperties: VkPhysicalDeviceProperties) =
    deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU

let getSuitablePhysicalDevice instance =
    let mutable deviceCount = 0u

    vkEnumeratePhysicalDevices(instance, &&deviceCount, vkNullPtr) |> checkResult

    if deviceCount = 0u then
        failwith "Unable to find GPUs with Vulkan support."

    let devices = Array.zeroCreate (int deviceCount)
    use pDevices = fixed devices
    vkEnumeratePhysicalDevices(instance, &&deviceCount, pDevices) |> checkResult

    let deviceOpt =
        devices
        |> Array.map (fun device ->
            let mutable deviceProperties = VkPhysicalDeviceProperties()
            vkGetPhysicalDeviceProperties(device, &&deviceProperties)

            let mutable deviceFeatures = VkPhysicalDeviceFeatures()
            vkGetPhysicalDeviceFeatures(device, &&deviceFeatures)

            (device, deviceProperties, deviceFeatures)
        )
        |> Array.filter (fun (_, deviceProperties, _) -> isDiscreteGpu deviceProperties || isIntegratedGpu deviceProperties)
        |> Array.sortBy (fun (_, deviceProperties, deviceFeatures) ->
            if isDiscreteGpuWithGeometryShader deviceProperties deviceFeatures then 0
            elif isDiscreteGpu deviceProperties then 1
            elif isIntegratedGpu deviceProperties then 2
            else Int32.MaxValue                
        )
        |> Array.tryHead
        |> Option.map (fun (device, _, _) -> device)

    match deviceOpt with
    | Some device -> device 
    | _ -> failwith "Unable to find suitable GPU."

type QueueFamilyIndices =
    {
        graphicsFamily: uint32 option
        presentFamily: uint32 option
        computeFamily: uint32 option
        transferFamily: uint32 option
    }

    member x.HasGraphics = x.graphicsFamily.IsSome && x.presentFamily.IsSome

    member x.HasCompute = x.computeFamily.IsSome

    member x.HasTransfer = x.transferFamily.IsSome

let getPhysicalDeviceQueueFamilies physicalDevice (surfaceOpt: VkSurfaceKHR option) =
    let mutable queueFamilyCount = 0u

    vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &&queueFamilyCount, vkNullPtr)

    let queueFamilies = Array.zeroCreate (int queueFamilyCount)
    use pQueueFamilies = fixed queueFamilies
    vkGetPhysicalDeviceQueueFamilyProperties(physicalDevice, &&queueFamilyCount, pQueueFamilies)
    
    queueFamilies
    |> Array.mapi (fun i x -> (i, x))
    |> Array.fold (fun indices (i, x) ->
        if x.queueCount > 0u then
            if x.queueFlags &&& VkQueueFlags.VK_QUEUE_GRAPHICS_BIT = VkQueueFlags.VK_QUEUE_GRAPHICS_BIT && surfaceOpt.IsSome then

                let indices =
                    let mutable presentSupport = VK_FALSE      
                    vkGetPhysicalDeviceSurfaceSupportKHR(physicalDevice, uint32 i, surfaceOpt.Value, &&presentSupport) |> checkResult
                    if presentSupport = VK_TRUE then
                        { indices with presentFamily = Some (uint32 i) }
                    else
                        indices

                let i = uint32 i
                { indices with graphicsFamily = Some (uint32 i) }
            elif x.queueFlags &&& VkQueueFlags.VK_QUEUE_COMPUTE_BIT = VkQueueFlags.VK_QUEUE_COMPUTE_BIT then
                { indices with computeFamily = Some (uint32 i) }
            elif x.queueFlags &&& VkQueueFlags.VK_QUEUE_TRANSFER_BIT = VkQueueFlags.VK_QUEUE_TRANSFER_BIT then
                { indices with transferFamily = Some (uint32 i) }
            else
                indices
        else
            indices) { graphicsFamily = None; presentFamily = None; computeFamily = None; transferFamily = None }

let mkDeviceQueueCreateInfo queueFamilyIndex pQueuePriorities =
    VkDeviceQueueCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO,
        queueFamilyIndex = queueFamilyIndex,
        queueCount = 1u,
        pQueuePriorities = pQueuePriorities
    )

let mkDeviceCreateInfo pQueueCreateInfos queueCreateInfoCount pEnabledFeatures enabledExtensionCount ppEnabledExtensionNames enabledLayerCount ppEnabledLayerNames =
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

let getDeviceExtensions physicalDevice deviceExtensions =
    let mutable extensionCount = 0u

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

let getDeviceLayers physicalDevice deviceLayers =
    let mutable layerCount = 0u

    vkEnumerateDeviceLayerProperties(physicalDevice, &&layerCount, vkNullPtr) |> checkResult

    let layers = Array.zeroCreate (int layerCount)
    use layersPtr = fixed layers
    vkEnumerateDeviceLayerProperties(physicalDevice, &&layerCount, layersPtr) |> checkResult

    deviceLayers
    |> Array.iter (fun x ->
        if layers |> Array.exists (fun y -> y.layerName.ToString() = x) |> not then
            failwithf "Unable to find device layer, %s." x
    )

    layers
    |> Array.filter (fun x -> deviceLayers |> Array.exists (fun y -> x.layerName.ToString() = y))

let mkLogicalDevice physicalDevice (indices: QueueFamilyIndices) deviceLayers deviceExtensions =
    let queueCreateInfos = 
        [|
            if indices.HasGraphics then
                yield indices.graphicsFamily.Value
                yield indices.presentFamily.Value
            
            if indices.HasCompute then
                yield indices.computeFamily.Value

            if indices.HasTransfer then
                yield indices.transferFamily.Value
        |]
        |> Array.distinct // we need to be distinct so we do not create duplicate create infos
        |> Array.map (fun familyIndex ->
            let mutable queuePriority = 1.f
            mkDeviceQueueCreateInfo familyIndex &&queuePriority
        )
    let extensions = getDeviceExtensions physicalDevice deviceExtensions |> Array.map (fun x -> x.extensionName.ToString())
    let layers = getDeviceLayers physicalDevice deviceLayers |> Array.map (fun x -> x.layerName.ToString())

    let mutable deviceFeatures = VkPhysicalDeviceFeatures()
    vkGetPhysicalDeviceFeatures(physicalDevice, &&deviceFeatures)

    use pQueueCreateInfos = fixed queueCreateInfos
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let mutable createInfo = 
        mkDeviceCreateInfo 
            pQueueCreateInfos (uint32 queueCreateInfos.Length)
            &&deviceFeatures 
            (uint32 extensions.Length) extensionsHandle.PtrPtr
            (uint32 layers.Length) layersHandle.PtrPtr

    // Uncomment when we have a way to set this.
    //let mutable features =
    //    VkPhysicalDeviceShaderFloat16Int8FeaturesKHR(
    //        sType = VkStructureType.VK_STRUCTURE_TYPE_PHYSICAL_DEVICE_FLOAT16_INT8_FEATURES_KHR,
    //        shaderInt8 = VK_TRUE
    //    )

    //createInfo.pNext <- &&features |> NativePtr.toNativeInt

    let mutable device = VkDevice()
    vkCreateDevice(physicalDevice, &&createInfo, vkNullPtr, &&device) |> checkResult
    device

let createVkDebugUtilsMessengerCallback debugCallback =
    PFN_vkDebugUtilsMessengerCallbackEXT.Create(fun messageSeverity messageType pCallbackData pUserData ->
        let callbackData = NativePtr.read pCallbackData
        let str = vkStringOfBytePtr callbackData.pMessage
        debugCallback str

        VK_FALSE
    )

let createWin32Surface hWnd hInstance vkInstance =
    let mutable createInfo = 
        VkWin32SurfaceCreateInfoKHR (
            sType = VkStructureType.VK_STRUCTURE_TYPE_WIN32_SURFACE_CREATE_INFO_KHR,
            hwnd = hWnd,
            hinstance = hInstance
        )
    let mutable surface = VkSurfaceKHR ()
    vkCreateWin32SurfaceKHR(vkInstance, &&createInfo, vkNullPtr, &&surface) |> checkResult
    surface