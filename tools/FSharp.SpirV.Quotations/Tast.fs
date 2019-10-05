module FSharp.Spirv.Quotations.Tast

open FSharp.Spirv
open FSharp.Spirv.Specification

type Decorations = (Decoration * uint32 list) list

type SpirvType =
    | SpirvTypeInt
    | SpirvTypeSingle
    | SpirvTypeVector2
    | SpirvTypeVector3
    | SpirvTypeVector4
    | SpirvTypeArray of SpirvType * count: int
    | SpirvDecorationType of Decorations * SpirvType

type SpirvVar = 
    {
        Name: string
        Type: SpirvType
        Decorations: Decorations
    }

type SpirvConst =
    | SpirvInt of int
    | SpirvSingle of single
    | SpirvArray of elementType: SpirvType * constants: SpirvConst list
    | SpirvDecorationConst of Decorations * SpirvConst

type SpirvExpr =
    | SpirvLet of SpirvVar * SpirvExpr * isMutable: bool * cont: SpirvExpr
    | SpirvSequential of SpirvExpr * SpirvExpr
    | SpirvNewVector2 of arg1: SpirvExpr * arg2: SpirvExpr
    | SpirvNewVector3 of arg1: SpirvExpr * arg2: SpirvExpr * arg3: SpirvExpr
    | SpirvNewVector4 of arg1: SpirvExpr * arg2: SpirvExpr * arg3: SpirvExpr * arg4: SpirvExpr
    | SpirvArrayIndexerGet of receiver: SpirvVar * arg: SpirvExpr 

type SpirvDefnExpr =
    | SpirvDefnConst of SpirvConst   
    | SpirvDefnVar of SpirvVar
    | SpirvDefnLambda of SpirvVar

type SpirvDefnDecl =
    | SpirvDefnDeclConst of SpirvConst
    | SpirvDefnDeclVar of SpirvVar
    | SpirvDefnDeclType of SpirvType

type EntryPoint =
    {
        ExecutionModel: ExecutionModel
        FunctionName: string
        Interface: SpirvVar list
    }

type SpirvDefnModule =
    {
        Capabilities: Capability list
        Extensions: string list
        ExtendedInstructionSets: string list
        MemoryModel: MemoryModel
        EntryPoints: EntryPoint list
        ExecutionModes: ExecutionMode list
        Annoations: Decorations
    }


