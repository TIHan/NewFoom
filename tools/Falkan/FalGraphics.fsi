[<AutoOpen>]
module FsGame.Graphics.Vulkan.Graphics

open System

[<Sealed>]
type FalGraphics =
    interface IDisposable

    member SetupCommands: unit -> unit

    member DrawFrame: unit -> unit

    member WaitIdle: unit -> unit

    [<RequiresExplicitTypeArguments>]
    member CreateBuffer<'T when 'T : unmanaged> : size: int * FalkanBufferFlags * FalkanBufferKind -> FalkanBuffer

    member CreateBuffer<'T when 'T : unmanaged> : size: int * FalkanBufferFlags * FalkanBufferKind * data: ReadOnlySpan<'T> -> FalkanBuffer

    member DestroyBuffer : FalkanBuffer -> unit

    member CreateImage : width: int * height: int * data: ReadOnlySpan<byte> -> FalkanImage

    member AddRenderSubpass : FalkanRenderSubpassDescription -> unit

    member CreateShader: VulkanShaderDescription * vertexSpirvSource: ReadOnlySpan<byte> * fragmentSpirvSource: ReadOnlySpan<byte> -> FalkanShader

    static member Create : VulkanDevice * invalidate: IEvent<unit> * renderSubpassDescs: FalkanRenderSubpassDescription list -> FalGraphics