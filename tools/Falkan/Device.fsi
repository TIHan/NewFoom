[<AutoOpen>]
module Falkan.Device

open System
open FSharp.Vulkan.Interop

type QueueFamilyIndices =
    {
        graphicsFamily: uint32 option
        presentFamily: uint32 option
        computeFamily: uint32 option
        transferFamily: uint32 option
    }

    member HasGraphics: bool

    member HasCompute: bool

    member HasTransfer: bool

[<Sealed>]
type FalDevice =
    interface IDisposable

    member PhysicalDevice: VkPhysicalDevice

    member Device: VkDevice

    member Indices: QueueFamilyIndices

    member Surface: VkSurfaceKHR option

    static member Create: appName: string * engineName: string * validationLayers: string list * deviceExtensions: string list * ?mkSurface: (VkInstance -> VkSurfaceKHR) -> FalDevice