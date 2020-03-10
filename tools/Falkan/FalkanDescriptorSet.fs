[<AutoOpen>]
module Falkan.DescriptorSets

open System
open FSharp.Vulkan.Interop

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
            descriptorCount = size)

    let mutable poolInfo =
        VkDescriptorPoolCreateInfo(
            sType = VkStructureType.VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO,
            poolSizeCount = 1u,
            pPoolSizes = &&poolSize,
            maxSets = size)

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
