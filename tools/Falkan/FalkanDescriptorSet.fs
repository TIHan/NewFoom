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

type FalkanDescriptorKind =
    | UniformBufferDescriptor
    | CombinedImageSamplerDescriptor

    member this.VkDescriptorType =
        match this with
        | UniformBufferDescriptor -> VkDescriptorType.VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER
        | CombinedImageSamplerDescriptor -> VkDescriptorType.VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER

[<Struct;NoEquality;NoComparison>]
type FalkanDescriptorPool =
    {
        vkDevice: VkDevice
        vkDescriptorPool: VkDescriptorPool
        vkDescriptorType: VkDescriptorType
    }

    interface IDisposable with

        member this.Dispose() =
            vkDestroyDescriptorPool(this.vkDevice, this.vkDescriptorPool, vkNullPtr)

type FalDevice with

    member this.CreateDescriptorPool(descriptorKind: FalkanDescriptorKind, poolSize: int) =
        let vkDescriptorType = descriptorKind.VkDescriptorType
        let vkDescriptorPool = mkDescriptorPool this.Device vkDescriptorType (uint32 poolSize)
        { vkDevice = this.Device; vkDescriptorPool = vkDescriptorPool; vkDescriptorType = vkDescriptorType }

[<Struct;NoEquality;NoComparison>]
type FalkanDescriptorSetLayout =
    {
        vkDevice: VkDevice
        vkDescriptorPool: VkDescriptorPool
        vkDescriptorType: VkDescriptorType
        vkDescriptorSetLayout: VkDescriptorSetLayout
        binding: uint32
    }

    interface IDisposable with

        member this.Dispose() =
            vkDestroyDescriptorSetLayout(this.vkDevice, this.vkDescriptorSetLayout, vkNullPtr)

type FalkanShaderStageKind =
    | VertexStage
    | FragmentStage

    member this.VkShaderStageFlags =
        match this with
        | VertexStage -> VkShaderStageFlags.VK_SHADER_STAGE_VERTEX_BIT
        | FragmentStage -> VkShaderStageFlags.VK_SHADER_STAGE_FRAGMENT_BIT

type FalkanDescriptorPool with

    member this.CreateSetLayout(shaderStageKind: FalkanShaderStageKind, binding) =
        let vkDescriptorSetLayout = mkDescriptorSetLayout this.vkDevice binding this.vkDescriptorType shaderStageKind.VkShaderStageFlags
        { vkDevice = this.vkDevice
          vkDescriptorSetLayout = vkDescriptorSetLayout
          vkDescriptorPool = this.vkDescriptorPool
          vkDescriptorType = this.vkDescriptorType
          binding = binding }
        
[<Struct;NoEquality;NoComparison>]
type FalkanDescriptorSets =
    {
        vkDevice: VkDevice
        vkDescriptorType: VkDescriptorType
        binding: uint32
        vkDescriptorSets: VkDescriptorSet []
    }

type FalkanDescriptorSetLayout with

    member this.CreateDescriptorSets(count: int) =
        let vkDescriptorSetLayout = this.vkDescriptorSetLayout
        let vkDescriptorSetLayouts = Array.init (int count) (fun _ -> vkDescriptorSetLayout)
        let vkDescriptorSets = mkDescriptorSets this.vkDevice count this.vkDescriptorPool vkDescriptorSetLayouts
        { vkDevice = this.vkDevice
          vkDescriptorSets = vkDescriptorSets
          vkDescriptorType = this.vkDescriptorType
          binding = this.binding }