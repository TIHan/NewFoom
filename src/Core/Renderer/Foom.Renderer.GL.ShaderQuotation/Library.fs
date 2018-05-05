module Foom.Renderer.GL.ShaderQuotation.Implementation

open System
open System.Numerics
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns

let println expr =
    let rec print expr =
        match expr with
        | Application(expr1, expr2) ->
            // Function application.
            print expr1
            printf " "
            print expr2
        | SpecificCall <@@ (+) @@> (_, _, exprList) ->
            // Matches a call to (+). Must appear before Call pattern.
            print exprList.Head
            printf " + "
            print exprList.Tail.Head
        | Call(exprOpt, methodInfo, exprList) ->
            // Method or module function call.
            match exprOpt with
            | Some expr -> print expr
            | None -> printf "%s" methodInfo.DeclaringType.Name
            printf ".%s(" methodInfo.Name
            if (exprList.IsEmpty) then printf ")" else
            print exprList.Head
            for expr in exprList.Tail do
                printf ","
                print expr
            printf ")"
        | Int32(n) ->
            printf "%d" n
        | Lambda(param, body) ->
            // Lambda expression.
            printf "fun (%s:%s) -> " param.Name (param.Type.ToString())
            print body
        | Let(var, expr1, expr2) ->
            // Let binding.
            if (var.IsMutable) then
                printf "let mutable %s = " var.Name
            else
                printf "let %s = " var.Name
            print expr1
            printf " in "
            print expr2
        | PropertyGet(_, propOrValInfo, _) ->
            printf "%s" propOrValInfo.Name
        | String(str) ->
            printf "%s" str
        | Value(value, typ) ->
            printf "%s" (value.ToString())
        | Var(var) ->
            printf "%s" var.Name
        | _ -> printf "%s" (expr.ToString())
    print expr
    printfn ""


let a = 2

// exprLambda has type "(int -> int)".
let exprLambda = <@ fun x -> x + 1 @>
// exprCall has type unit.
let exprCall = <@ a + 1 @>

println exprLambda
println exprCall
println <@@ let f x = x + 10 in f 10 @@>

type UniformAttribute() =
    inherit Attribute()

type GlobalAttribute() =
    inherit Attribute()

//let (*) (m: Matrix4x4, v: Vector4) = Matrix4x4.Identity

type MeshVertexOutput =
    {
        gl_Position: Vector4
        uv: Vector2
        color: Vector4
    }

let meshVertex =
    <@
    fun ([<Uniform>] projection: Matrix4x4) 
        ([<Uniform>] view: Matrix4x4) 
        (position: Vector3) 
        (uv: Vector2) 
        (color: Vector4) ->
        
        let snapToPixel = Vector4.Transform(Vector4(position, 1.f), projection * view)
        let vertex = snapToPixel

        {
            gl_Position = vertex
            uv = uv
            color = color
        }
    @>