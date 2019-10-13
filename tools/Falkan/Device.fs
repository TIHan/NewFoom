[<AutoOpen>]
module Falkan.Device

open System
open System.Threading
open System.Runtime.InteropServices
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
        apiVersion = VK_API_VERSION_1_0
    )

let getInstanceExtension<'T when 'T :> Delegate> instance =
    use pName = fixed vkBytesOfString typeof<'T>.Name
    vkGetInstanceProcAddr(instance, pName) 
    |> vkDelegateOfFunctionPointer<'T>

let getInstanceExtensions() =
    let extensionCount = 0u

    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, vkNullPtr) |> checkResult

    let extensions = Array.zeroCreate (int extensionCount)
    use extensionsPtr = fixed extensions
    vkEnumerateInstanceExtensionProperties(vkNullPtr, &&extensionCount, extensionsPtr) |> checkResult
    extensions

let getInstanceLayers validationLayers =
    let layerCount = 0u

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
   
    let appInfo = mkApplicationInfo appNamePtr engineNamePtr
    let extensions = getInstanceExtensions () |> Array.map (fun x -> x.extensionName.ToString())
    let layers = getInstanceLayers validationLayers |> Array.map (fun x -> x.layerName.ToString())
    
    use extensionsHandle = vkFixedStringArray extensions
    use layersHandle = vkFixedStringArray layers
    let createInfo = mkInstanceCreateInfo &&appInfo (uint32 extensions.Length) extensionsHandle.PtrPtr (uint32 layers.Length) layersHandle.PtrPtr

    let instance = VkInstance()
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
    let createInfo = mkDebugMessengerCreateInfo debugCallback

    let createDebugUtilsMessenger = getInstanceExtension<vkCreateDebugUtilsMessengerEXT> instance
    let debugMessenger = VkDebugUtilsMessengerEXT ()
    createDebugUtilsMessenger.Invoke(instance, &&createInfo, vkNullPtr, &&debugMessenger) |> checkResult
    debugMessenger

let isDiscreteGpu (deviceProperties: VkPhysicalDeviceProperties) =
    deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU

let isDiscreteGpuWithGeometryShader deviceProperties (deviceFeatures: VkPhysicalDeviceFeatures) =
    isDiscreteGpu deviceProperties && deviceFeatures.geometryShader = VK_TRUE

let isIntegratedGpu (deviceProperties: VkPhysicalDeviceProperties) =
    deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU

let getSuitablePhysicalDevice instance =
    let deviceCount = 0u

    vkEnumeratePhysicalDevices(instance, &&deviceCount, vkNullPtr) |> checkResult

    if deviceCount = 0u then
        failwith "Unable to find GPUs with Vulkan support."

    let devices = Array.zeroCreate (int deviceCount)
    let pDevices = fixed devices
    vkEnumeratePhysicalDevices(instance, &&deviceCount, pDevices) |> checkResult

    let deviceOpt =
        devices
        |> Array.map (fun device ->
            let deviceProperties = VkPhysicalDeviceProperties()
            vkGetPhysicalDeviceProperties(device, &&deviceProperties)

            let deviceFeatures = VkPhysicalDeviceFeatures()
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
    let queueFamilyCount = 0u

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
                    let presentSupport = VK_FALSE      
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

let getDeviceLayers physicalDevice validationLayers =
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

let mkLogicalDevice physicalDevice indices validationLayers deviceExtensions =
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

let mkCommandPool device queueFamilyIndex =
    let poolCreateInfo =
        VkCommandPoolCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO,
            queueFamilyIndex = queueFamilyIndex,
            flags = VkCommandPoolCreateFlags () // Optional
        )

    let commandPool = VkCommandPool ()
    vkCreateCommandPool(device, &&poolCreateInfo, vkNullPtr, &&commandPool) |> checkResult
    commandPool

[<Sealed>]
type FalDevice private 
    (
        instance: VkInstance,
        surfaceOpt: VkSurfaceKHR option,
        debugMessenger: VkDebugUtilsMessengerEXT,
        physicalDevice: VkPhysicalDevice,
        indices: QueueFamilyIndices,
        device: VkDevice,
        handles: GCHandle []
    ) =

    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "FalDevice is disposed."

    member _.PhysicalDevice =
        checkDispose ()
        physicalDevice

    member _.Device =
        checkDispose ()
        device

    member _.Indices =
        checkDispose ()
        indices

    member _.Surface =
        checkDispose ()
        surfaceOpt

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                checkDispose ()
            else
                GC.SuppressFinalize x

                vkDestroyDevice(device, vkNullPtr)

                if debugMessenger <> IntPtr.Zero then
                    let destroyDebugUtilsMessenger = getInstanceExtension<vkDestroyDebugUtilsMessengerEXT> instance
                    destroyDebugUtilsMessenger.Invoke(instance, debugMessenger, vkNullPtr)

                surfaceOpt |> Option.iter (fun surface -> vkDestroySurfaceKHR(instance, surface, vkNullPtr))
                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

    static member Create (appName, engineName, validationLayers, deviceExtensions, ?mkSurface) =
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
        // must create surface right after instance - influences device calls
        let surfaceOpt = match mkSurface with Some mkSurface -> Some(mkSurface instance) | _ -> None
        let debugMessenger = mkDebugMessenger instance debugCallback
        let physicalDevice = getSuitablePhysicalDevice instance
        let indices = getPhysicalDeviceQueueFamilies physicalDevice surfaceOpt
        let device = mkLogicalDevice physicalDevice indices validationLayers deviceExtensions

        new FalDevice (instance, surfaceOpt, debugMessenger, physicalDevice, indices, device, [|debugCallbackHandle|])