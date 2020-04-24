[<AutoOpen>]
module FSharp.Spirv.Quotations.Compilation

open System
open System.IO
open FSharp.Spirv
open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns
open FSharp.Spirv.Quotations.Checker
open FSharp.Spirv.Quotations.Optimizer
open FSharp.Spirv.Quotations.SpirvGen
open FSharp.Spirv.Quotations.ErrorLogger
open FSharp.Spirv.Quotations.TypedTree
open FSharp.Spirv.Quotations.Intrinsics

[<Sealed>]
type FSharpSpirvQuotationDiagnostic internal (error: Error) =

    member _.IsError = match error with Error(ErrorKind.Error, _, _) -> true | _ -> false

    member _.IsWarning = match error with Error(ErrorKind.Warning, _, _) -> true | _ -> false

    member _.Message = match error with Error(_, message, _) -> message

    override this.ToString() = this.Message

type FSharpSpirvQuotationCompilationOptions =
    {
        DebugEnabled: bool
        OptimizationsEnabled: bool
        Capabilities: Capability list
        ExtendedInstructionSets: string list
        AddressingModel: AddressingModel
        MemoryModel: MemoryModel
        ExecutionModel: ExecutionModel
        ExecutionMode: ExecutionMode option
    }

[<Sealed>]
type FSharpSpirvType internal (spvTy: SpirvType) =

    member _.IsSampledImage =
        match spvTy with
        | SpirvTypeSampledImage _ -> true
        | _ -> false

    member _.IsSampler =
        match spvTy with
        | SpirvTypeSampler _ -> true
        | _ -> false

    member _.IsImage =
        match spvTy with
        | SpirvTypeImage _ -> true
        | _ -> false

    member _.IsVoid = spvTy.IsVoid

    member _.IsOpaque = spvTy.IsOpaque

    member _.IsRuntimeArray = spvTy.IsRuntimeArray

    member _.IsStruct = spvTy.IsStruct

[<Sealed>]
type FSharpSpirvQuotationVariable internal (clrTy: Type, spvVar: SpirvVar, customAnnoations: Expr list) =
    
    member _.Name = spvVar.Name

    member _.ClrType = clrTy

    member _.Type = FSharpSpirvType(spvVar.Type)

    member _.StorageClass = spvVar.StorageClass

    member _.Decorations = spvVar.Decorations

    member _.CustomAnnoations = customAnnoations

let private getVariables (expr: Expr) =
    let rec loop expr acc =
        match expr with
        | Sequential(expr1, expr2) ->
            loop expr2 acc @ loop expr1 acc
        | Let(var, rhs, body) ->
            let acc =
                match rhs with
                | SpecificCall <@ Variable<_> @> (None, [_], args) ->
                    let spvVar, customAnnoations = Checker.CheckVariable var args
                    FSharpSpirvQuotationVariable(var.Type, spvVar, customAnnoations) :: acc
                | _ -> acc

            loop body acc
        | _ ->
            acc
    loop expr []
    |> List.rev

[<Sealed>]
type FSharpSpirvQuotationCompilation private (options, quotation: Expr<unit -> unit>) =

    let lazyCheckedQuotation =
        lazy 
            let checkedQuotation, errors = Check quotation
            checkedQuotation, (errors |> List.map FSharpSpirvQuotationDiagnostic)

    member _.Options = options

    member _.Quotation = quotation

    member _.MergeQuotation(quotation: Expr<unit -> unit>) : FSharpSpirvQuotationCompilation =
        failwith "Not implemented yet."

    member _.ReplaceQuotation quotation =
        FSharpSpirvQuotationCompilation(options, quotation)

    member _.GetDiagnostics() =
        let _, diagnostics = lazyCheckedQuotation.Value
        diagnostics

    member _.GetVariables() =
        getVariables quotation

    member _.Emit(spirvModuleStream: Stream) =
        let checkedQuotation, diagnostics = lazyCheckedQuotation.Value

        if diagnostics |> List.exists (fun x -> x.IsError) then
            false
        else

        let checkedQuotation =
            if options.OptimizationsEnabled then
                Optimize checkedQuotation
            else
                checkedQuotation

        let genOptions =
            {
                CodeGenDebugInformationEnabled = options.DebugEnabled
                Capabilities = options.Capabilities
                ExtendedInstructionSets = options.ExtendedInstructionSets
                AddressingModel = options.AddressingModel
                MemoryModel = options.MemoryModel
                ExecutionModel = options.ExecutionModel
                ExecutionMode = options.ExecutionMode
            }

        let spirvModule = GenModule genOptions checkedQuotation
        SpirvModule.Serialize(spirvModuleStream, spirvModule)
        true

    static member Create(options, quotation) =
        FSharpSpirvQuotationCompilation(options, quotation)
