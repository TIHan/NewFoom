// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System
open System.IO
open InternalHelpers

type ImageOperands =
   | None = 0x0000u
   | Bias = 0x0001u
   | Lod = 0x0002u
   | Grad = 0x0004u
   | ConstOffset = 0x0008u
   | Offset = 0x0010u
   | ConstOffsets = 0x0020u
   | Sample = 0x0040u
   | MinLod = 0x0080u
   | MakeTexelAvailable = 0x0100u
   | MakeTexelAvailableKHR = 0x0100u
   | MakeTexelVisible = 0x0200u
   | MakeTexelVisibleKHR = 0x0200u
   | NonPrivateTexel = 0x0400u
   | NonPrivateTexelKHR = 0x0400u
   | VolatileTexel = 0x0800u
   | VolatileTexelKHR = 0x0800u
   | SignExtend = 0x1000u
   | ZeroExtend = 0x2000u

type FPFastMathMode =
   | None = 0x0000u
   | NotNaN = 0x0001u
   | NotInf = 0x0002u
   | NSZ = 0x0004u
   | AllowRecip = 0x0008u
   | Fast = 0x0010u

type SelectionControl =
   | None = 0x0000u
   | Flatten = 0x0001u
   | DontFlatten = 0x0002u

type LoopControl =
   | None = 0x0000u
   | Unroll = 0x0001u
   | DontUnroll = 0x0002u
   | DependencyInfinite = 0x0004u
   | DependencyLength = 0x0008u
   | MinIterations = 0x0010u
   | MaxIterations = 0x0020u
   | IterationMultiple = 0x0040u
   | PeelCount = 0x0080u
   | PartialCount = 0x0100u

type FunctionControl =
   | None = 0x0000u
   | Inline = 0x0001u
   | DontInline = 0x0002u
   | Pure = 0x0004u
   | Const = 0x0008u

type MemorySemantics =
   | Relaxed = 0x0000u
   | None = 0x0000u
   | Acquire = 0x0002u
   | Release = 0x0004u
   | AcquireRelease = 0x0008u
   | SequentiallyConsistent = 0x0010u
   | UniformMemory = 0x0040u
   | SubgroupMemory = 0x0080u
   | WorkgroupMemory = 0x0100u
   | CrossWorkgroupMemory = 0x0200u
   | AtomicCounterMemory = 0x0400u
   | ImageMemory = 0x0800u
   | OutputMemory = 0x1000u
   | OutputMemoryKHR = 0x1000u
   | MakeAvailable = 0x2000u
   | MakeAvailableKHR = 0x2000u
   | MakeVisible = 0x4000u
   | MakeVisibleKHR = 0x4000u
   | Volatile = 0x8000u

type MemoryAccess =
   | None = 0x0000u
   | Volatile = 0x0001u
   | Aligned = 0x0002u
   | Nontemporal = 0x0004u
   | MakePointerAvailable = 0x0008u
   | MakePointerAvailableKHR = 0x0008u
   | MakePointerVisible = 0x0010u
   | MakePointerVisibleKHR = 0x0010u
   | NonPrivatePointer = 0x0020u
   | NonPrivatePointerKHR = 0x0020u

type KernelProfilingInfo =
   | None = 0x0000u
   | CmdExecTime = 0x0001u

type SourceLanguage =
   | Unknown = 0u
   | ESSL = 1u
   | GLSL = 2u
   | OpenCL_C = 3u
   | OpenCL_CPP = 4u
   | HLSL = 5u

type ExecutionModel =
   | Vertex = 0u
   | TessellationControl = 1u
   | TessellationEvaluation = 2u
   | Geometry = 3u
   | Fragment = 4u
   | GLCompute = 5u
   | Kernel = 6u
   | TaskNV = 5267u
   | MeshNV = 5268u
   | RayGenerationNV = 5313u
   | IntersectionNV = 5314u
   | AnyHitNV = 5315u
   | ClosestHitNV = 5316u
   | MissNV = 5317u
   | CallableNV = 5318u

type AddressingModel =
   | Logical = 0u
   | Physical32 = 1u
   | Physical64 = 2u
   | PhysicalStorageBuffer64 = 5348u
   | PhysicalStorageBuffer64EXT = 5348u

type MemoryModel =
   | Simple = 0u
   | GLSL450 = 1u
   | OpenCL = 2u
   | Vulkan = 3u
   | VulkanKHR = 3u

type ExecutionMode =
   | Invocations = 0u
   | SpacingEqual = 1u
   | SpacingFractionalEven = 2u
   | SpacingFractionalOdd = 3u
   | VertexOrderCw = 4u
   | VertexOrderCcw = 5u
   | PixelCenterInteger = 6u
   | OriginUpperLeft = 7u
   | OriginLowerLeft = 8u
   | EarlyFragmentTests = 9u
   | PointMode = 10u
   | Xfb = 11u
   | DepthReplacing = 12u
   | DepthGreater = 14u
   | DepthLess = 15u
   | DepthUnchanged = 16u
   | LocalSize = 17u
   | LocalSizeHint = 18u
   | InputPoints = 19u
   | InputLines = 20u
   | InputLinesAdjacency = 21u
   | Triangles = 22u
   | InputTrianglesAdjacency = 23u
   | Quads = 24u
   | Isolines = 25u
   | OutputVertices = 26u
   | OutputPoints = 27u
   | OutputLineStrip = 28u
   | OutputTriangleStrip = 29u
   | VecTypeHint = 30u
   | ContractionOff = 31u
   | Initializer = 33u
   | Finalizer = 34u
   | SubgroupSize = 35u
   | SubgroupsPerWorkgroup = 36u
   | SubgroupsPerWorkgroupId = 37u
   | LocalSizeId = 38u
   | LocalSizeHintId = 39u
   | PostDepthCoverage = 4446u
   | DenormPreserve = 4459u
   | DenormFlushToZero = 4460u
   | SignedZeroInfNanPreserve = 4461u
   | RoundingModeRTE = 4462u
   | RoundingModeRTZ = 4463u
   | StencilRefReplacingEXT = 5027u
   | OutputLinesNV = 5269u
   | OutputPrimitivesNV = 5270u
   | DerivativeGroupQuadsNV = 5289u
   | DerivativeGroupLinearNV = 5290u
   | OutputTrianglesNV = 5298u
   | PixelInterlockOrderedEXT = 5366u
   | PixelInterlockUnorderedEXT = 5367u
   | SampleInterlockOrderedEXT = 5368u
   | SampleInterlockUnorderedEXT = 5369u
   | ShadingRateInterlockOrderedEXT = 5370u
   | ShadingRateInterlockUnorderedEXT = 5371u

type StorageClass =
   | UniformConstant = 0u
   | Input = 1u
   | Uniform = 2u
   | Output = 3u
   | Workgroup = 4u
   | CrossWorkgroup = 5u
   | Private = 6u
   | Function = 7u
   | Generic = 8u
   | PushConstant = 9u
   | AtomicCounter = 10u
   | Image = 11u
   | StorageBuffer = 12u
   | CallableDataNV = 5328u
   | IncomingCallableDataNV = 5329u
   | RayPayloadNV = 5338u
   | HitAttributeNV = 5339u
   | IncomingRayPayloadNV = 5342u
   | ShaderRecordBufferNV = 5343u
   | PhysicalStorageBuffer = 5349u
   | PhysicalStorageBufferEXT = 5349u

type Dim =
   | One = 0u
   | Two = 1u
   | Three = 2u
   | Cube = 3u
   | Rect = 4u
   | Buffer = 5u
   | SubpassData = 6u

type SamplerAddressingMode =
   | None = 0u
   | ClampToEdge = 1u
   | Clamp = 2u
   | Repeat = 3u
   | RepeatMirrored = 4u

type SamplerFilterMode =
   | Nearest = 0u
   | Linear = 1u

type ImageFormat =
   | Unknown = 0u
   | Rgba32f = 1u
   | Rgba16f = 2u
   | R32f = 3u
   | Rgba8 = 4u
   | Rgba8Snorm = 5u
   | Rg32f = 6u
   | Rg16f = 7u
   | R11fG11fB10f = 8u
   | R16f = 9u
   | Rgba16 = 10u
   | Rgb10A2 = 11u
   | Rg16 = 12u
   | Rg8 = 13u
   | R16 = 14u
   | R8 = 15u
   | Rgba16Snorm = 16u
   | Rg16Snorm = 17u
   | Rg8Snorm = 18u
   | R16Snorm = 19u
   | R8Snorm = 20u
   | Rgba32i = 21u
   | Rgba16i = 22u
   | Rgba8i = 23u
   | R32i = 24u
   | Rg32i = 25u
   | Rg16i = 26u
   | Rg8i = 27u
   | R16i = 28u
   | R8i = 29u
   | Rgba32ui = 30u
   | Rgba16ui = 31u
   | Rgba8ui = 32u
   | R32ui = 33u
   | Rgb10a2ui = 34u
   | Rg32ui = 35u
   | Rg16ui = 36u
   | Rg8ui = 37u
   | R16ui = 38u
   | R8ui = 39u

type ImageChannelOrder =
   | R = 0u
   | A = 1u
   | RG = 2u
   | RA = 3u
   | RGB = 4u
   | RGBA = 5u
   | BGRA = 6u
   | ARGB = 7u
   | Intensity = 8u
   | Luminance = 9u
   | Rx = 10u
   | RGx = 11u
   | RGBx = 12u
   | Depth = 13u
   | DepthStencil = 14u
   | sRGB = 15u
   | sRGBx = 16u
   | sRGBA = 17u
   | sBGRA = 18u
   | ABGR = 19u

type ImageChannelDataType =
   | SnormInt8 = 0u
   | SnormInt16 = 1u
   | UnormInt8 = 2u
   | UnormInt16 = 3u
   | UnormShort565 = 4u
   | UnormShort555 = 5u
   | UnormInt101010 = 6u
   | SignedInt8 = 7u
   | SignedInt16 = 8u
   | SignedInt32 = 9u
   | UnsignedInt8 = 10u
   | UnsignedInt16 = 11u
   | UnsignedInt32 = 12u
   | HalfFloat = 13u
   | Float = 14u
   | UnormInt24 = 15u
   | UnormInt101010_2 = 16u

type FPRoundingMode =
   | RTE = 0u
   | RTZ = 1u
   | RTP = 2u
   | RTN = 3u

type LinkageType =
   | Export = 0u
   | Import = 1u

type AccessQualifier =
   | ReadOnly = 0u
   | WriteOnly = 1u
   | ReadWrite = 2u

type FunctionParameterAttribute =
   | Zext = 0u
   | Sext = 1u
   | ByVal = 2u
   | Sret = 3u
   | NoAlias = 4u
   | NoCapture = 5u
   | NoWrite = 6u
   | NoReadWrite = 7u

type Decoration =
   | RelaxedPrecision = 0u
   | SpecId = 1u
   | Block = 2u
   | BufferBlock = 3u
   | RowMajor = 4u
   | ColMajor = 5u
   | ArrayStride = 6u
   | MatrixStride = 7u
   | GLSLShared = 8u
   | GLSLPacked = 9u
   | CPacked = 10u
   | BuiltIn = 11u
   | NoPerspective = 13u
   | Flat = 14u
   | Patch = 15u
   | Centroid = 16u
   | Sample = 17u
   | Invariant = 18u
   | Restrict = 19u
   | Aliased = 20u
   | Volatile = 21u
   | Constant = 22u
   | Coherent = 23u
   | NonWritable = 24u
   | NonReadable = 25u
   | Uniform = 26u
   | UniformId = 27u
   | SaturatedConversion = 28u
   | Stream = 29u
   | Location = 30u
   | Component = 31u
   | Index = 32u
   | Binding = 33u
   | DescriptorSet = 34u
   | Offset = 35u
   | XfbBuffer = 36u
   | XfbStride = 37u
   | FuncParamAttr = 38u
   | FPRoundingMode = 39u
   | FPFastMathMode = 40u
   | LinkageAttributes = 41u
   | NoContraction = 42u
   | InputAttachmentIndex = 43u
   | Alignment = 44u
   | MaxByteOffset = 45u
   | AlignmentId = 46u
   | MaxByteOffsetId = 47u
   | NoSignedWrap = 4469u
   | NoUnsignedWrap = 4470u
   | ExplicitInterpAMD = 4999u
   | OverrideCoverageNV = 5248u
   | PassthroughNV = 5250u
   | ViewportRelativeNV = 5252u
   | SecondaryViewportRelativeNV = 5256u
   | PerPrimitiveNV = 5271u
   | PerViewNV = 5272u
   | PerTaskNV = 5273u
   | PerVertexNV = 5285u
   | NonUniform = 5300u
   | NonUniformEXT = 5300u
   | RestrictPointer = 5355u
   | RestrictPointerEXT = 5355u
   | AliasedPointer = 5356u
   | AliasedPointerEXT = 5356u
   | CounterBuffer = 5634u
   | HlslCounterBufferGOOGLE = 5634u
   | UserSemantic = 5635u
   | HlslSemanticGOOGLE = 5635u
   | UserTypeGOOGLE = 5636u

type BuiltIn =
   | Position = 0u
   | PointSize = 1u
   | ClipDistance = 3u
   | CullDistance = 4u
   | VertexId = 5u
   | InstanceId = 6u
   | PrimitiveId = 7u
   | InvocationId = 8u
   | Layer = 9u
   | ViewportIndex = 10u
   | TessLevelOuter = 11u
   | TessLevelInner = 12u
   | TessCoord = 13u
   | PatchVertices = 14u
   | FragCoord = 15u
   | PointCoord = 16u
   | FrontFacing = 17u
   | SampleId = 18u
   | SamplePosition = 19u
   | SampleMask = 20u
   | FragDepth = 22u
   | HelperInvocation = 23u
   | NumWorkgroups = 24u
   | WorkgroupSize = 25u
   | WorkgroupId = 26u
   | LocalInvocationId = 27u
   | GlobalInvocationId = 28u
   | LocalInvocationIndex = 29u
   | WorkDim = 30u
   | GlobalSize = 31u
   | EnqueuedWorkgroupSize = 32u
   | GlobalOffset = 33u
   | GlobalLinearId = 34u
   | SubgroupSize = 36u
   | SubgroupMaxSize = 37u
   | NumSubgroups = 38u
   | NumEnqueuedSubgroups = 39u
   | SubgroupId = 40u
   | SubgroupLocalInvocationId = 41u
   | VertexIndex = 42u
   | InstanceIndex = 43u
   | SubgroupEqMask = 4416u
   | SubgroupGeMask = 4417u
   | SubgroupGtMask = 4418u
   | SubgroupLeMask = 4419u
   | SubgroupLtMask = 4420u
   | SubgroupEqMaskKHR = 4416u
   | SubgroupGeMaskKHR = 4417u
   | SubgroupGtMaskKHR = 4418u
   | SubgroupLeMaskKHR = 4419u
   | SubgroupLtMaskKHR = 4420u
   | BaseVertex = 4424u
   | BaseInstance = 4425u
   | DrawIndex = 4426u
   | DeviceIndex = 4438u
   | ViewIndex = 4440u
   | BaryCoordNoPerspAMD = 4992u
   | BaryCoordNoPerspCentroidAMD = 4993u
   | BaryCoordNoPerspSampleAMD = 4994u
   | BaryCoordSmoothAMD = 4995u
   | BaryCoordSmoothCentroidAMD = 4996u
   | BaryCoordSmoothSampleAMD = 4997u
   | BaryCoordPullModelAMD = 4998u
   | FragStencilRefEXT = 5014u
   | ViewportMaskNV = 5253u
   | SecondaryPositionNV = 5257u
   | SecondaryViewportMaskNV = 5258u
   | PositionPerViewNV = 5261u
   | ViewportMaskPerViewNV = 5262u
   | FullyCoveredEXT = 5264u
   | TaskCountNV = 5274u
   | PrimitiveCountNV = 5275u
   | PrimitiveIndicesNV = 5276u
   | ClipDistancePerViewNV = 5277u
   | CullDistancePerViewNV = 5278u
   | LayerPerViewNV = 5279u
   | MeshViewCountNV = 5280u
   | MeshViewIndicesNV = 5281u
   | BaryCoordNV = 5286u
   | BaryCoordNoPerspNV = 5287u
   | FragSizeEXT = 5292u
   | FragmentSizeNV = 5292u
   | FragInvocationCountEXT = 5293u
   | InvocationsPerPixelNV = 5293u
   | LaunchIdNV = 5319u
   | LaunchSizeNV = 5320u
   | WorldRayOriginNV = 5321u
   | WorldRayDirectionNV = 5322u
   | ObjectRayOriginNV = 5323u
   | ObjectRayDirectionNV = 5324u
   | RayTminNV = 5325u
   | RayTmaxNV = 5326u
   | InstanceCustomIndexNV = 5327u
   | ObjectToWorldNV = 5330u
   | WorldToObjectNV = 5331u
   | HitTNV = 5332u
   | HitKindNV = 5333u
   | IncomingRayFlagsNV = 5351u
   | WarpsPerSMNV = 5374u
   | SMCountNV = 5375u
   | WarpIDNV = 5376u
   | SMIDNV = 5377u

