[<AutoOpen>]
module FsGame.Graphics.Vulkan.SwapChain

open System
open System.Threading
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop
open System.Collections.Generic
open System.Collections.ObjectModel
open FsGame.Core.Collections
open InternalPipelineHelpers

#nowarn "9"
#nowarn "51"

[<NoEquality;NoComparison>]
type SwapChainState =
    {
        count: int
        extent: VkExtent2D
        swapChain: VkSwapchainKHR
        imageViews: VkImageView []
        imageDepthAttachment: FalkanImageDepthAttachment
        renderPass: VkRenderPass
        framebuffers: VkFramebuffer []
        commandBuffers: VkCommandBuffer []
    }

[<Struct;NoEquality;NoComparison>]
type ImageSampler =
    {
        vkImageView: VkImageView
        vkSampler: VkSampler
        vkDescriptorSets: VkDescriptorSet []
    }

[<NoEquality;NoComparison>]
type SwapChainSupportDetails =
    {
        capabilities: VkSurfaceCapabilitiesKHR
        formats: VkSurfaceFormatKHR []
        presentModes: VkPresentModeKHR []
    }

let querySwapChainSupport physicalDevice surface =
    let mutable capabilities = VkSurfaceCapabilitiesKHR ()
    vkGetPhysicalDeviceSurfaceCapabilitiesKHR(physicalDevice, surface, &&capabilities) |> checkResult

    let mutable formatCount = 0u
    vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, &&formatCount, vkNullPtr) |> checkResult

    let formats = Array.zeroCreate (int formatCount)
    use pFormats = fixed formats
    vkGetPhysicalDeviceSurfaceFormatsKHR(physicalDevice, surface, &&formatCount, pFormats) |> checkResult

    let mutable presentModeCount = 0u
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

let mkSwapChain physicalDevice device surface graphicsFamily presentFamily =
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

    let queueFamilyIndices = [|graphicsFamily;presentFamily|]
    let isConcurrent = graphicsFamily <> presentFamily
    use pQueueFamilyIndices = fixed queueFamilyIndices
    let mutable createInfo = 
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

    let mutable swapChain = VkSwapchainKHR ()
    vkCreateSwapchainKHR(device, &&createInfo, vkNullPtr, &&swapChain) |> checkResult

    // get images too

    let mutable imageCount = 0u
    vkGetSwapchainImagesKHR(device, swapChain, &&imageCount, vkNullPtr) |> checkResult

    let images = Array.zeroCreate (int imageCount)
    use pImages = fixed images
    vkGetSwapchainImagesKHR(device, swapChain, &&imageCount, pImages) |> checkResult

    swapChain, surfaceFormat, extent, images

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

let mkColorAttachmentRef n =
    VkAttachmentReference (
        attachment = n,
        layout = VkImageLayout.VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL
    )

let mkDepthAttachment () =
    VkAttachmentDescription (
        format = VkFormat.VK_FORMAT_D24_UNORM_S8_UINT,
        samples = VkSampleCountFlags.VK_SAMPLE_COUNT_1_BIT,
        loadOp = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_CLEAR,
        storeOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_STORE,
        stencilLoadOp = VkAttachmentLoadOp.VK_ATTACHMENT_LOAD_OP_DONT_CARE,
        stencilStoreOp = VkAttachmentStoreOp.VK_ATTACHMENT_STORE_OP_DONT_CARE,
        initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
        finalLayout = VkImageLayout.VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL
    )

let mkDepthAttachmentRef n =
    VkAttachmentReference (
        attachment = n,
        layout = VkImageLayout.VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL
    )

let mkColorDepthStencilSubpassDesc pColorAttachmentRefs colorAttachmentRefCount pDepthStencilAttachment =
    // The index of the attachment in this array is directly referenced from the fragment shader 
    // with the layout(location = 0) out vec4 outColor directive!
    VkSubpassDescription (
        pipelineBindPoint = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
        colorAttachmentCount = colorAttachmentRefCount,
        pColorAttachments = pColorAttachmentRefs,
        pDepthStencilAttachment = pDepthStencilAttachment
    )

let mkColorSubpassDesc pColorAttachmentRefs colorAttachmentRefCount =
    // The index of the attachment in this array is directly referenced from the fragment shader 
    // with the layout(location = 0) out vec4 outColor directive!
    VkSubpassDescription (
        pipelineBindPoint = VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS,
        colorAttachmentCount = colorAttachmentRefCount,
        pColorAttachments = pColorAttachmentRefs
    )

