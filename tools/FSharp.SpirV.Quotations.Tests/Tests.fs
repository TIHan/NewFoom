module Tests

open System.Numerics
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open Xunit

let highLevelVertex =
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

        fun (vertexIndex: int) ->
            {|
                position = Vector4(positions.[vertexIndex], 0.f, 1.f)
                fragColor = colors.[vertexIndex]
            |}
    @>

let InVertexIndex = <@ NewDecorate<int> [Decoration.BuiltIn BuiltIn.VertexIndex] StorageClass.Input @>

let OutPosition = <@ NewDecorate<Vector4 ref> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output @>

let Out<'T> location = <@ NewDecorate<'T ref> [Decoration.Location location] StorageClass.Output @>

let inline (<*>) (u1: Quotations.Expr<'a -> 'b>) (u2: Quotations.Expr<'a>) : Quotations.Expr<'b> =
    <@ 
        (%u1) (%u2)
    @>

open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Reflection

let tryIt (x: Quotations.Expr<'a>) (f: Expr<'a -> unit>) : Expr =
    match x with
    | Application(expr, argExpr) ->
        let retTy = expr.Type
        if not (FSharpType.IsRecord retTy) then
            failwith "not a record"

        let outVars =
            FSharpType.GetRecordFields retTy
            |> Array.map (fun prop ->
                Var(prop.Name, prop.PropertyType)
            )

        let rec walk2 (outLets: Expr list ref) e =
            match e with
            | Let(var, rhs, body) ->
                Expr.Let(var, rhs, walk2 outLets body)
            | Sequential(e1, e2) ->
                Expr.Sequential(e1, walk2 outLets e2)
            | NewRecord(ty, args) ->
                failwith "doot"
                //outLets :=
                //    let vars =
                //        FSharpType.GetRecordFields retTy
                //        |> Array.map (fun prop ->
                //            Var(prop.Name, prop.PropertyType)
                //        )
                //    (vars, args)
                //    ||> Array.zip
                //    |> Array.reduce (fun (var1, arg1) (var2, arg2) ->
                //        Expr.Let(var1, arg1)
                //    )
        let rec walk e =
            match e with
            | Let(var, rhs, body) ->
                Expr.Let(var, rhs, walk body)
            | Lambda(var, body) ->
                
                Expr.Let(var, argExpr, Expr.Lambda(Var("_", typeof<unit>), body))
            | _ ->
                failwith "invalid"
                
        walk expr
    | _ ->
        failwith "Invalid expression."

let test () =
    tryIt (highLevelVertex <*> InVertexIndex) <@ fun res -> OutPosition := res.position; Out<Vector4> 0u := res.fragColor @>

[<Fact>]
let ``Compiler Vertex`` () =
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
       
            let gl_VertexIndex = NewDecorate<int> [Decoration.BuiltIn BuiltIn.VertexIndex] StorageClass.Input
            let mutable gl_Position  = NewDecorate<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
            let mutable fragColor = NewDecorate<Vector3> [Decoration.Location 0u] StorageClass.Output

            fun () ->
                gl_Position <- Vector4(positions.[gl_VertexIndex], 0.f, 1.f)
                fragColor <- colors.[gl_VertexIndex]
        @>

    let info = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], ["GLSL.std.450"])
    let expr = Checker.Check (test ()) //vertex
    let spv = SpirvGen.GenModule info expr
    ()

[<Fact>]
let ``Compiler Fragment`` () =
    let fragment = 
        <@ 
            let fragColor = NewDecorate<Vector3> [Decoration.Location 0u] StorageClass.Input
            let mutable outColor = NewDecorate<Vector4> [Decoration.Location 0u] StorageClass.Output

            fun () -> outColor <- Vector4(fragColor, 1.f)
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
