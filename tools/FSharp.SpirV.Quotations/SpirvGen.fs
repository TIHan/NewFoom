[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.SpirvGen

open System
open System.Collections.Generic
open System.Numerics
open FSharp.NativeInterop
open FSharp.Spirv
open TypedTree

[<NoEquality;NoComparison>]
type cenv =
    {
        nextResultId: IdResult ref

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

        labels: HashSet<IdRef>

        decorationInstructions: ResizeArray<Instruction>

        // Functions

        functionsByVar: Dictionary<SpirvVar, IdResult>
        functionParametersByVar: Dictionary<SpirvVar, IdResult>
        entryPointInstructions: ResizeArray<Instruction>

        // Local

        localVariables: Dictionary<IdResult, Instruction>
        localVariablesByVar: Dictionary<SpirvVar, IdResult>
        currentInstructions: ResizeArray<Instruction>
        locals: Dictionary<IdResult, Instruction>

        // Debug Info

        debugNames: ResizeArray<(string * IdRef)>

        // Other

        mutable lastLabel: IdRef
        mutable ignoreAddingInstructions: bool
    }

    static member Default =
        {
            nextResultId = ref 1u
            types = Dictionary()
            typesByType = Dictionary()
            typeFunctions = Dictionary()
            typePointers = Dictionary()
            typePointersByResultType = Dictionary()
            globalVariables = Dictionary()
            globalVariablesByVar = Dictionary()
            constants = Dictionary()
            constantComposites = Dictionary()
            labels = HashSet()
            decorationInstructions = ResizeArray ()
            functionsByVar = Dictionary ()
            functionParametersByVar = Dictionary ()
            entryPointInstructions = ResizeArray ()
            localVariables = Dictionary ()
            localVariablesByVar = Dictionary ()
            currentInstructions = ResizeArray ()
            locals = Dictionary ()
            debugNames = ResizeArray ()
            lastLabel = 0u
            ignoreAddingInstructions = false
        }

[<NoEquality;NoComparison>]
type env =
    {
        blockScope: int
        blockLabel: uint32
    }

#nowarn "9" 
#nowarn "40" 
#nowarn "51" 

let nextResultId cenv =
    let resultId = !cenv.nextResultId
    cenv.nextResultId := !cenv.nextResultId + 1u
    resultId

let addInstructions cenv instrs =
    if not cenv.ignoreAddingInstructions then
        for instr in instrs do
            match instr with
            // TODO: Add check that throws an error on OpKill in non-Fragment Execution Models.
            | OpKill -> cenv.ignoreAddingInstructions <- true
            | _ -> ()
            cenv.currentInstructions.Add instr

let addDecorationInstructions cenv instrs =
    cenv.decorationInstructions.AddRange instrs

let addEntryPointInstructions cenv instrs =
    cenv.entryPointInstructions.AddRange instrs

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

let GenTypeAux cenv ty f =
    match cenv.typesByType.TryGetValue ty with
    | true, resultType -> resultType
    | _ ->
        let resultId = nextResultId cenv
        cenv.typesByType.[ty] <- resultId
        cenv.types.[resultId] <- f resultId
        cenv.debugNames.Add("type_" + ty.Name, resultId)

        match ty with
        | SpirvType.SpirvTypeStruct(name, fields, isBlock) ->
            let mutable offset = 0u
            let mutable hasRuntimeArray = false
            let rec computeFields (fields: SpirvField list) =
                let fields = fields |> Array.ofList
                fields
                |> Array.iteri (fun i (SpirvField(_, fieldTy, _)) ->
                    let i = uint32 i
                    match fieldTy with
                    | SpirvTypeBool _ -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeInt _ -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeFloat _ -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeVector2 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeVector3 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeVector4 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeMatrix4x4 -> addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeRuntimeArray _ -> 
                        hasRuntimeArray <- true
                        addDecorationInstructions cenv [OpMemberDecorate(resultId, i, Decoration.Offset offset)]
                    | SpirvTypeStruct(_, fields, _) ->
                        computeFields fields
                    | _ -> failwithf "Invalid field type, %A." fieldTy // TODO
                    if i = uint32 (fields.Length - 1) then
                        () // skip offset calcuation on last field
                    else
                        offset <- offset + uint32 fieldTy.SizeHint)
            computeFields fields
            if isBlock then
                addDecorationInstructions cenv [OpDecorate(resultId, Decoration.Block)]
            else
                if hasRuntimeArray then
                    failwithf "Struct type, %s, must be marked as a block because it contains a runtime array." name
        | SpirvType.SpirvTypeMatrix4x4 ->
            addDecorationInstructions cenv [OpDecorate(resultId, Decoration.MatrixStride 0u)]
        | SpirvType.SpirvTypeRuntimeArray elementTy ->
            addDecorationInstructions cenv [OpDecorate(resultId, Decoration.ArrayStride(uint32 elementTy.SizeHint))]
        | _ -> ()       

        resultId

let GenTypeVoid cenv =
    GenTypeAux cenv SpirvTypeVoid (fun resultId -> OpTypeVoid resultId)

let GenTypeUInt32 cenv =
    GenTypeAux cenv SpirvTypeUInt32 (fun resultId -> OpTypeInt(resultId, 32u, 0u))

let emitTypePointer cenv storageClass typeId =
    match cenv.typePointersByResultType.TryGetValue ((storageClass, typeId)) with
    | true, resultId -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typePointersByResultType.[(storageClass, typeId)] <- resultId
        cenv.typePointers.[resultId] <- OpTypePointer(resultId, storageClass, typeId)
        resultId

let GenConstantAux cenv resultType debugName literal =
    match cenv.constants.TryGetValue literal with
    | true, (resultId, _) -> resultId
    | _ ->
        let create value =
            let resultId = nextResultId cenv
            cenv.constants.[literal] <- (resultId, OpConstant(resultType, resultId, value))
            cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
            resultId

        let createBoolTrue () =
            let resultId = nextResultId cenv
            cenv.constants.[literal] <- (resultId, OpSpecConstantTrue(resultType, resultId))
            cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
            resultId

        let createBoolFalse () =
            let resultId = nextResultId cenv
            cenv.constants.[literal] <- (resultId, OpSpecConstantFalse(resultType, resultId))
            cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
            resultId

        match literal with
        | SpirvConstInt (value, _) -> 
            let mutable value = value
            let valuePtr = &&value |> NativePtr.toVoidPtr
            BitConverter.ToUInt32(ReadOnlySpan(valuePtr, 4))
            |> create
        | SpirvConstSingle (value, _) ->
            let mutable value = value
            let valuePtr = &&value |> NativePtr.toVoidPtr
            BitConverter.ToUInt32(ReadOnlySpan(valuePtr, 4))
            |> create
        | SpirvConstBool (value, _, _) -> if value then createBoolTrue () else createBoolFalse ()
        | _ -> failwith "Invalid constant."

let GenConstantComposite cenv resultType debugName constituents =
    match cenv.constantComposites.TryGetValue constituents with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.constantComposites.[constituents] <- (resultId, OpConstantComposite(resultType, resultId, constituents))
        cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
        resultId

let rec GenType cenv ty =
    match ty with
    | SpirvTypeVoid -> GenTypeVoid cenv
    | SpirvTypeBool _ -> GenTypeAux cenv ty (fun resultId -> OpTypeBool resultId)
    | SpirvTypeInt(width, sign) -> GenTypeAux cenv ty (fun resultId -> OpTypeInt(resultId, uint32 width, if sign then 1u else 0u))
    | SpirvTypeFloat width -> GenTypeAux cenv ty (fun resultId -> OpTypeFloat(resultId, uint32 width))
    | SpirvTypeVector2 -> GenTypeVector2 cenv
    | SpirvTypeVector3 -> GenTypeVector3 cenv
    | SpirvTypeVector4 -> GenTypeVector4 cenv
    | SpirvTypeMatrix4x4 -> GenTypeMatrix4x4 cenv
    | SpirvTypeArray(elementTy, length) -> GenTypeArray cenv elementTy length
    | SpirvTypeRuntimeArray elementTy -> GenTypeRuntimeArray cenv elementTy
    | SpirvTypeStruct(_, fields, _) -> GenTypeStruct cenv ty fields
    | SpirvTypeImage imageTy -> GenTypeImage cenv imageTy
    | SpirvTypeSampler -> GenTypeSampler cenv
    | SpirvTypeSampledImage imageTy -> GenTypeSampledImage cenv imageTy
    | SpirvTypeFunction(parTys, retTy) -> GenTypeFunction cenv parTys retTy
    | SpirvTypePointer(ty, storageClass) -> GenTypePointer cenv ty storageClass 

and GenTypePointer cenv ty storageClass =
   let tyId = GenType cenv ty
   GenTypeAux cenv (SpirvTypePointer(ty, storageClass)) (fun resultId -> OpTypePointer(resultId, storageClass, tyId))

and GenTypeVector2 cenv =
    let componentType = GenType cenv SpirvTypeFloat32
    GenTypeAux cenv SpirvTypeVector2 (fun resultId -> OpTypeVector(resultId, componentType, 2u))

and GenTypeVector3 cenv =
    let componentType = GenType cenv SpirvTypeFloat32
    GenTypeAux cenv SpirvTypeVector3 (fun resultId -> OpTypeVector(resultId, componentType, 3u))

and GenTypeVector4 cenv =
    let componentType = GenType cenv SpirvTypeFloat32
    GenTypeAux cenv SpirvTypeVector4 (fun resultId -> OpTypeVector(resultId, componentType, 4u))

and GenTypeMatrix4x4 cenv =
    let columnType = GenTypeVector4 cenv
    GenTypeAux cenv SpirvTypeMatrix4x4 (fun resultId -> OpTypeMatrix(resultId, columnType, 4u))

and GenTypeSampler cenv =
    GenTypeAux cenv SpirvTypeSampler (fun resultId -> OpTypeSampler resultId)

and GenTypeArray cenv elementTy length =
    match elementTy with
    | SpirvTypeVoid -> failwithf "Element type, %A, is not valid." elementTy
    | _ ->
        let elementTyId = GenType cenv elementTy
        let lengthId = uint32 length |> GenConstantUInt32 cenv
        GenTypeAux cenv (SpirvTypeArray (elementTy, length)) (fun arrayTyId -> OpTypeArray(arrayTyId, elementTyId, lengthId))

and GenTypeRuntimeArray cenv elementTy =
    match elementTy with
    | SpirvTypeVoid -> failwithf "Element type, %A, is not valid." elementTy
    | _ ->
        let elementTyId = GenType cenv elementTy
        GenTypeAux cenv (SpirvTypeRuntimeArray elementTy) (fun arrayTyId -> OpTypeRuntimeArray(arrayTyId, elementTyId))

and GenTypeStruct cenv ty fields =
    let idRefs =
        fields
        |> List.map (fun (SpirvField(_, fieldTy, _)) -> 
            GenType cenv fieldTy
        )

    GenTypeAux cenv ty (fun resultId -> OpTypeStruct(resultId, idRefs))

and GenTypeImage cenv imageTy =
    match imageTy with
    | SpirvImageType(sampledType, dim, depth, arrayed, ms, sampled, format, accessQualifier) ->
        let sampledTypeId = GenType cenv sampledType
        GenTypeAux cenv (SpirvTypeImage imageTy) (fun resultId ->
            OpTypeImage(resultId, sampledTypeId, dim, depth, arrayed, ms, sampled, format, accessQualifier))

and GenTypeSampledImage cenv imageTy =
    let imageTyId = GenTypeImage cenv imageTy
    GenTypeAux cenv (SpirvTypeSampledImage imageTy) (fun resultId -> OpTypeSampledImage(resultId, imageTyId))

and GenTypeFunction cenv paramTys retTy =
    let paramTyIds =
        paramTys
        |> List.filter (fun x -> x <> SpirvTypeVoid)
        |> List.map (GenType cenv)
    let retTyId = GenType cenv retTy

    let tys = paramTyIds @ [retTyId]

    match cenv.typeFunctions.TryGetValue tys with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typeFunctions.[tys] <- (resultId, [OpTypeFunction(resultId, retTyId, paramTyIds)])
        resultId

and GenConstantBool cenv sizeHint (v: bool) =
    GenConstantAux cenv (GenType cenv (SpirvTypeBool sizeHint)) "bool" (SpirvConstBool(v, sizeHint, []))

and GenConstantInt cenv (n: int) =
    GenConstantAux cenv (GenType cenv SpirvTypeInt32) "Int32" (SpirvConstInt(n, []))

and GenConstantUInt32 cenv (n: uint32) =
    GenConstantAux cenv (GenType cenv SpirvTypeInt32) "UInt32" (SpirvConstUInt32(n, []))

let GenConstantFloat32 cenv (n: float32) =
    GenConstantAux cenv (GenType cenv SpirvTypeFloat32) "Float32" (SpirvConstSingle(n, []))

let GenConstantVector2 cenv (constituents: IdRef list) =
    GenConstantComposite cenv (GenTypeVector2 cenv) "Vector2" constituents

let GenConstantVector3 cenv (constituents: IdRef list) =
    GenConstantComposite cenv (GenTypeVector3 cenv) "Vector3" constituents

let GenConstantVector4 cenv (constituents: IdRef list) =
    GenConstantComposite cenv (GenTypeVector4 cenv) "Vector4" constituents

let GenConstantMatrix4x4 cenv (constituents: IdRef list) =
    GenConstantComposite cenv (GenTypeMatrix4x4 cenv) "Matrix4x4" constituents

let rec isConstant expr =
    match expr with
    | SpirvConst _ -> true
    | _ -> false

let rec GenConst cenv spvConst =
    match spvConst with
    | SpirvConstBool (n, sizeHint, decorations) ->
        GenConstantBool cenv sizeHint n
        
    | SpirvConstInt (n, decorations) ->
        GenConstantInt cenv n

    | SpirvConstUInt32 (n, decorations) ->
        GenConstantUInt32 cenv n

    | SpirvConstSingle (n, docorations) ->
        GenConstantFloat32 cenv n

    | SpirvConstVector2 (n1, n2, decorations) ->
        GenConstantVector2 cenv ([n1;n2] |> List.map (GenConstantFloat32 cenv))

    | SpirvConstVector3 (n1, n2, n3, decorations) ->
        GenConstantVector3 cenv ([n1;n2;n3] |> List.map (GenConstantFloat32 cenv))

    | SpirvConstVector4 (n1, n2, n3, n4, decorations) ->
        GenConstantVector4 cenv ([n1;n2;n3;n4] |> List.map (GenConstantFloat32 cenv))

    | SpirvConstMatrix4x4 (m11, m12, m13, m14,
                           m21, m22, m23, m24,
                           m31, m32, m33, m34,
                           m41, m42, m43, m44, decorations) ->
        let constituents =
            [ [m11; m12; m13; m14] |> List.map (GenConstantFloat32 cenv) |> GenConstantVector4 cenv
              [m21; m22; m23; m24] |> List.map (GenConstantFloat32 cenv) |> GenConstantVector4 cenv
              [m31; m32; m33; m34] |> List.map (GenConstantFloat32 cenv) |> GenConstantVector4 cenv
              [m41; m42; m43; m44] |> List.map (GenConstantFloat32 cenv) |> GenConstantVector4 cenv ]
        GenConstantMatrix4x4 cenv constituents

    | SpirvConstArray (elementTy, constants, decorations) ->
        let arrayTyId = GenTypeArray cenv elementTy constants.Length
        let constantIds = constants |> List.map (GenConst cenv)
        GenConstantComposite cenv arrayTyId (elementTy.Name + "[" + string constantIds.Length + "]") constantIds

let GenFunctionParameterVar cenv var =
    match cenv.localVariablesByVar.TryGetValue var with
    | true, resultId -> resultId
    | _ ->
        let resultType = GenType cenv var.Type
        let resultId = nextResultId cenv
        let instr = OpFunctionParameter(resultType, resultId)

        addInstructions cenv [instr]

        cenv.localVariablesByVar.[var] <- resultId
        cenv.debugNames.Add(string resultId + "_" + var.Name, resultId)
        resultId

let GenLocalVar cenv var =
    match cenv.localVariablesByVar.TryGetValue var with
    | true, resultId -> resultId
    | _ ->
        let resultType = GenType cenv var.Type |> emitTypePointer cenv StorageClass.Function
        let resultId = nextResultId cenv
        cenv.localVariables.[resultId] <- OpVariable(resultType, resultId, StorageClass.Function, None)
        cenv.localVariablesByVar.[var] <- resultId
        cenv.debugNames.Add(string resultId + "_" + var.Name, resultId)
        resultId

let rec GenGlobalVar cenv spvVar =
    match cenv.globalVariablesByVar.TryGetValue spvVar with
    | true, resultId -> resultId
    | _ ->
        let storageClass = spvVar.StorageClass

        if storageClass = StorageClass.Function then
            failwith "Invalid storage class for global variable."

        let resultType = GenType cenv spvVar.Type |> emitTypePointer cenv storageClass
        let resultId = nextResultId cenv
        cenv.globalVariables.[resultId] <- (OpVariable(resultType, resultId, storageClass, None), spvVar.Decorations)
        cenv.globalVariablesByVar.[spvVar] <- resultId
        cenv.debugNames.Add(string resultId + "_" + spvVar.Name, resultId)

        if storageClass = StorageClass.Input then
            match spvVar.Type with
            | SpirvTypeStruct(_, fields, _) ->
                // TODO: Maybe refactor this a bit better? Maybe lazily evaluate the existence of location and binding and put it into SpirvVar as a lazy evaluated member?
                // Review: This creates dummy variables if we have an input variable that is a struct. This ensures we get locations right.
                match spvVar.Decorations |> List.tryPick (function Decoration.Location n -> Some n | _ -> None) with
                | Some location ->
                    match spvVar.Decorations |> List.tryPick (function Decoration.Binding n -> Some n | _ -> None) with
                    | Some binding ->
                        let mutable location = location
                        fields
                        |> List.iter (fun field ->
                            GenGlobalVar 
                                cenv 
                                (mkSpirvVar("__" + spvVar.Name + "_" + field.Name, field.Type, [Decoration.Binding binding; Decoration.Location location], storageClass, spvVar.IsMutable)) |> ignore
                            location <- location + 1u)
                    | _ -> ()
                | _ -> ()
            | _ -> ()


        resultId

let StoreFunctionVar cenv spvVar rhs =
    cenv.functionsByVar.[spvVar] <- rhs

let getAccessChainResultType cenv pointer fieldIndex =
    let resultType = 
        match cenv.localVariables.TryGetValue pointer with
        | true, OpVariable(resultType, _, _, _) -> resultType
        | _ ->
            match cenv.globalVariables.TryGetValue pointer with
            | true, (OpVariable(resultType, _, _, _), _) -> resultType
            | _ ->
                match cenv.locals.TryGetValue pointer with
                | true, OpAccessChain(resultType, _, _, _) -> resultType
                | _ -> failwithf "Unable to find variable: %A" pointer

    match cenv.typePointers.TryGetValue resultType with
    | true, OpTypePointer(_, storageClass, baseType) -> 
        match cenv.types.TryGetValue baseType with
        | true, OpTypeRuntimeArray(_, elementType)
        | true, OpTypeArray (_, elementType, _) when fieldIndex = 0 -> elementType
        | true, OpTypeStruct (_, fields) when fieldIndex >= 0 && fieldIndex < fields.Length -> fields.[fieldIndex]
        | true, ty ->
            failwithf "Found type, %A, but unable to get backing pointer type on field index %i." ty fieldIndex
        | _ -> 
            failwith "Unable to get backing type for pointer."
        |> emitTypePointer cenv storageClass
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
            | true, OpTypePointer(_, _, baseType) -> baseType |> Some
            | _ -> failwith "Invalid pointer type."

        match baseType with
        | Some baseType ->
            let resultId = nextResultId cenv
            addInstructions cenv [OpLoad(baseType, resultId, pointer, None)]
            Some resultId
        | _ ->
            failwith "Invalid pointer type."
    | _ ->
        None

let emitLabel cenv label =
    if cenv.labels.Add label then
        addInstructions cenv [OpLabel label]
    else
        failwithf "Label, %i, has already been generated." label

let deref cenv (resultId: IdResult) =
    match tryEmitLoad cenv resultId with
    | Some resultId -> resultId
    | _ -> resultId

[<Literal>]
let ZeroResultId = 0u

let emitLoad cenv pointer =
    match tryEmitLoad cenv pointer with
    | Some resultId -> resultId
    | _ -> failwith "Unable to emit load instruction."

let checkValidResultId resultId =
    if resultId = ZeroResultId then
        failwith "Unexpected zero result."

let checkZeroResultId resultId =
    if resultId <> ZeroResultId then
        failwith "Unexpected result."

type Returnable =
    | BlockReturnable
    | NotReturnable

let GetVarResult cenv var =
    let id =
        match cenv.localVariablesByVar.TryGetValue var with
        | true, resultId -> resultId
        | _ -> cenv.globalVariablesByVar.[var]
    if id = 0u then
        failwithf "Invalid id: %i" id
    id

let rec GenExpr cenv (env: env) returnable expr =
    match expr with
    | SpirvUnreachable ->
        addInstructions cenv [OpUnreachable]
        ZeroResultId

    | SpirvConst spvConst ->
        GenConst cenv spvConst

    | SpirvLet(spvVar, rhs, body) ->
        let spvVarId = GenLocalVar cenv spvVar
        let rhsId = GenExpr cenv env NotReturnable rhs |> deref cenv
        addInstructions cenv [OpStore(spvVarId, rhsId, None)]
        GenExpr cenv env returnable body

    | SpirvSequential(expr1, expr2) ->
        GenExpr cenv env NotReturnable expr1 |> ignore
        GenExpr cenv env returnable expr2

    | SpirvNewVector2 args
    | SpirvNewVector3 args 
    | SpirvNewVector4 args ->
        GenVector cenv env returnable expr.Type args

    | SpirvArrayIndexerGet (receiver, arg, _) ->
        let receiverId = GenExpr cenv env NotReturnable receiver
        let argId = GenExpr cenv env NotReturnable arg |> deref cenv

        let resultType = getAccessChainResultType cenv receiverId 0
        let resultId = nextResultId cenv
        let instr = OpAccessChain(resultType, resultId, receiverId, [argId])
        addInstructions cenv [instr]
        cenv.locals.[resultId] <- instr
        resultId

    | SpirvArrayIndexerSet (receiver, indexArg, valueArg) ->
        let receiverId = GenExpr cenv env NotReturnable receiver
        let argId = GenExpr cenv env NotReturnable indexArg |> deref cenv
        let valueArg = GenExpr cenv env NotReturnable valueArg |> deref cenv

        let resultType = getAccessChainResultType cenv receiverId 0
        let resultId = nextResultId cenv
        let instr = OpAccessChain(resultType, resultId, receiverId, [argId])
        addInstructions cenv [instr;OpStore(resultId, valueArg, None)]
        ZeroResultId

    | SpirvVar var ->
        GetVarResult cenv var

    | SpirvVarSet (var, rhs) ->
        if not var.IsMutable then
            failwithf "'%s' is not mutable." var.Name

        let rhsId = GenExpr cenv env NotReturnable rhs |> deref cenv
        let id = GetVarResult cenv var
        addInstructions cenv [OpStore(id, rhsId, None)]
        ZeroResultId

    | SpirvExprOp op ->
        let retTy = GenType cenv op.ReturnType
        match op with
        | SpirvNop -> ZeroResultId
        | SpirvOp(instr, args, _) ->
            let args =
                args 
                |> List.map (fun arg -> GenExpr cenv env NotReturnable arg |> deref cenv)

            let resultId =
                if op.ReturnType.IsVoid then
                    ZeroResultId
                else
                    nextResultId cenv
            addInstructions cenv [instr retTy resultId args]
            resultId

    | SpirvIntrinsicFieldGet fieldGet ->
        let getComponent receiver fieldTy n =
            let receiverId = GenExpr cenv env NotReturnable receiver |> deref cenv
            let fieldTyId = GenType cenv fieldTy
            addCompositeExtractInstruction cenv fieldTyId receiverId [n]
            
        match fieldGet with
        | Vector2_Get_X(receiver, fieldTy)
        | Vector2Int_Get_X(receiver, fieldTy) 
        | Vector3_Get_X(receiver, fieldTy)
        | Vector4_Get_X(receiver, fieldTy) ->
            getComponent receiver fieldTy 0u
        | Vector2_Get_Y(receiver, fieldTy) 
        | Vector2Int_Get_Y(receiver, fieldTy) 
        | Vector3_Get_Y(receiver, fieldTy)
        | Vector4_Get_Y(receiver, fieldTy) ->
            getComponent receiver fieldTy 1u
        | Vector3_Get_Z(receiver, fieldTy)
        | Vector4_Get_Z(receiver, fieldTy) ->
            getComponent receiver fieldTy 2u
        | Vector4_Get_W(receiver, fieldTy) ->
            getComponent receiver fieldTy 3u

    | SpirvFieldGet (receiver, index) ->
        let receiverId = GenExpr cenv env NotReturnable receiver

        let resultType = getAccessChainResultType cenv receiverId index

        match getTypePointerInstruction cenv resultType with
        | OpTypePointer _ -> ()
        | _ -> failwith "Invalid pointer."

        let indexId = GenExpr cenv env NotReturnable (SpirvConst (SpirvConstInt(index, [])))
        let accessChainPointerId = nextResultId cenv
        let instr = OpAccessChain(resultType, accessChainPointerId, receiverId, [indexId])
        addInstructions cenv [instr]
        cenv.locals.[accessChainPointerId] <- instr
        accessChainPointerId

    | SpirvIfThenElse(condExpr, trueExpr, falseExpr) ->
        GenIfThenElse cenv env returnable condExpr trueExpr falseExpr expr.Type

    | SpirvCallFunction(var, args) ->
        let funId = cenv.functionsByVar.[var]
        let argIds = 
            args 
            |> List.choose (fun arg ->
                let argId = GenExpr cenv env NotReturnable arg |> deref cenv
                if argId = ZeroResultId then None
                else Some argId)

        let retTy =
            match var.Type with
            | SpirvTypeFunction(_, retTy) -> retTy
            | _ -> failwith "Bad function type."

        let retTyId = GenType cenv retTy

        let resId = nextResultId cenv
        addInstructions cenv [OpFunctionCall(retTyId, resId, funId, argIds)]
        resId

and GenIfThenElse cenv env returnable condExpr trueExpr falseExpr retTy =
    let resultIdCond = GenExpr cenv env NotReturnable condExpr

    let contLabel = nextResultId cenv
    let trueLabel = nextResultId cenv
    let falseLabel = nextResultId cenv

    addInstructions cenv [OpSelectionMerge(contLabel, SelectionControl.None)]
    addInstructions cenv [OpBranchConditional(resultIdCond, trueLabel, falseLabel, [])]

    emitLabel cenv trueLabel
    cenv.lastLabel <- trueLabel
    let trueResult = GenExprBlockContinue cenv env trueLabel contLabel returnable trueExpr
    let trueLabel = cenv.lastLabel
    emitLabel cenv falseLabel
    cenv.lastLabel <- falseLabel
    let falseResult = GenExprBlockContinue cenv env falseLabel contLabel returnable falseExpr
    let falseLabel = cenv.lastLabel

    emitLabel cenv contLabel
    cenv.lastLabel <- contLabel

    if retTy = SpirvTypeVoid then
        checkZeroResultId trueResult
        checkZeroResultId falseResult

        ZeroResultId
    else
        checkValidResultId trueResult
        checkValidResultId falseResult

        let resultType = GenType cenv retTy
        let resultId = nextResultId cenv
        let operands = [PairIdRefIdRef(trueResult, trueLabel);PairIdRefIdRef(falseResult, falseLabel)]
        addInstructions cenv [OpPhi(resultType, resultId, operands)]
        resultId

and GenExprBlockAux cenv env blockLabel blockLabelContinue returnable expr =
    let resultId = GenExpr cenv { env with blockScope = env.blockScope + 1; blockLabel = blockLabel } BlockReturnable expr |> deref cenv

    let resultId =
        if blockLabelContinue > 0u then
            addInstructions cenv [OpBranch blockLabelContinue]
            resultId
        else
            if returnable = BlockReturnable then
                if resultId = ZeroResultId then
                    addInstructions cenv [OpReturn]
                else
                    addInstructions cenv [OpReturnValue resultId]
                resultId
            else
                resultId

    cenv.ignoreAddingInstructions <- false
    resultId

and GenExprBlock cenv env blockLabel returnable expr =
    GenExprBlockAux cenv env blockLabel 0u returnable expr

and GenExprBlockContinue cenv env blockLabel blockLabelContinue returnable expr =
    GenExprBlockAux cenv env blockLabel blockLabelContinue returnable expr

and GenFunction cenv env isEntryPoint (args: SpirvVar list) (body: SpirvExpr) =
    let argTys = args |> List.map (fun x -> x.Type)
    let retTy = body.Type

    let functionResultId = nextResultId cenv
    addInstructions cenv 
        [ OpFunction(GenType cenv retTy, functionResultId, FunctionControl.None, 
                     GenTypeFunction cenv argTys retTy) ]

    args
    |> List.iter (fun arg ->
        if not arg.Type.IsVoid then
            GenFunctionParameterVar cenv arg |> ignore)

    let blockLabel = nextResultId cenv
    let cenvBody = { cenv with currentInstructions = ResizeArray () }
    GenExprBlock cenvBody env blockLabel BlockReturnable body |> ignore

    emitLabel cenv blockLabel
    addInstructions cenv cenv.localVariables.Values
    if isEntryPoint then
        addInstructions cenv cenv.entryPointInstructions

    cenv.currentInstructions.AddRange cenvBody.currentInstructions

    addInstructions cenv [OpFunctionEnd]

    cenv.locals.Clear()
    cenv.localVariables.Clear()
    cenv.localVariablesByVar.Clear()

    functionResultId

and GenVector cenv env returnable retTy args =
    let constituents =
        args
        |> List.map (fun arg ->
            let id = GenExpr cenv env returnable arg |> deref cenv
            match tryAddCompositeExtractInstructions cenv arg.Type id with
            | [] -> [id]
            | ids -> ids
        )
        |> List.concat

    let resultId = nextResultId cenv
    addInstructions cenv [OpCompositeConstruct(GenType cenv retTy, resultId, constituents)]
    resultId

let GenDecl cenv decl =
    match decl with
    | SpirvDeclConst (var, constt) ->
        // Review: Do we need to do all this work for scalar constants?
        let varId = GenGlobalVar cenv var
        let constId = GenConst cenv constt
        addEntryPointInstructions cenv [OpStore(varId, constId, None)]
        varId

    | SpirvDeclVar var ->
        GenGlobalVar cenv var

let rec GenTopLevelExpr cenv env isEntryPoint lambdaArgs expr =
    match expr with
    | SpirvTopLevelSequential (expr1, expr2) ->
        GenTopLevelExpr cenv env false [] expr1 |> ignore
        GenTopLevelExpr cenv env isEntryPoint [] expr2

    | SpirvTopLevelDecl decl ->
        GenDecl cenv decl

    | SpirvTopLevelLet(var, rhs, body) ->
        let idRes = GenTopLevelExpr cenv env false [] rhs
        StoreFunctionVar cenv var idRes
        GenTopLevelExpr cenv env isEntryPoint [] body

    | SpirvTopLevelLambda (var, body) ->
        GenTopLevelExpr cenv env isEntryPoint (lambdaArgs @ [var]) body

    | SpirvTopLevelLambdaBody (var, body) ->
        let lambdaArgs = lambdaArgs @ [var]
        GenFunction cenv env isEntryPoint lambdaArgs body

let GenModule (info: SpirvGenInfo) expr =
    let cenv = cenv.Default

    let entryPoint = GenTopLevelExpr cenv { blockScope = 0; blockLabel = 0u } true [] expr

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
            | OpVariable (_, _, StorageClass.Image, _)
            | OpVariable (_, _, StorageClass.StorageBuffer, _) ->
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

    let VK_MAKE_VERSION(major: uint32, minor: uint32) = ((major <<< 16) ||| (minor <<< 8))

    let version = VK_MAKE_VERSION(1u, 3u)

    SpirvModule.Create(version = version, instrs = instrs)