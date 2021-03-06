﻿module FSharp.Spirv.Quotations.Checker

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
open TypedTree
open Intrinsics
open ErrorLogger

[<NoEquality;NoComparison>]
type env =
    {
        varCache: Map<Var, SpirvVar>
    }

    static member Default =
        {
            varCache = Map.empty
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
    | _ when ty = typeof<bool> ->
        SpirvTypeBool sizeof<bool>
    | _ when ty = typeof<byte> ->
        SpirvTypeUInt8
    | _ when ty = typeof<int> -> 
        SpirvTypeInt32
    | _ when ty = typeof<float32> ->
        SpirvTypeFloat32
    | _ when ty = typeof<Vector2> ->
        SpirvTypeVector2
    | _ when ty = typeof<Vector3> ->
        SpirvTypeVector3
    | _ when ty = typeof<Vector4> ->
        SpirvTypeVector4
    | _ when ty = typeof<unit> || ty = typeof<System.Void> ->
        SpirvTypeVoid
    | _ when ty = typeof<Matrix4x4> ->
        SpirvTypeMatrix4x4
    | _ when ty.IsGenericType && ty.FullName.StartsWith(typedefof<Image<_, _, _, _, _, _, _, _>>.FullName) ->
        let tyArgs = ty.GenericTypeArguments
        let imageTy = mkSpirvImageType tyArgs.[0] tyArgs.[1] tyArgs.[2] tyArgs.[3] tyArgs.[4] tyArgs.[5] tyArgs.[6] tyArgs.[7]
        SpirvTypeImage imageTy
    | _ when ty = typeof<Sampler> ->
        SpirvTypeSampler
    | _ when ty.IsGenericType && ty.FullName.StartsWith(typedefof<SampledImage<_, _, _, _, _, _, _, _>>.FullName) ->
        let tyArgs = ty.GenericTypeArguments
        let imageTy = mkSpirvImageType tyArgs.[0] tyArgs.[1] tyArgs.[2] tyArgs.[3] tyArgs.[4] tyArgs.[5] tyArgs.[6] tyArgs.[7]
        SpirvTypeSampledImage imageTy
    | _ when ty.IsArray && ty.HasElementType ->
        let elementTy = ty.GetElementType()
        SpirvTypeRuntimeArray(mkSpirvType elementTy)
    | _ when ty.IsValueType ->
        let fields =
            getFields ty
            |> Seq.map (fun field ->      
                SpirvField (field.Name, mkSpirvType field.FieldType, [])
            )
            |> List.ofSeq
        let isBlock =
            ty.GetCustomAttributesData()
            |> Seq.exists (fun x -> x.AttributeType = typeof<BlockAttribute>)
        SpirvTypeStruct (ty.FullName, fields, isBlock)
    | _ when ty.FullName.StartsWith("Microsoft.FSharp.Core.FSharpFunc") ->
        let spvArgTys = ty.GenericTypeArguments |> Array.map mkSpirvType
        SpirvTypeFunction(spvArgTys |> Array.take (spvArgTys.Length - 1) |> List.ofArray, spvArgTys |> Array.last)
        |> flattenSpirvTypeFunction
    | _ -> 
        failwithf "Unable to make SpirvType from Type: %A" ty

and flattenSpirvTypeFunction (spvTy: SpirvType) =
    match spvTy with
    | SpirvTypeFunction(spvParTys, SpirvTypeFunction(spvParTys2, spvRetTy)) ->
        SpirvTypeFunction(spvParTys @ spvParTys2, spvRetTy) |> flattenSpirvTypeFunction
    | _ -> spvTy

and mkSpirvImageType (sampledType: Type) (dim: Type) (depth: Type) (arrayed: Type) (multisampled: Type) (sampled: Type) (format: Type) (accessQualifier: Type) =
    let sampledType = mkSpirvType sampledType
    let dim =
        if dim = typeof<DimKind.One> then
            Dim.One
        elif dim = typeof<DimKind.Two> then
            Dim.Two
        elif dim = typeof<DimKind.Three> then
            Dim.Three
        elif dim = typeof<DimKind.Cube> then
            Dim.Cube
        elif dim = typeof<DimKind.Rect> then
            Dim.Rect
        elif dim = typeof<DimKind.Buffer> then
            Dim.Buffer
        elif dim = typeof<DimKind.SubpassData> then
            Dim.SubpassData
        else
            failwithf "Invalid DimKind: %s" dim.Name
    let depth =
        if depth = typeof<ImageDepthKind.NoDepth> then
            0u
        elif depth = typeof<ImageDepthKind.Depth> then
            1u
        elif depth = typeof<ImageDepthKind.Unknown> then
            2u
        else
            failwithf "Invalid ImageDepthKind: %s" depth.Name
    let arrayed =
        if arrayed = typeof<ImageArrayedKind.NonArrayed> then
            0u
        elif arrayed = typeof<ImageArrayedKind.Arrayed> then
            1u
        else
            failwithf "Invalid ImageArrayedKind: %s" arrayed.Name
    let multisampled =
        if multisampled = typeof<ImageMultisampleKind.Single> then
            0u
        elif multisampled = typeof<ImageMultisampleKind.Multi> then
            1u
        else
            failwithf "Invalid ImageMultisampleKind: %s" multisampled.Name
    let sampled =
        if sampled = typeof<ImageSampleKind.RuntimeOnly> then
            0u
        elif sampled = typeof<ImageSampleKind.Sampler> then
            1u
        elif sampled = typeof<ImageSampleKind.NoSampler> then
            2u
        else
            failwithf "Invalid ImageSampleKind: %s" sampled.Name
    let format =
        if format = typeof<ImageFormatKind.Unknown> then
            ImageFormat.Unknown
        elif format = typeof<ImageFormatKind.Rgba32f> then
            ImageFormat.Rgba32f
        elif format = typeof<ImageFormatKind.Rgba16f> then
            ImageFormat.Rgba16f
        elif format = typeof<ImageFormatKind.R32f> then
            ImageFormat.R32f
        elif format = typeof<ImageFormatKind.Rgba8> then
            ImageFormat.Rgba8
        elif format = typeof<ImageFormatKind.Rgba8Snorm> then
            ImageFormat.Rgba16Snorm
        else
            failwithf "Invalid ImageFormatKind: %s" format.Name
    let accessQualifier =
        if accessQualifier = typeof<AccessQualifierKind.None> then
            None
        else
            let accessQualifier =
                if accessQualifier = typeof<AccessQualifierKind.ReadOnly> then
                    AccessQualifier.ReadOnly
                elif accessQualifier = typeof<AccessQualifierKind.WriteOnly> then
                    AccessQualifier.WriteOnly
                elif accessQualifier = typeof<AccessQualifierKind.ReadWrite> then
                    AccessQualifier.ReadWrite
                else
                    failwithf "Invalid AccessQualifierKind: %s" accessQualifier.Name
            Some accessQualifier

    SpirvImageType(sampledType, dim, depth, arrayed, multisampled, sampled, format, accessQualifier)

let mkSpirvArrayType elementSpvTy length =
    SpirvTypeArray (elementSpvTy, length)

let implicitSpirvTypeConvert storageClass spvTy =
    match spvTy with
    //| SpirvTypeBool -> 
    //    match storageClass with
    //    | StorageClass.UniformConstant
    //    | StorageClass.Uniform
    //    | StorageClass.Input
    //    | StorageClass.Output
    //    | StorageClass.PushConstant -> SpirvTypeUInt8
    //    | _ -> spvTy
    | _ -> spvTy

let mkSpirvVarOfVarAux env decorations storageClass var getSpvTy =
    match env.varCache.TryFind var with
    | Some spvVar -> env, spvVar
    | _ ->
        let spvTy = getSpvTy ()
        let spvVar = mkSpirvVar (var.Name, implicitSpirvTypeConvert storageClass spvTy, decorations, storageClass, var.IsMutable)
        { env with varCache = env.varCache.Add(var, spvVar) }, spvVar

let mkSpirvVarOfVar env decorations storageClass var =
    mkSpirvVarOfVarAux env decorations storageClass var (fun () -> mkSpirvType var.Type)

let mkSpirvVarOfVarWithArrayType env decorations storageClass customAnnotations var elementSpvTy arrayLen =
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

    | Value(x, _) when x.GetType() = typeof<bool> && unbox x = false ->
        SpirvConstBool(false, sizeof<bool>, [])

    | Value(x, _) when x.GetType() = typeof<bool> && unbox x = true ->
        SpirvConstBool(true, sizeof<bool>, [])

    | _ ->
        failwithf "Invalid expression for constant: %A" expr

let rec CheckExpr env isReturnable expr =
    match expr with
    | Value _ ->
        CheckValue env expr

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

    | Call (receiver, methInfo, args) ->
        let checkedTyArgs = 
            methInfo.GetGenericArguments()
            |> Array.choose (fun ty ->
                // Ignore these kinds of generic type arguments because they are only used for the Image and SampledImage types.
                if ty.BaseType = typeof<DimKind> || 
                   ty.BaseType = typeof<ImageDepthKind> || 
                   ty.BaseType = typeof<ImageArrayedKind> || 
                   ty.BaseType = typeof<ImageMultisampleKind> ||
                   ty.BaseType = typeof<ImageSampleKind> ||
                   ty.BaseType = typeof<ImageFormatKind> || 
                   ty.BaseType = typeof<AccessQualifierKind> then
                    None
                else
                    mkSpirvType ty |> Some)
            |> List.ofArray
        let args =
            match receiver with
            | Some receiver -> [receiver] @ args
            | _ -> args
        let env, checkedArgs = CheckExprs env args
        CheckCall env checkedTyArgs checkedArgs (mkSpirvType expr.Type) expr

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

    | IfThenElse(condExpr, trueExpr, falseExpr) ->
        let env, spvCondExpr = CheckExpr env false condExpr
        let env, spvTrueExpr = CheckExpr env isReturnable trueExpr
        let env, spvFalseExpr = CheckExpr env isReturnable falseExpr
        env, SpirvIfThenElse(spvCondExpr, spvTrueExpr, spvFalseExpr)

    | Application(expr1, expr2) ->
        CheckApplication env expr1 [expr2]

    | _ ->
        failwithf "Expression not supported: %A" expr

and CheckApplication env expr args =
    match expr with
    | Application(expr1, expr2) ->
        CheckApplication env expr1 (expr2 :: args)
    | _ ->
        let env, spvVarExpr = CheckExpr env false expr
        let spvVar =
            match spvVarExpr with
            | SpirvVar spvVar -> spvVar
            | _ -> failwith "Invalid function application."

        let env, spvArgExprs = 
            ((env, []), args)
            ||> List.fold (fun (env, acc) argExpr ->
                let env, spvArgExpr = CheckExpr env false argExpr
                (env, acc @ [spvArgExpr]))

        env, SpirvCallFunction(spvVar, spvArgExprs)

and CheckValue env expr =
    match expr with
    | Value (_, ty) when ty = typeof<unit> ->
        env, SpirvNop |> SpirvExprOp

    | Value _ ->
        env,
        mkSpirvConst expr
        |> SpirvConst

    | _ -> failwith "Invalid value expression."

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
        let env, checkedArgs = CheckExprs env ([receiver] @ args)
        match checkedArgs with
        | _ ->
            failwithf "Property get '%s' not supported." propInfo.Name

and CheckCall env checkedTyArgs checkedArgs spvRetTy expr =
    // TODO: Add ability for custom calls.
    CheckIntrinsicCall env checkedTyArgs checkedArgs spvRetTy expr

and CheckIntrinsicCall env checkedTyArgs checkedArgs checkedRetTy expr =
    let isScalarSInt ty =
        ty = typeof<int64> ||
        ty = typeof<int32> ||
        ty = typeof<int16> ||
        ty = typeof<int8>

    let isScalarUInt ty =
        ty = typeof<uint64> ||
        ty = typeof<uint32> ||
        ty = typeof<uint16> ||
        ty = typeof<uint8>

    let isScalarInt ty =
        isScalarSInt ty || isScalarUInt ty

    let isScalarFloat ty =
        ty = typeof<float32> ||
        ty = typeof<float>

    let isVectorFloat32 ty =
        ty = typeof<Vector2> ||
        ty = typeof<Vector3> ||
        ty = typeof<Vector4>

    let isScalarOrVectorSInt ty =
        isScalarSInt ty // TODO: Add vector2/3/4 int versions if it ever comes available

    let isScalarOrVectorUInt ty =
        isScalarUInt ty // TODO: Add vector2/3/4 int versions if it ever comes available

    let isScalarOrVectorInt ty =
        isScalarInt ty // TODO: Add vector2/3/4 int versions if it ever comes available

    let isScalarOrVectorFloat ty =
        isScalarFloat ty || isVectorFloat32 ty

    let isMatrix4x4Float32 ty =
        ty = typeof<Matrix4x4>

    let errorNotSupported () =
        failwithf "Call not supported: %A" expr

    match expr, checkedTyArgs, checkedArgs with
    | SpecificCall <@ Unchecked.defaultof<_[]>.[0] @> _, _, [receiver;arg] -> 
        env, SpirvArrayIndexerGet (receiver, arg, checkedRetTy)

    | SpecificCall <@ Unchecked.defaultof<_[]>.[0] <- Unchecked.defaultof<_> @> _, _, [receiver;indexArg;valueArg] -> 
        env, SpirvArrayIndexerSet (receiver, indexArg, valueArg)

    | _ ->
        let env, spvOp =
            match expr, checkedTyArgs, checkedArgs with
            | SpecificCall <@ float32 @> _, _, [arg] 
            | SpecificCall <@ float @> _, _, [arg] ->
                if arg.Type.IsScalarSInt then
                    env, SpirvExprOp.Create(OpConvertFToS, arg, checkedRetTy)
                elif arg.Type.IsScalarUInt then
                    env, SpirvExprOp.Create(OpConvertFToU, arg, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ int64 @> _, _, [arg]
            | SpecificCall <@ int32 @> _, _, [arg] 
            | SpecificCall <@ int16 @> _, _, [arg]
            | SpecificCall <@ int8 @> _, _, [arg] ->
                if arg.Type.IsScalarFloat then
                    env, SpirvExprOp.Create(OpConvertSToF, arg, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ uint64 @> _, _, [arg]
            | SpecificCall <@ uint32 @> _, _, [arg] 
            | SpecificCall <@ uint16 @> _, _, [arg]
            | SpecificCall <@ uint8 @> _, _, [arg] ->
                if arg.Type.IsScalarFloat then
                    env, SpirvExprOp.Create(OpConvertSToF, arg, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ Vector4.Transform(Unchecked.defaultof<Vector4>, Unchecked.defaultof<Matrix4x4>) @> _, _, [arg1;arg2] ->
                env, SpirvExprOp.Create(OpMatrixTimesVector, arg2, arg1, checkedRetTy)

            | SpecificCall <@ (<>) @> (_, [tyArg], _), _, [arg1;arg2] ->
                if isScalarOrVectorInt tyArg then
                    env, SpirvExprOp.Create(OpINotEqual, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorFloat tyArg then
                    env, SpirvExprOp.Create(OpFOrdNotEqual, arg1, arg2, checkedRetTy) 
                else
                    errorNotSupported ()

            | SpecificCall <@ (=) @> (_, [tyArg], _), _, [arg1;arg2] ->
                if isScalarOrVectorInt tyArg then
                    env, SpirvExprOp.Create(OpIEqual, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorFloat tyArg then
                    env, SpirvExprOp.Create(OpFOrdEqual, arg1, arg2, checkedRetTy) 
                else
                    errorNotSupported ()

            | SpecificCall <@ (+) @> (_, [tyArg1;tyArg2;tyArg3], _), _, [arg1;arg2] ->
                if isScalarOrVectorInt tyArg1 && isScalarOrVectorInt tyArg2 && isScalarOrVectorInt tyArg3 then
                     env, SpirvExprOp.Create(OpIAdd, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorFloat tyArg1 && isScalarOrVectorFloat tyArg2 && isScalarOrVectorFloat tyArg3 then
                     env, SpirvExprOp.Create(OpFAdd, arg1, arg2, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ (-) @> (_, [tyArg1;tyArg2;tyArg3], _), _, [arg1;arg2] ->
                if isScalarOrVectorInt tyArg1 && isScalarOrVectorInt tyArg2 && isScalarOrVectorInt tyArg3 then
                     env, SpirvExprOp.Create(OpISub, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorFloat tyArg1 && isScalarOrVectorFloat tyArg2 && isScalarOrVectorFloat tyArg3 then
                     env, SpirvExprOp.Create(OpFSub, arg1, arg2, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ (*) @> (_, [tyArg1;tyArg2;tyArg3], _), _, [arg1;arg2] ->
                if isScalarOrVectorInt tyArg1 && isScalarOrVectorInt tyArg2 && isScalarOrVectorInt tyArg3 then
                    env, SpirvExprOp.Create(OpIMul, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorFloat tyArg1 && isScalarOrVectorFloat tyArg2 && isScalarOrVectorFloat tyArg3 then
                    env, SpirvExprOp.Create(OpFMul, arg1, arg2, checkedRetTy)
                elif isMatrix4x4Float32 tyArg1 && isMatrix4x4Float32 tyArg2 && isMatrix4x4Float32 tyArg3 then
                    env, SpirvExprOp.Create(OpMatrixTimesMatrix, arg2, arg1, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ (/) @> (_, [tyArg1;tyArg2;tyArg3], _), _, [arg1;arg2] ->
                if isScalarOrVectorSInt tyArg1 && isScalarOrVectorSInt tyArg2 && isScalarOrVectorSInt tyArg3 then
                     env, SpirvExprOp.Create(OpSDiv, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorUInt tyArg1 && isScalarOrVectorUInt tyArg2 && isScalarOrVectorUInt tyArg3 then
                    env, SpirvExprOp.Create(OpUDiv, arg1, arg2, checkedRetTy)
                elif isScalarOrVectorFloat tyArg1 && isScalarOrVectorFloat tyArg2 && isScalarOrVectorFloat tyArg3 then
                     env, SpirvExprOp.Create(OpFDiv, arg1, arg2, checkedRetTy)
                else
                    errorNotSupported ()

            | SpecificCall <@ (<) : float32 -> float32 -> bool @> _, _, [arg1;arg2] ->
                env, SpirvExprOp.Create(OpFOrdLessThan, arg1, arg2, checkedRetTy)

            | SpecificCall <@ imageSampleImplicitLod @> _, _, [coordinate;sampledImage] ->
                env, SpirvExprOp.Create((fun (idResType, idRes, arg1, arg2) -> OpImageSampleImplicitLod(idResType, idRes, arg1, arg2, None)), sampledImage, coordinate, checkedRetTy)

            | SpecificCall <@ kill @> _, [], [] ->
                env, SpirvExprOp.Create((fun _ -> OpKill), checkedRetTy)
        
            | SpecificCall <@ Vector4.Multiply : Vector4 * float32 -> Vector4 @> _, _, [arg1;arg2] ->
                env, SpirvExprOp.Create(OpVectorTimesScalar, arg1, arg2, checkedRetTy)

            | _ ->
                env, SpirvNop

        match spvOp with
        | SpirvNop ->
            match expr with
            | Call (_, methInfo, _) ->
                match Expr.TryGetReflectedDefinition methInfo with
                | Some expr ->
                    CheckExpr env true expr
                | _ ->
                    errorNotSupported ()
            | _ ->
                errorNotSupported ()
        | _ ->
            env, SpirvExprOp spvOp

and CheckIntrinsicField env receiver fieldInfo =
    let env, spvReceiver = CheckExpr env false receiver
    match fieldInfo.Name with
    | "X" when receiver.Type = typeof<Vector2> ->
        env, Vector2_Get_X(spvReceiver, SpirvTypeFloat32)
    | "Y" when receiver.Type = typeof<Vector2> ->
        env, Vector2_Get_Y(spvReceiver, SpirvTypeFloat32)

    | "X" when receiver.Type = typeof<Vector3> ->
        env, Vector3_Get_X(spvReceiver, SpirvTypeFloat32)
    | "Y" when receiver.Type = typeof<Vector3> ->
        env, Vector3_Get_Y(spvReceiver, SpirvTypeFloat32)
    | "Z" when receiver.Type = typeof<Vector3> ->
        env, Vector3_Get_Z(spvReceiver, SpirvTypeFloat32)

    | "X" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_X(spvReceiver, SpirvTypeFloat32)
    | "Y" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_Y(spvReceiver, SpirvTypeFloat32)
    | "Z" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_Z(spvReceiver, SpirvTypeFloat32)
    | "W" when receiver.Type = typeof<Vector4> ->
        env, Vector4_Get_W(spvReceiver, SpirvTypeFloat32)
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
        let env, spvExpr1 = CheckTopLevelVariable env var args
        let env, spvExpr2 = CheckTopLevelExpr env body
        env, SpirvTopLevelSequential(spvExpr1, spvExpr2)

    | Let(var, ((Lambda _) as rhs), body) ->
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Function var
        let env, spvRhs = CheckTopLevelExpr env rhs
        let env, spvBody = CheckTopLevelExpr env body
        env, SpirvTopLevelLet(spvVar, spvRhs, spvBody)
                
    | Let(var, rhs, body) ->
        let spvConst = mkSpirvConst rhs
        let env, spvVar =
            match spvConst with
            | SpirvConstArray (elementTy, constants, _) -> 
                mkSpirvVarOfVarWithArrayType env [] StorageClass.Private [] var elementTy constants.Length
            | _ -> 
                mkSpirvVarOfVar env [] StorageClass.Private var

        let spvDecl = SpirvTopLevelDecl(SpirvDeclConst(spvVar, spvConst))
        
        let env, spvBody = CheckTopLevelExpr env body
        env, SpirvTopLevelSequential(spvDecl, spvBody)

    | Lambda(var, ((Lambda _) as body)) ->
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Private var
        let env, spvBody = CheckTopLevelExpr env body
        env, SpirvTopLevelLambda(spvVar, spvBody)

    | Lambda(var, body) ->
        let env, spvVar = mkSpirvVarOfVar env [] StorageClass.Private var
        let env, checkedBody = CheckExpr env true body
        env, SpirvTopLevelLambdaBody(spvVar, checkedBody)
                
    | _ ->
        error(ErrorText.QuotationExpressionNotSupportedAtTopLevel expr, TextSpan.Zero)

and CheckVariableAux env var args =
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
    | [listArg;storageClassArg;customAnnotationsArg] ->
        let flatArgs = flattenList listArg []
        let decorations = flatArgs |> List.map checkDecoration
        let storageClass = checkStorageClass storageClassArg
        let customAnnotations = flattenList customAnnotationsArg []
        let env, spvVar = mkSpirvVarOfVar env decorations storageClass var
        env, spvVar, customAnnotations
    | _ ->
        failwith "Invalid input or output variable."

and CheckTopLevelVariable env var args =
    let env, spvVar, _ = CheckVariableAux env var args
    env, spvVar |> SpirvDeclVar |> SpirvTopLevelDecl

and CheckVariable var (args: Expr list) =
    let _, spvVar, customAnnoations = CheckVariableAux env.Default var args
    spvVar, customAnnoations

let Check expr =
    ErrorLogger.Instance.Reset()
    let checkedExpr, errors =
        try
            let _, checkedExpr = CheckTopLevelExpr env.Default expr
            let errors = ErrorLogger.Instance.Errors
            checkedExpr, errors
        with
        | :? NonRecoverableErrorException ->
            SpirvTopLevelError, ErrorLogger.Instance.Errors
        | ex ->
            ErrorLogger.Instance.Reset()
            raise ex
    checkedExpr, errors
