module Foom.Vulkan

open System
open System.Threading
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
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

let private isDiscreteGpu (deviceProperties: VkPhysicalDeviceProperties) =
    deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU

let private isDiscreteGpuWithGeometryShader deviceProperties (deviceFeatures: VkPhysicalDeviceFeatures) =
    isDiscreteGpu deviceProperties && deviceFeatures.geometryShader = VK_TRUE

let private isIntegratedGpu (deviceProperties: VkPhysicalDeviceProperties) =
    deviceProperties.deviceType = VkPhysicalDeviceType.VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU

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
            [capabilities.minImageExtent.width; capabilities.maxImageExtent.width]
            |> List.max

        let height =
            [capabilities.minImageExtent.width; capabilities.maxImageExtent.width]
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

    // get images too

    let imageCount = 0u
    vkGetSwapchainImagesKHR(device, swapChain, &&imageCount, vkNullPtr) |> checkResult

    let images = Array.zeroCreate (int imageCount)
    use pImages = fixed images
    vkGetSwapchainImagesKHR(device, swapChain, &&imageCount, pImages) |> checkResult

    swapChain, surfaceFormat, extent, images

let mkImageViewCreateInfo format image =
    let components =
        VkComponentMapping (
            r = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY, 
            g = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY, 
            b = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY, 
            a = VkComponentSwizzle.VK_COMPONENT_SWIZZLE_IDENTITY
        )

    let subresourceRange =
        VkImageSubresourceRange (
            aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
            baseMipLevel = 0u,
            levelCount = 1u,
            baseArrayLayer = 0u,
            layerCount = 1u
        )

    VkImageViewCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
        image = image,
        viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
        format = format,
        components = components,
        subresourceRange = subresourceRange
    )

let mkImageView device format image =
    let createInfo = mkImageViewCreateInfo format image
    let imageView = VkImageView()
    vkCreateImageView(device, &&createInfo, vkNullPtr, &&imageView) |> checkResult
    imageView
    
let mkImageViews device surfaceFormat images =
    images
    |> Array.map (mkImageView device surfaceFormat)

let mkShaderModule device (code: nativeptr<byte>) (codeSize: uint32) =
    let createInfo = 
        VkShaderModuleCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO,
            codeSize = unativeint codeSize,
            pCode = vkCastPtr code
        )

    let shaderModule = VkShaderModule ()
    vkCreateShaderModule(device, &&createInfo, vkNullPtr, &&shaderModule) |> checkResult
    shaderModule

let mkColorAttachment format =
    VkAttachmentDescription (
        format = format,
        samples = VkSampleCountFlags.VK_SAMPLE_COUNT_1_BIT,
        loadOp = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
        storeOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE,
        stencilLoadOp = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
        stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
        initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
        finalLayout = VkImageLayout.VK_IMAGE_LAYOUT_PRESENT_SRC_KHR
    )

let mkColorAttachmentRef =
    VkAttachmentReference (
        attachment = 0u,
        layout = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL
    )

let mkSubpass pColorAttachmentRefs colorAttachmentRefCount =
    // The index of the attachment in this array is directly referenced from the fragment shader 
    // with the layout(location = 0) out vec4 outColor directive!
    VkSubpassDescription (
        pipelineBindPoint = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
        colorAttachmentCount = colorAttachmentRefCount,
        pColorAttachments = pColorAttachmentRefs
    )

let mkRenderPass device format =
    let colorAttachment = mkColorAttachment format
    let colorAttachmentRef = mkColorAttachmentRef
    let subpass = mkSubpass &&colorAttachmentRef 1u

    let dependency = 
        VkSubpassDependency (
            srcSubpass = VK_SUBPASS_EXTERNAL,
            dstSubpass = 0u,
            srcStageMask = VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
            srcAccessMask = VkAccessFlags (),
            dstStageMask = VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
            dstAccessMask = (VkAccessFlags.VK_ACCESS_COLOR_ATTACHMENT_READ_BIT ||| VkAccessFlags.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT)
        )

    let createInfo =
        VkRenderPassCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO,
            attachmentCount = 1u,
            pAttachments = &&colorAttachment,
            subpassCount = 1u,
            pSubpasses = &&subpass,
            dependencyCount = 1u,
            pDependencies = &&dependency
        )

    let renderPass = VkRenderPass ()
    vkCreateRenderPass(device, &&createInfo, vkNullPtr, &&renderPass) |> checkResult
    renderPass

