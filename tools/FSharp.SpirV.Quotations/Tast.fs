module FSharp.Spirv.Quotations.Tast

open System.Numerics
open FSharp.Spirv.Specification

let mutable nextStamp = 0L 
let newStamp () =
    System.Threading.Interlocked.Increment &nextStamp 

type Decorations = (Decoration * uint32 list) list

type SpirvType =
    | SpirvTypeVoid
    | SpirvTypeInt
    | SpirvTypeSingle
    | SpirvTypeVector2
    | SpirvTypeVector3
    | SpirvTypeVector4
    | SpirvTypeArray of SpirvType * length: int

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
    | SpirvConstInt of int * Decorations
    | SpirvConstSingle of single * Decorations
    | SpirvConstVector2 of single * single * Decorations
    | SpirvConstVector3 of single * single * single * Decorations
    | SpirvConstVector4 of single * single * single * single * Decorations
    | SpirvConstArray of elementTy: SpirvType * constants: SpirvConst list * Decorations

    member x.Decorations =
        match x with
        | SpirvConstInt (_, decorations)
        | SpirvConstSingle (_, decorations)
        | SpirvConstVector2 (_, _, decorations)
        | SpirvConstVector3 (_, _, _, decorations)
        | SpirvConstVector4 (_, _, _, _, decorations)
        | SpirvConstArray (_, _, decorations) -> decorations

type SpirvExpr =
    | SpirvNop
    | SpirvConst of SpirvConst
    | SpirvLet of SpirvVar * rhs: SpirvExpr * body: SpirvExpr
    | SpirvSequential of SpirvExpr * SpirvExpr
    | SpirvNewVector2 of args: SpirvExpr list
    | SpirvNewVector3 of args: SpirvExpr list
    | SpirvNewVector4 of args: SpirvExpr list
    | SpirvArrayIndexerGet of receiver: SpirvExpr * arg: SpirvExpr
    | SpirvVar of SpirvVar
    | SpirvVarSet of SpirvVar * SpirvExpr

    member x.Type =
        let rec getType expr =
            match expr with
            | SpirvNop -> 
                SpirvTypeVoid
            | SpirvConst spvConst ->
                match spvConst with
                | SpirvConstInt _ -> SpirvTypeInt
                | SpirvConstSingle _ -> SpirvTypeSingle
                | SpirvConstVector2 _ -> SpirvTypeVector2
                | SpirvConstVector3 _ -> SpirvTypeVector3
                | SpirvConstVector4 _ -> SpirvTypeVector4
                | SpirvConstArray (elementTy, constants, _) ->
                    SpirvTypeArray (elementTy, constants.Length)
            | SpirvLet(_, _, body) ->
                getType body
            | SpirvSequential (_, expr2) ->
                getType expr2
            | SpirvNewVector2 _ ->
                SpirvTypeVector2
            | SpirvNewVector3 _ ->
                SpirvTypeVector3
            | SpirvNewVector4 _ ->
                SpirvTypeVector4
            | SpirvArrayIndexerGet _ ->
                SpirvTypeVoid
            | SpirvVar spvVar ->
                spvVar.Type
            | SpirvVarSet _ ->
                SpirvTypeVoid
        getType x

type SpirvDecl =
    | SpirvDeclConst of SpirvVar * SpirvConst
    | SpirvDeclVar of SpirvVar

type SpirvTopLevelExpr =
    | SpirvTopLevelSequential of SpirvTopLevelExpr * SpirvTopLevelExpr
    | SpirvTopLevelDecl of SpirvDecl
    | SpirvTopLevelLambda of SpirvVar * SpirvTopLevelExpr
    | SpirvTopLevelLambdaBody of SpirvVar * SpirvExpr

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
        Body: SpirvTopLevelExpr
    }
