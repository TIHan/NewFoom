module Foom.Renderer.GL.ShaderQuotation.Transpiler

open System
open System.Collections.Generic
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Quotations.DerivedPatterns
open GlslAst

type GlslScope =
    {
        VarMap: Map<string, GlslVar>
    }

    static member Create() =
        {
            VarMap = Map.empty
        }

    member this.AddVar(GlslVar(name, _, _) as var) =
        { this with
            VarMap = this.VarMap.Add(name, var)
        }

type GlslEnv =
    {
        Scopes: GlslScope list
    }

    static member Create() =
        {
            Scopes = [ GlslScope.Create() ]
        }

    member this.TryFindVar(name) =
        (None, this.Scopes)
        ||> List.fold (fun state scope ->
            match state with
            | None -> scope.VarMap.TryFind(name)
            | _ -> state
        )

    member this.AddVar(name, var) =
        match this.Scopes with
        | [] -> failwith "No scope."
        | scope :: xs ->
            { this with 
                Scopes = scope.AddVar(var) :: xs
            }

    member this.PushScope() =
        { this with
            Scopes = GlslScope.Create() :: this.Scopes
        }

    member this.PopScope() =
        match this.Scopes with
        | _ :: xs ->
            { this with
                Scopes = xs
            }
        | _ -> failwith "no scope"

let rec translateType (typ: Type) =
    match typ with
    | x when x = typeof<mat4> -> GlslType.Matrix4x4
    | x when x = typeof<vec2> -> GlslType.Vector2 GlslVectorType.Float
    | x when x = typeof<vec3> -> GlslType.Vector3 GlslVectorType.Float
    | x when x = typeof<vec4> -> GlslType.Vector4 GlslVectorType.Float
    | x when x.GetGenericTypeDefinition() = typedefof<uniform<_>> -> translateType x.GenericTypeArguments.[0]
    | _ -> failwithf "Can't translated type: %A" typ

let rec translateExpr (expr: Expr) (hashNames: Set<string>) (ast: byref<GlslModule>) : GlslExpr =
    match expr with
    | Call(exprOpt, methodInfo, exprList) ->
        GlslExpr.NoOp
    | Let(var, expr1, expr2) ->
        let (name, hashNames) =
            if hashNames.Contains(var.Name) then
                let name = var.Name + "@"
                (name, hashNames.Add(name))
            else
                (var.Name, hashNames.Add(var.Name))

        let glslVar = mkMutableVar name (translateType var.Type)
        GlslExpr.DeclareVar(glslVar, translateExpr expr1 hashNames &ast, translateExpr expr2 hashNames &ast)
    | x -> failwithf "No supported. %A" x

let rec translateToModule (expr: Expr) (ast: GlslModule) : GlslModule =
    match expr with
    | Lambda(param, body) ->
        let ast =
            match param.Type with
            | x when x.GetGenericTypeDefinition() = typedefof<uniform<_>> ->
                { ast with
                    uniforms = mkVar param.Name (translateType x) :: ast.uniforms
                }
            | x ->
                { ast with
                    ins = mkVar param.Name (translateType x) :: ast.ins
                }
        translateToModule body ast
        // Lambda expression.
    //| Application(expr1, expr2) ->
        //// Function application.
        //let env = translate expr1 env
        //translate expr2 env
    | _ -> 

        // TODO: Should be able to resolve outs.

        let hashNames = 
            ast.ins @ ast.uniforms
            |> List.map (fun (GlslVar(name, _, _)) -> name)
            |> Set.ofList

        let mutable ast = ast
        let mainFunc = GlslFunction("main", [], GlslType.Void, translateExpr expr hashNames &ast)
        { ast with
            funcs = mainFunc :: ast.funcs
        }
    //| SpecificCall <@@ (+) @@> (_, _, exprList) ->
    //    // Matches a call to (+). Must appear before Call pattern.
    //    print exprList.Head
    //    printf " + "
    //    print exprList.Tail.Head
    //| Call(exprOpt, methodInfo, exprList) ->
    //    // Method or module function call.
    //    match exprOpt with
    //    | Some expr -> print expr
    //    | None -> printf "%s" methodInfo.DeclaringType.Name
    //    printf ".%s(" methodInfo.Name
    //    if (exprList.IsEmpty) then printf ")" else
    //    print exprList.Head
    //    for expr in exprList.Tail do
    //        printf ","
    //        print expr
    //    printf ")"
    //| Int32(n) ->
    //    printf "%d" n
    //| Lambda(param, body) ->
    //    // Lambda expression.
    //    printf "fun (%s:%s) -> " param.Name (param.Type.ToString())
    //    print body
    //| Let(var, expr1, expr2) ->
    //    // Let binding.
    //    if (var.IsMutable) then
    //        printf "let mutable %s = " var.Name
    //    else
    //        printf "let %s = " var.Name
    //    print expr1
    //    printf " in "
    //    print expr2
    //| PropertyGet(_, propOrValInfo, _) ->
    //    printf "%s" propOrValInfo.Name
    //| String(str) ->
    //    printf "%s" str
    //| Value(value, typ) ->
    //    printf "%s" (value.ToString())
    //| Var(var) ->
    //    printf "%s" var.Name
    //| _ -> printf "%s" (expr.ToString())