let mkShaderStageInfo stage pName shaderModule =
    VkPipelineShaderStageCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO,
        stage = stage,
        modul = shaderModule,
        pName = pName
    )

let mkInputAssemblyCreateInfo () =
    VkPipelineInputAssemblyStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO,
        topology = VkPrimitiveTopology.VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST,
        primitiveRestartEnable = VK_FALSE
    )

let mkViewport (extent: VkExtent2D) =
    VkViewport (
        x = 0.f,
        y = 0.f,
        width = float32 extent.width,
        height = float32 extent.height,
        minDepth = 0.f,
        maxDepth = 1.f
    )

let mkScissor (extent: VkExtent2D) =
    VkRect2D (
        offset = VkOffset2D (x = 0, y = 0),
        extent = extent
    )

let mkViewportStateCreateInfo pViewport pScissor =
    VkPipelineViewportStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO,
        viewportCount = 1u,
        pViewports = pViewport,
        scissorCount = 1u,
        pScissors = pScissor
    )

let mkRasterizerCreateInfo () =     
    VkPipelineRasterizationStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO,
        depthClampEnable = VK_FALSE,
        rasterizerDiscardEnable = VK_FALSE,
        polygonMode = VkPolygonMode.VK_POLYGON_MODE_FILL,
        lineWidth = 1.0f,
        cullMode = VkCullModeFlags.VK_CULL_MODE_BACK_BIT,
        frontFace = VkFrontFace.VK_FRONT_FACE_CLOCKWISE,
        depthBiasEnable = VK_FALSE,
        depthBiasConstantFactor = 0.f, // Optional
        depthBiasClamp = 0.f, // Optional
        depthBiasSlopeFactor = 0.f // Optional
    )

let mkMultisampleCreateInfo () =
    VkPipelineMultisampleStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO,
        sampleShadingEnable = VK_FALSE,
        rasterizationSamples = VkSampleCountFlags.VK_SAMPLE_COUNT_1_BIT,
        minSampleShading = 1.f, // Optional
        pSampleMask = vkNullPtr, // Optional
        alphaToCoverageEnable = VK_FALSE, // Optional
        alphaToOneEnable = VK_FALSE // Optional
    )

let mkDepthStencilCreateInfo () =
    VkPipelineDepthStencilStateCreateInfo ()

    //colorBlendAttachment.blendEnable = VK_TRUE;
    //colorBlendAttachment.srcColorBlendFactor = VK_BLEND_FACTOR_SRC_ALPHA;
    //colorBlendAttachment.dstColorBlendFactor = VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA;
    //colorBlendAttachment.colorBlendOp = VK_BLEND_OP_ADD;
    //colorBlendAttachment.srcAlphaBlendFactor = VK_BLEND_FACTOR_ONE;
    //colorBlendAttachment.dstAlphaBlendFactor = VK_BLEND_FACTOR_ZERO;
    //colorBlendAttachment.alphaBlendOp = VK_BLEND_OP_ADD;
let mkColorBlendAttachment () =
    VkPipelineColorBlendAttachmentState (
        colorWriteMask = (VkColorComponentFlags.VK_COLOR_COMPONENT_R_BIT |||
                            VkColorComponentFlags.VK_COLOR_COMPONENT_G_BIT |||
                            VkColorComponentFlags.VK_COLOR_COMPONENT_B_BIT |||
                            VkColorComponentFlags.VK_COLOR_COMPONENT_A_BIT),
        blendEnable = VK_FALSE,
        srcColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE, // Optional
        dstColorBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ZERO, // Optional
        colorBlendOp = VkBlendOp.VK_BLEND_OP_ADD, // Optional
        srcAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ONE, // Optional
        dstAlphaBlendFactor = VkBlendFactor.VK_BLEND_FACTOR_ZERO, // Optional
        alphaBlendOp = VkBlendOp.VK_BLEND_OP_ADD
    )

let mkColorBlendCreateInfo pAttachments attachmentCount =
    VkPipelineColorBlendStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO,
        logicOpEnable = VK_FALSE,
        logicOp = VkLogicOp.VK_LOGIC_OP_COPY, // Optional
        attachmentCount = attachmentCount,
        pAttachments = pAttachments,
        blendConstants = 
            (let mutable x = VkFixedArray_float32_4 ()
             x.[0] <- 0.f // Optional
             x.[1] <- 0.f // Optional
             x.[2] <- 0.f // Optional
             x.[3] <- 0.f // Optional
             x)
    )

