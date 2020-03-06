[<AutoOpen>]
module Falkan.FalkanCommand

open System
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

type FalkanCommandPool =
    {
        vkDevice: VkDevice
        vkCommandPool: VkCommandPool
    }

type FalDevice with

    member this.CreateCommandPool queueFamilyIndex =
        let vkCommandPool = mkCommandPool this.Device queueFamilyIndex
        { vkDevice = this.Device; vkCommandPool = vkCommandPool }