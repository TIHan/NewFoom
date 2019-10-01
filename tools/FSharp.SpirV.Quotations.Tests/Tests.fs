module Tests

open System.Numerics
open FSharp.SpirV
open FSharp.SpirV.Specification
open FSharp.SpirV.Quotations
open Xunit

[<Fact>]
let ``Compiler Vertex`` () =
//layout(location = 0) out vec3 fragColor;

//vec2 positions[3] = vec2[](
//    vec2(0.0, -0.5),
//    vec2(0.5, 0.5),
//    vec2(-0.5, 0.5)
//);

//vec3 colors[3] = vec3[](
//    vec3(1.0, 0.0, 0.0),
//    vec3(0.0, 1.0, 0.0),
//    vec3(0.0, 0.0, 1.0)
//);

//void main() {
//    gl_Position = vec4(positions[gl_VertexIndex], 0.0, 1.0);
//    fragColor = colors[gl_VertexIndex];
//}
    let vertex =
        <@
            let positions =
                [|
                    Vector2 (0.f, -0.5f)
                    Vector2 (0.5f, 0.5f)
                    Vector2 (-0.5f, 0.5f)
                |]
            let colors =
                [|
                    Vector3 (1.f, 0.f, 0.f)
                    Vector3 (0.f, 1.f, 0.f)
                    Vector3 (0.f, 0.f, 1.f)
                |]

            fun (gl_VertexIndex: int) ->
                {| 
                    gl_Position = Vector4(positions.[gl_VertexIndex], 0.f, 1.f)
                    fragColor = colors.[gl_VertexIndex]
                |}
        @>

    let spv = SPVGen.GenFragment vertex
    ()

[<Fact>]
let ``Compiler Fragment`` () =
    let fragment = 
        <@ 
        fun (fragColor: Vector3) ->
            let doot = fragColor
            {| outColor = Vector4(doot, 1.f) |}
        @>

    let spv = SPVGen.GenFragment fragment
    ()
