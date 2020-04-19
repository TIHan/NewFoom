[<AutoOpen>]
module FsGame.Graphics.Vulkan.VulkanDevice

open System
open FSharp.Vulkan.Interop
open InternalDeviceHelpers

// ===============================================================

[<Struct;NoEquality;NoComparison>]
type internal Block =
    {
        Offset: int
        Size: int
        Order: int
    }

[<Sealed>]
type internal Chunk

// ===============================================================

type VulkanDeviceLayer =
    | LunarGStandardValidation

type VulkanDeviceExtension =
    | ShaderFloat16Int8

/// Implicit vulkan device layers:
///     "VK_KHR_swapchain"
/// Vulkan version 1.1
[<Sealed>]
type VulkanDevice =
    interface IDisposable

    member internal PhysicalDevice: VkPhysicalDevice

    member internal Device: VkDevice

    member internal Indices: QueueFamilyIndices

    member internal Surface: VkSurfaceKHR option

    member internal VkCommandPool: VkCommandPool

    member internal VkTransferQueue: VkQueue

    static member CreateWin32 : hWnd: nativeint * hInstance: nativeint * appName: string * engineName: string * deviceLayers: VulkanDeviceLayer list * deviceExtensions: VulkanDeviceExtension list -> VulkanDevice


[<Struct;NoEquality;NoComparison>]
type VulkanMemory =
    internal {
        Device: VulkanDevice
        NativeDeviceMemory: VkDeviceMemory
        Block: Block
        Chunk: Chunk
        IsFree: bool ref
    }

    interface IDisposable

    static member internal Allocate : VulkanDevice -> VkMemoryRequirements -> VkMemoryPropertyFlags -> VulkanMemory

    member Map : offset: int * size: int -> nativeint

    [<RequiresExplicitTypeArguments>]
    member MapAsSpan<'T when 'T : unmanaged> : offset: int * count: int -> Span<'T>

    [<RequiresExplicitTypeArguments>]
    member MapAsSpan<'T when 'T : unmanaged> : count: int -> Span<'T>

    member Unmap : unit -> unit