[<AutoOpen>]
module internal FsGame.Graphics.Vulkan.Helpers

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
            flags = VkCommandPoolCreateFlags.VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT // Optional
        )

    let mutable commandPool = VkCommandPool ()
    vkCreateCommandPool(device, &&poolCreateInfo, vkNullPtr, &&commandPool) |> checkResult
    commandPool

let mkCommandBuffers device commandPool count =
    let commandBuffers = Array.zeroCreate count

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

let hasDepthComponent format =
    format = VkFormat.VK_FORMAT_D32_SFLOAT_S8_UINT || format = VkFormat.VK_FORMAT_D24_UNORM_S8_UINT || format = VkFormat.VK_FORMAT_D32_SFLOAT

let hasStencilComponent format =
    format = VkFormat.VK_FORMAT_D32_SFLOAT_S8_UINT || format = VkFormat.VK_FORMAT_D24_UNORM_S8_UINT

let getAspectMask format =
    if hasStencilComponent format then
        VkImageAspectFlags.VK_IMAGE_ASPECT_DEPTH_BIT ||| VkImageAspectFlags.VK_IMAGE_ASPECT_STENCIL_BIT
    elif hasDepthComponent format then
        VkImageAspectFlags.VK_IMAGE_ASPECT_DEPTH_BIT
    else
        VkImageAspectFlags.VK_IMAGE_ASPECT_COLOR_BIT

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
            aspectMask = getAspectMask format,
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