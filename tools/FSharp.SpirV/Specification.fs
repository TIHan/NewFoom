﻿module FSharp.SpirV.Specification

// https://www.khronos.org/registry/spir-v/#spec
// https://www.khronos.org/registry/spir-v/specs/unified1/SPIRV.pdf

[<Literal>]
let MagicNumber = 0x07230203u
[<Literal>]
let Version = 0x00010500u
[<Literal>]
let Revision = 1u
[<Literal>]
let OpCodeMask = 0xffffu
[<Literal>]
let WordCountShift = 16u

type SourceLanguage =
    | Unknown = 0
    | ESSL = 1
    | GLSL = 2
    | OpenCL_C = 3
    | OpenCL_CPP = 4
    | HLSL = 5

type ExecutionModel =
    | Vertex = 0
    | TessellationControl = 1
    | TessellationEvaluation = 2
    | Geometry = 3
    | Fragment = 4
    | GLCompute = 5
    | Kernel = 6
    | TaskNV = 5267
    | MeshNV = 5268
    | RayGenerationNV = 5313
    | IntersectionNV = 5314
    | AnyHitNV = 5315
    | ClosestHitNV = 5316
    | MissNV = 5317
    | CallableNV = 5318

type AddressingModel =
    | Logical = 0
    | Physical32 = 1
    | Physical64 = 2
    | PhysicalStorageBuffer64 = 5348
    | PhysicalStorageBuffer64EXT = 5348

type MemoryModel =
    | Simple = 0
    | GLSL450 = 1
    | OpenCL = 2
    | Vulkan = 3
    | VulkanKHR = 3

type ExecutionMode =
    | Invocations = 0
    | SpacingEqual = 1
    | SpacingFractionalEven = 2
    | SpacingFractionalOdd = 3
    | VertexOrderCw = 4
    | VertexOrderCcw = 5
    | PixelCenterInteger = 6
    | OriginUpperLeft = 7
    | OriginLowerLeft = 8
    | EarlyFragmentTests = 9
    | PointMode = 10
    | Xfb = 11
    | DepthReplacing = 12
    | DepthGreater = 14
    | DepthLess = 15
    | DepthUnchanged = 16
    | LocalSize = 17
    | LocalSizeHint = 18
    | InputPoints = 19
    | InputLines = 20
    | InputLinesAdjacency = 21
    | Triangles = 22
    | InputTrianglesAdjacency = 23
    | Quads = 24
    | Isolines = 25
    | OutputVertices = 26
    | OutputPoints = 27
    | OutputLineStrip = 28
    | OutputTriangleStrip = 29
    | VecTypeHint = 30
    | ContractionOff = 31
    | Initializer = 33
    | Finalizer = 34
    | SubgroupSize = 35
    | SubgroupsPerWorkgroup = 36
    | SubgroupsPerWorkgroupId = 37
    | LocalSizeId = 38
    | LocalSizeHintId = 39
    | PostDepthCoverage = 4446
    | DenormPreserve = 4459
    | DenormFlushToZero = 4460
    | SignedZeroInfNanPreserve = 4461
    | RoundingModeRTE = 4462
    | RoundingModeRTZ = 4463
    | StencilRefReplacingEXT = 5027
    | OutputLinesNV = 5269
    | OutputPrimitivesNV = 5270
    | DerivativeGroupQuadsNV = 5289
    | DerivativeGroupLinearNV = 5290
    | OutputTrianglesNV = 5298
    | PixelInterlockOrderedEXT = 5366
    | PixelInterlockUnorderedEXT = 5367
    | SampleInterlockOrderedEXT = 5368
    | SampleInterlockUnorderedEXT = 5369
    | ShadingRateInterlockOrderedEXT = 5370
    | ShadingRateInterlockUnorderedEXT = 5371

type StorageClass =
    | UniformConstant = 0
    | Input = 1
    | Uniform = 2
    | Output = 3
    | Workgroup = 4
    | CrossWorkgroup = 5
    | Private = 6
    | Function = 7
    | Generic = 8
    | PushConstant = 9
    | AtomicCounter = 10
    | Image = 11
    | StorageBuffer = 12
    | CallableDataNV = 5328
    | IncomingCallableDataNV = 5329
    | RayPayloadNV = 5338
    | HitAttributeNV = 5339
    | IncomingRayPayloadNV = 5342
    | ShaderRecordBufferNV = 5343
    | PhysicalStorageBuffer = 5349
    | PhysicalStorageBufferEXT = 5349

type Dim =
    | Dim1D = 0
    | Dim2D = 1
    | Dim3D = 2
    | Cube = 3
    | Rect = 4
    | Buffer = 5
    | SubpassData = 6

type SamplerAddressingMode =
    | None = 0
    | ClampToEdge = 1
    | Clamp = 2
    | Repeat = 3
    | RepeatMirrored = 4

type SamplerFilterMode =
    | Nearest = 0
    | Linear = 1

type ImageFormat =
    | Unknown = 0
    | Rgba32f = 1
    | Rgba16f = 2
    | R32f = 3
    | Rgba8 = 4
    | Rgba8Snorm = 5
    | Rg32f = 6
    | Rg16f = 7
    | R11fG11fB10f = 8
    | R16f = 9
    | Rgba16 = 10
    | Rgb10A2 = 11
    | Rg16 = 12
    | Rg8 = 13
    | R16 = 14
    | R8 = 15
    | Rgba16Snorm = 16
    | Rg16Snorm = 17
    | Rg8Snorm = 18
    | R16Snorm = 19
    | R8Snorm = 20
    | Rgba32i = 21
    | Rgba16i = 22
    | Rgba8i = 23
    | R32i = 24
    | Rg32i = 25
    | Rg16i = 26
    | Rg8i = 27
    | R16i = 28
    | R8i = 29
    | Rgba32ui = 30
    | Rgba16ui = 31
    | Rgba8ui = 32
    | R32ui = 33
    | Rgb10a2ui = 34
    | Rg32ui = 35
    | Rg16ui = 36
    | Rg8ui = 37
    | R16ui = 38
    | R8ui = 39

type ImageChannelOrder =
    | R = 0
    | A = 1
    | RG = 2
    | RA = 3
    | RGB = 4
    | RGBA = 5
    | BGRA = 6
    | ARGB = 7
    | Intensity = 8
    | Luminance = 9
    | Rx = 10
    | RGx = 11
    | RGBx = 12
    | Depth = 13
    | DepthStencil = 14
    | sRGB = 15
    | sRGBx = 16
    | sRGBA = 17
    | sBGRA = 18
    | ABGR = 19

type ImageChannelDataType =
    | SnormInt8 = 0
    | SnormInt16 = 1
    | UnormInt8 = 2
    | UnormInt16 = 3
    | UnormShort565 = 4
    | UnormShort555 = 5
    | UnormInt101010 = 6
    | SignedInt8 = 7
    | SignedInt16 = 8
    | SignedInt32 = 9
    | UnsignedInt8 = 10
    | UnsignedInt16 = 11
    | UnsignedInt32 = 12
    | HalfFloat = 13
    | Float = 14
    | UnormInt24 = 15
    | UnormInt101010_2 = 16

type ImageOperandsShift =
    | Bias = 0
    | Lod = 1
    | Grad = 2
    | ConstOffset = 3
    | Offset = 4
    | ConstOffsets = 5
    | Sample = 6
    | MinLod = 7
    | MakeTexelAvailable = 8
    | MakeTexelAvailableKHR = 8
    | MakeTexelVisible = 9
    | MakeTexelVisibleKHR = 9
    | NonPrivateTexel = 10
    | NonPrivateTexelKHR = 10
    | VolatileTexel = 11
    | VolatileTexelKHR = 11
    | SignExtend = 12
    | ZeroExtend = 13

type ImageOperandsMask =
    | MaskNone = 0
    | Bias = 0x00000001
    | Lod = 0x00000002
    | Grad = 0x00000004
    | ConstOffset = 0x00000008
    | Offset = 0x00000010
    | ConstOffsets = 0x00000020
    | Sample = 0x00000040
    | MinLod = 0x00000080
    | MakeTexelAvailable = 0x00000100
    | MakeTexelAvailableKHR = 0x00000100
    | MakeTexelVisible = 0x00000200
    | MakeTexelVisibleKHR = 0x00000200
    | NonPrivateTexel = 0x00000400
    | NonPrivateTexelKHR = 0x00000400
    | VolatileTexel = 0x00000800
    | VolatileTexelKHR = 0x00000800
    | SignExtend = 0x00001000
    | ZeroExtend = 0x00002000

type FPFastMathModeShift =
    | NotNaN = 0
    | NotInf = 1
    | NSZ = 2
    | AllowRecip = 3
    | Fast = 4

