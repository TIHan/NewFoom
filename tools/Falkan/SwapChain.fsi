[<AutoOpen>]
module Falkan.SwapChain

open System
open FSharp.Vulkan.Interop

type PipelineIndex = int

[<Struct>]
type ShaderInputKind =
    | PerVertex
    | PerInstance

open System.Numerics

[<Struct>]
type FalkanShaderInput<'Input> = private FalkanShaderInput of ShaderInputKind * binding: uint32 * location: uint32

[<RequireQualifiedAccess>]
module FalkanShaderInput =

    val createVector2 : ShaderInputKind * binding: uint32 * location: uint32 -> FalkanShaderInput<Vector2>

    val createVector3 : ShaderInputKind * binding: uint32 * location: uint32 -> FalkanShaderInput<Vector3>

[<Sealed>]
type FalkanShader<'T> =

    member AddDraw : FalkanImage * FalkanBuffer * vertexCount : int * instanceCount: int -> unit

[<Sealed>]
type FalkanShader<'T1, 'T2> =

    member AddDraw : FalkanImage * FalkanBuffer * FalkanBuffer * vertexCount : int * instanceCount: int -> unit

[<Sealed>]
type internal SwapChain =
    interface IDisposable

    member SetUniformBuffer: VkBuffer * size: int -> unit

    member SetSampler: VkImageView * VkSampler -> unit

    member DrawFrame: unit -> unit

    member SetupCommands: unit -> unit

    member WaitIdle: unit -> unit

    member CreateShader: FalkanShaderInput<'T> * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader<'T>

    member CreateShader: FalkanShaderInput<'T1> * FalkanShaderInput<'T2> * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader<'T1, 'T2>

    static member Create : FalDevice * VkSurfaceKHR * graphicsFamily: uint32 * presentFamily: uint32 * invalidate: IEvent<unit> -> SwapChain
