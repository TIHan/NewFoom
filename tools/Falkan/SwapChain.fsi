[<AutoOpen>]
module Falkan.SwapChain

open System
open FSharp.Vulkan.Interop

type FalkanShaderDescriptorLayoutKind =
    | UniformBufferDescriptor
    | CombinedImageSamplerDescriptor

type FalkanShaderStage =
    | VertexStage
    | FragmentStage

type FalkanShaderDescriptorLayout = FalkanShaderDescriptorLayout of FalkanShaderDescriptorLayoutKind * FalkanShaderStage * binding: uint32

type FalkanShaderVertexInputKind =
    | PerVertex
    | PerInstance

type FalkanShaderVertexInput = FalkanShaderVertexInput of FalkanShaderVertexInputKind * binding: uint32 * location: uint32 * Type

type FalkanShaderDescription = Shader of subpassIndex: int * enableDepth: bool * descriptors: FalkanShaderDescriptorLayout list * vertexInputs: FalkanShaderVertexInput list

type FalkanRenderSubpassKind =
    | ColorSubpass
    | ColorDepthStencilSubpass

type FalkanRenderSubpassDescription = RenderSubpass of FalkanRenderSubpassKind

[<Sealed>]
type FalkanShaderDrawVertexBuilder =

    member AddVertexBuffer : FalkanBuffer -> FalkanShaderDrawVertexBuilder

[<Sealed>]
type FalkanShaderDrawDescriptorBuilder =

    member AddDescriptorBuffer : FalkanBuffer * size: int -> FalkanShaderDrawDescriptorBuilder

    member AddDescriptorImage : FalkanImage -> FalkanShaderDrawDescriptorBuilder

    member Next : FalkanShaderDrawVertexBuilder

[<RequireQualifiedAccess>]
module FalkanShaderDrawBuilder =

    val Create : unit -> FalkanShaderDrawDescriptorBuilder

[<Sealed>]
type FalkanShader =

    member CreateDrawBuilder : unit -> FalkanShaderDrawDescriptorBuilder

    member AddDraw : drawBuilder: FalkanShaderDrawVertexBuilder * vertexCount: uint32 * instanceCount: uint32 -> unit

[<Sealed>]
type internal SwapChain =
    interface IDisposable

    member DrawFrame: unit -> unit

    member SetupCommands: unit -> unit

    member WaitIdle: unit -> unit

    member CreateShader: layout: FalkanShaderDescription * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader

    member AddRenderSubpass : FalkanRenderSubpassDescription -> unit

    static member Create : FalDevice * VkSurfaceKHR * graphicsFamily: uint32 * presentFamily: uint32 * invalidate: IEvent<unit> * renderSubpasDescs: FalkanRenderSubpassDescription list -> SwapChain
