[<AutoOpen>]
module Falkan.SwapChain

open System
open System.Threading
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

[<NoEquality;NoComparison>]
type SwapChainState =
    {
        extent: VkExtent2D
        swapChain: VkSwapchainKHR
        imageViews: VkImageView []
        renderPass: VkRenderPass
        pipelineLayout: VkPipelineLayout
        framebuffers: VkFramebuffer []
        commandBuffers: VkCommandBuffer []
    }

[<NoEquality;NoComparison>]
type DrawRecording =
    {
        pipelineIndex: int
        vertexBuffers: VkBuffer []
        vertexCount: uint32
        instanceCount: uint32
    }

let recordDraw extent (framebuffers: VkFramebuffer []) (commandBuffers: VkCommandBuffer []) renderPass graphicsPipeline (vertexBuffers: VkBuffer []) vertexCount instanceCount =
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

        vkCmdDraw(commandBuffer, vertexCount, instanceCount, 0u, 0u)

        // End render pass

        vkCmdEndRenderPass(commandBuffer)

        // Finish recording

        vkEndCommandBuffer(commandBuffer) |> checkResult
    )

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

[<NoEquality;NoComparison>]
type Shader =
    {
        vertexBindings: VkVertexInputBindingDescription []
        vertexAttributes: VkVertexInputAttributeDescription []

        vertex: VkShaderModule
        fragment: VkShaderModule
    }

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

let mkShader device vertexBindings vertexAttributes vert vertSize frag fragSize =
    let vertShaderModule = mkShaderModule device vert vertSize
    let fragShaderModule = mkShaderModule device frag fragSize
    {
        vertexBindings = vertexBindings
        vertexAttributes = vertexAttributes
        vertex = vertShaderModule
        fragment = fragShaderModule
    }

let destroyShaderGroup device group =
    vkDestroyShaderModule(device, group.vertex, vkNullPtr)
    vkDestroyShaderModule(device, group.fragment, vkNullPtr)

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

[<NoEquality;NoComparison>]
type SwapChainSupportDetails =
    {
        capabilities: VkSurfaceCapabilitiesKHR
        formats: VkSurfaceFormatKHR []
        presentModes: VkPresentModeKHR []
    }

let querySwapChainSupport physicalDevice surface =
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

let findSwapSurfaceFormat (formats: VkSurfaceFormatKHR []) =
    formats
    |> Array.find (fun x -> x.format = VkFormat.VK_FORMAT_B8G8R8A8_UNORM && x.colorSpace = VkColorSpaceKHR.VK_COLOR_SPACE_SRGB_NONLINEAR_KHR)

let getSwapPresentMode (formats: VkPresentModeKHR []) =
    formats
    |> Array.tryFind (fun x -> x = VkPresentModeKHR.VK_PRESENT_MODE_MAILBOX_KHR)
    |> Option.defaultValue VkPresentModeKHR.VK_PRESENT_MODE_FIFO_KHR // default is guaranteed

let getSwapExtent (capabilities: VkSurfaceCapabilitiesKHR) =
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

let mkSwapChain physicalDevice device surface indices =
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

let mkPipelineLayout device (layouts: VkDescriptorSetLayout []) =
    use pSetLayouts = fixed layouts
    let createInfo =
        VkPipelineLayoutCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
            setLayoutCount = uint32 layouts.Length,
            pSetLayouts = pSetLayouts,
            pushConstantRangeCount = 0u, // Optional
            pPushConstantRanges = vkNullPtr // Optional
        )

    let pipelineLayout = VkPipelineLayout ()
    vkCreatePipelineLayout(device, &&createInfo, vkNullPtr, &&pipelineLayout) |> checkResult
    pipelineLayout

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

[<Sealed>]
type FalSwapChain private (physicalDevice, device, surface, indices, commandPool, layout) =

    let gate = obj ()
    let mutable state = None
    let mutable currentFrame = 0
    let mutable isDisposed = 0

    let recordings = Collections.Generic.Dictionary<int, DrawRecording>()
    let shaders = ResizeArray ()
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
            pipelines.Add pipeline
        | _ ->
            failwith "should not happen"

    let record recording =
        let state = state.Value
        recordDraw 
            state.extent state.framebuffers state.commandBuffers state.renderPass 
            pipelines.[recording.pipelineIndex] recording.vertexBuffers recording.vertexCount recording.instanceCount

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
            let pipelineLayout = mkPipelineLayout device [|layout|]
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

            shaders
            |> Seq.iter (fun shader ->
                addPipeline shader
            )

            recordings.Values
            |> Seq.iter record

        finally
            Monitor.Exit gate

    member x.AddShader (vertexBindings, vertexAttributes, vertexBytes: ReadOnlySpan<byte>, fragmentBytes: ReadOnlySpan<byte>) =
        if not (Monitor.IsEntered gate) then
            Monitor.Enter gate

        try
            check ()

            let pVertexBytes = Unsafe.AsPointer(&Unsafe.AsRef(&vertexBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr
            let pFragmentBytes = Unsafe.AsPointer(&Unsafe.AsRef(&fragmentBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr

            let shader = 
                mkShader device 
                    vertexBindings vertexAttributes
                    pVertexBytes (uint32 vertexBytes.Length) 
                    pFragmentBytes (uint32 fragmentBytes.Length)

            let pipelineIndex = shaders.Count
            shaders.Add shader
            addPipeline shader
            pipelineIndex
        finally
            Monitor.Exit gate

    member x.RecordDraw(pipelineIndex, vertexBuffers, vertexCount, instanceCount) =
        lock gate |> fun _ ->

        check ()

        if pipelineIndex >= 0 && pipelineIndex < pipelines.Count then
            let recording =
                {
                    pipelineIndex = pipelineIndex
                    vertexBuffers = vertexBuffers
                    vertexCount = vertexCount
                    instanceCount = instanceCount
                }
            recordings.[pipelineIndex] <- recording
            record recording
        else
            failwith "Pipeline index is invalid."

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
                    shaders
                    |> Seq.rev
                    |> Seq.iter (fun group -> 
                        destroyShaderGroup device group
                    )

                    destroy ()
                )