module Foom.Renderer.GL.ShaderQuotation.Implementation

open System
open System.Collections.Generic
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

module rec GlslLang =

    [<RequireQualifiedAccess>]
    type GlslVectorType =
        | Bool
        | Int
        | UInt
        | Float
        | Double

    [<RequireQualifiedAccess>]
    type GlslType =
        | Void
        | Bool
        | Int
        | UInt
        | Float
        | Double
        | Vector2 of GlslVectorType
        | Vector3 of GlslVectorType
        | Vector4 of GlslVectorType
        | Matrix4x4
        | Struct of fields: (string * GlslType) list

    [<RequireQualifiedAccess>]
    type GlslLiteral =
        | Bool of bool
        | Int of int
        | UInt of uint32
        | Float of float32
        | Double of float

    type GlslParameter = GlslParameter of name: string * GlslType
    type GlslFunction = GlslFunction of name: string * parms: GlslParameter list * ret: GlslType * body: GlslExpr
    type GlslVar = GlslVar of name: string * GlslType
    type GlslVal = GlslVal of name: string * GlslType

    [<RequireQualifiedAccess>]
    type GlslExpr =
        | Internal
        | Call of func: GlslFunction * exprList: GlslExpr list
        | Literal of GlslLiteral
        | Var of GlslVar
        | Val of GlslVal
        | DeclareVal of name: string * GlslExpr * next: GlslExpr
        | DeclareVar of name: string * GlslExpr * next: GlslExpr

    type GlslModule = GlslModule of uniforms: GlslVal list * ins: GlslVal list * outs: GlslVar list * GlslFunction list

    let testMeshVertex =

        let multiplyOp_mat4x4_mat4x4 = 
            GlslFunction("*", 
                [
                    GlslParameter("m0", GlslType.Matrix4x4)
                    GlslParameter("m1", GlslType.Matrix4x4)
                ], GlslType.Matrix4x4, GlslExpr.Internal)

        let multiplyOp_mat4x4_vec4 = 
            GlslFunction("*", 
                [
                    GlslParameter("m0", GlslType.Matrix4x4)
                    GlslParameter("v0", GlslType.Vector4 GlslVectorType.Float)
                ], GlslType.Vector4 GlslVectorType.Float, GlslExpr.Internal)

        let projection = GlslVal("projection", GlslType.Matrix4x4)
        let view = GlslVal("view", GlslType.Matrix4x4)
        let position = GlslVal("position", GlslType.Vector3 GlslVectorType.Float)
        let vec4_ctor = 
            GlslFunction("vec4.ctor", 
                [
                    GlslParameter("xyz", GlslType.Vector3 GlslVectorType.Float)
                    GlslParameter("w", GlslType.Float)
                ], GlslType.Float, GlslExpr.Internal
            )
        GlslModule(
            [
                projection
                view
            ],
            [
                position
                GlslVal("uv", GlslType.Vector2 GlslVectorType.Float)
                GlslVal("color", GlslType.Vector4 GlslVectorType.Float)
            ],
            [
                GlslVar("out_uv", GlslType.Vector2 GlslVectorType.Float)
                GlslVar("out_color", GlslType.Vector4 GlslVectorType.Float)
            ],
            [
                GlslFunction("main", [], GlslType.Void,
                    GlslExpr.DeclareVal("snapToPixel",
                        GlslExpr.Call(multiplyOp_mat4x4_vec4,
                            [
                                GlslExpr.Call(multiplyOp_mat4x4_vec4,
                                    [
                                        GlslExpr.Val projection
                                        GlslExpr.Val view
                                    ]
                                )
                                GlslExpr.Call(vec4_ctor,
                                    [
                                        GlslExpr.Val position
                                        GlslExpr.Literal(GlslLiteral.Float 1.f)
                                    ]
                                )
                            ]
                        ), 
                        GlslExpr.DeclareVal("vertex",
                            GlslExpr.Val(GlslVal("snapToPixel", GlslType.Vector4 GlslVectorType.Float)),
                            GlslExpr.Internal
                        )
                    )
                )
            ]
        )


type UniformAttribute() =
    inherit Attribute()

type GlobalAttribute() =
    inherit Attribute()

module QuotationTypes =

    type vec2<'a> =
        val mutable x : 'a
        val mutable y : 'a

        new (x, y) = { x = x; y = y }

    type vec3<'a> =
        val mutable x : 'a
        val mutable y : 'a
        val mutable z : 'a

        new (x, y, z) = { x = x; y = y; z = z }

    type vec4<'a> =
        val mutable x : 'a
        val mutable y : 'a
        val mutable z : 'a
        val mutable w : 'a

        new (x, y, z, w) = { x = x; y = y; z = z; w = w }

        new (xyz: vec3<'a>, w) = { x = xyz.x; y = xyz.y; z = xyz.z; w = w }

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

open QuotationTypes

[<Struct>]
type MeshVertexOutput =
    {
        gl_Position: vec4
        uv: vec2
        color: vec4
    }

let meshVertex =
    <@
    fun ([<Uniform>] projection: mat4)
        ([<Uniform>] view: mat4)
        (position: vec3)
        (uv: vec2)
        (color: vec4) ->
        
        let snapToPixel = projection * view * vec4(position, 1.f)
        let vertex = snapToPixel

        {
            gl_Position = vertex
            uv = uv
            color = color
        }
    @>