module internal Falkan.InternalPipelineHelpers

open System
open System.Threading
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop
open System.Collections.Generic
open System.Collections.ObjectModel

#nowarn "9"
#nowarn "51"

let mkDescriptorSetLayout device binding typ stageFlags =
    let mutable binding =
        VkDescriptorSetLayoutBinding (
            binding = binding,
            descriptorType = typ,
            descriptorCount = 1u,
            stageFlags = stageFlags,
            pImmutableSamplers = vkNullPtr // Optional, for image samplers
        )

    let mutable layoutInfo =
        VkDescriptorSetLayoutCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO,
            bindingCount = 1u,
            pBindings = &&binding
        )

    let mutable descriptorSetLayout = VkDescriptorSetLayout ()
    vkCreateDescriptorSetLayout(device, &&layoutInfo, vkNullPtr, &&descriptorSetLayout) |> checkResult
    descriptorSetLayout

let mkDescriptorPool device typ size =
    let mutable poolSize =
        VkDescriptorPoolSize(
            typ = typ,
            descriptorCount = uint32 size)

    let mutable poolInfo =
        VkDescriptorPoolCreateInfo(
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO,
            poolSizeCount = 1u,
            pPoolSizes = &&poolSize,
            maxSets = uint32 size)

    let mutable descriptorPool = VkDescriptorPool()
    vkCreateDescriptorPool(device, &&poolInfo, vkNullPtr, &&descriptorPool) |> checkResult
    descriptorPool

let mkDescriptorSets device size descriptorPool (setLayouts: VkDescriptorSetLayout []) =
    use pSetLayouts = fixed setLayouts
    let mutable allocInfo =
        VkDescriptorSetAllocateInfo(
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO,
            descriptorPool = descriptorPool,
            descriptorSetCount = uint32 size,
            pSetLayouts = pSetLayouts)

    let descriptorSets = Array.zeroCreate size
    use pDescriptorSets = fixed descriptorSets
    vkAllocateDescriptorSets(device, &&allocInfo, pDescriptorSets) |> checkResult
    descriptorSets

let mkDescriptorBufferInfo uniformBuffer size =
    let mutable bufferInfo =
        VkDescriptorBufferInfo(
            buffer = uniformBuffer,
            offset = 0UL,
            range = uint64 size)
    bufferInfo

let updateDescriptorSet device descriptorSet pBufferInfo =
    let mutable descriptorWrite =
        VkWriteDescriptorSet(
            sType = VkStructureType.VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET,
            dstSet = descriptorSet,
            dstBinding = 0u,
            dstArrayElement = 0u,
            descriptorType = VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER,
            descriptorCount = 1u,
            pBufferInfo = pBufferInfo,
            pImageInfo = vkNullPtr, // optional
            pTexelBufferView = vkNullPtr (* optional *))

    vkUpdateDescriptorSets(device, 1u, &&descriptorWrite, 0u, vkNullPtr)

let mkDescriptorImageInfo imageView sampler =
    let mutable imageInfo =
        VkDescriptorImageInfo(
            imageLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL,
            imageView = imageView,
            sampler = sampler)
    imageInfo

let updateDescriptorImageSet device descriptorSet pImageInfo =
    let mutable descriptorWrite =
        VkWriteDescriptorSet(
            sType = VkStructureType.VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET,
            dstSet = descriptorSet,
            dstBinding = 1u,
            dstArrayElement = 0u,
            descriptorType = VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER,
            descriptorCount = 1u,
            pImageInfo = pImageInfo)

    vkUpdateDescriptorSets(device, 1u, &&descriptorWrite, 0u, vkNullPtr)

[<Struct;NoEquality;NoComparison>]
type Draw =
    {
        vkDescriptorSets: VkDescriptorSet [][]
        vkDescriptorPools: VkDescriptorPool []
        vertexVkBuffers: VkBuffer []
        vertexCount: uint32
        instanceCount: uint32
    }

[<Struct;NoEquality;NoComparison>]
type PipelineLayout =
    {
        vkPipelineLayout: VkPipelineLayout
        vkDescriptorSetLayouts: struct(VkDescriptorSetLayout * VkDescriptorType) []
        draws: ResizeArray<Draw>
    }

[<Struct;NoEquality;NoComparison>]
type Pipeline =
    {
        vkPipeline: VkPipeline
        layout: PipelineLayout
    }

type PipelineIndex = int

