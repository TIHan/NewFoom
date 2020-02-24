[<AutoOpen>]
module internal Falkan.SwapChain

open System
open FSharp.Vulkan.Interop

type PipelineIndex = int

[<Sealed>]
type SwapChain =
    interface IDisposable

    member AddShader: vertexBindings: VkVertexInputBindingDescription [] * vertexAttributes: VkVertexInputAttributeDescription [] * ReadOnlySpan<byte> * fragmentBytes: ReadOnlySpan<byte> -> PipelineIndex

    member SetUniformBuffer: VkBuffer * size: int -> unit

    member SetSampler: VkImageView * VkSampler -> unit

    member RecordDraw: pipelineIndex: PipelineIndex * vertexBuffers: VkBuffer [] * vertexCount: int * instanceCount: int -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    static member Create : VkPhysicalDevice * VkDevice * VkSurfaceKHR * graphicsFamily: uint32 * presentFamily: uint32 * VkCommandPool -> SwapChain