type FPFastMathModeMask =
    | MaskNone = 0
    | NotNaN = 0x00000001
    | NotInf = 0x00000002
    | NSZ = 0x00000004
    | AllowRecip = 0x00000008
    | Fast = 0x00000010

type FPRoundingMode =
    | RTE = 0
    | RTZ = 1
    | RTP = 2
    | RTN = 3

type LinkageType =
    | Export = 0
    | Import = 1

type AccessQualifier =
    | ReadOnly = 0
    | WriteOnly = 1
    | ReadWrite = 2

type FunctionParameterAttribute =
    | Zext = 0
    | Sext = 1
    | ByVal = 2
    | Sret = 3
    | NoAlias = 4
    | NoCapture = 5
    | NoWrite = 6
    | NoReadWrite = 7

type Decoration =
    | RelaxedPrecision = 0
    | SpecId = 1
    | Block = 2
    | BufferBlock = 3
    | RowMajor = 4
    | ColMajor = 5
    | ArrayStride = 6
    | MatrixStride = 7
    | GLSLShared = 8
    | GLSLPacked = 9
    | CPacked = 10
    | BuiltIn = 11
    | NoPerspective = 13
    | Flat = 14
    | Patch = 15
    | Centroid = 16
    | Sample = 17
    | Invariant = 18
    | Restrict = 19
    | Aliased = 20
    | Volatile = 21
    | Constant = 22
    | Coherent = 23
    | NonWritable = 24
    | NonReadable = 25
    | Uniform = 26
    | UniformId = 27
    | SaturatedConversion = 28
    | Stream = 29
    | Location = 30
    | Component = 31
    | Index = 32
    | Binding = 33
    | DescriptorSet = 34
    | Offset = 35
    | XfbBuffer = 36
    | XfbStride = 37
    | FuncParamAttr = 38
    | FPRoundingMode = 39
    | FPFastMathMode = 40
    | LinkageAttributes = 41
    | NoContraction = 42
    | InputAttachmentIndex = 43
    | Alignment = 44
    | MaxByteOffset = 45
    | AlignmentId = 46
    | MaxByteOffsetId = 47
    | NoSignedWrap = 4469
    | NoUnsignedWrap = 4470
    | ExplicitInterpAMD = 4999
    | OverrideCoverageNV = 5248
    | PassthroughNV = 5250
    | ViewportRelativeNV = 5252
    | SecondaryViewportRelativeNV = 5256
    | PerPrimitiveNV = 5271
    | PerViewNV = 5272
    | PerTaskNV = 5273
    | PerVertexNV = 5285
    | NonUniform = 5300
    | NonUniformEXT = 5300
    | RestrictPointer = 5355
    | RestrictPointerEXT = 5355
    | AliasedPointer = 5356
    | AliasedPointerEXT = 5356
    | CounterBuffer = 5634
    | HlslCounterBufferGOOGLE = 5634
    | HlslSemanticGOOGLE = 5635
    | UserSemantic = 5635
    | UserTypeGOOGLE = 5636

type BuiltIn =
    | Position = 0
    | PointSize = 1
    | ClipDistance = 3
    | CullDistance = 4
    | VertexId = 5
    | InstanceId = 6
    | PrimitiveId = 7
    | InvocationId = 8
    | Layer = 9
    | ViewportIndex = 10
    | TessLevelOuter = 11
    | TessLevelInner = 12
    | TessCoord = 13
    | PatchVertices = 14
    | FragCoord = 15
    | PointCoord = 16
    | FrontFacing = 17
    | SampleId = 18
    | SamplePosition = 19
    | SampleMask = 20
    | FragDepth = 22
    | HelperInvocation = 23
    | NumWorkgroups = 24
    | WorkgroupSize = 25
    | WorkgroupId = 26
    | LocalInvocationId = 27
    | GlobalInvocationId = 28
    | LocalInvocationIndex = 29
    | WorkDim = 30
    | GlobalSize = 31
    | EnqueuedWorkgroupSize = 32
    | GlobalOffset = 33
    | GlobalLinearId = 34
    | SubgroupSize = 36
    | SubgroupMaxSize = 37
    | NumSubgroups = 38
    | NumEnqueuedSubgroups = 39
    | SubgroupId = 40
    | SubgroupLocalInvocationId = 41
    | VertexIndex = 42
    | InstanceIndex = 43
    | SubgroupEqMask = 4416
    | SubgroupEqMaskKHR = 4416
    | SubgroupGeMask = 4417
    | SubgroupGeMaskKHR = 4417
    | SubgroupGtMask = 4418
    | SubgroupGtMaskKHR = 4418
    | SubgroupLeMask = 4419
    | SubgroupLeMaskKHR = 4419
    | SubgroupLtMask = 4420
    | SubgroupLtMaskKHR = 4420
    | BaseVertex = 4424
    | BaseInstance = 4425
    | DrawIndex = 4426
    | DeviceIndex = 4438
    | ViewIndex = 4440
    | BaryCoordNoPerspAMD = 4992
    | BaryCoordNoPerspCentroidAMD = 4993
    | BaryCoordNoPerspSampleAMD = 4994
    | BaryCoordSmoothAMD = 4995
    | BaryCoordSmoothCentroidAMD = 4996
    | BaryCoordSmoothSampleAMD = 4997
    | BaryCoordPullModelAMD = 4998
    | FragStencilRefEXT = 5014
    | ViewportMaskNV = 5253
    | SecondaryPositionNV = 5257
    | SecondaryViewportMaskNV = 5258
    | PositionPerViewNV = 5261
    | ViewportMaskPerViewNV = 5262
    | FullyCoveredEXT = 5264
    | TaskCountNV = 5274
    | PrimitiveCountNV = 5275
    | PrimitiveIndicesNV = 5276
    | ClipDistancePerViewNV = 5277
    | CullDistancePerViewNV = 5278
    | LayerPerViewNV = 5279
    | MeshViewCountNV = 5280
    | MeshViewIndicesNV = 5281
    | BaryCoordNV = 5286
    | BaryCoordNoPerspNV = 5287
    | FragSizeEXT = 5292
    | FragmentSizeNV = 5292
    | FragInvocationCountEXT = 5293
    | InvocationsPerPixelNV = 5293
    | LaunchIdNV = 5319
    | LaunchSizeNV = 5320
    | WorldRayOriginNV = 5321
    | WorldRayDirectionNV = 5322
    | ObjectRayOriginNV = 5323
    | ObjectRayDirectionNV = 5324
    | RayTminNV = 5325
    | RayTmaxNV = 5326
    | InstanceCustomIndexNV = 5327
    | ObjectToWorldNV = 5330
    | WorldToObjectNV = 5331
    | HitTNV = 5332
    | HitKindNV = 5333
    | IncomingRayFlagsNV = 5351
    | WarpsPerSMNV = 5374
    | SMCountNV = 5375
    | WarpIDNV = 5376
    | SMIDNV = 5377

type SelectionControlShift =
    | Flatten = 0
    | DontFlatten = 1

type SelectionControlMask =
    | MaskNone = 0
    | Flatten = 0x00000001
    | DontFlatten = 0x00000002

type LoopControlShift =
    | Unroll = 0
    | DontUnroll = 1
    | DependencyInfinite = 2
    | DependencyLength = 3
    | MinIterations = 4
    | MaxIterations = 5
    | IterationMultiple = 6
    | PeelCount = 7
    | PartialCount = 8

type LoopControlMask =
    | MaskNone = 0
    | Unroll = 0x00000001
    | DontUnroll = 0x00000002
    | DependencyInfinite = 0x00000004
    | DependencyLength = 0x00000008
    | MinIterations = 0x00000010
    | MaxIterations = 0x00000020
    | IterationMultiple = 0x00000040
    | PeelCount = 0x00000080
    | PartialCount = 0x00000100

type FunctionControlShift =
    | Inline = 0
    | DontInline = 1
    | Pure = 2
    | Const = 3

type FunctionControlMask =
    | MaskNone = 0
    | Inline = 0x00000001
    | DontInline = 0x00000002
    | Pure = 0x00000004
    | Const = 0x00000008

type MemorySemanticsShift =
    | Acquire = 1
    | Release = 2
    | AcquireRelease = 3
    | SequentiallyConsistent = 4
    | UniformMemory = 6
    | SubgroupMemory = 7
    | WorkgroupMemory = 8
    | CrossWorkgroupMemory = 9
    | AtomicCounterMemory = 10
    | ImageMemory = 11
    | OutputMemory = 12
    | OutputMemoryKHR = 12
    | MakeAvailable = 13
    | MakeAvailableKHR = 13
    | MakeVisible = 14
    | MakeVisibleKHR = 14
    | Volatile = 15

