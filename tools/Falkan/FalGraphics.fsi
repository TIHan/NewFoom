[<AutoOpen>]
module Falkan.Graphics

open System
open FSharp.Vulkan.Interop

[<Struct;NoComparison>]
type Buffer =
    private {
        buffer: VkBuffer
        memory: VkDeviceMemory
        isShared: bool
    }

type PipelineIndex = int

[<Struct;NoComparison>]
type FalBuffer<'T when 'T : unmanaged> = private { buffer: Buffer } with

    member Buffer: VkBuffer

[<Sealed>]
type FalGraphics =
    interface IDisposable

    member AddShader: vertexBindings: VkVertexInputBindingDescription [] * vertexAttributes: VkVertexInputAttributeDescription [] * ReadOnlySpan<byte> * fragmentBytes: ReadOnlySpan<byte> -> PipelineIndex

    member RecordDraw: pipelineIndex: PipelineIndex * buffers: VkBuffer [] * vertexCount: int * instanceCount: int -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    member CreateVertexBuffer<'T when 'T : unmanaged> : count: int -> FalBuffer<'T>

    member FillBuffer<'T when 'T : unmanaged> : FalBuffer<'T> * ReadOnlySpan<'T> -> unit

    member DestroyBuffer : FalBuffer<_> -> unit

    static member Create : FalDevice -> FalGraphics