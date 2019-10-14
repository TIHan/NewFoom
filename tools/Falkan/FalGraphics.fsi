[<AutoOpen>]
module Falkan.Graphics

open System
open FSharp.Vulkan.Interop

type PipelineIndex = int

[<Sealed>]
type FalGraphics =
    interface IDisposable

    member AddShader: vertexBindings: VkVertexInputBindingDescription [] * vertexAttributes: VkVertexInputAttributeDescription [] * ReadOnlySpan<byte> * fragmentBytes: ReadOnlySpan<byte> -> PipelineIndex

    member RecordDraw: pipelineIndex: PipelineIndex * buffers: FalBuffer [] * vertexCount: int * instanceCount: int -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    member CreateVertexBuffer<'T when 'T : unmanaged> : count: int -> VertexBuffer<'T>

    member DestroyBuffer : FalBuffer -> unit

    static member Create : FalDevice -> FalGraphics