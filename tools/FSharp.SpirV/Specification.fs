module rec FSharp.SpirV.Specification

// https://www.khronos.org/registry/spir-v/specs/unified1/SPIRV.pdf

type SpirVCapability =
    | Matrix                                = 0us
    | Shader                                = 1us // depends on Matrix
    | Geometry                              = 2us // depends on Shader
    | Tessellation                          = 3us // depends on Shader
    | Addresses                             = 4us
    | Linkage                               = 5us
    | Kernel                                = 6us
    | Vector16                              = 7us // depends on Kernel
    | Float16Buffer                         = 8us // depends on Kernel
    | Float16                               = 9us
    | Float64                               = 10us
    | Int64                                 = 11us
    | Int64Atomics                          = 12us // dpends on Int64
    | ImageBasic                            = 13us // depends on Kernel
    | ImageReadWrite                        = 14us // depends on ImageBasic
    | ImageMipmap                           = 15us // depends on ImageBasic
    | Pipes                                 = 17us // depends on Kernel
    | Groups                                = 18us
    | DeviceEnqueue                         = 19us // depends on Kernel
    | LiteralSampler                        = 20us // depends on Kernel
    | AtomicStorage                         = 21us // depends on Shader
    | Int16                                 = 22us
    | TessellationPointSize                 = 23us // depends on Tessellation
    | GeometryPointSize                     = 24us // depends on Geometry
    | ImageGatherExtended                   = 25us // depends on Shader
    | StorageImageMultisample               = 27us // depends on Shader
    | UniformBufferArrayDynamicIndexing     = 28us // depends on Shader
    | SampledImageArrayDynamicIndexing      = 29us // depends on Shader
    | StorageBufferArrayDynamicIndexing     = 30us // depends on Shader
    | StorageImageArrayDynamicIndexing      = 31us // depends on Shader
    | ClipDistance                          = 32us // depends on Shader
    | CullDistance                          = 33us // depends on Shader
    | ImageCubeArray                        = 34us // depends on SampledCubeArray
    | SampleRateShading                     = 35us // depends on Shader
    | ImageRect                             = 36us // depends on SampledRect
    | SampledRect                           = 37us // depends on Shader
    | GenericPoint                          = 38us // depends on Addresses
    | Int8                                  = 39us
    | InputAttachment                       = 40us // depends on Shader
    | SparseResidency                       = 41us // depends on Shader
    | MinLod                                = 42us // depends on Shader
    | Sampled1D                             = 43us
    | Image1D                               = 44us // depends on Sampled1D
    | SampledCubeArray                      = 45us // depends on Shader
    | SampledBuffer                         = 46us
    | ImageBuffer                           = 47us // depends on SampledBuffer
    | ImageMSArray                          = 48us // depends on shader
    | StorageImageExtendedFormats           = 49us // depends on Shader
    | ImageQuery                            = 50us // depends on Shader
    | DerivativeControl                     = 51us // depends on Shader
    | InterpolationFunction                 = 52us // depends on Shader
    | TransformFeedback                     = 53us // depends on Shader
    | GeometryStreams                       = 54us // depends on Geometry
    | StorageImageReadWithoutFormat         = 55us // depends on Shader
    | StorageImageWriteWithoutFormat        = 56us // depends on Shader
    | MultiViewport                         = 57us // depends on Geometry
    // End 1.0 spec

let MagicNumber = 0x07230203u

type SourceLanguage =
    | Unknown = 0u
    | ESSL = 1u
    | GLSL = 2u
    | OpenCL_C = 3u
    | OpenCL_CPP = 4u
    | HLSL = 5u

type Word = uint32

type Operand = Word

type id = Word

type Result_id = Word

type LiteralString = string

type LiteralNumber = Word list

type LiteralNumberLimitOne = Word

type Literal = Word list