let mkRenderPass device (subpassDescs: VkSubpassDescription []) (attachmentDescs: VkAttachmentDescription []) =
    let mutable dependency = 
        VkSubpassDependency (
            srcSubpass = VK_SUBPASS_EXTERNAL,
            dstSubpass = 0u,
            srcStageMask = VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
            srcAccessMask = VkAccessFlags (),
            dstStageMask = VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT,
            dstAccessMask = (VkAccessFlags.VK_ACCESS_COLOR_ATTACHMENT_READ_BIT ||| VkAccessFlags.VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT)
        )

    use pSubpasses = fixed subpassDescs
    use pAttachments = fixed attachmentDescs
    let mutable createInfo =
        VkRenderPassCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO,
            attachmentCount = uint32 attachmentDescs.Length,
            pAttachments = pAttachments,
            subpassCount = uint32 subpassDescs.Length,
            pSubpasses = pSubpasses,
            dependencyCount = 1u,
            pDependencies = &&dependency
        )

    let mutable renderPass = VkRenderPass ()
    vkCreateRenderPass(device, &&createInfo, vkNullPtr, &&renderPass) |> checkResult
    renderPass

let mkPipelineLayout device (layouts: VkDescriptorSetLayout []) =
    use pSetLayouts = fixed layouts
    let mutable createInfo =
        VkPipelineLayoutCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO,
            setLayoutCount = uint32 layouts.Length,
            pSetLayouts = pSetLayouts,
            pushConstantRangeCount = 0u, // Optional
            pPushConstantRanges = vkNullPtr // Optional
        )

    let mutable pipelineLayout = VkPipelineLayout ()
    vkCreatePipelineLayout(device, &&createInfo, vkNullPtr, &&pipelineLayout) |> checkResult
    pipelineLayout

let mkFramebuffers device renderPass (extent: VkExtent2D) imageViews depthImageViews =
    (imageViews, depthImageViews)
    ||> Array.map2 (fun imageView depthImageView ->
        let attachments = [|imageView;depthImageView|]
        use pAttachments = fixed attachments
        let mutable imageView = imageView
        let mutable framebufferCreateInfo = 
            VkFramebufferCreateInfo (
                sType = VkStructureType.VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO,
                renderPass = renderPass,
                attachmentCount = uint32 attachments.Length,
                pAttachments = pAttachments,
                width = extent.width,
                height = extent.height,
                layers = 1u
            )

        let mutable framebuffer = VkFramebuffer ()
        vkCreateFramebuffer(device, &&framebufferCreateInfo, vkNullPtr, &&framebuffer) |> checkResult
        framebuffer)

[<NoEquality;NoComparison>]
type Sync =
    {
        imageAvailableSemaphores: VkSemaphore []
        renderFinishedSemaphores: VkSemaphore []
        inFlightFences: VkFence []
    }

[<Literal>]
let MaxFramesInFlight = 2

