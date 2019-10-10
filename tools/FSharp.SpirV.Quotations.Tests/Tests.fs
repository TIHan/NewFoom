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
       
            let gl_VertexIndex = Intrinsics.NewDecorate<int> [Decoration.BuiltIn BuiltIn.VertexIndex] StorageClass.Input
            let mutable gl_Position  = Intrinsics.NewDecorate<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Input
            let mutable fragColor = Intrinsics.NewDecorate<Vector3> [Decoration.Location 0u] StorageClass.Output

            fun () ->
                gl_Position <- Vector4(positions.[gl_VertexIndex], 0.f, 1.f)
                fragColor <- colors.[gl_VertexIndex]
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

    let info = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], ExecutionMode.OriginUpperLeft)
    let expr = Checker.Check fragment
    let spv = SpirvGen.GenModule info expr
    ()

[<Fact>]
let ``Compiler Vertex - 2`` () =
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

            fun (gl_VertexIndex: int) (position: Vector3) (in_uv: Vector2) (in_color: Vector4) (uni_projection: Matrix4x4) (uni_view: Matrix4x4) ->
                let snapToPixel = Vector4.Transform(Vector4(position, 1.f), uni_view * uni_projection)
                let vertex = snapToPixel

                //vertex.xyz = snapToPixel.xyz / snapToPixel.w;
                //vertex.x = floor(160 * vertex.x) / 160;
                //vertex.y = floor(120 * vertex.y) / 120;
                //vertex.xyz = vertex.xyz * snapToPixel.w;

                {| 
                    gl_Position = vertex
                    uv = in_uv
                    color = in_color
                |}
        @>

    let info = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], ["GLSL.std.450"])
    let expr = Checker.Check vertex
    let spv = SpirvGen.GenModule info expr
    ()
