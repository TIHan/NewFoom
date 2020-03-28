[<AutoOpen>]
module FsGame.Graphics.Vulkan.FalkanImage

open System
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let getImageMemoryRequirements device image =
    let mutable memRequirements = VkMemoryRequirements ()
    vkGetImageMemoryRequirements(device, image, &&memRequirements)
    memRequirements

let internal bindImage physicalDevice device image properties =
    let memRequirements = getImageMemoryRequirements device image
    let memory = VulkanMemory.Allocate physicalDevice device memRequirements properties
    vkBindImageMemory(device, image, memory.DeviceMemory, uint64 memory.Block.Offset) |> checkResult
    memory

let mkImage device width height format tiling usage =
    let mutable imageInfo =
        VkImageCreateInfo(
            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO,
            imageType = VkImageType.VK_IMAGE_TYPE_2D,
            extent = VkExtent3D(width = uint32 width, height = uint32 height, depth = 1u),
            mipLevels = 1u,
            arrayLayers = 1u,
            format = format,
            tiling = tiling,
            initialLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED,
            usage = usage,
            samples = VkSampleCountFlags.VK_SAMPLE_COUNT_1_BIT,
            sharingMode = VkSharingMode.VK_SHARING_MODE_EXCLUSIVE)

    let mutable image = VkImage()
    vkCreateImage(device, &&imageInfo, vkNullPtr, &&image) |> checkResult
    image

//let mkImageView device format image =
//    let mutable viewInfo =
//        VkImageViewCreateInfo(
//            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO,
//            image = image,
//            viewType = VkImageViewType.VK_IMAGE_VIEW_TYPE_2D,
//            format = format,
//            subresourceRange = VkImageSubresourceRange(aspectMask = VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT, baseMipLevel = 0u, levelCount = 1u, baseArrayLayer = 0u, layerCount = 1u))
        
//    let mutable imageView = VkImageView()
//    vkCreateImageView(device, &&viewInfo, vkNullPtr, &&imageView) |> checkResult
//    imageView

let mkSampler device =
    let mutable samplerInfo = 
        VkSamplerCreateInfo(
            sType = VkStructureType.VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO,
            magFilter = VkFilter.VK_FILTER_NEAREST,
            minFilter = VkFilter.VK_FILTER_NEAREST,
            addressModeU = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeV = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            addressModeW = VkSamplerAddressMode.VK_SAMPLER_ADDRESS_MODE_REPEAT,
            anisotropyEnable = VK_TRUE,
            maxAnisotropy = 16.f,
            borderColor = VkBorderColor.VK_BORDER_COLOR_INT_OPAQUE_BLACK,
            unnormalizedCoordinates = VK_FALSE,
            compareEnable = VK_FALSE,
            compareOp = VkCompareOp.VK_COMPARE_OP_ALWAYS,
            mipmapMode = VkSamplerMipmapMode.VK_SAMPLER_MIPMAP_MODE_LINEAR,
            mipLodBias = 0.f,
            minLod = 0.f,
            maxLod = 0.f)

    let mutable sampler = VkSampler()
    vkCreateSampler(device, &&samplerInfo, vkNullPtr, &&sampler) |> checkResult
    sampler

