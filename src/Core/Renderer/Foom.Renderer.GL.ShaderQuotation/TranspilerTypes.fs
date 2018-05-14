namespace Foom.Renderer.GL.ShaderQuotation

open System

[<Struct>]
type vec2<'a> =
    val mutable x : 'a
    val mutable y : 'a

    new (x, y) = { x = x; y = y }

[<Struct>]
type vec3<'a> =
    val mutable x : 'a
    val mutable y : 'a
    val mutable z : 'a

    new (x, y, z) = { x = x; y = y; z = z }

[<Struct>]
type vec4<'a> =
    val mutable x : 'a
    val mutable y : 'a
    val mutable z : 'a
    val mutable w : 'a

    new (x, y, z, w) = { x = x; y = y; z = z; w = w }

    new (xyz: vec3<'a>, w) = { x = xyz.x; y = xyz.y; z = xyz.z; w = w }

[<Struct>]
type mat4x4<'a> =
    val mutable v0 : vec4<'a>
    val mutable v1 : vec4<'a>
    val mutable v2 : vec4<'a>
    val mutable v3 : vec4<'a>

    new (v0, v1, v2, v3) = { v0 = v0; v1 = v1; v2 = v2; v3 = v3 }

    static member (*) (_m0: mat4x4<'a>, _m1: mat4x4<'a>) : mat4x4<'a> = raise (NotImplementedException())

    static member (*) (_m0: mat4x4<'a>, _v0: vec4<'a>) : vec4<'a> = raise (NotImplementedException())

type vec2 = vec2<float32>
type vec3 = vec3<float32>
type vec4 = vec4<float32>
type mat4x4 = mat4x4<float32>
type mat4 = mat4x4

type uniform<'a> = 
    val value : 'a

type input<'a> =
    val value : 'a

type output<'a> =
    val mutable value : 'a