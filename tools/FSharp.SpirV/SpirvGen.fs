// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System

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
   | OpUndef of IdResultType * IdResult
   | OpSourceContinued of ContinuedSource: LiteralString
   | OpSource of SourceLanguage * Version: LiteralInteger * File: IdRef option * Source: LiteralString option
   | OpSourceExtension of Extension: LiteralString
   | OpName of Target: IdRef * Name: LiteralString
   | OpMemberName of Type: IdRef * Member: LiteralInteger * Name: LiteralString
   | OpString of IdResult * String: LiteralString
   | OpLine of File: IdRef * Line: LiteralInteger * Column: LiteralInteger
   | OpExtension of Name: LiteralString
   | OpExtInstImport of IdResult * Name: LiteralString
   | OpExtInst of IdResultType * IdResult * Set: IdRef * Instruction: LiteralExtInstInteger * Operand1: IdRef list
   | OpMemoryModel of AddressingModel * MemoryModel
   | OpEntryPoint of ExecutionModel * EntryPoint: IdRef * Name: LiteralString * Interface: IdRef list
   | OpExecutionMode of EntryPoint: IdRef * Mode: ExecutionMode
   | OpCapability of Capability: Capability
   | OpTypeVoid of IdResult
   | OpTypeBool of IdResult
   | OpTypeInt of IdResult * Width: LiteralInteger * Signedness: LiteralInteger
   | OpTypeFloat of IdResult * Width: LiteralInteger
   | OpTypeVector of IdResult * ComponentType: IdRef * ComponentCount: LiteralInteger
   | OpTypeMatrix of IdResult * ColumnType: IdRef * ColumnCount: LiteralInteger
   | OpTypeImage of IdResult * SampledType: IdRef * Dim * Depth: LiteralInteger * Arrayed: LiteralInteger * MS: LiteralInteger * Sampled: LiteralInteger * ImageFormat * AccessQualifier option
   | OpTypeSampler of IdResult
   | OpTypeSampledImage of IdResult * ImageType: IdRef
   | OpTypeArray of IdResult * ElementType: IdRef * Length: IdRef
   | OpTypeRuntimeArray of IdResult * ElementType: IdRef
   | OpTypeStruct of IdResult * Member0type: IdRef list
   | OpTypeOpaque of IdResult * LiteralString
   | OpTypePointer of IdResult * StorageClass * Type: IdRef
   | OpTypeFunction of IdResult * ReturnType: IdRef * Parameter0Type: IdRef list
   | OpTypeEvent of IdResult
   | OpTypeDeviceEvent of IdResult
   | OpTypeReserveId of IdResult
   | OpTypeQueue of IdResult
   | OpTypePipe of IdResult * Qualifier: AccessQualifier
   | OpTypeForwardPointer of PointerType: IdRef * StorageClass
   | OpConstantTrue of IdResultType * IdResult
   | OpConstantFalse of IdResultType * IdResult
   | OpConstant of IdResultType * IdResult * Value: LiteralContextDependentNumber
   | OpConstantComposite of IdResultType * IdResult * Constituents: IdRef list
   | OpConstantSampler of IdResultType * IdResult * SamplerAddressingMode * Param: LiteralInteger * SamplerFilterMode
   | OpConstantNull of IdResultType * IdResult
   | OpSpecConstantTrue of IdResultType * IdResult
   | OpSpecConstantFalse of IdResultType * IdResult
   | OpSpecConstant of IdResultType * IdResult * Value: LiteralContextDependentNumber
   | OpSpecConstantComposite of IdResultType * IdResult * Constituents: IdRef list
   | OpSpecConstantOp of IdResultType * IdResult * Opcode: LiteralSpecConstantOpInteger
   | OpFunction of IdResultType * IdResult * FunctionControl * FunctionType: IdRef
   | OpFunctionParameter of IdResultType * IdResult
   | OpFunctionEnd
   | OpFunctionCall of IdResultType * IdResult * Function: IdRef * Argument0: IdRef list
   | OpVariable of IdResultType * IdResult * StorageClass * Initializer: IdRef option
   | OpImageTexelPointer of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * Sample: IdRef
   | OpLoad of IdResultType * IdResult * Pointer: IdRef * MemoryAccess option
   | OpStore of Pointer: IdRef * Object: IdRef * MemoryAccess option
   | OpCopyMemory of Target: IdRef * Source: IdRef * MemoryAccess option * MemoryAccess option
   | OpCopyMemorySized of Target: IdRef * Source: IdRef * Size: IdRef * MemoryAccess option * MemoryAccess option
   | OpAccessChain of IdResultType * IdResult * Base: IdRef * Indexes: IdRef list
   | OpInBoundsAccessChain of IdResultType * IdResult * Base: IdRef * Indexes: IdRef list
   | OpPtrAccessChain of IdResultType * IdResult * Base: IdRef * Element: IdRef * Indexes: IdRef list
   | OpArrayLength of IdResultType * IdResult * Structure: IdRef * Arraymember: LiteralInteger
   | OpGenericPtrMemSemantics of IdResultType * IdResult * Pointer: IdRef
   | OpInBoundsPtrAccessChain of IdResultType * IdResult * Base: IdRef * Element: IdRef * Indexes: IdRef list
   | OpDecorate of Target: IdRef * Decoration
   | OpMemberDecorate of StructureType: IdRef * Member: LiteralInteger * Decoration
   | OpDecorationGroup of IdResult
   | OpGroupDecorate of DecorationGroup: IdRef * Targets: IdRef list
   | OpGroupMemberDecorate of DecorationGroup: IdRef * Targets: PairIdRefLiteralInteger list
   | OpVectorExtractDynamic of IdResultType * IdResult * Vector: IdRef * Index: IdRef
   | OpVectorInsertDynamic of IdResultType * IdResult * Vector: IdRef * Component: IdRef * Index: IdRef
   | OpVectorShuffle of IdResultType * IdResult * Vector1: IdRef * Vector2: IdRef * Components: LiteralInteger list
   | OpCompositeConstruct of IdResultType * IdResult * Constituents: IdRef list
   | OpCompositeExtract of IdResultType * IdResult * Composite: IdRef * Indexes: LiteralInteger list
   | OpCompositeInsert of IdResultType * IdResult * Object: IdRef * Composite: IdRef * Indexes: LiteralInteger list
   | OpCopyObject of IdResultType * IdResult * Operand: IdRef
   | OpTranspose of IdResultType * IdResult * Matrix: IdRef
   | OpSampledImage of IdResultType * IdResult * Image: IdRef * Sampler: IdRef
   | OpImageSampleImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageSampleExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands
   | OpImageSampleDrefImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands option
   | OpImageSampleDrefExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands
   | OpImageSampleProjImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageSampleProjExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands
   | OpImageSampleProjDrefImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands option
   | OpImageSampleProjDrefExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands
   | OpImageFetch of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageGather of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Component: IdRef * ImageOperands option
   | OpImageDrefGather of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands option
   | OpImageRead of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageWrite of Image: IdRef * Coordinate: IdRef * Texel: IdRef * ImageOperands option
   | OpImage of IdResultType * IdResult * SampledImage: IdRef
   | OpImageQueryFormat of IdResultType * IdResult * Image: IdRef
   | OpImageQueryOrder of IdResultType * IdResult * Image: IdRef
   | OpImageQuerySizeLod of IdResultType * IdResult * Image: IdRef * LevelofDetail: IdRef
   | OpImageQuerySize of IdResultType * IdResult * Image: IdRef
   | OpImageQueryLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef
   | OpImageQueryLevels of IdResultType * IdResult * Image: IdRef
   | OpImageQuerySamples of IdResultType * IdResult * Image: IdRef
   | OpConvertFToU of IdResultType * IdResult * FloatValue: IdRef
   | OpConvertFToS of IdResultType * IdResult * FloatValue: IdRef
   | OpConvertSToF of IdResultType * IdResult * SignedValue: IdRef
   | OpConvertUToF of IdResultType * IdResult * UnsignedValue: IdRef
   | OpUConvert of IdResultType * IdResult * UnsignedValue: IdRef
   | OpSConvert of IdResultType * IdResult * SignedValue: IdRef
   | OpFConvert of IdResultType * IdResult * FloatValue: IdRef
   | OpQuantizeToF16 of IdResultType * IdResult * Value: IdRef
   | OpConvertPtrToU of IdResultType * IdResult * Pointer: IdRef
   | OpSatConvertSToU of IdResultType * IdResult * SignedValue: IdRef
   | OpSatConvertUToS of IdResultType * IdResult * UnsignedValue: IdRef
   | OpConvertUToPtr of IdResultType * IdResult * IntegerValue: IdRef
   | OpPtrCastToGeneric of IdResultType * IdResult * Pointer: IdRef
   | OpGenericCastToPtr of IdResultType * IdResult * Pointer: IdRef
   | OpGenericCastToPtrExplicit of IdResultType * IdResult * Pointer: IdRef * Storage: StorageClass
   | OpBitcast of IdResultType * IdResult * Operand: IdRef
   | OpSNegate of IdResultType * IdResult * Operand: IdRef
   | OpFNegate of IdResultType * IdResult * Operand: IdRef
   | OpIAdd of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFAdd of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpISub of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFSub of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpIMul of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFMul of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUDiv of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSDiv of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFDiv of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUMod of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSRem of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSMod of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFRem of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFMod of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpVectorTimesScalar of IdResultType * IdResult * Vector: IdRef * Scalar: IdRef
   | OpMatrixTimesScalar of IdResultType * IdResult * Matrix: IdRef * Scalar: IdRef
   | OpVectorTimesMatrix of IdResultType * IdResult * Vector: IdRef * Matrix: IdRef
   | OpMatrixTimesVector of IdResultType * IdResult * Matrix: IdRef * Vector: IdRef
   | OpMatrixTimesMatrix of IdResultType * IdResult * LeftMatrix: IdRef * RightMatrix: IdRef
   | OpOuterProduct of IdResultType * IdResult * Vector1: IdRef * Vector2: IdRef
   | OpDot of IdResultType * IdResult * Vector1: IdRef * Vector2: IdRef
   | OpIAddCarry of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpISubBorrow of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUMulExtended of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSMulExtended of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpAny of IdResultType * IdResult * Vector: IdRef
   | OpAll of IdResultType * IdResult * Vector: IdRef
   | OpIsNan of IdResultType * IdResult * x: IdRef
   | OpIsInf of IdResultType * IdResult * x: IdRef
   | OpIsFinite of IdResultType * IdResult * x: IdRef
   | OpIsNormal of IdResultType * IdResult * x: IdRef
   | OpSignBitSet of IdResultType * IdResult * x: IdRef
   | OpLessOrGreater of IdResultType * IdResult * x: IdRef * y: IdRef
   | OpOrdered of IdResultType * IdResult * x: IdRef * y: IdRef
   | OpUnordered of IdResultType * IdResult * x: IdRef * y: IdRef
   | OpLogicalEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpLogicalNotEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpLogicalOr of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpLogicalAnd of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpLogicalNot of IdResultType * IdResult * Operand: IdRef
   | OpSelect of IdResultType * IdResult * Condition: IdRef * Object1: IdRef * Object2: IdRef
   | OpIEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpINotEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUGreaterThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSGreaterThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUGreaterThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSGreaterThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpULessThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSLessThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpULessThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSLessThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFOrdEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFUnordEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFOrdNotEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFUnordNotEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFOrdLessThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFUnordLessThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFOrdGreaterThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFUnordGreaterThan of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFOrdLessThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFUnordLessThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFOrdGreaterThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpFUnordGreaterThanEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpShiftRightLogical of IdResultType * IdResult * Base: IdRef * Shift: IdRef
   | OpShiftRightArithmetic of IdResultType * IdResult * Base: IdRef * Shift: IdRef
   | OpShiftLeftLogical of IdResultType * IdResult * Base: IdRef * Shift: IdRef
   | OpBitwiseOr of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpBitwiseXor of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpBitwiseAnd of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpNot of IdResultType * IdResult * Operand: IdRef
   | OpBitFieldInsert of IdResultType * IdResult * Base: IdRef * Insert: IdRef * Offset: IdRef * Count: IdRef
   | OpBitFieldSExtract of IdResultType * IdResult * Base: IdRef * Offset: IdRef * Count: IdRef
   | OpBitFieldUExtract of IdResultType * IdResult * Base: IdRef * Offset: IdRef * Count: IdRef
   | OpBitReverse of IdResultType * IdResult * Base: IdRef
   | OpBitCount of IdResultType * IdResult * Base: IdRef
   | OpDPdx of IdResultType * IdResult * P: IdRef
   | OpDPdy of IdResultType * IdResult * P: IdRef
   | OpFwidth of IdResultType * IdResult * P: IdRef
   | OpDPdxFine of IdResultType * IdResult * P: IdRef
   | OpDPdyFine of IdResultType * IdResult * P: IdRef
   | OpFwidthFine of IdResultType * IdResult * P: IdRef
   | OpDPdxCoarse of IdResultType * IdResult * P: IdRef
   | OpDPdyCoarse of IdResultType * IdResult * P: IdRef
   | OpFwidthCoarse of IdResultType * IdResult * P: IdRef
   | OpEmitVertex
   | OpEndPrimitive
   | OpEmitStreamVertex of Stream: IdRef
   | OpEndStreamPrimitive of Stream: IdRef
   | OpControlBarrier of Execution: IdScope * Memory: IdScope * Semantics: IdMemorySemantics
   | OpMemoryBarrier of Memory: IdScope * Semantics: IdMemorySemantics
   | OpAtomicLoad of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics
   | OpAtomicStore of Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicExchange of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicCompareExchange of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Equal: IdMemorySemantics * Unequal: IdMemorySemantics * Value: IdRef * Comparator: IdRef
   | OpAtomicCompareExchangeWeak of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Equal: IdMemorySemantics * Unequal: IdMemorySemantics * Value: IdRef * Comparator: IdRef
   | OpAtomicIIncrement of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics
   | OpAtomicIDecrement of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics
   | OpAtomicIAdd of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicISub of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicSMin of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicUMin of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicSMax of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicUMax of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicAnd of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicOr of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpAtomicXor of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics * Value: IdRef
   | OpPhi of IdResultType * IdResult * VariableParent: PairIdRefIdRef list
   | OpLoopMerge of MergeBlock: IdRef * ContinueTarget: IdRef * LoopControl
   | OpSelectionMerge of MergeBlock: IdRef * SelectionControl
   | OpLabel of IdResult
   | OpBranch of TargetLabel: IdRef
   | OpBranchConditional of Condition: IdRef * TrueLabel: IdRef * FalseLabel: IdRef * Branchweights: LiteralInteger list
   | OpSwitch of Selector: IdRef * Default: IdRef * Target: PairLiteralIntegerIdRef list
   | OpKill
   | OpReturn
   | OpReturnValue of Value: IdRef
   | OpUnreachable
   | OpLifetimeStart of Pointer: IdRef * Size: LiteralInteger
   | OpLifetimeStop of Pointer: IdRef * Size: LiteralInteger
   | OpGroupAsyncCopy of IdResultType * IdResult * Execution: IdScope * Destination: IdRef * Source: IdRef * NumElements: IdRef * Stride: IdRef * Event: IdRef
   | OpGroupWaitEvents of Execution: IdScope * NumEvents: IdRef * EventsList: IdRef
   | OpGroupAll of IdResultType * IdResult * Execution: IdScope * Predicate: IdRef
   | OpGroupAny of IdResultType * IdResult * Execution: IdScope * Predicate: IdRef
   | OpGroupBroadcast of IdResultType * IdResult * Execution: IdScope * Value: IdRef * LocalId: IdRef
   | OpGroupIAdd of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupFAdd of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupFMin of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupUMin of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupSMin of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupFMax of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupUMax of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupSMax of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpReadPipe of IdResultType * IdResult * Pipe: IdRef * Pointer: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpWritePipe of IdResultType * IdResult * Pipe: IdRef * Pointer: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpReservedReadPipe of IdResultType * IdResult * Pipe: IdRef * ReserveId: IdRef * Index: IdRef * Pointer: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpReservedWritePipe of IdResultType * IdResult * Pipe: IdRef * ReserveId: IdRef * Index: IdRef * Pointer: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpReserveReadPipePackets of IdResultType * IdResult * Pipe: IdRef * NumPackets: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpReserveWritePipePackets of IdResultType * IdResult * Pipe: IdRef * NumPackets: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpCommitReadPipe of Pipe: IdRef * ReserveId: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpCommitWritePipe of Pipe: IdRef * ReserveId: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpIsValidReserveId of IdResultType * IdResult * ReserveId: IdRef
   | OpGetNumPipePackets of IdResultType * IdResult * Pipe: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpGetMaxPipePackets of IdResultType * IdResult * Pipe: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpGroupReserveReadPipePackets of IdResultType * IdResult * Execution: IdScope * Pipe: IdRef * NumPackets: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpGroupReserveWritePipePackets of IdResultType * IdResult * Execution: IdScope * Pipe: IdRef * NumPackets: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpGroupCommitReadPipe of Execution: IdScope * Pipe: IdRef * ReserveId: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpGroupCommitWritePipe of Execution: IdScope * Pipe: IdRef * ReserveId: IdRef * PacketSize: IdRef * PacketAlignment: IdRef
   | OpEnqueueMarker of IdResultType * IdResult * Queue: IdRef * NumEvents: IdRef * WaitEvents: IdRef * RetEvent: IdRef
   | OpEnqueueKernel of IdResultType * IdResult * Queue: IdRef * Flags: IdRef * NDRange: IdRef * NumEvents: IdRef * WaitEvents: IdRef * RetEvent: IdRef * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef * LocalSize: IdRef list
   | OpGetKernelNDrangeSubGroupCount of IdResultType * IdResult * NDRange: IdRef * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef
   | OpGetKernelNDrangeMaxSubGroupSize of IdResultType * IdResult * NDRange: IdRef * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef
   | OpGetKernelWorkGroupSize of IdResultType * IdResult * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef
   | OpGetKernelPreferredWorkGroupSizeMultiple of IdResultType * IdResult * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef
   | OpRetainEvent of Event: IdRef
   | OpReleaseEvent of Event: IdRef
   | OpCreateUserEvent of IdResultType * IdResult
   | OpIsValidEvent of IdResultType * IdResult * Event: IdRef
   | OpSetUserEventStatus of Event: IdRef * Status: IdRef
   | OpCaptureEventProfilingInfo of Event: IdRef * ProfilingInfo: IdRef * Value: IdRef
   | OpGetDefaultQueue of IdResultType * IdResult
   | OpBuildNDRange of IdResultType * IdResult * GlobalWorkSize: IdRef * LocalWorkSize: IdRef * GlobalWorkOffset: IdRef
   | OpImageSparseSampleImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageSparseSampleExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands
   | OpImageSparseSampleDrefImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands option
   | OpImageSparseSampleDrefExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands
   | OpImageSparseSampleProjImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageSparseSampleProjExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * ImageOperands
   | OpImageSparseSampleProjDrefImplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands option
   | OpImageSparseSampleProjDrefExplicitLod of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands
   | OpImageSparseFetch of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * ImageOperands option
   | OpImageSparseGather of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Component: IdRef * ImageOperands option
   | OpImageSparseDrefGather of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Dref: IdRef * ImageOperands option
   | OpImageSparseTexelsResident of IdResultType * IdResult * ResidentCode: IdRef
   | OpNoLine
   | OpAtomicFlagTestAndSet of IdResultType * IdResult * Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics
   | OpAtomicFlagClear of Pointer: IdRef * Memory: IdScope * Semantics: IdMemorySemantics
   | OpImageSparseRead of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * ImageOperands option
   | OpSizeOf of IdResultType * IdResult * Pointer: IdRef
   | OpTypePipeStorage of IdResult
   | OpConstantPipeStorage of IdResultType * IdResult * PacketSize: LiteralInteger * PacketAlignment: LiteralInteger * Capacity: LiteralInteger
   | OpCreatePipeFromPipeStorage of IdResultType * IdResult * PipeStorage: IdRef
   | OpGetKernelLocalSizeForSubgroupCount of IdResultType * IdResult * SubgroupCount: IdRef * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef
   | OpGetKernelMaxNumSubgroups of IdResultType * IdResult * Invoke: IdRef * Param: IdRef * ParamSize: IdRef * ParamAlign: IdRef
   | OpTypeNamedBarrier of IdResult
   | OpNamedBarrierInitialize of IdResultType * IdResult * SubgroupCount: IdRef
   | OpMemoryNamedBarrier of NamedBarrier: IdRef * Memory: IdScope * Semantics: IdMemorySemantics
   | OpModuleProcessed of Process: LiteralString
   | OpExecutionModeId of EntryPoint: IdRef * Mode: ExecutionMode
   | OpDecorateId of Target: IdRef * Decoration
   | OpGroupNonUniformElect of IdResultType * IdResult * Execution: IdScope
   | OpGroupNonUniformAll of IdResultType * IdResult * Execution: IdScope * Predicate: IdRef
   | OpGroupNonUniformAny of IdResultType * IdResult * Execution: IdScope * Predicate: IdRef
   | OpGroupNonUniformAllEqual of IdResultType * IdResult * Execution: IdScope * Value: IdRef
   | OpGroupNonUniformBroadcast of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Id: IdRef
   | OpGroupNonUniformBroadcastFirst of IdResultType * IdResult * Execution: IdScope * Value: IdRef
   | OpGroupNonUniformBallot of IdResultType * IdResult * Execution: IdScope * Predicate: IdRef
   | OpGroupNonUniformInverseBallot of IdResultType * IdResult * Execution: IdScope * Value: IdRef
   | OpGroupNonUniformBallotBitExtract of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Index: IdRef
   | OpGroupNonUniformBallotBitCount of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef
   | OpGroupNonUniformBallotFindLSB of IdResultType * IdResult * Execution: IdScope * Value: IdRef
   | OpGroupNonUniformBallotFindMSB of IdResultType * IdResult * Execution: IdScope * Value: IdRef
   | OpGroupNonUniformShuffle of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Id: IdRef
   | OpGroupNonUniformShuffleXor of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Mask: IdRef
   | OpGroupNonUniformShuffleUp of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Delta: IdRef
   | OpGroupNonUniformShuffleDown of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Delta: IdRef
   | OpGroupNonUniformIAdd of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformFAdd of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformIMul of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformFMul of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformSMin of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformUMin of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformFMin of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformSMax of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformUMax of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformFMax of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformBitwiseAnd of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformBitwiseOr of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformBitwiseXor of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformLogicalAnd of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformLogicalOr of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformLogicalXor of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * Value: IdRef * ClusterSize: IdRef option
   | OpGroupNonUniformQuadBroadcast of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Index: IdRef
   | OpGroupNonUniformQuadSwap of IdResultType * IdResult * Execution: IdScope * Value: IdRef * Direction: IdRef
   | OpCopyLogical of IdResultType * IdResult * Operand: IdRef
   | OpPtrEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpPtrNotEqual of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpPtrDiff of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpSubgroupBallotKHR of IdResultType * IdResult * Predicate: IdRef
   | OpSubgroupFirstInvocationKHR of IdResultType * IdResult * Value: IdRef
   | OpSubgroupAllKHR of IdResultType * IdResult * Predicate: IdRef
   | OpSubgroupAnyKHR of IdResultType * IdResult * Predicate: IdRef
   | OpSubgroupAllEqualKHR of IdResultType * IdResult * Predicate: IdRef
   | OpSubgroupReadInvocationKHR of IdResultType * IdResult * Value: IdRef * Index: IdRef
   | OpGroupIAddNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupFAddNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupFMinNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupUMinNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupSMinNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupFMaxNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupUMaxNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpGroupSMaxNonUniformAMD of IdResultType * IdResult * Execution: IdScope * Operation: GroupOperation * X: IdRef
   | OpFragmentMaskFetchAMD of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef
   | OpFragmentFetchAMD of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * FragmentIndex: IdRef
   | OpReadClockKHR of IdResultType * IdResult * Execution: IdScope
   | OpImageSampleFootprintNV of IdResultType * IdResult * SampledImage: IdRef * Coordinate: IdRef * Granularity: IdRef * Coarse: IdRef * ImageOperands option
   | OpGroupNonUniformPartitionNV of IdResultType * IdResult * Value: IdRef
   | OpWritePackedPrimitiveIndices4x8NV of IndexOffset: IdRef * PackedIndices: IdRef
   | OpReportIntersectionNV of IdResultType * IdResult * Hit: IdRef * HitKind: IdRef
   | OpIgnoreIntersectionNV
   | OpTerminateRayNV
   | OpTraceNV of Accel: IdRef * RayFlags: IdRef * CullMask: IdRef * SBTOffset: IdRef * SBTStride: IdRef * MissIndex: IdRef * RayOrigin: IdRef * RayTmin: IdRef * RayDirection: IdRef * RayTmax: IdRef * PayloadId: IdRef
   | OpTypeAccelerationStructureNV of IdResult
   | OpExecuteCallableNV of SBTIndex: IdRef * CallableDataId: IdRef
   | OpTypeCooperativeMatrixNV of IdResult * ComponentType: IdRef * Execution: IdScope * Rows: IdRef * Columns: IdRef
   | OpCooperativeMatrixLoadNV of IdResultType * IdResult * Pointer: IdRef * Stride: IdRef * ColumnMajor: IdRef * MemoryAccess option
   | OpCooperativeMatrixStoreNV of Pointer: IdRef * Object: IdRef * Stride: IdRef * ColumnMajor: IdRef * MemoryAccess option
   | OpCooperativeMatrixMulAddNV of IdResultType * IdResult * A: IdRef * B: IdRef * C: IdRef
   | OpCooperativeMatrixLengthNV of IdResultType * IdResult * Type: IdRef
   | OpBeginInvocationInterlockEXT
   | OpEndInvocationInterlockEXT
   | OpDemoteToHelperInvocationEXT
   | OpIsHelperInvocationEXT of IdResultType * IdResult
   | OpSubgroupShuffleINTEL of IdResultType * IdResult * Data: IdRef * InvocationId: IdRef
   | OpSubgroupShuffleDownINTEL of IdResultType * IdResult * Current: IdRef * Next: IdRef * Delta: IdRef
   | OpSubgroupShuffleUpINTEL of IdResultType * IdResult * Previous: IdRef * Current: IdRef * Delta: IdRef
   | OpSubgroupShuffleXorINTEL of IdResultType * IdResult * Data: IdRef * Value: IdRef
   | OpSubgroupBlockReadINTEL of IdResultType * IdResult * Ptr: IdRef
   | OpSubgroupBlockWriteINTEL of Ptr: IdRef * Data: IdRef
   | OpSubgroupImageBlockReadINTEL of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef
   | OpSubgroupImageBlockWriteINTEL of Image: IdRef * Coordinate: IdRef * Data: IdRef
   | OpSubgroupImageMediaBlockReadINTEL of IdResultType * IdResult * Image: IdRef * Coordinate: IdRef * Width: IdRef * Height: IdRef
   | OpSubgroupImageMediaBlockWriteINTEL of Image: IdRef * Coordinate: IdRef * Width: IdRef * Height: IdRef * Data: IdRef
   | OpUCountLeadingZerosINTEL of IdResultType * IdResult * Operand: IdRef
   | OpUCountTrailingZerosINTEL of IdResultType * IdResult * Operand: IdRef
   | OpAbsISubINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpAbsUSubINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpIAddSatINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUAddSatINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpIAverageINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUAverageINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpIAverageRoundedINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUAverageRoundedINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpISubSatINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUSubSatINTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpIMul32x16INTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpUMul32x16INTEL of IdResultType * IdResult * Operand1: IdRef * Operand2: IdRef
   | OpDecorateString of Target: IdRef * Decoration
   | OpDecorateStringGOOGLE of Target: IdRef * Decoration
   | OpMemberDecorateString of StructType: IdRef * Member: LiteralInteger * Decoration
   | OpMemberDecorateStringGOOGLE of StructType: IdRef * Member: LiteralInteger * Decoration
   | OpVmeImageINTEL of IdResultType * IdResult * ImageType: IdRef * Sampler: IdRef
   | OpTypeVmeImageINTEL of IdResult * ImageType: IdRef
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
   | OpSubgroupAvcMceGetDefaultInterBaseMultiReferencePenaltyINTEL of IdResultType * IdResult * SliceType: IdRef * Qp: IdRef
   | OpSubgroupAvcMceSetInterBaseMultiReferencePenaltyINTEL of IdResultType * IdResult * ReferenceBasePenalty: IdRef * Payload: IdRef
   | OpSubgroupAvcMceGetDefaultInterShapePenaltyINTEL of IdResultType * IdResult * SliceType: IdRef * Qp: IdRef
   | OpSubgroupAvcMceSetInterShapePenaltyINTEL of IdResultType * IdResult * PackedShapePenalty: IdRef * Payload: IdRef
   | OpSubgroupAvcMceGetDefaultInterDirectionPenaltyINTEL of IdResultType * IdResult * SliceType: IdRef * Qp: IdRef
   | OpSubgroupAvcMceSetInterDirectionPenaltyINTEL of IdResultType * IdResult * DirectionCost: IdRef * Payload: IdRef
   | OpSubgroupAvcMceGetDefaultIntraLumaShapePenaltyINTEL of IdResultType * IdResult * SliceType: IdRef * Qp: IdRef
   | OpSubgroupAvcMceGetDefaultInterMotionVectorCostTableINTEL of IdResultType * IdResult * SliceType: IdRef * Qp: IdRef
   | OpSubgroupAvcMceGetDefaultHighPenaltyCostTableINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceGetDefaultMediumPenaltyCostTableINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceGetDefaultLowPenaltyCostTableINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceSetMotionVectorCostFunctionINTEL of IdResultType * IdResult * PackedCostCenterDelta: IdRef * PackedCostTable: IdRef * CostPrecision: IdRef * Payload: IdRef
   | OpSubgroupAvcMceGetDefaultIntraLumaModePenaltyINTEL of IdResultType * IdResult * SliceType: IdRef * Qp: IdRef
   | OpSubgroupAvcMceGetDefaultNonDcLumaIntraPenaltyINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceGetDefaultIntraChromaModeBasePenaltyINTEL of IdResultType * IdResult
   | OpSubgroupAvcMceSetAcOnlyHaarINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceSetSourceInterlacedFieldPolarityINTEL of IdResultType * IdResult * SourceFieldPolarity: IdRef * Payload: IdRef
   | OpSubgroupAvcMceSetSingleReferenceInterlacedFieldPolarityINTEL of IdResultType * IdResult * ReferenceFieldPolarity: IdRef * Payload: IdRef
   | OpSubgroupAvcMceSetDualReferenceInterlacedFieldPolaritiesINTEL of IdResultType * IdResult * ForwardReferenceFieldPolarity: IdRef * BackwardReferenceFieldPolarity: IdRef * Payload: IdRef
   | OpSubgroupAvcMceConvertToImePayloadINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceConvertToImeResultINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceConvertToRefPayloadINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceConvertToRefResultINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceConvertToSicPayloadINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceConvertToSicResultINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetMotionVectorsINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterDistortionsINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetBestInterDistortionsINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterMajorShapeINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterMinorShapeINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterDirectionsINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterMotionVectorCountINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterReferenceIdsINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcMceGetInterReferenceInterlacedFieldPolaritiesINTEL of IdResultType * IdResult * PackedReferenceIds: IdRef * PackedReferenceParameterFieldPolarities: IdRef * Payload: IdRef
   | OpSubgroupAvcImeInitializeINTEL of IdResultType * IdResult * SrcCoord: IdRef * PartitionMask: IdRef * SADAdjustment: IdRef
   | OpSubgroupAvcImeSetSingleReferenceINTEL of IdResultType * IdResult * RefOffset: IdRef * SearchWindowConfig: IdRef * Payload: IdRef
   | OpSubgroupAvcImeSetDualReferenceINTEL of IdResultType * IdResult * FwdRefOffset: IdRef * BwdRefOffset: IdRef * idSearchWindowConfig: IdRef * Payload: IdRef
   | OpSubgroupAvcImeRefWindowSizeINTEL of IdResultType * IdResult * SearchWindowConfig: IdRef * DualRef: IdRef
   | OpSubgroupAvcImeAdjustRefOffsetINTEL of IdResultType * IdResult * RefOffset: IdRef * SrcCoord: IdRef * RefWindowSize: IdRef * ImageSize: IdRef
   | OpSubgroupAvcImeConvertToMcePayloadINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeSetMaxMotionVectorCountINTEL of IdResultType * IdResult * MaxMotionVectorCount: IdRef * Payload: IdRef
   | OpSubgroupAvcImeSetUnidirectionalMixDisableINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeSetEarlySearchTerminationThresholdINTEL of IdResultType * IdResult * Threshold: IdRef * Payload: IdRef
   | OpSubgroupAvcImeSetWeightedSadINTEL of IdResultType * IdResult * PackedSadWeights: IdRef * Payload: IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * RefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * FwdRefImage: IdRef * BwdRefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminINTEL of IdResultType * IdResult * SrcImage: IdRef * RefImage: IdRef * Payload: IdRef * StreaminComponents: IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminINTEL of IdResultType * IdResult * SrcImage: IdRef * FwdRefImage: IdRef * BwdRefImage: IdRef * Payload: IdRef * StreaminComponents: IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreamoutINTEL of IdResultType * IdResult * SrcImage: IdRef * RefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreamoutINTEL of IdResultType * IdResult * SrcImage: IdRef * FwdRefImage: IdRef * BwdRefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcImeEvaluateWithSingleReferenceStreaminoutINTEL of IdResultType * IdResult * SrcImage: IdRef * RefImage: IdRef * Payload: IdRef * StreaminComponents: IdRef
   | OpSubgroupAvcImeEvaluateWithDualReferenceStreaminoutINTEL of IdResultType * IdResult * SrcImage: IdRef * FwdRefImage: IdRef * BwdRefImage: IdRef * Payload: IdRef * StreaminComponents: IdRef
   | OpSubgroupAvcImeConvertToMceResultINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeGetSingleReferenceStreaminINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeGetDualReferenceStreaminINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeStripSingleReferenceStreamoutINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeStripDualReferenceStreamoutINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeMotionVectorsINTEL of IdResultType * IdResult * Payload: IdRef * MajorShape: IdRef
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeDistortionsINTEL of IdResultType * IdResult * Payload: IdRef * MajorShape: IdRef
   | OpSubgroupAvcImeGetStreamoutSingleReferenceMajorShapeReferenceIdsINTEL of IdResultType * IdResult * Payload: IdRef * MajorShape: IdRef
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeMotionVectorsINTEL of IdResultType * IdResult * Payload: IdRef * MajorShape: IdRef * Direction: IdRef
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeDistortionsINTEL of IdResultType * IdResult * Payload: IdRef * MajorShape: IdRef * Direction: IdRef
   | OpSubgroupAvcImeGetStreamoutDualReferenceMajorShapeReferenceIdsINTEL of IdResultType * IdResult * Payload: IdRef * MajorShape: IdRef * Direction: IdRef
   | OpSubgroupAvcImeGetBorderReachedINTEL of IdResultType * IdResult * ImageSelect: IdRef * Payload: IdRef
   | OpSubgroupAvcImeGetTruncatedSearchIndicationINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeGetUnidirectionalEarlySearchTerminationINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeGetWeightingPatternMinimumMotionVectorINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcImeGetWeightingPatternMinimumDistortionINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcFmeInitializeINTEL of IdResultType * IdResult * SrcCoord: IdRef * MotionVectors: IdRef * MajorShapes: IdRef * MinorShapes: IdRef * Direction: IdRef * PixelResolution: IdRef * SadAdjustment: IdRef
   | OpSubgroupAvcBmeInitializeINTEL of IdResultType * IdResult * SrcCoord: IdRef * MotionVectors: IdRef * MajorShapes: IdRef * MinorShapes: IdRef * Direction: IdRef * PixelResolution: IdRef * BidirectionalWeight: IdRef * SadAdjustment: IdRef
   | OpSubgroupAvcRefConvertToMcePayloadINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcRefSetBidirectionalMixDisableINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcRefSetBilinearFilterEnableINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcRefEvaluateWithSingleReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * RefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcRefEvaluateWithDualReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * FwdRefImage: IdRef * BwdRefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcRefEvaluateWithMultiReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * PackedReferenceIds: IdRef * Payload: IdRef
   | OpSubgroupAvcRefEvaluateWithMultiReferenceInterlacedINTEL of IdResultType * IdResult * SrcImage: IdRef * PackedReferenceIds: IdRef * PackedReferenceFieldPolarities: IdRef * Payload: IdRef
   | OpSubgroupAvcRefConvertToMceResultINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicInitializeINTEL of IdResultType * IdResult * SrcCoord: IdRef
   | OpSubgroupAvcSicConfigureSkcINTEL of IdResultType * IdResult * SkipBlockPartitionType: IdRef * SkipMotionVectorMask: IdRef * MotionVectors: IdRef * BidirectionalWeight: IdRef * SadAdjustment: IdRef * Payload: IdRef
   | OpSubgroupAvcSicConfigureIpeLumaINTEL of IdResultType * IdResult * LumaIntraPartitionMask: IdRef * IntraNeighbourAvailabilty: IdRef * LeftEdgeLumaPixels: IdRef * UpperLeftCornerLumaPixel: IdRef * UpperEdgeLumaPixels: IdRef * UpperRightEdgeLumaPixels: IdRef * SadAdjustment: IdRef * Payload: IdRef
   | OpSubgroupAvcSicConfigureIpeLumaChromaINTEL of IdResultType * IdResult * LumaIntraPartitionMask: IdRef * IntraNeighbourAvailabilty: IdRef * LeftEdgeLumaPixels: IdRef * UpperLeftCornerLumaPixel: IdRef * UpperEdgeLumaPixels: IdRef * UpperRightEdgeLumaPixels: IdRef * LeftEdgeChromaPixels: IdRef * UpperLeftCornerChromaPixel: IdRef * UpperEdgeChromaPixels: IdRef * SadAdjustment: IdRef * Payload: IdRef
   | OpSubgroupAvcSicGetMotionVectorMaskINTEL of IdResultType * IdResult * SkipBlockPartitionType: IdRef * Direction: IdRef
   | OpSubgroupAvcSicConvertToMcePayloadINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicSetIntraLumaShapePenaltyINTEL of IdResultType * IdResult * PackedShapePenalty: IdRef * Payload: IdRef
   | OpSubgroupAvcSicSetIntraLumaModeCostFunctionINTEL of IdResultType * IdResult * LumaModePenalty: IdRef * LumaPackedNeighborModes: IdRef * LumaPackedNonDcPenalty: IdRef * Payload: IdRef
   | OpSubgroupAvcSicSetIntraChromaModeCostFunctionINTEL of IdResultType * IdResult * ChromaModeBasePenalty: IdRef * Payload: IdRef
   | OpSubgroupAvcSicSetBilinearFilterEnableINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicSetSkcForwardTransformEnableINTEL of IdResultType * IdResult * PackedSadCoefficients: IdRef * Payload: IdRef
   | OpSubgroupAvcSicSetBlockBasedRawSkipSadINTEL of IdResultType * IdResult * BlockBasedSkipType: IdRef * Payload: IdRef
   | OpSubgroupAvcSicEvaluateIpeINTEL of IdResultType * IdResult * SrcImage: IdRef * Payload: IdRef
   | OpSubgroupAvcSicEvaluateWithSingleReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * RefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcSicEvaluateWithDualReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * FwdRefImage: IdRef * BwdRefImage: IdRef * Payload: IdRef
   | OpSubgroupAvcSicEvaluateWithMultiReferenceINTEL of IdResultType * IdResult * SrcImage: IdRef * PackedReferenceIds: IdRef * Payload: IdRef
   | OpSubgroupAvcSicEvaluateWithMultiReferenceInterlacedINTEL of IdResultType * IdResult * SrcImage: IdRef * PackedReferenceIds: IdRef * PackedReferenceFieldPolarities: IdRef * Payload: IdRef
   | OpSubgroupAvcSicConvertToMceResultINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetIpeLumaShapeINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetBestIpeLumaDistortionINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetBestIpeChromaDistortionINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetPackedIpeLumaModesINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetIpeChromaModeINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetPackedSkcLumaCountThresholdINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetPackedSkcLumaSumThresholdINTEL of IdResultType * IdResult * Payload: IdRef
   | OpSubgroupAvcSicGetInterRawSadsINTEL of IdResultType * IdResult * Payload: IdRef

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
       | OpDecorateStringGOOGLE _ -> 1.4m
       | OpMemberDecorateString _ -> 1.4m
       | OpMemberDecorateStringGOOGLE _ -> 1.4m
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