let defaultDynamicStates =
    [|
        VkDynamicState.VK_DYNAMIC_STATE_VIEWPORT
        VkDynamicState.VK_DYNAMIC_STATE_LINE_WIDTH  
    |]

let mkDynamicStateCreateInfo pDynamicStates dynamicStateCount =
    VkPipelineDynamicStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO,
        dynamicStateCount = dynamicStateCount,
        pDynamicStates = pDynamicStates
    )

let mkPipelineLayout device =
    let createInfo =
        VkPipelineLayoutCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
            setLayoutCount = 0u, // Optional
            pSetLayouts = vkNullPtr, // Optional
            pushConstantRangeCount = 0u, // Optional
            pPushConstantRanges = vkNullPtr // Optional
        )

    let pipelineLayout = VkPipelineLayout ()
    vkCreatePipelineLayout(device, &&createInfo, vkNullPtr, &&pipelineLayout) |> checkResult
    pipelineLayout

type ShaderGroup =
    {
        vertexBindings: VkVertexInputBindingDescription []
        vertexAttributes: VkVertexInputAttributeDescription []
        vertexBuffers: VkBuffer []

        vertex: VkShaderModule
        fragment: VkShaderModule
    }

let mkShaderGroup device vertexBindings vertexAttributes vertexBuffers vert vertSize frag fragSize =
    let vertShaderModule = mkShaderModule device vert vertSize
    let fragShaderModule = mkShaderModule device frag fragSize
    {
        vertexBindings = vertexBindings
        vertexAttributes = vertexAttributes
        vertexBuffers = vertexBuffers
        vertex = vertShaderModule
        fragment = fragShaderModule
    }

let destroyShaderGroup device group =
    vkDestroyShaderModule(device, group.vertex, vkNullPtr)
    vkDestroyShaderModule(device, group.fragment, vkNullPtr)

[<RequiresExplicitTypeArguments>]
let mkVertexInputBinding<'T when 'T : unmanaged> binding inputRate =
    VkVertexInputBindingDescription (
        binding = binding,
        stride = uint32 sizeof<'T>,
        inputRate = inputRate
    )
    
let mkVertexAttributeDescription binding location format offset =
    VkVertexInputAttributeDescription (
        binding = binding,
        location = location,
        format = format,
        offset = offset
    )
    
[<RequiresExplicitTypeArguments>]
let mkVertexAttributeDescriptions<'T when 'T : unmanaged> locationOffset binding =
    let rec mk (ty: Type) location offset = 
        match ty with
        | _ when ty = typeof<single> -> 
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32_SFLOAT offset|]
    
        | _ when ty = typeof<int> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32_SINT offset|]
    
        | _ when ty = typeof<Numerics.Vector2> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32_SFLOAT offset|]
    
        | _ when ty = typeof<Numerics.Vector3> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32_SFLOAT offset|]
    
        | _ when ty = typeof<Numerics.Vector4> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT offset|]
    
        | _ when ty = typeof<Numerics.Matrix3x2> ->
            failwith "Matrix3x2 not supported yet."
    
        | _ when ty = typeof<Numerics.Matrix4x4> ->
            [|
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT offset
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT (offset + uint32 sizeof<Numerics.Vector4>)
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT (offset + uint32 (sizeof<Numerics.Vector4> * 2))
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT (offset + uint32 (sizeof<Numerics.Vector4> * 3))
            |]
    
        | _ when ty.IsPrimitive ->
            failwithf "Primitive type not supported: %A" ty
    
        | _ when ty.IsValueType ->
            ty.GetFields(Reflection.BindingFlags.NonPublic ||| Reflection.BindingFlags.Public ||| Reflection.BindingFlags.Instance)
            |> Array.mapi (fun i field ->
                mk field.FieldType (locationOffset + uint32 i) (Marshal.OffsetOf(field.FieldType, field.Name) |> uint32)
            )
            |> Array.concat
    
        | _ ->
            failwithf "Type not supported: %A" ty
    
    mk typeof<'T> locationOffset 0u

let mkVertexBuffer<'T when 'T : unmanaged> device count =
    let bufferInfo =
        VkBufferCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO,
            size = uint64 (sizeof<'T> * count),
            usage = VkBufferUsageFlags.VK_BUFFER_USAGE_VERTEX_BUFFER_BIT,
            sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE
        )

    let vertexBuffer = VkBuffer ()
    vkCreateBuffer(device, &&bufferInfo, vkNullPtr, &&vertexBuffer) |> checkResult
    vertexBuffer

