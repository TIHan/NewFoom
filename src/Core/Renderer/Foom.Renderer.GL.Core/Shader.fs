namespace Foom.Renderer.GL

open System.Numerics
open System.Collections.Generic

type MeshInput(p: ShaderProgram) =

    member val Position = p.CreateVertexAttribute<Buffer<Vector3>> ("position")

    member val Uv = p.CreateVertexAttribute<Buffer<Vector2>> ("in_uv")

    member val Color = p.CreateVertexAttribute<Buffer<Vector4>> ("in_color")

    member val Texture = p.CreateUniform<Texture2DBuffer []> ("uni_texture")

    member val View = p.CreateUniform<Matrix4x4> ("uni_view")

    member val Projection = p.CreateUniform<Matrix4x4> ("uni_projection")

    member val Time = p.CreateUniform<float32> ("uTime")

    member val TextureResolution = p.CreateUniform<Vector2> ("uTextureResolution")

[<Sealed>]
type Shader(id: int, program: ShaderProgram, input: MeshInput) =

    member val Id = id with get, set

    member val Program = program

    member val Input = input

    member val ProgramId = program.Id

    static member Create(id, program, createInput: ShaderProgram -> #MeshInput) =
        Shader(id, program, createInput program)

[<Sealed>]
type ShaderManager() =

    let queue = Queue()
    let mutable nextId = 1

    member __.Create(program, createInput) =
        let id =
            if queue.Count > 0 then
                queue.Dequeue()
            else
                let id = nextId
                nextId <- nextId + 1
                id

        Shader(id, program, createInput)

    member __.Delete(shader: Shader, gl: IGL) =
        queue.Enqueue(shader.Id)
        shader.Id <- 0
        gl.DeleteProgram(shader.ProgramId)
