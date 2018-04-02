namespace Foom.Renderer.GL

open System.Numerics
open System.Collections.Generic
open Foom.Renderer

type MeshImpl(id: uint32, position: Vector3 [], uv: Vector2 [], color: Vector4 []) =

    member val Id = id with get, set

    member val TextureBufferIndex = 0 with get, set

    member val PositionBuffer = Buffer.Create(position)

    member val UvBuffer = Buffer.Create(uv)

    member val ColorBuffer = Buffer.Create(color)

    abstract Release : IGL -> unit

    default this.Release gl =
        this.PositionBuffer.Release gl
        this.UvBuffer.Release gl
        this.ColorBuffer.Release gl

    abstract SetInput : MeshInput -> unit

    default this.SetInput(input: MeshInput) =
        input.Position.Set this.PositionBuffer
        input.Uv.Set this.UvBuffer
        input.Color.Set this.ColorBuffer

    interface IMesh

[<Sealed>]
type MeshManager() =

    let mutable nextId = 1u

    member this.Create(position: Vector3 [], uv: Vector2 [], color: Vector4 []) =
        let mesh = MeshImpl(0u, position, uv, color)
        this.AssignId(mesh)
        mesh

    member __.AssignId(mesh: MeshImpl) =
        mesh.Id <- nextId
        nextId <- nextId + 1u
        if nextId = 0u then
            failwith "Congratulations, how in the world did 4+ billion different meshes get created?"

    member __.Delete(mesh: MeshImpl, gl: IGL) =
        mesh.Id <- 0u
        mesh.Release gl