let mkSync device =
    let mutable semaphoreCreateInfo =
        VkSemaphoreCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO
        )

    let imageAvailableSemaphores =
        Array.init MaxFramesInFlight (fun _ ->
            let mutable imageAvailableSemaphore = VkSemaphore ()
            vkCreateSemaphore(device, &&semaphoreCreateInfo, vkNullPtr, &&imageAvailableSemaphore) |> checkResult
            imageAvailableSemaphore
        )

    let renderFinishedSemaphores =
        Array.init MaxFramesInFlight (fun _ ->
            let mutable renderFinishedSemaphore = VkSemaphore ()
            vkCreateSemaphore(device, &&semaphoreCreateInfo, vkNullPtr, &&renderFinishedSemaphore) |> checkResult
            renderFinishedSemaphore
        )

    let mutable fenceCreateInfo =
        VkFenceCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_FENCE_CREATE_INFO,
            flags = VkFenceCreateFlags.VK_FENCE_CREATE_SIGNALED_BIT // we need this so we do not lock up on the first 'drawFrame'
        )

    let inFlightFences = 
        Array.init MaxFramesInFlight (fun _ ->
            let mutable fence = VkFence ()
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

    let mutable imageIndex = 0u
    vkAcquireNextImageKHR(device, swapChain, UInt64.MaxValue, sync.imageAvailableSemaphores.[currentFrame], VK_NULL_HANDLE, &&imageIndex) |> checkResult

    let waitSemaphores = [|sync.imageAvailableSemaphores.[currentFrame]|]
    let waitStages = [|VkPipelineStageFlags.VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT|]

    let signalSemaphores = [|sync.renderFinishedSemaphores.[currentFrame]|]

    use pWaitSemaphores = fixed waitSemaphores
    use waitStages = fixed waitStages
    use pSignalSemaphores = fixed signalSemaphores
    use pCommandBuffers = fixed &commandBuffers.[int imageIndex]
    let mutable submitInfo =
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
    let mutable presentInfo =
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

let computeFrame device sync (commandBuffers: VkCommandBuffer []) computeQueue =
    use pFences = fixed &sync.inFlightFences.[0]
    vkWaitForFences(device, 1u, pFences, VK_TRUE, UInt64.MaxValue) |> checkResult
    vkResetFences(device, 1u, pFences) |> checkResult

    let waitSemaphores = [|sync.imageAvailableSemaphores.[0]|]
    let waitStages = [|VkPipelineStageFlags.VK_PIPELINE_STAGE_COMPUTE_SHADER_BIT|]

    let signalSemaphores = [|sync.renderFinishedSemaphores.[0]|]

    use pWaitSemaphores = fixed waitSemaphores
    use waitStages = fixed waitStages
    use pSignalSemaphores = fixed signalSemaphores
    use pCommandBuffers = fixed &commandBuffers.[0]
    let mutable submitInfo =
        VkSubmitInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            //waitSemaphoreCount = uint32 waitSemaphores.Length,
            //pWaitSemaphores = pWaitSemaphores,
            //pWaitDstStageMask = waitStages,
            commandBufferCount = 1u,
            pCommandBuffers = pCommandBuffers
            //signalSemaphoreCount = uint32 signalSemaphores.Length,
            //pSignalSemaphores = pSignalSemaphores
        )

    vkQueueSubmit(computeQueue, 1u, &&submitInfo, sync.inFlightFences.[0]) |> checkResult

type VulkanShaderDescriptorLayoutKind =
    | UniformBufferDescriptor
    | StorageBufferDescriptor
    | StorageBufferDynamicDescriptor
    | CombinedImageSamplerDescriptor

    member this.VkDescriptorType =
        match this with
        | UniformBufferDescriptor -> VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER
        | StorageBufferDescriptor -> VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER
        | StorageBufferDynamicDescriptor -> VkDescriptorType.VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC
        | CombinedImageSamplerDescriptor -> VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER

type VulkanShaderStage =
    | VertexStage
    | FragmentStage
    | AllGraphicsStage
    | ComputeStage

    member this.VkShaderStageFlags =
        match this with
        | VertexStage -> VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT
        | FragmentStage -> VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT
        | AllGraphicsStage -> VkShaderStageFlags.VK_SHADER_STAGE_ALL_GRAPHICS
        | ComputeStage -> VkShaderStageFlags.VK_SHADER_STAGE_COMPUTE_BIT

type VulkanShaderDescriptorLayout = ShaderDescriptorLayout of VulkanShaderDescriptorLayoutKind * VulkanShaderStage * binding: uint32 with

    member this.Kind =
        match this with
        | ShaderDescriptorLayout(kind, _, _) -> kind

    member this.Binding =
        match this with
        | ShaderDescriptorLayout(_, _, binding) -> binding

    member this.Build vkDevice =
        match this with
        | ShaderDescriptorLayout(kind, stage, binding) ->
            struct(mkDescriptorSetLayout vkDevice binding kind.VkDescriptorType stage.VkShaderStageFlags, kind.VkDescriptorType)

type VulkanShaderVertexInputRate =
    | PerVertex
    | PerInstance

type VulkanShaderVertexInput = ShaderVertexInput of VulkanShaderVertexInputRate * Type * binding: uint32 with

    member this.Build(location) =
        match this with
        | ShaderVertexInput(kind, ty, binding) ->
            let vkVertexInputRate =
                match kind with
                | PerVertex -> VkVertexInputRate.VK_VERTEX_INPUT_RATE_VERTEX
                | PerInstance -> VkVertexInputRate.VK_VERTEX_INPUT_RATE_INSTANCE
            let vkVertexInputBindingDescription = mkVertexInputBinding binding vkVertexInputRate ty
            let vkVertexInputAttributeDescriptions = mkVertexAttributeDescriptions location binding ty
            let lastLocation = (vkVertexInputAttributeDescriptions |> Array.last).location
            (vkVertexInputBindingDescription, vkVertexInputAttributeDescriptions), lastLocation

