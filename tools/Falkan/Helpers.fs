[<AutoOpen>]
module internal Falkan.Helpers

open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

let checkResult result =
    if result <> VkResult.VK_SUCCESS then
        failwithf "%A" result

let mkCommandBuffers device commandPool (framebuffers: VkFramebuffer []) =
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