[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.Checker

open System
open System.Numerics
open System.Collections.Generic
open System.Reflection
open FSharp.NativeInterop
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Reflection
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

let getFields (ty: Type) =
    ty.GetFields(Reflection.BindingFlags.NonPublic ||| Reflection.BindingFlags.Public ||| Reflection.BindingFlags.Instance)

let tryGetIndexForBackingField (propInfo: PropertyInfo) =
    propInfo.CustomAttributes 
    |> Seq.tryPick (fun x -> 
        if x.AttributeType = typeof<CompilationMappingAttribute> && x.ConstructorArguments.Count = 2 &&
           x.ConstructorArguments.[0].ArgumentType = typeof<SourceConstructFlags> && x.ConstructorArguments.[1].ArgumentType = typeof<int> then
            if x.ConstructorArguments.[0].Value :?> SourceConstructFlags = SourceConstructFlags.Field then
                let index = x.ConstructorArguments.[1].Value :?> int
                Some index
            else
                None
        else
            None)

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
    | _ when ty = typeof<Sampler2d> ->
        SpirvTypeSampler
    | _ when ty.IsArray ->
        failwith "Array can not be made here as it needs a specific length."
    | _ when ty.IsValueType ->
        let fields =
            getFields ty
            |> Seq.map (fun field ->
                SpirvField (field.Name, mkSpirvType field.FieldType, [])
            )
            |> List.ofSeq
        SpirvTypeStruct (ty.FullName, fields)
    | _ -> 
        failwithf "Unable to make SpirvType from Type: %A" ty

let mkSpirvArrayType elementSpvTy length =
    SpirvTypeArray (elementSpvTy, length)

let mkSpirvVarOfVarAux env decorations storageClass var getSpvTy =
    match env.varCache.TryFind var with
    | Some spvVar -> env, spvVar
    | _ ->
        let spvTy = getSpvTy ()
        let spvVar = mkSpirvVar (var.Name, spvTy, decorations, storageClass, var.IsMutable)
        { env with varCache = env.varCache.Add(var, spvVar) }, spvVar

let mkSpirvVarOfVar env decorations storageClass var =
    mkSpirvVarOfVarAux env decorations storageClass var (fun () -> mkSpirvType var.Type)

let mkSpirvVarOfVarWithArrayType env decorations storageClass var elementSpvTy arrayLen =
    mkSpirvVarOfVarAux env decorations storageClass var (fun () -> mkSpirvArrayType elementSpvTy arrayLen)

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

        | SpirvTypeMatrix4x4, 
            [Single m11; Single m12; Single m13; Single m14
             Single m21; Single m22; Single m23; Single m24
             Single m31; Single m32; Single m33; Single m34
             Single m41; Single m42; Single m43; Single m44] ->
            SpirvConstMatrix4x4 (m11, m12, m13, m14,
                                 m21, m22, m23, m24,
                                 m31, m32, m33, m34,
                                 m41, m42, m43, m44, [])

        | _ ->
            failwithf "Invalid NewObject for constant: %A" spvTy

    | NewArray (elementTy, exprs) ->
        let elementSpvTy = mkSpirvType elementTy
        SpirvConstArray(elementSpvTy, exprs |> List.map mkSpirvConst, [])

    | _ ->
        failwithf "Invalid expression for constant: %A" expr

let addSpirvDeclVar env spvVar =
    { env with decls = env.decls @ [SpirvDeclVar spvVar] }, spvVar

let addSpirvDeclConst env var rhs =
    let spvConst = mkSpirvConst rhs
    let env, spvVar =
        match spvConst with
        | SpirvConstArray (elementTy, constants, _) -> 
            mkSpirvVarOfVarWithArrayType env [] StorageClass.Private var elementTy constants.Length
        | _ -> 
            mkSpirvVarOfVar env [] StorageClass.Private var

    { env with decls = env.decls @ [SpirvDeclConst (spvVar, spvConst)] }, spvVar

let CheckValue expr =
    mkSpirvConst expr
    |> SpirvConst

let rec CheckExpr env isReturnable expr =
    match expr with

    | Value (_, ty) when ty = typeof<unit> ->
        env, SpirvNop

    | Value _ ->
        env, CheckValue expr

    | Var var ->
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Function var
        env, SpirvVar spvVar

    | VarSet (var, rhs) ->
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Function var
        let env, spvRhs = CheckExpr env false rhs
        env, SpirvVarSet(spvVar, spvRhs)

    | Sequential(expr1, expr2) ->
        let env, spvExpr1 = CheckExpr env false expr1
        let env, spvExpr2 = CheckExpr env true expr2 
        env, SpirvSequential (spvExpr1, spvExpr2)

    | Call (None, _, args) ->
        let env, checkedArgs = CheckExprs env args
        CheckIntrinsicCall env checkedArgs expr

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
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Function var
        let env, spvRhs = CheckExpr env false rhs
        let env, spvBody = CheckExpr env true body
        env, SpirvLet (spvVar, spvRhs, spvBody)

    | FieldGet(Some receiver, fieldInfo) ->
        let env, spvFieldGet = CheckIntrinsicField env receiver fieldInfo
        env, SpirvIntrinsicFieldGet spvFieldGet
    | PropertyGet(Some receiver, propInfo, args) ->
        CheckPropertyGet env receiver propInfo args
    | _ ->
        failwithf "Expression not supported: %A" expr

and CheckPropertyGet env receiver propInfo args =
    let receiverTy = receiver.Type
    if FSharpType.IsRecord receiverTy then
        if not receiverTy.IsValueType then
            failwithf "Receiver '%s' is not a value type." receiverTy.FullName

        match tryGetIndexForBackingField propInfo with
        | Some index ->
            let env, spvReceiver = CheckExpr env false receiver
            env, SpirvFieldGet (spvReceiver, index)
        | _ ->
            failwithf "Property get '%s' does not use backing field." propInfo.Name
    else
        failwithf "Property get '%s' not supported." propInfo.Name

/// Check for intrinsic calls.
and CheckIntrinsicCall env checkedArgs expr =
    match expr, checkedArgs with
    | SpecificCall <@ Unchecked.defaultof<_[]>.[0] @> _, [receiver;arg] ->
        env, SpirvArrayIndexerGet (receiver, arg)

    | SpecificCall <@ Vector4.Transform(Unchecked.defaultof<Vector4>, Unchecked.defaultof<Matrix4x4>) @> _, [arg1;arg2] ->
        env, Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2) |> SpirvIntrinsicCall

    | SpecificCall <@ (*) @> (_, tyArgs, _), [arg1;arg2] ->
        match tyArgs with
        | _ when tyArgs = [typeof<Matrix4x4>;typeof<Matrix4x4>;typeof<Matrix4x4>] ->
            env, Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) |> SpirvIntrinsicCall
        | _ ->
            failwithf "Call not supported: %A" expr

    | _ ->
        failwithf "Call not supported: %A" expr