let getMemoryRequirements device buffer =
    let memRequirements = VkMemoryRequirements ()
    vkGetBufferMemoryRequirements(device, buffer, &&memRequirements)
    memRequirements

let getSuitableMemoryTypeIndex physicalDevice typeFilter properties =
    let memProperties = VkPhysicalDeviceMemoryProperties ()
    vkGetPhysicalDeviceMemoryProperties(physicalDevice, &&memProperties)

    [| for i = 0 to int memProperties.memoryTypeCount - 1 do
        if typeFilter &&& (1u <<< i) <> 0u then
            let memType = memProperties.memoryTypes.[i]
            if memType.propertyFlags &&& properties = properties then
                yield uint32 i |]
    |> Array.head

let allocateMemory physicalDevice device (memRequirements: VkMemoryRequirements) =
    let memTypeIndex = 
        getSuitableMemoryTypeIndex 
            physicalDevice memRequirements.memoryTypeBits 
            (VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
             VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT)

    let allocInfo =
        VkMemoryAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO,
            allocationSize = memRequirements.size,
            memoryTypeIndex = memTypeIndex
        )

    let bufferMemory = VkDeviceMemory ()
    vkAllocateMemory(device, &&allocInfo, vkNullPtr, &&bufferMemory) |> checkResult
    bufferMemory

let mkGraphicsPipeline device extent pipelineLayout renderPass group =
    use pNameMain = fixed vkBytesOfString "main"

    let stages =
        [|
            mkShaderStageInfo VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT pNameMain group.vertex
            mkShaderStageInfo VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT pNameMain group.fragment
        |]

    use pVertexBindingDescriptions = fixed group.vertexBindings
    use pVertexAttributeDescriptions = fixed group.vertexAttributes
        
    let vertexInputCreateInfo =
        VkPipelineVertexInputStateCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO,
            vertexBindingDescriptionCount = uint32 group.vertexBindings.Length,
            pVertexBindingDescriptions = pVertexBindingDescriptions,
            vertexAttributeDescriptionCount = uint32 group.vertexAttributes.Length,
            pVertexAttributeDescriptions = pVertexAttributeDescriptions
        )

    let inputAssemblyCreateInfo = mkInputAssemblyCreateInfo ()

    let viewport = mkViewport extent
    let scissor = mkScissor extent
    let viewportStateCreateInfo = mkViewportStateCreateInfo &&viewport &&scissor

    let rasterizerCreateInfo = mkRasterizerCreateInfo ()
    let multisampleCreateInfo = mkMultisampleCreateInfo ()
    // TODO: depth stencil state

    let colorBlendAttachment = mkColorBlendAttachment ()
    let colorBlendCreateInfo = mkColorBlendCreateInfo &&colorBlendAttachment 1u

    // TODO: dynamic state

    use pStages = fixed stages
    let createInfo =
        VkGraphicsPipelineCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO,
            stageCount = uint32 stages.Length,
            pStages = pStages,
                
            pVertexInputState = &&vertexInputCreateInfo,
            pInputAssemblyState = &&inputAssemblyCreateInfo,
            pViewportState = &&viewportStateCreateInfo,
            pRasterizationState = &&rasterizerCreateInfo,
            pMultisampleState = &&multisampleCreateInfo,
            pDepthStencilState = vkNullPtr, // Optional
            pColorBlendState = &&colorBlendCreateInfo,
            pDynamicState = vkNullPtr, // Optional
            layout = pipelineLayout,
            renderPass = renderPass,
            subpass = 0u,

            basePipelineHandle = VK_NULL_HANDLE, // Optional
            basePipelineIndex = -1 // Optional
        )

    let graphicsPipeline = VkPipeline ()
    vkCreateGraphicsPipelines(device, VK_NULL_HANDLE, 1u, &&createInfo, vkNullPtr, &&graphicsPipeline) |> checkResult
    graphicsPipeline

let mkFramebuffers device renderPass (extent: VkExtent2D) imageViews =
    imageViews
    |> Array.map (fun imageView ->
        let framebufferCreateInfo = 
            VkFramebufferCreateInfo (
                sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO,
                renderPass = renderPass,
                attachmentCount = 1u,
                pAttachments = &&imageView,
                width = extent.width,
                height = extent.height,
                layers = 1u
            )

        let framebuffer = VkFramebuffer ()
        vkCreateFramebuffer(device, &&framebufferCreateInfo, vkNullPtr, &&framebuffer) |> checkResult
        framebuffer)

