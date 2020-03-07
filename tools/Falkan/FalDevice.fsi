[<AutoOpen>]
module Falkan.Device

open System
open FSharp.Vulkan.Interop

type internal QueueFamilyIndices =
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

    member internal PhysicalDevice: VkPhysicalDevice

    member internal Device: VkDevice

    member internal Indices: QueueFamilyIndices

    member internal Surface: VkSurfaceKHR option

    member internal VkCommandPool: VkCommandPool

    member internal VkTransferQueue: VkQueue

    static member internal Create: appName: string * engineName: string * validationLayers: string list * deviceExtensions: string list * ?mkSurface: (VkInstance -> VkSurfaceKHR) -> FalDevice

    static member CreateWin32Surface : HWND * HINSTANCE * appName: string * engineName: string * validationLayers: string list * deviceExtensions: string list -> FalDevice