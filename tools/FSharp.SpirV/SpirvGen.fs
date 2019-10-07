// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System

type ImageOperands =
   | None = 0x0000
   | Bias = 0x0001
   | Lod = 0x0002
   | Grad = 0x0004
   | ConstOffset = 0x0008
   | Offset = 0x0010
   | ConstOffsets = 0x0020
   | Sample = 0x0040
   | MinLod = 0x0080
   | MakeTexelAvailable = 0x0100
   | MakeTexelAvailableKHR = 0x0100
   | MakeTexelVisible = 0x0200
   | MakeTexelVisibleKHR = 0x0200
   | NonPrivateTexel = 0x0400
   | NonPrivateTexelKHR = 0x0400
   | VolatileTexel = 0x0800
   | VolatileTexelKHR = 0x0800
   | SignExtend = 0x1000
   | ZeroExtend = 0x2000

type FPFastMathMode =
   | None = 0x0000
   | NotNaN = 0x0001
   | NotInf = 0x0002
   | NSZ = 0x0004
   | AllowRecip = 0x0008
   | Fast = 0x0010

type SelectionControl =
   | None = 0x0000
   | Flatten = 0x0001
   | DontFlatten = 0x0002

type LoopControl =
   | None = 0x0000
   | Unroll = 0x0001
   | DontUnroll = 0x0002
   | DependencyInfinite = 0x0004
   | DependencyLength = 0x0008
   | MinIterations = 0x0010
   | MaxIterations = 0x0020
   | IterationMultiple = 0x0040
   | PeelCount = 0x0080
   | PartialCount = 0x0100

type FunctionControl =
   | None = 0x0000
   | Inline = 0x0001
   | DontInline = 0x0002
   | Pure = 0x0004
   | Const = 0x0008

type MemorySemantics =
   | Relaxed = 0x0000
   | None = 0x0000
   | Acquire = 0x0002
   | Release = 0x0004
   | AcquireRelease = 0x0008
   | SequentiallyConsistent = 0x0010
   | UniformMemory = 0x0040
   | SubgroupMemory = 0x0080
   | WorkgroupMemory = 0x0100
   | CrossWorkgroupMemory = 0x0200
   | AtomicCounterMemory = 0x0400
   | ImageMemory = 0x0800
   | OutputMemory = 0x1000
   | OutputMemoryKHR = 0x1000
   | MakeAvailable = 0x2000
   | MakeAvailableKHR = 0x2000
   | MakeVisible = 0x4000
   | MakeVisibleKHR = 0x4000
   | Volatile = 0x8000

type MemoryAccess =
   | None = 0x0000
   | Volatile = 0x0001
   | Aligned = 0x0002
   | Nontemporal = 0x0004
   | MakePointerAvailable = 0x0008
   | MakePointerAvailableKHR = 0x0008
   | MakePointerVisible = 0x0010
   | MakePointerVisibleKHR = 0x0010
   | NonPrivatePointer = 0x0020
   | NonPrivatePointerKHR = 0x0020

type KernelProfilingInfo =
   | None = 0x0000
   | CmdExecTime = 0x0001

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
   | One = 0
   | Two = 1
   | Three = 2
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
   | UserSemantic = 5635
   | HlslSemanticGOOGLE = 5635
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
   | SubgroupGeMask = 4417
   | SubgroupGtMask = 4418
   | SubgroupLeMask = 4419
   | SubgroupLtMask = 4420
   | SubgroupEqMaskKHR = 4416
   | SubgroupGeMaskKHR = 4417
   | SubgroupGtMaskKHR = 4418
   | SubgroupLeMaskKHR = 4419
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
   | UniformAndStorageBuffer16BitAccess = 4434
   | StorageUniform16 = 4434
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

/// Reference to an <id> representing the result's type of the enclosing instruction
type IdResultType = uint32

/// Definition of an <id> representing the result of the enclosing instruction
type IdResult = uint32

/// Reference to an <id> representing a 32-bit integer that is a mask from the MemorySemantics operand kind
type IdMemorySemantics = uint32

/// Reference to an <id> representing a 32-bit integer that is a mask from the Scope operand kind
type IdScope = uint32

/// Reference to an <id>
type IdRef = uint32

/// An integer consuming one or more words
type LiteralInteger = uint32 list

/// A null-terminated stream of characters consuming an integral number of words
type LiteralString = string

/// A literal number whose size and format are determined by a previous operand in the enclosing instruction
type LiteralContextDependentNumber = uint32 list

/// A 32-bit unsigned integer indicating which instruction to use and determining the layout of following operands (for OpExtInst)
type LiteralExtInstInteger = uint32

/// An opcode indicating the operation to be performed and determining the layout of following operands (for OpSpecConstantOp)
type LiteralSpecConstantOpInteger = uint32

type PairLiteralIntegerIdRef = PairLiteralIntegerIdRef of LiteralInteger * IdRef

type PairIdRefLiteralInteger = PairIdRefLiteralInteger of IdRef * LiteralInteger

type PairIdRefIdRef = PairIdRefIdRef of IdRef * IdRef

