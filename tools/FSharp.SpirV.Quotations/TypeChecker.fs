[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.TypeChecker

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
type cenv =
    {
        decls: Dictionary<string, SpirvTopLevelDecl>
    }

    static member Default =
        {
            decls = Dictionary ()
        }

[<NoEquality;NoComparison>]
type env =
    {
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

let mkSpirvVar (var: Var) =
    let spvTy = mkSpirvType var.Type
    { Name = var.Name; Type = spvTy; Decorations = []; StorageClass = StorageClass.Private; IsMutable = var.IsMutable }

let rec TcExpr cenv (env: env) expr =
    match expr with

    | Int32 n ->
        SpirvInt n
        |> SpirvConst

    | Single n ->
        SpirvSingle n
        |> SpirvConst

    | NewRecord(ty, args) when env.isReturnable ->
        (None, args, ty.GetProperties() |> List.ofArray)
        |||> List.fold2 (fun exprOpt arg prop ->
            let spvTy = mkSpirvType prop.PropertyType
            let decorations = outputDecorationsByPropertyName prop.Name
            let var = { Name = Guid.NewGuid().ToString(); Type = spvTy; Decorations = decorations; StorageClass = StorageClass.Output; IsMutable = true }
            cenv.decls.Add(prop.PropertyType.FullName, SpirvTopLevelDeclVar var)

            match exprOpt with
            | None -> Some (SpirvVarSet (var, TcExpr cenv { env with isReturnable = false } arg))
            | Some expr -> Some (SpirvSequential (expr, SpirvVarSet (var, TcExpr cenv { env with isReturnable = false } arg))) 
        )
        |> Option.defaultValue SpirvNop

    | Var var ->
        SpirvVar (mkSpirvVar var)

    | Sequential(expr1, expr2) ->
        SpirvSequential (TcExpr cenv env expr1, TcExpr cenv env expr2)

    | Call (None, methInfo, args) ->
        let env = { env with isReturnable = false }

        match methInfo.DeclaringType.FullName with
        | "Microsoft.FSharp.Core.LanguagePrimitives+IntrinsicFunctions" ->

            match methInfo.Name, args with
            | "GetArray", [receiver;arg] ->
                SpirvArrayIndexerGet (TcExpr cenv env receiver, TcExpr cenv env arg)
            | _ ->
                failwithf "Method not supported: %A" methInfo

        | _ ->
            failwithf "Declaring type of method not supported: %A" methInfo.DeclaringType

    | NewObject (ctorInfo, args) ->
        let spvTy = mkSpirvType ctorInfo.DeclaringType
        match spvTy, args with
        | SpirvTypeVector2, [arg1;arg2] -> 
            SpirvNewVector2 (TcExpr cenv env arg1, TcExpr cenv env arg2)
        | SpirvTypeVector3, [arg1;arg2;arg3] -> 
            SpirvNewVector3 (TcExpr cenv env arg1, TcExpr cenv env arg2, TcExpr cenv env arg3)
        | SpirvTypeVector4, [arg1;arg2;arg3;arg4] ->
            SpirvNewVector4 (TcExpr cenv env arg1, TcExpr cenv env arg2, TcExpr cenv env arg3, TcExpr cenv env arg4)
        | _ ->
            failwithf "Invalid type for NewObject: %A" spvTy

    //| Lambda(var, expr) when not env.inMain ->
    //    SpirvLa
    //    match expr with
    //    | Lambda(var, expr) ->
    //        emitGlobalInputVariable cenv var |> ignore
    //        GenExpr cenv env expr
    //    | _ ->
    //        failwith "Should not happen."

    //| Lambda(var, expr) when not env.inMain ->
    //    emitGlobalInputVariable cenv var |> ignore
    //    GenMainLambda cenv { env with inMain = true } expr

    //// Only supports constants
    //| NewArray(ty, args) when not env.inMain ->
    //    GenNewArray cenv env ty args

    | Let(var, body, expr) ->
        SpirvLet (mkSpirvVar var, TcExpr cenv env body, TcExpr cenv env expr)

    | _ ->
        failwithf "Expression not supported: %A" expr