type DecorationKind =
    | RelaxedPrecision      = 0u
    | SpecId                = 1u
    | Block                 = 2u
    | BufferBlock           = 3u
    | RowMajor              = 4u
    | ColMajor              = 5u
    | ArrayStride           = 6u
    | MatrixStride          = 7u
    | GLSLShared            = 8u
    | GLSLPacked            = 9u
    | CPacked               = 10u
    | BuiltIn               = 11u
    | NoPerspective         = 13u
    | Flat                  = 14u
    | Patch                 = 15u
    | Centroid              = 16u
    | Sample                = 17u
    | Invariant             = 18u
    | Restrict              = 19u
    | Aliased               = 20u
    | Volatile              = 21u
    | Constant              = 22u
    | Coherent              = 23u
    | NonWritable           = 24u
    | NonReadable           = 25u
    | Uniform               = 26u
    | SaturatedConversion   = 28u
    | Stream                = 29u
    | Location              = 30u
    | Component             = 31u
    | Index                 = 32u
    | Binding               = 33u
    | DescriptorSet         = 34u
    | Offset                = 35u
    | XfbBuffer             = 36u
    | XfbStride             = 37u
    | FuncParamAttr         = 38u
    | FPRoundingMode        = 39u
    | FFFastMathMode        = 40u
    | LinkageAttributes     = 41u
    | NoContraction         = 42u
    | InputAttachmentIndex  = 43u
    | Alignment             = 44u

type Decoration = Decoration of DecorationKind * Literal option

type OpCode =
    | Nop                   = 0us
    | Undef                 = 1us
    | SourceContinued       = 2us
    | Source                = 3us
    | SourceExtension       = 4us
    | Name                  = 5us
    | MemberName            = 6us
    | String                = 7us
    | Line                  = 8us
    | NoLine                = 317us
    | Decorate              = 71us
    | MemberDecorate        = 72us
    | DecorationGroup       = 73us

[<RequireQualifiedAccess>]
type Instr =
    /// 1 word.
    /// This has no semantic impact and can safely be removed from a module.
    | Nop
    /// 3 words.
    /// Make an intermediate object whose value is undefined.
    /// Result Type is the type of object to make.
    /// Each consumption of Result <id> yields an arbitrary, possibly different bit pattern or abstract value resulting in
    /// possibly different concrete, abstract, or opaque values.
    | Undef of resultType: id * Result_id
    /// 2 + variable words
    | SourceContinued of continuedSource: LiteralString
    /// 3 + variable words
    | Source of SourceLanguage * version: LiteralNumberLimitOne * file: id option * source: LiteralString option
    /// 2 + variable words
    | SourceExtension of extension: LiteralString
    /// 3 + variable words
    | Name of target: id * name: LiteralString
    /// 4 + variable words
    | MemberName of type': id * member': LiteralNumberLimitOne * name: LiteralString
    /// 3 + variable words
    | String of Result_id * string: LiteralString
    /// 4 words
    | Line of file: id * line: LiteralNumberLimitOne * column: LiteralNumberLimitOne
    /// 1 word
    | NoLine
    /// 3 + variable words
    | Decorate of target: id * Decoration
    /// 4 + variable words
    | MemberDecorate of structureType: id * member': LiteralNumber * Decoration
    /// 2 words
    | DecorationGroup of Result_id

type SpirVModuleHeader = SpirVModule of magicNumber: Word * versionNumber: Word * generatorMagicNumber: Word * bound: Word * reserved: Word * firstInstruction: Word

type SpirVInstructions = SpirVInstructiosn of Word list

type SpirVModule = SpirVModule of SpirVModuleHeader * SpirVInstructions

module Unpickle =

    open FSharp.SpirV.UnpickleHelper

    let u_wordCount = u_uint16

    let u_opCode: Unpickle<OpCode> = 
        u_uint16 |>> fun word -> LanguagePrimitives.EnumOfValue word

    let u_instr =
        u_bpipe2 u_wordCount u_opCode <|
        fun wordCount opCode ->
            let remainingWordCount = wordCount - 1us

            match opCode with
            | OpCode.Nop -> 
                u_return Instr.Nop

            | OpCode.Undef ->
                
                u_return Instr.Nop

            | _ ->
                failwith "OpCode not supported."
