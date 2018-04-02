namespace Foom.Renderer.GL

open System
open System.Numerics
open System.Collections.Generic

[<Sealed>]
type Uniform<'T> (name) =

    member val Name = name

    member val Location = -1 with get, set

    member val Value = Unchecked.defaultof<'T> with get, set

    member val IsDirty = false with get, set

    member this.Set value =
        this.Value <- value
        this.IsDirty <- true

// TODO: Vertex attrib needs a isDirty.
[<Sealed>]
type VertexAttribute<'T> (name, divisor) =

    member val Name : string = name

    member val Location = -1 with get, set

    member val Value = Unchecked.defaultof<'T> with get, set

    member val Divisor = divisor

    member this.Set value = this.Value <- value

[<Sealed>]
type InstanceAttribute<'T> (name) =

    member val VertexAttribute = VertexAttribute<'T> (name, 1)

    member this.Set value = this.VertexAttribute.Set value

type DrawOperation =
    | Normal
    | Instanced

type ShaderProgram =
    {
        gl: IGL
        mutable programId: int
        mutable drawOperation: DrawOperation
        mutable isUnbinded: bool
        mutable isInitialized: bool
        mutable length: int
        mutable inits: ResizeArray<unit -> unit>
        mutable binds: ResizeArray<unit -> unit>
        mutable unbinds: ResizeArray<unit -> unit>
        mutable instanceCount: int
        mutable activeTextureCount: int
    }

    static member Create (gl, programId) =
        {
            gl = gl
            programId = programId
            drawOperation = DrawOperation.Normal
            isUnbinded = true
            isInitialized = false
            length = -1
            inits = ResizeArray ()
            binds = ResizeArray ()
            unbinds = ResizeArray ()
            instanceCount = -1
            activeTextureCount = 0
        }

    static member Create(gl: IGL, vertexSource, fragmentSource) =
        ShaderProgram.Create(gl, gl.LoadProgram(vertexSource, fragmentSource))

    member this.Id = this.programId

    member this.CreateUniform<'T> (name) =
        let gl = this.gl

        if this.isInitialized then failwithf "Cannot create uniform, %s. Shader already initialized." name

        let uni = Uniform<'T> (name)
            
        let initUni =
            fun () ->
                uni.Location <- gl.GetUniformLocation (this.programId, uni.Name)

        let setValue =
            match uni :> obj with
            | :? Uniform<int> as uni ->              
                fun () -> 
                    if uni.IsDirty && uni.Location > -1 then 
                        gl.BindUniform (uni.Location, uni.Value)
                        uni.IsDirty <- false

            | :? Uniform<float32> as uni ->              
                fun () -> 
                    if uni.IsDirty && uni.Location > -1 then 
                        gl.BindUniform (uni.Location, uni.Value)
                        uni.IsDirty <- false

            | :? Uniform<Vector2> as uni ->         
                fun () -> 
                    if uni.IsDirty && uni.Location > -1 then 
                        gl.BindUniform (uni.Location, uni.Value)
                        uni.IsDirty <- false

            | :? Uniform<Vector4> as uni ->         
                fun () -> 
                    if uni.IsDirty && uni.Location > -1 then 
                        gl.BindUniform (uni.Location, uni.Value)
                        uni.IsDirty <- false

            | :? Uniform<Matrix4x4> as uni ->        
                fun () -> 
                    if uni.IsDirty && uni.Location > -1 then 
                        gl.BindUniform (uni.Location, uni.Value)
                        uni.IsDirty <- false

            | :? Uniform<Texture2DBuffer> as uni ->
                fun () ->
                    if uni.IsDirty && not (obj.ReferenceEquals (uni.Value, null)) && uni.Location > -1 then 
                        uni.Value.TryBufferData this.gl |> ignore
                        gl.BindUniform (uni.Location, 0)
                        gl.ActiveTexture (this.activeTextureCount)
                        uni.Value.Bind this.gl
                        uni.IsDirty <- false

                        this.activeTextureCount <- this.activeTextureCount + 1

            | :? Uniform<Texture2DBuffer []> as uni ->
                let values = Array.zeroCreate 128 // Add checks
                fun () ->
                    if uni.IsDirty && not (obj.ReferenceEquals (uni.Value, null)) && uni.Location > -1 && uni.Value.Length > 0 then 

                        for i = 0 to uni.Value.Length - 1 do
                            let x = uni.Value.[i]
                            x.TryBufferData this.gl |> ignore
                            values.[i] <- this.activeTextureCount + i

                        gl.BindUniform (uni.Location, uni.Value.Length, values)

                        for i = 0 to uni.Value.Length - 1 do
                            let x = uni.Value.[i]
                            gl.ActiveTexture this.activeTextureCount
                            x.Bind this.gl
                            this.activeTextureCount <- this.activeTextureCount + 1

                        uni.IsDirty <- false

            | :? Uniform<RenderTexture> as uni ->
                fun () ->
                    if uni.IsDirty && not (obj.ReferenceEquals (uni.Value, null)) && uni.Location > -1 then 
                        gl.BindUniform (uni.Location, 0)
                        gl.ActiveTexture (this.activeTextureCount)
                        uni.Value.BindTexture this.gl
                        uni.IsDirty <- false

                        this.activeTextureCount <- this.activeTextureCount + 1

            | _ -> failwith "This should not happen."

        let bind = setValue

        let unbind =
            fun () -> uni.Set Unchecked.defaultof<'T>

        this.inits.Add initUni
        this.inits.Add setValue

        this.binds.Add bind
        this.unbinds.Add unbind

        uni

    member this.CreateVertexAttribute<'T> (name) =
        if this.isInitialized then failwithf "Cannot create vertex attribute, %s. Shader already initialized." name

        let attrib = VertexAttribute<'T> (name, 0)

        this.AddVertexAttribute attrib
        attrib

    member this.CreateInstanceAttribute<'T> (name) =
        if this.isInitialized then failwithf "Cannot create instance attribute, %s. Shader already initialized." name

        let attrib = InstanceAttribute<'T> (name)

        this.drawOperation <- DrawOperation.Instanced
        this.AddVertexAttribute attrib.VertexAttribute
        attrib

    member this.AddVertexAttribute<'T> (attrib: VertexAttribute<'T>) =
        let gl = this.gl

        let initAttrib =
            fun () ->
                attrib.Location <- gl.GetAttributeLocation (this.programId, attrib.Name)
                if (attrib.Location = -1) then
                    System.Diagnostics.Debug.WriteLine("Could not find attribute, " + attrib.Name + ".")

        let bufferData =
            match attrib :> obj with
            | :? VertexAttribute<Buffer<Vector2>> as attrib -> 
                fun () -> 
                    if obj.ReferenceEquals (attrib.Value, null) |> not then
                        attrib.Value.TryBufferData this.gl |> ignore

            | :? VertexAttribute<Buffer<Vector3>> as attrib -> 
                fun () -> 
                    if obj.ReferenceEquals (attrib.Value, null) |> not then
                        attrib.Value.TryBufferData this.gl |> ignore

            | :? VertexAttribute<Buffer<Vector4>> as attrib ->
                fun () -> 
                    if obj.ReferenceEquals (attrib.Value, null) |> not then
                        attrib.Value.TryBufferData this.gl |> ignore

            | _ -> failwith "Should not happen."

        let bindBuffer =
            match attrib :> obj with
            | :? VertexAttribute<Buffer<Vector2>> as attrib -> 
                fun () -> 
                    if obj.ReferenceEquals (attrib.Value, null) |> not then
                        attrib.Value.Bind this.gl

            | :? VertexAttribute<Buffer<Vector3>> as attrib ->
                fun () -> 
                    if obj.ReferenceEquals (attrib.Value, null) |> not then
                        attrib.Value.Bind this.gl

            | :? VertexAttribute<Buffer<Vector4>> as attrib ->
                fun () -> 
                    if obj.ReferenceEquals (attrib.Value, null) |> not then
                        attrib.Value.Bind this.gl

            | _ -> failwith "Should not happen."

        let size =
            match attrib :> obj with
            | :? VertexAttribute<Buffer<Vector2>> -> 2
            | :? VertexAttribute<Buffer<Vector3>> -> 3
            | :? VertexAttribute<Buffer<Vector4>> -> 4
            | _ -> failwith "Should not happen."

        let getLength =
            match attrib :> obj with
            | :? VertexAttribute<Buffer<Vector2>> as attrib -> fun () -> attrib.Value.Length
            | :? VertexAttribute<Buffer<Vector3>> as attrib -> fun () -> attrib.Value.Length
            | :? VertexAttribute<Buffer<Vector4>> as attrib -> fun () -> attrib.Value.Length
            | _ -> failwith "Should not happen."

        let bind =
            fun () ->
                if attrib.Location > -1 then
                    if not (obj.ReferenceEquals (attrib.Value, null)) then
                        bufferData ()
                        bindBuffer ()

                        // TODO: this will change
                        gl.BindAttributePointerFloat32 (attrib.Location, size)
                        gl.EnableAttribute attrib.Location
                        gl.AttributeDivisor (attrib.Location, attrib.Divisor)

                        let length = getLength ()
                        if attrib.Divisor > 0 then
                            this.instanceCount <-
                                if this.instanceCount = -1 then
                                    length
                                elif length < this.instanceCount then
                                    length
                                else
                                    this.instanceCount
                        else
                            this.length <-
                                if this.length = -1 then
                                    length
                                elif length < this.length then
                                    length
                                else
                                    this.length
                    else
                        this.length <- 0

        let unbind =
            fun () ->
                attrib.Value <- Unchecked.defaultof<'T>

        this.inits.Add initAttrib
        this.inits.Add bufferData

        this.binds.Add bind
        this.unbinds.Add unbind

    member this.Unbind () =
        if not this.isUnbinded then
            for i = 0 to this.unbinds.Count - 1 do
                let f = this.unbinds.[i]
                f ()

            this.length <- -1
            this.instanceCount <- -1
            this.isUnbinded <- true

    member private this.Draw () =

        if this.length > 0 then
            match this.drawOperation with
            | Normal ->
                this.gl.DrawTriangles (0, this.length)
            | Instanced ->
                if this.instanceCount > 0 then
                    this.gl.DrawTrianglesInstanced (this.length, this.instanceCount)

    member this.Run () =
        let gl = this.gl

        if this.programId > 0 then
            this.isUnbinded <- false
            if not this.isInitialized then

                for i = 0 to this.inits.Count - 1 do
                    let f = this.inits.[i]
                    f ()

                this.isInitialized <- true

            for i = 0 to this.binds.Count - 1 do
                let f = this.binds.[i]
                f ()

            this.Draw ()

            this.activeTextureCount <- 0

