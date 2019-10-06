﻿[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.Checker

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
        varCache: Map<Var, SpirvVar>
        decls: SpirvDecl list
    }

    static member Default =
        {
            varCache = Map.empty
            decls = []
        }

let rec mkSpirvType ty =
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
    | _ when ty = typeof<unit> ->
        SpirvTypeVoid
    | _ when ty = typeof<Matrix4x4> ->
        SpirvTypeMatrix4x4
    | _ when ty.IsArray ->
        failwith "Array can not be made here as it needs a specific length."
    | _ -> 
        failwithf "Unable to make SpirvType from Type: %A" ty

let mkSpirvArrayType elementSpvTy length =
    SpirvTypeArray (elementSpvTy, length)

let outputDecorationsByName (name: string) =
    match name with
    | "gl_Position" -> [(Decoration.BuiltIn, [uint32 BuiltIn.Position])]
    | _ -> []

let inputDecorationsByName (name: string) =
    match name with
    | "gl_VertexIndex" -> [(Decoration.BuiltIn, [uint32 BuiltIn.VertexIndex])]
    | _ -> []

let mkSpirvVarOfVarAux env storageClass var getSpvTy =
    match env.varCache.TryFind var with
    | Some spvVar -> env, spvVar
    | _ ->
        let spvTy = getSpvTy ()
        let decorations = inputDecorationsByName var.Name
        let spvVar = mkSpirvVar (var.Name, spvTy, decorations, storageClass, var.IsMutable)
        { env with varCache = env.varCache.Add(var, spvVar) }, spvVar

let mkSpirvVarOfVar env storageClass var =
    mkSpirvVarOfVarAux env storageClass var (fun () -> mkSpirvType var.Type)

let mkSpirvVarOfVarWithArrayType env storageClass var elementSpvTy arrayLen =
    mkSpirvVarOfVarAux env storageClass var (fun () -> mkSpirvArrayType elementSpvTy arrayLen)

let rec mkSpirvConst expr =
    match expr with
    | Int32 n ->
        SpirvConstInt (n, [])

    | Single n ->
        SpirvConstSingle (n, [])

    | NewObject (ctorInfo, args) ->
        let spvTy = mkSpirvType ctorInfo.DeclaringType
        match spvTy, args with
        | SpirvTypeVector2, [Single arg1; Single arg2] -> 
            SpirvConstVector2 (arg1, arg2, [])

        | SpirvTypeVector3, [Single arg1; Single arg2; Single arg3] -> 
            SpirvConstVector3 (arg1, arg2, arg3, [])

        | SpirvTypeVector4, [Single arg1; Single arg2; Single arg3; Single arg4] ->
            SpirvConstVector4 (arg1, arg2, arg3, arg4, [])

        | _ ->
            failwithf "Invalid NewObject for constant: %A" spvTy

    | NewArray (elementTy, exprs) ->
        let elementSpvTy = mkSpirvType elementTy
        SpirvConstArray(elementSpvTy, exprs |> List.map mkSpirvConst, [])

    | _ ->
        failwithf "Invalid expression for constant: %A" expr

let addSpirvDeclVar env spvVar =
    { env with decls = env.decls @ [SpirvDeclVar spvVar] }, spvVar

let addInputSpirvDeclVar env var =
    let env, spvVar = mkSpirvVarOfVar env StorageClass.Input var
    addSpirvDeclVar env spvVar

let addSpirvDeclConst env var rhs =
    let spvConst = mkSpirvConst rhs
    let env, spvVar =
        match spvConst with
        | SpirvConstArray (elementTy, constants, _) -> 
            mkSpirvVarOfVarWithArrayType env StorageClass.Private var elementTy constants.Length
        | _ -> 
            mkSpirvVarOfVar env StorageClass.Private var

    { env with decls = env.decls @ [SpirvDeclConst (spvVar, spvConst)] }, spvVar

let CheckValue expr =
    mkSpirvConst expr
    |> SpirvConst

