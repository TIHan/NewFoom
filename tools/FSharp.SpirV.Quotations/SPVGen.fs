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

        //mutable 

        mutable nextResultId: Result_id

        // Types, Variables, Constants

        types: Dictionary<Result_id, SPVInstruction>
        typesByType: Dictionary<Type, Result_id>
        typeFunctions: Dictionary<id list, Result_id * SPVInstruction list>
        typePointers: Dictionary<Result_id, SPVInstruction>
        typePointersByResultType: Dictionary<id, Result_id>
        globalVariables: Dictionary<Var, Result_id * SPVInstruction>
        constants: Dictionary<Literal, Result_id * SPVInstruction list>
        // Functions

        functions: Dictionary<string, Result_id * SPVInstruction list>

        mutable currentInstructions: ResizeArray<SPVInstruction>
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
            functions = Dictionary ()
            currentInstructions = ResizeArray 100
        }

[<NoEquality;NoComparison>]
type env =
    {
        inApp: bool
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

let genTypeAux cenv ty f =
    match cenv.typesByType.TryGetValue ty with
    | true, resultType -> resultType
    | _ ->
        let resultId = nextResultId cenv
        cenv.typesByType.[ty] <- resultId
        cenv.types.[resultId] <- f resultId
        resultId

let genTypeUnit cenv =
    genTypeAux cenv typeof<unit> (fun resultId -> OpTypeVoid resultId)

let genTypeSingle cenv =
    genTypeAux cenv typeof<single> (fun resultId -> OpTypeFloat(resultId, [32u]))

let genTypeInt cenv =
    genTypeAux cenv typeof<int> (fun resultId -> OpTypeInt(resultId, 32u, [1u]))

let genTypeVector2 cenv =
    genTypeAux cenv typeof<Vector2> (fun resultId -> OpTypeVector(resultId, genTypeSingle cenv, [2u]))

let genTypeVector3 cenv =
    genTypeAux cenv typeof<Vector3> (fun resultId -> OpTypeVector(resultId, genTypeSingle cenv, [3u]))

let genTypeVector4 cenv =
    genTypeAux cenv typeof<Vector4> (fun resultId -> OpTypeVector(resultId, genTypeSingle cenv, [4u]))

let genType cenv ty =
    match ty with
    | _ when ty = typeof<unit> -> genTypeUnit cenv
    | _ when ty = typeof<single> -> genTypeSingle cenv
    | _ when ty = typeof<int> -> genTypeInt cenv
    | _ when ty = typeof<Vector2> -> genTypeVector2 cenv
    | _ when ty = typeof<Vector3> -> genTypeVector3 cenv
    | _ when ty = typeof<Vector4> -> genTypeVector4 cenv
    | _ ->
        failwithf "Type not supported: %A" ty

let genTypeFunction cenv paramTys retTy =
    let paramTyIds =
        paramTys
        |> List.filter (fun x -> x <> typeof<unit>)
        |> List.map (genType cenv)
    let retTyId = genType cenv retTy

    let tys = paramTyIds @ [retTyId]

    match cenv.typeFunctions.TryGetValue tys with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typeFunctions.[tys] <- (resultId, [OpTypeFunction(resultId, retTyId, paramTyIds)])
        resultId

let genTypeAsPointer cenv storageClass ty =
    let tyId = genType cenv ty
    
    match cenv.typePointersByResultType.TryGetValue tyId with
    | true, resultId -> resultId
    | _ ->
        let resultId = nextResultId cenv
        cenv.typePointersByResultType.[tyId] <- resultId
        cenv.typePointers.[resultId] <- OpTypePointer(resultId, storageClass, tyId)
        resultId

let genConstant cenv resultType literal =
    match cenv.constants.TryGetValue literal with
    | true, (resultId, _) -> resultId
    | _ ->
        let resultId = nextResultId cenv

        cenv.currentInstructions.Add (OpConstant(resultType, resultId, literal))
        resultId

let genConstantInt cenv (n: int) =
    genConstant cenv (genTypeInt cenv) [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

let genConstantSingle cenv (n: single) =
    genConstant cenv (genTypeSingle cenv) [BitConverter.ToUInt32(ReadOnlySpan(&&n |> NativePtr.toVoidPtr, 4))]

//let genFunction cenv typars paramTys retTy =

let Gen cenv (env: env) expr =
    let rec gen cenv env expr : id list =
        match expr with

        | Int32 n ->
            [genConstantInt cenv n]

        | Single n ->
            [genConstantSingle cenv n]

        | Var var ->
            match cenv.globalVariables.TryGetValue var with
            | true, (_, OpVariable(pointerResultType, pointer, _, _)) ->
                match getTypePointerInstruction cenv pointerResultType with
                | OpTypePointer(_, _, resultType) ->
                    let resultId = nextResultId cenv
                    addInstructions cenv [OpLoad(resultType, resultId, pointer, None)]
                    [resultId]
                | _ ->
                    failwith "Unexpected instruction."
            | _ ->
                failwithf "Global output variable, %s, does not exist." var.Name

        | Let(var, SpecificCall <@@ input<_> @@> (None, [typar], []), expr2) ->
            let resultId = nextResultId cenv
            let op = OpVariable (genTypeAsPointer cenv StorageClass.Input typar, resultId, StorageClass.Input, None)
            cenv.globalVariables.Add (var, (resultId, op))
            gen cenv env expr2

        | Let(var, SpecificCall <@@ output<_> @@> (None, [typar], []), expr2) ->
            let resultId = nextResultId cenv
            let op = OpVariable (genTypeAsPointer cenv StorageClass.Output typar, resultId, StorageClass.Output, None)
            cenv.globalVariables.Add (var, (resultId, op))
            gen cenv env expr2

        | SpecificCall <@@ (:=) @@> (None, _, [Var var; rightExpr]) ->
            match cenv.globalVariables.TryGetValue var with
            | true, (pointer, _) ->
                match gen cenv env rightExpr with
                | [object] ->
                    addInstructions cenv [OpStore (pointer, object, None)]
                    []
                | _ ->
                    failwith "Invalid expression."
            | _ ->
                failwithf "Global output variable, %s, does not exist." var.Name

        | NewObject (ctorInfo, args) ->
            if ctorInfo.DeclaringType <> typeof<Vector4> then
                failwithf "NewObject expression not supported: %A" expr

            let constituents =
                args
                |> List.map (fun arg ->
                    match gen cenv env arg with
                    | [id] ->
                        match tryAddCompositeExtractInstructions cenv arg.Type id with
                        | [] -> [id]
                        | ids -> ids
                    | _ ->
                        failwith "Invalid expression."
                )
                |> List.concat

            let resultId = nextResultId cenv
            addInstructions cenv [OpCompositeConstruct(genType cenv typeof<Vector4>, resultId, constituents)]
            [resultId]

        | _ ->
            failwithf "Expression not supported: %A" expr

    gen cenv env expr |> ignore

let GenFragment expr =
    let cenv = cenv.Default

    // Entry Point

    let entryPoint = nextResultId cenv

    OpFunction(
        genTypeUnit cenv, entryPoint, FunctionControlMask.MaskNone, 
        genTypeFunction cenv [typeof<unit>] typeof<unit>)
    |> cenv.currentInstructions.Add

    OpLabel(nextResultId cenv)
    |> cenv.currentInstructions.Add

    Gen cenv { inApp = true } expr

    cenv.currentInstructions.Add OpFunctionEnd

    // Entry Point End

    let interfaces =
        cenv.globalVariables.Values
        |> Seq.map (fun (resultId, _) -> resultId)
        |> List.ofSeq

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

            | _ ->
                failwithf "Invalid instruction or global variable not supported: %A" instr
        )
        annoations
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
        annotations
        @
        (cenv.currentInstructions |> List.ofSeq)

    SPVModule.Create(instrs = instrs)

//let mkSPVModule cenv =
//    let spv = 
//        SPVModule.Create(
//            instrs =
//                [
//                    OpCapability Capability.Shader
//                    OpExtInstImport (nextResultId cenv, "GLSL.std.450")
//                    OpMemoryModel (AddressingModel.Logical, MemoryModel.GLSL450)
//                  //  OpEntryPoint (ExecutionModel.Fragment, )
//                ]
//        )

//    { cenv with spv = spv }


//let a = 2

//// exprLambda has type "(int -> int)".
//let exprLambda = <@ fun x -> x + 1 @>
//// exprCall has type unit.
//let exprCall = <@ a + 1 @>

//cln exprLambda
//cln exprCall
//cln <@@ let f x = x + 10 in f 10 @@>
