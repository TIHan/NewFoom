namespace Foom.Renderer.GL

open System.Collections.Generic

type Texture(id: int, textureBuffer: Texture2DBuffer) =

    member val Id = id with get, set

    member val Buffer = textureBuffer

    member this.Release gl =
        this.Buffer.Release gl

[<Sealed>]
type TextureManager() =

    let queue = Queue()
    let mutable nextId = 1

    member __.Create(textureBuffer) =
        let id =
            if queue.Count > 0 then
                queue.Dequeue()
            else
                let id = nextId
                nextId <- nextId + 1
                id

        Texture(id, textureBuffer)

    member __.Delete(texture: Texture, gl: IGL) =
        queue.Enqueue(texture.Id)
        texture.Id <- 0
        texture.Release gl
