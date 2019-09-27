namespace FSharp.SpirV

open System
open Specification

type Word = uint32

type Operand = Word

type id = Word

type Result_id = Word

type LiteralString = string

type LiteralStrings = LiteralString list

type LiteralNumber = Word list

type LiteralNumberLimitOne = Word

type Literal = Word list

[<NoEquality;NoComparison>]
type SPVInstruction =
    | OpNop
    | OpUndef of resultType: id * Result_id
    /// Missing before version 1.1
    | OpSizeOf of resultType: id * Result_id * pointer: id
    | OpSourceContinued of continuedSource: LiteralString
    | OpSource of SourceLanguage * version: LiteralNumberLimitOne * file: id option * source: LiteralString option
    | OpSourceExtension of extension: LiteralString
    | OpName of target: id * name: LiteralString
    | OpMemberName of type': id * member': LiteralNumberLimitOne * name: LiteralString
    | OpString of Result_id * string: LiteralString
    | OpLine of file: id * line: LiteralNumberLimitOne * column: LiteralNumberLimitOne
    | OpNoLine
    /// Missing before version 1.1
    | OpModuleProcessed of process': LiteralString
    | OpDecorate of target: id * Decoration * Literal
    | OpMemberDecorate of structureType: id * member': LiteralNumber * Decoration * Literal
    | [<Obsolete("directly use non-group decoration instructions instead")>] OpDecorationGroup of Result_id
    | [<Obsolete("directly use non-group decoration instructions instead")>] OpGroupDecorate of decorationGroup: id * targets: id list
    | [<Obsolete("directly use non-group decoration instructions instead")>] OpGroupMemberDecorate of decorationGroup: id * targets: Word list
    | OpDecorateId of target: id * Decoration * id list
    /// Missing before version 1.4
    | OpDecorateString of target: id * Decoration * LiteralString * LiteralString list
    /// Missing before version 1.4
    | OpMemberDecorateString of structType: id * member': LiteralNumber * Decoration * LiteralString * LiteralStrings
    | OpExtension of name: LiteralString
    | OpExtInstImport of Result_id * name: LiteralString
    | OpExtInst of resultType: id * Result_id * set: id * instruction: LiteralNumber * id list
    | OpMemoryModel of AddressingModel * MemoryModel
    | OpEntryPoint of ExecutionModel * entryPoint: id * name: LiteralString * interface': id list
    | OpExecutionMode of entryPoint: id * ExecutionMode * LiteralNumber
    | OpCapability of Capability
    /// Missing before version 1.2
    | OpExecutionModeId of entryPoint: id * mode: ExecutionMode * id list

    // Type-Declaration Instructions

    | OpTypeVoid of Result_id
    | OpTypeBool of Result_id
    | OpTypeInt of Result_id * width: LiteralNumberLimitOne * signedness: LiteralNumber
    | OpTypeFloat of Result_id * width: LiteralNumber
    | OpTypeVector of Result_id * componentType: id * componentCount: LiteralNumber
    | OpTypeMatrix of Result_id * columnType: id * columnCount: LiteralNumber
    | OpTypeImage of Result_id * sampledType: id * Dim * depth: LiteralNumberLimitOne * arrayed: LiteralNumberLimitOne * ms: LiteralNumberLimitOne * sampled: LiteralNumberLimitOne * ImageFormat * AccessQualifier option
    | OpTypeSampler of Result_id
    | OpTypeSampledImage of Result_id * imageType: id
    | OpTypeArray of Result_id * elementType: id * length: id
    | OpTypeRuntimeArray of Result_id * elementType: id
    | OpTypeStruct of Result_id * members: id list
    | OpTypeOpaque of Result_id * LiteralString
    | OpTypePointer of Result_id * StorageClass * type': id
    | OpTypeFunction of Result_id * returnType: id * parameters: id list
    | OpTypeEvent of Result_id
    | OpTypeDeviceEvent of Result_id
    | OpTypeReserveId of Result_id
    | OpTypeQueue of Result_id
    | OpTypePipe of Result_id * AccessQualifier
    | OpTypeForwardPointer of pointerType: id * StorageClass
    | OpTypePipeStorage of Result_id
    | OpTypeNamedBarrier of Result_id

    // Constant-Create Instructions

    | OpConstantTrue of resultType: id * Result_id
    | OpConstantFalse of resultType: id * Result_id
    | OpConstant of resultType: id * Result_id * value: Literal
    | OpConstantComposite of resultType: id * Result_id * constituents: id list
    | OpConstantSampler of resultType: id * Result_id * SamplerAddressingMode * param: LiteralNumberLimitOne * SamplerFilterMode
    | OpConstantNull of resultType: id * Result_id
    | OpSpecConstantTrue of resultType: id * Result_id
    | OpSpecConstantFalse of resultType: id * Result_id
    | OpSpecConstant of resultType: id * Result_id * value: Literal
    | OpSpecConstantComposite of resultType: id * Result_id * constituents: id list
    | OpSpecConstantOp of resultType: id * Result_id * opcode: LiteralNumberLimitOne * operands: id list

    // Memory Instructions

    | OpVariable of resultType: id * Result_id * StorageClass * initializer: id option
    | OpImageTexelPointer of resultType: id * Result_id * image: id * coordinate: id * sample: id
    | OpLoad of resultType: id * Result_id * pointer: id * MemoryAccessMask option
    | OpStore of pointer: id * object: id * MemoryAccessMask option
    | OpCopyMemorySized of target: id * source: id * size: id * MemoryAccessMask option * MemoryAccessMask option
    | OpAccessChain of resultType: id * Result_id * base': id * indexes: id list
    | OpInBoundsAccessChain of resultType: id * Result_id * base': id * indexes: id list
    | OpPtrAccessChain of resultType: id * Result_id * base': id * element: id * indexes: id list
    | OpArrayLength of resultType: id * Result_id * structure: id * arrayMember: LiteralNumberLimitOne
    | OpGenericPtrMemSemantics of resultType: id * Result_id * base': id * element: id list * indexes: id list
    /// Missing before version 1.4
    | OpPtrEqual of resultType: id * Result_id * operand1: id * operand2: id
    /// Missing before version 1.4
    | OpPtrNotEqual of resultType: id * Result_id * operand1: id * operand2: id
    /// Missing before version 1.4
    | OpPtrDiff of resultType: id * Result_id * operand1: id * operand2: id

    // Function Instructions

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