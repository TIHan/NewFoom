module Foom.Renderer.GL.ShaderQuotation.GlslGen

open System
open GlslAst

let genType = function
    | GlslType.Void -> "void"
    | GlslType.Bool -> "bool"
    | GlslType.Int -> "int"
    | GlslType.Float -> "float"
    | GlslType.Double -> "double"
    | GlslType.Vector2 GlslVectorType.Float -> "vec2"
    | GlslType.Vector3 GlslVectorType.Float -> "vec3"
    | GlslType.Vector4 GlslVectorType.Float -> "vec4"
    | GlslType.Matrix4x4 -> "mat4"
    | x -> failwithf "Type not supported yet: %A" x

let genUniforms = function
    | [] -> String.Empty
    | uniforms ->
        uniforms
        |> List.map (function
            | GlslVar(name, typ, isMutable) ->
                if isMutable then failwithf "`uniform` attribute, %s, must not be marked mutable." name
                sprintf "uniform %s %s;\n" (genType typ) name
        )
        |> List.reduce (+)

let genIns = function
    | [] -> String.Empty
    | ins ->
        ins
        |> List.map (function
            | GlslVar(name, typ, isMutable) ->
                if isMutable then failwithf "`in` attribute, %s, must not be marked mutable." name
                sprintf "in %s %s;\n" (genType typ) name
        )
        |> List.reduce (+)

let genOuts = function
    | [] -> String.Empty
    | ins ->
        ins
        |> List.map (function
            | GlslVar(name, typ, isMutable) ->
                if not isMutable then failwithf "`out` attribute, %s, must be marked mutable." name
                sprintf "out %s %s;\n" (genType typ) name
        )
        |> List.reduce (+)

let genParameter = function
    | GlslParameter(name, typ) ->
        sprintf "%s %s" (genType typ) name

let genParameters = function
    | [] -> String.Empty
    | parms ->
        parms
        |> List.map genParameter
        |> List.reduce (+)

let genVar = function
    | GlslVar(name, _, _) -> name

let rec genExpr = function
    | GlslExpr.NoOp -> String.Empty
    | GlslExpr.Internal -> String.Empty
    | GlslExpr.Var(var) -> genVar var
 //   | GlslExpr.VarSet(var, expr) -> ()
            
    | x -> failwithf "%A not supported yet." x


let genFunction = function
    | GlslFunction(name, parms, ret, body) ->
        sprintf "%s %s(%s){\n %s\n }" (genType ret) name (genParameters parms) String.Empty
////let genFunctions = function
    //| [] -> String.Empty
    //| funcs ->
        //funcs

let gen { uniforms = uniforms; ins = ins; outs = outs; funcs = funcs } =
    """
#version 330 core

    """ +
    genUniforms uniforms +
    genIns ins +
    genOuts outs