type VulkanShaderDescription = Shader of subpassIndex: int * enableDepth: bool * descriptors: VulkanShaderDescriptorLayout list * vertexInputs: VulkanShaderVertexInput list with

    member this.Build vkDevice =
        match this with
        | Shader(subpassIndex, enableDepth, descriptors, vertexInputs) ->
            let descriptors =
                descriptors
                |> List.map (fun x -> x.Build vkDevice)
                |> Array.ofList

            let bindingDescriptions, attributeDescriptions =
                let mutable location = 0u
                vertexInputs
                |> List.map (fun x -> 
                    let res, currentLocation = x.Build location
                    location <- currentLocation + 1u
                    res)
                |> Array.ofList
                |> Array.unzip

            let pipelineFlags = PipelineFlags.Default
            let pipelineFlags = if enableDepth then PipelineFlags.Depth else pipelineFlags

            (subpassIndex, pipelineFlags, descriptors, bindingDescriptions, attributeDescriptions)

[<Struct;NoComparison>]
type ShaderId = ShaderId of subpassIndex: int * Guid

type ShaderInput =
    | ShaderVertexInputBuffer of VkBuffer * VulkanShaderVertexInputRate
    | ShaderDescriptorInputBuffer of VkBuffer * size: uint64
    | ShaderDescriptorInputImage of FalkanImage

type FalkanRenderSubpassKind =
    | ColorSubpass
    | ColorDepthStencilSubpass

type FalkanRenderSubpassDescription = RenderSubpass of FalkanRenderSubpassKind with

    member this.Build(device, colorAttachmentRef: nativeptr<VkAttachmentReference>, depthAttachmentRef: nativeptr<VkAttachmentReference>) =
        match this with
        | RenderSubpass(kind) ->
            let subpassDesc =
                match kind with
                | ColorSubpass -> mkColorSubpassDesc colorAttachmentRef 1u
                | ColorDepthStencilSubpass -> mkColorDepthStencilSubpassDesc colorAttachmentRef 1u depthAttachmentRef

            subpassDesc

type FalkanRenderPassDescription = RenderPass of FalkanRenderSubpassDescription list with

    member this.Build(device, format) =
        match this with
        | RenderPass(fsubpassDescs) -> 
            let mutable colorAttachment = mkColorAttachment format
            let mutable colorAttachmentRef = mkColorAttachmentRef 0u
            let mutable depthAttachment = mkDepthAttachment ()
            let mutable depthAttachmentRef = mkDepthAttachmentRef 1u

            let subpassDescs = Array.zeroCreate fsubpassDescs.Length
            let mutable i = 0
            for fsubpassDesc in fsubpassDescs do
                let subpassDesc = fsubpassDesc.Build(device, &&colorAttachmentRef, &&depthAttachmentRef)
                subpassDescs.[i] <- subpassDesc
            mkRenderPass device subpassDescs [|colorAttachment;depthAttachment|]

