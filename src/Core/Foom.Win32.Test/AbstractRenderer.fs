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
type TextureId = private TextureId of ItemId with

    member this.ItemId = match this with TextureId itemId -> itemId

[<Struct>]
type MeshId = private MeshId of ItemId with

    member this.ItemId = match this with MeshId itemId -> itemId

[<Struct>]
type ShaderId = private ShaderId of ItemId with

    member this.ItemId = match this with ShaderId itemId -> itemId

[<Struct>]
type ModelViewProjection =
    {
        Model: Matrix4x4
        View: Matrix4x4
        Projection: Matrix4x4
    }
    
    member this.InvertedView =
        let mutable invertedView = Matrix4x4.Identity
        Matrix4x4.Invert(this.View, &invertedView) |> ignore
        { Model = this.Model; View = invertedView; Projection = this.Projection }

[<AbstractClass>]
type AbstractRenderer<'Shader, 'Mesh, 'Texture>() =
    
    let shaders = ItemManager<'Shader> 1
    let meshes = ItemManager<'Mesh> 1
    let textures = ItemManager<'Texture> 1

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

    abstract CreateSpirvShaderCore: vertex: ReadOnlySpan<byte> * fragment: ReadOnlySpan<byte> -> 'Shader        

    member this.CreateSpirvShader(vertex: Expr<unit>, fragment: Expr<unit>) =
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

        let shader = this.CreateSpirvShaderCore(ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)
        ShaderId(shaders.Add shader)

    abstract CreateMeshCore: shader: 'Shader * vertices: ReadOnlySpan<Vector3> * uv: ReadOnlySpan<Vector2> * texture: 'Texture -> 'Mesh

    member this.CreateMesh(shaderId: ShaderId, vertices: ReadOnlySpan<Vector3>, uv: ReadOnlySpan<Vector2>, textureId: TextureId) =
        let shader = getShader shaderId.ItemId
        let texture = getTexture textureId.ItemId
        let mesh = this.CreateMeshCore(shader, vertices, uv, texture)
        MeshId(meshes.Add mesh)

    abstract CreateTextureCore: bitmap: Bitmap -> 'Texture

    member this.CreateTexture(bitmap: Bitmap) =
        let texture = this.CreateTextureCore bitmap
        TextureId(textures.Add texture)

    abstract Dispose: unit -> unit

    interface IDisposable with

        member this.Dispose() = this.Dispose()


