[<AutoOpen>]
module Falkan.Graphics

open System
open FSharp.Vulkan.Interop

[<Sealed>]
type FalGraphics =
    interface IDisposable

    member SetupCommands: unit -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    member CreateBuffer<'T when 'T : unmanaged> : size: int * FalkanBufferFlags * FalkanBufferKind -> FalkanBuffer

    member FillBuffer<'T when 'T : unmanaged> : FalkanBuffer * ReadOnlySpan<'T> -> unit

    member DestroyBuffer : FalkanBuffer -> unit

    member CreateImage : width: int * height: int -> FalkanImage

    member FillImage : FalkanImage * ReadOnlySpan<byte> -> unit

    member SetUniformBuffer<'T when 'T : unmanaged> : FalkanBuffer -> unit

    member SetSampler: FalkanImage -> unit

    member CreateShader: FalkanShaderInput<'T> * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader<'T>

    member CreateShader: FalkanShaderInput<'T1> * FalkanShaderInput<'T2> * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader<'T1, 'T2>

    static member Create : FalDevice * invalidate: IEvent<unit> -> FalGraphics