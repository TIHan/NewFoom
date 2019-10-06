module Tests

open System.Numerics
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open Xunit

[<Fact>]
let ``Compiler Vertex`` () =
    let vertex =
        <@
            let positions =
                [|
                    Vector2 (1.f, -0.5f)
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

    let info = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], ["GLSL.std.450"])
    let expr = Checker.Check vertex
    let spv = SpirvGen.GenModule info expr
    ()

[<Fact>]
let ``Compiler Fragment`` () =
    let fragment = 
        <@ 
        fun (fragColor: Vector3) ->
            {| outColor = Vector4(fragColor, 1.f) |}
        @>

    let info = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], (ExecutionMode.OriginUpperLeft, []))
    let expr = Checker.Check fragment
    let spv = SpirvGen.GenModule info expr
    ()