type MemorySemanticsMask =
    | MaskNone = 0
    | Acquire = 0x00000002
    | Release = 0x00000004
    | AcquireRelease = 0x00000008
    | SequentiallyConsistent = 0x00000010
    | UniformMemory = 0x00000040
    | SubgroupMemory = 0x00000080
    | WorkgroupMemory = 0x00000100
    | CrossWorkgroupMemory = 0x00000200
    | AtomicCounterMemory = 0x00000400
    | ImageMemory = 0x00000800
    | OutputMemory = 0x00001000
    | OutputMemoryKHR = 0x00001000
    | MakeAvailable = 0x00002000
    | MakeAvailableKHR = 0x00002000
    | MakeVisible = 0x00004000
    | MakeVisibleKHR = 0x00004000
    | Volatile = 0x00008000

type MemoryAccessShift =
    | Volatile = 0
    | Aligned = 1
    | Nontemporal = 2
    | MakePointerAvailable = 3
    | MakePointerAvailableKHR = 3
    | MakePointerVisible = 4
    | MakePointerVisibleKHR = 4
    | NonPrivatePointer = 5
    | NonPrivatePointerKHR = 5

type MemoryAccessMask =
    | MaskNone = 0
    | Volatile = 0x00000001
    | Aligned = 0x00000002
    | Nontemporal = 0x00000004
    | MakePointerAvailable = 0x00000008
    | MakePointerAvailableKHR = 0x00000008
    | MakePointerVisible = 0x00000010
    | MakePointerVisibleKHR = 0x00000010
    | NonPrivatePointer = 0x00000020
    | NonPrivatePointerKHR = 0x00000020

type Scope =
    | CrossDevice = 0
    | Device = 1
    | Workgroup = 2
    | Subgroup = 3
    | Invocation = 4
    | QueueFamily = 5
    | QueueFamilyKHR = 5

type GroupOperation =
    | Reduce = 0
    | InclusiveScan = 1
    | ExclusiveScan = 2
    | ClusteredReduce = 3
    | PartitionedReduceNV = 6
    | PartitionedInclusiveScanNV = 7
    | PartitionedExclusiveScanNV = 8

type KernelEnqueueFlags =
    | NoWait = 0
    | WaitKernel = 1
    | WaitWorkGroup = 2

type KernelProfilingInfoShift =
    | CmdExecTime = 0

type KernelProfilingInfoMask =
    | MaskNone = 0
    | CmdExecTime = 0x00000001

type Capability =
    | Matrix = 0
    | Shader = 1
    | Geometry = 2
    | Tessellation = 3
    | Addresses = 4
    | Linkage = 5
    | Kernel = 6
    | Vector16 = 7
    | Float16Buffer = 8
    | Float16 = 9
    | Float64 = 10
    | Int64 = 11
    | Int64Atomics = 12
    | ImageBasic = 13
    | ImageReadWrite = 14
    | ImageMipmap = 15
    | Pipes = 17
    | Groups = 18
    | DeviceEnqueue = 19
    | LiteralSampler = 20
    | AtomicStorage = 21
    | Int16 = 22
    | TessellationPointSize = 23
    | GeometryPointSize = 24
    | ImageGatherExtended = 25
    | StorageImageMultisample = 27
    | UniformBufferArrayDynamicIndexing = 28
    | SampledImageArrayDynamicIndexing = 29
    | StorageBufferArrayDynamicIndexing = 30
    | StorageImageArrayDynamicIndexing = 31
    | ClipDistance = 32
    | CullDistance = 33
    | ImageCubeArray = 34
    | SampleRateShading = 35
    | ImageRect = 36
    | SampledRect = 37
    | GenericPointer = 38
    | Int8 = 39
    | InputAttachment = 40
    | SparseResidency = 41
    | MinLod = 42
    | Sampled1D = 43
    | Image1D = 44
    | SampledCubeArray = 45
    | SampledBuffer = 46
    | ImageBuffer = 47
    | ImageMSArray = 48
    | StorageImageExtendedFormats = 49
    | ImageQuery = 50
    | DerivativeControl = 51
    | InterpolationFunction = 52
    | TransformFeedback = 53
    | GeometryStreams = 54
    | StorageImageReadWithoutFormat = 55
    | StorageImageWriteWithoutFormat = 56
    | MultiViewport = 57
    | SubgroupDispatch = 58
    | NamedBarrier = 59
    | PipeStorage = 60
    | GroupNonUniform = 61
    | GroupNonUniformVote = 62
    | GroupNonUniformArithmetic = 63
    | GroupNonUniformBallot = 64
    | GroupNonUniformShuffle = 65
    | GroupNonUniformShuffleRelative = 66
    | GroupNonUniformClustered = 67
    | GroupNonUniformQuad = 68
    | ShaderLayer = 69
    | ShaderViewportIndex = 70
    | SubgroupBallotKHR = 4423
    | DrawParameters = 4427
    | SubgroupVoteKHR = 4431
    | StorageBuffer16BitAccess = 4433
    | StorageUniformBufferBlock16 = 4433
    | StorageUniform16 = 4434
    | UniformAndStorageBuffer16BitAccess = 4434
    | StoragePushConstant16 = 4435
    | StorageInputOutput16 = 4436
    | DeviceGroup = 4437
    | MultiView = 4439
    | VariablePointersStorageBuffer = 4441
    | VariablePointers = 4442
    | AtomicStorageOps = 4445
    | SampleMaskPostDepthCoverage = 4447
    | StorageBuffer8BitAccess = 4448
    | UniformAndStorageBuffer8BitAccess = 4449
    | StoragePushConstant8 = 4450
    | DenormPreserve = 4464
    | DenormFlushToZero = 4465
    | SignedZeroInfNanPreserve = 4466
    | RoundingModeRTE = 4467
    | RoundingModeRTZ = 4468
    | Float16ImageAMD = 5008
    | ImageGatherBiasLodAMD = 5009
    | FragmentMaskAMD = 5010
    | StencilExportEXT = 5013
    | ImageReadWriteLodAMD = 5015
    | ShaderClockKHR = 5055
    | SampleMaskOverrideCoverageNV = 5249
    | GeometryShaderPassthroughNV = 5251
    | ShaderViewportIndexLayerEXT = 5254
    | ShaderViewportIndexLayerNV = 5254
    | ShaderViewportMaskNV = 5255
    | ShaderStereoViewNV = 5259
    | PerViewAttributesNV = 5260
    | FragmentFullyCoveredEXT = 5265
    | MeshShadingNV = 5266
    | ImageFootprintNV = 5282
    | FragmentBarycentricNV = 5284
    | ComputeDerivativeGroupQuadsNV = 5288
    | FragmentDensityEXT = 5291
    | ShadingRateNV = 5291
    | GroupNonUniformPartitionedNV = 5297
    | ShaderNonUniform = 5301
    | ShaderNonUniformEXT = 5301
    | RuntimeDescriptorArray = 5302
    | RuntimeDescriptorArrayEXT = 5302
    | InputAttachmentArrayDynamicIndexing = 5303
    | InputAttachmentArrayDynamicIndexingEXT = 5303
    | UniformTexelBufferArrayDynamicIndexing = 5304
    | UniformTexelBufferArrayDynamicIndexingEXT = 5304
    | StorageTexelBufferArrayDynamicIndexing = 5305
    | StorageTexelBufferArrayDynamicIndexingEXT = 5305
    | UniformBufferArrayNonUniformIndexing = 5306
    | UniformBufferArrayNonUniformIndexingEXT = 5306
    | SampledImageArrayNonUniformIndexing = 5307
    | SampledImageArrayNonUniformIndexingEXT = 5307
    | StorageBufferArrayNonUniformIndexing = 5308
    | StorageBufferArrayNonUniformIndexingEXT = 5308
    | StorageImageArrayNonUniformIndexing = 5309
    | StorageImageArrayNonUniformIndexingEXT = 5309
    | InputAttachmentArrayNonUniformIndexing = 5310
    | InputAttachmentArrayNonUniformIndexingEXT = 5310
    | UniformTexelBufferArrayNonUniformIndexing = 5311
    | UniformTexelBufferArrayNonUniformIndexingEXT = 5311
    | StorageTexelBufferArrayNonUniformIndexing = 5312
    | StorageTexelBufferArrayNonUniformIndexingEXT = 5312
    | RayTracingNV = 5340
    | VulkanMemoryModel = 5345
    | VulkanMemoryModelKHR = 5345
    | VulkanMemoryModelDeviceScope = 5346
    | VulkanMemoryModelDeviceScopeKHR = 5346
    | PhysicalStorageBufferAddresses = 5347
    | PhysicalStorageBufferAddressesEXT = 5347
    | ComputeDerivativeGroupLinearNV = 5350
    | CooperativeMatrixNV = 5357
    | FragmentShaderSampleInterlockEXT = 5363
    | FragmentShaderShadingRateInterlockEXT = 5372
    | ShaderSMBuiltinsNV = 5373
    | FragmentShaderPixelInterlockEXT = 5378
    | DemoteToHelperInvocationEXT = 5379
    | SubgroupShuffleINTEL = 5568
    | SubgroupBufferBlockIOINTEL = 5569
    | SubgroupImageBlockIOINTEL = 5570
    | SubgroupImageMediaBlockIOINTEL = 5579
    | IntegerFunctions2INTEL = 5584
    | SubgroupAvcMotionEstimationINTEL = 5696
    | SubgroupAvcMotionEstimationIntraINTEL = 5697
    | SubgroupAvcMotionEstimationChromaINTEL = 5698