let transitionImageLayout (commandBuffer: VkCommandBuffer) image (format: VkFormat) oldLayout newLayout =
    let aspectMask, srcAccessMask, dstAccessMask, srcStage, dstStage =
        if oldLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED && newLayout = VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL then
            VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
            Unchecked.defaultof<_>, 
            VkAccessFlags.VK_ACCESS_TRANSFER_WRITE_BIT, 
            VkPipelineStageFlags.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            VkPipelineStageFlags.VK_PIPELINE_STAGE_TRANSFER_BIT
        elif oldLayout = VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL && newLayout = VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL then
            VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT,
            VkAccessFlags.VK_ACCESS_TRANSFER_WRITE_BIT, 
            VkAccessFlags.VK_ACCESS_SHADER_READ_BIT, 
            VkPipelineStageFlags.VK_PIPELINE_STAGE_TRANSFER_BIT,
            VkPipelineStageFlags.VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT
        elif oldLayout = VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED && newLayout = VkImageLayout.VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL then
            let aspectMask =
                if hasStencilComponent format then
                    VkImageAspectFlags.VK_IMAGE_ASPECT_DEPTH_BIT ||| VkImageAspectFlags.VK_IMAGE_ASPECT_STENCIL_BIT
                else
                    VkImageAspectFlags.VK_IMAGE_ASPECT_DEPTH_BIT
            aspectMask,
            Unchecked.defaultof<_>,
            (VkAccessFlags.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_READ_BIT ||| VkAccessFlags.VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT),
            VkPipelineStageFlags.VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT,
            VkPipelineStageFlags.VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT
        else
            failwith "Unsupported layout transition."
    let mutable barrier =
        VkImageMemoryBarrier(
            sType = VkStructureType.VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER,
            oldLayout = oldLayout,
            newLayout = newLayout,
            srcQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
            dstQueueFamilyIndex = VK_QUEUE_FAMILY_IGNORED,
            image = image,
            subresourceRange = VkImageSubresourceRange(aspectMask = aspectMask, baseMipLevel = 0u, levelCount = 1u, baseArrayLayer = 0u, layerCount = 1u),
            srcAccessMask = srcAccessMask,
            dstAccessMask = dstAccessMask)

    vkCmdPipelineBarrier(commandBuffer, srcStage, dstStage, Unchecked.defaultof<_>, 0u, vkNullPtr, 0u, vkNullPtr, 1u, &&barrier)
    aspectMask

let recordCopyImage (commandBuffer: VkCommandBuffer) format width height srcBuffer dstImage =
    let mutable beginInfo =
        VkCommandBufferBeginInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO,
            flags = VkCommandBufferUsageFlags.VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT,
            pInheritanceInfo = vkNullPtr
        )

    vkBeginCommandBuffer(commandBuffer, &&beginInfo) |> checkResult

    let aspectMask = transitionImageLayout commandBuffer dstImage format VkImageLayout.VK_IMAGE_LAYOUT_UNDEFINED VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL

    let mutable copyRegion =
        VkBufferImageCopy (
            bufferOffset = 0UL,
            bufferRowLength = 0u,
            bufferImageHeight = 0u,
            imageSubresource = VkImageSubresourceLayers(aspectMask = aspectMask, mipLevel = 0u, baseArrayLayer = 0u, layerCount = 1u),
            imageOffset = VkOffset3D(),
            imageExtent = VkExtent3D(width = uint32 width, height = uint32 height, depth = 1u))

    vkCmdCopyBufferToImage(commandBuffer, srcBuffer, dstImage, VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL, 1u, &&copyRegion)

    transitionImageLayout commandBuffer dstImage format VkImageLayout.VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL VkImageLayout.VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL |> ignore

    vkEndCommandBuffer(commandBuffer) |> checkResult

let copyImage device commandPool format width height srcBuffer dstImage queue =
    let mutable allocInfo =
        VkCommandBufferAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandPool = commandPool,
            commandBufferCount = 1u
        )

    let mutable commandBuffer = VkCommandBuffer()
    vkAllocateCommandBuffers(device, &&allocInfo, &&commandBuffer) |> checkResult
    recordCopyImage commandBuffer format width height srcBuffer dstImage

    let mutable submitInfo =
        VkSubmitInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_SUBMIT_INFO,
            commandBufferCount = 1u,
            pCommandBuffers = &&commandBuffer
        )

    vkQueueSubmit(queue, 1u, &&submitInfo, VK_NULL_HANDLE) |> checkResult
    vkQueueWaitIdle(queue) |> checkResult

    vkFreeCommandBuffers(device, commandPool, 1u, &&commandBuffer)

