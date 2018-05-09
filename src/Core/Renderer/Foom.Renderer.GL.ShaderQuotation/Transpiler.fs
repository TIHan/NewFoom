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

let mkMultiplyFunction x0 x1 ret = 
    mkFunction 
        "op_multiply" 
        [ mkParameter "x0" (translateType x1); mkParameter "x1" (translateType x1) ] 
        (translateType ret) 
        GlslExpr.Internal

let vec4CtorFunction =
    mkFunction
        "ctor_vec4"
        [ mkParameter "xyz" (GlslType.Vector3 GlslVectorType.Float); mkParameter "w" GlslType.Float ]
        (GlslType.Vector4 GlslVectorType.Float)
        GlslExpr.Internal

let rec translateExpr (expr: Expr) : GlslExpr =
    match expr with
    | Sequential(expr1, expr2) ->
        GlslExpr.Sequential(translateExpr expr1, translateExpr expr2)
    | NewRecord(typ, exprList) ->
        let props = typ.GetProperties()
        if props.Length <> exprList.Length then
            failwithf "Record can only be constructed at the end of the quotation: %A" typ
    
        (props, exprList)
        ||> Seq.map2 (fun prop expr ->
            GlslExpr.VarSet(mkVar prop.Name (translateType prop.PropertyType), translateExpr expr)
        )
        |> Seq.reduce (fun glslExpr1 glslExpr2 -> GlslExpr.Sequential(glslExpr1, glslExpr2))
    | VarSet(var, expr) ->
        GlslExpr.VarSet(mkMutableVar var.Name (translateType var.Type), translateExpr expr)
    | Var(var) ->
        GlslExpr.Var(mkMutableVar var.Name (translateType var.Type))
    | Value(value, typ) ->
        if typ = typeof<unit> then
            GlslExpr.NoOp
        else

        let literal =
            match typ with
            | x when x = typeof<bool> -> mkLiteralBool (value :?> bool)
            | x when x = typeof<int> -> mkLiteralInt (value :?> int)
            | x when x = typeof<uint32> -> mkLiteralUInt (value :?> uint32)
            | x when x = typeof<float32> -> mkLiteralFloat (value :?> float32)
            | x when x = typeof<double> -> mkLiteralDouble (value :?> float)
            | _ -> failwithf "Literal not supported: %A %A" value typ

        GlslExpr.Literal(literal)
    | NewObject(ctorInfo, exprList) ->
        GlslExpr.Call(vec4CtorFunction,
            exprList
            |> List.map translateExpr
        )
    | PropertyGet(Some(Var(var)), propOrValInfo, _) ->
        let typedef = var.Type.GetGenericTypeDefinition()
        if (typedefof<uniform<_>> = typedef || typedefof<input<_>> = typedef) && propOrValInfo.Name = "value" then
            GlslExpr.Var(mkVar var.Name (translateType var.Type.GenericTypeArguments.[0]))
        else
            failwithf "PropertyGet not supported: %A" expr
    | SpecificCall <@@ (*) @@> (None, typeList, exprList) ->
        GlslExpr.Call(mkMultiplyFunction typeList.[0] typeList.[1] typeList.[2],
            exprList
            |> List.map (fun x -> translateExpr x)
        )
    | Call(exprOpt, methodInfo, exprList) ->
        GlslExpr.NoOp
    | Let(var, expr1, expr2) ->
        let name = var.Name
        let glslVar = mkMutableVar name (translateType var.Type)
        GlslExpr.DeclareVar(glslVar, translateExpr expr1, translateExpr expr2)
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

        let mainFunc = GlslFunction("main", [], GlslType.Void, translateExpr expr)
        { ast with
            funcs = mainFunc :: (ast).funcs
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
