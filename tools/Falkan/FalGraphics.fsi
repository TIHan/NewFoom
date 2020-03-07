[<AutoOpen>]
module Falkan.Graphics

open System
open FSharp.Vulkan.Interop

[<Sealed>]
type FalGraphics =
    interface IDisposable

    member AddShader: vertexBindings: VkVertexInputBindingDescription [] * vertexAttributes: VkVertexInputAttributeDescription [] * ReadOnlySpan<byte> * fragmentBytes: ReadOnlySpan<byte> -> PipelineIndex

    member RecordDraw: pipelineIndex: PipelineIndex * buffers: FalkanBuffer seq * vertexCount: int * instanceCount: int -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    member CreateBuffer<'T when 'T : unmanaged> : size: int * FalkanBufferFlags * FalkanBufferKind -> FalkanBuffer

    member FillBuffer<'T when 'T : unmanaged> : FalkanBuffer * ReadOnlySpan<'T> -> unit

    member DestroyBuffer : FalkanBuffer -> unit

    member CreateImage : width: int * height: int -> FalkanImage

    member FillImage : FalkanImage * ReadOnlySpan<byte> -> unit

    member SetUniformBuffer<'T when 'T : unmanaged> : FalkanBuffer -> unit

    member SetSampler: FalkanImage -> unit

    static member Create : FalDevice * invalidate: IEvent<unit> -> FalGraphics