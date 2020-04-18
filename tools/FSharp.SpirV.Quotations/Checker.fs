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
    | _ when ty = typeof<bool> ->
        SpirvTypeBool
    | _ when ty = typeof<byte> ->
        SpirvTypeUInt8
    | _ when ty = typeof<int> -> 
        SpirvTypeInt32
    | _ when ty = typeof<float32> ->
        SpirvTypeFloat32
    | _ when ty = typeof<Vector2> ->
        SpirvTypeVector2
    | _ when ty = typeof<Vector2Int> ->
        SpirvTypeVector2Int
    | _ when ty = typeof<Vector3> ->
        SpirvTypeVector3
    | _ when ty = typeof<Vector4> ->
        SpirvTypeVector4
    | _ when ty = typeof<unit> ->
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

    | Value(x, _) when x.GetType() = typeof<bool> && unbox x = false ->
        SpirvConstBool(false, [])

    | Value(x, _) when x.GetType() = typeof<bool> && unbox x = true ->
        SpirvConstBool(true, [])

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

    | Call (receiver, _, args) ->
        let args =
            match receiver with
            | Some receiver -> [receiver] @ args
            | _ -> args
        let env, checkedArgs = CheckExprs env args
        CheckIntrinsicCall env checkedArgs expr

    | NewObject (ctorInfo, args) ->
        let spvTy = mkSpirvType ctorInfo.DeclaringType
        match spvTy, args with
        | SpirvTypeVector2, args -> 
            let env, spvArgs = CheckExprs env args
            env, SpirvNewVector2 spvArgs

        | SpirvTypeVector2Int, args -> 
            let env, spvArgs = CheckExprs env args
            match spvArgs with
            | [spvArg] when spvArg.Type = SpirvTypeVector2 ->
                env, ConvertAnyFloatToAnySInt spvArg |> SpirvIntrinsicCall
            | _ ->
                env, SpirvNewVector2Int spvArgs

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
        let env, checkedArgs = CheckExprs env ([receiver] @ args)
        match checkedArgs with
        | [arg] when propInfo.DeclaringType.FullName.StartsWith(typedefof<SampledImage<_, _, _, _, _, _, _, _>>.FullName) && propInfo.Name = "Image" ->
            env, GetImage arg |> SpirvIntrinsicCall
        | _ ->
            failwithf "Property get '%s' not supported." propInfo.Name

/// Check for intrinsic calls.
and CheckIntrinsicCall env checkedArgs expr =
    let tyArgs =
        match expr with
        | Call (_, methInfo, _) -> methInfo.GetGenericArguments()
        | _ -> failwithf "Expr is not a call: %A" expr

    match expr, tyArgs, checkedArgs with
    | SpecificCall <@ int @> _, _, [arg] 
    | SpecificCall <@ SpirvInstrinsics.ConvertFloatToInt @> _, _, [arg] ->
        env, ConvertAnyFloatToAnySInt arg |> SpirvIntrinsicCall

    | SpecificCall <@ float32 @> _, _, [arg] ->
        env, ConvertSIntToFloat arg |> SpirvIntrinsicCall

    | SpecificCall <@ Unchecked.defaultof<_[]>.[0] @> _, _, [receiver;arg] ->
        env, SpirvArrayIndexerGet (receiver, arg)

    | SpecificCall <@ Vector4.Transform(Unchecked.defaultof<Vector4>, Unchecked.defaultof<Matrix4x4>) @> _, _, [arg1;arg2] ->
        env, Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2) |> SpirvIntrinsicCall

    //| SpecificCall <@ (*) : float32 -> float32 -> float32 @> _, _, [arg1;arg2] ->
    //    env, FloatMultiply(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall

    //| SpecificCall <@ (/) : float32 -> float32 -> float32 @> _, _, [arg1;arg2] ->
    //    env, FloatDivide(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall

    | SpecificCall <@ (+) @> (_, tyArgs, _), _, [arg1;arg2] ->
        match tyArgs with
        | _ when tyArgs = [typeof<float32>;typeof<float32>;typeof<float32>] ->
            env, FloatAdd(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall
        | _ ->
            failwithf "Call not supported: %A" expr

    | SpecificCall <@ (-) @> (_, tyArgs, _), _, [arg1;arg2] ->
        match tyArgs with
        | _ when tyArgs = [typeof<float32>;typeof<float32>;typeof<float32>] ->
            env, FloatSubtract(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall
        | _ ->
            failwithf "Call not supported: %A" expr

    | SpecificCall <@ (*) @> (_, tyArgs, _), _, [arg1;arg2] ->
        match tyArgs with
        | _ when tyArgs = [typeof<Matrix4x4>;typeof<Matrix4x4>;typeof<Matrix4x4>] ->
            env, Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) |> SpirvIntrinsicCall
        | _ when tyArgs = [typeof<float32>;typeof<float32>;typeof<float32>] ->
            env, FloatMultiply(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall
        | _ ->
            failwithf "Call not supported: %A" expr

    | SpecificCall <@ (/) @> (_, tyArgs, _), _, [arg1;arg2] ->
        match tyArgs with
        | _ when tyArgs = [typeof<float32>;typeof<float32>;typeof<float32>] ->
            env, FloatDivide(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall
        | _ ->
            failwithf "Call not supported: %A" expr

    | Call(_, methInfo, _), _, [arg1;arg2] when methInfo.DeclaringType.FullName.StartsWith(typedefof<Image<_, _, _, _, _, _, _, _>>.FullName) && methInfo.Name = "Fetch" ->
        env, ImageFetch (arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall

    | Call(_, methInfo, _), _, [arg1;arg2;arg3] when methInfo.DeclaringType.FullName.StartsWith(typedefof<SampledImage<_, _, _, _, _, _, _, _>>.FullName) && methInfo.Name = "Gather" ->
        env, ImageGather (arg1, arg2, arg3, mkSpirvType expr.Type) |> SpirvIntrinsicCall

    | Call(_, methInfo, _), _, [arg1;arg2] when methInfo.DeclaringType.FullName.StartsWith(typedefof<SampledImage<_, _, _, _, _, _, _, _>>.FullName) && methInfo.Name = "ImplicitLod" ->
        env, ImplicitLod (arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall

    | SpecificCall <@ SpirvInstrinsics.VectorShuffle<_> @> _, [|_|], [arg1;arg2] ->
        env, VectorShuffle(arg1, arg2, [0u;1u], mkSpirvType expr.Type) |> SpirvIntrinsicCall

    | SpecificCall <@ kill @> _, [||], [] ->
        env, SpirvIntrinsicCall Kill

    | SpecificCall <@ (<) : float32 -> float32 -> bool @> _, _, [arg1;arg2] ->
        env, FloatUnorderedLessThan(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall
        
    | SpecificCall <@ Vector4.Multiply : Vector4 * float32 -> Vector4 @> _, _, [arg1;arg2] ->
        env, VectorTimesScalar(arg1, arg2, mkSpirvType expr.Type) |> SpirvIntrinsicCall

    | _ ->
        failwithf "Call not supported: %A" expr

and CheckIntrinsicField env receiver fieldInfo =
    let env, spvReceiver = CheckExpr env false receiver
    match fieldInfo.Name with
    | "X" when receiver.Type = typeof<Vector2> ->
        env, Vector2_Get_X(spvReceiver, SpirvTypeFloat32)
    | "Y" when receiver.Type = typeof<Vector2> ->
        env, Vector2_Get_Y(spvReceiver, SpirvTypeFloat32)

    | "X" when receiver.Type = typeof<Vector2Int> ->
        env, Vector2Int_Get_X(spvReceiver, SpirvTypeInt32)
    | "Y" when receiver.Type = typeof<Vector2Int> ->
        env, Vector2Int_Get_Y(spvReceiver, SpirvTypeInt32)

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
