[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.Spirv

open System
open System.Numerics
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Spirv
open FSharp.Spirv.Specification

[<NoEquality;NoComparison>]
type cenv =
    {
        mutable nextResultId: Result_id

        // Types, Variables, Constants

        types: Dictionary<Result_id, SpirvInstruction>
        typesByType: Dictionary<Type, Result_id>
        typeFunctions: Dictionary<id list, Result_id * SpirvInstruction list>
        typePointers: Dictionary<Result_id, SpirvInstruction>
        typePointersByResultType: Dictionary<StorageClass * id, Result_id>
        globalVariables: Dictionary<Result_id, SpirvInstruction * BuiltIn option>
        globalVariablesByVar: Dictionary<Var, Result_id>
        constants: Dictionary<Literal, Result_id * SpirvInstruction>
        constantComposites: Dictionary<id list, Result_id * SpirvInstruction>

        // Functions

        functions: Dictionary<string, Result_id * SpirvInstruction list>
        mainInitInstructions: ResizeArray<SpirvInstruction>

        // Local

        localVariables: Dictionary<Result_id, SpirvInstruction>
        localVariablesByVar: Dictionary<Var, Result_id>
        currentInstructions: ResizeArray<SpirvInstruction>
        locals: Dictionary<Result_id, SpirvInstruction>
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

let emitConstantUInt32 cenv (n: uint32) =
    emitConstantAux cenv (emitTypeInt cenv) [n]

let emitConstantSingle cenv (n: single) =
    emitConstantAux cenv (emitTypeSingle cenv) [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

let emitConstantComposite cenv resultType constituents =
    match cenv.constantComposites.TryGetValue constituents with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.constantComposites.[constituents] <- (resultId, OpConstantComposite(resultType, resultId, constituents))
        resultId

let emitTypeVector2 cenv =
    let componentType = emitTypeSingle cenv
    emitTypeAux cenv typeof<Vector2> (fun resultId -> OpTypeVector(resultId, componentType, [2u]))

let emitTypeVector3 cenv =
    let componentType = emitTypeSingle cenv
    emitTypeAux cenv typeof<Vector3> (fun resultId -> OpTypeVector(resultId, componentType, [3u]))

let emitTypeVector4 cenv =
    let componentType = emitTypeSingle cenv
    emitTypeAux cenv typeof<Vector4> (fun resultId -> OpTypeVector(resultId, componentType, [4u]))

let emitConstantVector2 cenv constituents =
    emitConstantComposite cenv (emitTypeVector2 cenv) constituents

let emitConstantVector3 cenv constituents =
    emitConstantComposite cenv (emitTypeVector3 cenv) constituents

let emitConstantVector4 cenv constituents =
    emitConstantComposite cenv (emitTypeVector4 cenv) constituents

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
        let elementType = emitType cenv elementTy
        let arrayTy = elementTy.MakeArrayType()
        let lengthId = uint32 length |> emitConstantUInt32 cenv
        emitTypeAux cenv arrayTy (fun resultId -> OpTypeArray(resultId, elementType, lengthId))

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
    cenv.localVariables.[resultId] <- OpVariable(resultType, resultId, StorageClass.Function, None)
    cenv.localVariablesByVar.[var] <- resultId
    resultId

let emitGlobalVariableAux cenv storageClass (var: Var) =
    match cenv.globalVariablesByVar.TryGetValue var with
    | true, resultId -> resultId
    | _ ->
        let builtInOpt =
            if var.Name.StartsWith("gl_") then
                let mutable builtIn = Unchecked.defaultof<BuiltIn>
                match storageClass, BuiltIn.TryParse(var.Name.Replace("gl_", ""), &builtIn) with
                | StorageClass.Input, true -> Some builtIn
                | StorageClass.Output, true -> Some builtIn
                | _, true -> failwithf "BuiltIn not valid with StorageClass: %A" storageClass
                | _ -> None
            else
                None

        let resultType = emitType cenv var.Type |> emitPointer cenv storageClass
        let resultId = nextResultId cenv
        cenv.globalVariables.[resultId] <- (OpVariable(resultType, resultId, storageClass, None), builtInOpt)
        cenv.globalVariablesByVar.[var] <- resultId
        resultId

let emitGlobalVariable cenv var =
    emitGlobalVariableAux cenv StorageClass.Private var

let emitGlobalInputVariable cenv var =
    emitGlobalVariableAux cenv StorageClass.Input var

let emitGlobalOutputVariable cenv var =
    emitGlobalVariableAux cenv StorageClass.Output var

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

    | Int32 n ->
        GenInt32 cenv n

    | Single n ->
        GenSingle cenv n

    | NewRecord(ty, args) when env.isReturnable ->
        GenNewRecord cenv env ty args

    | Var var when env.inMain ->
        match cenv.localVariablesByVar.TryGetValue var with
        | true, resultId -> [resultId]
        | _ -> [cenv.globalVariablesByVar.[var]]

    | Sequential(expr1, expr2) when env.inMain ->
        GenExpr cenv env expr1 |> ignore
        GenExpr cenv env expr2

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
                let id =
                    match cenv.globalVariables.TryGetValue id with
                    | true, _ ->
                        emitLoad cenv id
                    | _ ->
                        match cenv.locals.TryGetValue id with
                        | true, _ -> emitLoad cenv id
                        | _ -> id
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

    let gens =
        args
        |> List.map (fun x -> (fun () -> GenExpr cenv env x))

    let vars =
        ty.GetProperties()
        |> Seq.map (fun prop ->
            Var(prop.Name, prop.PropertyType, false)
        )

    (gens, vars)
    ||> Seq.iter2 (fun gen var ->
        match gen () with
        | [pointer] ->
            let pointer = 
                match tryEmitLoad cenv pointer with
                | Some pointer -> pointer
                | _ -> pointer
            let id = emitGlobalOutputVariable cenv var
            addInstructions cenv [OpStore(id, pointer, None)]
        | _ -> failwith "Invalid instruction."
    )

    []

and GenCall cenv env methInfo args =
    let env = { env with isReturnable = false }

    match methInfo.DeclaringType.FullName with
    | "Microsoft.FSharp.Core.LanguagePrimitives+IntrinsicFunctions" ->

        match methInfo.Name, args with
        | "GetArray", [receiverExpr;indexExpr] ->
            match GenExpr cenv env receiverExpr, GenExpr cenv env indexExpr with
            | [pointer], [indexPointer] ->
                let resultType = getAccessChainResultType cenv pointer
                let index = emitLoad cenv indexPointer
                let resultId = nextResultId cenv
                let op = OpAccessChain(resultType, resultId, pointer, [index])
                addInstructions cenv [op]
                cenv.locals.[resultId] <- op
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
    let localVarInstrs = ResizeArray (cenv.localVariables.Values)

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

let GenModule (info: SpirvGenInfo) expr =
    let cenv = cenv.Default

    let entryPoint = nextResultId cenv

    GenExpr cenv { entryPoint = entryPoint; inMain = false; isReturnable = false } expr |> ignore

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
        annotations @ typesAndConstants @ variables
        @
        (cenv.currentInstructions |> List.ofSeq)

    SpirvModule.Create(instrs = instrs)
