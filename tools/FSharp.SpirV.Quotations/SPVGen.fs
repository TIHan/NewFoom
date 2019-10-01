[<RequireQualifiedAccess>]
module FSharp.SpirV.Quotations.SPVGen

open System
open System.Numerics
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.SpirV
open FSharp.SpirV.Specification

[<NoEquality;NoComparison>]
type cenv =
    {
        mutable nextResultId: Result_id

        // Types, Variables, Constants

        types: Dictionary<Result_id, SPVInstruction>
        typesByType: Dictionary<Type, Result_id>
        typeFunctions: Dictionary<id list, Result_id * SPVInstruction list>
        typePointers: Dictionary<Result_id, SPVInstruction>
        typePointersByResultType: Dictionary<StorageClass * id, Result_id>
        globalVariables: Dictionary<Var, Result_id * SPVInstruction>
        constants: Dictionary<Literal, Result_id * SPVInstruction>
        constantComposites: Dictionary<id list, Result_id * SPVInstruction>

        loadPointers: Dictionary<id, id>

        // Functions

        functions: Dictionary<string, Result_id * SPVInstruction list>
        mainInitInstructions: ResizeArray<SPVInstruction>

        // Local

        localVariables: Dictionary<Var, Result_id * SPVInstruction>
        currentInstructions: ResizeArray<SPVInstruction>
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
            constants = Dictionary ()
            constantComposites = Dictionary ()
            loadPointers = Dictionary ()
            functions = Dictionary ()
            mainInitInstructions = ResizeArray 100
            localVariables = Dictionary ()
            currentInstructions = ResizeArray 100
        }

