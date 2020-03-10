[<AutoOpen>]
module Falkan.Device

open System
open FSharp.Vulkan.Interop
open InternalDeviceHelpers

type VulkanDeviceLayer =
    | LunarGStandardValidation

type VulkanDeviceExtension =
    | ShaderFloat16Int8

/// Implicit vulkan device layers:
///     "VK_KHR_swapchain"
/// Vulkan version 1.1
[<Sealed>]
type FalDevice =
    interface IDisposable

    member internal PhysicalDevice: VkPhysicalDevice

    member internal Device: VkDevice

    member internal Indices: QueueFamilyIndices

    member internal Surface: VkSurfaceKHR option

    member internal VkCommandPool: VkCommandPool

    member internal VkTransferQueue: VkQueue

    static member CreateWin32 : hWnd: nativeint * hInstance: nativeint * appName: string * engineName: string * deviceLayers: VulkanDeviceLayer list * deviceExtensions: VulkanDeviceExtension list -> FalDevice