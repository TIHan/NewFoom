[<AutoOpen>]
module Falkan.Graphics

open System

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

    member CreateShader: FalkanShaderLayout * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader

    static member Create : FalDevice * invalidate: IEvent<unit> -> FalGraphics