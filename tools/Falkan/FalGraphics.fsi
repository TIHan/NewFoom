[<AutoOpen>]
module Falkan.Graphics

open System
open FSharp.Vulkan.Interop

[<Flags>]
type BufferFlags =
    | None = 0b0uy
    /// Indicates that the buffer's memory is shared between the CPU and GPU. Great for dynamic data.
    | SharedMemory = 0b1uy

type BufferKind =
    | Unspecified
    | Vertex
    | Index
    | Uniform

type PipelineIndex = int

[<Struct;NoComparison;NoEquality>]
type FalBuffer<'T when 'T : unmanaged> =

    member Buffer: VkBuffer

    member IsShared: bool

[<Struct;NoComparison;NoEquality>]
type FalImage =

    member Image: VkImage

[<Sealed>]
type FalGraphics =
    interface IDisposable

    member AddShader: vertexBindings: VkVertexInputBindingDescription [] * vertexAttributes: VkVertexInputAttributeDescription [] * ReadOnlySpan<byte> * fragmentBytes: ReadOnlySpan<byte> -> PipelineIndex

    member RecordDraw: pipelineIndex: PipelineIndex * buffers: VkBuffer [] * vertexCount: int * instanceCount: int -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    member CreateBuffer<'T when 'T : unmanaged> : size: int * BufferFlags * BufferKind -> FalBuffer<'T>

    member FillBuffer<'T when 'T : unmanaged> : FalBuffer<'T> * ReadOnlySpan<'T> -> unit

    member DestroyBuffer : FalBuffer<_> -> unit

    member CreateImage : width: int * height: int -> FalImage

    member FillImage : FalImage * ReadOnlySpan<byte> -> unit

    member SetUniformBuffer: FalBuffer<_> -> unit

    member SetSampler: FalImage -> unit

    static member Create : FalDevice -> FalGraphics