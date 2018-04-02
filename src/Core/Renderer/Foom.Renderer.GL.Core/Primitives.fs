namespace Foom.Renderer.GL

open System.Numerics
open Foom.Renderer

type MaterialImpl(shader: Shader, texture: Texture) =
    inherit Material()

    member val Shader = shader

    member val Texture = texture

type CameraImpl(index: int, view: Matrix4x4, projection: Matrix4x4) =
    inherit Camera()

    let mutable view = view
    let mutable projection = projection

    member val Index = index

    override __.Translation
        with get() = 
            view.Translation
        and set value = 
            view.Translation <- value

    override __.Rotation 
        with get () = Quaternion.CreateFromRotationMatrix (view)

        and set value = 
            let mutable m = Matrix4x4.CreateFromQuaternion(value)
            m.Translation <- view.Translation
            view <- m

    override __.View
        with get () = view
        and set value = view <- value

    override __.Projection
        with get () = projection
        and set value = projection <- value

    override val ViewLerp = view with get, set
    