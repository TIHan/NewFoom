module internal FSharp.Spirv.Quotations.Tast

open FSharp.Spirv.Specification

let mutable nextStamp = 0L 
let newStamp () =
    System.Threading.Interlocked.Increment &nextStamp 

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
        Stamp: int64
        Name: string
        Type: SpirvType
        Decorations: Decorations
        StorageClass: StorageClass
        IsMutable: bool
    }

let mkSpirvVar (name, ty, decorations, storageClass, isMutable) =
    let stamp = newStamp ()
    {
        Stamp = stamp
        Name = name
        Type = ty
        Decorations = decorations
        StorageClass = storageClass
        IsMutable = isMutable
    }

type SpirvConst =
    | SpirvInt of int
    | SpirvSingle of single
    | SpirvArray of elementType: SpirvType * constants: SpirvConst list
    | SpirvDecorationConst of Decorations * SpirvConst

type SpirvExpr =
    | SpirvNop
    | SpirvConst of SpirvConst
    | SpirvLet of SpirvVar * body: SpirvExpr * cont: SpirvExpr
    | SpirvSequential of SpirvExpr * SpirvExpr
    | SpirvNewVector2 of arg1: SpirvExpr * arg2: SpirvExpr
    | SpirvNewVector3 of arg1: SpirvExpr * arg2: SpirvExpr * arg3: SpirvExpr
    | SpirvNewVector4 of arg1: SpirvExpr * arg2: SpirvExpr * arg3: SpirvExpr * arg4: SpirvExpr
    | SpirvArrayIndexerGet of receiver: SpirvExpr * arg: SpirvExpr
    | SpirvVar of SpirvVar
    | SpirvVarSet of SpirvVar * SpirvExpr

type SpirvTopLevelExpr =
    | SpirvTopLevelLambda of SpirvVar

type SpirvDecl =
    | SpirvDeclConst of SpirvVar * SpirvConst
    | SpirvDeclVar of SpirvVar

type SpirvEntryPoint =
    {
        ExecutionModel: ExecutionModel
        FunctionName: string
    }

type SpirvDefnModule =
    {
        Capabilities: Capability list
        Extensions: string list
        ExtendedInstructionSets: string list
        MemoryModel: MemoryModel
        EntryPoints: SpirvEntryPoint list
        ExecutionModes: ExecutionMode list
        Declarations: SpirvDecl list
        Body: SpirvTopLevelExpr
    }
