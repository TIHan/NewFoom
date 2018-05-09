module Tests

open System
open Xunit
open Foom.Renderer.GL.ShaderQuotation

[<Struct>]
type MeshVertexOutput =
    {
        gl_Position: vec4
        uv: vec2
        color: vec4
    }

let meshVertex =
    <@
    fun (projection: mat4 uniform)
        (view: mat4 uniform)
        (position: vec3 input)
        (uv: vec2 input)
        (color: vec4 input) ->

            let snapToPixel = projection.value * view.value * vec4(position.value, 1.f)
            let snapToPixel = snapToPixel

            {
                gl_Position = snapToPixel
                uv = uv.value
                color = color.value
            }
    @>

open GlslAst

[<Fact>]
let ``My test`` () =

    let result = Transpiler.translateToModule meshVertex GlslModule.empty
    printfn "%A" result
