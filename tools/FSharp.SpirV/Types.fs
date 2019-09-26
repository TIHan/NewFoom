namespace FSharp.SpirV

open Specification

type Word = uint32

type Operand = Word

type id = Word

type Result_id = Word

type LiteralString = string

type LiteralNumber = Word list

type LiteralNumberLimitOne = Word

type Literal = Word list

[<NoEquality;NoComparison>]
type SPVInstruction =
    /// 1 word.
    /// This has no semantic impact and can safely be removed from a module.
    | OpNop
    /// 3 words.
    /// Make an intermediate object whose value is undefined.
    /// Result Type is the type of object to make.
    /// Each consumption of Result <id> yields an arbitrary, possibly different bit pattern or abstract value resulting in
    /// possibly different concrete, abstract, or opaque values.
    | OpUndef of resultType: id * Result_id
    /// 2 + variable words
    | OpSourceContinued of continuedSource: LiteralString

    | OpSource of SourceLanguage * version: LiteralNumberLimitOne * file: id * source: LiteralString
    /// 2 + variable words
    | OpSourceExtension of extension: LiteralString
    /// 3 + variable words
    | OpName of target: id * name: LiteralString
    /// 4 + variable words
    | OpMemberName of type': id * member': LiteralNumberLimitOne * name: LiteralString
    | OpString of Result_id * string: LiteralString
    /// 4 words
    | OpLine of file: id * line: LiteralNumberLimitOne * column: LiteralNumberLimitOne
    /// 1 word
    | OpNoLine
    /// 3 + variable words
    | OpDecorate of target: id * Decoration
    /// 4 + variable words
    | OpMemberDecorate of structureType: id * member': LiteralNumber * Decoration
    /// 2 words
    | OpDecorationGroup of Result_id

    | OpExtInstImport of Result_id * name: LiteralString
    | OpMemoryModel of AddressingModel * MemoryModel
    | OpEntryPoint of ExecutionModel * entryPoint: id * name: LiteralString * interfaceIds: id list
    | OpExecutionMode of entryPoint: id * ExecutionMode * LiteralNumber
    | OpCapability of Capability

    | OpExecutionModeId of entryPoint: id * mode: ExecutionMode * id list

    | UnhandledOp of Op * Word list

[<NoEquality;NoComparison>]
type SPVModule = 
    internal {
        magicNumber: uint32
        versionNumber: uint32
        genMagicNumber: uint32
        bound: uint32
        instrs: SPVInstruction list
    }

    member x.MagicNumber = x.magicNumber