type Instruction =
   | OpNop
   | OpUndef of IdResultType * IdResult
   | OpSourceContinued of LiteralString
   | OpSource of SourceLanguage * LiteralInteger * IdRef * LiteralString
   | OpSourceExtension of LiteralString
   | OpName of IdRef * LiteralString
   | OpMemberName of IdRef * LiteralInteger * LiteralString
   | OpString of IdResult * LiteralString
   | OpLine of IdRef * LiteralInteger * LiteralInteger
   | OpExtension of LiteralString
   | OpExtInstImport of IdResult * LiteralString
   | OpExtInst of IdResultType * IdResult * IdRef * LiteralExtInstInteger * IdRef
   | OpMemoryModel of AddressingModel * MemoryModel
   | OpEntryPoint of ExecutionModel * IdRef * LiteralString * IdRef
   | OpExecutionMode of IdRef * ExecutionMode
   | OpCapability of Capability
   | OpTypeVoid of IdResult
   | OpTypeBool of IdResult
   | OpTypeInt of IdResult * LiteralInteger * LiteralInteger
   | OpTypeFloat of IdResult * LiteralInteger
   | OpTypeVector of IdResult * IdRef * LiteralInteger
   | OpTypeMatrix of IdResult * IdRef * LiteralInteger
   | OpTypeImage of IdResult * IdRef * Dim * LiteralInteger * LiteralInteger * LiteralInteger * LiteralInteger * ImageFormat * AccessQualifier
   | OpTypeSampler of IdResult
   | OpTypeSampledImage of IdResult * IdRef
   | OpTypeArray of IdResult * IdRef * IdRef
   | OpTypeRuntimeArray of IdResult * IdRef
   | OpTypeStruct of IdResult * IdRef
   | OpTypeOpaque of IdResult * LiteralString
   | OpTypePointer of IdResult * StorageClass * IdRef
   | OpTypeFunction of IdResult * IdRef * IdRef
   | OpTypeEvent of IdResult
   | OpTypeDeviceEvent of IdResult
   | OpTypeReserveId of IdResult
   | OpTypeQueue of IdResult
   | OpTypePipe of IdResult * AccessQualifier
   | OpTypeForwardPointer of IdRef * StorageClass
   | OpConstantTrue of IdResultType * IdResult
   | OpConstantFalse of IdResultType * IdResult
   | OpConstant of IdResultType * IdResult * LiteralContextDependentNumber
   | OpConstantComposite of IdResultType * IdResult * IdRef
   | OpConstantSampler of IdResultType * IdResult * SamplerAddressingMode * LiteralInteger * SamplerFilterMode
   | OpConstantNull of IdResultType * IdResult
   | OpSpecConstantTrue of IdResultType * IdResult
   | OpSpecConstantFalse of IdResultType * IdResult
   | OpSpecConstant of IdResultType * IdResult * LiteralContextDependentNumber
   | OpSpecConstantComposite of IdResultType * IdResult * IdRef
   | OpSpecConstantOp of IdResultType * IdResult * LiteralSpecConstantOpInteger
   | OpFunction of IdResultType * IdResult * FunctionControl * IdRef
   | OpFunctionParameter of IdResultType * IdResult
   | OpFunctionEnd
   | OpFunctionCall of IdResultType * IdResult * IdRef * IdRef
   | OpVariable of IdResultType * IdResult * StorageClass * IdRef
   | OpImageTexelPointer of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpLoad of IdResultType * IdResult * IdRef * MemoryAccess
   | OpStore of IdRef * IdRef * MemoryAccess
   | OpCopyMemory of IdRef * IdRef * MemoryAccess * MemoryAccess
   | OpCopyMemorySized of IdRef * IdRef * IdRef * MemoryAccess * MemoryAccess
   | OpAccessChain of IdResultType * IdResult * IdRef * IdRef
   | OpInBoundsAccessChain of IdResultType * IdResult * IdRef * IdRef
   | OpPtrAccessChain of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpArrayLength of IdResultType * IdResult * IdRef * LiteralInteger
   | OpGenericPtrMemSemantics of IdResultType * IdResult * IdRef
   | OpInBoundsPtrAccessChain of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpDecorate of IdRef * Decoration
   | OpMemberDecorate of IdRef * LiteralInteger * Decoration
   | OpDecorationGroup of IdResult
   | OpGroupDecorate of IdRef * IdRef
   | OpGroupMemberDecorate of IdRef * PairIdRefLiteralInteger
   | OpVectorExtractDynamic of IdResultType * IdResult * IdRef * IdRef
   | OpVectorInsertDynamic of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpVectorShuffle of IdResultType * IdResult * IdRef * IdRef * LiteralInteger
   | OpCompositeConstruct of IdResultType * IdResult * IdRef
   | OpCompositeExtract of IdResultType * IdResult * IdRef * LiteralInteger
   | OpCompositeInsert of IdResultType * IdResult * IdRef * IdRef * LiteralInteger
   | OpCopyObject of IdResultType * IdResult * IdRef
   | OpTranspose of IdResultType * IdResult * IdRef
   | OpSampledImage of IdResultType * IdResult * IdRef * IdRef
   | OpImageSampleImplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSampleExplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSampleDrefImplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSampleDrefExplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSampleProjImplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSampleProjExplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSampleProjDrefImplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSampleProjDrefExplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageFetch of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageGather of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageDrefGather of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageRead of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageWrite of IdRef * IdRef * IdRef * ImageOperands
   | OpImage of IdResultType * IdResult * IdRef
   | OpImageQueryFormat of IdResultType * IdResult * IdRef
   | OpImageQueryOrder of IdResultType * IdResult * IdRef
   | OpImageQuerySizeLod of IdResultType * IdResult * IdRef * IdRef
   | OpImageQuerySize of IdResultType * IdResult * IdRef
   | OpImageQueryLod of IdResultType * IdResult * IdRef * IdRef
   | OpImageQueryLevels of IdResultType * IdResult * IdRef
   | OpImageQuerySamples of IdResultType * IdResult * IdRef
   | OpConvertFToU of IdResultType * IdResult * IdRef
   | OpConvertFToS of IdResultType * IdResult * IdRef
   | OpConvertSToF of IdResultType * IdResult * IdRef
   | OpConvertUToF of IdResultType * IdResult * IdRef
   | OpUConvert of IdResultType * IdResult * IdRef
   | OpSConvert of IdResultType * IdResult * IdRef
   | OpFConvert of IdResultType * IdResult * IdRef
   | OpQuantizeToF16 of IdResultType * IdResult * IdRef
   | OpConvertPtrToU of IdResultType * IdResult * IdRef
   | OpSatConvertSToU of IdResultType * IdResult * IdRef
   | OpSatConvertUToS of IdResultType * IdResult * IdRef
   | OpConvertUToPtr of IdResultType * IdResult * IdRef
   | OpPtrCastToGeneric of IdResultType * IdResult * IdRef
   | OpGenericCastToPtr of IdResultType * IdResult * IdRef
   | OpGenericCastToPtrExplicit of IdResultType * IdResult * IdRef * StorageClass
   | OpBitcast of IdResultType * IdResult * IdRef
   | OpSNegate of IdResultType * IdResult * IdRef
   | OpFNegate of IdResultType * IdResult * IdRef
   | OpIAdd of IdResultType * IdResult * IdRef * IdRef
   | OpFAdd of IdResultType * IdResult * IdRef * IdRef
   | OpISub of IdResultType * IdResult * IdRef * IdRef
   | OpFSub of IdResultType * IdResult * IdRef * IdRef
   | OpIMul of IdResultType * IdResult * IdRef * IdRef
   | OpFMul of IdResultType * IdResult * IdRef * IdRef
   | OpUDiv of IdResultType * IdResult * IdRef * IdRef
   | OpSDiv of IdResultType * IdResult * IdRef * IdRef
   | OpFDiv of IdResultType * IdResult * IdRef * IdRef
   | OpUMod of IdResultType * IdResult * IdRef * IdRef
   | OpSRem of IdResultType * IdResult * IdRef * IdRef
   | OpSMod of IdResultType * IdResult * IdRef * IdRef
   | OpFRem of IdResultType * IdResult * IdRef * IdRef
   | OpFMod of IdResultType * IdResult * IdRef * IdRef
   | OpVectorTimesScalar of IdResultType * IdResult * IdRef * IdRef
   | OpMatrixTimesScalar of IdResultType * IdResult * IdRef * IdRef
   | OpVectorTimesMatrix of IdResultType * IdResult * IdRef * IdRef
   | OpMatrixTimesVector of IdResultType * IdResult * IdRef * IdRef
   | OpMatrixTimesMatrix of IdResultType * IdResult * IdRef * IdRef
   | OpOuterProduct of IdResultType * IdResult * IdRef * IdRef
   | OpDot of IdResultType * IdResult * IdRef * IdRef
   | OpIAddCarry of IdResultType * IdResult * IdRef * IdRef
   | OpISubBorrow of IdResultType * IdResult * IdRef * IdRef
   | OpUMulExtended of IdResultType * IdResult * IdRef * IdRef
   | OpSMulExtended of IdResultType * IdResult * IdRef * IdRef
   | OpAny of IdResultType * IdResult * IdRef
   | OpAll of IdResultType * IdResult * IdRef
   | OpIsNan of IdResultType * IdResult * IdRef
   | OpIsInf of IdResultType * IdResult * IdRef
   | OpIsFinite of IdResultType * IdResult * IdRef
   | OpIsNormal of IdResultType * IdResult * IdRef
   | OpSignBitSet of IdResultType * IdResult * IdRef
   | OpLessOrGreater of IdResultType * IdResult * IdRef * IdRef
   | OpOrdered of IdResultType * IdResult * IdRef * IdRef
   | OpUnordered of IdResultType * IdResult * IdRef * IdRef
   | OpLogicalEqual of IdResultType * IdResult * IdRef * IdRef
   | OpLogicalNotEqual of IdResultType * IdResult * IdRef * IdRef
   | OpLogicalOr of IdResultType * IdResult * IdRef * IdRef
   | OpLogicalAnd of IdResultType * IdResult * IdRef * IdRef
   | OpLogicalNot of IdResultType * IdResult * IdRef
   | OpSelect of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpIEqual of IdResultType * IdResult * IdRef * IdRef
   | OpINotEqual of IdResultType * IdResult * IdRef * IdRef
   | OpUGreaterThan of IdResultType * IdResult * IdRef * IdRef
   | OpSGreaterThan of IdResultType * IdResult * IdRef * IdRef
   | OpUGreaterThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpSGreaterThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpULessThan of IdResultType * IdResult * IdRef * IdRef
   | OpSLessThan of IdResultType * IdResult * IdRef * IdRef
   | OpULessThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpSLessThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFOrdEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFUnordEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFOrdNotEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFUnordNotEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFOrdLessThan of IdResultType * IdResult * IdRef * IdRef
   | OpFUnordLessThan of IdResultType * IdResult * IdRef * IdRef
   | OpFOrdGreaterThan of IdResultType * IdResult * IdRef * IdRef
   | OpFUnordGreaterThan of IdResultType * IdResult * IdRef * IdRef
   | OpFOrdLessThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFUnordLessThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFOrdGreaterThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpFUnordGreaterThanEqual of IdResultType * IdResult * IdRef * IdRef
   | OpShiftRightLogical of IdResultType * IdResult * IdRef * IdRef
   | OpShiftRightArithmetic of IdResultType * IdResult * IdRef * IdRef
   | OpShiftLeftLogical of IdResultType * IdResult * IdRef * IdRef
   | OpBitwiseOr of IdResultType * IdResult * IdRef * IdRef
   | OpBitwiseXor of IdResultType * IdResult * IdRef * IdRef
   | OpBitwiseAnd of IdResultType * IdResult * IdRef * IdRef
   | OpNot of IdResultType * IdResult * IdRef
   | OpBitFieldInsert of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpBitFieldSExtract of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpBitFieldUExtract of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpBitReverse of IdResultType * IdResult * IdRef
   | OpBitCount of IdResultType * IdResult * IdRef
   | OpDPdx of IdResultType * IdResult * IdRef
   | OpDPdy of IdResultType * IdResult * IdRef
   | OpFwidth of IdResultType * IdResult * IdRef
   | OpDPdxFine of IdResultType * IdResult * IdRef
   | OpDPdyFine of IdResultType * IdResult * IdRef
   | OpFwidthFine of IdResultType * IdResult * IdRef
   | OpDPdxCoarse of IdResultType * IdResult * IdRef
   | OpDPdyCoarse of IdResultType * IdResult * IdRef
   | OpFwidthCoarse of IdResultType * IdResult * IdRef
   | OpEmitVertex
   | OpEndPrimitive
   | OpEmitStreamVertex of IdRef
   | OpEndStreamPrimitive of IdRef
   | OpControlBarrier of IdScope * IdScope * IdMemorySemantics
   | OpMemoryBarrier of IdScope * IdMemorySemantics
   | OpAtomicLoad of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics
   | OpAtomicStore of IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicExchange of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicCompareExchange of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdMemorySemantics * IdRef * IdRef
   | OpAtomicCompareExchangeWeak of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdMemorySemantics * IdRef * IdRef
   | OpAtomicIIncrement of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics
   | OpAtomicIDecrement of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics
   | OpAtomicIAdd of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicISub of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicSMin of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicUMin of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicSMax of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicUMax of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicAnd of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicOr of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpAtomicXor of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics * IdRef
   | OpPhi of IdResultType * IdResult * PairIdRefIdRef
   | OpLoopMerge of IdRef * IdRef * LoopControl
   | OpSelectionMerge of IdRef * SelectionControl
   | OpLabel of IdResult
   | OpBranch of IdRef
   | OpBranchConditional of IdRef * IdRef * IdRef * LiteralInteger
   | OpSwitch of IdRef * IdRef * PairLiteralIntegerIdRef
   | OpKill
   | OpReturn
   | OpReturnValue of IdRef
   | OpUnreachable
   | OpLifetimeStart of IdRef * LiteralInteger
   | OpLifetimeStop of IdRef * LiteralInteger
   | OpGroupAsyncCopy of IdResultType * IdResult * IdScope * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpGroupWaitEvents of IdScope * IdRef * IdRef
   | OpGroupAll of IdResultType * IdResult * IdScope * IdRef
   | OpGroupAny of IdResultType * IdResult * IdScope * IdRef
   | OpGroupBroadcast of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupIAdd of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupFAdd of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupFMin of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupUMin of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupSMin of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupFMax of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupUMax of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupSMax of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpReadPipe of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpWritePipe of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpReservedReadPipe of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpReservedWritePipe of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpReserveReadPipePackets of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpReserveWritePipePackets of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpCommitReadPipe of IdRef * IdRef * IdRef * IdRef
   | OpCommitWritePipe of IdRef * IdRef * IdRef * IdRef
   | OpIsValidReserveId of IdResultType * IdResult * IdRef
   | OpGetNumPipePackets of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpGetMaxPipePackets of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpGroupReserveReadPipePackets of IdResultType * IdResult * IdScope * IdRef * IdRef * IdRef * IdRef
   | OpGroupReserveWritePipePackets of IdResultType * IdResult * IdScope * IdRef * IdRef * IdRef * IdRef
   | OpGroupCommitReadPipe of IdScope * IdRef * IdRef * IdRef * IdRef
   | OpGroupCommitWritePipe of IdScope * IdRef * IdRef * IdRef * IdRef
   | OpEnqueueMarker of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpEnqueueKernel of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpGetKernelNDrangeSubGroupCount of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpGetKernelNDrangeMaxSubGroupSize of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpGetKernelWorkGroupSize of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpGetKernelPreferredWorkGroupSizeMultiple of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpRetainEvent of IdRef
   | OpReleaseEvent of IdRef
   | OpCreateUserEvent of IdResultType * IdResult
   | OpIsValidEvent of IdResultType * IdResult * IdRef
   | OpSetUserEventStatus of IdRef * IdRef
   | OpCaptureEventProfilingInfo of IdRef * IdRef * IdRef
   | OpGetDefaultQueue of IdResultType * IdResult
   | OpBuildNDRange of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpImageSparseSampleImplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleExplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleDrefImplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleDrefExplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleProjImplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleProjExplicitLod of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleProjDrefImplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSparseSampleProjDrefExplicitLod of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSparseFetch of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpImageSparseGather of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSparseDrefGather of IdResultType * IdResult * IdRef * IdRef * IdRef * ImageOperands
   | OpImageSparseTexelsResident of IdResultType * IdResult * IdRef
   | OpNoLine
   | OpAtomicFlagTestAndSet of IdResultType * IdResult * IdRef * IdScope * IdMemorySemantics
   | OpAtomicFlagClear of IdRef * IdScope * IdMemorySemantics
   | OpImageSparseRead of IdResultType * IdResult * IdRef * IdRef * ImageOperands
   | OpSizeOf of IdResultType * IdResult * IdRef
   | OpTypePipeStorage of IdResult
   | OpConstantPipeStorage of IdResultType * IdResult * LiteralInteger * LiteralInteger * LiteralInteger
   | OpCreatePipeFromPipeStorage of IdResultType * IdResult * IdRef
   | OpGetKernelLocalSizeForSubgroupCount of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpGetKernelMaxNumSubgroups of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpTypeNamedBarrier of IdResult
   | OpNamedBarrierInitialize of IdResultType * IdResult * IdRef
   | OpMemoryNamedBarrier of IdRef * IdScope * IdMemorySemantics
   | OpModuleProcessed of LiteralString
   | OpExecutionModeId of IdRef * ExecutionMode
   | OpDecorateId of IdRef * Decoration
   | OpGroupNonUniformElect of IdResultType * IdResult * IdScope
   | OpGroupNonUniformAll of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformAny of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformAllEqual of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformBroadcast of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformBroadcastFirst of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformBallot of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformInverseBallot of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformBallotBitExtract of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformBallotBitCount of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupNonUniformBallotFindLSB of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformBallotFindMSB of IdResultType * IdResult * IdScope * IdRef
   | OpGroupNonUniformShuffle of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformShuffleXor of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformShuffleUp of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformShuffleDown of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformIAdd of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformFAdd of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformIMul of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformFMul of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformSMin of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformUMin of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformFMin of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformSMax of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformUMax of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformFMax of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformBitwiseAnd of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformBitwiseOr of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformBitwiseXor of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformLogicalAnd of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformLogicalOr of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformLogicalXor of IdResultType * IdResult * IdScope * GroupOperation * IdRef * IdRef
   | OpGroupNonUniformQuadBroadcast of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpGroupNonUniformQuadSwap of IdResultType * IdResult * IdScope * IdRef * IdRef
   | OpCopyLogical of IdResultType * IdResult * IdRef
   | OpPtrEqual of IdResultType * IdResult * IdRef * IdRef
   | OpPtrNotEqual of IdResultType * IdResult * IdRef * IdRef
   | OpPtrDiff of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupBallotKHR of IdResultType * IdResult * IdRef
   | OpSubgroupFirstInvocationKHR of IdResultType * IdResult * IdRef
   | OpSubgroupAllKHR of IdResultType * IdResult * IdRef
   | OpSubgroupAnyKHR of IdResultType * IdResult * IdRef
   | OpSubgroupAllEqualKHR of IdResultType * IdResult * IdRef
   | OpSubgroupReadInvocationKHR of IdResultType * IdResult * IdRef * IdRef
   | OpGroupIAddNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupFAddNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupFMinNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupUMinNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupSMinNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupFMaxNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupUMaxNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpGroupSMaxNonUniformAMD of IdResultType * IdResult * IdScope * GroupOperation * IdRef
   | OpFragmentMaskFetchAMD of IdResultType * IdResult * IdRef * IdRef
   | OpFragmentFetchAMD of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpReadClockKHR of IdResultType * IdResult * IdScope
   | OpImageSampleFootprintNV of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * ImageOperands
   | OpGroupNonUniformPartitionNV of IdResultType * IdResult * IdRef
   | OpWritePackedPrimitiveIndices4x8NV of IdRef * IdRef
   | OpReportIntersectionNV of IdResultType * IdResult * IdRef * IdRef
   | OpIgnoreIntersectionNV
   | OpTerminateRayNV
   | OpTraceNV of IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpTypeAccelerationStructureNV of IdResult
   | OpExecuteCallableNV of IdRef * IdRef
   | OpTypeCooperativeMatrixNV of IdResult * IdRef * IdScope * IdRef * IdRef
   | OpCooperativeMatrixLoadNV of IdResultType * IdResult * IdRef * IdRef * IdRef * MemoryAccess
   | OpCooperativeMatrixStoreNV of IdRef * IdRef * IdRef * IdRef * MemoryAccess
   | OpCooperativeMatrixMulAddNV of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpCooperativeMatrixLengthNV of IdResultType * IdResult * IdRef
   | OpBeginInvocationInterlockEXT
   | OpEndInvocationInterlockEXT
   | OpDemoteToHelperInvocationEXT
   | OpIsHelperInvocationEXT of IdResultType * IdResult
   | OpSubgroupShuffleINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupShuffleDownINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupShuffleUpINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupShuffleXorINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupBlockReadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupBlockWriteINTEL of IdRef * IdRef
   | OpSubgroupImageBlockReadINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupImageBlockWriteINTEL of IdRef * IdRef * IdRef
   | OpSubgroupImageMediaBlockReadINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupImageMediaBlockWriteINTEL of IdRef * IdRef * IdRef * IdRef * IdRef
   | OpUCountLeadingZerosINTEL of IdResultType * IdResult * IdRef
   | OpUCountTrailingZerosINTEL of IdResultType * IdResult * IdRef
   | OpAbsISubINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpAbsUSubINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpIAddSatINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpUAddSatINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpIAverageINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpUAverageINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpIAverageRoundedINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpUAverageRoundedINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpISubSatINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpUSubSatINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpIMul32x16INTEL of IdResultType * IdResult * IdRef * IdRef
   | OpUMul32x16INTEL of IdResultType * IdResult * IdRef * IdRef
   | OpDecorateString of IdRef * Decoration
   | OpDecorateStringGOOGLE of IdRef * Decoration
   | OpMemberDecorateString of IdRef * LiteralInteger * Decoration
   | OpMemberDecorateStringGOOGLE of IdRef * LiteralInteger * Decoration
   | OpVmeImageINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpTypeVmeImageINTEL of IdResult * IdRef
   | OpTypeAvcImePayloadINTEL of IdResult
   | OpTypeAvcRefPayloadINTEL of IdResult
   | OpTypeAvcSicPayloadINTEL of IdResult
   | OpTypeAvcMcePayloadINTEL of IdResult
   | OpTypeAvcMceResultINTEL of IdResult
   | OpTypeAvcImeResultINTEL of IdResult
   | OpTypeAvcImeResultSingleReferenceStreamoutINTEL of IdResult
   | OpTypeAvcImeResultDualReferenceStreamoutINTEL of IdResult
   | OpTypeAvcImeSingleReferenceStreaminINTEL of IdResult
   | OpTypeAvcImeDualReferenceStreaminINTEL of IdResult
   | OpTypeAvcRefResultINTEL of IdResult
   | OpTypeAvcSicResultINTEL of IdResult
   | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceSetInterShapePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceSetAcOnlyHaarINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcMceConvertToImePayloadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceConvertToImeResultINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceConvertToRefPayloadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceConvertToRefResultINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceConvertToSicPayloadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceConvertToSicResultINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetMotionVectorsINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterDistortionsINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetBestInterDistortionsINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterMajorShapeINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterMinorShapeINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterDirectionsINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterMotionVectorCountINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterReferenceIdsINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeInitializeINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeSetSingleReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeSetDualReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeRefWindowSizeINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeAdjustRefOffsetINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeConvertToMcePayloadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeSetWeightedSadINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeConvertToMceResultINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeGetDualReferenceStreaminINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcImeGetBorderReachedINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcFmeInitializeINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcBmeInitializeINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcRefConvertToMcePayloadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcRefSetBilinearFilterEnableINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcRefConvertToMceResultINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicInitializeINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicConfigureSkcINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicConfigureIpeLumaINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicGetMotionVectorMaskINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcSicConvertToMcePayloadINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcSicSetBilinearFilterEnableINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcSicEvaluateIpeINTEL of IdResultType * IdResult * IdRef * IdRef
   | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL of IdResultType * IdResult * IdRef * IdRef * IdRef * IdRef
   | OpSubgroupAvcSicConvertToMceResultINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetIpeLumaShapeINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetIpeChromaModeINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL of IdResultType * IdResult * IdRef
   | OpSubgroupAvcSicGetInterRawSadsINTEL of IdResultType * IdResult * IdRef

   member x.Opcode =
       match x with
       | OpNop -> 0u
       | OpUndef _ -> 1u
       | OpSourceContinued _ -> 2u
       | OpSource _ -> 3u
       | OpSourceExtension _ -> 4u
       | OpName _ -> 5u
       | OpMemberName _ -> 6u
       | OpString _ -> 7u
       | OpLine _ -> 8u
       | OpExtension _ -> 10u
       | OpExtInstImport _ -> 11u
       | OpExtInst _ -> 12u
       | OpMemoryModel _ -> 14u
       | OpEntryPoint _ -> 15u
       | OpExecutionMode _ -> 16u
       | OpCapability _ -> 17u
       | OpTypeVoid _ -> 19u
       | OpTypeBool _ -> 20u
       | OpTypeInt _ -> 21u
       | OpTypeFloat _ -> 22u
       | OpTypeVector _ -> 23u
       | OpTypeMatrix _ -> 24u
       | OpTypeImage _ -> 25u
       | OpTypeSampler _ -> 26u
       | OpTypeSampledImage _ -> 27u
       | OpTypeArray _ -> 28u
       | OpTypeRuntimeArray _ -> 29u
       | OpTypeStruct _ -> 30u
       | OpTypeOpaque _ -> 31u
       | OpTypePointer _ -> 32u
       | OpTypeFunction _ -> 33u
       | OpTypeEvent _ -> 34u
       | OpTypeDeviceEvent _ -> 35u
       | OpTypeReserveId _ -> 36u
       | OpTypeQueue _ -> 37u
       | OpTypePipe _ -> 38u
       | OpTypeForwardPointer _ -> 39u
       | OpConstantTrue _ -> 41u
       | OpConstantFalse _ -> 42u
       | OpConstant _ -> 43u
       | OpConstantComposite _ -> 44u
       | OpConstantSampler _ -> 45u
       | OpConstantNull _ -> 46u
       | OpSpecConstantTrue _ -> 48u
       | OpSpecConstantFalse _ -> 49u
       | OpSpecConstant _ -> 50u
       | OpSpecConstantComposite _ -> 51u
       | OpSpecConstantOp _ -> 52u
       | OpFunction _ -> 54u
       | OpFunctionParameter _ -> 55u
       | OpFunctionEnd -> 56u
       | OpFunctionCall _ -> 57u
       | OpVariable _ -> 59u
       | OpImageTexelPointer _ -> 60u
       | OpLoad _ -> 61u
       | OpStore _ -> 62u
       | OpCopyMemory _ -> 63u
       | OpCopyMemorySized _ -> 64u
       | OpAccessChain _ -> 65u
       | OpInBoundsAccessChain _ -> 66u
       | OpPtrAccessChain _ -> 67u
       | OpArrayLength _ -> 68u
       | OpGenericPtrMemSemantics _ -> 69u
       | OpInBoundsPtrAccessChain _ -> 70u
       | OpDecorate _ -> 71u
       | OpMemberDecorate _ -> 72u
       | OpDecorationGroup _ -> 73u
       | OpGroupDecorate _ -> 74u
       | OpGroupMemberDecorate _ -> 75u
       | OpVectorExtractDynamic _ -> 77u
       | OpVectorInsertDynamic _ -> 78u
       | OpVectorShuffle _ -> 79u
       | OpCompositeConstruct _ -> 80u
       | OpCompositeExtract _ -> 81u
       | OpCompositeInsert _ -> 82u
       | OpCopyObject _ -> 83u
       | OpTranspose _ -> 84u
       | OpSampledImage _ -> 86u
       | OpImageSampleImplicitLod _ -> 87u
       | OpImageSampleExplicitLod _ -> 88u
       | OpImageSampleDrefImplicitLod _ -> 89u
       | OpImageSampleDrefExplicitLod _ -> 90u
       | OpImageSampleProjImplicitLod _ -> 91u
       | OpImageSampleProjExplicitLod _ -> 92u
       | OpImageSampleProjDrefImplicitLod _ -> 93u
       | OpImageSampleProjDrefExplicitLod _ -> 94u
       | OpImageFetch _ -> 95u
       | OpImageGather _ -> 96u
       | OpImageDrefGather _ -> 97u
       | OpImageRead _ -> 98u
       | OpImageWrite _ -> 99u
       | OpImage _ -> 100u
       | OpImageQueryFormat _ -> 101u
       | OpImageQueryOrder _ -> 102u
       | OpImageQuerySizeLod _ -> 103u
       | OpImageQuerySize _ -> 104u
       | OpImageQueryLod _ -> 105u
       | OpImageQueryLevels _ -> 106u
       | OpImageQuerySamples _ -> 107u
       | OpConvertFToU _ -> 109u
       | OpConvertFToS _ -> 110u
       | OpConvertSToF _ -> 111u
       | OpConvertUToF _ -> 112u
       | OpUConvert _ -> 113u
       | OpSConvert _ -> 114u
       | OpFConvert _ -> 115u
       | OpQuantizeToF16 _ -> 116u
       | OpConvertPtrToU _ -> 117u
       | OpSatConvertSToU _ -> 118u
       | OpSatConvertUToS _ -> 119u
       | OpConvertUToPtr _ -> 120u
       | OpPtrCastToGeneric _ -> 121u
       | OpGenericCastToPtr _ -> 122u
       | OpGenericCastToPtrExplicit _ -> 123u
       | OpBitcast _ -> 124u
       | OpSNegate _ -> 126u
       | OpFNegate _ -> 127u
       | OpIAdd _ -> 128u
       | OpFAdd _ -> 129u
       | OpISub _ -> 130u
       | OpFSub _ -> 131u
       | OpIMul _ -> 132u
       | OpFMul _ -> 133u
       | OpUDiv _ -> 134u
       | OpSDiv _ -> 135u
       | OpFDiv _ -> 136u
       | OpUMod _ -> 137u
       | OpSRem _ -> 138u
       | OpSMod _ -> 139u
       | OpFRem _ -> 140u
       | OpFMod _ -> 141u
       | OpVectorTimesScalar _ -> 142u
       | OpMatrixTimesScalar _ -> 143u
       | OpVectorTimesMatrix _ -> 144u
       | OpMatrixTimesVector _ -> 145u
       | OpMatrixTimesMatrix _ -> 146u
       | OpOuterProduct _ -> 147u
       | OpDot _ -> 148u
       | OpIAddCarry _ -> 149u
       | OpISubBorrow _ -> 150u
       | OpUMulExtended _ -> 151u
       | OpSMulExtended _ -> 152u
       | OpAny _ -> 154u
       | OpAll _ -> 155u
       | OpIsNan _ -> 156u
       | OpIsInf _ -> 157u
       | OpIsFinite _ -> 158u
       | OpIsNormal _ -> 159u
       | OpSignBitSet _ -> 160u
       | OpLessOrGreater _ -> 161u
       | OpOrdered _ -> 162u
       | OpUnordered _ -> 163u
       | OpLogicalEqual _ -> 164u
       | OpLogicalNotEqual _ -> 165u
       | OpLogicalOr _ -> 166u
       | OpLogicalAnd _ -> 167u
       | OpLogicalNot _ -> 168u
       | OpSelect _ -> 169u
       | OpIEqual _ -> 170u
       | OpINotEqual _ -> 171u
       | OpUGreaterThan _ -> 172u
       | OpSGreaterThan _ -> 173u
       | OpUGreaterThanEqual _ -> 174u
       | OpSGreaterThanEqual _ -> 175u
       | OpULessThan _ -> 176u
       | OpSLessThan _ -> 177u
       | OpULessThanEqual _ -> 178u
       | OpSLessThanEqual _ -> 179u
       | OpFOrdEqual _ -> 180u
       | OpFUnordEqual _ -> 181u
       | OpFOrdNotEqual _ -> 182u
       | OpFUnordNotEqual _ -> 183u
       | OpFOrdLessThan _ -> 184u
       | OpFUnordLessThan _ -> 185u
       | OpFOrdGreaterThan _ -> 186u
       | OpFUnordGreaterThan _ -> 187u
       | OpFOrdLessThanEqual _ -> 188u
       | OpFUnordLessThanEqual _ -> 189u
       | OpFOrdGreaterThanEqual _ -> 190u
       | OpFUnordGreaterThanEqual _ -> 191u
       | OpShiftRightLogical _ -> 194u
       | OpShiftRightArithmetic _ -> 195u
       | OpShiftLeftLogical _ -> 196u
       | OpBitwiseOr _ -> 197u
       | OpBitwiseXor _ -> 198u
       | OpBitwiseAnd _ -> 199u
       | OpNot _ -> 200u
       | OpBitFieldInsert _ -> 201u
       | OpBitFieldSExtract _ -> 202u
       | OpBitFieldUExtract _ -> 203u
       | OpBitReverse _ -> 204u
       | OpBitCount _ -> 205u
       | OpDPdx _ -> 207u
       | OpDPdy _ -> 208u
       | OpFwidth _ -> 209u
       | OpDPdxFine _ -> 210u
       | OpDPdyFine _ -> 211u
       | OpFwidthFine _ -> 212u
       | OpDPdxCoarse _ -> 213u
       | OpDPdyCoarse _ -> 214u
       | OpFwidthCoarse _ -> 215u
       | OpEmitVertex -> 218u
       | OpEndPrimitive -> 219u
       | OpEmitStreamVertex _ -> 220u
       | OpEndStreamPrimitive _ -> 221u
       | OpControlBarrier _ -> 224u
       | OpMemoryBarrier _ -> 225u
       | OpAtomicLoad _ -> 227u
       | OpAtomicStore _ -> 228u
       | OpAtomicExchange _ -> 229u
       | OpAtomicCompareExchange _ -> 230u
       | OpAtomicCompareExchangeWeak _ -> 231u
       | OpAtomicIIncrement _ -> 232u
       | OpAtomicIDecrement _ -> 233u
       | OpAtomicIAdd _ -> 234u
       | OpAtomicISub _ -> 235u
       | OpAtomicSMin _ -> 236u
       | OpAtomicUMin _ -> 237u
       | OpAtomicSMax _ -> 238u
       | OpAtomicUMax _ -> 239u
       | OpAtomicAnd _ -> 240u
       | OpAtomicOr _ -> 241u
       | OpAtomicXor _ -> 242u
       | OpPhi _ -> 245u
       | OpLoopMerge _ -> 246u
       | OpSelectionMerge _ -> 247u
       | OpLabel _ -> 248u
       | OpBranch _ -> 249u
       | OpBranchConditional _ -> 250u
       | OpSwitch _ -> 251u
       | OpKill -> 252u
       | OpReturn -> 253u
       | OpReturnValue _ -> 254u
       | OpUnreachable -> 255u
       | OpLifetimeStart _ -> 256u
       | OpLifetimeStop _ -> 257u
       | OpGroupAsyncCopy _ -> 259u
       | OpGroupWaitEvents _ -> 260u
       | OpGroupAll _ -> 261u
       | OpGroupAny _ -> 262u
       | OpGroupBroadcast _ -> 263u
       | OpGroupIAdd _ -> 264u
       | OpGroupFAdd _ -> 265u
       | OpGroupFMin _ -> 266u
       | OpGroupUMin _ -> 267u
       | OpGroupSMin _ -> 268u
       | OpGroupFMax _ -> 269u
       | OpGroupUMax _ -> 270u
       | OpGroupSMax _ -> 271u
       | OpReadPipe _ -> 274u
       | OpWritePipe _ -> 275u
       | OpReservedReadPipe _ -> 276u
       | OpReservedWritePipe _ -> 277u
       | OpReserveReadPipePackets _ -> 278u
       | OpReserveWritePipePackets _ -> 279u
       | OpCommitReadPipe _ -> 280u
       | OpCommitWritePipe _ -> 281u
       | OpIsValidReserveId _ -> 282u
       | OpGetNumPipePackets _ -> 283u
       | OpGetMaxPipePackets _ -> 284u
       | OpGroupReserveReadPipePackets _ -> 285u
       | OpGroupReserveWritePipePackets _ -> 286u
       | OpGroupCommitReadPipe _ -> 287u
       | OpGroupCommitWritePipe _ -> 288u
       | OpEnqueueMarker _ -> 291u
       | OpEnqueueKernel _ -> 292u
       | OpGetKernelNDrangeSubGroupCount _ -> 293u
       | OpGetKernelNDrangeMaxSubGroupSize _ -> 294u
       | OpGetKernelWorkGroupSize _ -> 295u
       | OpGetKernelPreferredWorkGroupSizeMultiple _ -> 296u
       | OpRetainEvent _ -> 297u
       | OpReleaseEvent _ -> 298u
       | OpCreateUserEvent _ -> 299u
       | OpIsValidEvent _ -> 300u
       | OpSetUserEventStatus _ -> 301u
       | OpCaptureEventProfilingInfo _ -> 302u
       | OpGetDefaultQueue _ -> 303u
       | OpBuildNDRange _ -> 304u
       | OpImageSparseSampleImplicitLod _ -> 305u
       | OpImageSparseSampleExplicitLod _ -> 306u
       | OpImageSparseSampleDrefImplicitLod _ -> 307u
       | OpImageSparseSampleDrefExplicitLod _ -> 308u
       | OpImageSparseSampleProjImplicitLod _ -> 309u
       | OpImageSparseSampleProjExplicitLod _ -> 310u
       | OpImageSparseSampleProjDrefImplicitLod _ -> 311u
       | OpImageSparseSampleProjDrefExplicitLod _ -> 312u
       | OpImageSparseFetch _ -> 313u
       | OpImageSparseGather _ -> 314u
       | OpImageSparseDrefGather _ -> 315u
       | OpImageSparseTexelsResident _ -> 316u
       | OpNoLine -> 317u
       | OpAtomicFlagTestAndSet _ -> 318u
       | OpAtomicFlagClear _ -> 319u
       | OpImageSparseRead _ -> 320u
       | OpSizeOf _ -> 321u
       | OpTypePipeStorage _ -> 322u
       | OpConstantPipeStorage _ -> 323u
       | OpCreatePipeFromPipeStorage _ -> 324u
       | OpGetKernelLocalSizeForSubgroupCount _ -> 325u
       | OpGetKernelMaxNumSubgroups _ -> 326u
       | OpTypeNamedBarrier _ -> 327u
       | OpNamedBarrierInitialize _ -> 328u
       | OpMemoryNamedBarrier _ -> 329u
       | OpModuleProcessed _ -> 330u
       | OpExecutionModeId _ -> 331u
       | OpDecorateId _ -> 332u
       | OpGroupNonUniformElect _ -> 333u
       | OpGroupNonUniformAll _ -> 334u
       | OpGroupNonUniformAny _ -> 335u
       | OpGroupNonUniformAllEqual _ -> 336u
       | OpGroupNonUniformBroadcast _ -> 337u
       | OpGroupNonUniformBroadcastFirst _ -> 338u
       | OpGroupNonUniformBallot _ -> 339u
       | OpGroupNonUniformInverseBallot _ -> 340u
       | OpGroupNonUniformBallotBitExtract _ -> 341u
       | OpGroupNonUniformBallotBitCount _ -> 342u
       | OpGroupNonUniformBallotFindLSB _ -> 343u
       | OpGroupNonUniformBallotFindMSB _ -> 344u
       | OpGroupNonUniformShuffle _ -> 345u
       | OpGroupNonUniformShuffleXor _ -> 346u
       | OpGroupNonUniformShuffleUp _ -> 347u
       | OpGroupNonUniformShuffleDown _ -> 348u
       | OpGroupNonUniformIAdd _ -> 349u
       | OpGroupNonUniformFAdd _ -> 350u
       | OpGroupNonUniformIMul _ -> 351u
       | OpGroupNonUniformFMul _ -> 352u
       | OpGroupNonUniformSMin _ -> 353u
       | OpGroupNonUniformUMin _ -> 354u
       | OpGroupNonUniformFMin _ -> 355u
       | OpGroupNonUniformSMax _ -> 356u
       | OpGroupNonUniformUMax _ -> 357u
       | OpGroupNonUniformFMax _ -> 358u
       | OpGroupNonUniformBitwiseAnd _ -> 359u
       | OpGroupNonUniformBitwiseOr _ -> 360u
       | OpGroupNonUniformBitwiseXor _ -> 361u
       | OpGroupNonUniformLogicalAnd _ -> 362u
       | OpGroupNonUniformLogicalOr _ -> 363u
       | OpGroupNonUniformLogicalXor _ -> 364u
       | OpGroupNonUniformQuadBroadcast _ -> 365u
       | OpGroupNonUniformQuadSwap _ -> 366u
       | OpCopyLogical _ -> 400u
       | OpPtrEqual _ -> 401u
       | OpPtrNotEqual _ -> 402u
       | OpPtrDiff _ -> 403u
       | OpSubgroupBallotKHR _ -> 4421u
       | OpSubgroupFirstInvocationKHR _ -> 4422u
       | OpSubgroupAllKHR _ -> 4428u
       | OpSubgroupAnyKHR _ -> 4429u
       | OpSubgroupAllEqualKHR _ -> 4430u
       | OpSubgroupReadInvocationKHR _ -> 4432u
       | OpGroupIAddNonUniformAMD _ -> 5000u
       | OpGroupFAddNonUniformAMD _ -> 5001u
       | OpGroupFMinNonUniformAMD _ -> 5002u
       | OpGroupUMinNonUniformAMD _ -> 5003u
       | OpGroupSMinNonUniformAMD _ -> 5004u
       | OpGroupFMaxNonUniformAMD _ -> 5005u
       | OpGroupUMaxNonUniformAMD _ -> 5006u
       | OpGroupSMaxNonUniformAMD _ -> 5007u
       | OpFragmentMaskFetchAMD _ -> 5011u
       | OpFragmentFetchAMD _ -> 5012u
       | OpReadClockKHR _ -> 5056u
       | OpImageSampleFootprintNV _ -> 5283u
       | OpGroupNonUniformPartitionNV _ -> 5296u
       | OpWritePackedPrimitiveIndices4x8NV _ -> 5299u
       | OpReportIntersectionNV _ -> 5334u
       | OpIgnoreIntersectionNV -> 5335u
       | OpTerminateRayNV -> 5336u
       | OpTraceNV _ -> 5337u
       | OpTypeAccelerationStructureNV _ -> 5341u
       | OpExecuteCallableNV _ -> 5344u
       | OpTypeCooperativeMatrixNV _ -> 5358u
       | OpCooperativeMatrixLoadNV _ -> 5359u
       | OpCooperativeMatrixStoreNV _ -> 5360u
       | OpCooperativeMatrixMulAddNV _ -> 5361u
       | OpCooperativeMatrixLengthNV _ -> 5362u
       | OpBeginInvocationInterlockEXT -> 5364u
       | OpEndInvocationInterlockEXT -> 5365u
       | OpDemoteToHelperInvocationEXT -> 5380u
       | OpIsHelperInvocationEXT _ -> 5381u
       | OpSubgroupShuffleINTEL _ -> 5571u
       | OpSubgroupShuffleDownINTEL _ -> 5572u
       | OpSubgroupShuffleUpINTEL _ -> 5573u
       | OpSubgroupShuffleXorINTEL _ -> 5574u
       | OpSubgroupBlockReadINTEL _ -> 5575u
       | OpSubgroupBlockWriteINTEL _ -> 5576u
       | OpSubgroupImageBlockReadINTEL _ -> 5577u
       | OpSubgroupImageBlockWriteINTEL _ -> 5578u
       | OpSubgroupImageMediaBlockReadINTEL _ -> 5580u
       | OpSubgroupImageMediaBlockWriteINTEL _ -> 5581u
       | OpUCountLeadingZerosINTEL _ -> 5585u
       | OpUCountTrailingZerosINTEL _ -> 5586u
       | OpAbsISubINTEL _ -> 5587u
       | OpAbsUSubINTEL _ -> 5588u
       | OpIAddSatINTEL _ -> 5589u
       | OpUAddSatINTEL _ -> 5590u
       | OpIAverageINTEL _ -> 5591u
       | OpUAverageINTEL _ -> 5592u
       | OpIAverageRoundedINTEL _ -> 5593u
       | OpUAverageRoundedINTEL _ -> 5594u
       | OpISubSatINTEL _ -> 5595u
       | OpUSubSatINTEL _ -> 5596u
       | OpIMul32x16INTEL _ -> 5597u
       | OpUMul32x16INTEL _ -> 5598u
       | OpDecorateString _ -> 5632u
       | OpDecorateStringGOOGLE _ -> 5632u
       | OpMemberDecorateString _ -> 5633u
       | OpMemberDecorateStringGOOGLE _ -> 5633u
       | OpVmeImageINTEL _ -> 5699u
       | OpTypeVmeImageINTEL _ -> 5700u
       | OpTypeAvcImePayloadINTEL _ -> 5701u
       | OpTypeAvcRefPayloadINTEL _ -> 5702u
       | OpTypeAvcSicPayloadINTEL _ -> 5703u
       | OpTypeAvcMcePayloadINTEL _ -> 5704u
       | OpTypeAvcMceResultINTEL _ -> 5705u
       | OpTypeAvcImeResultINTEL _ -> 5706u
       | OpTypeAvcImeResultSingleReferenceStreamoutINTEL _ -> 5707u
       | OpTypeAvcImeResultDualReferenceStreamoutINTEL _ -> 5708u
       | OpTypeAvcImeSingleReferenceStreaminINTEL _ -> 5709u
       | OpTypeAvcImeDualReferenceStreaminINTEL _ -> 5710u
       | OpTypeAvcRefResultINTEL _ -> 5711u
       | OpTypeAvcSicResultINTEL _ -> 5712u
       | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL _ -> 5713u
       | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL _ -> 5714u
       | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL _ -> 5715u
       | OpSubgroupAvcMceSetInterShapePenaltyINTEL _ -> 5716u
       | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL _ -> 5717u
       | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL _ -> 5718u
       | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL _ -> 5719u
       | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL _ -> 5720u
       | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL _ -> 5721u
       | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL _ -> 5722u
       | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL _ -> 5723u
       | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL _ -> 5724u
       | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL _ -> 5725u
       | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL _ -> 5726u
       | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL _ -> 5727u
       | OpSubgroupAvcMceSetAcOnlyHaarINTEL _ -> 5728u
       | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL _ -> 5729u
       | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL _ -> 5730u
       | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL _ -> 5731u
       | OpSubgroupAvcMceConvertToImePayloadINTEL _ -> 5732u
       | OpSubgroupAvcMceConvertToImeResultINTEL _ -> 5733u
       | OpSubgroupAvcMceConvertToRefPayloadINTEL _ -> 5734u
       | OpSubgroupAvcMceConvertToRefResultINTEL _ -> 5735u
       | OpSubgroupAvcMceConvertToSicPayloadINTEL _ -> 5736u
       | OpSubgroupAvcMceConvertToSicResultINTEL _ -> 5737u
       | OpSubgroupAvcMceGetMotionVectorsINTEL _ -> 5738u
       | OpSubgroupAvcMceGetInterDistortionsINTEL _ -> 5739u
       | OpSubgroupAvcMceGetBestInterDistortionsINTEL _ -> 5740u
       | OpSubgroupAvcMceGetInterMajorShapeINTEL _ -> 5741u
       | OpSubgroupAvcMceGetInterMinorShapeINTEL _ -> 5742u
       | OpSubgroupAvcMceGetInterDirectionsINTEL _ -> 5743u
       | OpSubgroupAvcMceGetInterMotionVectorCountINTEL _ -> 5744u
       | OpSubgroupAvcMceGetInterReferenceIdsINTEL _ -> 5745u
       | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL _ -> 5746u
       | OpSubgroupAvcImeInitializeINTEL _ -> 5747u
       | OpSubgroupAvcImeSetSingleReferenceINTEL _ -> 5748u
       | OpSubgroupAvcImeSetDualReferenceINTEL _ -> 5749u
       | OpSubgroupAvcImeRefWindowSizeINTEL _ -> 5750u
       | OpSubgroupAvcImeAdjustRefOffsetINTEL _ -> 5751u
       | OpSubgroupAvcImeConvertToMcePayloadINTEL _ -> 5752u
       | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL _ -> 5753u
       | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL _ -> 5754u
       | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL _ -> 5755u
       | OpSubgroupAvcImeSetWeightedSadINTEL _ -> 5756u
       | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL _ -> 5757u
       | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL _ -> 5758u
       | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL _ -> 5759u
       | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL _ -> 5760u
       | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL _ -> 5761u
       | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL _ -> 5762u
       | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL _ -> 5763u
       | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL _ -> 5764u
       | OpSubgroupAvcImeConvertToMceResultINTEL _ -> 5765u
       | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL _ -> 5766u
       | OpSubgroupAvcImeGetDualReferenceStreaminINTEL _ -> 5767u
       | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL _ -> 5768u
       | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL _ -> 5769u
       | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL _ -> 5770u
       | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL _ -> 5771u
       | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL _ -> 5772u
       | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL _ -> 5773u
       | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL _ -> 5774u
       | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL _ -> 5775u
       | OpSubgroupAvcImeGetBorderReachedINTEL _ -> 5776u
       | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL _ -> 5777u
       | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL _ -> 5778u
       | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL _ -> 5779u
       | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL _ -> 5780u
       | OpSubgroupAvcFmeInitializeINTEL _ -> 5781u
       | OpSubgroupAvcBmeInitializeINTEL _ -> 5782u
       | OpSubgroupAvcRefConvertToMcePayloadINTEL _ -> 5783u
       | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL _ -> 5784u
       | OpSubgroupAvcRefSetBilinearFilterEnableINTEL _ -> 5785u
       | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL _ -> 5786u
       | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL _ -> 5787u
       | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL _ -> 5788u
       | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL _ -> 5789u
       | OpSubgroupAvcRefConvertToMceResultINTEL _ -> 5790u
       | OpSubgroupAvcSicInitializeINTEL _ -> 5791u
       | OpSubgroupAvcSicConfigureSkcINTEL _ -> 5792u
       | OpSubgroupAvcSicConfigureIpeLumaINTEL _ -> 5793u
       | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL _ -> 5794u
       | OpSubgroupAvcSicGetMotionVectorMaskINTEL _ -> 5795u
       | OpSubgroupAvcSicConvertToMcePayloadINTEL _ -> 5796u
       | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL _ -> 5797u
       | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL _ -> 5798u
       | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL _ -> 5799u
       | OpSubgroupAvcSicSetBilinearFilterEnableINTEL _ -> 5800u
       | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL _ -> 5801u
       | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL _ -> 5802u
       | OpSubgroupAvcSicEvaluateIpeINTEL _ -> 5803u
       | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL _ -> 5804u
       | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL _ -> 5805u
       | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL _ -> 5806u
       | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL _ -> 5807u
       | OpSubgroupAvcSicConvertToMceResultINTEL _ -> 5808u
       | OpSubgroupAvcSicGetIpeLumaShapeINTEL _ -> 5809u
       | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL _ -> 5810u
       | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL _ -> 5811u
       | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL _ -> 5812u
       | OpSubgroupAvcSicGetIpeChromaModeINTEL _ -> 5813u
       | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL _ -> 5814u
       | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL _ -> 5815u
       | OpSubgroupAvcSicGetInterRawSadsINTEL _ -> 5816u