[<NoEquality;NoComparison>]
type env =
    {
        entryPoint: uint32

        // TODO: Make flags for these instead of bools.
        inMain: bool
        isReturnable: bool
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

let isCompositeType ty =
    match ty with
    | _ when ty = typeof<Vector2> -> true
    | _ when ty = typeof<Vector3> -> true
    | _ when ty = typeof<Vector4> -> true
    | _ when ty = typeof<Matrix3x2> -> true
    | _ when ty = typeof<Matrix4x4> -> true
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
        resultId

let emitTypeUnit cenv =
    emitTypeAux cenv typeof<unit> (fun resultId -> OpTypeVoid resultId)

let emitTypeSingle cenv =
    emitTypeAux cenv typeof<single> (fun resultId -> OpTypeFloat(resultId, [32u]))

let emitTypeInt cenv =
    emitTypeAux cenv typeof<int> (fun resultId -> OpTypeInt(resultId, 32u, [1u]))

let emitTypeVector2 cenv =
    emitTypeAux cenv typeof<Vector2> (fun resultId -> OpTypeVector(resultId, emitTypeSingle cenv, [2u]))

let emitTypeVector3 cenv =
    emitTypeAux cenv typeof<Vector3> (fun resultId -> OpTypeVector(resultId, emitTypeSingle cenv, [3u]))

let emitTypeVector4 cenv =
    emitTypeAux cenv typeof<Vector4> (fun resultId -> OpTypeVector(resultId, emitTypeSingle cenv, [4u]))

let emitType cenv ty =
    match ty with
    | _ when ty = typeof<unit> -> emitTypeUnit cenv
    | _ when ty = typeof<single> -> emitTypeSingle cenv
    | _ when ty = typeof<int> -> emitTypeInt cenv
    | _ when ty = typeof<Vector2> -> emitTypeVector2 cenv
    | _ when ty = typeof<Vector3> -> emitTypeVector3 cenv
    | _ when ty = typeof<Vector4> -> emitTypeVector4 cenv
    | _ ->
        emitTypeAux cenv ty (fun _ -> failwithf "Type not supported: %A" ty)

let emitArrayType cenv elementTy length =
    match elementTy with
    | _ when elementTy = typeof<unit> -> failwith "Element type, 'unit', is not valid."
    | _ ->
        let sizeOfType = System.Runtime.InteropServices.Marshal.SizeOf(elementTy)
        let elementType = emitType cenv elementTy
        let arrayTy = elementTy.MakeArrayType()
        emitTypeAux cenv arrayTy (fun resultId -> OpTypeArray(resultId, elementType, uint32 (sizeOfType * length)))

let emitTypeFunction cenv paramTys retTy =
    let paramTyIds =
        paramTys
        |> List.filter (fun x -> x <> typeof<unit>)
        |> List.map (emitType cenv)
    let retTyId = emitType cenv retTy

    let tys = paramTyIds @ [retTyId]

    match cenv.typeFunctions.TryGetValue tys with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typeFunctions.[tys] <- (resultId, [OpTypeFunction(resultId, retTyId, paramTyIds)])
        resultId

let emitPointer cenv storageClass typeId =    
    match cenv.typePointersByResultType.TryGetValue ((storageClass, typeId)) with
    | true, resultId -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typePointersByResultType.[(storageClass, typeId)] <- resultId
        cenv.typePointers.[resultId] <- OpTypePointer(resultId, storageClass, typeId)
        resultId

let emitConstantAux cenv resultType literal =
    match cenv.constants.TryGetValue literal with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.constants.[literal] <- (resultId, OpConstant(resultType, resultId, literal))
        resultId

let emitConstantInt cenv (n: int) =
    emitConstantAux cenv (emitTypeInt cenv) [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

let emitConstantSingle cenv (n: single) =
    emitConstantAux cenv (emitTypeSingle cenv) [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

let emitConstantComposite cenv resultType constituents =
    match cenv.constantComposites.TryGetValue constituents with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.constantComposites.[constituents] <- (resultId, OpConstantComposite(resultType, resultId, constituents))
        resultId

let emitConstantVector2 cenv constituents =
    emitConstantComposite cenv (emitTypeVector2 cenv) constituents

let emitConstantVector3 cenv constituents =
    emitConstantComposite cenv (emitTypeVector3 cenv) constituents

let emitConstantVector4 cenv constituents =
    emitConstantComposite cenv (emitTypeVector4 cenv) constituents

let rec isConstant expr =
    match expr with
    | Int32 _
    | Single _ -> true
    | NewObject (ctorInfo, args) when ctorInfo.DeclaringType = typeof<Vector2> && args |> List.forall isConstant -> true
    | NewObject (ctorInfo, args) when ctorInfo.DeclaringType = typeof<Vector3> && args |> List.forall isConstant -> true
    | NewObject (ctorInfo, args) when ctorInfo.DeclaringType = typeof<Vector4> && args |> List.forall isConstant -> true
    | _ -> false

let rec tryEmitConstant cenv expr =
    match expr with
    | Int32 n -> 
        emitConstantInt cenv n |> ValueSome
    | Single n -> 
        emitConstantSingle cenv n |> ValueSome
    | NewObject (ctorInfo, args) when ctorInfo.DeclaringType = typeof<Vector2> && args |> List.forall isConstant ->
        emitConstantVector2 cenv (args |> List.map (fun x -> (tryEmitConstant cenv x).Value)) |> ValueSome
    | NewObject (ctorInfo, args) when ctorInfo.DeclaringType = typeof<Vector3> && args |> List.forall isConstant ->
        emitConstantVector3 cenv (args |> List.map (fun x -> (tryEmitConstant cenv x).Value)) |> ValueSome
    | NewObject (ctorInfo, args) when ctorInfo.DeclaringType = typeof<Vector4> && args |> List.forall isConstant ->
        emitConstantVector4 cenv (args |> List.map (fun x -> (tryEmitConstant cenv x).Value)) |> ValueSome
    | _ ->
        ValueNone

let emitConstant cenv expr =
    match tryEmitConstant cenv expr with
    | ValueSome resultType -> resultType
    | _ -> failwithf "Expression is not a constant: %A" expr

let emitConstants cenv (exprs: Expr list) =
    exprs
    |> List.map (emitConstant cenv)

let emitLocalVariable cenv (var: Var) =
    let resultType = emitType cenv var.Type |> emitPointer cenv StorageClass.Function
    let resultId = nextResultId cenv
    cenv.localVariables.[var] <- (resultId, OpVariable(resultType, resultId, StorageClass.Function, None))
    resultId

let emitGlobalVariableAux cenv storageClass (var: Var) =
    match cenv.globalVariables.TryGetValue var with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultType = emitType cenv var.Type |> emitPointer cenv storageClass
        let resultId = nextResultId cenv
        cenv.globalVariables.[var] <- (resultId, OpVariable(resultType, resultId, storageClass, None))
        resultId

let emitGlobalVariable cenv var =
    emitGlobalVariableAux cenv StorageClass.Private var

let emitGlobalInputVariable cenv var =
    emitGlobalVariableAux cenv StorageClass.Input var

let emitGlobalOutputVariable cenv var =
    emitGlobalVariableAux cenv StorageClass.Output var

let rec GenExpr cenv (env: env) expr =
    match expr with

    | Int32 n ->
        GenInt32 cenv n

    | Single n ->
        GenSingle cenv n

    | NewRecord(ty, args) when env.isReturnable ->
        GenNewRecord cenv env ty args

    | Var var when env.inMain ->
        let pointer, storageClass = 
            match cenv.localVariables.TryGetValue var with
            | true, (resultId, _) -> resultId, StorageClass.Function
            | _ ->
                match cenv.globalVariables.TryGetValue var with
                | true, (resultId, OpVariable(_, _, storageClass, _)) -> resultId, storageClass
                | _ ->  failwithf "Unable to find variable: %A" var

        let resultType = emitType cenv var.Type
        let resultId = nextResultId cenv
        addInstructions cenv [OpLoad(resultType, resultId, pointer, None)]
        cenv.loadPointers.[resultId] <- emitPointer cenv storageClass resultType
        [resultId]

    | Call (None, methInfo, args) when env.inMain ->
        GenCall cenv env methInfo args

    | NewObject (ctorInfo, args) when env.inMain ->
        GenNewObject cenv env ctorInfo args

    | Lambda(_, Lambda _) when not env.inMain ->
        match expr with
        | Lambda(var, expr) ->
            emitGlobalInputVariable cenv var |> ignore
            GenExpr cenv env expr
        | _ ->
            failwith "Should not happen."

    | Lambda(var, expr) when not env.inMain ->
        emitGlobalInputVariable cenv var |> ignore
        GenMainLambda cenv { env with inMain = true } expr

    // Only supports constants
    | NewArray(ty, args) when not env.inMain ->
        GenNewArray cenv env ty args

    // TODO: Tail call
    | Let(var, body, expr2) ->
        GenLet cenv env var body expr2

    | _ ->
        failwithf "Expression not supported: %A" expr

and GenInt32 cenv n =
    [emitConstantInt cenv n]

and GenSingle cenv n =
    [emitConstantSingle cenv n]

and GenNewArray cenv env ty (args: Expr list) =
    if env.inMain then
        failwith "Array construction can only happen outside of functions."

    let resultType = 
        emitArrayType cenv ty args.Length
        |> emitPointer cenv StorageClass.Private
    
    [emitConstantComposite cenv resultType (emitConstants cenv args)]

and GenLet cenv env var body expr =
    match GenExpr cenv { env with isReturnable = false } body with
    | [bodyId] ->
        if env.inMain then
            let varId = emitLocalVariable cenv var
            addInstructions cenv [OpStore(varId, bodyId, None)]
        else
            let varId = emitGlobalVariable cenv var
            addMainInitInstructions cenv [OpStore(varId, bodyId, None)]

        GenExpr cenv env expr
    | _ ->
        failwith "Invalid let expression."

and GenNewObject cenv env ctorInfo args =
    let ty = ctorInfo.DeclaringType
    if ty <> typeof<Vector2> && ty <> typeof<Vector3> && ty <> typeof<Vector4> then
        failwithf "Type not supported for new object: %A" ty

    let env = { env with isReturnable = false }

    let constituents =
        args
        |> List.map (fun arg ->
            match GenExpr cenv env arg with
            | [id] ->
                match tryAddCompositeExtractInstructions cenv arg.Type id with
                | [] -> [id]
                | ids -> ids
            | _ ->
                failwith "Invalid expression."
        )
        |> List.concat

    let resultId = nextResultId cenv
    addInstructions cenv [OpCompositeConstruct(emitType cenv ty, resultId, constituents)]
    [resultId]

and GenNewRecord cenv env ty args =
    if not env.isReturnable then
        failwith "Record construction must only appear at the end of the main function to dictate output."

    let ids =
        args
        |> List.map (GenExpr cenv env)
        |> List.concat

    let vars =
        ty.GetProperties()
        |> Seq.map (fun prop ->
            Var(prop.Name, prop.PropertyType, false)
        )

    (ids, vars)
    ||> Seq.iter2 (fun id var ->
        let pointer = emitGlobalOutputVariable cenv var
        addInstructions cenv [OpStore(pointer, id, None)]
    )

    []

and GenCall cenv env methInfo args =
    let env = { env with isReturnable = false }

    match methInfo.DeclaringType.FullName with
    | "Microsoft.FSharp.Core.LanguagePrimitives+IntrinsicFunctions" ->

        match methInfo.Name, args with
        | "GetArray", [receiverExpr;indexExpr] ->
            match GenExpr cenv env receiverExpr, GenExpr cenv env indexExpr with
            | [receiverId], [indexId] ->
                let receiverType = cenv.loadPointers.[receiverId]
                let resultId = nextResultId cenv
                addInstructions cenv [OpAccessChain(receiverType, resultId, receiverId, [indexId])]
                [resultId]
            | _ ->
                failwith "Invalid arg expressions for GetArray."
        | _ ->
            failwithf "Method not supported: %A" methInfo

    | _ ->
        failwithf "Declaring type of method not supported: %A" methInfo.DeclaringType

and GenMainLambda cenv env expr =
    GenExpr cenv { env with isReturnable = true } expr |> ignore

    let instrs = ResizeArray cenv.currentInstructions
    let localVarInstrs = ResizeArray (cenv.localVariables.Values |> Seq.map (fun (_, instr) -> instr))

    cenv.currentInstructions.Clear()
    cenv.localVariables.Clear()

    let funInstrs = cenv.currentInstructions

    OpFunction(
        emitTypeUnit cenv, env.entryPoint, FunctionControlMask.MaskNone, 
        emitTypeFunction cenv [typeof<unit>] typeof<unit>)
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

let GenFragment expr =
    let cenv = cenv.Default

    let entryPoint = nextResultId cenv

    GenExpr cenv { entryPoint = entryPoint; inMain = false; isReturnable = false } expr |> ignore

    let annotations =
        let mutable input = 0u
        let mutable output = 0u
        let annoations = ResizeArray ()
        cenv.globalVariables.Values
        |> Seq.iter (fun (_, instr) ->
            match instr with
            | OpVariable (_, resultId, StorageClass.Input, _) ->
                OpDecorate (resultId, Decoration.Location, [input])
                |> annoations.Add
                input <- input + 1u

            | OpVariable (_, resultId, StorageClass.Output, _) ->
                OpDecorate (resultId, Decoration.Location, [output])
                |> annoations.Add
                output <- output + 1u

            | OpVariable (_, _, StorageClass.Private, _) -> ()

            | _ ->
                failwithf "Invalid instruction or global variable not supported: %A" instr
        )
        annoations
        |> List.ofSeq

    let types =
        (cenv.types.Values
         |> List.ofSeq)
        @
        (cenv.typePointers.Values
         |> List.ofSeq)
        @
        (cenv.typeFunctions.Values
         |> Seq.map (fun (_, instr) -> instr)
         |> Seq.concat
         |> List.ofSeq)

    let variables =
        cenv.globalVariables.Values
        |> Seq.map (fun (_, instr) -> instr)
        |> List.ofSeq

    let constants =
        cenv.constants.Values
        |> Seq.map (fun (_, instr) -> instr)
        |> List.ofSeq

    let constantComposites =
        cenv.constantComposites.Values
        |> Seq.map (fun (_, instr) -> instr)
        |> List.ofSeq

    let interfaces =
        cenv.globalVariables.Values
        |> Seq.map (fun (resultId, _) -> resultId)
        |> List.ofSeq

    let instrs = 
        [
            OpCapability Capability.Shader
            OpExtInstImport (nextResultId cenv, "GLSL.std.450")
            OpMemoryModel (AddressingModel.Logical, MemoryModel.GLSL450)
            OpEntryPoint (ExecutionModel.Fragment, entryPoint, "main", interfaces)
            OpExecutionMode (entryPoint, ExecutionMode.OriginUpperLeft, [])
        ]
        @
        annotations @ types @ variables @ constants @ constantComposites
        @
        (cenv.currentInstructions |> List.ofSeq)

    SPVModule.Create(instrs = instrs)