type Scope =
   | CrossDevice = 0u
   | Device = 1u
   | Workgroup = 2u
   | Subgroup = 3u
   | Invocation = 4u
   | QueueFamily = 5u
   | QueueFamilyKHR = 5u

type GroupOperation =
   | Reduce = 0u
   | InclusiveScan = 1u
   | ExclusiveScan = 2u
   | ClusteredReduce = 3u
   | PartitionedReduceNV = 6u
   | PartitionedInclusiveScanNV = 7u
   | PartitionedExclusiveScanNV = 8u

type KernelEnqueueFlags =
   | NoWait = 0u
   | WaitKernel = 1u
   | WaitWorkGroup = 2u

type Capability =
   | Matrix = 0u
   | Shader = 1u
   | Geometry = 2u
   | Tessellation = 3u
   | Addresses = 4u
   | Linkage = 5u
   | Kernel = 6u
   | Vector16 = 7u
   | Float16Buffer = 8u
   | Float16 = 9u
   | Float64 = 10u
   | Int64 = 11u
   | Int64Atomics = 12u
   | ImageBasic = 13u
   | ImageReadWrite = 14u
   | ImageMipmap = 15u
   | Pipes = 17u
   | Groups = 18u
   | DeviceEnqueue = 19u
   | LiteralSampler = 20u
   | AtomicStorage = 21u
   | Int16 = 22u
   | TessellationPointSize = 23u
   | GeometryPointSize = 24u
   | ImageGatherExtended = 25u
   | StorageImageMultisample = 27u
   | UniformBufferArrayDynamicIndexing = 28u
   | SampledImageArrayDynamicIndexing = 29u
   | StorageBufferArrayDynamicIndexing = 30u
   | StorageImageArrayDynamicIndexing = 31u
   | ClipDistance = 32u
   | CullDistance = 33u
   | ImageCubeArray = 34u
   | SampleRateShading = 35u
   | ImageRect = 36u
   | SampledRect = 37u
   | GenericPointer = 38u
   | Int8 = 39u
   | InputAttachment = 40u
   | SparseResidency = 41u
   | MinLod = 42u
   | Sampled1D = 43u
   | Image1D = 44u
   | SampledCubeArray = 45u
   | SampledBuffer = 46u
   | ImageBuffer = 47u
   | ImageMSArray = 48u
   | StorageImageExtendedFormats = 49u
   | ImageQuery = 50u
   | DerivativeControl = 51u
   | InterpolationFunction = 52u
   | TransformFeedback = 53u
   | GeometryStreams = 54u
   | StorageImageReadWithoutFormat = 55u
   | StorageImageWriteWithoutFormat = 56u
   | MultiViewport = 57u
   | SubgroupDispatch = 58u
   | NamedBarrier = 59u
   | PipeStorage = 60u
   | GroupNonUniform = 61u
   | GroupNonUniformVote = 62u
   | GroupNonUniformArithmetic = 63u
   | GroupNonUniformBallot = 64u
   | GroupNonUniformShuffle = 65u
   | GroupNonUniformShuffleRelative = 66u
   | GroupNonUniformClustered = 67u
   | GroupNonUniformQuad = 68u
   | ShaderLayer = 69u
   | ShaderViewportIndex = 70u
   | SubgroupBallotKHR = 4423u
   | DrawParameters = 4427u
   | SubgroupVoteKHR = 4431u
   | StorageBuffer16BitAccess = 4433u
   | StorageUniformBufferBlock16 = 4433u
   | UniformAndStorageBuffer16BitAccess = 4434u
   | StorageUniform16 = 4434u
   | StoragePushConstant16 = 4435u
   | StorageInputOutput16 = 4436u
   | DeviceGroup = 4437u
   | MultiView = 4439u
   | VariablePointersStorageBuffer = 4441u
   | VariablePointers = 4442u
   | AtomicStorageOps = 4445u
   | SampleMaskPostDepthCoverage = 4447u
   | StorageBuffer8BitAccess = 4448u
   | UniformAndStorageBuffer8BitAccess = 4449u
   | StoragePushConstant8 = 4450u
   | DenormPreserve = 4464u
   | DenormFlushToZero = 4465u
   | SignedZeroInfNanPreserve = 4466u
   | RoundingModeRTE = 4467u
   | RoundingModeRTZ = 4468u
   | Float16ImageAMD = 5008u
   | ImageGatherBiasLodAMD = 5009u
   | FragmentMaskAMD = 5010u
   | StencilExportEXT = 5013u
   | ImageReadWriteLodAMD = 5015u
   | ShaderClockKHR = 5055u
   | SampleMaskOverrideCoverageNV = 5249u
   | GeometryShaderPassthroughNV = 5251u
   | ShaderViewportIndexLayerEXT = 5254u
   | ShaderViewportIndexLayerNV = 5254u
   | ShaderViewportMaskNV = 5255u
   | ShaderStereoViewNV = 5259u
   | PerViewAttributesNV = 5260u
   | FragmentFullyCoveredEXT = 5265u
   | MeshShadingNV = 5266u
   | ImageFootprintNV = 5282u
   | FragmentBarycentricNV = 5284u
   | ComputeDerivativeGroupQuadsNV = 5288u
   | FragmentDensityEXT = 5291u
   | ShadingRateNV = 5291u
   | GroupNonUniformPartitionedNV = 5297u
   | ShaderNonUniform = 5301u
   | ShaderNonUniformEXT = 5301u
   | RuntimeDescriptorArray = 5302u
   | RuntimeDescriptorArrayEXT = 5302u
   | InputAttachmentArrayDynamicIndexing = 5303u
   | InputAttachmentArrayDynamicIndexingEXT = 5303u
   | UniformTexelBufferArrayDynamicIndexing = 5304u
   | UniformTexelBufferArrayDynamicIndexingEXT = 5304u
   | StorageTexelBufferArrayDynamicIndexing = 5305u
   | StorageTexelBufferArrayDynamicIndexingEXT = 5305u
   | UniformBufferArrayNonUniformIndexing = 5306u
   | UniformBufferArrayNonUniformIndexingEXT = 5306u
   | SampledImageArrayNonUniformIndexing = 5307u
   | SampledImageArrayNonUniformIndexingEXT = 5307u
   | StorageBufferArrayNonUniformIndexing = 5308u
   | StorageBufferArrayNonUniformIndexingEXT = 5308u
   | StorageImageArrayNonUniformIndexing = 5309u
   | StorageImageArrayNonUniformIndexingEXT = 5309u
   | InputAttachmentArrayNonUniformIndexing = 5310u
   | InputAttachmentArrayNonUniformIndexingEXT = 5310u
   | UniformTexelBufferArrayNonUniformIndexing = 5311u
   | UniformTexelBufferArrayNonUniformIndexingEXT = 5311u
   | StorageTexelBufferArrayNonUniformIndexing = 5312u
   | StorageTexelBufferArrayNonUniformIndexingEXT = 5312u
   | RayTracingNV = 5340u
   | VulkanMemoryModel = 5345u
   | VulkanMemoryModelKHR = 5345u
   | VulkanMemoryModelDeviceScope = 5346u
   | VulkanMemoryModelDeviceScopeKHR = 5346u
   | PhysicalStorageBufferAddresses = 5347u
   | PhysicalStorageBufferAddressesEXT = 5347u
   | ComputeDerivativeGroupLinearNV = 5350u
   | CooperativeMatrixNV = 5357u
   | FragmentShaderSampleInterlockEXT = 5363u
   | FragmentShaderShadingRateInterlockEXT = 5372u
   | ShaderSMBuiltinsNV = 5373u
   | FragmentShaderPixelInterlockEXT = 5378u
   | DemoteToHelperInvocationEXT = 5379u
   | SubgroupShuffleINTEL = 5568u
   | SubgroupBufferBlockIOINTEL = 5569u
   | SubgroupImageBlockIOINTEL = 5570u
   | SubgroupImageMediaBlockIOINTEL = 5579u
   | IntegerFunctions2INTEL = 5584u
   | SubgroupAvcMotionEstimationINTEL = 5696u
   | SubgroupAvcMotionEstimationIntraINTEL = 5697u
   | SubgroupAvcMotionEstimationChromaINTEL = 5698u

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
type LiteralInteger = uint32

/// A null-terminated stream of characters consuming an integral number of words
type LiteralString = string

/// A literal number whose size and format are determined by a previous operand in the enclosing instruction
type LiteralContextDependentNumber = uint32

/// A 32-bit unsigned integer indicating which instruction to use and determining the layout of following operands (for OpExtInst)
type LiteralExtInstInteger = uint32

/// An opcode indicating the operation to be performed and determining the layout of following operands (for OpSpecConstantOp)
type LiteralSpecConstantOpInteger = uint32

type PairLiteralIntegerIdRef = PairLiteralIntegerIdRef of LiteralInteger * IdRef

type PairIdRefLiteralInteger = PairIdRefLiteralInteger of IdRef * LiteralInteger

type PairIdRefIdRef = PairIdRefIdRef of IdRef * IdRef