let private mkCommandPool device indices =
    let poolCreateInfo =
        VkCommandPoolCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO,
            queueFamilyIndex = indices.graphicsFamily.Value,
            flags = VkCommandPoolCreateFlags () // Optional
        )

    let commandPool = VkCommandPool ()
    vkCreateCommandPool(device, &&poolCreateInfo, vkNullPtr, &&commandPool) |> checkResult
    commandPool

let private mkCommandBuffers device commandPool (framebuffers: VkFramebuffer []) =
    let commandBuffers = Array.zeroCreate framebuffers.Length

    let allocCreateInfo =
        VkCommandBufferAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            commandPool = commandPool,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandBufferCount = uint32 commandBuffers.Length
        )

    use pCommandBuffers = fixed commandBuffers
    vkAllocateCommandBuffers(device, &&allocCreateInfo, pCommandBuffers) |> checkResult
    commandBuffers

module Cmd =

    let recordDraw extent (framebuffers: VkFramebuffer []) (commandBuffers: VkCommandBuffer []) renderPass graphicsPipeline (vertexBuffers: VkBuffer []) =
        (framebuffers, commandBuffers)
        ||> Array.iter2 (fun framebuffer commandBuffer ->

            // Begin command buffer

            let beginInfo =
                VkCommandBufferBeginInfo (
                    sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
                    flags = VkCommandBufferUsageFlags (), // Optional
                    pInheritanceInfo = vkNullPtr
                )

            vkBeginCommandBuffer(commandBuffer, &&beginInfo) |> checkResult

            // Begin Render pass

            let clearColor = 
                let colorValue = VkFixedArray_float32_4 (_0 = 0.f, _1 = 0.f, _2 = 0.f, _3 = 1.f)
                let color = VkClearColorValue (float32 = colorValue)
                VkClearValue (color = color)

            let beginInfo =
                VkRenderPassBeginInfo (
                    sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
                    renderPass = renderPass,
                    framebuffer = framebuffer,
                    renderArea = VkRect2D (offset = VkOffset2D (x = 0, y = 0), extent = extent),
                    clearValueCount = 1u,
                    pClearValues = &&clearColor
                )

            vkCmdBeginRenderPass(commandBuffer, &&beginInfo, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE)

            // Bind graphics pipeline

            vkCmdBindPipeline(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, graphicsPipeline)

            // Bind vertex buffers

            if vertexBuffers |> Array.isEmpty |> not then          
                let offsets = 0UL
                use pVertexBuffers = fixed vertexBuffers
                vkCmdBindVertexBuffers(commandBuffer, 0u, uint32 vertexBuffers.Length, pVertexBuffers, &&offsets)

            // Draw

            vkCmdDraw(commandBuffer, 3u, 1u, 0u, 0u)

            // End render pass

            vkCmdEndRenderPass(commandBuffer)

            // Finish recording

            vkEndCommandBuffer(commandBuffer) |> checkResult
        )

type Sync =
    {
        imageAvailableSemaphores: VkSemaphore []
        renderFinishedSemaphores: VkSemaphore []
        inFlightFences: VkFence []
    }

[<Literal>]
let MaxFramesInFlight = 2

let mkSync device =
    let semaphoreCreateInfo =
        VkSemaphoreCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO
        )

    let imageAvailableSemaphores =
        Array.init MaxFramesInFlight (fun _ ->
            let imageAvailableSemaphore = VkSemaphore ()
            vkCreateSemaphore(device, &&semaphoreCreateInfo, vkNullPtr, &&imageAvailableSemaphore) |> checkResult
            imageAvailableSemaphore
        )

    let renderFinishedSemaphores =
        Array.init MaxFramesInFlight (fun _ ->
            let renderFinishedSemaphore = VkSemaphore ()
            vkCreateSemaphore(device, &&semaphoreCreateInfo, vkNullPtr, &&renderFinishedSemaphore) |> checkResult
            renderFinishedSemaphore
        )

    let fenceCreateInfo =
        VkFenceCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_FENCE_CREATE_INFO,
            flags = VkFenceCreateFlags.VK_FENCE_CREATE_SIGNALED_BIT // we need this so we do not lock up on the first 'drawFrame'
        )

    let inFlightFences = 
        Array.init MaxFramesInFlight (fun _ ->
            let fence = VkFence ()
            vkCreateFence(device, &&fenceCreateInfo, vkNullPtr, &&fence) |> checkResult
            fence
        )

    {
        imageAvailableSemaphores = imageAvailableSemaphores
        renderFinishedSemaphores = renderFinishedSemaphores
        inFlightFences = inFlightFences
    }