type Op =
    | OpNop = 0
    | OpUndef = 1
    | OpSourceContinued = 2
    | OpSource = 3
    | OpSourceExtension = 4
    | OpName = 5
    | OpMemberName = 6
    | OpString = 7
    | OpLine = 8
    | OpExtension = 10
    | OpExtInstImport = 11
    | OpExtInst = 12
    | OpMemoryModel = 14
    | OpEntryPoint = 15
    | OpExecutionMode = 16
    | OpCapability = 17
    | OpTypeVoid = 19
    | OpTypeBool = 20
    | OpTypeInt = 21
    | OpTypeFloat = 22
    | OpTypeVector = 23
    | OpTypeMatrix = 24
    | OpTypeImage = 25
    | OpTypeSampler = 26
    | OpTypeSampledImage = 27
    | OpTypeArray = 28
    | OpTypeRuntimeArray = 29
    | OpTypeStruct = 30
    | OpTypeOpaque = 31
    | OpTypePointer = 32
    | OpTypeFunction = 33
    | OpTypeEvent = 34
    | OpTypeDeviceEvent = 35
    | OpTypeReserveId = 36
    | OpTypeQueue = 37
    | OpTypePipe = 38
    | OpTypeForwardPointer = 39
    | OpConstantTrue = 41
    | OpConstantFalse = 42
    | OpConstant = 43
    | OpConstantComposite = 44
    | OpConstantSampler = 45
    | OpConstantNull = 46
    | OpSpecConstantTrue = 48
    | OpSpecConstantFalse = 49
    | OpSpecConstant = 50
    | OpSpecConstantComposite = 51
    | OpSpecConstantOp = 52
    | OpFunction = 54
    | OpFunctionParameter = 55
    | OpFunctionEnd = 56
    | OpFunctionCall = 57
    | OpVariable = 59
    | OpImageTexelPointer = 60
    | OpLoad = 61
    | OpStore = 62
    | OpCopyMemory = 63
    | OpCopyMemorySized = 64
    | OpAccessChain = 65
    | OpInBoundsAccessChain = 66
    | OpPtrAccessChain = 67
    | OpArrayLength = 68
    | OpGenericPtrMemSemantics = 69
    | OpInBoundsPtrAccessChain = 70
    | OpDecorate = 71
    | OpMemberDecorate = 72
    | OpDecorationGroup = 73
    | OpGroupDecorate = 74
    | OpGroupMemberDecorate = 75
    | OpVectorExtractDynamic = 77
    | OpVectorInsertDynamic = 78
    | OpVectorShuffle = 79
    | OpCompositeConstruct = 80
    | OpCompositeExtract = 81
    | OpCompositeInsert = 82
    | OpCopyObject = 83
    | OpTranspose = 84
    | OpSampledImage = 86
    | OpImageSampleImplicitLod = 87
    | OpImageSampleExplicitLod = 88
    | OpImageSampleDrefImplicitLod = 89
    | OpImageSampleDrefExplicitLod = 90
    | OpImageSampleProjImplicitLod = 91
    | OpImageSampleProjExplicitLod = 92
    | OpImageSampleProjDrefImplicitLod = 93
    | OpImageSampleProjDrefExplicitLod = 94
    | OpImageFetch = 95
    | OpImageGather = 96
    | OpImageDrefGather = 97
    | OpImageRead = 98
    | OpImageWrite = 99
    | OpImage = 100
    | OpImageQueryFormat = 101
    | OpImageQueryOrder = 102
    | OpImageQuerySizeLod = 103
    | OpImageQuerySize = 104
    | OpImageQueryLod = 105
    | OpImageQueryLevels = 106
    | OpImageQuerySamples = 107
    | OpConvertFToU = 109
    | OpConvertFToS = 110
    | OpConvertSToF = 111
    | OpConvertUToF = 112
    | OpUConvert = 113
    | OpSConvert = 114
    | OpFConvert = 115
    | OpQuantizeToF16 = 116
    | OpConvertPtrToU = 117
    | OpSatConvertSToU = 118
    | OpSatConvertUToS = 119
    | OpConvertUToPtr = 120
    | OpPtrCastToGeneric = 121
    | OpGenericCastToPtr = 122
    | OpGenericCastToPtrExplicit = 123
    | OpBitcast = 124
    | OpSNegate = 126
    | OpFNegate = 127
    | OpIAdd = 128
    | OpFAdd = 129
    | OpISub = 130
    | OpFSub = 131
    | OpIMul = 132
    | OpFMul = 133
    | OpUDiv = 134
    | OpSDiv = 135
    | OpFDiv = 136
    | OpUMod = 137
    | OpSRem = 138
    | OpSMod = 139
    | OpFRem = 140
    | OpFMod = 141
    | OpVectorTimesScalar = 142
    | OpMatrixTimesScalar = 143
    | OpVectorTimesMatrix = 144
    | OpMatrixTimesVector = 145
    | OpMatrixTimesMatrix = 146
    | OpOuterProduct = 147
    | OpDot = 148
    | OpIAddCarry = 149
    | OpISubBorrow = 150
    | OpUMulExtended = 151
    | OpSMulExtended = 152
    | OpAny = 154
    | OpAll = 155
    | OpIsNan = 156
    | OpIsInf = 157
    | OpIsFinite = 158
    | OpIsNormal = 159
    | OpSignBitSet = 160
    | OpLessOrGreater = 161
    | OpOrdered = 162
    | OpUnordered = 163
    | OpLogicalEqual = 164
    | OpLogicalNotEqual = 165
    | OpLogicalOr = 166
    | OpLogicalAnd = 167
    | OpLogicalNot = 168
    | OpSelect = 169
    | OpIEqual = 170
    | OpINotEqual = 171
    | OpUGreaterThan = 172
    | OpSGreaterThan = 173
    | OpUGreaterThanEqual = 174
    | OpSGreaterThanEqual = 175
    | OpULessThan = 176
    | OpSLessThan = 177
    | OpULessThanEqual = 178
    | OpSLessThanEqual = 179
    | OpFOrdEqual = 180
    | OpFUnordEqual = 181
    | OpFOrdNotEqual = 182
    | OpFUnordNotEqual = 183
    | OpFOrdLessThan = 184
    | OpFUnordLessThan = 185
    | OpFOrdGreaterThan = 186
    | OpFUnordGreaterThan = 187
    | OpFOrdLessThanEqual = 188
    | OpFUnordLessThanEqual = 189
    | OpFOrdGreaterThanEqual = 190
    | OpFUnordGreaterThanEqual = 191
    | OpShiftRightLogical = 194
    | OpShiftRightArithmetic = 195
    | OpShiftLeftLogical = 196
    | OpBitwiseOr = 197
    | OpBitwiseXor = 198
    | OpBitwiseAnd = 199
    | OpNot = 200
    | OpBitFieldInsert = 201
    | OpBitFieldSExtract = 202
    | OpBitFieldUExtract = 203
    | OpBitReverse = 204
    | OpBitCount = 205
    | OpDPdx = 207
    | OpDPdy = 208
    | OpFwidth = 209
    | OpDPdxFine = 210
    | OpDPdyFine = 211
    | OpFwidthFine = 212
    | OpDPdxCoarse = 213
    | OpDPdyCoarse = 214
    | OpFwidthCoarse = 215
    | OpEmitVertex = 218
    | OpEndPrimitive = 219
    | OpEmitStreamVertex = 220
    | OpEndStreamPrimitive = 221
    | OpControlBarrier = 224
    | OpMemoryBarrier = 225
    | OpAtomicLoad = 227
    | OpAtomicStore = 228
    | OpAtomicExchange = 229
    | OpAtomicCompareExchange = 230
    | OpAtomicCompareExchangeWeak = 231
    | OpAtomicIIncrement = 232
    | OpAtomicIDecrement = 233
    | OpAtomicIAdd = 234
    | OpAtomicISub = 235
    | OpAtomicSMin = 236
    | OpAtomicUMin = 237
    | OpAtomicSMax = 238
    | OpAtomicUMax = 239
    | OpAtomicAnd = 240
    | OpAtomicOr = 241
    | OpAtomicXor = 242
    | OpPhi = 245
    | OpLoopMerge = 246
    | OpSelectionMerge = 247
    | OpLabel = 248
    | OpBranch = 249
    | OpBranchConditional = 250
    | OpSwitch = 251
    | OpKill = 252
    | OpReturn = 253
    | OpReturnValue = 254
    | OpUnreachable = 255
    | OpLifetimeStart = 256
    | OpLifetimeStop = 257
    | OpGroupAsyncCopy = 259
    | OpGroupWaitEvents = 260
    | OpGroupAll = 261
    | OpGroupAny = 262
    | OpGroupBroadcast = 263
    | OpGroupIAdd = 264
    | OpGroupFAdd = 265
    | OpGroupFMin = 266
    | OpGroupUMin = 267
    | OpGroupSMin = 268
    | OpGroupFMax = 269
    | OpGroupUMax = 270
    | OpGroupSMax = 271
    | OpReadPipe = 274
    | OpWritePipe = 275
    | OpReservedReadPipe = 276
    | OpReservedWritePipe = 277
    | OpReserveReadPipePackets = 278
    | OpReserveWritePipePackets = 279
    | OpCommitReadPipe = 280
    | OpCommitWritePipe = 281
    | OpIsValidReserveId = 282
    | OpGetNumPipePackets = 283
    | OpGetMaxPipePackets = 284
    | OpGroupReserveReadPipePackets = 285
    | OpGroupReserveWritePipePackets = 286
    | OpGroupCommitReadPipe = 287
    | OpGroupCommitWritePipe = 288
    | OpEnqueueMarker = 291
    | OpEnqueueKernel = 292
    | OpGetKernelNDrangeSubGroupCount = 293
    | OpGetKernelNDrangeMaxSubGroupSize = 294
    | OpGetKernelWorkGroupSize = 295
    | OpGetKernelPreferredWorkGroupSizeMultiple = 296
    | OpRetainEvent = 297
    | OpReleaseEvent = 298
    | OpCreateUserEvent = 299
    | OpIsValidEvent = 300
    | OpSetUserEventStatus = 301
    | OpCaptureEventProfilingInfo = 302
    | OpGetDefaultQueue = 303
    | OpBuildNDRange = 304
    | OpImageSparseSampleImplicitLod = 305
    | OpImageSparseSampleExplicitLod = 306
    | OpImageSparseSampleDrefImplicitLod = 307
    | OpImageSparseSampleDrefExplicitLod = 308
    | OpImageSparseSampleProjImplicitLod = 309
    | OpImageSparseSampleProjExplicitLod = 310
    | OpImageSparseSampleProjDrefImplicitLod = 311
    | OpImageSparseSampleProjDrefExplicitLod = 312
    | OpImageSparseFetch = 313
    | OpImageSparseGather = 314
    | OpImageSparseDrefGather = 315
    | OpImageSparseTexelsResident = 316
    | OpNoLine = 317
    | OpAtomicFlagTestAndSet = 318
    | OpAtomicFlagClear = 319
    | OpImageSparseRead = 320
    | OpSizeOf = 321
    | OpTypePipeStorage = 322
    | OpConstantPipeStorage = 323
    | OpCreatePipeFromPipeStorage = 324
    | OpGetKernelLocalSizeForSubgroupCount = 325
    | OpGetKernelMaxNumSubgroups = 326
    | OpTypeNamedBarrier = 327
    | OpNamedBarrierInitialize = 328
    | OpMemoryNamedBarrier = 329
    | OpModuleProcessed = 330
    | OpExecutionModeId = 331
    | OpDecorateId = 332
    | OpGroupNonUniformElect = 333
    | OpGroupNonUniformAll = 334
    | OpGroupNonUniformAny = 335
    | OpGroupNonUniformAllEqual = 336
    | OpGroupNonUniformBroadcast = 337
    | OpGroupNonUniformBroadcastFirst = 338
    | OpGroupNonUniformBallot = 339
    | OpGroupNonUniformInverseBallot = 340
    | OpGroupNonUniformBallotBitExtract = 341
    | OpGroupNonUniformBallotBitCount = 342
    | OpGroupNonUniformBallotFindLSB = 343
    | OpGroupNonUniformBallotFindMSB = 344
    | OpGroupNonUniformShuffle = 345
    | OpGroupNonUniformShuffleXor = 346
    | OpGroupNonUniformShuffleUp = 347
    | OpGroupNonUniformShuffleDown = 348
    | OpGroupNonUniformIAdd = 349
    | OpGroupNonUniformFAdd = 350
    | OpGroupNonUniformIMul = 351
    | OpGroupNonUniformFMul = 352
    | OpGroupNonUniformSMin = 353
    | OpGroupNonUniformUMin = 354
    | OpGroupNonUniformFMin = 355
    | OpGroupNonUniformSMax = 356
    | OpGroupNonUniformUMax = 357
    | OpGroupNonUniformFMax = 358
    | OpGroupNonUniformBitwiseAnd = 359
    | OpGroupNonUniformBitwiseOr = 360
    | OpGroupNonUniformBitwiseXor = 361
    | OpGroupNonUniformLogicalAnd = 362
    | OpGroupNonUniformLogicalOr = 363
    | OpGroupNonUniformLogicalXor = 364
    | OpGroupNonUniformQuadBroadcast = 365
    | OpGroupNonUniformQuadSwap = 366
    | OpCopyLogical = 400
    | OpPtrEqual = 401
    | OpPtrNotEqual = 402
    | OpPtrDiff = 403
    | OpSubgroupBallotKHR = 4421
    | OpSubgroupFirstInvocationKHR = 4422
    | OpSubgroupAllKHR = 4428
    | OpSubgroupAnyKHR = 4429
    | OpSubgroupAllEqualKHR = 4430
    | OpSubgroupReadInvocationKHR = 4432
    | OpGroupIAddNonUniformAMD = 5000
    | OpGroupFAddNonUniformAMD = 5001
    | OpGroupFMinNonUniformAMD = 5002
    | OpGroupUMinNonUniformAMD = 5003
    | OpGroupSMinNonUniformAMD = 5004
    | OpGroupFMaxNonUniformAMD = 5005
    | OpGroupUMaxNonUniformAMD = 5006
    | OpGroupSMaxNonUniformAMD = 5007
    | OpFragmentMaskFetchAMD = 5011
    | OpFragmentFetchAMD = 5012
    | OpReadClockKHR = 5056
    | OpImageSampleFootprintNV = 5283
    | OpGroupNonUniformPartitionNV = 5296
    | OpWritePackedPrimitiveIndices4x8NV = 5299
    | OpReportIntersectionNV = 5334
    | OpIgnoreIntersectionNV = 5335
    | OpTerminateRayNV = 5336
    | OpTraceNV = 5337
    | OpTypeAccelerationStructureNV = 5341
    | OpExecuteCallableNV = 5344
    | OpTypeCooperativeMatrixNV = 5358
    | OpCooperativeMatrixLoadNV = 5359
    | OpCooperativeMatrixStoreNV = 5360
    | OpCooperativeMatrixMulAddNV = 5361
    | OpCooperativeMatrixLengthNV = 5362
    | OpBeginInvocationInterlockEXT = 5364
    | OpEndInvocationInterlockEXT = 5365
    | OpDemoteToHelperInvocationEXT = 5380
    | OpIsHelperInvocationEXT = 5381
    | OpSubgroupShuffleINTEL = 5571
    | OpSubgroupShuffleDownINTEL = 5572
    | OpSubgroupShuffleUpINTEL = 5573
    | OpSubgroupShuffleXorINTEL = 5574
    | OpSubgroupBlockReadINTEL = 5575
    | OpSubgroupBlockWriteINTEL = 5576
    | OpSubgroupImageBlockReadINTEL = 5577
    | OpSubgroupImageBlockWriteINTEL = 5578
    | OpSubgroupImageMediaBlockReadINTEL = 5580
    | OpSubgroupImageMediaBlockWriteINTEL = 5581
    | OpUCountLeadingZerosINTEL = 5585
    | OpUCountTrailingZerosINTEL = 5586
    | OpAbsISubINTEL = 5587
    | OpAbsUSubINTEL = 5588
    | OpIAddSatINTEL = 5589
    | OpUAddSatINTEL = 5590
    | OpIAverageINTEL = 5591
    | OpUAverageINTEL = 5592
    | OpIAverageRoundedINTEL = 5593
    | OpUAverageRoundedINTEL = 5594
    | OpISubSatINTEL = 5595
    | OpUSubSatINTEL = 5596
    | OpIMul32x16INTEL = 5597
    | OpUMul32x16INTEL = 5598
    | OpDecorateString = 5632
    | OpDecorateStringGOOGLE = 5632
    | OpMemberDecorateString = 5633
    | OpMemberDecorateStringGOOGLE = 5633
    | OpVmeImageINTEL = 5699
    | OpTypeVmeImageINTEL = 5700
    | OpTypeAvcImePayloadINTEL = 5701
    | OpTypeAvcRefPayloadINTEL = 5702
    | OpTypeAvcSicPayloadINTEL = 5703
    | OpTypeAvcMcePayloadINTEL = 5704
    | OpTypeAvcMceResultINTEL = 5705
    | OpTypeAvcImeResultINTEL = 5706
    | OpTypeAvcImeResultSingleReferenceStreamoutINTEL = 5707
    | OpTypeAvcImeResultDualReferenceStreamoutINTEL = 5708
    | OpTypeAvcImeSingleReferenceStreaminINTEL = 5709
    | OpTypeAvcImeDualReferenceStreaminINTEL = 5710
    | OpTypeAvcRefResultINTEL = 5711
    | OpTypeAvcSicResultINTEL = 5712
    | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL = 5713
    | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL = 5714
    | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL = 5715
    | OpSubgroupAvcMceSetInterShapePenaltyINTEL = 5716
    | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL = 5717
    | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL = 5718
    | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL = 5719
    | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL = 5720
    | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL = 5721
    | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL = 5722
    | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL = 5723
    | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL = 5724
    | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL = 5725
    | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL = 5726
    | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL = 5727
    | OpSubgroupAvcMceSetAcOnlyHaarINTEL = 5728
    | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL = 5729
    | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL = 5730
    | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL = 5731
    | OpSubgroupAvcMceConvertToImePayloadINTEL = 5732
    | OpSubgroupAvcMceConvertToImeResultINTEL = 5733
    | OpSubgroupAvcMceConvertToRefPayloadINTEL = 5734
    | OpSubgroupAvcMceConvertToRefResultINTEL = 5735
    | OpSubgroupAvcMceConvertToSicPayloadINTEL = 5736
    | OpSubgroupAvcMceConvertToSicResultINTEL = 5737
    | OpSubgroupAvcMceGetMotionVectorsINTEL = 5738
    | OpSubgroupAvcMceGetInterDistortionsINTEL = 5739
    | OpSubgroupAvcMceGetBestInterDistortionsINTEL = 5740
    | OpSubgroupAvcMceGetInterMajorShapeINTEL = 5741
    | OpSubgroupAvcMceGetInterMinorShapeINTEL = 5742
    | OpSubgroupAvcMceGetInterDirectionsINTEL = 5743
    | OpSubgroupAvcMceGetInterMotionVectorCountINTEL = 5744
    | OpSubgroupAvcMceGetInterReferenceIdsINTEL = 5745
    | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL = 5746
    | OpSubgroupAvcImeInitializeINTEL = 5747
    | OpSubgroupAvcImeSetSingleReferenceINTEL = 5748
    | OpSubgroupAvcImeSetDualReferenceINTEL = 5749
    | OpSubgroupAvcImeRefWindowSizeINTEL = 5750
    | OpSubgroupAvcImeAdjustRefOffsetINTEL = 5751
    | OpSubgroupAvcImeConvertToMcePayloadINTEL = 5752
    | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL = 5753
    | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL = 5754
    | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL = 5755
    | OpSubgroupAvcImeSetWeightedSadINTEL = 5756
    | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL = 5757
    | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL = 5758
    | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL = 5759
    | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL = 5760
    | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL = 5761
    | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL = 5762
    | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL = 5763
    | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL = 5764
    | OpSubgroupAvcImeConvertToMceResultINTEL = 5765
    | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL = 5766
    | OpSubgroupAvcImeGetDualReferenceStreaminINTEL = 5767
    | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL = 5768
    | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL = 5769
    | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL = 5770
    | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL = 5771
    | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL = 5772
    | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL = 5773
    | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL = 5774
    | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL = 5775
    | OpSubgroupAvcImeGetBorderReachedINTEL = 5776
    | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL = 5777
    | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL = 5778
    | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL = 5779
    | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL = 5780
    | OpSubgroupAvcFmeInitializeINTEL = 5781
    | OpSubgroupAvcBmeInitializeINTEL = 5782
    | OpSubgroupAvcRefConvertToMcePayloadINTEL = 5783
    | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL = 5784
    | OpSubgroupAvcRefSetBilinearFilterEnableINTEL = 5785
    | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL = 5786
    | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL = 5787
    | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL = 5788
    | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL = 5789
    | OpSubgroupAvcRefConvertToMceResultINTEL = 5790
    | OpSubgroupAvcSicInitializeINTEL = 5791
    | OpSubgroupAvcSicConfigureSkcINTEL = 5792
    | OpSubgroupAvcSicConfigureIpeLumaINTEL = 5793
    | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL = 5794
    | OpSubgroupAvcSicGetMotionVectorMaskINTEL = 5795
    | OpSubgroupAvcSicConvertToMcePayloadINTEL = 5796
    | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL = 5797
    | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL = 5798
    | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL = 5799
    | OpSubgroupAvcSicSetBilinearFilterEnableINTEL = 5800
    | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL = 5801
    | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL = 5802
    | OpSubgroupAvcSicEvaluateIpeINTEL = 5803
    | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL = 5804
    | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL = 5805
    | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL = 5806
    | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL = 5807
    | OpSubgroupAvcSicConvertToMceResultINTEL = 5808
    | OpSubgroupAvcSicGetIpeLumaShapeINTEL = 5809
    | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL = 5810
    | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL = 5811
    | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL = 5812
    | OpSubgroupAvcSicGetIpeChromaModeINTEL = 5813
    | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL = 5814
    | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL = 5815
    | OpSubgroupAvcSicGetInterRawSadsINTEL = 5816