let rec CheckExpr env isReturnable expr =
    match expr with

    | Value _ ->
        env, CheckValue expr

    | NewRecord(ty, args) when isReturnable ->
        let env, exprOpt =
            ((env, None), args, ty.GetProperties() |> List.ofArray)
            |||> List.fold2 (fun (env, exprOpt) arg prop ->
                let spvTy = mkSpirvType prop.PropertyType
                let decorations = outputDecorationsByName prop.Name
                let var = mkSpirvVar (prop.Name, spvTy, decorations, StorageClass.Output, true)
                let env, _ = addSpirvDeclVar env var

                let env, spvExpr = CheckExpr env false arg
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
        let env, spvVar = mkSpirvVarOfVar env StorageClass.Private var
        env, SpirvVar spvVar

    | Sequential(expr1, expr2) ->
        let env, spvExpr1 = CheckExpr env false expr1
        let env, spvExpr2 = CheckExpr env true expr2 
        env, SpirvSequential (spvExpr1, spvExpr2)

    | Call (None, methInfo, args) ->
        let env, args = CheckExprs env args

        match methInfo.DeclaringType.FullName with
        | "Microsoft.FSharp.Core.LanguagePrimitives+IntrinsicFunctions" ->

            match methInfo.Name, args with
            | "GetArray", [receiver;arg] ->
                env, SpirvArrayIndexerGet (receiver, arg)
            | _ ->
                failwithf "Method not supported: %A" methInfo

        | "System.Numerics.Vector4" ->

            match methInfo.Name with
            | "Transform" ->

                let pars = methInfo.GetParameters() |> List.ofArray
                match pars, args with
                | [par1;par2], [arg1;arg2] when par1.ParameterType = typeof<Vector4> && par2.ParameterType = typeof<Matrix4x4> ->
                    env, Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2) |> SpirvIntrinsicCall
                | _ ->
                    failwithf "Method not supported: %A" methInfo

            | _ ->
                failwithf "Method not supported: %A" methInfo

        | "Microsoft.FSharp.Core.Operators" ->

            match methInfo.Name with
            | "op_Multiply" ->
                let pars = methInfo.GetParameters() |> List.ofArray
                match pars, args with
                | [par1;par2], [arg1;arg2] when par1.ParameterType = typeof<Matrix4x4> && par2.ParameterType = typeof<Matrix4x4> ->
                    env, Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) |> SpirvIntrinsicCall
                | _ ->
                    failwithf "Method not supported: %A" methInfo
            | _ ->
                failwithf "Method not supported: %A" methInfo

        | _ ->
            failwithf "Declaring type of method not supported: %A" methInfo.DeclaringType

    | NewObject (ctorInfo, args) ->
        let spvTy = mkSpirvType ctorInfo.DeclaringType
        match spvTy, args with
        | SpirvTypeVector2, args -> 
            let env, spvArgs = CheckExprs env args
            env, SpirvNewVector2 spvArgs

        | SpirvTypeVector3, args -> 
            let env, spvArgs = CheckExprs env args
            env, SpirvNewVector3 spvArgs

        | SpirvTypeVector4, args ->
            let env, spvArgs = CheckExprs env args
            env, SpirvNewVector4 spvArgs

        | _ ->
            failwithf "Invalid type for NewObject: %A" spvTy

    | Let(var, rhs, body) ->
        let env, spvVar = mkSpirvVarOfVar env StorageClass.Private var
        let env, spvRhs = CheckExpr env false rhs
        let env, spvBody = CheckExpr env true body
        env, SpirvLet (spvVar, spvRhs, spvBody)

    | _ ->
        failwithf "Expression not supported: %A" expr

and CheckExprs (env: env) exprs =
    ((env, []), exprs)
    ||> List.fold (fun (env, spvExprs) expr ->
        let env, spvExpr = CheckExpr env false expr
        (env, spvExprs @ [spvExpr])
    )

let rec CheckTopLevelExpr env expr =
    match expr with
    | Let(var, rhs, body) ->
        let env, _ = addSpirvDeclConst env var rhs
        CheckTopLevelExpr env body

    | Lambda(var, ((Lambda _) as body)) ->
        let env, spvVar = addInputSpirvDeclVar env var
        let env, checkedBody = CheckTopLevelExpr env body
        env, SpirvTopLevelLambda(spvVar, checkedBody)

    | Lambda(var, body) ->
        let env, spvVar = addInputSpirvDeclVar env var
        let env, checkedBody = CheckExpr env true body
        env, SpirvTopLevelLambdaBody(spvVar, checkedBody)
                
    | _ ->
        failwithf "Top-level expression not supported: %A" expr

let Check expr =
    let env, checkedExpr = CheckTopLevelExpr env.Default expr

    let declsExpr =
        env.decls
        |> List.map SpirvTopLevelDecl
        |> List.reduce (fun x y -> SpirvTopLevelSequential(x, y))

    SpirvTopLevelSequential(declsExpr, checkedExpr)
