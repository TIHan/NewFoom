module internal FSharp.Spirv.Quotations.SpirvGen

open FSharp.Spirv
open TypedTree

type SpirvGenOptions = 
    {
        CodeGenDebugInformationEnabled: bool
        Capabilities: Capability list
        ExtendedInstructionSets: string list
        AddressingModel: AddressingModel
        MemoryModel: MemoryModel
        ExecutionModel: ExecutionModel
        ExecutionMode: ExecutionMode option
    }

val GenModule: SpirvGenOptions -> SpirvTopLevelExpr -> SpirvModule