[<Sealed>]
type FalkanShaderDrawVertexBuilder (inputs: ShaderInput list) =

    member _.Inputs = inputs
    
    member _.AddVertexBuffer(buffer: VulkanBuffer<_>, inputRate) =
        ShaderVertexInputBuffer(buffer.buffer, inputRate) :: inputs
        |> FalkanShaderDrawVertexBuilder
        
    member _.Build (vkDevice, vkDescriptorSetLayouts: struct(VkDescriptorSetLayout * VkDescriptorType) [], descriptorSetCount, vertexCount, instanceCount) =
        let inputs = inputs |> List.rev |> Array.ofList

        let descriptorSets, descriptorPools =
            let mutable i = 0
            inputs
            |> Array.choose (fun x ->
                match x with
                | ShaderDescriptorInputBuffer(buffer, size) ->    
                    let struct(vkDescriptorSetLayout, vkDescriptorType) = vkDescriptorSetLayouts.[i]
                    let vkDescriptorPool = mkDescriptorPool vkDevice vkDescriptorType descriptorSetCount
                    let vkDescriptorSetLayouts = Array.init (int descriptorSetCount) (fun _ -> vkDescriptorSetLayout)
                    let vkDescriptorSets = mkDescriptorSets vkDevice descriptorSetCount vkDescriptorPool vkDescriptorSetLayouts

                    vkDescriptorSets
                    |> Array.iter (fun d ->
                        let size = uint64 size
                        let mutable info = mkDescriptorBufferInfo buffer size
                        updateDescriptorSet vkDevice (uint32 i) d vkDescriptorType &&info
                        () (* prevent tail-call *))

                    i <- i + 1
                    Some(vkDescriptorSets, vkDescriptorPool)

                | ShaderDescriptorInputImage image ->    
                    let struct(vkDescriptorSetLayout, vkDescriptorType) = vkDescriptorSetLayouts.[i]
                    let vkDescriptorPool = mkDescriptorPool vkDevice vkDescriptorType descriptorSetCount
                    let vkDescriptorSetLayouts = Array.init (int descriptorSetCount) (fun _ -> vkDescriptorSetLayout)
                    let vkDescriptorSets = mkDescriptorSets vkDevice descriptorSetCount vkDescriptorPool vkDescriptorSetLayouts

                    vkDescriptorSets
                    |> Array.iter (fun d ->
                        let mutable info = mkDescriptorImageInfo image.vkImageView image.vkSampler
                        updateDescriptorImageSet vkDevice (uint32 i) d &&info
                        () (* prevent tail-call *))

                    i <- i + 1
                    Some(vkDescriptorSets, vkDescriptorPool)
                | _ ->
                    None)
            |> Array.unzip

        let vertexVkBuffers =
            inputs
            |> Array.choose (fun x ->
                match x with
                | ShaderVertexInputBuffer(buffer, PerVertex) -> Some buffer
                | _ -> None)

        let instanceVkBuffers =
            inputs
            |> Array.choose (fun x ->
                match x with
                | ShaderVertexInputBuffer(buffer, PerInstance) -> Some buffer
                | _ -> None)

        {
            vkDescriptorSets = descriptorSets
            vkDescriptorPools = descriptorPools
            vertexVkBuffers = vertexVkBuffers
            vertexCount = vertexCount
            instanceVkBuffers = instanceVkBuffers
            instanceCount = instanceCount
        }