type Word = uint32

type Operand = Word

type id = Word

type Result_id = Word

type LiteralString = string

type LiteralNumber = Word list

type LiteralNumberLimitOne = Word

type Literal = Word list

[<RequireQualifiedAccess;NoEquality;NoComparison>]
type SPVInstr =
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

    | Source of SourceLanguage * version: LiteralNumberLimitOne * file: id * source: LiteralString
    /// 2 + variable words
    | SourceExtension of extension: LiteralString
    /// 3 + variable words
    | Name of target: id * name: LiteralString
    /// 4 + variable words
    | MemberName of type': id * member': LiteralNumberLimitOne * name: LiteralString
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

    | ExtInstImport of Result_id * name: LiteralString
    | MemoryModel of AddressingModel * MemoryModel
    | EntryPoint of ExecutionModel * entryPoint: id * name: LiteralString * interfaceIds: id list
    | ExecutionMode of entryPoint: id * ExecutionMode * LiteralNumber
    | Capability of Capability

    | ExecutionModeId of entryPoint: id * mode: ExecutionMode * id list

    | UnhandledOp of Op

[<NoEquality;NoComparison>]
type SPVModule = 
    private {
        magicNumber: uint32
        versionNumber: uint32
        genMagicNumber: uint32
        bound: uint32
        instrs: SPVInstr list
    }

    member x.MagicNumber = x.magicNumber