let drawFrame device swapChain sync (commandBuffers: VkCommandBuffer []) graphicsQueue presentQueue currentFrame =
    use pFences = fixed &sync.inFlightFences.[currentFrame]
    vkWaitForFences(device, 1u, pFences, VK_TRUE, UInt64.MaxValue) |> checkResult
    vkResetFences(device, 1u, pFences) |> checkResult

    let imageIndex = 0u
    vkAcquireNextImageKHR(device, swapChain, UInt64.MaxValue, sync.imageAvailableSemaphores.[currentFrame], VK_NULL_HANDLE, &&imageIndex) |> checkResult

    let waitSemaphores = [|sync.imageAvailableSemaphores.[currentFrame]|]
    let waitStages = [|VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT|]

    let signalSemaphores = [|sync.renderFinishedSemaphores.[currentFrame]|]

    use pWaitSemaphores = fixed waitSemaphores
    use waitStages = fixed waitStages
    use pSignalSemaphores = fixed signalSemaphores
    use pCommandBuffers = fixed &commandBuffers.[int imageIndex]
    let submitInfo =
        VkSubmitInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            waitSemaphoreCount = uint32 waitSemaphores.Length,
            pWaitSemaphores = pWaitSemaphores,
            pWaitDstStageMask = waitStages,
            commandBufferCount = 1u,
            pCommandBuffers = pCommandBuffers,
            signalSemaphoreCount = uint32 signalSemaphores.Length,
            pSignalSemaphores = pSignalSemaphores
        )

    vkQueueSubmit(graphicsQueue, 1u, &&submitInfo, sync.inFlightFences.[currentFrame]) |> checkResult

    // Presentation

    let swapChains = [|swapChain|]

    use pSwapChains = fixed swapChains
    let presentInfo =
        VkPresentInfoKHR (
            sType = VkStructureType.VK_STRUCTURE_TYPE_PRESENT_INFO_KHR,
            waitSemaphoreCount = uint32 signalSemaphores.Length,
            pWaitSemaphores = pSignalSemaphores,
            swapchainCount = uint32 swapChains.Length,
            pSwapchains = pSwapChains,
            pImageIndices = &&imageIndex,
            pResults = vkNullPtr // Optional
        )

    let res = vkQueuePresentKHR(presentQueue, &&presentInfo)

    (currentFrame + 1) % MaxFramesInFlight, res

type private SwapChainState =
    {
        extent: VkExtent2D
        swapChain: VkSwapchainKHR
        imageViews: VkImageView []
        renderPass: VkRenderPass
        pipelineLayout: VkPipelineLayout
        framebuffers: VkFramebuffer []
        commandBuffers: VkCommandBuffer []
    }

