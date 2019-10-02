namespace FSharp.Spirv.Quotations

open FSharp.Spirv
open FSharp.Spirv.Specification

[<Sealed>]
type SpirvGenInfo private (addressingModel, memoryModel, executionModel, capabilities, extendedInstructionSets, executionMode) = 

    member val Capabilities: Capability list = capabilities

    member val ExtendedInstructionSets: string list = extendedInstructionSets

    member val AddressingModel: AddressingModel = addressingModel

    member val MemoryModel: MemoryModel = memoryModel

    member val ExecutionModel: ExecutionModel = executionModel

    member val ExecutionMode: (ExecutionMode * LiteralNumber) option = executionMode

    static member Create (addressingModel, memoryModel, executionModel, ?capabilities, ?extendedInstructionSets, ?executionMode) =
        SpirvGenInfo (addressingModel, memoryModel, executionModel, defaultArg capabilities [], defaultArg extendedInstructionSets [], executionMode)

