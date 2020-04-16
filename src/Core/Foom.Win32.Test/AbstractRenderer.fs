[<AutoOpen>]
module FsGame.Renderer.AbstractRenderer

open System
open System.Numerics
open System.Drawing
open System.Collections.Generic
open System.Collections.Concurrent
open FSharp.Quotations
open FSharp.Spirv
open FSharp.Spirv.Quotations
open FsGame.Core.Collections
open FsGame.Graphics.Vulkan

[<Struct>]
type UniformBufferId<'T> = private UniformBufferId of ItemId with

    member this.ItemId = match this with UniformBufferId itemId -> itemId

[<Struct>]
type VertexBufferId<'T> = private VertexBufferId of ItemId with

    member this.ItemId = match this with VertexBufferId itemId -> itemId

[<Struct>]
type TextureId = private TextureId of ItemId with

    member this.ItemId = match this with TextureId itemId -> itemId

[<Struct>]
type ShaderId = private ShaderId of ItemId with

    member this.ItemId = match this with ShaderId itemId -> itemId

[<Struct>]
type DrawCallId = private DrawCallId of ItemId with

    member this.ItemId = match this with DrawCallId itemId -> itemId
    
    
[<AbstractClass>] 
type AbstractRenderer<'UniformBuffer, 'VertexBuffer, 'Texture, 'Shader, 'DrawCall>() =
    
    let uniformBuffers = ItemManager<'UniformBuffer> 1
    let vertexBuffers = ItemManager<'VertexBuffer> 1
    let textures = ItemManager<'Texture> 1
    let shaders = ItemManager<'Shader> 1
    let drawCalls = ItemManager<'DrawCall> 1

    let getItem (manager: ItemManager<_>) id =
        match manager.TryGet(id) with
        | true, camera -> camera
        | _ -> invalidArg "id" "Item does not exist."

    abstract CreateUniformBufferCore<'T when 'T : unmanaged> : value: 'T -> 'UniformBuffer

    abstract SetUniformBufferCore<'T when 'T : unmanaged> : uniformBuffer: 'UniformBuffer * value: 'T -> unit

    member this.CreateUniformBuffer<'T when 'T : unmanaged>(value: 'T) : UniformBufferId<'T> =
        let internalUniformBuffer = this.CreateUniformBufferCore value
        UniformBufferId(uniformBuffers.Add internalUniformBuffer)

    member this.SetUniformBuffer<'T when 'T : unmanaged>(uniformBufferId: UniformBufferId<'T>, value: 'T) =
        this.SetUniformBufferCore(getItem uniformBuffers uniformBufferId.ItemId, value)


    abstract CreateVertexBufferCore<'T when 'T : unmanaged> : value: ReadOnlySpan<'T> -> 'VertexBuffer

    abstract SetVertexBufferCore<'T when 'T : unmanaged> : vertexBuffer: 'VertexBuffer * value: ReadOnlySpan<'T> -> unit

    member this.CreateVertexBuffer<'T when 'T : unmanaged>(value: ReadOnlySpan<'T>) : VertexBufferId<'T> =
        let internalVertexBuffer = this.CreateVertexBufferCore value
        VertexBufferId(vertexBuffers.Add internalVertexBuffer)

    member this.SetVertexBuffer<'T when 'T : unmanaged>(vertexBufferId: VertexBufferId<'T>, value: ReadOnlySpan<'T>) =
        this.SetVertexBufferCore(getItem vertexBuffers vertexBufferId.ItemId, value)


    abstract CreateTextureCore: bitmap: Bitmap -> 'Texture

    member this.CreateTexture(bitmap: Bitmap) =
        let texture = this.CreateTextureCore bitmap
        TextureId(textures.Add texture)



    abstract CreateSpirvShaderCore : vertex: ReadOnlySpan<byte> * fragment: ReadOnlySpan<byte> * vertexType: Type * instanceTypeOpt: Type option -> 'Shader        

    member this.CreateSpirvShader(vertex: Expr<unit -> unit>, fragment: Expr<unit -> unit>, vertexType, instanceTypeOpt) : ShaderId =
        let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], [])
        let spvVertex =
            Checker.Check vertex
            |> SpirvGen.GenModule spvVertexInfo

        let spvFragmentInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], ExecutionMode.OriginUpperLeft)
        let spvFragment = 
            Checker.Check fragment
            |> SpirvGen.GenModule spvFragmentInfo

        let vertexBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvVertex)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes
        let fragmentBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvFragment)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes

        let shader = this.CreateSpirvShaderCore(ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes, vertexType, instanceTypeOpt)
        ShaderId(shaders.Add shader)


    abstract CreateDrawCallCore : uniformBuffer: 'UniformBuffer * vertexBuffer: 'VertexBuffer * instanceBuffer: 'VertexBuffer option * texture: 'Texture * shader: 'Shader * instanceCount: int -> 'DrawCall

    member this.CreateDrawCall<'Uniform, 'Vertex, 'Instance>(uniformBufferId: UniformBufferId<'Uniform>, vertexBufferId: VertexBufferId<'Vertex>, instanceBufferIdOpt: VertexBufferId<'Instance> option, textureId: TextureId, shaderId: ShaderId, instanceCount: int) =
        let uniformBuffer = getItem uniformBuffers uniformBufferId.ItemId
        let vertexBuffer = getItem vertexBuffers vertexBufferId.ItemId
        let instanceBufferOpt = instanceBufferIdOpt |> Option.map (fun instanceBufferId -> getItem vertexBuffers instanceBufferId.ItemId)
        let texture = getItem textures textureId.ItemId
        let shader = getItem shaders shaderId.ItemId
        let internalDrawCall = this.CreateDrawCallCore(uniformBuffer, vertexBuffer, instanceBufferOpt, texture, shader, instanceCount)
        DrawCallId(drawCalls.Add internalDrawCall)

    abstract Refresh: unit -> unit

    abstract Draw: unit -> unit

    abstract WaitIdle: unit -> unit

    abstract Dispose: unit -> unit

    interface IDisposable with

        member this.Dispose() = this.Dispose()


