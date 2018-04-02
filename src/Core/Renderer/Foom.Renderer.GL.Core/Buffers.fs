namespace Foom.Renderer.GL

open System
open System.Numerics
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.InteropServices

// *****************************************
// *****************************************
// Array Buffers
// *****************************************
// *****************************************

[<Sealed>]
type Buffer<'T when 'T : struct> 
        (data: 'T [], length, 
         bufferData: IGL -> 'T [] -> int -> int -> unit,
         bufferSubData: IGL -> 'T [] -> int -> int -> int -> unit) =

    let mutable id = 0
    let mutable length = length
    let mutable queuedData = Some(data)
    let mutable queuedValues = Queue<struct(int * 'T)>()

    member this.Set(data) =
        queuedData <- Some (data)

    member this.Set(data, size) =
        length <- size
        queuedData <- Some (data)

    member __.Length
        with get () = length
        and set value = length <- value

    member __.Item
        with set index value = queuedValues.Enqueue(struct(index, value))

    member this.Bind (gl: IGL) =
        if id <> 0 then
            gl.BindBuffer id

    member this.TryBufferData (gl: IGL) =
        match queuedData with
        | Some(data) ->
            if id = 0 then
                id <- gl.CreateBuffer ()

            bufferData gl data data.Length id
            queuedData <- None
            ()
        | _ -> ()

        while queuedValues.Count > 0 do
            let struct(index, value) = queuedValues.Dequeue()
            // TODO: Not efficient
            bufferSubData gl [|value|] index 1 id

        true

    member this.Id = id

    member this.Release (gl: IGL) =
        if id <> 0 then
            gl.DeleteBuffer id
            id <- 0
            length <- 0
            queuedData <- None

[<AbstractClass;Sealed>]
type Buffer private () =

    static member Create(data, length) =
        Buffer<Vector2>(data, length, 
            (fun gl data size id -> gl.BufferData (data, (sizeof<Vector2> * size), id)),
            fun gl data offset size id -> gl.BufferSubData(data, sizeof<Vector2> * offset, sizeof<Vector2> * size, id)
        )

    static member Create(data, length) =
        Buffer<Vector3> (data, length, 
            (fun gl data size id -> gl.BufferData (data, (sizeof<Vector3> * size), id)),
            fun gl data offset size id -> gl.BufferSubData(data, sizeof<Vector3> * offset, sizeof<Vector3> * size, id)
        )

    static member Create(data, length) =
        Buffer<Vector4> (data, length, 
            (fun gl data size id -> gl.BufferData (data, (sizeof<Vector4> * size), id)),
            fun gl data offset size id -> gl.BufferSubData(data, sizeof<Vector4> * offset, sizeof<Vector4> * size, id)
        )

    static member Create(data: Vector2 []) =
        Buffer.Create(data, data.Length)

    static member Create(data: Vector3 []) =
        Buffer.Create(data, data.Length)

    static member Create(data: Vector4 []) =
        Buffer.Create(data, data.Length)

type Texture2DBufferQueueItem =
    | Empty
    | One of nativeint
    | Many of nativeint list

[<Sealed>]
type Texture2DBuffer () =

    let mutable id = 0
    let mutable width = 0
    let mutable height = 0
    let mutable queuedData = Empty
    let mutable isTransparent = false

    member this.Id = id

    member this.Width = width

    member this.Height = height

    member this.HasData = id > 0 || queuedData <> Empty

    member this.Set (one: nativeint, width', height') =
        width <- width'
        height <- height'
        queuedData <- One(one)

    member this.Set (many: nativeint list, width', height') =
        width <- width'
        height <- height'
        queuedData <- Many(many)

    member this.Bind (gl: IGL) =
        if id <> 0 then
            gl.BindTexture id 

    member this.TryBufferData (gl: IGL) =
        match queuedData with
        | One(data) ->

            id <- gl.CreateTexture (width, height, data)
            queuedData <- Empty
            true

        | Many(many) ->

            id <- gl.CreateTexture (width, height, nativeint 0)

            let mutable xOffset = 0
            many
            |> List.iter (fun data ->
                gl.SetSubTexture (xOffset, 0, width, height, data, id)
                xOffset <- xOffset + width
            )

            queuedData <- Empty
            true

        | _ -> false

    member this.Release (gl: IGL) =
        if id <> 0 then
            gl.DeleteTexture id
            id <- 0
            width <- 0
            height <- 0
            queuedData <- Empty

[<Sealed>]
type RenderTexture (width, height) =

    let mutable framebufferId = 0
    let mutable depthBufferId = 0
    let mutable stencilBufferId = 0
    let mutable textureId = 0
    let mutable width = width
    let mutable height = height

    member this.Width = width

    member this.Height = height

    member this.Bind (gl: IGL) =
        if framebufferId <> 0 then
            gl.BindFramebuffer framebufferId

    member this.Unbind (gl: IGL) =
        gl.BindFramebuffer 0

    member this.BindTexture (gl: IGL) =
        if textureId <> 0 then
            gl.BindTexture textureId

    member this.TryBufferData (gl: IGL) =
        
        if framebufferId = 0 then
            textureId <- gl.CreateFramebufferTexture (width, height, nativeint 0)
            framebufferId <- gl.CreateFramebuffer ()

            gl.BindFramebuffer framebufferId
            depthBufferId <- gl.CreateRenderbuffer (width, height)
            gl.BindFramebuffer framebufferId

            gl.BindFramebuffer framebufferId
            gl.BindTexture textureId
            gl.SetFramebufferTexture textureId
            gl.BindFramebuffer 0
            true
        else
            false

    member this.TextureId = textureId

    member this.Release (gl: IGL) =
        ()
        // TODO: