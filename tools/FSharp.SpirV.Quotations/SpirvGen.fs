[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.SpirvGen

open System
open System.Collections.Generic
open System.Numerics
open FSharp.NativeInterop
open FSharp.Spirv
open Tast

[<NoEquality;NoComparison>]
type cenv =
    {
        mutable nextResultId: IdResult

        // Types, Variables, Constants

        types: Dictionary<IdResult, Instruction>
        typesByType: Dictionary<SpirvType, IdResult>
        typeFunctions: Dictionary<IdRef list, IdResult * Instruction list>
        typePointers: Dictionary<IdResult, Instruction>
        typePointersByResultType: Dictionary<StorageClass * IdRef, IdResult>
        globalVariables: Dictionary<IdResult, Instruction * Decoration list>
        globalVariablesByVar: Dictionary<SpirvVar, IdResult>
        constants: Dictionary<SpirvConst, IdResult * Instruction>
        constantComposites: Dictionary<IdRef list, IdResult * Instruction>

        //

        decorationInstructions: ResizeArray<Instruction>

        // Functions

        functions: Dictionary<string, IdResult * Instruction list>
        mainInitInstructions: ResizeArray<Instruction>

        // Local

        localVariables: Dictionary<IdResult, Instruction>
        localVariablesByVar: Dictionary<SpirvVar, IdResult>
        currentInstructions: ResizeArray<Instruction>
        locals: Dictionary<IdResult, Instruction>

        // Debug Info

        debugNames: ResizeArray<(string * IdRef)>
    }

    static member Default =
        {
            nextResultId = 1u
            types = Dictionary ()
            typesByType = Dictionary ()
            typeFunctions = Dictionary ()
            typePointers = Dictionary ()
            typePointersByResultType = Dictionary ()
            globalVariables = Dictionary ()
            globalVariablesByVar = Dictionary ()
            constants = Dictionary ()
            constantComposites = Dictionary ()
            decorationInstructions = ResizeArray 100
            functions = Dictionary ()
            mainInitInstructions = ResizeArray 100
            localVariables = Dictionary ()
            localVariablesByVar = Dictionary ()
            currentInstructions = ResizeArray 100
            locals = Dictionary ()
            debugNames = ResizeArray 100
        }

[<NoEquality;NoComparison>]
type env =
    {
        entryPoint: uint32
    }

#nowarn "9" 
#nowarn "40" 
#nowarn "51" 

let nextResultId cenv =
    let resultId = cenv.nextResultId
    cenv.nextResultId <- cenv.nextResultId + 1u
    resultId

let addInstructions cenv instrs =
    cenv.currentInstructions.AddRange instrs

let addDecorationInstructions cenv instrs =
    cenv.decorationInstructions.AddRange instrs

let addMainInitInstructions cenv instrs =
    cenv.mainInitInstructions.AddRange instrs

let getTypeInstruction cenv id =
    cenv.types.[id]

let getTypeByTypeInstruction cenv ty : IdRef =
    cenv.typesByType.[ty]

let getTypePointerInstruction cenv id =
    cenv.typePointers.[id]
    
let isAggregateType ty =
    match ty with
    | SpirvTypeStruct _
    | SpirvTypeArray _ -> true
    | _ -> false

let isCompositeType ty =
    match ty with
    | SpirvTypeVector2
    | SpirvTypeVector2Int
    | SpirvTypeVector3
    | SpirvTypeVector4 -> true
    | _ -> isAggregateType ty

let getTypeVectorInfo cenv ty =
    let resultType = getTypeByTypeInstruction cenv ty
    match getTypeInstruction cenv resultType with
    | OpTypeVector(_, componentType, componentCount) ->
        (componentType, componentCount)
    | _ ->
        failwith "Invalid vector type."

let addCompositeExtractInstruction cenv componentTypeId compositeId indices  =
    let resultId = nextResultId cenv
    addInstructions cenv [OpCompositeExtract(componentTypeId, resultId, compositeId, indices)]
    resultId

let addCompositeExtractInstructions cenv ty composite =
    let componentType, componentCount = getTypeVectorInfo cenv ty
    [ for i = 0 to int componentCount - 1 do
        yield addCompositeExtractInstruction cenv componentType composite [uint32 i] ]

let tryAddCompositeExtractInstructions cenv ty composite =
    if isCompositeType ty then
        addCompositeExtractInstructions cenv ty composite
    else
        []

let emitTypeAux cenv ty f =
    match cenv.typesByType.TryGetValue ty with
    | true, resultType -> resultType
    | _ ->
        let resultId = nextResultId cenv
        cenv.typesByType.[ty] <- resultId
        cenv.types.[resultId] <- f resultId
        cenv.debugNames.Add("type_" + ty.Name, resultId)

        match ty with
        | SpirvType.SpirvTypeStruct(_, fields) ->
            let mutable offset = 0u
            fields
            |> List.iteri (fun i (SpirvField(_, fieldTy, _)) ->
                let i = uint32 i
                match fieldTy with
                | SpirvTypeInt _ -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                | SpirvTypeFloat _ -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                | SpirvTypeVector2 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                | SpirvTypeVector3 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                | SpirvTypeVector4 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                | SpirvTypeMatrix4x4 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                | _ -> failwithf "Invalid field type, %A." fieldTy // TODO
                offset <- offset + uint32 fieldTy.Size)
            addDecorationInstructions cenv [OpDecorate(resultId, Decoration.Block)]
        | SpirvType.SpirvTypeMatrix4x4 ->
            addDecorationInstructions cenv [OpDecorate(resultId, Decoration.MatrixStride 0u)]
        | _ -> ()       

        resultId

let emitTypeVoid cenv =
    emitTypeAux cenv SpirvTypeVoid (fun resultId -> OpTypeVoid resultId)

let emitTypeUInt32 cenv =
    emitTypeAux cenv SpirvTypeUInt32 (fun resultId -> OpTypeInt(resultId, 32u, 0u))

let emitPointer cenv storageClass typeId =    
    match cenv.typePointersByResultType.TryGetValue ((storageClass, typeId)) with
    | true, resultId -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typePointersByResultType.[(storageClass, typeId)] <- resultId
        cenv.typePointers.[resultId] <- OpTypePointer(resultId, storageClass, typeId)
        resultId

let emitConstantAux cenv resultType debugName literal =
    match cenv.constants.TryGetValue literal with
    | true, (resultId, _) -> resultId
    | _ ->
        let valuePtr =
            match literal with
            | SpirvConstInt (value, _) -> &&value |> NativePtr.toVoidPtr
            | SpirvConstSingle (value, _) -> &&value |> NativePtr.toVoidPtr
            | _ -> failwith "Invalid constant."
        let value = BitConverter.ToUInt32(ReadOnlySpan(valuePtr, 4))

        let resultId = nextResultId cenv
        cenv.constants.[literal] <- (resultId, OpConstant(resultType, resultId, value))
        cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
        resultId

let emitConstantComposite cenv resultType debugName constituents =
    match cenv.constantComposites.TryGetValue constituents with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.constantComposites.[constituents] <- (resultId, OpConstantComposite(resultType, resultId, constituents))
        cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
        resultId

let rec emitType cenv ty =
    match ty with
    | SpirvTypeVoid -> emitTypeVoid cenv
    | SpirvTypeInt (width, sign) -> emitTypeAux cenv ty (fun resultId -> OpTypeInt(resultId, uint32 width, if sign then 1u else 0u))
    | SpirvTypeFloat width -> emitTypeAux cenv ty (fun resultId -> OpTypeFloat(resultId, uint32 width))
    | SpirvTypeVector2 -> emitTypeVector2 cenv
    | SpirvTypeVector2Int -> emitTypeVector2Int cenv
    | SpirvTypeVector3 -> emitTypeVector3 cenv
    | SpirvTypeVector4 -> emitTypeVector4 cenv
    | SpirvTypeMatrix4x4 -> emitTypeMatrix4x4 cenv
    | SpirvTypeArray (elementTy, length) -> emitArrayType cenv elementTy length
    | SpirvTypeStruct (_, fields) -> emitStructType cenv ty fields
    | SpirvTypeImage imageTy -> emitTypeImage cenv imageTy
    | SpirvTypeSampler -> emitTypeSampler cenv
    | SpirvTypeSampledImage imageTy -> emitTypeSampledImage cenv imageTy

and emitTypeVector2 cenv =
    let componentType = emitType cenv SpirvTypeFloat32
    emitTypeAux cenv SpirvTypeVector2 (fun resultId -> OpTypeVector(resultId, componentType, 2u))

and emitTypeVector2Int cenv =
    let componentType = emitType cenv SpirvTypeInt32
    emitTypeAux cenv SpirvTypeVector2Int (fun resultId -> OpTypeVector(resultId, componentType, 2u))

and emitTypeVector3 cenv =
    let componentType = emitType cenv SpirvTypeFloat32
    emitTypeAux cenv SpirvTypeVector3 (fun resultId -> OpTypeVector(resultId, componentType, 3u))

and emitTypeVector4 cenv =
    let componentType = emitType cenv SpirvTypeFloat32
    emitTypeAux cenv SpirvTypeVector4 (fun resultId -> OpTypeVector(resultId, componentType, 4u))

and emitTypeMatrix4x4 cenv =
    let columnType = emitTypeVector4 cenv
    emitTypeAux cenv SpirvTypeMatrix4x4 (fun resultId -> OpTypeMatrix(resultId, columnType, 4u))

and emitTypeSampler cenv =
    emitTypeAux cenv SpirvTypeSampler (fun resultId -> OpTypeSampler resultId)

and emitArrayType cenv elementTy length =
    match elementTy with
    | SpirvTypeVoid -> failwithf "Element type, %A, is not valid." elementTy
    | _ ->
        let elementTyId = emitType cenv elementTy
        let lengthId = uint32 length |> emitConstantUInt32 cenv
        emitTypeAux cenv (SpirvTypeArray (elementTy, length)) (fun arrayTyId -> OpTypeArray(arrayTyId, elementTyId, lengthId))

and emitStructType cenv ty fields =
    let idRefs =
        fields
        |> List.map (fun (SpirvField(_, fieldTy, _)) -> 
            emitType cenv fieldTy
        )

    emitTypeAux cenv ty (fun resultId -> OpTypeStruct(resultId, idRefs))

and emitTypeImage cenv imageTy =
    match imageTy with
    | SpirvImageType(sampledType, dim, depth, arrayed, ms, sampled, format, accessQualifier) ->
        let sampledTypeId = emitType cenv sampledType
        emitTypeAux cenv (SpirvTypeImage imageTy) (fun resultId ->
            OpTypeImage(resultId, sampledTypeId, dim, depth, arrayed, ms, sampled, format, accessQualifier))

and emitTypeSampledImage cenv imageTy =
    let imageTyId = emitTypeImage cenv imageTy
    emitTypeAux cenv (SpirvTypeSampledImage imageTy) (fun resultId -> OpTypeSampledImage(resultId, imageTyId))

and emitTypeFunction cenv paramTys retTy =
    let paramTyIds =
        paramTys
        |> List.filter (fun x -> x <> SpirvTypeVoid)
        |> List.map (emitType cenv)
    let retTyId = emitType cenv retTy

    let tys = paramTyIds @ [retTyId]

    match cenv.typeFunctions.TryGetValue tys with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typeFunctions.[tys] <- (resultId, [OpTypeFunction(resultId, retTyId, paramTyIds)])
        resultId

and emitConstantInt cenv (n: int) =
    emitConstantAux cenv (emitType cenv SpirvTypeInt32) "int" (SpirvConstInt(n, []))

and emitConstantUInt32 cenv (n: uint32) =
    emitConstantAux cenv (emitType cenv SpirvTypeInt32) "uint32" (SpirvConstUInt32(n, []))

let emitConstantSingle cenv (n: single) =
    emitConstantAux cenv (emitType cenv SpirvTypeFloat32) "single" (SpirvConstSingle(n, []))

let emitConstantVector2 cenv (constituents: IdRef list) =
    emitConstantComposite cenv (emitTypeVector2 cenv) "Vector2" constituents

let emitConstantVector2Int cenv (constituents: IdRef list) =
    emitConstantComposite cenv (emitTypeVector2Int cenv) "Vector2<int>" constituents

let emitConstantVector3 cenv (constituents: IdRef list) =
    emitConstantComposite cenv (emitTypeVector3 cenv) "Vector3" constituents

let emitConstantVector4 cenv (constituents: IdRef list) =
    emitConstantComposite cenv (emitTypeVector4 cenv) "Vector4" constituents

let emitConstantMatrix4x4 cenv (constituents: IdRef list) =
    emitConstantComposite cenv (emitTypeMatrix4x4 cenv) "Matrix4x4" constituents

let rec isConstant expr =
    match expr with
    | SpirvConst _ -> true
    | _ -> false

let rec GenConst cenv spvConst =
    match spvConst with
    | SpirvConstInt (n, decorations) ->
        emitConstantInt cenv n

    | SpirvConstUInt32 (n, decorations) ->
        emitConstantUInt32 cenv n

    | SpirvConstSingle (n, docorations) ->
        emitConstantSingle cenv n

    | SpirvConstVector2 (n1, n2, decorations) ->
        emitConstantVector2 cenv ([n1;n2] |> List.map (emitConstantSingle cenv))

    | SpirvConstVector2Int (n1, n2, decorations) ->
        emitConstantVector2Int cenv ([n1;n2] |> List.map (emitConstantInt cenv))

    | SpirvConstVector3 (n1, n2, n3, decorations) ->
        emitConstantVector3 cenv ([n1;n2;n3] |> List.map (emitConstantSingle cenv))

    | SpirvConstVector4 (n1, n2, n3, n4, decorations) ->
        emitConstantVector4 cenv ([n1;n2;n3;n4] |> List.map (emitConstantSingle cenv))

    | SpirvConstMatrix4x4 (m11, m12, m13, m14,
                           m21, m22, m23, m24,
                           m31, m32, m33, m34,
                           m41, m42, m43, m44, decorations) ->
        let constituents =
            [ [m11; m12; m13; m14] |> List.map (emitConstantSingle cenv) |> emitConstantVector4 cenv
              [m21; m22; m23; m24] |> List.map (emitConstantSingle cenv) |> emitConstantVector4 cenv
              [m31; m32; m33; m34] |> List.map (emitConstantSingle cenv) |> emitConstantVector4 cenv
              [m41; m42; m43; m44] |> List.map (emitConstantSingle cenv) |> emitConstantVector4 cenv ]
        emitConstantMatrix4x4 cenv constituents

    | SpirvConstArray (elementTy, constants, decorations) ->
        let arrayTyId = emitArrayType cenv elementTy constants.Length
        let constantIds = constants |> List.map (GenConst cenv)
        emitConstantComposite cenv arrayTyId (elementTy.Name + "[" + string constantIds.Length + "]") constantIds

let GenLocalVar cenv spvVar =
    let resultType = emitType cenv spvVar.Type |> emitPointer cenv StorageClass.Function
    let resultId = nextResultId cenv
    cenv.localVariables.[resultId] <- OpVariable(resultType, resultId, StorageClass.Function, None)
    cenv.localVariablesByVar.[spvVar] <- resultId
    cenv.debugNames.Add(string resultId + "_" + spvVar.Name, resultId)
    resultId

let GenGlobalVar cenv spvVar =
    match cenv.globalVariablesByVar.TryGetValue spvVar with
    | true, resultId -> resultId
    | _ ->
        let storageClass = spvVar.StorageClass

        let resultType = emitType cenv spvVar.Type |> emitPointer cenv storageClass
        let resultId = nextResultId cenv
        cenv.globalVariables.[resultId] <- (OpVariable(resultType, resultId, storageClass, None), spvVar.Decorations)
        cenv.globalVariablesByVar.[spvVar] <- resultId
        cenv.debugNames.Add(string resultId + "_" + spvVar.Name, resultId)
        resultId

let getAccessChainResultType cenv pointer index =
    let resultType = 
        match cenv.localVariables.TryGetValue pointer with
        | true, OpVariable(resultType, _, _, _) -> resultType
        | _ ->
            match cenv.globalVariables.TryGetValue pointer with
            | true, (OpVariable(resultType, _, _, _), _) -> resultType
            | _ ->  failwithf "Unable to find variable: %A" pointer

    match cenv.typePointers.TryGetValue resultType with
    | true, OpTypePointer(_, storageClass, baseType) -> 
        match cenv.types.TryGetValue baseType with
        | true, OpTypeArray (_, elementType, _) when index = 0 -> elementType
        | true, OpTypeStruct (_, fields) when index >= 0 && index < fields.Length -> fields.[index]
        | _ -> failwith "Unable to get backing type for pointer."
        |> emitPointer cenv storageClass
    | _ ->
        failwith "Invalid pointer type."

let tryEmitLoad cenv pointer =
    let resultTypeOpt = 
        match cenv.localVariables.TryGetValue pointer with
        | true, OpVariable(resultType, _, _, _) -> resultType |> Some
        | _ ->
            match cenv.globalVariables.TryGetValue pointer with
            | true, (OpVariable(resultType, _, _, _), _) -> resultType |> Some
            | _ ->
                match cenv.locals.TryGetValue pointer with
                | true, OpAccessChain(resultType, _, _, _) -> resultType |> Some
                | _ -> None

    match resultTypeOpt with
    | Some resultType ->
        let baseType =
            match cenv.typePointers.TryGetValue resultType with
            | true, OpTypePointer(_, _, baseType) -> baseType
            | _ -> failwith "Invalid pointer type."

        let resultId = nextResultId cenv
        addInstructions cenv [OpLoad(baseType, resultId, pointer, None)]
        Some resultId
    | _ ->
        None

let deref cenv (resultId: IdResult) =
    match tryEmitLoad cenv resultId with
    | Some resultId -> resultId
    | _ -> resultId

//let tryEmitCopyObject cenv pointer  =
    

let emitLoad cenv pointer =
    match tryEmitLoad cenv pointer with
    | Some resultId -> resultId
    | _ -> failwith "Unable to emit load instruction."

let rec GenExpr cenv (env: env) expr =
    match expr with

    | SpirvNop -> 0u

    | SpirvConst spvConst ->
        GenConst cenv spvConst

    | SpirvLet(spvVar, rhs, body) ->
        let spvVarId = GenLocalVar cenv spvVar
        let rhsId = GenExpr cenv env rhs |> deref cenv
        addInstructions cenv [OpStore(spvVarId, rhsId, None)]
        GenExpr cenv env body

    | SpirvSequential(expr1, expr2) ->
        GenExpr cenv env expr1 |> ignore
        GenExpr cenv env expr2

    | SpirvNewVector2 args
    | SpirvNewVector2Int args
    | SpirvNewVector3 args 
    | SpirvNewVector4 args ->
        GenVector cenv env expr.Type args

    | SpirvArrayIndexerGet (receiver, arg) ->
        let receiverId = GenExpr cenv env receiver
        let argId = GenExpr cenv env arg

        let resultType = getAccessChainResultType cenv receiverId 0
        let index = emitLoad cenv argId
        let resultId = nextResultId cenv
        let op = OpAccessChain(resultType, resultId, receiverId, [index])
        addInstructions cenv [op]
        cenv.locals.[resultId] <- op
        resultId

    | SpirvVar spvVar ->
        match cenv.localVariablesByVar.TryGetValue spvVar with
        | true, resultId -> resultId
        | _ -> cenv.globalVariablesByVar.[spvVar]

    | SpirvVarSet (var, rhs) ->
        if not var.IsMutable then
            failwithf "'%s' is not mutable." var.Name

        let rhsId = GenExpr cenv env rhs |> deref cenv
        let id = GenGlobalVar cenv var
        addInstructions cenv [OpStore(id, rhsId, None)]
        id

    | SpirvIntrinsicCall call ->
        let retTy = emitType cenv call.ReturnType
        match call with
        | Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2) ->
            let arg1 = GenExpr cenv env arg1 |> deref cenv
            let arg2 = GenExpr cenv env arg2 |> deref cenv

            let resultId = nextResultId cenv
            addInstructions cenv [OpMatrixTimesVector(retTy, resultId, arg2, arg1)]
            resultId

        | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) ->
            let arg1 = GenExpr cenv env arg1 |> deref cenv
            let arg2 = GenExpr cenv env arg2 |> deref cenv

            let resultId = nextResultId cenv
            addInstructions cenv [OpMatrixTimesMatrix(retTy, resultId, arg2, arg1)]
            resultId

        | ConvertAnyFloatToAnySInt arg ->
            let arg = GenExpr cenv env arg |> deref cenv

            let resultId = nextResultId cenv
            addInstructions cenv [OpConvertFToS(retTy, resultId, arg)]
            resultId
            
        | GetImage arg ->
            let arg = GenExpr cenv env arg |> deref cenv

            let resultId = nextResultId cenv
            addInstructions cenv [OpImage(retTy, resultId, arg)]
            resultId

        | ImageFetch (arg1, arg2, _) ->
            let arg1 = GenExpr cenv env arg1 |> deref cenv
            let arg2 = GenExpr cenv env arg2 |> deref cenv

            let resultId = nextResultId cenv
            addInstructions cenv [OpImageFetch(retTy, resultId, arg1, arg2, None)]
            resultId

        | ImageGather (arg1, arg2, arg3, _) ->
            let arg1 = GenExpr cenv env arg1 |> deref cenv
            let arg2 = GenExpr cenv env arg2 |> deref cenv
            let arg3 = GenExpr cenv env arg3 |> deref cenv

            let resultId = nextResultId cenv
            addInstructions cenv [OpImageGather(retTy, resultId, arg1, arg2, arg3, None)]
            resultId

    | SpirvIntrinsicFieldGet fieldGet ->
        let getComponent receiver fieldTy n =
            let receiverId = GenExpr cenv env receiver |> deref cenv
            let fieldTyId = emitType cenv fieldTy
            addCompositeExtractInstruction cenv fieldTyId receiverId [n]
            
        match fieldGet with
        | Vector2_Get_X(receiver, fieldTy) 
        | Vector3_Get_X(receiver, fieldTy)
        | Vector4_Get_X(receiver, fieldTy) ->
            getComponent receiver fieldTy 0u
        | Vector2_Get_Y(receiver, fieldTy) 
        | Vector3_Get_Y(receiver, fieldTy)
        | Vector4_Get_Y(receiver, fieldTy) ->
            getComponent receiver fieldTy 1u
        | Vector3_Get_Z(receiver, fieldTy)
        | Vector4_Get_Z(receiver, fieldTy) ->
            getComponent receiver fieldTy 2u
        | Vector4_Get_W(receiver, fieldTy) ->
            getComponent receiver fieldTy 3u

    | SpirvFieldGet (receiver, index) ->
        let receiverId = GenExpr cenv env receiver

        let resultType = getAccessChainResultType cenv receiverId index

        match getTypePointerInstruction cenv resultType with
        | OpTypePointer _ -> ()
        | _ -> failwith "Invalid pointer."

        let indexId = GenExpr cenv env (SpirvConst (SpirvConstInt(index, [])))
        let accessChainPointerId = nextResultId cenv
        let op = OpAccessChain(resultType, accessChainPointerId, receiverId, [indexId])
        addInstructions cenv [op]
        cenv.locals.[accessChainPointerId] <- op
        emitLoad cenv accessChainPointerId

and GenVector cenv env retTy args =
    let constituents =
        args
        |> List.map (fun arg ->
            let id = GenExpr cenv env arg |> deref cenv
            match tryAddCompositeExtractInstructions cenv arg.Type id with
            | [] -> [id]
            | ids -> ids
        )
        |> List.concat

    let resultId = nextResultId cenv
    addInstructions cenv [OpCompositeConstruct(emitType cenv retTy, resultId, constituents)]
    resultId

let GenDecl cenv decl =
    match decl with
    | SpirvDeclConst (var, constt) ->
        let varId = GenGlobalVar cenv var
        let constId = GenConst cenv constt
        addMainInitInstructions cenv [OpStore(varId, constId, None)]

    | SpirvDeclVar var ->
        GenGlobalVar cenv var |> ignore

let rec GenTopLevelExpr cenv env expr =
    match expr with
    | SpirvTopLevelSequential (expr1, expr2) ->
        GenTopLevelExpr cenv env expr1 |> ignore
        GenTopLevelExpr cenv env expr2

    | SpirvTopLevelDecl decl ->
        GenDecl cenv decl

    | SpirvTopLevelLambda (var, body) ->
        if not var.Type.IsVoid then
            GenGlobalVar cenv var |> ignore
        GenTopLevelExpr cenv env body

    | SpirvTopLevelLambdaBody (var, body) ->
        if not var.Type.IsVoid then
            GenGlobalVar cenv var |> ignore
        GenExpr cenv env body |> ignore

and GenMain cenv env expr =
    GenTopLevelExpr cenv env expr |> ignore

    let instrs = ResizeArray cenv.currentInstructions
    let localVarInstrs = ResizeArray (cenv.localVariables.Values)

    cenv.currentInstructions.Clear()
    cenv.localVariables.Clear()

    let funInstrs = cenv.currentInstructions

    OpFunction(
        emitTypeVoid cenv, env.entryPoint, FunctionControl.None, 
        emitTypeFunction cenv [SpirvTypeVoid] SpirvTypeVoid)
    |> funInstrs.Add

    OpLabel(nextResultId cenv)
    |> funInstrs.Add

    funInstrs.AddRange localVarInstrs

    funInstrs.AddRange cenv.mainInitInstructions
    cenv.mainInitInstructions.Clear()

    funInstrs.AddRange instrs

    funInstrs.Add OpReturn
    funInstrs.Add OpFunctionEnd

    []

let GenModule (info: SpirvGenInfo) expr =
    let cenv = cenv.Default

    let entryPoint = nextResultId cenv

    GenMain cenv { entryPoint = entryPoint } expr |> ignore

    let annotations =
        let annoations = ResizeArray ()
        cenv.globalVariables
        |> Seq.iter (fun pair ->
            let resultId = pair.Key
            let instr, decorations = pair.Value

            match instr with
            | OpVariable (_, _, StorageClass.Input, _)
            | OpVariable (_, _, StorageClass.Output, _) 
            | OpVariable (_, _, StorageClass.Private, _)
            | OpVariable (_, _, StorageClass.Uniform, _) 
            | OpVariable (_, _, StorageClass.UniformConstant, _) 
            | OpVariable (_, _, StorageClass.Image, _) ->
                decorations
                |> List.iter (fun decoration ->
                    OpDecorate(resultId, decoration)
                    |> annoations.Add
                )
            | _ ->
                failwithf "Invalid instruction or global variable not supported: %A" instr
        )
        annoations
        |> List.ofSeq

    let typesAndConstants =
        (cenv.types
         |> Seq.map (fun pair -> pair.Key, pair.Value)
         |> List.ofSeq)
        @
        (cenv.typePointers
         |> Seq.map (fun pair -> pair.Key, pair.Value)
         |> List.ofSeq)
        @
        (cenv.typeFunctions.Values
         |> Seq.map (fun (resultId, instrs) -> instrs |> List.map (fun x -> resultId, x))
         |> Seq.concat
         |> List.ofSeq)
        @
        (cenv.constants.Values
         |> List.ofSeq)
        @
        (cenv.constantComposites.Values
         |> List.ofSeq)
        |> List.sortBy (fun (resultId, _) -> resultId)
        |> List.map (fun (_, instr) -> instr)

    let variables =
        cenv.globalVariables.Values
        |> Seq.map (fun (instr, _) -> instr)
        |> List.ofSeq

    let interfaces =
        cenv.globalVariablesByVar
        |> Seq.choose (fun pair ->
            let var = pair.Key
            let resultId = pair.Value
            match var.StorageClass with
            | StorageClass.Input
            | StorageClass.Output -> Some resultId
            | _ -> None
        )
        |> List.ofSeq

    let instrs = 
        (info.Capabilities
         |> List.map OpCapability)
        @
        (info.ExtendedInstructionSets
         |> List.map (fun x -> OpExtInstImport (nextResultId cenv, x)))
        @
        [OpMemoryModel (info.AddressingModel, info.MemoryModel)]
        @
        [OpEntryPoint (info.ExecutionModel, entryPoint, "main", interfaces)]
        @
        (info.ExecutionMode
         |> Option.map (fun executionMode -> OpExecutionMode (entryPoint, executionMode))
         |> Option.toList)
        @
        (cenv.debugNames
         |> Seq.map (fun (name, id) -> OpName (id, name))
         |> List.ofSeq)
        @
        annotations 
        @
        (cenv.decorationInstructions |> List.ofSeq)
        @
        typesAndConstants @ variables 
        @
        (cenv.currentInstructions |> List.ofSeq)

    SpirvModule.Create(instrs = instrs)