[<Sealed>]
type private SwapChain (physicalDevice, device, surface, indices, commandPool) =

    let gate = obj ()
    let mutable state = None
    let mutable currentFrame = 0
    let mutable isDisposed = 0

    let shaderGroups = ResizeArray ()
    let pipelines = ResizeArray ()

    let checkDispose () =       
        if isDisposed <> 0 then
            failwith "SwapChain disposed."

    let check () =
        checkDispose ()

        if state.IsNone then
            failwith "SwapChain never created."

    let destroy () =
        match Interlocked.Exchange(&state, None) with
        | None -> ()
        | Some {
                swapChain = swapChain
                imageViews = imageViews
                renderPass = renderPass
                pipelineLayout = pipelineLayout
                framebuffers = framebuffers
                commandBuffers = commandBuffers
                } ->

        vkDeviceWaitIdle(device) |> checkResult

        currentFrame <- 0

        pipelines
        |> Seq.rev
        |> Seq.iter (fun pipeline ->
            vkDestroyPipeline(device, pipeline, vkNullPtr)
        )

        pipelines.Clear ()

        use pCommandBuffers = fixed commandBuffers
        vkFreeCommandBuffers(device, commandPool, uint32 commandBuffers.Length, pCommandBuffers)

        framebuffers
        |> Array.iter (fun framebuffer ->
            vkDestroyFramebuffer(device, framebuffer, vkNullPtr)
        )

        vkDestroyPipelineLayout(device, pipelineLayout, vkNullPtr)
        vkDestroyRenderPass(device, renderPass, vkNullPtr)

        imageViews
        |> Array.iter (fun imageView ->
            vkDestroyImageView(device, imageView, vkNullPtr)
        )

        vkDestroySwapchainKHR(device, swapChain, vkNullPtr)

    let addPipeline group =
        match state with
        | Some state ->
            let pipeline = mkGraphicsPipeline device state.extent state.pipelineLayout state.renderPass group

            Cmd.recordDraw state.extent state.framebuffers state.commandBuffers state.renderPass pipeline group.vertexBuffers

            pipelines.Add pipeline
        | _ ->
            ()

    member _.SwapChain = 
        check ()
        state.Value.swapChain

    member _.CommandBuffers =
        check ()
        state.Value.commandBuffers

    member x.Recreate () =
        if not (Monitor.IsEntered gate) then
            Monitor.Enter gate

        try
            checkDispose ()
            destroy ()

            let swapChain, surfaceFormat, extent, images = mkSwapChain physicalDevice device surface indices
            let imageViews = mkImageViews device surfaceFormat.format images
            let renderPass = mkRenderPass device surfaceFormat.format
            let pipelineLayout = mkPipelineLayout device
            let framebuffers = mkFramebuffers device renderPass extent imageViews
            let commandBuffers = mkCommandBuffers device commandPool framebuffers

            state <- 
                { 
                    extent = extent
                    swapChain = swapChain
                    imageViews = imageViews
                    renderPass = renderPass
                    pipelineLayout = pipelineLayout
                    framebuffers = framebuffers
                    commandBuffers = commandBuffers
                }
                |> Some

            shaderGroups
            |> Seq.iter (fun group ->
                addPipeline group
            )

        finally
            Monitor.Exit gate

    member x.AddShader (vertexBindings, vertexAttributes, vertexBuffers, vertexBytes: ReadOnlySpan<byte>, fragmentBytes: ReadOnlySpan<byte>) =
        if not (Monitor.IsEntered gate) then
            Monitor.Enter gate

        try
            check ()

            let pVertexBytes = Unsafe.AsPointer(&Unsafe.AsRef(&vertexBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr
            let pFragmentBytes = Unsafe.AsPointer(&Unsafe.AsRef(&fragmentBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr

            let group = 
                mkShaderGroup device 
                    vertexBindings vertexAttributes vertexBuffers
                    pVertexBytes (uint32 vertexBytes.Length) 
                    pFragmentBytes (uint32 fragmentBytes.Length)

            shaderGroups.Add group
            addPipeline group

        finally
            Monitor.Exit gate

    member x.DrawFrame (sync, graphicsQueue, presentQueue) =
        lock gate |> fun _ ->

        if pipelines.Count > 0 then
            check ()

            let nextFrame, res = drawFrame device x.SwapChain sync x.CommandBuffers graphicsQueue presentQueue currentFrame

            if res = VkResult.VK_ERROR_OUT_OF_DATE_KHR || res = VkResult.VK_SUBOPTIMAL_KHR then
                x.Recreate ()
            else
                checkResult res
                currentFrame <- nextFrame

    interface IDisposable with

        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                failwith "SwapChain already disposed"
            else
                GC.SuppressFinalize x
                lock gate (fun () ->
                    shaderGroups
                    |> Seq.rev
                    |> Seq.iter (fun group -> 
                        destroyShaderGroup device group
                    )

                    destroy ()
                )

[<Sealed>]
type VulkanInstance 
    private
    (instance: VkInstance, 
     debugMessenger: VkDebugUtilsMessengerEXT, 
     physicalDevice: VkPhysicalDevice,
     device: VkDevice, 
     surface: VkSurfaceKHR,
     commandPool: VkCommandPool,
     sync: Sync,
     graphicsQueue: VkQueue, presentQueue: VkQueue, handles: GCHandle[],
     swapChain: SwapChain) =

    let gate = obj ()
    let buffers = Collections.Generic.Dictionary<VkBuffer, VkDeviceMemory voption>()
    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "Vulkan instance is disposed."

    let destroyBuffer buffer memoryOpt =
        vkDestroyBuffer(device, buffer, vkNullPtr)
        match memoryOpt with
        | ValueSome memory -> vkFreeMemory(device, memory, vkNullPtr)
        | _ -> ()

    member __.Instance = 
        checkDispose ()
        instance

    member __.DebugMessenger =
        checkDispose ()
        debugMessenger

    member __.AddShader (vertexBindings, vertexAttributes, vertexBuffers, vertexBytes, fragmentBytes) =
        checkDispose ()
        swapChain.AddShader (vertexBindings, vertexAttributes, vertexBuffers, vertexBytes, fragmentBytes)

    member __.DrawFrame () =
        checkDispose ()
        swapChain.DrawFrame(sync, graphicsQueue, presentQueue)

    member __.WaitIdle () =
        checkDispose ()
        vkQueueWaitIdle(presentQueue) |> checkResult
        vkQueueWaitIdle(graphicsQueue) |> checkResult
        vkDeviceWaitIdle(device) |> checkResult

    [<RequiresExplicitTypeArguments>]
    member _.CreateVertexBuffer<'T when 'T : unmanaged> count =
        lock gate <| fun _ ->

        checkDispose ()

        let buffer = mkVertexBuffer<'T> device count
        buffers.[buffer] <- ValueNone
        buffer

    member _.AllocateMemory buffer =
        lock gate <| fun _ ->

        match buffers.TryGetValue buffer with
        | true, ValueNone ->
            let memRequirements = getMemoryRequirements device buffer
            let bufferMemory = allocateMemory physicalDevice device memRequirements
            vkBindBufferMemory(device, buffer, bufferMemory, 0UL) |> checkResult
            buffers.[buffer] <- ValueSome bufferMemory
            bufferMemory
        | true, _ -> 
            failwith "Buffer already has memory allocated to it."
        | _ ->
            failwith "Buffer does not exist within this vulkan instance."

    member _.CopyToMemory<'T when 'T : unmanaged> (data: ReadOnlySpan<'T>, memory: VkDeviceMemory) =
        checkDispose ()

        let size = sizeof<'T> * data.Length
        let deviceData = nativeint 0
        let pDeviceData = &&deviceData |> NativePtr.toNativeInt
        let deviceData = Span<'T>(deviceData |> NativePtr.ofNativeInt<'T> |> NativePtr.toVoidPtr, size)

        vkMapMemory(device, memory, 0UL, uint64 (sizeof<'T> * data.Length), VkMemoryMapFlags.MinValue, pDeviceData) |> checkResult
        data.CopyTo deviceData
        vkUnmapMemory(device, memory)

    member _.DestroyBuffer (buffer: VkBuffer) =
        lock gate <| fun _ ->

        checkDispose ()
        match buffers.Remove buffer : bool * VkDeviceMemory voption with
        | true, memoryOpt -> 
            destroyBuffer buffer memoryOpt
        | _ ->
            failwith "Buffer is not in the vulkan instance."

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                failwith "VulkanInstance already disposed"
            else
                GC.SuppressFinalize x

                (swapChain :> IDisposable).Dispose ()

                lock gate (fun () ->
                    buffers
                    |> Seq.iter (fun pair ->
                        destroyBuffer pair.Key pair.Value
                    )

                    buffers.Clear()
                )

                sync.inFlightFences
                |> Array.rev
                |> Array.iter (fun f ->
                    vkDestroyFence(device, f, vkNullPtr)
                )

                sync.renderFinishedSemaphores
                |> Array.rev
                |> Array.iter (fun s ->
                    vkDestroySemaphore(device, s, vkNullPtr)
                )

                sync.imageAvailableSemaphores
                |> Array.rev
                |> Array.iter (fun s ->
                    vkDestroySemaphore(device, s, vkNullPtr)
                )

                vkDestroyCommandPool(device, commandPool, vkNullPtr)
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
        let commandPool = mkCommandPool device indices
        let sync = mkSync device
        let graphicsQueue = mkQueue device indices.graphicsFamily.Value
        let presentQueue = mkQueue device indices.presentFamily.Value

        let swapChain = new SwapChain(physicalDevice, device, surface, indices, commandPool)
        swapChain.Recreate ()

        new VulkanInstance (
            instance,
            debugMessenger,
            physicalDevice,
            device,
            surface,
            commandPool,
            sync,
            graphicsQueue, presentQueue, [|debugCallbackHandle|],
            swapChain
        )

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
