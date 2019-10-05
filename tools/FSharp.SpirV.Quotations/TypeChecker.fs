[<RequireQualifiedAccess>]
module internal FSharp.Spirv.Quotations.TypeChecker

open System
open System.Numerics
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Spirv
open FSharp.Spirv.Specification
open Tast

[<NoEquality;NoComparison>]
type env =
    {
        decls: SpirvDecl list
        entryPoint: uint32
        isReturnable: bool
    }

let rec mkSpirvType (ty: Type) =
    match ty with
    | _ when ty = typeof<int> -> 
        SpirvTypeInt
    | _ when ty = typeof<single> ->
        SpirvTypeSingle
    | _ when ty = typeof<Vector2> ->
        SpirvTypeVector2
    | _ when ty = typeof<Vector3> ->
        SpirvTypeVector3
    | _ when ty = typeof<Vector4> ->
        SpirvTypeVector4
    | _ -> 
        failwithf "Unable to make SpirvType from Type: %A" ty

let outputDecorationsByPropertyName (name: string) =
    match name with
    | "gl_Position" -> [(Decoration.BuiltIn, [uint32 BuiltIn.Position])]
    | "gl_VertexIndex" -> [(Decoration.BuiltIn, [uint32 BuiltIn.VertexIndex])]
    | _ -> []

let mkSpirvVarOfVar storageClass (var: Var) =
    let spvTy = mkSpirvType var.Type
    mkSpirvVar (var.Name, spvTy, [], storageClass, var.IsMutable)

let mkSpirvConst expr =
    match expr with
    | Int32 n ->
        SpirvInt n

    | Single n ->
        SpirvSingle n

    | _ ->
        failwithf "Invalid expression for constant: %A" expr

let addSpirvDeclVar env spvVar =
    { env with decls = env.decls @ [SpirvDeclVar spvVar] }

let addInputSpirvDeclVar env var =
    let spvVar = mkSpirvVarOfVar StorageClass.Input var
    addSpirvDeclVar env spvVar

let addSpirvDeclConst env var rhs =
    let spvVar = mkSpirvVarOfVar StorageClass.Private var
    let spvConst = mkSpirvConst rhs
    { env with decls = env.decls @ [SpirvDeclConst (spvVar, spvConst)] }

let TcValue expr =
    mkSpirvConst expr
    |> SpirvConst

let rec TcExpr env expr =
    match expr with

    | Value _ ->
        env, TcValue expr

    | NewRecord(ty, args) when env.isReturnable ->
        let env, exprOpt =
            ((env, None), args, ty.GetProperties() |> List.ofArray)
            |||> List.fold2 (fun (env, exprOpt) arg prop ->
                let spvTy = mkSpirvType prop.PropertyType
                let decorations = outputDecorationsByPropertyName prop.Name
                let var = mkSpirvVar (prop.Name, spvTy, decorations, StorageClass.Output, false)
                let env = addSpirvDeclVar env var

                let env, spvExpr = TcExpr { env with isReturnable = false } arg
                match exprOpt with
                | None -> 
                    env, Some (SpirvVarSet (var, spvExpr))
                | Some expr ->
                    env, Some (SpirvSequential (expr, SpirvVarSet (var, spvExpr))) 
            )
        match exprOpt with
        | Some expr -> env, expr
        | _ -> env, SpirvNop

    | Var var ->
        env, SpirvVar (mkSpirvVarOfVar StorageClass.Private var)

    | Sequential(expr1, expr2) ->
        let env, spvExpr1 = TcExpr env expr1
        let env, spvExpr2 = TcExpr env expr2 
        env, SpirvSequential (spvExpr1, spvExpr2)

    | Call (None, methInfo, args) ->
        let env = { env with isReturnable = false }

        match methInfo.DeclaringType.FullName with
        | "Microsoft.FSharp.Core.LanguagePrimitives+IntrinsicFunctions" ->

            match methInfo.Name, args with
            | "GetArray", [receiver;arg] ->
                let env, spvReceiver = TcExpr env receiver
                let env, spvArg = TcExpr env arg
                env, SpirvArrayIndexerGet (spvReceiver, spvArg)
            | _ ->
                failwithf "Method not supported: %A" methInfo

        | _ ->
            failwithf "Declaring type of method not supported: %A" methInfo.DeclaringType

    | NewObject (ctorInfo, args) ->
        let spvTy = mkSpirvType ctorInfo.DeclaringType
        match spvTy, args with
        | SpirvTypeVector2, [arg1;arg2] -> 
            let env, spvArg1 = TcExpr env arg1
            let env, spvArg2 = TcExpr env arg2
            env, SpirvNewVector2 (spvArg1, spvArg2)

        | SpirvTypeVector3, [arg1;arg2;arg3] -> 
            let env, spvArg1 = TcExpr env arg1
            let env, spvArg2 = TcExpr env arg2
            let env, spvArg3 = TcExpr env arg3
            env, SpirvNewVector3 (spvArg1, spvArg2, spvArg3)

        | SpirvTypeVector4, [arg1;arg2;arg3;arg4] ->
            let env, spvArg1 = TcExpr env arg1
            let env, spvArg2 = TcExpr env arg2
            let env, spvArg3 = TcExpr env arg3
            let env, spvArg4 = TcExpr env arg4
            env, SpirvNewVector4 (spvArg1, spvArg2, spvArg3, spvArg4)

        | _ ->
            failwithf "Invalid type for NewObject: %A" spvTy

    | Let(var, rhs, body) ->
        let env, spvRhs = TcExpr env rhs
        let env, spvBody = TcExpr env body
        env, SpirvLet (mkSpirvVarOfVar StorageClass.Private var, spvRhs, spvBody)

    | _ ->
        failwithf "Expression not supported: %A" expr

and TcExprs (env: env) exprs =
    ((env, []), exprs)
    ||> List.fold (fun (env, spvExprs) expr ->
        let env, spvExpr = TcExpr env expr
        (env, spvExprs @ [spvExpr])
    )

let rec TcTopLevelExpr env expr =
    match expr with
    | Let(var, rhs, body) ->
        let env = addSpirvDeclConst env var rhs
        TcTopLevelExpr env body

    | Lambda(var, ((Lambda _) as body)) ->
        let env = addInputSpirvDeclVar env var
        TcTopLevelExpr env body

    | Lambda(var, body) ->
        let env = addInputSpirvDeclVar env var
        TcExpr env body
                
    | _ ->
        failwithf "Top-level expression not supported: %A" expr
