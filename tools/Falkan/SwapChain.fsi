[<AutoOpen>]
module FsGame.Graphics.Vulkan.SwapChain

open System
open FSharp.Vulkan.Interop

type VulkanShaderDescriptorLayoutKind =
    | UniformBufferDescriptor
    | StorageBufferDescriptor
    | StorageBufferDynamicDescriptor
    | CombinedImageSamplerDescriptor

type VulkanShaderStage =
    | VertexStage
    | FragmentStage

type VulkanShaderDescriptorLayout = ShaderDescriptorLayout of VulkanShaderDescriptorLayoutKind * VulkanShaderStage * binding: uint32

type VulkanShaderVertexInputRate =
    | PerVertex
    | PerInstance

type VulkanShaderVertexInput = ShaderVertexInput of VulkanShaderVertexInputRate * Type * binding: uint32

type VulkanShaderDescription = Shader of subpassIndex: int * enableDepth: bool * descriptors: VulkanShaderDescriptorLayout list * vertexInputs: VulkanShaderVertexInput list

type FalkanRenderSubpassKind =
    | ColorSubpass
    | ColorDepthStencilSubpass

type FalkanRenderSubpassDescription = RenderSubpass of FalkanRenderSubpassKind

[<Sealed>]
type FalkanShaderDrawVertexBuilder =

    member AddVertexBuffer : VulkanBuffer<'T> * VulkanShaderVertexInputRate -> FalkanShaderDrawVertexBuilder

[<Sealed>]
type FalkanShaderDrawDescriptorBuilder =

    member AddDescriptorBuffer : VulkanBuffer<'T> -> FalkanShaderDrawDescriptorBuilder

    member AddDescriptorImage : FalkanImage -> FalkanShaderDrawDescriptorBuilder

    member Next : FalkanShaderDrawVertexBuilder

[<RequireQualifiedAccess>]
module FalkanShaderDrawBuilder =

    val Create : unit -> FalkanShaderDrawDescriptorBuilder

[<Sealed>]
type FalkanShader =

    member CreateDrawBuilder : unit -> FalkanShaderDrawDescriptorBuilder

    member AddDraw : drawBuilder: FalkanShaderDrawVertexBuilder * vertexCount: uint32 * instanceCount: uint32 -> int

    member RemoveDraw : drawId: int -> unit

[<Sealed>]
type internal SwapChain =
    interface IDisposable

    member DrawFrame: unit -> unit

    member SetupCommands: unit -> unit

    member WaitIdle: unit -> unit

    member CreateShader: shaderDesc: VulkanShaderDescription * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader

    member AddRenderSubpass : FalkanRenderSubpassDescription -> unit

    static member Create : VulkanDevice * VkSurfaceKHR * graphicsFamily: uint32 * presentFamily: uint32 * invalidate: IEvent<unit> * renderSubpasDescs: FalkanRenderSubpassDescription list -> SwapChain