open System

module internal Unpickle =

    open FSharp.SpirV.UnpickleHelper

    let u_word = u_uint32

    let u_wordCount = u_uint16

    let u_id: Unpickle<id> = u_word

    let u_resultId: Unpickle<Result_id> = u_word

    let u_idOpt opPos wordCount : Unpickle<id option> =
        u_streamPosition >>= fun pos ->
            let length = ((int wordCount * sizeof<id>) - (int pos - int opPos)) / sizeof<id>
            if length > 0 then
                u_id |>>
                fun id -> Some id
            else
                u_return None

    let u_ids opPos wordCount : Unpickle<id list> =
        u_streamPosition >>= fun pos ->
            let length = ((int wordCount * sizeof<id>) - (int pos - int opPos)) / sizeof<id>
            if length > 0 then
                u_list length u_id
            else
                u_return []

    let u_literalNumber opPos wordCount : Unpickle<LiteralNumber> =
        u_streamPosition >>= fun pos ->
            let length = ((int wordCount * sizeof<Word>) - (int pos - int opPos)) / sizeof<Word>
            if length > 0 then
                u_list length u_word
            else
                u_return []

    let u_literalNumberLimitOne : Unpickle<LiteralNumberLimitOne> = u_word

    let u_literalString: Unpickle<LiteralString> =
        fun stream ->
            let position = stream.Position
            let mutable length = 0
            while not (u_byte stream = 0uy && length % sizeof<Word> = 0) do
                length <- length + 1
            stream.Seek position
            let bytes = Array.zeroCreate length
            for i = 0 to length - 1 do
                bytes.[i] <- u_byte stream

            // Padding
            u_word stream |> ignore

            Text.UTF8Encoding.UTF8.GetString(bytes)

    let u_literalStringOpt opPos wordCount =
        u_streamPosition >>= fun pos ->
            let length = ((int wordCount * sizeof<Word>) - (int pos - int opPos)) / sizeof<Word>
            if length > 0 then
                u_literalString |>>
                fun str -> Some str
            else
                u_return None

    let u_interface_ids opPos wordCount : Unpickle<id list> =
        u_streamPosition >>= fun pos ->
            let length = ((int wordCount * sizeof<Word>) - (int pos - int opPos)) / sizeof<Word>
            if length > 0 then
                u_list length u_id
            else
                u_return []           

    // Enums

    let u_op: Unpickle<Op> = 
        u_uint16
        |>> fun word -> 
            if Enum.GetName(typeof<Op>, int word) = null then
                failwith "Invalid op."
            LanguagePrimitives.EnumOfValue (int word)

    let u_sourceLanguage: Unpickle<SourceLanguage> = 
        u_word
        |>> fun word -> 
            if Enum.GetName(typeof<SourceLanguage>, int word) = null then
                failwith "Invalid source language."
            LanguagePrimitives.EnumOfValue (int word)

    let u_addressingModel =
        u_word
        |>> fun word ->
            if Enum.GetName(typeof<AddressingModel>, int word) = null then
                failwith "Invalid addressing model."
            LanguagePrimitives.EnumOfValue (int word)

    let u_memoryModel =
        u_word
        |>> fun word ->
            if Enum.GetName(typeof<MemoryModel>, int word) = null then
                failwith "Invalid memory model."
            LanguagePrimitives.EnumOfValue (int word)

    let u_executionModel =
        u_word
        |>> fun word ->
            if Enum.GetName(typeof<ExecutionModel>, int word) = null then
                failwith "Invalid execution model."
            LanguagePrimitives.EnumOfValue (int word)

    let u_executionMode =
        u_word
        |>> fun word ->
            if Enum.GetName(typeof<ExecutionMode>, int word) = null then
                failwith "Invalid execution mode."
            LanguagePrimitives.EnumOfValue (int word)

    let u_capability =
        u_word
        |>> fun word ->
            if Enum.GetName(typeof<Capability>, int word) = null then
                failwith "Invalid capability."
            LanguagePrimitives.EnumOfValue (int word)

    // Instruction

    let u_instr: Unpickle<SPVInstr> =
        u_streamPosition >>= fun opPos ->
            u_bpipe2 u_op u_wordCount <|
            fun op wordCount ->
                let remainingWordCount = int wordCount - 1

                match op with

                //| Op.OpSource ->
                //    u_bpipe3 u_sourceLanguage u_literalNumberLimitOne u_id <|
                //    fun sourceLanguage version fileOpt ->
                //        u_streamPosition >>= fun opPos2 ->
                //            u_literalString |>>
                //            fun sourceOpt ->
                //                SPVInstr.Source (sourceLanguage, version, fileOpt, sourceOpt)

                | Op.OpExtInstImport ->
                    u_pipe2 u_resultId u_literalString <|
                    fun resultId name ->
                        SPVInstr.ExtInstImport (resultId, name)

                | Op.OpMemoryModel ->
                    u_pipe2 u_addressingModel u_memoryModel <|
                    fun addressingModel memoryModel ->
                        SPVInstr.MemoryModel (addressingModel, memoryModel)

                | Op.OpEntryPoint ->
                    u_pipe4 u_executionModel u_id u_literalString (u_interface_ids opPos wordCount) <|
                    fun executionModel entryPoint name interfaceIds ->
                        SPVInstr.EntryPoint (executionModel, entryPoint, name, interfaceIds)

                | Op.OpExecutionMode ->
                    u_pipe3 u_id u_executionMode (u_literalNumber opPos wordCount) <|
                    fun id executionMode literalNumber ->
                        SPVInstr.ExecutionMode (id, executionMode, literalNumber)

                | Op.OpExecutionModeId ->
                    u_pipe3 u_id u_executionMode (u_ids opPos wordCount) <|
                    fun id executionMode ids ->
                        SPVInstr.ExecutionModeId (id, executionMode, ids)

                | Op.OpCapability ->
                    u_capability |>>
                    fun cap -> 
                        SPVInstr.Capability cap

                | _ ->
                    u_array remainingWordCount u_word |>> fun _ -> SPVInstr.UnhandledOp op

    let u_instrs (stream: ReadStream) =
        let xs = ResizeArray()
        while stream.Position < stream.Length do
            xs.Add(u_instr stream)
        xs |> List.ofSeq

    let u_module: Unpickle<SPVModule> =
        u_pipe6 u_word u_word u_word u_word u_word u_instrs <|
        fun magicNumber versionNumber genMagicNumber bound _reserved instrs ->
            {
                magicNumber = magicNumber
                versionNumber = versionNumber
                genMagicNumber = genMagicNumber
                bound = bound
                instrs = instrs
            }

