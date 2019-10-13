[<AutoOpen>]
module internal Falkan.Helpers

open FSharp.Vulkan.Interop

let checkResult result =
    if result <> VkResult.VK_SUCCESS then
        failwithf "%A" result