let recordDraw extent (framebuffers: VkFramebuffer []) (commandBuffers: VkCommandBuffer []) renderPass (pipelines: Pipeline seq) =
    for i = 0 to framebuffers.Length - 1 do
        let framebuffer = framebuffers.[i]
        let commandBuffer = commandBuffers.[i]

        // Begin command buffer

        let mutable beginInfo =
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

        let clearDepthStencil = 
            let depthStencil = VkClearDepthStencilValue(depth = 1.f, stencil = uint32 0)
            VkClearValue (depthStencil = depthStencil)

        let clearValues = [|clearColor;clearDepthStencil|]
        use pClearValues = fixed clearValues

        let mutable beginInfo =
            VkRenderPassBeginInfo (
                sType = VkStructureType.VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO,
                renderPass = renderPass,
                framebuffer = framebuffer,
                renderArea = VkRect2D (offset = VkOffset2D (x = 0, y = 0), extent = extent),
                clearValueCount = uint32 clearValues.Length,
                pClearValues = pClearValues
            )

        vkCmdBeginRenderPass(commandBuffer, &&beginInfo, VkSubpassContents.VK_SUBPASS_CONTENTS_INLINE)

        for pipeline in pipelines do

            // Bind graphics pipeline

            vkCmdBindPipeline(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, pipeline.vkPipeline)


            for draw in pipeline.layout.draws do

                // REVIEW: Might be expensive per draw but it works.
                // Bind descriptor sets

                let sets =
                    draw.vkDescriptorSets
                    |> Array.map (fun x -> x.[i])
                use pDescriptorSets = fixed sets
                vkCmdBindDescriptorSets(commandBuffer, VkPipelineBindPoint.VK_PIPELINE_BIND_POINT_GRAPHICS, pipeline.layout.vkPipelineLayout, 0u, uint32 draw.vkDescriptorSets.Length, pDescriptorSets, 0u, vkNullPtr)

                // Bind vertex buffers
        
                if draw.vertexVkBuffers |> Array.isEmpty |> not then          
                    let offsets = draw.vertexVkBuffers |> Array.map (fun _ -> 0UL) // REVIEW: Doesn't allocate much at all, but maybe a way to get rid of it regardless?
                    use pOffsets = fixed offsets
                    use pVertexBuffers = fixed draw.vertexVkBuffers
                    vkCmdBindVertexBuffers(commandBuffer, 0u, uint32 draw.vertexVkBuffers.Length, pVertexBuffers, pOffsets)

                // Draw

                vkCmdDraw(commandBuffer, draw.vertexCount, draw.instanceCount, 0u, 0u)

        // End render pass

        vkCmdEndRenderPass(commandBuffer)

        // Finish recording

        vkEndCommandBuffer(commandBuffer) |> checkResult


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
        y = float32 extent.height,
        width = float32 extent.width,
        height = -float32 extent.height,
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
        frontFace = VkFrontFace.VK_FRONT_FACE_COUNTER_CLOCKWISE,
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
    VkPipelineDepthStencilStateCreateInfo (
        sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO,
        depthTestEnable = VK_TRUE,
        depthWriteEnable = VK_TRUE,
        depthCompareOp = VkCompareOp.VK_COMPARE_OP_LESS,
        depthBoundsTestEnable = VK_FALSE,
        minDepthBounds = 0.f, // optional
        maxDepthBounds = 0.f, // optional
        front = Unchecked.defaultof<_>, // optional
        back = Unchecked.defaultof<_>, // optional
        stencilTestEnable = VK_FALSE)

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
    let mutable createInfo = 
        VkShaderModuleCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO,
            codeSize = unativeint codeSize,
            pCode = vkCastPtr code
        )

    let mutable shaderModule = VkShaderModule ()
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

let mkGraphicsPipeline device extent pipelineLayout renderPass group =
    use pNameMain = fixed vkBytesOfString "main"

    let stages =
        [|
            mkShaderStageInfo VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT pNameMain group.vertex
            mkShaderStageInfo VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT pNameMain group.fragment
        |]

    use pVertexBindingDescriptions = fixed group.vertexBindings
    use pVertexAttributeDescriptions = fixed group.vertexAttributes
        
    let mutable vertexInputCreateInfo =
        VkPipelineVertexInputStateCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO,
            vertexBindingDescriptionCount = uint32 group.vertexBindings.Length,
            pVertexBindingDescriptions = pVertexBindingDescriptions,
            vertexAttributeDescriptionCount = uint32 group.vertexAttributes.Length,
            pVertexAttributeDescriptions = pVertexAttributeDescriptions
        )

    let mutable inputAssemblyCreateInfo = mkInputAssemblyCreateInfo ()

    let mutable viewport = mkViewport extent
    let mutable scissor = mkScissor extent
    let mutable viewportStateCreateInfo = mkViewportStateCreateInfo &&viewport &&scissor

    let mutable rasterizerCreateInfo = mkRasterizerCreateInfo ()
    let mutable multisampleCreateInfo = mkMultisampleCreateInfo ()
    let mutable depthStencilCreateInfo = mkDepthStencilCreateInfo ()

    let mutable colorBlendAttachment = mkColorBlendAttachment ()
    let mutable colorBlendCreateInfo = mkColorBlendCreateInfo &&colorBlendAttachment 1u

    // TODO: dynamic state

    use pStages = fixed stages
    let mutable createInfo =
        VkGraphicsPipelineCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO,
            stageCount = uint32 stages.Length,
            pStages = pStages,
                
            pVertexInputState = &&vertexInputCreateInfo,
            pInputAssemblyState = &&inputAssemblyCreateInfo,
            pViewportState = &&viewportStateCreateInfo,
            pRasterizationState = &&rasterizerCreateInfo,
            pMultisampleState = &&multisampleCreateInfo,
            pDepthStencilState = &&depthStencilCreateInfo,
            pColorBlendState = &&colorBlendCreateInfo,
            pDynamicState = vkNullPtr, // Optional
            layout = pipelineLayout,
            renderPass = renderPass,
            subpass = 0u,

            basePipelineHandle = VK_NULL_HANDLE, // Optional
            basePipelineIndex = -1 // Optional
        )

    let mutable graphicsPipeline = VkPipeline ()
    vkCreateGraphicsPipelines(device, VK_NULL_HANDLE, 1u, &&createInfo, vkNullPtr, &&graphicsPipeline) |> checkResult
    graphicsPipeline