let fillImage physicalDevice device commandPool transferQueue (vkImage: VkImage) format width height (data: ReadOnlySpan<byte>) =
    // Memory that is not shared can not be written directly to from the CPU.
    // In order to set it from the CPU, a temporary shared memory buffer is used as a staging buffer to transfer the data.
    // This means that write times to local memory is more expensive but is highly-performant when read from the GPU.
    // Effectively, local memory is great for static data and shared memory is great for dynamic data.
    let stagingBuffer = mkBuffer device data.Length VkBufferUsageFlags.VK_BUFFER_USAGE_TRANSFER_SRC_BIT
    let stagingProperties = 
            VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT ||| 
            VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_HOST_COHERENT_BIT
    use stagingMemory = bindMemory physicalDevice device stagingBuffer stagingProperties

    mapMemory device stagingMemory.DeviceMemory stagingMemory.Block.Offset data
    copyImage device commandPool format width height stagingBuffer vkImage transferQueue

    vkDestroyBuffer(device, stagingBuffer, vkNullPtr)

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

[<Struct;NoComparison;NoEquality>]
type FalkanImageDepthAttachment =
    internal {
        vkDevice: VkDevice
        vkImage: VkImage
        vkImageView: VkImageView
        memory: VulkanMemory
        width: int
        height: int
    }

    member internal this.Destroy() =
        vkDestroyImageView(this.vkDevice, this.vkImageView, vkNullPtr)
        vkDestroyImage(this.vkDevice, this.vkImage, vkNullPtr)

type VulkanDevice with

    member this.CreateImageDepthAttachment(width, height) =
        let image = 
            mkImage this.Device width height 
                VkFormat.VK_FORMAT_D24_UNORM_S8_UINT
                VkImageTiling.VK_IMAGE_TILING_OPTIMAL 
                VkImageUsageFlags.VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT
        let memory = bindImage this.PhysicalDevice this.Device image VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
        let imageView = mkImageView this.Device VkFormat.VK_FORMAT_D24_UNORM_S8_UINT image

        { vkDevice = this.Device
          vkImage = image
          vkImageView = imageView
          memory = memory
          width = width
          height = height }

[<Struct;NoComparison;NoEquality>]
type FalkanImage =
    internal {
        vkDevice: VkDevice
        vkImage: VkImage
        vkImageView: VkImageView
        vkSampler: VkSampler
        memory: VulkanMemory
        format: VkFormat
        width: int
        height: int
    //    depthAttachment: FalkanImageDepthAttachment
    }

    member this.Width = int this.width

    member this.Height = int this.height

    member internal this.Destroy() =
        vkDestroySampler(this.vkDevice, this.vkSampler, vkNullPtr)
        vkDestroyImageView(this.vkDevice, this.vkImageView, vkNullPtr)
        vkDestroyImage(this.vkDevice, this.vkImage, vkNullPtr)

let defaultImageFormat = VkFormat.VK_FORMAT_B8G8R8A8_UNORM

type VulkanDevice with

    member this.CreateImage(width, height) =
        let image = 
            mkImage this.Device width height 
                defaultImageFormat
                VkImageTiling.VK_IMAGE_TILING_OPTIMAL 
                (VkImageUsageFlags.VK_IMAGE_USAGE_TRANSFER_DST_BIT ||| VkImageUsageFlags.VK_IMAGE_USAGE_SAMPLED_BIT)
        let memory = bindImage this.PhysicalDevice this.Device image VkMemoryPropertyFlags.VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT
        let imageView = mkImageView this.Device defaultImageFormat image
        let sampler = mkSampler this.Device

      //  let depthAttachment = this.CreateImageDepthAttachment(width, height)

        { vkDevice = this.Device
          vkImage = image
          vkImageView = imageView
          vkSampler = sampler
          memory = memory
          format = defaultImageFormat
          width = width
          height = height }