type Instruction =
   | OpNop
   | OpUndef of uint32 * uint32
   | OpSourceContinued of ContinuedSource: string
   | OpSource of SourceLanguage * Version: uint32 * File: uint32 option * Source: string option
   | OpSourceExtension of Extension: string
   | OpName of Target: uint32 * Name: string
   | OpMemberName of Type: uint32 * Member: uint32 * Name: string
   | OpString of uint32 * String: string
   | OpLine of File: uint32 * Line: uint32 * Column: uint32
   | OpExtension of Name: string
   | OpExtInstImport of uint32 * Name: string
   | OpExtInst of uint32 * uint32 * Set: uint32 * Instruction: uint32 * Operand1: uint32 list
   | OpMemoryModel of AddressingModel * MemoryModel
   | OpEntryPoint of ExecutionModel * EntryPoint: uint32 * Name: string * Interface: uint32 list
   | OpExecutionMode of EntryPoint: uint32 * Mode: ExecutionMode
   | OpCapability of Capability: Capability
   | OpTypeVoid of uint32
   | OpTypeBool of uint32
   | OpTypeInt of uint32 * Width: uint32 * Signedness: uint32
   | OpTypeFloat of uint32 * Width: uint32
   | OpTypeVector of uint32 * ComponentType: uint32 * ComponentCount: uint32
   | OpTypeMatrix of uint32 * ColumnType: uint32 * ColumnCount: uint32
   | OpTypeImage of uint32 * SampledType: uint32 * Dim * Depth: uint32 * Arrayed: uint32 * MS: uint32 * Sampled: uint32 * ImageFormat * AccessQualifier option
   | OpTypeSampler of uint32
   | OpTypeSampledImage of uint32 * ImageType: uint32
   | OpTypeArray of uint32 * ElementType: uint32 * Length: uint32
   | OpTypeRuntimeArray of uint32 * ElementType: uint32
   | OpTypeStruct of uint32 * Member0type: uint32 list
   | OpTypeOpaque of uint32 * string
   | OpTypePointer of uint32 * StorageClass * Type: uint32
   | OpTypeFunction of uint32 * ReturnType: uint32 * Parameter0Type: uint32 list
   | OpTypeEvent of uint32
   | OpTypeDeviceEvent of uint32
   | OpTypeReserveId of uint32
   | OpTypeQueue of uint32
   | OpTypePipe of uint32 * Qualifier: AccessQualifier
   | OpTypeForwardPointer of PointerType: uint32 * StorageClass
   | OpConstantTrue of uint32 * uint32
   | OpConstantFalse of uint32 * uint32
   | OpConstant of uint32 * uint32 * Value: uint32
   | OpConstantComposite of uint32 * uint32 * Constituents: uint32 list
   | OpConstantSampler of uint32 * uint32 * SamplerAddressingMode * Param: uint32 * SamplerFilterMode
   | OpConstantNull of uint32 * uint32
   | OpSpecConstantTrue of uint32 * uint32
   | OpSpecConstantFalse of uint32 * uint32
   | OpSpecConstant of uint32 * uint32 * Value: uint32
   | OpSpecConstantComposite of uint32 * uint32 * Constituents: uint32 list
   | OpSpecConstantOp of uint32 * uint32 * Opcode: uint32
   | OpFunction of uint32 * uint32 * FunctionControl * FunctionType: uint32
   | OpFunctionParameter of uint32 * uint32
   | OpFunctionEnd
   | OpFunctionCall of uint32 * uint32 * Function: uint32 * Argument0: uint32 list
   | OpVariable of uint32 * uint32 * StorageClass * Initializer: uint32 option
   | OpImageTexelPointer of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * Sample: uint32
   | OpLoad of uint32 * uint32 * Pointer: uint32 * MemoryAccess option
   | OpStore of Pointer: uint32 * Object: uint32 * MemoryAccess option
   | OpCopyMemory of Target: uint32 * Source: uint32 * MemoryAccess option * MemoryAccess option
   | OpCopyMemorySized of Target: uint32 * Source: uint32 * Size: uint32 * MemoryAccess option * MemoryAccess option
   | OpAccessChain of uint32 * uint32 * Base: uint32 * Indexes: uint32 list
   | OpInBoundsAccessChain of uint32 * uint32 * Base: uint32 * Indexes: uint32 list
   | OpPtrAccessChain of uint32 * uint32 * Base: uint32 * Element: uint32 * Indexes: uint32 list
   | OpArrayLength of uint32 * uint32 * Structure: uint32 * Arraymember: uint32
   | OpGenericPtrMemSemantics of uint32 * uint32 * Pointer: uint32
   | OpInBoundsPtrAccessChain of uint32 * uint32 * Base: uint32 * Element: uint32 * Indexes: uint32 list
   | OpDecorate of Target: uint32 * Decoration
   | OpMemberDecorate of StructureType: uint32 * Member: uint32 * Decoration
   | OpDecorationGroup of uint32
   | OpGroupDecorate of DecorationGroup: uint32 * Targets: uint32 list
   | OpGroupMemberDecorate of DecorationGroup: uint32 * Targets: PairIdRefLiteralInteger list
   | OpVectorExtractDynamic of uint32 * uint32 * Vector: uint32 * Index: uint32
   | OpVectorInsertDynamic of uint32 * uint32 * Vector: uint32 * Component: uint32 * Index: uint32
   | OpVectorShuffle of uint32 * uint32 * Vector1: uint32 * Vector2: uint32 * Components: uint32 list
   | OpCompositeConstruct of uint32 * uint32 * Constituents: uint32 list
   | OpCompositeExtract of uint32 * uint32 * Composite: uint32 * Indexes: uint32 list
   | OpCompositeInsert of uint32 * uint32 * Object: uint32 * Composite: uint32 * Indexes: uint32 list
   | OpCopyObject of uint32 * uint32 * Operand: uint32
   | OpTranspose of uint32 * uint32 * Matrix: uint32
   | OpSampledImage of uint32 * uint32 * Image: uint32 * Sampler: uint32
   | OpImageSampleImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageSampleExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands
   | OpImageSampleDrefImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands option
   | OpImageSampleDrefExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands
   | OpImageSampleProjImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageSampleProjExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands
   | OpImageSampleProjDrefImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands option
   | OpImageSampleProjDrefExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands
   | OpImageFetch of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageGather of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Component: uint32 * ImageOperands option
   | OpImageDrefGather of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands option
   | OpImageRead of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageWrite of Image: uint32 * Coordinate: uint32 * Texel: uint32 * ImageOperands option
   | OpImage of uint32 * uint32 * SampledImage: uint32
   | OpImageQueryFormat of uint32 * uint32 * Image: uint32
   | OpImageQueryOrder of uint32 * uint32 * Image: uint32
   | OpImageQuerySizeLod of uint32 * uint32 * Image: uint32 * LevelofDetail: uint32
   | OpImageQuerySize of uint32 * uint32 * Image: uint32
   | OpImageQueryLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32
   | OpImageQueryLevels of uint32 * uint32 * Image: uint32
   | OpImageQuerySamples of uint32 * uint32 * Image: uint32
   | OpConvertFToU of uint32 * uint32 * FloatValue: uint32
   | OpConvertFToS of uint32 * uint32 * FloatValue: uint32
   | OpConvertSToF of uint32 * uint32 * SignedValue: uint32
   | OpConvertUToF of uint32 * uint32 * UnsignedValue: uint32
   | OpUConvert of uint32 * uint32 * UnsignedValue: uint32
   | OpSConvert of uint32 * uint32 * SignedValue: uint32
   | OpFConvert of uint32 * uint32 * FloatValue: uint32
   | OpQuantizeToF16 of uint32 * uint32 * Value: uint32
   | OpConvertPtrToU of uint32 * uint32 * Pointer: uint32
   | OpSatConvertSToU of uint32 * uint32 * SignedValue: uint32
   | OpSatConvertUToS of uint32 * uint32 * UnsignedValue: uint32
   | OpConvertUToPtr of uint32 * uint32 * IntegerValue: uint32
   | OpPtrCastToGeneric of uint32 * uint32 * Pointer: uint32
   | OpGenericCastToPtr of uint32 * uint32 * Pointer: uint32
   | OpGenericCastToPtrExplicit of uint32 * uint32 * Pointer: uint32 * Storage: StorageClass
   | OpBitcast of uint32 * uint32 * Operand: uint32
   | OpSNegate of uint32 * uint32 * Operand: uint32
   | OpFNegate of uint32 * uint32 * Operand: uint32
   | OpIAdd of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFAdd of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpISub of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFSub of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpIMul of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFMul of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUDiv of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSDiv of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFDiv of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUMod of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSRem of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSMod of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFRem of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFMod of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpVectorTimesScalar of uint32 * uint32 * Vector: uint32 * Scalar: uint32
   | OpMatrixTimesScalar of uint32 * uint32 * Matrix: uint32 * Scalar: uint32
   | OpVectorTimesMatrix of uint32 * uint32 * Vector: uint32 * Matrix: uint32
   | OpMatrixTimesVector of uint32 * uint32 * Matrix: uint32 * Vector: uint32
   | OpMatrixTimesMatrix of uint32 * uint32 * LeftMatrix: uint32 * RightMatrix: uint32
   | OpOuterProduct of uint32 * uint32 * Vector1: uint32 * Vector2: uint32
   | OpDot of uint32 * uint32 * Vector1: uint32 * Vector2: uint32
   | OpIAddCarry of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpISubBorrow of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUMulExtended of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSMulExtended of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpAny of uint32 * uint32 * Vector: uint32
   | OpAll of uint32 * uint32 * Vector: uint32
   | OpIsNan of uint32 * uint32 * x: uint32
   | OpIsInf of uint32 * uint32 * x: uint32
   | OpIsFinite of uint32 * uint32 * x: uint32
   | OpIsNormal of uint32 * uint32 * x: uint32
   | OpSignBitSet of uint32 * uint32 * x: uint32
   | OpLessOrGreater of uint32 * uint32 * x: uint32 * y: uint32
   | OpOrdered of uint32 * uint32 * x: uint32 * y: uint32
   | OpUnordered of uint32 * uint32 * x: uint32 * y: uint32
   | OpLogicalEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpLogicalNotEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpLogicalOr of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpLogicalAnd of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpLogicalNot of uint32 * uint32 * Operand: uint32
   | OpSelect of uint32 * uint32 * Condition: uint32 * Object1: uint32 * Object2: uint32
   | OpIEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpINotEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUGreaterThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSGreaterThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUGreaterThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSGreaterThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpULessThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSLessThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpULessThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSLessThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFOrdEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFUnordEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFOrdNotEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFUnordNotEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFOrdLessThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFUnordLessThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFOrdGreaterThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFUnordGreaterThan of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFOrdLessThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFUnordLessThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFOrdGreaterThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpFUnordGreaterThanEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpShiftRightLogical of uint32 * uint32 * Base: uint32 * Shift: uint32
   | OpShiftRightArithmetic of uint32 * uint32 * Base: uint32 * Shift: uint32
   | OpShiftLeftLogical of uint32 * uint32 * Base: uint32 * Shift: uint32
   | OpBitwiseOr of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpBitwiseXor of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpBitwiseAnd of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpNot of uint32 * uint32 * Operand: uint32
   | OpBitFieldInsert of uint32 * uint32 * Base: uint32 * Insert: uint32 * Offset: uint32 * Count: uint32
   | OpBitFieldSExtract of uint32 * uint32 * Base: uint32 * Offset: uint32 * Count: uint32
   | OpBitFieldUExtract of uint32 * uint32 * Base: uint32 * Offset: uint32 * Count: uint32
   | OpBitReverse of uint32 * uint32 * Base: uint32
   | OpBitCount of uint32 * uint32 * Base: uint32
   | OpDPdx of uint32 * uint32 * P: uint32
   | OpDPdy of uint32 * uint32 * P: uint32
   | OpFwidth of uint32 * uint32 * P: uint32
   | OpDPdxFine of uint32 * uint32 * P: uint32
   | OpDPdyFine of uint32 * uint32 * P: uint32
   | OpFwidthFine of uint32 * uint32 * P: uint32
   | OpDPdxCoarse of uint32 * uint32 * P: uint32
   | OpDPdyCoarse of uint32 * uint32 * P: uint32
   | OpFwidthCoarse of uint32 * uint32 * P: uint32
   | OpEmitVertex
   | OpEndPrimitive
   | OpEmitStreamVertex of Stream: uint32
   | OpEndStreamPrimitive of Stream: uint32
   | OpControlBarrier of Execution: uint32 * Memory: uint32 * Semantics: uint32
   | OpMemoryBarrier of Memory: uint32 * Semantics: uint32
   | OpAtomicLoad of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32
   | OpAtomicStore of Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicExchange of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicCompareExchange of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Equal: uint32 * Unequal: uint32 * Value: uint32 * Comparator: uint32
   | OpAtomicCompareExchangeWeak of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Equal: uint32 * Unequal: uint32 * Value: uint32 * Comparator: uint32
   | OpAtomicIIncrement of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32
   | OpAtomicIDecrement of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32
   | OpAtomicIAdd of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicISub of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicSMin of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicUMin of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicSMax of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicUMax of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicAnd of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicOr of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpAtomicXor of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32 * Value: uint32
   | OpPhi of uint32 * uint32 * VariableParent: PairIdRefIdRef list
   | OpLoopMerge of MergeBlock: uint32 * ContinueTarget: uint32 * LoopControl
   | OpSelectionMerge of MergeBlock: uint32 * SelectionControl
   | OpLabel of uint32
   | OpBranch of TargetLabel: uint32
   | OpBranchConditional of Condition: uint32 * TrueLabel: uint32 * FalseLabel: uint32 * Branchweights: uint32 list
   | OpSwitch of Selector: uint32 * Default: uint32 * Target: PairLiteralIntegerIdRef list
   | OpKill
   | OpReturn
   | OpReturnValue of Value: uint32
   | OpUnreachable
   | OpLifetimeStart of Pointer: uint32 * Size: uint32
   | OpLifetimeStop of Pointer: uint32 * Size: uint32
   | OpGroupAsyncCopy of uint32 * uint32 * Execution: uint32 * Destination: uint32 * Source: uint32 * NumElements: uint32 * Stride: uint32 * Event: uint32
   | OpGroupWaitEvents of Execution: uint32 * NumEvents: uint32 * EventsList: uint32
   | OpGroupAll of uint32 * uint32 * Execution: uint32 * Predicate: uint32
   | OpGroupAny of uint32 * uint32 * Execution: uint32 * Predicate: uint32
   | OpGroupBroadcast of uint32 * uint32 * Execution: uint32 * Value: uint32 * LocalId: uint32
   | OpGroupIAdd of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupFAdd of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupFMin of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupUMin of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupSMin of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupFMax of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupUMax of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupSMax of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpReadPipe of uint32 * uint32 * Pipe: uint32 * Pointer: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpWritePipe of uint32 * uint32 * Pipe: uint32 * Pointer: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpReservedReadPipe of uint32 * uint32 * Pipe: uint32 * ReserveId: uint32 * Index: uint32 * Pointer: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpReservedWritePipe of uint32 * uint32 * Pipe: uint32 * ReserveId: uint32 * Index: uint32 * Pointer: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpReserveReadPipePackets of uint32 * uint32 * Pipe: uint32 * NumPackets: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpReserveWritePipePackets of uint32 * uint32 * Pipe: uint32 * NumPackets: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpCommitReadPipe of Pipe: uint32 * ReserveId: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpCommitWritePipe of Pipe: uint32 * ReserveId: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpIsValidReserveId of uint32 * uint32 * ReserveId: uint32
   | OpGetNumPipePackets of uint32 * uint32 * Pipe: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpGetMaxPipePackets of uint32 * uint32 * Pipe: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpGroupReserveReadPipePackets of uint32 * uint32 * Execution: uint32 * Pipe: uint32 * NumPackets: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpGroupReserveWritePipePackets of uint32 * uint32 * Execution: uint32 * Pipe: uint32 * NumPackets: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpGroupCommitReadPipe of Execution: uint32 * Pipe: uint32 * ReserveId: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpGroupCommitWritePipe of Execution: uint32 * Pipe: uint32 * ReserveId: uint32 * PacketSize: uint32 * PacketAlignment: uint32
   | OpEnqueueMarker of uint32 * uint32 * Queue: uint32 * NumEvents: uint32 * WaitEvents: uint32 * RetEvent: uint32
   | OpEnqueueKernel of uint32 * uint32 * Queue: uint32 * Flags: uint32 * NDRange: uint32 * NumEvents: uint32 * WaitEvents: uint32 * RetEvent: uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32 * LocalSize: uint32 list
   | OpGetKernelNDrangeSubGroupCount of uint32 * uint32 * NDRange: uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32
   | OpGetKernelNDrangeMaxSubGroupSize of uint32 * uint32 * NDRange: uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32
   | OpGetKernelWorkGroupSize of uint32 * uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32
   | OpGetKernelPreferredWorkGroupSizeMultiple of uint32 * uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32
   | OpRetainEvent of Event: uint32
   | OpReleaseEvent of Event: uint32
   | OpCreateUserEvent of uint32 * uint32
   | OpIsValidEvent of uint32 * uint32 * Event: uint32
   | OpSetUserEventStatus of Event: uint32 * Status: uint32
   | OpCaptureEventProfilingInfo of Event: uint32 * ProfilingInfo: uint32 * Value: uint32
   | OpGetDefaultQueue of uint32 * uint32
   | OpBuildNDRange of uint32 * uint32 * GlobalWorkSize: uint32 * LocalWorkSize: uint32 * GlobalWorkOffset: uint32
   | OpImageSparseSampleImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageSparseSampleExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands
   | OpImageSparseSampleDrefImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands option
   | OpImageSparseSampleDrefExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands
   | OpImageSparseSampleProjImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageSparseSampleProjExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * ImageOperands
   | OpImageSparseSampleProjDrefImplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands option
   | OpImageSparseSampleProjDrefExplicitLod of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands
   | OpImageSparseFetch of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * ImageOperands option
   | OpImageSparseGather of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Component: uint32 * ImageOperands option
   | OpImageSparseDrefGather of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Dref: uint32 * ImageOperands option
   | OpImageSparseTexelsResident of uint32 * uint32 * ResidentCode: uint32
   | OpNoLine
   | OpAtomicFlagTestAndSet of uint32 * uint32 * Pointer: uint32 * Memory: uint32 * Semantics: uint32
   | OpAtomicFlagClear of Pointer: uint32 * Memory: uint32 * Semantics: uint32
   | OpImageSparseRead of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * ImageOperands option
   | OpSizeOf of uint32 * uint32 * Pointer: uint32
   | OpTypePipeStorage of uint32
   | OpConstantPipeStorage of uint32 * uint32 * PacketSize: uint32 * PacketAlignment: uint32 * Capacity: uint32
   | OpCreatePipeFromPipeStorage of uint32 * uint32 * PipeStorage: uint32
   | OpGetKernelLocalSizeForSubgroupCount of uint32 * uint32 * SubgroupCount: uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32
   | OpGetKernelMaxNumSubgroups of uint32 * uint32 * Invoke: uint32 * Param: uint32 * ParamSize: uint32 * ParamAlign: uint32
   | OpTypeNamedBarrier of uint32
   | OpNamedBarrierInitialize of uint32 * uint32 * SubgroupCount: uint32
   | OpMemoryNamedBarrier of NamedBarrier: uint32 * Memory: uint32 * Semantics: uint32
   | OpModuleProcessed of Process: string
   | OpExecutionModeId of EntryPoint: uint32 * Mode: ExecutionMode
   | OpDecorateId of Target: uint32 * Decoration
   | OpGroupNonUniformElect of uint32 * uint32 * Execution: uint32
   | OpGroupNonUniformAll of uint32 * uint32 * Execution: uint32 * Predicate: uint32
   | OpGroupNonUniformAny of uint32 * uint32 * Execution: uint32 * Predicate: uint32
   | OpGroupNonUniformAllEqual of uint32 * uint32 * Execution: uint32 * Value: uint32
   | OpGroupNonUniformBroadcast of uint32 * uint32 * Execution: uint32 * Value: uint32 * Id: uint32
   | OpGroupNonUniformBroadcastFirst of uint32 * uint32 * Execution: uint32 * Value: uint32
   | OpGroupNonUniformBallot of uint32 * uint32 * Execution: uint32 * Predicate: uint32
   | OpGroupNonUniformInverseBallot of uint32 * uint32 * Execution: uint32 * Value: uint32
   | OpGroupNonUniformBallotBitExtract of uint32 * uint32 * Execution: uint32 * Value: uint32 * Index: uint32
   | OpGroupNonUniformBallotBitCount of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32
   | OpGroupNonUniformBallotFindLSB of uint32 * uint32 * Execution: uint32 * Value: uint32
   | OpGroupNonUniformBallotFindMSB of uint32 * uint32 * Execution: uint32 * Value: uint32
   | OpGroupNonUniformShuffle of uint32 * uint32 * Execution: uint32 * Value: uint32 * Id: uint32
   | OpGroupNonUniformShuffleXor of uint32 * uint32 * Execution: uint32 * Value: uint32 * Mask: uint32
   | OpGroupNonUniformShuffleUp of uint32 * uint32 * Execution: uint32 * Value: uint32 * Delta: uint32
   | OpGroupNonUniformShuffleDown of uint32 * uint32 * Execution: uint32 * Value: uint32 * Delta: uint32
   | OpGroupNonUniformIAdd of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformFAdd of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformIMul of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformFMul of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformSMin of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformUMin of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformFMin of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformSMax of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformUMax of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformFMax of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformBitwiseAnd of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformBitwiseOr of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformBitwiseXor of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformLogicalAnd of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformLogicalOr of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformLogicalXor of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * Value: uint32 * ClusterSize: uint32 option
   | OpGroupNonUniformQuadBroadcast of uint32 * uint32 * Execution: uint32 * Value: uint32 * Index: uint32
   | OpGroupNonUniformQuadSwap of uint32 * uint32 * Execution: uint32 * Value: uint32 * Direction: uint32
   | OpCopyLogical of uint32 * uint32 * Operand: uint32
   | OpPtrEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpPtrNotEqual of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpPtrDiff of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpSubgroupBallotKHR of uint32 * uint32 * Predicate: uint32
   | OpSubgroupFirstInvocationKHR of uint32 * uint32 * Value: uint32
   | OpSubgroupAllKHR of uint32 * uint32 * Predicate: uint32
   | OpSubgroupAnyKHR of uint32 * uint32 * Predicate: uint32
   | OpSubgroupAllEqualKHR of uint32 * uint32 * Predicate: uint32
   | OpSubgroupReadInvocationKHR of uint32 * uint32 * Value: uint32 * Index: uint32
   | OpGroupIAddNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupFAddNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupFMinNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupUMinNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupSMinNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupFMaxNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupUMaxNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpGroupSMaxNonUniformAMD of uint32 * uint32 * Execution: uint32 * Operation: GroupOperation * X: uint32
   | OpFragmentMaskFetchAMD of uint32 * uint32 * Image: uint32 * Coordinate: uint32
   | OpFragmentFetchAMD of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * FragmentIndex: uint32
   | OpReadClockKHR of uint32 * uint32 * Execution: uint32
   | OpImageSampleFootprintNV of uint32 * uint32 * SampledImage: uint32 * Coordinate: uint32 * Granularity: uint32 * Coarse: uint32 * ImageOperands option
   | OpGroupNonUniformPartitionNV of uint32 * uint32 * Value: uint32
   | OpWritePackedPrimitiveIndices4x8NV of IndexOffset: uint32 * PackedIndices: uint32
   | OpReportIntersectionNV of uint32 * uint32 * Hit: uint32 * HitKind: uint32
   | OpIgnoreIntersectionNV
   | OpTerminateRayNV
   | OpTraceNV of Accel: uint32 * RayFlags: uint32 * CullMask: uint32 * SBTOffset: uint32 * SBTStride: uint32 * MissIndex: uint32 * RayOrigin: uint32 * RayTmin: uint32 * RayDirection: uint32 * RayTmax: uint32 * PayloadId: uint32
   | OpTypeAccelerationStructureNV of uint32
   | OpExecuteCallableNV of SBTIndex: uint32 * CallableDataId: uint32
   | OpTypeCooperativeMatrixNV of uint32 * ComponentType: uint32 * Execution: uint32 * Rows: uint32 * Columns: uint32
   | OpCooperativeMatrixLoadNV of uint32 * uint32 * Pointer: uint32 * Stride: uint32 * ColumnMajor: uint32 * MemoryAccess option
   | OpCooperativeMatrixStoreNV of Pointer: uint32 * Object: uint32 * Stride: uint32 * ColumnMajor: uint32 * MemoryAccess option
   | OpCooperativeMatrixMulAddNV of uint32 * uint32 * A: uint32 * B: uint32 * C: uint32
   | OpCooperativeMatrixLengthNV of uint32 * uint32 * Type: uint32
   | OpBeginInvocationInterlockEXT
   | OpEndInvocationInterlockEXT
   | OpDemoteToHelperInvocationEXT
   | OpIsHelperInvocationEXT of uint32 * uint32
   | OpSubgroupShuffleINTEL of uint32 * uint32 * Data: uint32 * InvocationId: uint32
   | OpSubgroupShuffleDownINTEL of uint32 * uint32 * Current: uint32 * Next: uint32 * Delta: uint32
   | OpSubgroupShuffleUpINTEL of uint32 * uint32 * Previous: uint32 * Current: uint32 * Delta: uint32
   | OpSubgroupShuffleXorINTEL of uint32 * uint32 * Data: uint32 * Value: uint32
   | OpSubgroupBlockReadINTEL of uint32 * uint32 * Ptr: uint32
   | OpSubgroupBlockWriteINTEL of Ptr: uint32 * Data: uint32
   | OpSubgroupImageBlockReadINTEL of uint32 * uint32 * Image: uint32 * Coordinate: uint32
   | OpSubgroupImageBlockWriteINTEL of Image: uint32 * Coordinate: uint32 * Data: uint32
   | OpSubgroupImageMediaBlockReadINTEL of uint32 * uint32 * Image: uint32 * Coordinate: uint32 * Width: uint32 * Height: uint32
   | OpSubgroupImageMediaBlockWriteINTEL of Image: uint32 * Coordinate: uint32 * Width: uint32 * Height: uint32 * Data: uint32
   | OpUCountLeadingZerosINTEL of uint32 * uint32 * Operand: uint32
   | OpUCountTrailingZerosINTEL of uint32 * uint32 * Operand: uint32
   | OpAbsISubINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpAbsUSubINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpIAddSatINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUAddSatINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpIAverageINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUAverageINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpIAverageRoundedINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUAverageRoundedINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpISubSatINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUSubSatINTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpIMul32x16INTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpUMul32x16INTEL of uint32 * uint32 * Operand1: uint32 * Operand2: uint32
   | OpDecorateString of Target: uint32 * Decoration
   | OpMemberDecorateString of StructType: uint32 * Member: uint32 * Decoration
   | OpVmeImageINTEL of uint32 * uint32 * ImageType: uint32 * Sampler: uint32
   | OpTypeVmeImageINTEL of uint32 * ImageType: uint32
   | OpTypeAvcImePayloadINTEL of uint32
   | OpTypeAvcRefPayloadINTEL of uint32
   | OpTypeAvcSicPayloadINTEL of uint32
   | OpTypeAvcMcePayloadINTEL of uint32
   | OpTypeAvcMceResultINTEL of uint32
   | OpTypeAvcImeResultINTEL of uint32
   | OpTypeAvcImeResultSingleReferenceStreamoutINTEL of uint32
   | OpTypeAvcImeResultDualReferenceStreamoutINTEL of uint32
   | OpTypeAvcImeSingleReferenceStreaminINTEL of uint32
   | OpTypeAvcImeDualReferenceStreaminINTEL of uint32
   | OpTypeAvcRefResultINTEL of uint32
   | OpTypeAvcSicResultINTEL of uint32
   | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL of uint32 * uint32 * SliceType: uint32 * Qp: uint32
   | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL of uint32 * uint32 * ReferenceBasePenalty: uint32 * Payload: uint32
   | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL of uint32 * uint32 * SliceType: uint32 * Qp: uint32
   | OpSubgroupAvcMceSetInterShapePenaltyINTEL of uint32 * uint32 * PackedShapePenalty: uint32 * Payload: uint32
   | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL of uint32 * uint32 * SliceType: uint32 * Qp: uint32
   | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL of uint32 * uint32 * DirectionCost: uint32 * Payload: uint32
   | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL of uint32 * uint32 * SliceType: uint32 * Qp: uint32
   | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL of uint32 * uint32 * SliceType: uint32 * Qp: uint32
   | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL of uint32 * uint32
   | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL of uint32 * uint32
   | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL of uint32 * uint32
   | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL of uint32 * uint32 * PackedCostCenterDelta: uint32 * PackedCostTable: uint32 * CostPrecision: uint32 * Payload: uint32
   | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL of uint32 * uint32 * SliceType: uint32 * Qp: uint32
   | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL of uint32 * uint32
   | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL of uint32 * uint32
   | OpSubgroupAvcMceSetAcOnlyHaarINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL of uint32 * uint32 * SourceFieldPolarity: uint32 * Payload: uint32
   | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL of uint32 * uint32 * ReferenceFieldPolarity: uint32 * Payload: uint32
   | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL of uint32 * uint32 * ForwardReferenceFieldPolarity: uint32 * BackwardReferenceFieldPolarity: uint32 * Payload: uint32
   | OpSubgroupAvcMceConvertToImePayloadINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceConvertToImeResultINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceConvertToRefPayloadINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceConvertToRefResultINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceConvertToSicPayloadINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceConvertToSicResultINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetMotionVectorsINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterDistortionsINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetBestInterDistortionsINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterMajorShapeINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterMinorShapeINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterDirectionsINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterMotionVectorCountINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterReferenceIdsINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL of uint32 * uint32 * PackedReferenceIds: uint32 * PackedReferenceParameterFieldPolarities: uint32 * Payload: uint32
   | OpSubgroupAvcImeInitializeINTEL of uint32 * uint32 * SrcCoord: uint32 * PartitionMask: uint32 * SADAdjustment: uint32
   | OpSubgroupAvcImeSetSingleReferenceINTEL of uint32 * uint32 * RefOffset: uint32 * SearchWindowConfig: uint32 * Payload: uint32
   | OpSubgroupAvcImeSetDualReferenceINTEL of uint32 * uint32 * FwdRefOffset: uint32 * BwdRefOffset: uint32 * idSearchWindowConfig: uint32 * Payload: uint32
   | OpSubgroupAvcImeRefWindowSizeINTEL of uint32 * uint32 * SearchWindowConfig: uint32 * DualRef: uint32
   | OpSubgroupAvcImeAdjustRefOffsetINTEL of uint32 * uint32 * RefOffset: uint32 * SrcCoord: uint32 * RefWindowSize: uint32 * ImageSize: uint32
   | OpSubgroupAvcImeConvertToMcePayloadINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL of uint32 * uint32 * MaxMotionVectorCount: uint32 * Payload: uint32
   | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL of uint32 * uint32 * Threshold: uint32 * Payload: uint32
   | OpSubgroupAvcImeSetWeightedSadINTEL of uint32 * uint32 * PackedSadWeights: uint32 * Payload: uint32
   | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * RefImage: uint32 * Payload: uint32
   | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * FwdRefImage: uint32 * BwdRefImage: uint32 * Payload: uint32
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL of uint32 * uint32 * SrcImage: uint32 * RefImage: uint32 * Payload: uint32 * StreaminComponents: uint32
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL of uint32 * uint32 * SrcImage: uint32 * FwdRefImage: uint32 * BwdRefImage: uint32 * Payload: uint32 * StreaminComponents: uint32
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL of uint32 * uint32 * SrcImage: uint32 * RefImage: uint32 * Payload: uint32
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL of uint32 * uint32 * SrcImage: uint32 * FwdRefImage: uint32 * BwdRefImage: uint32 * Payload: uint32
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL of uint32 * uint32 * SrcImage: uint32 * RefImage: uint32 * Payload: uint32 * StreaminComponents: uint32
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL of uint32 * uint32 * SrcImage: uint32 * FwdRefImage: uint32 * BwdRefImage: uint32 * Payload: uint32 * StreaminComponents: uint32
   | OpSubgroupAvcImeConvertToMceResultINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeGetDualReferenceStreaminINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL of uint32 * uint32 * Payload: uint32 * MajorShape: uint32
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL of uint32 * uint32 * Payload: uint32 * MajorShape: uint32
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL of uint32 * uint32 * Payload: uint32 * MajorShape: uint32
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL of uint32 * uint32 * Payload: uint32 * MajorShape: uint32 * Direction: uint32
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL of uint32 * uint32 * Payload: uint32 * MajorShape: uint32 * Direction: uint32
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL of uint32 * uint32 * Payload: uint32 * MajorShape: uint32 * Direction: uint32
   | OpSubgroupAvcImeGetBorderReachedINTEL of uint32 * uint32 * ImageSelect: uint32 * Payload: uint32
   | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcFmeInitializeINTEL of uint32 * uint32 * SrcCoord: uint32 * MotionVectors: uint32 * MajorShapes: uint32 * MinorShapes: uint32 * Direction: uint32 * PixelResolution: uint32 * SadAdjustment: uint32
   | OpSubgroupAvcBmeInitializeINTEL of uint32 * uint32 * SrcCoord: uint32 * MotionVectors: uint32 * MajorShapes: uint32 * MinorShapes: uint32 * Direction: uint32 * PixelResolution: uint32 * BidirectionalWeight: uint32 * SadAdjustment: uint32
   | OpSubgroupAvcRefConvertToMcePayloadINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcRefSetBilinearFilterEnableINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * RefImage: uint32 * Payload: uint32
   | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * FwdRefImage: uint32 * BwdRefImage: uint32 * Payload: uint32
   | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * PackedReferenceIds: uint32 * Payload: uint32
   | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL of uint32 * uint32 * SrcImage: uint32 * PackedReferenceIds: uint32 * PackedReferenceFieldPolarities: uint32 * Payload: uint32
   | OpSubgroupAvcRefConvertToMceResultINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicInitializeINTEL of uint32 * uint32 * SrcCoord: uint32
   | OpSubgroupAvcSicConfigureSkcINTEL of uint32 * uint32 * SkipBlockPartitionType: uint32 * SkipMotionVectorMask: uint32 * MotionVectors: uint32 * BidirectionalWeight: uint32 * SadAdjustment: uint32 * Payload: uint32
   | OpSubgroupAvcSicConfigureIpeLumaINTEL of uint32 * uint32 * LumaIntraPartitionMask: uint32 * IntraNeighbourAvailabilty: uint32 * LeftEdgeLumaPixels: uint32 * UpperLeftCornerLumaPixel: uint32 * UpperEdgeLumaPixels: uint32 * UpperRightEdgeLumaPixels: uint32 * SadAdjustment: uint32 * Payload: uint32
   | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL of uint32 * uint32 * LumaIntraPartitionMask: uint32 * IntraNeighbourAvailabilty: uint32 * LeftEdgeLumaPixels: uint32 * UpperLeftCornerLumaPixel: uint32 * UpperEdgeLumaPixels: uint32 * UpperRightEdgeLumaPixels: uint32 * LeftEdgeChromaPixels: uint32 * UpperLeftCornerChromaPixel: uint32 * UpperEdgeChromaPixels: uint32 * SadAdjustment: uint32 * Payload: uint32
   | OpSubgroupAvcSicGetMotionVectorMaskINTEL of uint32 * uint32 * SkipBlockPartitionType: uint32 * Direction: uint32
   | OpSubgroupAvcSicConvertToMcePayloadINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL of uint32 * uint32 * PackedShapePenalty: uint32 * Payload: uint32
   | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL of uint32 * uint32 * LumaModePenalty: uint32 * LumaPackedNeighborModes: uint32 * LumaPackedNonDcPenalty: uint32 * Payload: uint32
   | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL of uint32 * uint32 * ChromaModeBasePenalty: uint32 * Payload: uint32
   | OpSubgroupAvcSicSetBilinearFilterEnableINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL of uint32 * uint32 * PackedSadCoefficients: uint32 * Payload: uint32
   | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL of uint32 * uint32 * BlockBasedSkipType: uint32 * Payload: uint32
   | OpSubgroupAvcSicEvaluateIpeINTEL of uint32 * uint32 * SrcImage: uint32 * Payload: uint32
   | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * RefImage: uint32 * Payload: uint32
   | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * FwdRefImage: uint32 * BwdRefImage: uint32 * Payload: uint32
   | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL of uint32 * uint32 * SrcImage: uint32 * PackedReferenceIds: uint32 * Payload: uint32
   | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL of uint32 * uint32 * SrcImage: uint32 * PackedReferenceIds: uint32 * PackedReferenceFieldPolarities: uint32 * Payload: uint32
   | OpSubgroupAvcSicConvertToMceResultINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetIpeLumaShapeINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetIpeChromaModeINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL of uint32 * uint32 * Payload: uint32
   | OpSubgroupAvcSicGetInterRawSadsINTEL of uint32 * uint32 * Payload: uint32

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
       | OpMemberDecorateString _ -> 5633u
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

    member x.Version =
       match x with
       | OpNop -> 1.0m
       | OpUndef _ -> 1.0m
       | OpSourceContinued _ -> 1.0m
       | OpSource _ -> 1.0m
       | OpSourceExtension _ -> 1.0m
       | OpName _ -> 1.0m
       | OpMemberName _ -> 1.0m
       | OpString _ -> 1.0m
       | OpLine _ -> 1.0m
       | OpExtension _ -> 1.0m
       | OpExtInstImport _ -> 1.0m
       | OpExtInst _ -> 1.0m
       | OpMemoryModel _ -> 1.0m
       | OpEntryPoint _ -> 1.0m
       | OpExecutionMode _ -> 1.0m
       | OpCapability _ -> 1.0m
       | OpTypeVoid _ -> 1.0m
       | OpTypeBool _ -> 1.0m
       | OpTypeInt _ -> 1.0m
       | OpTypeFloat _ -> 1.0m
       | OpTypeVector _ -> 1.0m
       | OpTypeMatrix _ -> 1.0m
       | OpTypeImage _ -> 1.0m
       | OpTypeSampler _ -> 1.0m
       | OpTypeSampledImage _ -> 1.0m
       | OpTypeArray _ -> 1.0m
       | OpTypeRuntimeArray _ -> 1.0m
       | OpTypeStruct _ -> 1.0m
       | OpTypeOpaque _ -> 1.0m
       | OpTypePointer _ -> 1.0m
       | OpTypeFunction _ -> 1.0m
       | OpTypeEvent _ -> 1.0m
       | OpTypeDeviceEvent _ -> 1.0m
       | OpTypeReserveId _ -> 1.0m
       | OpTypeQueue _ -> 1.0m
       | OpTypePipe _ -> 1.0m
       | OpTypeForwardPointer _ -> 1.0m
       | OpConstantTrue _ -> 1.0m
       | OpConstantFalse _ -> 1.0m
       | OpConstant _ -> 1.0m
       | OpConstantComposite _ -> 1.0m
       | OpConstantSampler _ -> 1.0m
       | OpConstantNull _ -> 1.0m
       | OpSpecConstantTrue _ -> 1.0m
       | OpSpecConstantFalse _ -> 1.0m
       | OpSpecConstant _ -> 1.0m
       | OpSpecConstantComposite _ -> 1.0m
       | OpSpecConstantOp _ -> 1.0m
       | OpFunction _ -> 1.0m
       | OpFunctionParameter _ -> 1.0m
       | OpFunctionEnd -> 1.0m
       | OpFunctionCall _ -> 1.0m
       | OpVariable _ -> 1.0m
       | OpImageTexelPointer _ -> 1.0m
       | OpLoad _ -> 1.0m
       | OpStore _ -> 1.0m
       | OpCopyMemory _ -> 1.0m
       | OpCopyMemorySized _ -> 1.0m
       | OpAccessChain _ -> 1.0m
       | OpInBoundsAccessChain _ -> 1.0m
       | OpPtrAccessChain _ -> 1.0m
       | OpArrayLength _ -> 1.0m
       | OpGenericPtrMemSemantics _ -> 1.0m
       | OpInBoundsPtrAccessChain _ -> 1.0m
       | OpDecorate _ -> 1.0m
       | OpMemberDecorate _ -> 1.0m
       | OpDecorationGroup _ -> 1.0m
       | OpGroupDecorate _ -> 1.0m
       | OpGroupMemberDecorate _ -> 1.0m
       | OpVectorExtractDynamic _ -> 1.0m
       | OpVectorInsertDynamic _ -> 1.0m
       | OpVectorShuffle _ -> 1.0m
       | OpCompositeConstruct _ -> 1.0m
       | OpCompositeExtract _ -> 1.0m
       | OpCompositeInsert _ -> 1.0m
       | OpCopyObject _ -> 1.0m
       | OpTranspose _ -> 1.0m
       | OpSampledImage _ -> 1.0m
       | OpImageSampleImplicitLod _ -> 1.0m
       | OpImageSampleExplicitLod _ -> 1.0m
       | OpImageSampleDrefImplicitLod _ -> 1.0m
       | OpImageSampleDrefExplicitLod _ -> 1.0m
       | OpImageSampleProjImplicitLod _ -> 1.0m
       | OpImageSampleProjExplicitLod _ -> 1.0m
       | OpImageSampleProjDrefImplicitLod _ -> 1.0m
       | OpImageSampleProjDrefExplicitLod _ -> 1.0m
       | OpImageFetch _ -> 1.0m
       | OpImageGather _ -> 1.0m
       | OpImageDrefGather _ -> 1.0m
       | OpImageRead _ -> 1.0m
       | OpImageWrite _ -> 1.0m
       | OpImage _ -> 1.0m
       | OpImageQueryFormat _ -> 1.0m
       | OpImageQueryOrder _ -> 1.0m
       | OpImageQuerySizeLod _ -> 1.0m
       | OpImageQuerySize _ -> 1.0m
       | OpImageQueryLod _ -> 1.0m
       | OpImageQueryLevels _ -> 1.0m
       | OpImageQuerySamples _ -> 1.0m
       | OpConvertFToU _ -> 1.0m
       | OpConvertFToS _ -> 1.0m
       | OpConvertSToF _ -> 1.0m
       | OpConvertUToF _ -> 1.0m
       | OpUConvert _ -> 1.0m
       | OpSConvert _ -> 1.0m
       | OpFConvert _ -> 1.0m
       | OpQuantizeToF16 _ -> 1.0m
       | OpConvertPtrToU _ -> 1.0m
       | OpSatConvertSToU _ -> 1.0m
       | OpSatConvertUToS _ -> 1.0m
       | OpConvertUToPtr _ -> 1.0m
       | OpPtrCastToGeneric _ -> 1.0m
       | OpGenericCastToPtr _ -> 1.0m
       | OpGenericCastToPtrExplicit _ -> 1.0m
       | OpBitcast _ -> 1.0m
       | OpSNegate _ -> 1.0m
       | OpFNegate _ -> 1.0m
       | OpIAdd _ -> 1.0m
       | OpFAdd _ -> 1.0m
       | OpISub _ -> 1.0m
       | OpFSub _ -> 1.0m
       | OpIMul _ -> 1.0m
       | OpFMul _ -> 1.0m
       | OpUDiv _ -> 1.0m
       | OpSDiv _ -> 1.0m
       | OpFDiv _ -> 1.0m
       | OpUMod _ -> 1.0m
       | OpSRem _ -> 1.0m
       | OpSMod _ -> 1.0m
       | OpFRem _ -> 1.0m
       | OpFMod _ -> 1.0m
       | OpVectorTimesScalar _ -> 1.0m
       | OpMatrixTimesScalar _ -> 1.0m
       | OpVectorTimesMatrix _ -> 1.0m
       | OpMatrixTimesVector _ -> 1.0m
       | OpMatrixTimesMatrix _ -> 1.0m
       | OpOuterProduct _ -> 1.0m
       | OpDot _ -> 1.0m
       | OpIAddCarry _ -> 1.0m
       | OpISubBorrow _ -> 1.0m
       | OpUMulExtended _ -> 1.0m
       | OpSMulExtended _ -> 1.0m
       | OpAny _ -> 1.0m
       | OpAll _ -> 1.0m
       | OpIsNan _ -> 1.0m
       | OpIsInf _ -> 1.0m
       | OpIsFinite _ -> 1.0m
       | OpIsNormal _ -> 1.0m
       | OpSignBitSet _ -> 1.0m
       | OpLessOrGreater _ -> 1.0m
       | OpOrdered _ -> 1.0m
       | OpUnordered _ -> 1.0m
       | OpLogicalEqual _ -> 1.0m
       | OpLogicalNotEqual _ -> 1.0m
       | OpLogicalOr _ -> 1.0m
       | OpLogicalAnd _ -> 1.0m
       | OpLogicalNot _ -> 1.0m
       | OpSelect _ -> 1.0m
       | OpIEqual _ -> 1.0m
       | OpINotEqual _ -> 1.0m
       | OpUGreaterThan _ -> 1.0m
       | OpSGreaterThan _ -> 1.0m
       | OpUGreaterThanEqual _ -> 1.0m
       | OpSGreaterThanEqual _ -> 1.0m
       | OpULessThan _ -> 1.0m
       | OpSLessThan _ -> 1.0m
       | OpULessThanEqual _ -> 1.0m
       | OpSLessThanEqual _ -> 1.0m
       | OpFOrdEqual _ -> 1.0m
       | OpFUnordEqual _ -> 1.0m
       | OpFOrdNotEqual _ -> 1.0m
       | OpFUnordNotEqual _ -> 1.0m
       | OpFOrdLessThan _ -> 1.0m
       | OpFUnordLessThan _ -> 1.0m
       | OpFOrdGreaterThan _ -> 1.0m
       | OpFUnordGreaterThan _ -> 1.0m
       | OpFOrdLessThanEqual _ -> 1.0m
       | OpFUnordLessThanEqual _ -> 1.0m
       | OpFOrdGreaterThanEqual _ -> 1.0m
       | OpFUnordGreaterThanEqual _ -> 1.0m
       | OpShiftRightLogical _ -> 1.0m
       | OpShiftRightArithmetic _ -> 1.0m
       | OpShiftLeftLogical _ -> 1.0m
       | OpBitwiseOr _ -> 1.0m
       | OpBitwiseXor _ -> 1.0m
       | OpBitwiseAnd _ -> 1.0m
       | OpNot _ -> 1.0m
       | OpBitFieldInsert _ -> 1.0m
       | OpBitFieldSExtract _ -> 1.0m
       | OpBitFieldUExtract _ -> 1.0m
       | OpBitReverse _ -> 1.0m
       | OpBitCount _ -> 1.0m
       | OpDPdx _ -> 1.0m
       | OpDPdy _ -> 1.0m
       | OpFwidth _ -> 1.0m
       | OpDPdxFine _ -> 1.0m
       | OpDPdyFine _ -> 1.0m
       | OpFwidthFine _ -> 1.0m
       | OpDPdxCoarse _ -> 1.0m
       | OpDPdyCoarse _ -> 1.0m
       | OpFwidthCoarse _ -> 1.0m
       | OpEmitVertex -> 1.0m
       | OpEndPrimitive -> 1.0m
       | OpEmitStreamVertex _ -> 1.0m
       | OpEndStreamPrimitive _ -> 1.0m
       | OpControlBarrier _ -> 1.0m
       | OpMemoryBarrier _ -> 1.0m
       | OpAtomicLoad _ -> 1.0m
       | OpAtomicStore _ -> 1.0m
       | OpAtomicExchange _ -> 1.0m
       | OpAtomicCompareExchange _ -> 1.0m
       | OpAtomicCompareExchangeWeak _ -> 1.0m
       | OpAtomicIIncrement _ -> 1.0m
       | OpAtomicIDecrement _ -> 1.0m
       | OpAtomicIAdd _ -> 1.0m
       | OpAtomicISub _ -> 1.0m
       | OpAtomicSMin _ -> 1.0m
       | OpAtomicUMin _ -> 1.0m
       | OpAtomicSMax _ -> 1.0m
       | OpAtomicUMax _ -> 1.0m
       | OpAtomicAnd _ -> 1.0m
       | OpAtomicOr _ -> 1.0m
       | OpAtomicXor _ -> 1.0m
       | OpPhi _ -> 1.0m
       | OpLoopMerge _ -> 1.0m
       | OpSelectionMerge _ -> 1.0m
       | OpLabel _ -> 1.0m
       | OpBranch _ -> 1.0m
       | OpBranchConditional _ -> 1.0m
       | OpSwitch _ -> 1.0m
       | OpKill -> 1.0m
       | OpReturn -> 1.0m
       | OpReturnValue _ -> 1.0m
       | OpUnreachable -> 1.0m
       | OpLifetimeStart _ -> 1.0m
       | OpLifetimeStop _ -> 1.0m
       | OpGroupAsyncCopy _ -> 1.0m
       | OpGroupWaitEvents _ -> 1.0m
       | OpGroupAll _ -> 1.0m
       | OpGroupAny _ -> 1.0m
       | OpGroupBroadcast _ -> 1.0m
       | OpGroupIAdd _ -> 1.0m
       | OpGroupFAdd _ -> 1.0m
       | OpGroupFMin _ -> 1.0m
       | OpGroupUMin _ -> 1.0m
       | OpGroupSMin _ -> 1.0m
       | OpGroupFMax _ -> 1.0m
       | OpGroupUMax _ -> 1.0m
       | OpGroupSMax _ -> 1.0m
       | OpReadPipe _ -> 1.0m
       | OpWritePipe _ -> 1.0m
       | OpReservedReadPipe _ -> 1.0m
       | OpReservedWritePipe _ -> 1.0m
       | OpReserveReadPipePackets _ -> 1.0m
       | OpReserveWritePipePackets _ -> 1.0m
       | OpCommitReadPipe _ -> 1.0m
       | OpCommitWritePipe _ -> 1.0m
       | OpIsValidReserveId _ -> 1.0m
       | OpGetNumPipePackets _ -> 1.0m
       | OpGetMaxPipePackets _ -> 1.0m
       | OpGroupReserveReadPipePackets _ -> 1.0m
       | OpGroupReserveWritePipePackets _ -> 1.0m
       | OpGroupCommitReadPipe _ -> 1.0m
       | OpGroupCommitWritePipe _ -> 1.0m
       | OpEnqueueMarker _ -> 1.0m
       | OpEnqueueKernel _ -> 1.0m
       | OpGetKernelNDrangeSubGroupCount _ -> 1.0m
       | OpGetKernelNDrangeMaxSubGroupSize _ -> 1.0m
       | OpGetKernelWorkGroupSize _ -> 1.0m
       | OpGetKernelPreferredWorkGroupSizeMultiple _ -> 1.0m
       | OpRetainEvent _ -> 1.0m
       | OpReleaseEvent _ -> 1.0m
       | OpCreateUserEvent _ -> 1.0m
       | OpIsValidEvent _ -> 1.0m
       | OpSetUserEventStatus _ -> 1.0m
       | OpCaptureEventProfilingInfo _ -> 1.0m
       | OpGetDefaultQueue _ -> 1.0m
       | OpBuildNDRange _ -> 1.0m
       | OpImageSparseSampleImplicitLod _ -> 1.0m
       | OpImageSparseSampleExplicitLod _ -> 1.0m
       | OpImageSparseSampleDrefImplicitLod _ -> 1.0m
       | OpImageSparseSampleDrefExplicitLod _ -> 1.0m
       | OpImageSparseSampleProjImplicitLod _ -> 1.0m
       | OpImageSparseSampleProjExplicitLod _ -> 1.0m
       | OpImageSparseSampleProjDrefImplicitLod _ -> 1.0m
       | OpImageSparseSampleProjDrefExplicitLod _ -> 1.0m
       | OpImageSparseFetch _ -> 1.0m
       | OpImageSparseGather _ -> 1.0m
       | OpImageSparseDrefGather _ -> 1.0m
       | OpImageSparseTexelsResident _ -> 1.0m
       | OpNoLine -> 1.0m
       | OpAtomicFlagTestAndSet _ -> 1.0m
       | OpAtomicFlagClear _ -> 1.0m
       | OpImageSparseRead _ -> 1.0m
       | OpSizeOf _ -> 1.1m
       | OpTypePipeStorage _ -> 1.1m
       | OpConstantPipeStorage _ -> 1.1m
       | OpCreatePipeFromPipeStorage _ -> 1.1m
       | OpGetKernelLocalSizeForSubgroupCount _ -> 1.1m
       | OpGetKernelMaxNumSubgroups _ -> 1.1m
       | OpTypeNamedBarrier _ -> 1.1m
       | OpNamedBarrierInitialize _ -> 1.1m
       | OpMemoryNamedBarrier _ -> 1.1m
       | OpModuleProcessed _ -> 1.1m
       | OpExecutionModeId _ -> 1.2m
       | OpDecorateId _ -> 1.2m
       | OpGroupNonUniformElect _ -> 1.3m
       | OpGroupNonUniformAll _ -> 1.3m
       | OpGroupNonUniformAny _ -> 1.3m
       | OpGroupNonUniformAllEqual _ -> 1.3m
       | OpGroupNonUniformBroadcast _ -> 1.3m
       | OpGroupNonUniformBroadcastFirst _ -> 1.3m
       | OpGroupNonUniformBallot _ -> 1.3m
       | OpGroupNonUniformInverseBallot _ -> 1.3m
       | OpGroupNonUniformBallotBitExtract _ -> 1.3m
       | OpGroupNonUniformBallotBitCount _ -> 1.3m
       | OpGroupNonUniformBallotFindLSB _ -> 1.3m
       | OpGroupNonUniformBallotFindMSB _ -> 1.3m
       | OpGroupNonUniformShuffle _ -> 1.3m
       | OpGroupNonUniformShuffleXor _ -> 1.3m
       | OpGroupNonUniformShuffleUp _ -> 1.3m
       | OpGroupNonUniformShuffleDown _ -> 1.3m
       | OpGroupNonUniformIAdd _ -> 1.3m
       | OpGroupNonUniformFAdd _ -> 1.3m
       | OpGroupNonUniformIMul _ -> 1.3m
       | OpGroupNonUniformFMul _ -> 1.3m
       | OpGroupNonUniformSMin _ -> 1.3m
       | OpGroupNonUniformUMin _ -> 1.3m
       | OpGroupNonUniformFMin _ -> 1.3m
       | OpGroupNonUniformSMax _ -> 1.3m
       | OpGroupNonUniformUMax _ -> 1.3m
       | OpGroupNonUniformFMax _ -> 1.3m
       | OpGroupNonUniformBitwiseAnd _ -> 1.3m
       | OpGroupNonUniformBitwiseOr _ -> 1.3m
       | OpGroupNonUniformBitwiseXor _ -> 1.3m
       | OpGroupNonUniformLogicalAnd _ -> 1.3m
       | OpGroupNonUniformLogicalOr _ -> 1.3m
       | OpGroupNonUniformLogicalXor _ -> 1.3m
       | OpGroupNonUniformQuadBroadcast _ -> 1.3m
       | OpGroupNonUniformQuadSwap _ -> 1.3m
       | OpCopyLogical _ -> 1.4m
       | OpPtrEqual _ -> 1.4m
       | OpPtrNotEqual _ -> 1.4m
       | OpPtrDiff _ -> 1.4m
       | OpSubgroupBallotKHR _ -> 1.0m
       | OpSubgroupFirstInvocationKHR _ -> 1.0m
       | OpSubgroupAllKHR _ -> 1.0m
       | OpSubgroupAnyKHR _ -> 1.0m
       | OpSubgroupAllEqualKHR _ -> 1.0m
       | OpSubgroupReadInvocationKHR _ -> 1.0m
       | OpGroupIAddNonUniformAMD _ -> 1.0m
       | OpGroupFAddNonUniformAMD _ -> 1.0m
       | OpGroupFMinNonUniformAMD _ -> 1.0m
       | OpGroupUMinNonUniformAMD _ -> 1.0m
       | OpGroupSMinNonUniformAMD _ -> 1.0m
       | OpGroupFMaxNonUniformAMD _ -> 1.0m
       | OpGroupUMaxNonUniformAMD _ -> 1.0m
       | OpGroupSMaxNonUniformAMD _ -> 1.0m
       | OpFragmentMaskFetchAMD _ -> 1.0m
       | OpFragmentFetchAMD _ -> 1.0m
       | OpReadClockKHR _ -> 1.0m
       | OpImageSampleFootprintNV _ -> 1.0m
       | OpGroupNonUniformPartitionNV _ -> 1.0m
       | OpWritePackedPrimitiveIndices4x8NV _ -> 1.0m
       | OpReportIntersectionNV _ -> 1.0m
       | OpIgnoreIntersectionNV -> 1.0m
       | OpTerminateRayNV -> 1.0m
       | OpTraceNV _ -> 1.0m
       | OpTypeAccelerationStructureNV _ -> 1.0m
       | OpExecuteCallableNV _ -> 1.0m
       | OpTypeCooperativeMatrixNV _ -> 1.0m
       | OpCooperativeMatrixLoadNV _ -> 1.0m
       | OpCooperativeMatrixStoreNV _ -> 1.0m
       | OpCooperativeMatrixMulAddNV _ -> 1.0m
       | OpCooperativeMatrixLengthNV _ -> 1.0m
       | OpBeginInvocationInterlockEXT -> 1.0m
       | OpEndInvocationInterlockEXT -> 1.0m
       | OpDemoteToHelperInvocationEXT -> 1.0m
       | OpIsHelperInvocationEXT _ -> 1.0m
       | OpSubgroupShuffleINTEL _ -> 1.0m
       | OpSubgroupShuffleDownINTEL _ -> 1.0m
       | OpSubgroupShuffleUpINTEL _ -> 1.0m
       | OpSubgroupShuffleXorINTEL _ -> 1.0m
       | OpSubgroupBlockReadINTEL _ -> 1.0m
       | OpSubgroupBlockWriteINTEL _ -> 1.0m
       | OpSubgroupImageBlockReadINTEL _ -> 1.0m
       | OpSubgroupImageBlockWriteINTEL _ -> 1.0m
       | OpSubgroupImageMediaBlockReadINTEL _ -> 1.0m
       | OpSubgroupImageMediaBlockWriteINTEL _ -> 1.0m
       | OpUCountLeadingZerosINTEL _ -> 1.0m
       | OpUCountTrailingZerosINTEL _ -> 1.0m
       | OpAbsISubINTEL _ -> 1.0m
       | OpAbsUSubINTEL _ -> 1.0m
       | OpIAddSatINTEL _ -> 1.0m
       | OpUAddSatINTEL _ -> 1.0m
       | OpIAverageINTEL _ -> 1.0m
       | OpUAverageINTEL _ -> 1.0m
       | OpIAverageRoundedINTEL _ -> 1.0m
       | OpUAverageRoundedINTEL _ -> 1.0m
       | OpISubSatINTEL _ -> 1.0m
       | OpUSubSatINTEL _ -> 1.0m
       | OpIMul32x16INTEL _ -> 1.0m
       | OpUMul32x16INTEL _ -> 1.0m
       | OpDecorateString _ -> 1.4m
       | OpMemberDecorateString _ -> 1.4m
       | OpVmeImageINTEL _ -> 1.0m
       | OpTypeVmeImageINTEL _ -> 1.0m
       | OpTypeAvcImePayloadINTEL _ -> 1.0m
       | OpTypeAvcRefPayloadINTEL _ -> 1.0m
       | OpTypeAvcSicPayloadINTEL _ -> 1.0m
       | OpTypeAvcMcePayloadINTEL _ -> 1.0m
       | OpTypeAvcMceResultINTEL _ -> 1.0m
       | OpTypeAvcImeResultINTEL _ -> 1.0m
       | OpTypeAvcImeResultSingleReferenceStreamoutINTEL _ -> 1.0m
       | OpTypeAvcImeResultDualReferenceStreamoutINTEL _ -> 1.0m
       | OpTypeAvcImeSingleReferenceStreaminINTEL _ -> 1.0m
       | OpTypeAvcImeDualReferenceStreaminINTEL _ -> 1.0m
       | OpTypeAvcRefResultINTEL _ -> 1.0m
       | OpTypeAvcSicResultINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetInterShapePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetAcOnlyHaarINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL _ -> 1.0m
       | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL _ -> 1.0m
       | OpSubgroupAvcMceConvertToImePayloadINTEL _ -> 1.0m
       | OpSubgroupAvcMceConvertToImeResultINTEL _ -> 1.0m
       | OpSubgroupAvcMceConvertToRefPayloadINTEL _ -> 1.0m
       | OpSubgroupAvcMceConvertToRefResultINTEL _ -> 1.0m
       | OpSubgroupAvcMceConvertToSicPayloadINTEL _ -> 1.0m
       | OpSubgroupAvcMceConvertToSicResultINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetMotionVectorsINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterDistortionsINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetBestInterDistortionsINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterMajorShapeINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterMinorShapeINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterDirectionsINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterMotionVectorCountINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterReferenceIdsINTEL _ -> 1.0m
       | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL _ -> 1.0m
       | OpSubgroupAvcImeInitializeINTEL _ -> 1.0m
       | OpSubgroupAvcImeSetSingleReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcImeSetDualReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcImeRefWindowSizeINTEL _ -> 1.0m
       | OpSubgroupAvcImeAdjustRefOffsetINTEL _ -> 1.0m
       | OpSubgroupAvcImeConvertToMcePayloadINTEL _ -> 1.0m
       | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL _ -> 1.0m
       | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL _ -> 1.0m
       | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL _ -> 1.0m
       | OpSubgroupAvcImeSetWeightedSadINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL _ -> 1.0m
       | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL _ -> 1.0m
       | OpSubgroupAvcImeConvertToMceResultINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetDualReferenceStreaminINTEL _ -> 1.0m
       | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL _ -> 1.0m
       | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetBorderReachedINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL _ -> 1.0m
       | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL _ -> 1.0m
       | OpSubgroupAvcFmeInitializeINTEL _ -> 1.0m
       | OpSubgroupAvcBmeInitializeINTEL _ -> 1.0m
       | OpSubgroupAvcRefConvertToMcePayloadINTEL _ -> 1.0m
       | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL _ -> 1.0m
       | OpSubgroupAvcRefSetBilinearFilterEnableINTEL _ -> 1.0m
       | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL _ -> 1.0m
       | OpSubgroupAvcRefConvertToMceResultINTEL _ -> 1.0m
       | OpSubgroupAvcSicInitializeINTEL _ -> 1.0m
       | OpSubgroupAvcSicConfigureSkcINTEL _ -> 1.0m
       | OpSubgroupAvcSicConfigureIpeLumaINTEL _ -> 1.0m
       | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetMotionVectorMaskINTEL _ -> 1.0m
       | OpSubgroupAvcSicConvertToMcePayloadINTEL _ -> 1.0m
       | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL _ -> 1.0m
       | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL _ -> 1.0m
       | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL _ -> 1.0m
       | OpSubgroupAvcSicSetBilinearFilterEnableINTEL _ -> 1.0m
       | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL _ -> 1.0m
       | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL _ -> 1.0m
       | OpSubgroupAvcSicEvaluateIpeINTEL _ -> 1.0m
       | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL _ -> 1.0m
       | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL _ -> 1.0m
       | OpSubgroupAvcSicConvertToMceResultINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetIpeLumaShapeINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetIpeChromaModeINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL _ -> 1.0m
       | OpSubgroupAvcSicGetInterRawSadsINTEL _ -> 1.0m

    static member internal Serialize(instr: Instruction, stream: SpirvStream) =
        match instr with
        | OpNop ->
            ()
        | OpUndef(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSourceContinued(arg0) ->
            stream.WriteString(arg0)
        | OpSource(arg0, arg1, arg2, arg3) ->
            stream.WriteEnum(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteOption(arg2, fun v -> stream.WriteUInt32(v))
            stream.WriteOption(arg3, fun v -> stream.WriteString(v))
        | OpSourceExtension(arg0) ->
            stream.WriteString(arg0)
        | OpName(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteString(arg1)
        | OpMemberName(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteString(arg2)
        | OpString(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteString(arg1)
        | OpLine(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpExtension(arg0) ->
            stream.WriteString(arg0)
        | OpExtInstImport(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteString(arg1)
        | OpExtInst(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteList(arg4, fun v -> stream.WriteUInt32(v))
        | OpMemoryModel(arg0, arg1) ->
            stream.WriteEnum(arg0)
            stream.WriteEnum(arg1)
        | OpEntryPoint(arg0, arg1, arg2, arg3) ->
            stream.WriteEnum(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteString(arg2)
            stream.WriteList(arg3, fun v -> stream.WriteUInt32(v))
        | OpExecutionMode(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpCapability(arg0) ->
            stream.WriteEnum(arg0)
        | OpTypeVoid(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeBool(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeInt(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpTypeFloat(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpTypeVector(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpTypeMatrix(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpTypeImage(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteEnum(arg7)
            stream.WriteOption(arg8, fun v -> stream.WriteEnum(v))
        | OpTypeSampler(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeSampledImage(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpTypeArray(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpTypeRuntimeArray(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpTypeStruct(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteList(arg1, fun v -> stream.WriteUInt32(v))
        | OpTypeOpaque(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteString(arg1)
        | OpTypePointer(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
            stream.WriteUInt32(arg2)
        | OpTypeFunction(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteList(arg2, fun v -> stream.WriteUInt32(v))
        | OpTypeEvent(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeDeviceEvent(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeReserveId(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeQueue(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypePipe(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpTypeForwardPointer(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpConstantTrue(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpConstantFalse(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpConstant(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConstantComposite(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteList(arg2, fun v -> stream.WriteUInt32(v))
        | OpConstantSampler(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteEnum(arg4)
        | OpConstantNull(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSpecConstantTrue(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSpecConstantFalse(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSpecConstant(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSpecConstantComposite(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteList(arg2, fun v -> stream.WriteUInt32(v))
        | OpSpecConstantOp(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpFunction(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
            stream.WriteUInt32(arg3)
        | OpFunctionParameter(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpFunctionEnd ->
            ()
        | OpFunctionCall(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteList(arg3, fun v -> stream.WriteUInt32(v))
        | OpVariable(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
            stream.WriteOption(arg3, fun v -> stream.WriteUInt32(v))
        | OpImageTexelPointer(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpLoad(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteOption(arg3, fun v -> stream.WriteEnum(v))
        | OpStore(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteOption(arg2, fun v -> stream.WriteEnum(v))
        | OpCopyMemory(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteOption(arg2, fun v -> stream.WriteEnum(v))
            stream.WriteOption(arg3, fun v -> stream.WriteEnum(v))
        | OpCopyMemorySized(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteOption(arg3, fun v -> stream.WriteEnum(v))
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpAccessChain(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteList(arg3, fun v -> stream.WriteUInt32(v))
        | OpInBoundsAccessChain(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteList(arg3, fun v -> stream.WriteUInt32(v))
        | OpPtrAccessChain(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteList(arg4, fun v -> stream.WriteUInt32(v))
        | OpArrayLength(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGenericPtrMemSemantics(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpInBoundsPtrAccessChain(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteList(arg4, fun v -> stream.WriteUInt32(v))
        | OpDecorate(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpMemberDecorate(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
        | OpDecorationGroup(arg0) ->
            stream.WriteUInt32(arg0)
        | OpGroupDecorate(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteList(arg1, fun v -> stream.WriteUInt32(v))
        | OpGroupMemberDecorate(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteList(arg1, fun v -> match v with PairIdRefLiteralInteger(v_0, v_1) -> stream.WriteUInt32(v_0);stream.WriteUInt32(v_1))
        | OpVectorExtractDynamic(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpVectorInsertDynamic(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpVectorShuffle(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteList(arg4, fun v -> stream.WriteUInt32(v))
        | OpCompositeConstruct(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteList(arg2, fun v -> stream.WriteUInt32(v))
        | OpCompositeExtract(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteList(arg3, fun v -> stream.WriteUInt32(v))
        | OpCompositeInsert(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteList(arg4, fun v -> stream.WriteUInt32(v))
        | OpCopyObject(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpTranspose(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSampledImage(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpImageSampleImplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageSampleExplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteEnum(arg4)
        | OpImageSampleDrefImplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageSampleDrefExplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteEnum(arg5)
        | OpImageSampleProjImplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageSampleProjExplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteEnum(arg4)
        | OpImageSampleProjDrefImplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageSampleProjDrefExplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteEnum(arg5)
        | OpImageFetch(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageGather(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageDrefGather(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageRead(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageWrite(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteOption(arg3, fun v -> stream.WriteEnum(v))
        | OpImage(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageQueryFormat(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageQueryOrder(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageQuerySizeLod(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpImageQuerySize(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageQueryLod(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpImageQueryLevels(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageQuerySamples(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConvertFToU(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConvertFToS(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConvertSToF(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConvertUToF(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpUConvert(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSConvert(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpFConvert(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpQuantizeToF16(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConvertPtrToU(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSatConvertSToU(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSatConvertUToS(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpConvertUToPtr(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpPtrCastToGeneric(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGenericCastToPtr(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGenericCastToPtrExplicit(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
        | OpBitcast(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSNegate(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpFNegate(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpIAdd(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFAdd(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpISub(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFSub(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIMul(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFMul(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUDiv(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSDiv(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFDiv(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUMod(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSRem(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSMod(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFRem(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFMod(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpVectorTimesScalar(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpMatrixTimesScalar(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpVectorTimesMatrix(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpMatrixTimesVector(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpMatrixTimesMatrix(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpOuterProduct(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpDot(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIAddCarry(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpISubBorrow(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUMulExtended(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSMulExtended(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpAny(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpAll(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpIsNan(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpIsInf(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpIsFinite(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpIsNormal(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSignBitSet(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpLessOrGreater(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpOrdered(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUnordered(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpLogicalEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpLogicalNotEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpLogicalOr(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpLogicalAnd(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpLogicalNot(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSelect(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpIEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpINotEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUGreaterThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSGreaterThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUGreaterThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSGreaterThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpULessThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSLessThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpULessThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSLessThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFOrdEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFUnordEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFOrdNotEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFUnordNotEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFOrdLessThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFUnordLessThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFOrdGreaterThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFUnordGreaterThan(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFOrdLessThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFUnordLessThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFOrdGreaterThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFUnordGreaterThanEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpShiftRightLogical(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpShiftRightArithmetic(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpShiftLeftLogical(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpBitwiseOr(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpBitwiseXor(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpBitwiseAnd(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpNot(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpBitFieldInsert(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpBitFieldSExtract(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpBitFieldUExtract(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpBitReverse(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpBitCount(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpDPdx(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpDPdy(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpFwidth(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpDPdxFine(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpDPdyFine(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpFwidthFine(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpDPdxCoarse(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpDPdyCoarse(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpFwidthCoarse(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpEmitVertex ->
            ()
        | OpEndPrimitive ->
            ()
        | OpEmitStreamVertex(arg0) ->
            stream.WriteUInt32(arg0)
        | OpEndStreamPrimitive(arg0) ->
            stream.WriteUInt32(arg0)
        | OpControlBarrier(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpMemoryBarrier(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpAtomicLoad(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpAtomicStore(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpAtomicExchange(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicCompareExchange(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
        | OpAtomicCompareExchangeWeak(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
        | OpAtomicIIncrement(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpAtomicIDecrement(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpAtomicIAdd(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicISub(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicSMin(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicUMin(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicSMax(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicUMax(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicAnd(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicOr(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpAtomicXor(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpPhi(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteList(arg2, fun v -> match v with PairIdRefIdRef(v_0, v_1) -> stream.WriteUInt32(v_0);stream.WriteUInt32(v_1))
        | OpLoopMerge(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
        | OpSelectionMerge(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpLabel(arg0) ->
            stream.WriteUInt32(arg0)
        | OpBranch(arg0) ->
            stream.WriteUInt32(arg0)
        | OpBranchConditional(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteList(arg3, fun v -> stream.WriteUInt32(v))
        | OpSwitch(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteList(arg2, fun v -> match v with PairLiteralIntegerIdRef(v_0, v_1) -> stream.WriteUInt32(v_0);stream.WriteUInt32(v_1))
        | OpKill ->
            ()
        | OpReturn ->
            ()
        | OpReturnValue(arg0) ->
            stream.WriteUInt32(arg0)
        | OpUnreachable ->
            ()
        | OpLifetimeStart(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpLifetimeStop(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpGroupAsyncCopy(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
        | OpGroupWaitEvents(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGroupAll(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupAny(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupBroadcast(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupIAdd(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupFAdd(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupFMin(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupUMin(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupSMin(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupFMax(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupUMax(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupSMax(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpReadPipe(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpWritePipe(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpReservedReadPipe(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
        | OpReservedWritePipe(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
        | OpReserveReadPipePackets(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpReserveWritePipePackets(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpCommitReadPipe(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpCommitWritePipe(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIsValidReserveId(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGetNumPipePackets(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGetMaxPipePackets(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupReserveReadPipePackets(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpGroupReserveWritePipePackets(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpGroupCommitReadPipe(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupCommitWritePipe(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpEnqueueMarker(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpEnqueueKernel(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
            stream.WriteUInt32(arg8)
            stream.WriteUInt32(arg9)
            stream.WriteUInt32(arg10)
            stream.WriteUInt32(arg11)
            stream.WriteList(arg12, fun v -> stream.WriteUInt32(v))
        | OpGetKernelNDrangeSubGroupCount(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpGetKernelNDrangeMaxSubGroupSize(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpGetKernelWorkGroupSize(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpGetKernelPreferredWorkGroupSizeMultiple(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpRetainEvent(arg0) ->
            stream.WriteUInt32(arg0)
        | OpReleaseEvent(arg0) ->
            stream.WriteUInt32(arg0)
        | OpCreateUserEvent(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpIsValidEvent(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSetUserEventStatus(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpCaptureEventProfilingInfo(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGetDefaultQueue(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpBuildNDRange(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpImageSparseSampleImplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageSparseSampleExplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteEnum(arg4)
        | OpImageSparseSampleDrefImplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageSparseSampleDrefExplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteEnum(arg5)
        | OpImageSparseSampleProjImplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageSparseSampleProjExplicitLod(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteEnum(arg4)
        | OpImageSparseSampleProjDrefImplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageSparseSampleProjDrefExplicitLod(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteEnum(arg5)
        | OpImageSparseFetch(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpImageSparseGather(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageSparseDrefGather(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpImageSparseTexelsResident(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpNoLine ->
            ()
        | OpAtomicFlagTestAndSet(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpAtomicFlagClear(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageSparseRead(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpSizeOf(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpTypePipeStorage(arg0) ->
            stream.WriteUInt32(arg0)
        | OpConstantPipeStorage(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpCreatePipeFromPipeStorage(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGetKernelLocalSizeForSubgroupCount(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpGetKernelMaxNumSubgroups(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpTypeNamedBarrier(arg0) ->
            stream.WriteUInt32(arg0)
        | OpNamedBarrierInitialize(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpMemoryNamedBarrier(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpModuleProcessed(arg0) ->
            stream.WriteString(arg0)
        | OpExecutionModeId(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpDecorateId(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpGroupNonUniformElect(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpGroupNonUniformAll(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformAny(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformAllEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformBroadcast(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformBroadcastFirst(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformBallot(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformInverseBallot(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformBallotBitExtract(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformBallotBitCount(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformBallotFindLSB(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformBallotFindMSB(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupNonUniformShuffle(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformShuffleXor(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformShuffleUp(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformShuffleDown(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformIAdd(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformFAdd(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformIMul(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformFMul(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformSMin(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformUMin(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformFMin(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformSMax(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformUMax(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformFMax(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformBitwiseAnd(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformBitwiseOr(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformBitwiseXor(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformLogicalAnd(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformLogicalOr(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformLogicalXor(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteUInt32(v))
        | OpGroupNonUniformQuadBroadcast(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupNonUniformQuadSwap(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpCopyLogical(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpPtrEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpPtrNotEqual(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpPtrDiff(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupBallotKHR(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupFirstInvocationKHR(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAllKHR(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAnyKHR(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAllEqualKHR(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupReadInvocationKHR(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpGroupIAddNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupFAddNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupFMinNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupUMinNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupSMinNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupFMaxNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupUMaxNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpGroupSMaxNonUniformAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteEnum(arg3)
            stream.WriteUInt32(arg4)
        | OpFragmentMaskFetchAMD(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpFragmentFetchAMD(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpReadClockKHR(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpImageSampleFootprintNV(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteOption(arg6, fun v -> stream.WriteEnum(v))
        | OpGroupNonUniformPartitionNV(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpWritePackedPrimitiveIndices4x8NV(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpReportIntersectionNV(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIgnoreIntersectionNV ->
            ()
        | OpTerminateRayNV ->
            ()
        | OpTraceNV(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
            stream.WriteUInt32(arg8)
            stream.WriteUInt32(arg9)
            stream.WriteUInt32(arg10)
        | OpTypeAccelerationStructureNV(arg0) ->
            stream.WriteUInt32(arg0)
        | OpExecuteCallableNV(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpTypeCooperativeMatrixNV(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpCooperativeMatrixLoadNV(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteOption(arg5, fun v -> stream.WriteEnum(v))
        | OpCooperativeMatrixStoreNV(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteOption(arg4, fun v -> stream.WriteEnum(v))
        | OpCooperativeMatrixMulAddNV(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpCooperativeMatrixLengthNV(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpBeginInvocationInterlockEXT ->
            ()
        | OpEndInvocationInterlockEXT ->
            ()
        | OpDemoteToHelperInvocationEXT ->
            ()
        | OpIsHelperInvocationEXT(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupShuffleINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupShuffleDownINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupShuffleUpINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupShuffleXorINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupBlockReadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupBlockWriteINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupImageBlockReadINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupImageBlockWriteINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupImageMediaBlockReadINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupImageMediaBlockWriteINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpUCountLeadingZerosINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpUCountTrailingZerosINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpAbsISubINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpAbsUSubINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIAddSatINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUAddSatINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIAverageINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUAverageINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIAverageRoundedINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUAverageRoundedINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpISubSatINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUSubSatINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpIMul32x16INTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpUMul32x16INTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpDecorateString(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteEnum(arg1)
        | OpMemberDecorateString(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteEnum(arg2)
        | OpVmeImageINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpTypeVmeImageINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpTypeAvcImePayloadINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcRefPayloadINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcSicPayloadINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcMcePayloadINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcMceResultINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcImeResultINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcImeResultSingleReferenceStreamoutINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcImeResultDualReferenceStreamoutINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcImeSingleReferenceStreaminINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcImeDualReferenceStreaminINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcRefResultINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpTypeAvcSicResultINTEL(arg0) ->
            stream.WriteUInt32(arg0)
        | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceSetInterShapePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL(arg0, arg1) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
        | OpSubgroupAvcMceSetAcOnlyHaarINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcMceConvertToImePayloadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceConvertToImeResultINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceConvertToRefPayloadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceConvertToRefResultINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceConvertToSicPayloadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceConvertToSicResultINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetMotionVectorsINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterDistortionsINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetBestInterDistortionsINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterMajorShapeINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterMinorShapeINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterDirectionsINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterMotionVectorCountINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterReferenceIdsINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeInitializeINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeSetSingleReferenceINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeSetDualReferenceINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcImeRefWindowSizeINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeAdjustRefOffsetINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcImeConvertToMcePayloadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeSetWeightedSadINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
        | OpSubgroupAvcImeConvertToMceResultINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeGetDualReferenceStreaminINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcImeGetBorderReachedINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcFmeInitializeINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
            stream.WriteUInt32(arg8)
        | OpSubgroupAvcBmeInitializeINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
            stream.WriteUInt32(arg8)
            stream.WriteUInt32(arg9)
        | OpSubgroupAvcRefConvertToMcePayloadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcRefSetBilinearFilterEnableINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcRefConvertToMceResultINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicInitializeINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicConfigureSkcINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
        | OpSubgroupAvcSicConfigureIpeLumaINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
            stream.WriteUInt32(arg8)
            stream.WriteUInt32(arg9)
        | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL(arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
            stream.WriteUInt32(arg6)
            stream.WriteUInt32(arg7)
            stream.WriteUInt32(arg8)
            stream.WriteUInt32(arg9)
            stream.WriteUInt32(arg10)
            stream.WriteUInt32(arg11)
            stream.WriteUInt32(arg12)
        | OpSubgroupAvcSicGetMotionVectorMaskINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcSicConvertToMcePayloadINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcSicSetBilinearFilterEnableINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcSicEvaluateIpeINTEL(arg0, arg1, arg2, arg3) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
        | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL(arg0, arg1, arg2, arg3, arg4) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
        | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL(arg0, arg1, arg2, arg3, arg4, arg5) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
            stream.WriteUInt32(arg3)
            stream.WriteUInt32(arg4)
            stream.WriteUInt32(arg5)
        | OpSubgroupAvcSicConvertToMceResultINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetIpeLumaShapeINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetIpeChromaModeINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)
        | OpSubgroupAvcSicGetInterRawSadsINTEL(arg0, arg1, arg2) ->
            stream.WriteUInt32(arg0)
            stream.WriteUInt32(arg1)
            stream.WriteUInt32(arg2)

    static member internal Deserialize(opcode: uint32, stream: SpirvStream) =
        match opcode with
        | 0u ->
            OpNop
        | 1u ->
            OpUndef(stream.ReadUInt32(), stream.ReadUInt32())
        | 2u ->
            OpSourceContinued(stream.ReadString())
        | 3u ->
            OpSource(stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()), stream.ReadOption(fun () -> stream.ReadString()))
        | 4u ->
            OpSourceExtension(stream.ReadString())
        | 5u ->
            OpName(stream.ReadUInt32(), stream.ReadString())
        | 6u ->
            OpMemberName(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadString())
        | 7u ->
            OpString(stream.ReadUInt32(), stream.ReadString())
        | 8u ->
            OpLine(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 10u ->
            OpExtension(stream.ReadString())
        | 11u ->
            OpExtInstImport(stream.ReadUInt32(), stream.ReadString())
        | 12u ->
            OpExtInst(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 14u ->
            OpMemoryModel(stream.ReadEnum(), stream.ReadEnum())
        | 15u ->
            OpEntryPoint(stream.ReadEnum(), stream.ReadUInt32(), stream.ReadString(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 16u ->
            OpExecutionMode(stream.ReadUInt32(), stream.ReadEnum())
        | 17u ->
            OpCapability(stream.ReadEnum())
        | 19u ->
            OpTypeVoid(stream.ReadUInt32())
        | 20u ->
            OpTypeBool(stream.ReadUInt32())
        | 21u ->
            OpTypeInt(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 22u ->
            OpTypeFloat(stream.ReadUInt32(), stream.ReadUInt32())
        | 23u ->
            OpTypeVector(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 24u ->
            OpTypeMatrix(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 25u ->
            OpTypeImage(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 26u ->
            OpTypeSampler(stream.ReadUInt32())
        | 27u ->
            OpTypeSampledImage(stream.ReadUInt32(), stream.ReadUInt32())
        | 28u ->
            OpTypeArray(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 29u ->
            OpTypeRuntimeArray(stream.ReadUInt32(), stream.ReadUInt32())
        | 30u ->
            OpTypeStruct(stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 31u ->
            OpTypeOpaque(stream.ReadUInt32(), stream.ReadString())
        | 32u ->
            OpTypePointer(stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 33u ->
            OpTypeFunction(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 34u ->
            OpTypeEvent(stream.ReadUInt32())
        | 35u ->
            OpTypeDeviceEvent(stream.ReadUInt32())
        | 36u ->
            OpTypeReserveId(stream.ReadUInt32())
        | 37u ->
            OpTypeQueue(stream.ReadUInt32())
        | 38u ->
            OpTypePipe(stream.ReadUInt32(), stream.ReadEnum())
        | 39u ->
            OpTypeForwardPointer(stream.ReadUInt32(), stream.ReadEnum())
        | 41u ->
            OpConstantTrue(stream.ReadUInt32(), stream.ReadUInt32())
        | 42u ->
            OpConstantFalse(stream.ReadUInt32(), stream.ReadUInt32())
        | 43u ->
            OpConstant(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 44u ->
            OpConstantComposite(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 45u ->
            OpConstantSampler(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadEnum())
        | 46u ->
            OpConstantNull(stream.ReadUInt32(), stream.ReadUInt32())
        | 48u ->
            OpSpecConstantTrue(stream.ReadUInt32(), stream.ReadUInt32())
        | 49u ->
            OpSpecConstantFalse(stream.ReadUInt32(), stream.ReadUInt32())
        | 50u ->
            OpSpecConstant(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 51u ->
            OpSpecConstantComposite(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 52u ->
            OpSpecConstantOp(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 54u ->
            OpFunction(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 55u ->
            OpFunctionParameter(stream.ReadUInt32(), stream.ReadUInt32())
        | 56u ->
            OpFunctionEnd
        | 57u ->
            OpFunctionCall(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 59u ->
            OpVariable(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 60u ->
            OpImageTexelPointer(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 61u ->
            OpLoad(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 62u ->
            OpStore(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 63u ->
            OpCopyMemory(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 64u ->
            OpCopyMemorySized(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 65u ->
            OpAccessChain(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 66u ->
            OpInBoundsAccessChain(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 67u ->
            OpPtrAccessChain(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 68u ->
            OpArrayLength(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 69u ->
            OpGenericPtrMemSemantics(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 70u ->
            OpInBoundsPtrAccessChain(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 71u ->
            OpDecorate(stream.ReadUInt32(), stream.ReadEnum())
        | 72u ->
            OpMemberDecorate(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 73u ->
            OpDecorationGroup(stream.ReadUInt32())
        | 74u ->
            OpGroupDecorate(stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 75u ->
            OpGroupMemberDecorate(stream.ReadUInt32(), stream.ReadList(fun () -> PairIdRefLiteralInteger(stream.ReadUInt32(), stream.ReadUInt32())))
        | 77u ->
            OpVectorExtractDynamic(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 78u ->
            OpVectorInsertDynamic(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 79u ->
            OpVectorShuffle(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 80u ->
            OpCompositeConstruct(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 81u ->
            OpCompositeExtract(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 82u ->
            OpCompositeInsert(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 83u ->
            OpCopyObject(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 84u ->
            OpTranspose(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 86u ->
            OpSampledImage(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 87u ->
            OpImageSampleImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 88u ->
            OpImageSampleExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 89u ->
            OpImageSampleDrefImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 90u ->
            OpImageSampleDrefExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 91u ->
            OpImageSampleProjImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 92u ->
            OpImageSampleProjExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 93u ->
            OpImageSampleProjDrefImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 94u ->
            OpImageSampleProjDrefExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 95u ->
            OpImageFetch(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 96u ->
            OpImageGather(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 97u ->
            OpImageDrefGather(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 98u ->
            OpImageRead(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 99u ->
            OpImageWrite(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 100u ->
            OpImage(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 101u ->
            OpImageQueryFormat(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 102u ->
            OpImageQueryOrder(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 103u ->
            OpImageQuerySizeLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 104u ->
            OpImageQuerySize(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 105u ->
            OpImageQueryLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 106u ->
            OpImageQueryLevels(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 107u ->
            OpImageQuerySamples(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 109u ->
            OpConvertFToU(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 110u ->
            OpConvertFToS(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 111u ->
            OpConvertSToF(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 112u ->
            OpConvertUToF(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 113u ->
            OpUConvert(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 114u ->
            OpSConvert(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 115u ->
            OpFConvert(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 116u ->
            OpQuantizeToF16(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 117u ->
            OpConvertPtrToU(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 118u ->
            OpSatConvertSToU(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 119u ->
            OpSatConvertUToS(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 120u ->
            OpConvertUToPtr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 121u ->
            OpPtrCastToGeneric(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 122u ->
            OpGenericCastToPtr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 123u ->
            OpGenericCastToPtrExplicit(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 124u ->
            OpBitcast(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 126u ->
            OpSNegate(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 127u ->
            OpFNegate(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 128u ->
            OpIAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 129u ->
            OpFAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 130u ->
            OpISub(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 131u ->
            OpFSub(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 132u ->
            OpIMul(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 133u ->
            OpFMul(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 134u ->
            OpUDiv(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 135u ->
            OpSDiv(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 136u ->
            OpFDiv(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 137u ->
            OpUMod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 138u ->
            OpSRem(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 139u ->
            OpSMod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 140u ->
            OpFRem(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 141u ->
            OpFMod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 142u ->
            OpVectorTimesScalar(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 143u ->
            OpMatrixTimesScalar(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 144u ->
            OpVectorTimesMatrix(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 145u ->
            OpMatrixTimesVector(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 146u ->
            OpMatrixTimesMatrix(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 147u ->
            OpOuterProduct(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 148u ->
            OpDot(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 149u ->
            OpIAddCarry(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 150u ->
            OpISubBorrow(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 151u ->
            OpUMulExtended(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 152u ->
            OpSMulExtended(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 154u ->
            OpAny(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 155u ->
            OpAll(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 156u ->
            OpIsNan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 157u ->
            OpIsInf(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 158u ->
            OpIsFinite(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 159u ->
            OpIsNormal(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 160u ->
            OpSignBitSet(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 161u ->
            OpLessOrGreater(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 162u ->
            OpOrdered(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 163u ->
            OpUnordered(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 164u ->
            OpLogicalEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 165u ->
            OpLogicalNotEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 166u ->
            OpLogicalOr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 167u ->
            OpLogicalAnd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 168u ->
            OpLogicalNot(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 169u ->
            OpSelect(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 170u ->
            OpIEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 171u ->
            OpINotEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 172u ->
            OpUGreaterThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 173u ->
            OpSGreaterThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 174u ->
            OpUGreaterThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 175u ->
            OpSGreaterThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 176u ->
            OpULessThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 177u ->
            OpSLessThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 178u ->
            OpULessThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 179u ->
            OpSLessThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 180u ->
            OpFOrdEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 181u ->
            OpFUnordEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 182u ->
            OpFOrdNotEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 183u ->
            OpFUnordNotEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 184u ->
            OpFOrdLessThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 185u ->
            OpFUnordLessThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 186u ->
            OpFOrdGreaterThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 187u ->
            OpFUnordGreaterThan(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 188u ->
            OpFOrdLessThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 189u ->
            OpFUnordLessThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 190u ->
            OpFOrdGreaterThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 191u ->
            OpFUnordGreaterThanEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 194u ->
            OpShiftRightLogical(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 195u ->
            OpShiftRightArithmetic(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 196u ->
            OpShiftLeftLogical(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 197u ->
            OpBitwiseOr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 198u ->
            OpBitwiseXor(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 199u ->
            OpBitwiseAnd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 200u ->
            OpNot(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 201u ->
            OpBitFieldInsert(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 202u ->
            OpBitFieldSExtract(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 203u ->
            OpBitFieldUExtract(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 204u ->
            OpBitReverse(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 205u ->
            OpBitCount(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 207u ->
            OpDPdx(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 208u ->
            OpDPdy(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 209u ->
            OpFwidth(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 210u ->
            OpDPdxFine(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 211u ->
            OpDPdyFine(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 212u ->
            OpFwidthFine(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 213u ->
            OpDPdxCoarse(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 214u ->
            OpDPdyCoarse(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 215u ->
            OpFwidthCoarse(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 218u ->
            OpEmitVertex
        | 219u ->
            OpEndPrimitive
        | 220u ->
            OpEmitStreamVertex(stream.ReadUInt32())
        | 221u ->
            OpEndStreamPrimitive(stream.ReadUInt32())
        | 224u ->
            OpControlBarrier(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 225u ->
            OpMemoryBarrier(stream.ReadUInt32(), stream.ReadUInt32())
        | 227u ->
            OpAtomicLoad(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 228u ->
            OpAtomicStore(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 229u ->
            OpAtomicExchange(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 230u ->
            OpAtomicCompareExchange(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 231u ->
            OpAtomicCompareExchangeWeak(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 232u ->
            OpAtomicIIncrement(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 233u ->
            OpAtomicIDecrement(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 234u ->
            OpAtomicIAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 235u ->
            OpAtomicISub(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 236u ->
            OpAtomicSMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 237u ->
            OpAtomicUMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 238u ->
            OpAtomicSMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 239u ->
            OpAtomicUMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 240u ->
            OpAtomicAnd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 241u ->
            OpAtomicOr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 242u ->
            OpAtomicXor(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 245u ->
            OpPhi(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> PairIdRefIdRef(stream.ReadUInt32(), stream.ReadUInt32())))
        | 246u ->
            OpLoopMerge(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 247u ->
            OpSelectionMerge(stream.ReadUInt32(), stream.ReadEnum())
        | 248u ->
            OpLabel(stream.ReadUInt32())
        | 249u ->
            OpBranch(stream.ReadUInt32())
        | 250u ->
            OpBranchConditional(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 251u ->
            OpSwitch(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> PairLiteralIntegerIdRef(stream.ReadUInt32(), stream.ReadUInt32())))
        | 252u ->
            OpKill
        | 253u ->
            OpReturn
        | 254u ->
            OpReturnValue(stream.ReadUInt32())
        | 255u ->
            OpUnreachable
        | 256u ->
            OpLifetimeStart(stream.ReadUInt32(), stream.ReadUInt32())
        | 257u ->
            OpLifetimeStop(stream.ReadUInt32(), stream.ReadUInt32())
        | 259u ->
            OpGroupAsyncCopy(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 260u ->
            OpGroupWaitEvents(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 261u ->
            OpGroupAll(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 262u ->
            OpGroupAny(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 263u ->
            OpGroupBroadcast(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 264u ->
            OpGroupIAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 265u ->
            OpGroupFAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 266u ->
            OpGroupFMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 267u ->
            OpGroupUMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 268u ->
            OpGroupSMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 269u ->
            OpGroupFMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 270u ->
            OpGroupUMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 271u ->
            OpGroupSMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 274u ->
            OpReadPipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 275u ->
            OpWritePipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 276u ->
            OpReservedReadPipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 277u ->
            OpReservedWritePipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 278u ->
            OpReserveReadPipePackets(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 279u ->
            OpReserveWritePipePackets(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 280u ->
            OpCommitReadPipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 281u ->
            OpCommitWritePipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 282u ->
            OpIsValidReserveId(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 283u ->
            OpGetNumPipePackets(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 284u ->
            OpGetMaxPipePackets(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 285u ->
            OpGroupReserveReadPipePackets(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 286u ->
            OpGroupReserveWritePipePackets(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 287u ->
            OpGroupCommitReadPipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 288u ->
            OpGroupCommitWritePipe(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 291u ->
            OpEnqueueMarker(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 292u ->
            OpEnqueueKernel(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadList(fun () -> stream.ReadUInt32()))
        | 293u ->
            OpGetKernelNDrangeSubGroupCount(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 294u ->
            OpGetKernelNDrangeMaxSubGroupSize(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 295u ->
            OpGetKernelWorkGroupSize(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 296u ->
            OpGetKernelPreferredWorkGroupSizeMultiple(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 297u ->
            OpRetainEvent(stream.ReadUInt32())
        | 298u ->
            OpReleaseEvent(stream.ReadUInt32())
        | 299u ->
            OpCreateUserEvent(stream.ReadUInt32(), stream.ReadUInt32())
        | 300u ->
            OpIsValidEvent(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 301u ->
            OpSetUserEventStatus(stream.ReadUInt32(), stream.ReadUInt32())
        | 302u ->
            OpCaptureEventProfilingInfo(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 303u ->
            OpGetDefaultQueue(stream.ReadUInt32(), stream.ReadUInt32())
        | 304u ->
            OpBuildNDRange(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 305u ->
            OpImageSparseSampleImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 306u ->
            OpImageSparseSampleExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 307u ->
            OpImageSparseSampleDrefImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 308u ->
            OpImageSparseSampleDrefExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 309u ->
            OpImageSparseSampleProjImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 310u ->
            OpImageSparseSampleProjExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 311u ->
            OpImageSparseSampleProjDrefImplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 312u ->
            OpImageSparseSampleProjDrefExplicitLod(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 313u ->
            OpImageSparseFetch(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 314u ->
            OpImageSparseGather(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 315u ->
            OpImageSparseDrefGather(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 316u ->
            OpImageSparseTexelsResident(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 317u ->
            OpNoLine
        | 318u ->
            OpAtomicFlagTestAndSet(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 319u ->
            OpAtomicFlagClear(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 320u ->
            OpImageSparseRead(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 321u ->
            OpSizeOf(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 322u ->
            OpTypePipeStorage(stream.ReadUInt32())
        | 323u ->
            OpConstantPipeStorage(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 324u ->
            OpCreatePipeFromPipeStorage(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 325u ->
            OpGetKernelLocalSizeForSubgroupCount(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 326u ->
            OpGetKernelMaxNumSubgroups(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 327u ->
            OpTypeNamedBarrier(stream.ReadUInt32())
        | 328u ->
            OpNamedBarrierInitialize(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 329u ->
            OpMemoryNamedBarrier(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 330u ->
            OpModuleProcessed(stream.ReadString())
        | 331u ->
            OpExecutionModeId(stream.ReadUInt32(), stream.ReadEnum())
        | 332u ->
            OpDecorateId(stream.ReadUInt32(), stream.ReadEnum())
        | 333u ->
            OpGroupNonUniformElect(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 334u ->
            OpGroupNonUniformAll(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 335u ->
            OpGroupNonUniformAny(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 336u ->
            OpGroupNonUniformAllEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 337u ->
            OpGroupNonUniformBroadcast(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 338u ->
            OpGroupNonUniformBroadcastFirst(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 339u ->
            OpGroupNonUniformBallot(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 340u ->
            OpGroupNonUniformInverseBallot(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 341u ->
            OpGroupNonUniformBallotBitExtract(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 342u ->
            OpGroupNonUniformBallotBitCount(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 343u ->
            OpGroupNonUniformBallotFindLSB(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 344u ->
            OpGroupNonUniformBallotFindMSB(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 345u ->
            OpGroupNonUniformShuffle(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 346u ->
            OpGroupNonUniformShuffleXor(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 347u ->
            OpGroupNonUniformShuffleUp(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 348u ->
            OpGroupNonUniformShuffleDown(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 349u ->
            OpGroupNonUniformIAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 350u ->
            OpGroupNonUniformFAdd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 351u ->
            OpGroupNonUniformIMul(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 352u ->
            OpGroupNonUniformFMul(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 353u ->
            OpGroupNonUniformSMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 354u ->
            OpGroupNonUniformUMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 355u ->
            OpGroupNonUniformFMin(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 356u ->
            OpGroupNonUniformSMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 357u ->
            OpGroupNonUniformUMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 358u ->
            OpGroupNonUniformFMax(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 359u ->
            OpGroupNonUniformBitwiseAnd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 360u ->
            OpGroupNonUniformBitwiseOr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 361u ->
            OpGroupNonUniformBitwiseXor(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 362u ->
            OpGroupNonUniformLogicalAnd(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 363u ->
            OpGroupNonUniformLogicalOr(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 364u ->
            OpGroupNonUniformLogicalXor(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadUInt32()))
        | 365u ->
            OpGroupNonUniformQuadBroadcast(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 366u ->
            OpGroupNonUniformQuadSwap(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 400u ->
            OpCopyLogical(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 401u ->
            OpPtrEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 402u ->
            OpPtrNotEqual(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 403u ->
            OpPtrDiff(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 4421u ->
            OpSubgroupBallotKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 4422u ->
            OpSubgroupFirstInvocationKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 4428u ->
            OpSubgroupAllKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 4429u ->
            OpSubgroupAnyKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 4430u ->
            OpSubgroupAllEqualKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 4432u ->
            OpSubgroupReadInvocationKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5000u ->
            OpGroupIAddNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5001u ->
            OpGroupFAddNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5002u ->
            OpGroupFMinNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5003u ->
            OpGroupUMinNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5004u ->
            OpGroupSMinNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5005u ->
            OpGroupFMaxNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5006u ->
            OpGroupUMaxNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5007u ->
            OpGroupSMaxNonUniformAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum(), stream.ReadUInt32())
        | 5011u ->
            OpFragmentMaskFetchAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5012u ->
            OpFragmentFetchAMD(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5056u ->
            OpReadClockKHR(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5283u ->
            OpImageSampleFootprintNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 5296u ->
            OpGroupNonUniformPartitionNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5299u ->
            OpWritePackedPrimitiveIndices4x8NV(stream.ReadUInt32(), stream.ReadUInt32())
        | 5334u ->
            OpReportIntersectionNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5335u ->
            OpIgnoreIntersectionNV
        | 5336u ->
            OpTerminateRayNV
        | 5337u ->
            OpTraceNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5341u ->
            OpTypeAccelerationStructureNV(stream.ReadUInt32())
        | 5344u ->
            OpExecuteCallableNV(stream.ReadUInt32(), stream.ReadUInt32())
        | 5358u ->
            OpTypeCooperativeMatrixNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5359u ->
            OpCooperativeMatrixLoadNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 5360u ->
            OpCooperativeMatrixStoreNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadOption(fun () -> stream.ReadEnum()))
        | 5361u ->
            OpCooperativeMatrixMulAddNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5362u ->
            OpCooperativeMatrixLengthNV(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5364u ->
            OpBeginInvocationInterlockEXT
        | 5365u ->
            OpEndInvocationInterlockEXT
        | 5380u ->
            OpDemoteToHelperInvocationEXT
        | 5381u ->
            OpIsHelperInvocationEXT(stream.ReadUInt32(), stream.ReadUInt32())
        | 5571u ->
            OpSubgroupShuffleINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5572u ->
            OpSubgroupShuffleDownINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5573u ->
            OpSubgroupShuffleUpINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5574u ->
            OpSubgroupShuffleXorINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5575u ->
            OpSubgroupBlockReadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5576u ->
            OpSubgroupBlockWriteINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5577u ->
            OpSubgroupImageBlockReadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5578u ->
            OpSubgroupImageBlockWriteINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5580u ->
            OpSubgroupImageMediaBlockReadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5581u ->
            OpSubgroupImageMediaBlockWriteINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5585u ->
            OpUCountLeadingZerosINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5586u ->
            OpUCountTrailingZerosINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5587u ->
            OpAbsISubINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5588u ->
            OpAbsUSubINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5589u ->
            OpIAddSatINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5590u ->
            OpUAddSatINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5591u ->
            OpIAverageINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5592u ->
            OpUAverageINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5593u ->
            OpIAverageRoundedINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5594u ->
            OpUAverageRoundedINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5595u ->
            OpISubSatINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5596u ->
            OpUSubSatINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5597u ->
            OpIMul32x16INTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5598u ->
            OpUMul32x16INTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5632u ->
            OpDecorateString(stream.ReadUInt32(), stream.ReadEnum())
        | 5633u ->
            OpMemberDecorateString(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadEnum())
        | 5699u ->
            OpVmeImageINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5700u ->
            OpTypeVmeImageINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5701u ->
            OpTypeAvcImePayloadINTEL(stream.ReadUInt32())
        | 5702u ->
            OpTypeAvcRefPayloadINTEL(stream.ReadUInt32())
        | 5703u ->
            OpTypeAvcSicPayloadINTEL(stream.ReadUInt32())
        | 5704u ->
            OpTypeAvcMcePayloadINTEL(stream.ReadUInt32())
        | 5705u ->
            OpTypeAvcMceResultINTEL(stream.ReadUInt32())
        | 5706u ->
            OpTypeAvcImeResultINTEL(stream.ReadUInt32())
        | 5707u ->
            OpTypeAvcImeResultSingleReferenceStreamoutINTEL(stream.ReadUInt32())
        | 5708u ->
            OpTypeAvcImeResultDualReferenceStreamoutINTEL(stream.ReadUInt32())
        | 5709u ->
            OpTypeAvcImeSingleReferenceStreaminINTEL(stream.ReadUInt32())
        | 5710u ->
            OpTypeAvcImeDualReferenceStreaminINTEL(stream.ReadUInt32())
        | 5711u ->
            OpTypeAvcRefResultINTEL(stream.ReadUInt32())
        | 5712u ->
            OpTypeAvcSicResultINTEL(stream.ReadUInt32())
        | 5713u ->
            OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5714u ->
            OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5715u ->
            OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5716u ->
            OpSubgroupAvcMceSetInterShapePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5717u ->
            OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5718u ->
            OpSubgroupAvcMceSetInterDirectionPenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5719u ->
            OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5720u ->
            OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5721u ->
            OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5722u ->
            OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5723u ->
            OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5724u ->
            OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5725u ->
            OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5726u ->
            OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5727u ->
            OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32())
        | 5728u ->
            OpSubgroupAvcMceSetAcOnlyHaarINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5729u ->
            OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5730u ->
            OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5731u ->
            OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5732u ->
            OpSubgroupAvcMceConvertToImePayloadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5733u ->
            OpSubgroupAvcMceConvertToImeResultINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5734u ->
            OpSubgroupAvcMceConvertToRefPayloadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5735u ->
            OpSubgroupAvcMceConvertToRefResultINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5736u ->
            OpSubgroupAvcMceConvertToSicPayloadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5737u ->
            OpSubgroupAvcMceConvertToSicResultINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5738u ->
            OpSubgroupAvcMceGetMotionVectorsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5739u ->
            OpSubgroupAvcMceGetInterDistortionsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5740u ->
            OpSubgroupAvcMceGetBestInterDistortionsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5741u ->
            OpSubgroupAvcMceGetInterMajorShapeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5742u ->
            OpSubgroupAvcMceGetInterMinorShapeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5743u ->
            OpSubgroupAvcMceGetInterDirectionsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5744u ->
            OpSubgroupAvcMceGetInterMotionVectorCountINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5745u ->
            OpSubgroupAvcMceGetInterReferenceIdsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5746u ->
            OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5747u ->
            OpSubgroupAvcImeInitializeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5748u ->
            OpSubgroupAvcImeSetSingleReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5749u ->
            OpSubgroupAvcImeSetDualReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5750u ->
            OpSubgroupAvcImeRefWindowSizeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5751u ->
            OpSubgroupAvcImeAdjustRefOffsetINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5752u ->
            OpSubgroupAvcImeConvertToMcePayloadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5753u ->
            OpSubgroupAvcImeSetMaxMotionVectorCountINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5754u ->
            OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5755u ->
            OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5756u ->
            OpSubgroupAvcImeSetWeightedSadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5757u ->
            OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5758u ->
            OpSubgroupAvcImeEvaluateWithDualReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5759u ->
            OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5760u ->
            OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5761u ->
            OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5762u ->
            OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5763u ->
            OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5764u ->
            OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5765u ->
            OpSubgroupAvcImeConvertToMceResultINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5766u ->
            OpSubgroupAvcImeGetSingleReferenceStreaminINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5767u ->
            OpSubgroupAvcImeGetDualReferenceStreaminINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5768u ->
            OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5769u ->
            OpSubgroupAvcImeStripDualReferenceStreamoutINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5770u ->
            OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5771u ->
            OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5772u ->
            OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5773u ->
            OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5774u ->
            OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5775u ->
            OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5776u ->
            OpSubgroupAvcImeGetBorderReachedINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5777u ->
            OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5778u ->
            OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5779u ->
            OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5780u ->
            OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5781u ->
            OpSubgroupAvcFmeInitializeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5782u ->
            OpSubgroupAvcBmeInitializeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5783u ->
            OpSubgroupAvcRefConvertToMcePayloadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5784u ->
            OpSubgroupAvcRefSetBidirectionalMixDisableINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5785u ->
            OpSubgroupAvcRefSetBilinearFilterEnableINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5786u ->
            OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5787u ->
            OpSubgroupAvcRefEvaluateWithDualReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5788u ->
            OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5789u ->
            OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5790u ->
            OpSubgroupAvcRefConvertToMceResultINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5791u ->
            OpSubgroupAvcSicInitializeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5792u ->
            OpSubgroupAvcSicConfigureSkcINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5793u ->
            OpSubgroupAvcSicConfigureIpeLumaINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5794u ->
            OpSubgroupAvcSicConfigureIpeLumaChromaINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5795u ->
            OpSubgroupAvcSicGetMotionVectorMaskINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5796u ->
            OpSubgroupAvcSicConvertToMcePayloadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5797u ->
            OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5798u ->
            OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5799u ->
            OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5800u ->
            OpSubgroupAvcSicSetBilinearFilterEnableINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5801u ->
            OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5802u ->
            OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5803u ->
            OpSubgroupAvcSicEvaluateIpeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5804u ->
            OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5805u ->
            OpSubgroupAvcSicEvaluateWithDualReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5806u ->
            OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5807u ->
            OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5808u ->
            OpSubgroupAvcSicConvertToMceResultINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5809u ->
            OpSubgroupAvcSicGetIpeLumaShapeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5810u ->
            OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5811u ->
            OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5812u ->
            OpSubgroupAvcSicGetPackedIpeLumaModesINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5813u ->
            OpSubgroupAvcSicGetIpeChromaModeINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5814u ->
            OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5815u ->
            OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())
        | 5816u ->
            OpSubgroupAvcSicGetInterRawSadsINTEL(stream.ReadUInt32(), stream.ReadUInt32(), stream.ReadUInt32())        | _ -> failwith "invalid opcode" 