[<Sealed>]
type FalkanShaderDrawDescriptorBuilder (inputs: ShaderInput list) =

    member _.Inputs = inputs

    member _.AddDescriptorBuffer(buffer: VulkanBuffer<'T>) =
         ShaderDescriptorInputBuffer(buffer.buffer, uint64 (sizeof<'T>)) :: inputs
        |> FalkanShaderDrawDescriptorBuilder

    member _.AddDescriptorImage(image: FalkanImage) =
         ShaderDescriptorInputImage image :: inputs
        |> FalkanShaderDrawDescriptorBuilder

    member _.Next = FalkanShaderDrawVertexBuilder inputs

[<RequireQualifiedAccess>]
module FalkanShaderDrawBuilder =

    let Create() =
        FalkanShaderDrawDescriptorBuilder []

type DeviceKind =
    | GraphicsDevice of surface: VkSurfaceKHR * graphicsFamily: uint32 * graphicsQueue: VkQueue * presentFamily: uint32 * presentQueue: VkQueue
    | ComputeDevice of computeFamily: uint32 * computeQueue: VkQueue

type FalkanShader = private FalkanShader of ShaderId * struct(VkDescriptorSetLayout * VkDescriptorType) [] * swapChain: SwapChain with

    member _.CreateDrawBuilder() =
        FalkanShaderDrawDescriptorBuilder []

    member this.AddDraw(drawBuilder: FalkanShaderDrawVertexBuilder, vertexCount, instanceCount) =
        match this with
        | FalkanShader(shaderId, descriptorSetLayouts, swapChain) ->
            let draw = drawBuilder.Build(swapChain.VkDevice, descriptorSetLayouts, swapChain.ImageCount, vertexCount, instanceCount)
            swapChain.RecordDraw(shaderId, draw)

    member this.RemoveDraw drawId =
        match this with
        | FalkanShader(shaderId, _, swapChain) ->
            swapChain.RemoveDraw(shaderId, drawId)

and [<Sealed>] SwapChain private (fdevice: VulkanDevice, sync, kind: DeviceKind, invalidate: IEvent<unit>) =

    let device = fdevice.Device
    let physicalDevice = fdevice.PhysicalDevice
    let gate = obj ()
    let mutable state = None
    let mutable currentFrame = 0
    let mutable isDisposed = 0
    let mutable isInvalidated = false

    let renderSubpassDescs = ResizeArray<FalkanRenderSubpassDescription>()
    let renderSubpassShaders = ResizeArray<Collections.Generic.Dictionary<Guid, struct(int * Shader * PipelineLayout)>>()
    let renderSubpassPipelines = ResizeArray<ResizeArray<Pipeline>>()

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
                imageDepthAttachment = imageDepthAttachment
                renderPass = renderPass
                framebuffers = framebuffers
                commandBuffers = commandBuffers
                } ->

        vkDeviceWaitIdle(device) |> checkResult

        currentFrame <- 0

        for pipelines in renderSubpassPipelines do
            pipelines
            |> Seq.rev
            |> Seq.iter (fun pipeline ->
                vkDestroyPipeline(device, pipeline.vkPipeline, vkNullPtr)
            )

            pipelines.Clear ()

        use pCommandBuffers = fixed commandBuffers
        vkFreeCommandBuffers(device, fdevice.VkCommandPool, uint32 commandBuffers.Length, pCommandBuffers)

        framebuffers
        |> Array.iter (fun framebuffer ->
            vkDestroyFramebuffer(device, framebuffer, vkNullPtr)
        )

        vkDestroyRenderPass(device, renderPass, vkNullPtr)

        imageDepthAttachment.Destroy()
        imageViews
        |> Array.iter (fun imageView ->
            vkDestroyImageView(device, imageView, vkNullPtr)
        )

        vkDestroySwapchainKHR(device, swapChain, vkNullPtr)

    let addPipeline subpassIndex pipelineLayout shader =
        match state with
        | Some state ->
            let vkPipeline = 
                match shader.fragment with
                | ValueSome _ ->
                    mkGraphicsPipeline device state.extent pipelineLayout.vkPipelineLayout state.renderPass subpassIndex pipelineLayout.pipelineFlags shader
                | _ ->
                    mkComputePipeline device pipelineLayout.vkPipelineLayout shader
            let pipeline = { layout = pipelineLayout; vkPipeline = vkPipeline }
            renderSubpassPipelines.[subpassIndex].Add pipeline
        | _ ->
            failwith "should not happen"

    let record () =
        let state = state.Value
        match kind with
        | GraphicsDevice _ ->
            recordDraw state.extent state.framebuffers state.commandBuffers state.imageDepthAttachment.vkImage state.renderPass renderSubpassPipelines
        | ComputeDevice _ ->
            recordComputeCommands state.commandBuffers renderSubpassPipelines

    do
        invalidate.Add(fun () -> isInvalidated <- true)

    member _.ImageCount = 
        check ()

        match state with
        | None -> failwith "Swap chain not initialized."
        | Some state -> state.count

    member _.VkDevice = device

    member x.Recreate () =
        if not (Monitor.IsEntered gate) then
            Monitor.Enter gate

        try
            checkDispose ()
            destroy ()

            match kind with
            | GraphicsDevice(surface, graphicsFamily, _, presentFamily, _) ->
                let swapChain, surfaceFormat, extent, images = mkSwapChain physicalDevice device surface graphicsFamily presentFamily
                let imageViews = mkImageViews device surfaceFormat.format images
                let imageDepthAttachment = fdevice.CreateImageDepthAttachment(int extent.width, int extent.height)
                let renderPass = (RenderPass (renderSubpassDescs |> Seq.toList)).Build(device, surfaceFormat.format)
                let framebuffers = mkFramebuffers device renderPass extent imageViews (Array.init imageViews.Length (fun _ -> imageDepthAttachment.vkImageView))
                let commandBuffers = mkCommandBuffers device fdevice.VkCommandPool framebuffers.Length

                isInvalidated <- false
                state <- 
                    { 
                        count = imageViews.Length
                        extent = extent
                        swapChain = swapChain
                        imageViews = imageViews
                        imageDepthAttachment = imageDepthAttachment
                        renderPass = renderPass
                        framebuffers = framebuffers
                        commandBuffers = commandBuffers
                    }
                    |> Some

                for shaders in renderSubpassShaders do
                    shaders.Values
                    |> Seq.iter (fun struct(subpassIndex, shader, pipelineLayout) -> 
                        addPipeline subpassIndex pipelineLayout shader
                    )

                record ()
            | ComputeDevice(computeFamily, _) ->
                let commandBuffers = mkCommandBuffers device fdevice.VkCommandPool 1

                isInvalidated <- false
                state <- 
                    { 
                        count = 1
                        extent = Unchecked.defaultof<_>
                        swapChain = Unchecked.defaultof<_>
                        imageViews = Unchecked.defaultof<_>
                        imageDepthAttachment = Unchecked.defaultof<_>
                        renderPass = Unchecked.defaultof<_>
                        framebuffers = Unchecked.defaultof<_>
                        commandBuffers = commandBuffers
                    }
                    |> Some

                for shaders in renderSubpassShaders do
                    shaders.Values
                    |> Seq.iter (fun struct(subpassIndex, shader, pipelineLayout) -> 
                        addPipeline subpassIndex pipelineLayout shader
                    )

                record ()

        finally
            Monitor.Exit gate

    member x.AddShader (subpassIndex, pipelineFlags, descriptorSetLayouts: struct(VkDescriptorSetLayout * VkDescriptorType) [], vertexBindings, vertexAttributes, vertexBytes: ReadOnlySpan<byte>, fragmentBytes: ReadOnlySpan<byte>) =
        if not (Monitor.IsEntered gate) then
            Monitor.Enter gate

        try
            check ()

            let pipelineLayout = 
                {
                    vkPipelineLayout = mkPipelineLayout device (descriptorSetLayouts |> Array.map (fun struct(x, _) -> x))
                    vkDescriptorSetLayouts = descriptorSetLayouts
                    pipelineFlags = pipelineFlags
                    draws = SparseResizeArray 1
                }

            let pVertexBytes = Unsafe.AsPointer(&Unsafe.AsRef(&vertexBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr
            let pFragmentBytes = Unsafe.AsPointer(&Unsafe.AsRef(&fragmentBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr

            let shader = 
                mkShader device 
                    vertexBindings vertexAttributes
                    pVertexBytes (uint32 vertexBytes.Length) 
                    pFragmentBytes (uint32 fragmentBytes.Length)

            let id = Guid.NewGuid()
            renderSubpassShaders.[subpassIndex].[id] <- struct(subpassIndex, shader, pipelineLayout)
            addPipeline subpassIndex pipelineLayout shader
            ShaderId(subpassIndex, id)
        finally 
            Monitor.Exit gate

    member x.AddComputeShader (pipelineFlags, descriptorSetLayouts: struct(VkDescriptorSetLayout * VkDescriptorType) [], vertexBindings, vertexAttributes, vertexBytes: ReadOnlySpan<byte>) =
        if not (Monitor.IsEntered gate) then
            Monitor.Enter gate

        try
            check ()

            let pipelineLayout = 
                {
                    vkPipelineLayout = mkPipelineLayout device (descriptorSetLayouts |> Array.map (fun struct(x, _) -> x))
                    vkDescriptorSetLayouts = descriptorSetLayouts
                    pipelineFlags = pipelineFlags
                    draws = SparseResizeArray 1
                }

            let pVertexBytes = Unsafe.AsPointer(&Unsafe.AsRef(&vertexBytes.GetPinnableReference())) |> NativePtr.ofVoidPtr

            let shader = 
                mkComputeShader device 
                    vertexBindings vertexAttributes
                    pVertexBytes (uint32 vertexBytes.Length) 

            let id = Guid.NewGuid()
            renderSubpassShaders.[0].[id] <- struct(0, shader, pipelineLayout)
            addPipeline 0 pipelineLayout shader
            ShaderId(0, id)
        finally 
            Monitor.Exit gate

    member x.RecordDraw((ShaderId(subpassIndex, id)): ShaderId, draw: Draw) =
        lock gate |> fun _ ->

        check ()

        match renderSubpassShaders.[subpassIndex].TryGetValue id with
        | false, _ -> failwith "Unable to find shader."
        | true, (_, _, pipelineLayout) ->
            pipelineLayout.draws.Add draw

    member x.RemoveDraw((ShaderId(subpassIndex, id)): ShaderId, drawId: int) =
        lock gate |> fun _ ->

        check ()

        match renderSubpassShaders.[subpassIndex].TryGetValue id with
        | false, _ -> failwith "Unable to find shader."
        | true, (_, _, pipelineLayout) ->
            pipelineLayout.draws.Remove drawId

    member x.SetupCommands() =
        check ()
        match kind with
        | GraphicsDevice(_, _, graphicsQueue, _, _) ->
            vkQueueWaitIdle(graphicsQueue) |> checkResult
        | ComputeDevice(_, computeQueue) ->
            vkQueueWaitIdle(computeQueue) |> checkResult
        record ()

    member _.AddRenderSubpass(renderSubpassDesc: FalkanRenderSubpassDescription) =
        lock gate |> fun _ ->

        renderSubpassDescs.Add renderSubpassDesc
        renderSubpassShaders.Add(System.Collections.Generic.Dictionary())
        renderSubpassPipelines.Add(ResizeArray())

    member x.Run () =
        lock gate |> fun _ ->

        check ()

        let state = state.Value
        let swapchain = state.swapChain
        let commandBuffers = state.commandBuffers

        match kind with
        | GraphicsDevice(_, _, graphicsQueue, _, presentQueue) ->
            let nextFrame, res = drawFrame device swapchain sync commandBuffers graphicsQueue presentQueue currentFrame

            if res = VkResult.VK_ERROR_OUT_OF_DATE_KHR || res = VkResult.VK_SUBOPTIMAL_KHR || isInvalidated then
                x.Recreate ()
            else
                checkResult res
                currentFrame <- nextFrame
        | ComputeDevice(_, computeQueue) ->
            computeFrame device sync commandBuffers computeQueue

    member _.WaitIdle () =
        check ()
        match kind with
        | GraphicsDevice(_, _, graphicsQueue, _, presentQueue) ->
            vkQueueWaitIdle(presentQueue) |> checkResult
            vkQueueWaitIdle(graphicsQueue) |> checkResult
        | ComputeDevice(_, computeQueue) ->
            vkQueueWaitIdle(computeQueue) |> checkResult
        vkDeviceWaitIdle(device) |> checkResult

    member this.CreateShader(shaderDesc: VulkanShaderDescription, vertexSpirvSource: ReadOnlySpan<byte>, fragmentSpirvSource: ReadOnlySpan<byte>) =
        let subpassIndex, pipelineFlags, descriptorSetLayouts, bindingDescriptions, attributeDescriptions = shaderDesc.Build device
        let id = this.AddShader(subpassIndex, pipelineFlags, descriptorSetLayouts, bindingDescriptions, attributeDescriptions |> Array.concat, vertexSpirvSource, fragmentSpirvSource)
        FalkanShader(id, descriptorSetLayouts, this)

    member this.CreateComputeShader(shaderDesc: VulkanShaderDescription, vertexSpirvSource: ReadOnlySpan<byte>) =
        let _subpassIndex, pipelineFlags, descriptorSetLayouts, bindingDescriptions, attributeDescriptions = shaderDesc.Build device
        let id = this.AddComputeShader(pipelineFlags, descriptorSetLayouts, bindingDescriptions, attributeDescriptions |> Array.concat, vertexSpirvSource)
        FalkanShader(id, descriptorSetLayouts, this)

    interface IDisposable with

        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                failwith "SwapChain already disposed"
            else
                GC.SuppressFinalize x

                lock gate (fun () ->
                    for shaders in renderSubpassShaders do
                        shaders.Values
                        |> Seq.iter (fun struct(_, shader, pipelineLayout) ->
                            for x in pipelineLayout.draws.AsSpan() do
                                x.vkDescriptorPools
                                |> Array.iter (fun descriptorPool -> vkDestroyDescriptorPool(device, descriptorPool, vkNullPtr))
                            pipelineLayout.vkDescriptorSetLayouts
                            |> Array.iter (fun struct(descriptorSetLayout, _) -> vkDestroyDescriptorSetLayout(device, descriptorSetLayout, vkNullPtr))
                            vkDestroyPipelineLayout(device, pipelineLayout.vkPipelineLayout, vkNullPtr)
                            vkDestroyShaderModule(device, shader.vertex, vkNullPtr)
                            match shader.fragment with
                            | ValueSome fragment ->
                                vkDestroyShaderModule(device, fragment, vkNullPtr)
                            | _ -> ()
                        )

                    destroy ()
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

    static member Create(device: VulkanDevice, surface, graphicsFamily, presentFamily, invalidate, renderSubpassDescs) =
        let sync = mkSync device.Device
        let graphicsQueue = mkQueue device.Device graphicsFamily
        let presentQueue = mkQueue device.Device presentFamily
        let swapChain = new SwapChain(device, sync, GraphicsDevice(surface, graphicsFamily, graphicsQueue, presentFamily, presentQueue), invalidate)
        renderSubpassDescs |> List.iter swapChain.AddRenderSubpass
        swapChain.Recreate ()
        swapChain

    static member CreateCompute(device: VulkanDevice, computeFamily, invalidate) =
        let sync = mkSync device.Device
        let computeQueue = mkQueue device.Device computeFamily
        let swapChain = new SwapChain(device, sync, ComputeDevice(computeFamily, computeQueue), invalidate)
        swapChain.AddRenderSubpass(RenderSubpass(ColorSubpass))
        swapChain.Recreate ()
        swapChain
