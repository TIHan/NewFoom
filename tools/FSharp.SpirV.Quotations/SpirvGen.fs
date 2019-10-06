[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.SpirvGen

open System
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Spirv
open Specification
open Tast

[<NoEquality;NoComparison>]
type cenv =
    {
        mutable nextResultId: Result_id

        // Types, Variables, Constants

        types: Dictionary<Result_id, SpirvInstruction>
        typesByType: Dictionary<SpirvType, Result_id>
        typeFunctions: Dictionary<id list, Result_id * SpirvInstruction list>
        typePointers: Dictionary<Result_id, SpirvInstruction>
        typePointersByResultType: Dictionary<StorageClass * id, Result_id>
        globalVariables: Dictionary<Result_id, SpirvInstruction * BuiltIn option>
        globalVariablesByVar: Dictionary<SpirvVar, Result_id>
        constants: Dictionary<Literal, Result_id * SpirvInstruction>
        constantComposites: Dictionary<id list, Result_id * SpirvInstruction>

        // Functions

        functions: Dictionary<string, Result_id * SpirvInstruction list>
        mainInitInstructions: ResizeArray<SpirvInstruction>

        // Local

        localVariables: Dictionary<Result_id, SpirvInstruction>
        localVariablesByVar: Dictionary<SpirvVar, Result_id>
        currentInstructions: ResizeArray<SpirvInstruction>
        locals: Dictionary<Result_id, SpirvInstruction>

        // Debug Info

        debugNames: ResizeArray<(string * id)>
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

let addMainInitInstructions cenv instrs =
    cenv.mainInitInstructions.AddRange instrs

let getTypeInstruction cenv id =
    cenv.types.[id]

let getTypeByTypeInstruction cenv ty : id =
    cenv.typesByType.[ty]

let getTypePointerInstruction cenv id =
    cenv.typePointers.[id]
    
let isAggregateType ty =
    match ty with
    | SpirvTypeArray _ -> true
    | _ -> false

let isCompositeType ty =
    match ty with
    | SpirvTypeVector2
    | SpirvTypeVector3
    | SpirvTypeVector4 -> true
    | _ -> false

let tryAddCompositeExtractInstructions cenv ty composite =
    if isCompositeType ty then
        let resultType = getTypeByTypeInstruction cenv ty
        match getTypeInstruction cenv resultType with
        | OpTypeVector(_, componentType, [componentCount]) ->
            [ for i = 0 to int componentCount - 1 do
                let resultId = nextResultId cenv
                addInstructions cenv [OpCompositeExtract(componentType, resultId, composite, [uint32 i])]
                yield resultId ]
        | _ ->
            failwith "Unexpected instruction"
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
        resultId

let emitTypeVoid cenv =
    emitTypeAux cenv SpirvTypeVoid (fun resultId -> OpTypeVoid resultId)

let emitTypeSingle cenv =
    emitTypeAux cenv SpirvTypeSingle (fun resultId -> OpTypeFloat(resultId, [32u]))

let emitTypeInt cenv =
    emitTypeAux cenv SpirvTypeInt (fun resultId -> OpTypeInt(resultId, 32u, [1u]))

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
        let resultId = nextResultId cenv
        cenv.constants.[literal] <- (resultId, OpConstant(resultType, resultId, literal))
        cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
        resultId

let emitConstantInt cenv (n: int) =
    emitConstantAux cenv (emitTypeInt cenv) "int" [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

let emitConstantUInt32 cenv (n: uint32) =
    emitConstantAux cenv (emitTypeInt cenv) "uint32" [n]

let emitConstantSingle cenv (n: single) =
    emitConstantAux cenv (emitTypeSingle cenv) "single" [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

let emitConstantComposite cenv resultType debugName constituents =
    match cenv.constantComposites.TryGetValue constituents with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.constantComposites.[constituents] <- (resultId, OpConstantComposite(resultType, resultId, constituents))
        cenv.debugNames.Add(string resultId + "_const_" + debugName, resultId)
        resultId

let emitTypeVector2 cenv =
    let componentType = emitTypeSingle cenv
    emitTypeAux cenv SpirvTypeVector2 (fun resultId -> OpTypeVector(resultId, componentType, [2u]))

let emitTypeVector3 cenv =
    let componentType = emitTypeSingle cenv
    emitTypeAux cenv SpirvTypeVector3 (fun resultId -> OpTypeVector(resultId, componentType, [3u]))

let emitTypeVector4 cenv =
    let componentType = emitTypeSingle cenv
    emitTypeAux cenv SpirvTypeVector4 (fun resultId -> OpTypeVector(resultId, componentType, [4u]))

let emitConstantVector2 cenv (constituents: id list) =
    emitConstantComposite cenv (emitTypeVector2 cenv) "Vector2" constituents

let emitConstantVector3 cenv (constituents: id list) =
    emitConstantComposite cenv (emitTypeVector3 cenv) "Vector3" constituents

let emitConstantVector4 cenv (constituents: id list) =
    emitConstantComposite cenv (emitTypeVector4 cenv) "Vector4" constituents

let emitTypeMatrix4x4 cenv =
    let columnType = emitTypeVector4 cenv
    emitTypeAux cenv SpirvTypeMatrix4x4 (fun resultId -> OpTypeMatrix(resultId, columnType, [4u]))

let emitConstantMatrix4x4 cenv (constituents: id list) =
    emitConstantComposite cenv (emitTypeMatrix4x4 cenv) "Matrix4x4" constituents

let rec emitType cenv ty =
    match ty with
    | SpirvTypeVoid -> emitTypeVoid cenv
    | SpirvTypeInt -> emitTypeInt cenv
    | SpirvTypeSingle -> emitTypeSingle cenv
    | SpirvTypeVector2 -> emitTypeVector2 cenv
    | SpirvTypeVector3 -> emitTypeVector3 cenv
    | SpirvTypeVector4 -> emitTypeVector4 cenv
    | SpirvTypeMatrix4x4 -> emitTypeMatrix4x4 cenv
    | SpirvTypeArray (elementTy, length) -> emitArrayType cenv elementTy length

and emitArrayType cenv elementTy length =
    match elementTy with
    | SpirvTypeVoid -> failwithf "Element type, %A, is not valid." elementTy
    | _ ->
        let elementTyId = emitType cenv elementTy
        let lengthId = uint32 length |> emitConstantUInt32 cenv
        emitTypeAux cenv (SpirvTypeArray (elementTy, length)) (fun arrayTyId -> OpTypeArray(arrayTyId, elementTyId, lengthId))

let emitTypeFunction cenv paramTys retTy =
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

let rec isConstant expr =
    match expr with
    | SpirvConst _ -> true
    | _ -> false

let rec GenConst cenv spvConst =
    match spvConst with
    | SpirvConstInt (n, decorations) ->
        emitConstantInt cenv n

    | SpirvConstSingle (n, docorations) ->
        emitConstantSingle cenv n

    | SpirvConstVector2 (n1, n2, decorations) ->
        emitConstantVector2 cenv ([n1;n2] |> List.map (emitConstantSingle cenv))

    | SpirvConstVector3 (n1, n2, n3, decorations) ->
        emitConstantVector3 cenv ([n1;n2;n3] |> List.map (emitConstantSingle cenv))

    | SpirvConstVector4 (n1, n2, n3, n4, decorations) ->
        emitConstantVector4 cenv ([n1;n2;n3;n4] |> List.map (emitConstantSingle cenv))

    | SpirvConstMatrix4x4 (m11, m12, m13, m14,
                           m21, m22, m23, m24,
                           m31, m32, m33, m34,
                           m41, m42, m43, m44, decorations) ->
        let constituents =
            [ m11; m12; m13; m14;
              m21; m22; m23; m24;
              m31; m32; m33; m34;
              m41; m42; m43; m44 ]
            |> List.map (emitConstantSingle cenv)
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

        let builtInOpt =
            match spvVar.Decorations with
            | [(Decoration.BuiltIn, [builtInValue])] -> 
                Some (LanguagePrimitives.EnumOfValue (int builtInValue))
            | _ -> 
                None

        let resultType = emitType cenv spvVar.Type |> emitPointer cenv storageClass
        let resultId = nextResultId cenv
        cenv.globalVariables.[resultId] <- (OpVariable(resultType, resultId, storageClass, None), builtInOpt)
        cenv.globalVariablesByVar.[spvVar] <- resultId
        cenv.debugNames.Add(string resultId + "_" + spvVar.Name, resultId)
        resultId

let getAccessChainResultType cenv pointer =
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
        | true, OpTypeArray (_, elementType, _) -> elementType
        | _ -> baseType
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
        let rhsId = GenExpr cenv env rhs
        addInstructions cenv [OpStore(spvVarId, rhsId, None)]
        GenExpr cenv env body

    | SpirvSequential(expr1, expr2) ->
        GenExpr cenv env expr1 |> ignore
        GenExpr cenv env expr2

    | SpirvNewVector2 args
    | SpirvNewVector3 args 
    | SpirvNewVector4 args ->
        GenVector cenv env expr.Type args

    | SpirvArrayIndexerGet (receiver, arg) ->
        let receiverId = GenExpr cenv env receiver
        let argId = GenExpr cenv env arg

        let resultType = getAccessChainResultType cenv receiverId
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

        let rhsId = GenExpr cenv env rhs
        let pointer = 
            match tryEmitLoad cenv rhsId with
            | Some pointer -> pointer
            | _ -> rhsId
        let id = GenGlobalVar cenv var
        addInstructions cenv [OpStore(id, pointer, None)]
        id

    | SpirvIntrinsicCall call ->
        let retTy = emitType cenv call.ReturnType
        match call with
        | Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2) ->
            let arg1 = GenExpr cenv env arg1
            let arg2 = GenExpr cenv env arg2

            let resultId = nextResultId cenv
            addInstructions cenv [OpMatrixTimesVector(retTy, resultId, arg1, arg2)]
            resultId

        | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) ->
            let arg1 = GenExpr cenv env arg1
            let arg2 = GenExpr cenv env arg2

            let resultId = nextResultId cenv
            addInstructions cenv [OpMatrixTimesMatrix(retTy, resultId, arg1, arg2)]
            resultId

and GenVector cenv env retTy args =
    let constituents =
        args
        |> List.map (fun arg ->
            let id = GenExpr cenv env arg
            let id =
                match tryEmitLoad cenv id with
                | Some id -> id
                | _ -> id

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
        GenGlobalVar cenv var |> ignore
        GenTopLevelExpr cenv env body

    | SpirvTopLevelLambdaBody (var, body) ->
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
        emitTypeVoid cenv, env.entryPoint, FunctionControlMask.MaskNone, 
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
        let mutable input = 0u
        let mutable output = 0u
        let annoations = ResizeArray ()
        cenv.globalVariables
        |> Seq.iter (fun pair ->
            let resultId = pair.Key
            let instr, builtInOpt = pair.Value

            match builtInOpt with
            | Some builtIn ->
                OpDecorate (resultId, Decoration.BuiltIn, [uint32 builtIn])
                |> annoations.Add
            | _ -> ()

            match instr with
            | OpVariable (_, _, StorageClass.Input, _) ->
                if builtInOpt.IsNone then
                    OpDecorate (resultId, Decoration.Location, [input])
                    |> annoations.Add
                    input <- input + 1u

            | OpVariable (_, _, StorageClass.Output, _) ->
                if builtInOpt.IsNone then
                    OpDecorate (resultId, Decoration.Location, [output])
                    |> annoations.Add
                    output <- output + 1u

            | OpVariable (_, _, StorageClass.Private, _) -> ()

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
        cenv.globalVariables
        |> Seq.choose (fun pair ->
            let resultId = pair.Key
            let instr = fst pair.Value
            match instr with
            | OpVariable (_, _, StorageClass.Input, _)
            | OpVariable (_, _, StorageClass.Output, _) -> Some resultId
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
         |> Option.map (fun (executionMode, literalNumber) -> OpExecutionMode (entryPoint, executionMode, literalNumber))
         |> Option.toList)
        @
        (cenv.debugNames
         |> Seq.map (fun (name, id) -> OpName (id, name))
         |> List.ofSeq)
        @
        annotations @ typesAndConstants @ variables
        @
        (cenv.currentInstructions |> List.ofSeq)

    SpirvModule.Create(instrs = instrs)