open System
open System.IO

type SPVModule with

    static member Deserialize(stream: Stream) =
        let readStream = UnpickleHelper.ReadStream (stream)
        UnpickleHelper.u_run Unpickle.u_module readStream

[<RequireQualifiedAccess>]
module LittleEndian =

    let inline write8 (data: Span<byte>) offset value =
        data.[offset] <- byte value

    let inline write16 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)

    let inline write32 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)
        data.[offset + 2] <- byte (value >>> 16)
        data.[offset + 3] <- byte (value >>> 24)

    let inline write64 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)
        data.[offset + 2] <- byte (value >>> 16)
        data.[offset + 3] <- byte (value >>> 24)
        data.[offset + 4] <- byte (value >>> 32)
        data.[offset + 5] <- byte (value >>> 40)
        data.[offset + 6] <- byte (value >>> 48)
        data.[offset + 7] <- byte (value >>> 56)

    let inline read8 (data: ReadOnlySpan<byte>) offset =
        data.[offset]

    let inline read16 (data: ReadOnlySpan<byte>) offset =
        ( uint16 data.[offset]) |||
        ((uint16 data.[offset + 1]) <<< 8)

    let inline read32 (data: ReadOnlySpan<byte>) offset =
        ( uint32 data.[offset]) |||
        ((uint32 data.[offset + 1]) <<< 8) |||
        ((uint32 data.[offset + 2]) <<< 16) |||
        ((uint32 data.[offset + 3]) <<< 24)

    let inline read64 (data: ReadOnlySpan<byte>) offset =
        ( uint64 data.[offset]) |||
        ((uint64 data.[offset + 1]) <<< 8) |||
        ((uint64 data.[offset + 2]) <<< 16) |||
        ((uint64 data.[offset + 3]) <<< 24) |||
        ((uint64 data.[offset + 4]) <<< 32) |||
        ((uint64 data.[offset + 5]) <<< 40) |||
        ((uint64 data.[offset + 6]) <<< 48) |||
        ((uint64 data.[offset + 7]) <<< 56)

type SPVPicklerStream =
    {
        stream: Stream
        mutable remaining: int
        buffer128: byte []
        isReader: bool
    }

    member inline x.ReadOnlyBuffer len = ReadOnlySpan(x.buffer128, 0, len)

    member inline x.Buffer len = Span(x.buffer128, 0, len)

    member inline x.Position = int x.stream.Position

    member inline x.Seek (offset, origin) = x.stream.Seek (int64 offset, origin) |> ignore

    member x.Int (res: byref<int>) =
        if x.isReader then
            let buf = x.Buffer 4
            x.stream.Read buf |> ignore
            res <- int (LittleEndian.read32 (Span.op_Implicit buf) 0)
        else
            let buf = x.Buffer 4
            LittleEndian.write32 buf 0 res
            x.stream.Write (Span.op_Implicit buf)

    member x.UInt32 (res: byref<uint32>) =
        if x.isReader then
            let buf = x.Buffer 4
            x.stream.Read buf |> ignore
            res <- LittleEndian.read32 (Span.op_Implicit buf) 0
        else
            let buf = x.Buffer 4
            LittleEndian.write32 buf 0 res
            x.stream.Write (Span.op_Implicit buf)

    member x.Word (res: byref<Word>) =
        x.UInt32 &res

    member x.WordList (res: byref<Word list>) =
        if x.isReader then
            if x.remaining % sizeof<Word> <> 0 then
                failwith "Remaining not a multiple of four."

            let count = x.remaining / sizeof<Word>
            let arr = Array.zeroCreate count
            for i = 0 to count - 1 do
                x.Word &arr.[i]
            res <- arr |> List.ofArray
        else
            res
            |> List.iter (fun word ->
                let mutable word = word
                x.Word &word
            )

    member x.Id (res: byref<id>) =
        x.UInt32 &res

    member x.ResultId (res: byref<Result_id>) =
        x.UInt32 &res

    member x.LiteralString (res: byref<LiteralString>) =
        if x.isReader then
            let position = x.Position
            let mutable length = 0

            while not (x.stream.ReadByte() = 0 && length % sizeof<Word> = 0) do
                length <- length + 1

            x.Seek(position, SeekOrigin.Begin)

            let bytes = Array.zeroCreate length
            x.stream.Read(bytes, 0, bytes.Length) |> ignore

            // Padding
            x.Seek(4, SeekOrigin.Current)

            res <- Text.UTF8Encoding.UTF8.GetString(bytes)
        else
            let bytes = Text.UTF8Encoding.UTF8.GetBytes res
            let remainder = bytes.Length % sizeof<Word>

            for i = 0 to remainder - 1 do
                x.buffer128.[i] <- 0uy

            x.buffer128.[remainder + 0] <- 0uy
            x.buffer128.[remainder + 1] <- 0uy
            x.buffer128.[remainder + 2] <- 0uy
            x.buffer128.[remainder + 3] <- 0uy

            x.stream.Write(bytes, 0, bytes.Length)
            x.stream.Write(x.buffer128, 0, remainder + sizeof<Word>)

    member x.InterfaceIds (res: byref<id list>) =
        x.WordList &res

    member x.LiteralNumber (res: byref<LiteralNumber>) =
        x.WordList &res

    member x.Ids (res: byref<id list>) =
        x.WordList &res

