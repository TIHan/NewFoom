﻿[<AutoOpen>]
module internal Falkan.Helpers

open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let checkResult result =
    if result <> VkResult.VK_SUCCESS then
        failwithf "%A" result

let mkQueue device familyIndex =
    let mutable queue = VkQueue()
    vkGetDeviceQueue(device, familyIndex, 0u, &&queue)
    queue

let mkCommandPool device queueFamilyIndex =
    let mutable poolCreateInfo =
        VkCommandPoolCreateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO,
            queueFamilyIndex = queueFamilyIndex,
            flags = VkCommandPoolCreateFlags () // Optional
        )

    let mutable commandPool = VkCommandPool ()
    vkCreateCommandPool(device, &&poolCreateInfo, vkNullPtr, &&commandPool) |> checkResult
    commandPool

let mkCommandBuffers device commandPool (framebuffers: VkFramebuffer []) =
    let commandBuffers = Array.zeroCreate framebuffers.Length

    let mutable allocCreateInfo =
        VkCommandBufferAllocateInfo (
            sType = VkStructureType.VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO,
            commandPool = commandPool,
            level = VkCommandBufferLevel.VK_COMMAND_BUFFER_LEVEL_PRIMARY,
            commandBufferCount = uint32 commandBuffers.Length
        )

    use pCommandBuffers = fixed commandBuffers
    vkAllocateCommandBuffers(device, &&allocCreateInfo, pCommandBuffers) |> checkResult
    commandBuffers

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
    let mutable createInfo = mkImageViewCreateInfo format image
    let mutable imageView = VkImageView()
    vkCreateImageView(device, &&createInfo, vkNullPtr, &&imageView) |> checkResult
    imageView
    
let mkImageViews device surfaceFormat images =
    images
    |> Array.map (mkImageView device surfaceFormat)