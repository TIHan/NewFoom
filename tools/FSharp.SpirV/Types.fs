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

type Literals = Word list

[<NoEquality;NoComparison>]
type SPVInstruction =

    // Miscellaneous Instructions

    | OpNop
    | OpUndef of resultType: id * Result_id
    /// Missing before version 1.1
    | OpSizeOf of resultType: id * Result_id * pointer: id

    // Debug Instructions

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

    // Annotation Instructions

    | OpDecorate of target: id * Decoration * Literal
    | OpMemberDecorate of structureType: id * member': LiteralNumberLimitOne * Decoration * Literal
    | [<Obsolete("directly use non-group decoration instructions instead")>] OpDecorationGroup of Result_id
    | [<Obsolete("directly use non-group decoration instructions instead")>] OpGroupDecorate of decorationGroup: id * targets: id list
    | [<Obsolete("directly use non-group decoration instructions instead")>] OpGroupMemberDecorate of decorationGroup: id * targets: Word list
    | OpDecorateId of target: id * Decoration * id list
    /// Missing before version 1.4
    | OpDecorateString of target: id * Decoration * LiteralString * LiteralString list
    /// Missing before version 1.4
    | OpMemberDecorateString of structType: id * member': LiteralNumber * Decoration * LiteralString * LiteralStrings

    // Extension Instructions

    | OpExtension of name: LiteralString
    | OpExtInstImport of Result_id * name: LiteralString
    | OpExtInst of resultType: id * Result_id * set: id * instruction: LiteralNumber * id list

    // Mode-Settings Instructions

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
    | OpConstant of resultType: id * Result_id * value: Literals
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

    | OpFunction of resultType: id * Result_id * FunctionControlMask * functionType: id
    | OpFunctionParameter of resultType: id * Result_id
    | OpFunctionEnd
    | OpFunctionCall of resultType: id * Result_id * function': id * arguments: id list

    // Conversion Instructions

    | OpSampledImage of resultType: id * Result_id * image: id * sampler: id
    | OpImageSampleImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask option * id list  
    | OpImageSampleExplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask * id * id list
    | OpImageSampleDrefImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask option * id list
    | OpImageSampleDrefExplicitLod of resultType: id * Result_id * smapledImage: id * coordinate: id * dref: id * ImageOperandsMask * id * id list
    | OpImageSampleProjImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageSampleProjExplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask * id * id list
    | OpImageSampleProjDrefImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask option * id list
    | OpImageSampleProjDrefExplicitLod of resultType: id * Result_id * smapledImage: id * coordinate: id * dref: id * ImageOperandsMask * id * id list
    | OpImageFetch of resultType: id * Result_id * image: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageGather of resultType: id * Result_id * sampledImage: id * coordinate: id * component': id * ImageOperandsMask option * id list
    | OpImageDrefGather of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask option * id list
    | OpImageRead of resultType: id * Result_id * image: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageWrite of image: id * coordinate: id * texel: id * ImageOperandsMask option * id list
    | OpImage of resultType: id * Result_id * sampledImage: id
    | OpImageQueryFormat of resultType: id * Result_id * image: id
    | OpImageQueryOrder of resultType: id * Result_id * image: id
    | OpImageQuerySizeLod of resultType: id * Result_id * image: id * levelOfDetail: id
    | OpImageQuerySize of resultType: id * Result_id * image: id
    | OpImageQueryLod of resultType: id * Result_id * sampledImage: id * coordinate: id
    | OpImageQueryLevels of resultType: id * Result_id * image: id
    | OpImageQuerySamples of resultType: id * Result_id * image: id
    | OpImageSparseSampleImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageSparseSampleExplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask * id * id list
    | OpImageSparseSampleDrefImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask option * id list
    | OpImageSparseSampleDrefExplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask * id * id list
    | OpImageSparseSampleProjImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageSparseSampleProjExplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * ImageOperandsMask * id * id list
    | OpImageSparseSampleProjectDrefImplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask option * id list
    | OpImageSparseSampleProjectDrefExplicitLod of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask * id * id list
    | OpImageSparseFetch of resultType: id * Result_id * image: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageSparseGather of resultType: id * Result_id * sampledImage: id * coordinate: id * component': id * ImageOperandsMask option * id list
    | OpImageSparseDrefGather of resultType: id * Result_id * sampledImage: id * coordinate: id * dref: id * ImageOperandsMask option * id list
    | OpImageSparseTexelsResident of resultType: id * Result_id * residentCode: id
    | OpImageSparseRead of resultType: id * Result_id * image: id * coordinate: id * ImageOperandsMask option * id list
    | OpImageSampleFootprintNV of resultType: id * Result_id * sampledImage: id * coordinate: id * granularity: id * coarse: id * ImageOperandsMask option * id list

    // Conversion Instructions

    | OpConvertFToU of resultType: id * Result_id * floatValue: id
    | OpConvertFToS of resultType: id * Result_id * floatValue: id
    | OpConvertSToF of resultType: id * Result_id * signedValue: id
    | OpConvertUToF of resultType: id * Result_id * unsignedValue: id
    | OpUConvert of resultType: id * Result_id * unsignedValue: id
    | OpSConvert of resultType: id * Result_id * signedValue: id
    | OpFConvert of resultType: id * Result_id * floatValue: id
    | OpQuantizeToF16 of resultType: id * Result_id * value: id
    | OpConvertPtrToU of resultType: id * Result_id * pointer: id
    | OpSatConvertSToU of resultType: id * Result_id * signedValue: id
    | OpSatConvertUToS of resultType: id * Result_id * unsignedValue: id
    | OpConvertUToPtr of resultType: id * Result_id * integerValue: id
    | OpPtrCastToGeneric of resultType: id * Result_id * pointer: id
    | OpGenericCastToPtr of resultType: id * Result_id * pointer: id
    | OpGenericCastToPtrExplicit of resultType: id * Result_id * pointer: id * StorageClass
    | OpBitcast of resultType: id * Result_id * operand: id

    // Composite Instructions

    | OpVectorExtractDynamic of resultType: id * Result_id * vector: id * index: id
    | OpCompositeConstruct of resultType: id * Result_id * constituents: id list
    | OpCompositeExtract of resultType: id * Result_id * composite: id * indexes: Literals

    // Relational and Logical Instructions

    | OpReturn

    // Control-Flow Instructions

    | OpLabel of Result_id

    | UnhandledOp of Op * Word list

[<NoEquality;NoComparison>]
type SPVModule = 
    internal {
        magicNumber: Word
        versionNumber: Word
        genMagicNumber: Word
        bound: Word
        instrs: SPVInstruction list
    }

    static member Create (?version: Word, ?bound: Word, ?instrs) =
        let version = defaultArg version 65536u
        let bound = defaultArg bound 65536u
        let instrs = defaultArg instrs []

        {
            magicNumber = MagicNumber
            versionNumber = version
            genMagicNumber = 0u
            bound = bound
            instrs = instrs
        }

    member x.Instructions = x.instrs

    member x.AddInstructions instrs =
        { x with instrs = x.instrs @ instrs }