type SPVPickler<'T> = 
    {
        read: SPVPicklerStream -> 'T
        write: SPVPicklerStream -> 'T -> unit
    }

module SPVPickler =

    let Id = 
        {
            read = (fun stream ->
                let mutable res = 0u
                stream.ResultId &res
                res
            )
            write = (fun stream res ->
                let mutable res = res
                stream.ResultId &res
            )
        }

    let ResultId = 
        {
            read = (fun stream ->
                let mutable res = 0u
                stream.ResultId &res
                res
            )
            write = (fun stream res ->
                let mutable res = res
                stream.ResultId &res
            )
        }

    let LiteralString =
        {
            read = (fun stream ->
                let mutable res = Unchecked.defaultof<string>
                stream.LiteralString &res
                res
            )
            write = (fun stream res ->
                let mutable res = res
                stream.LiteralString &res
            )
        }

    let LiteralNumber =
        {
            read = (fun stream ->
                let mutable res = Unchecked.defaultof<LiteralNumber>
                stream.LiteralNumber &res
                res
            )
            write = (fun stream res ->
                let mutable res = res
                stream.LiteralNumber &res
            )
        }

    let InterfaceIds =
        {
            read = (fun stream ->
                let mutable res = []
                stream.InterfaceIds &res
                res
            )
            write = (fun stream res ->
                let mutable res = res
                stream.InterfaceIds &res
            )
        }

    let Ids =
        {
            read = (fun stream ->
                let mutable res = []
                stream.Ids &res
                res
            )
            write = (fun stream res ->
                let mutable res = res
                stream.Ids &res
            )
        }

    let Enum<'T when 'T : enum<int>> =
        {
            read = (fun stream ->
                let mutable res = 0
                stream.Int &res
                LanguagePrimitives.EnumOfValue<int, 'T> res
            )
            write = (fun stream res ->
                let mutable res = LanguagePrimitives.EnumToValue res
                stream.Int &res
            )
        }

    let p1 (f: ('Arg1 -> 'T)) (v1: SPVPickler<'Arg1>) (g: 'T -> 'Arg1) : SPVPickler<'T> =
        {
            read = (fun stream ->
                f (v1.read stream)
            )
            write = (fun stream res ->
                let mutable arg1 = g res
                v1.write stream arg1
            )
        }

    let p2 (f: ('Arg1 * 'Arg2 -> 'T)) (v1: SPVPickler<'Arg1>) (v2: SPVPickler<'Arg2>) (g: 'T -> 'Arg1 * 'Arg2) : SPVPickler<'T> =
        {
            read = (fun stream ->
                f (v1.read stream, v2.read stream)
            )
            write = (fun stream res ->
                let mutable (arg1, arg2) = g res
                v1.write stream arg1
                v2.write stream arg2
            )
        }

    let p3 (f: ('Arg1 * 'Arg2 * 'Arg3 -> 'T)) (v1: SPVPickler<'Arg1>) (v2: SPVPickler<'Arg2>) (v3: SPVPickler<'Arg3>) (g: 'T -> 'Arg1 * 'Arg2 * 'Arg3) : SPVPickler<'T> =
        {
            read = (fun stream ->
                f (v1.read stream, v2.read stream, v3.read stream)
            )
            write = (fun stream res ->
                let mutable (arg1, arg2, arg3) = g res
                v1.write stream arg1
                v2.write stream arg2
                v3.write stream arg3
            )
        }

    let p4 (f: ('Arg1 * 'Arg2 * 'Arg3 * 'Arg4 -> 'T)) (v1: SPVPickler<'Arg1>) (v2: SPVPickler<'Arg2>) (v3: SPVPickler<'Arg3>) (v4: SPVPickler<'Arg4>) (g: 'T -> 'Arg1 * 'Arg2 * 'Arg3 * 'Arg4) : SPVPickler<'T> =
        {
            read = (fun stream ->
                f (v1.read stream, v2.read stream, v3.read stream, v4.read stream)
            )
            write = (fun stream res ->
                let mutable (arg1, arg2, arg3, arg4) = g res
                v1.write stream arg1
                v2.write stream arg2
                v3.write stream arg3
                v4.write stream arg4
            )
        }

    let instrsLooup =
        [
            Op.OpExtInstImport,   p2 SPVInstr.ExtInstImport ResultId LiteralString                          (function SPVInstr.ExtInstImport (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
            Op.OpMemoryModel,     p2 SPVInstr.MemoryModel Enum<AddressingModel> Enum<MemoryModel>           (function SPVInstr.MemoryModel (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
            Op.OpEntryPoint,      p4 SPVInstr.EntryPoint Enum<ExecutionModel> Id LiteralString InterfaceIds (function SPVInstr.EntryPoint (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
            Op.OpExecutionMode,   p3 SPVInstr.ExecutionMode Id Enum<ExecutionMode> LiteralNumber            (function SPVInstr.ExecutionMode (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
            Op.OpExecutionModeId, p3 SPVInstr.ExecutionModeId Id Enum<ExecutionMode> Ids                    (function SPVInstr.ExecutionModeId (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
            Op.OpCapability,      p1 SPVInstr.Capability Enum<Capability>                                   (function SPVInstr.Capability arg1 -> arg1 | _ -> failwith "invalid")
        ]
        |> Map.ofList

    let Instruction =
        {
            read = (fun stream ->
            // TODO:
            )
            write = (fun stream res -> ()
            )
        }

    let Instructions =
        {
            read = (fun stream ->
                let xs = ResizeArray()
                while stream.stream.Position < stream.stream.Length do
                    xs.Add(Instruction.read stream)
                xs |> List.ofSeq
            )
            write = (fun stream res -> ()
            )
        }

    let Module =
        {
            read = (fun stream ->
                let mutable magicNumber = 0u
                let mutable versionNumber = 0u
                let mutable genMagicNumber = 0u
                let mutable bound = 0u

                stream.Word &magicNumber
                stream.Word &versionNumber
                stream.Word &genMagicNumber
                stream.Word &bound

                {
                    magicNumber = magicNumber
                    versionNumber = versionNumber
                    genMagicNumber = genMagicNumber
                    bound = bound
                    instrs = []
                }
            )
            write = (fun stream res -> ()
            )
        }
//let u_instr: Unpickle<SPVInstr> =
//    u_streamPosition >>= fun opPos ->
//        u_bpipe2 u_op u_wordCount <|
//        fun op wordCount ->
//            let remainingWordCount = int wordCount - 1
//            | _ ->
//                u_array remainingWordCount u_word |>> fun _ -> SPVInstr.UnhandledOp op

//let u_instrs (stream: ReadStream) =
//    let xs = ResizeArray()
//    while stream.Position < stream.Length do
//        xs.Add(u_instr stream)
//    xs |> List.ofSeq


type SPVModule with

    static member Deserialize2(stream: Stream) =
        let readStream = UnpickleHelper.ReadStream (stream)
        UnpickleHelper.u_run Unpickle.u_module readStream

    static member Serialize(spvModule: SPVModule, stream: Stream) =
        stream.Position <- 0L
        //let stream =
        //    {
        //        stream = stream
        //        remaining = 0
        //        buffer = Array.zeroCreate 128
        //        isReader = false
        //    }