and CheckIntrinsicField env receiver fieldInfo =
    let env, spvReceiver = CheckExpr env false receiver
    match fieldInfo.Name with
    | "X" when receiver.Type = typeof<Vector2> ->
        env, Vector2_Get_X(spvReceiver, SpirvTypeSingle)
    | "Y" when receiver.Type = typeof<Vector2> ->
        env, Vector2_Get_Y(spvReceiver, SpirvTypeSingle)
    | "X" when receiver.Type = typeof<Vector3> ->
        env, Vector3_Get_X(spvReceiver, SpirvTypeSingle)
    | "Y" when receiver.Type = typeof<Vector3> ->
        env, Vector3_Get_Y(spvReceiver, SpirvTypeSingle)
    | "Z" when receiver.Type = typeof<Vector3> ->
        env, Vector3_Get_Z(spvReceiver, SpirvTypeSingle)
    | "X" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_X(spvReceiver, SpirvTypeSingle)
    | "Y" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_Y(spvReceiver, SpirvTypeSingle)
    | "Z" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_Z(spvReceiver, SpirvTypeSingle)
    | "W" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_W(spvReceiver, SpirvTypeSingle)
    | x ->
        failwithf "Invalid field: %A" x

and CheckExprs (env: env) exprs =
    ((env, []), exprs)
    ||> List.fold (fun (env, spvExprs) expr ->
        let env, spvExpr = CheckExpr env false expr
        (env, spvExprs @ [spvExpr])
    )  

let rec CheckTopLevelExpr env expr =
    match expr with
    | Let(var, SpecificCall <@ Variable<_> @> (None, [_], args), body) ->
        let env, spvExpr1 = CheckVariable env var args
        let env, spvExpr2 = CheckTopLevelExpr env body
        env, SpirvTopLevelSequential(spvExpr1, spvExpr2)
                
    | Let(var, rhs, body) ->
        let env, _ = addSpirvDeclConst env var rhs
        CheckTopLevelExpr env body

    | Lambda(var, body) ->
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Private var
        let env, checkedBody = CheckExpr env true body
        env, SpirvTopLevelLambdaBody(spvVar, checkedBody)
                
    | _ ->
        failwithf "Top-level expression not supported: %A" expr

and CheckVariable env var args =

    let rec flattenList x acc =
        match x with
        | NewUnionCase(caseInfo, [arg0;arg1]) when caseInfo.DeclaringType.GetGenericTypeDefinition() = typeof<_ list>.GetGenericTypeDefinition() && caseInfo.Name = "Cons" ->
            flattenList arg1 (acc @ [arg0])
        | _ -> acc

    let rec checkDecorationValue x =
        match x with
        | Value(x, _) -> x
        | NewUnionCase(caseInfo, args) ->
            FSharpValue.MakeUnion(caseInfo, args |> List.map checkDecorationValue |> Array.ofList)
        | _ ->
            failwithf "Invalid decoration value: %A" x

    let checkDecoration x =
        match x with
        | NewUnionCase(caseInfo, args) when caseInfo.DeclaringType = typeof<Decoration> ->
            FSharpValue.MakeUnion(caseInfo, args |> List.map checkDecorationValue |> Array.ofList) :?> Decoration
        | _ ->
            failwith "Invalid decoration."

    let checkStorageClass x =
        match x with
        | Value(v, ty) when ty = typeof<StorageClass> -> v :?> StorageClass
        | _ -> failwith "Invalid storage class."

    match args with
    | [listArg;storageClassArg] ->
        let flatArgs = flattenList listArg []
        let decorations = flatArgs |> List.map checkDecoration
        let storageClass = checkStorageClass storageClassArg
        let env, spvVar = mkSpirvVarOfVar env decorations storageClass var
        let env, spvVar = addSpirvDeclVar env spvVar
        env, SpirvDeclVar spvVar |> SpirvTopLevelDecl
    | _ ->
        failwith "Invalid new decorate."

let Check expr =
    let env, checkedExpr = CheckTopLevelExpr env.Default expr

    let declsExpr =
        env.decls
        |> List.map SpirvTopLevelDecl
        |> List.reduce (fun x y -> SpirvTopLevelSequential(x, y))

    SpirvTopLevelSequential(declsExpr, checkedExpr)
