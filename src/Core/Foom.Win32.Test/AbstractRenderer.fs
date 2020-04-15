[<AutoOpen>]
module FsGame.Renderer.AbstractRenderer

open System
open System.Numerics
open System.Drawing
open System.Collections.Generic
open FSharp.Quotations
open FSharp.Spirv
open FSharp.Spirv.Quotations
open FsGame.Core.Collections

[<Struct>]
type UniformBufferId = private UniformBufferId of ItemId with

    member this.ItemId = match this with UniformBufferId itemId -> itemId

[<Struct>]
type TextureId = private TextureId of ItemId with

    member this.ItemId = match this with TextureId itemId -> itemId

[<Struct>]
type MeshId = private MeshId of ItemId with

    member this.ItemId = match this with MeshId itemId -> itemId

[<Struct>]
type ShaderId = private ShaderId of ItemId with

    member this.ItemId = match this with ShaderId itemId -> itemId
    
[<AbstractClass>] 
type AbstractRenderer<'UniformBuffer, 'Shader, 'Mesh, 'Texture>() =
    
    let uniformBuffers = ItemManager<'UniformBuffer> 1
    let shaders = ItemManager<'Shader> 1
    let meshes = ItemManager<'Mesh> 1
    let textures = ItemManager<'Texture> 1

    let getItem (manager: ItemManager<_>) id =
        match manager.TryGet(id) with
        | true, camera -> camera
        | _ -> invalidArg "id" "Item does not exist."

    let getShader shaderId =
        match shaders.TryGet(shaderId) with
        | true, shader -> shader
        | _ -> invalidArg "shaderId" "Shader does not exist."

    let getMesh meshId =
        match meshes.TryGet(meshId) with
        | true, mesh -> mesh
        | _ -> invalidArg "meshId" "Mesh does not exist."

    let getTexture textureId =
        match textures.TryGet(textureId) with
        | true, texture -> texture
        | _ -> invalidArg "textureId" "Texture does not exist."

    abstract CreateUniformBufferCore<'T when 'T : unmanaged> : value: 'T -> 'UniformBuffer

    abstract SetUniformBufferCore<'T when 'T : unmanaged> : uniformBuffer: 'UniformBuffer * value: 'T -> unit

    member this.CreateUniformBuffer<'T when 'T : unmanaged>(value: 'T) =
        let internalUniformBuffer = this.CreateUniformBufferCore value
        UniformBufferId(uniformBuffers.Add internalUniformBuffer)

    member this.SetUniformBuffer<'T when 'T : unmanaged>(uniformBufferId: UniformBufferId, value: 'T) =
        this.SetUniformBufferCore(getItem uniformBuffers uniformBufferId.ItemId, value)


    //abstract CreateVertexBufferCore<'T when 'T : unmanaged> : value: 'T -> 'UniformBuffer

    //abstract SetUniformBufferCore<'T when 'T : unmanaged> : uniformBuffer: 'UniformBuffer * value: 'T -> unit

    //member this.CreateUniformBuffer<'T when 'T : unmanaged>(value: 'T) =
    //    let internalUniformBuffer = this.CreateUniformBufferCore value
    //    UniformBufferId(uniformBuffers.Add internalUniformBuffer)

    //member this.SetUniformBuffer<'T when 'T : unmanaged>(uniformBufferId: UniformBufferId, value: 'T) =
    //    this.SetUniformBufferCore(getItem uniformBuffers uniformBufferId.ItemId, value)


    abstract CreateSpirvShaderCore<'T when 'T : unmanaged> : vertex: ReadOnlySpan<byte> * fragment: ReadOnlySpan<byte> -> 'Shader        

    [<RequiresExplicitTypeArguments>]
    member this.CreateSpirvShader<'T when 'T : unmanaged>(vertex: Expr<unit -> unit>, fragment: Expr<unit -> unit>) =
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

        let shader = this.CreateSpirvShaderCore<'T>(ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)
        ShaderId(shaders.Add shader)


    abstract CreateMeshCore<'T when 'T : unmanaged> : uniformBuffer: 'UniformBuffer * shader: 'Shader * vertices: ReadOnlySpan<'T> * texture: 'Texture -> 'Mesh

    member this.CreateMesh<'T when 'T : unmanaged>(uniformBufferId: UniformBufferId, shaderId: ShaderId, vertices: ReadOnlySpan<'T>, textureId: TextureId) =
        let camera = getItem uniformBuffers uniformBufferId.ItemId
        let shader = getShader shaderId.ItemId
        let texture = getTexture textureId.ItemId
        let mesh = this.CreateMeshCore(camera, shader, vertices, texture)
        MeshId(meshes.Add mesh)


    abstract CreateTextureCore: bitmap: Bitmap -> 'Texture

    member this.CreateTexture(bitmap: Bitmap) =
        let texture = this.CreateTextureCore bitmap
        TextureId(textures.Add texture)


    abstract Refresh: unit -> unit

    abstract Draw: unit -> unit

    abstract WaitIdle: unit -> unit

    abstract Dispose: unit -> unit

    interface IDisposable with

        member this.Dispose() = this.Dispose()


