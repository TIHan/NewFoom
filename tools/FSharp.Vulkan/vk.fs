// File is generated. Do not modify.
module rec FSharp.Vulkan

open System
open System.Runtime.InteropServices

#nowarn "9" 

type VkImageLayout =
    /// Implicit layout an image is when its contents are undefined due to various reasons (e.g. right after creation)
    | VK_IMAGE_LAYOUT_UNDEFINED = 0
    /// General layout when image can be used for any kind of access
    | VK_IMAGE_LAYOUT_GENERAL = 1
    /// Optimal layout when image is only used for color attachment read/write
    | VK_IMAGE_LAYOUT_COLOR_ATTACHMENT_OPTIMAL = 2
    /// Optimal layout when image is only used for depth/stencil attachment read/write
    | VK_IMAGE_LAYOUT_DEPTH_STENCIL_ATTACHMENT_OPTIMAL = 3
    /// Optimal layout when image is used for read only depth/stencil attachment and shader access
    | VK_IMAGE_LAYOUT_DEPTH_STENCIL_READ_ONLY_OPTIMAL = 4
    /// Optimal layout when image is used for read only shader access
    | VK_IMAGE_LAYOUT_SHADER_READ_ONLY_OPTIMAL = 5
    /// Optimal layout when image is used only as source of transfer operations
    | VK_IMAGE_LAYOUT_TRANSFER_SRC_OPTIMAL = 6
    /// Optimal layout when image is used only as destination of transfer operations
    | VK_IMAGE_LAYOUT_TRANSFER_DST_OPTIMAL = 7
    /// Initial layout used when the data is populated by the CPU
    | VK_IMAGE_LAYOUT_PREINITIALIZED = 8

type VkAttachmentLoadOp =
    | VK_ATTACHMENT_LOAD_OP_LOAD = 0
    | VK_ATTACHMENT_LOAD_OP_CLEAR = 1
    | VK_ATTACHMENT_LOAD_OP_DONT_CARE = 2

type VkAttachmentStoreOp =
    | VK_ATTACHMENT_STORE_OP_STORE = 0
    | VK_ATTACHMENT_STORE_OP_DONT_CARE = 1

type VkImageType =
    | VK_IMAGE_TYPE_1D = 0
    | VK_IMAGE_TYPE_2D = 1
    | VK_IMAGE_TYPE_3D = 2

type VkImageTiling =
    | VK_IMAGE_TILING_OPTIMAL = 0
    | VK_IMAGE_TILING_LINEAR = 1

type VkImageViewType =
    | VK_IMAGE_VIEW_TYPE_1D = 0
    | VK_IMAGE_VIEW_TYPE_2D = 1
    | VK_IMAGE_VIEW_TYPE_3D = 2
    | VK_IMAGE_VIEW_TYPE_CUBE = 3
    | VK_IMAGE_VIEW_TYPE_1D_ARRAY = 4
    | VK_IMAGE_VIEW_TYPE_2D_ARRAY = 5
    | VK_IMAGE_VIEW_TYPE_CUBE_ARRAY = 6

type VkCommandBufferLevel =
    | VK_COMMAND_BUFFER_LEVEL_PRIMARY = 0
    | VK_COMMAND_BUFFER_LEVEL_SECONDARY = 1

type VkComponentSwizzle =
    | VK_COMPONENT_SWIZZLE_IDENTITY = 0
    | VK_COMPONENT_SWIZZLE_ZERO = 1
    | VK_COMPONENT_SWIZZLE_ONE = 2
    | VK_COMPONENT_SWIZZLE_R = 3
    | VK_COMPONENT_SWIZZLE_G = 4
    | VK_COMPONENT_SWIZZLE_B = 5
    | VK_COMPONENT_SWIZZLE_A = 6

type VkDescriptorType =
    | VK_DESCRIPTOR_TYPE_SAMPLER = 0
    | VK_DESCRIPTOR_TYPE_COMBINED_IMAGE_SAMPLER = 1
    | VK_DESCRIPTOR_TYPE_SAMPLED_IMAGE = 2
    | VK_DESCRIPTOR_TYPE_STORAGE_IMAGE = 3
    | VK_DESCRIPTOR_TYPE_UNIFORM_TEXEL_BUFFER = 4
    | VK_DESCRIPTOR_TYPE_STORAGE_TEXEL_BUFFER = 5
    | VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER = 6
    | VK_DESCRIPTOR_TYPE_STORAGE_BUFFER = 7
    | VK_DESCRIPTOR_TYPE_UNIFORM_BUFFER_DYNAMIC = 8
    | VK_DESCRIPTOR_TYPE_STORAGE_BUFFER_DYNAMIC = 9
    | VK_DESCRIPTOR_TYPE_INPUT_ATTACHMENT = 10

type VkQueryType =
    | VK_QUERY_TYPE_OCCLUSION = 0
    /// Optional
    | VK_QUERY_TYPE_PIPELINE_STATISTICS = 1
    | VK_QUERY_TYPE_TIMESTAMP = 2

type VkBorderColor =
    | VK_BORDER_COLOR_FLOAT_TRANSPARENT_BLACK = 0
    | VK_BORDER_COLOR_INT_TRANSPARENT_BLACK = 1
    | VK_BORDER_COLOR_FLOAT_OPAQUE_BLACK = 2
    | VK_BORDER_COLOR_INT_OPAQUE_BLACK = 3
    | VK_BORDER_COLOR_FLOAT_OPAQUE_WHITE = 4
    | VK_BORDER_COLOR_INT_OPAQUE_WHITE = 5

type VkPipelineBindPoint =
    | VK_PIPELINE_BIND_POINT_GRAPHICS = 0
    | VK_PIPELINE_BIND_POINT_COMPUTE = 1

type VkPipelineCacheHeaderVersion =
    | VK_PIPELINE_CACHE_HEADER_VERSION_ONE = 1

type VkPrimitiveTopology =
    | VK_PRIMITIVE_TOPOLOGY_POINT_LIST = 0
    | VK_PRIMITIVE_TOPOLOGY_LINE_LIST = 1
    | VK_PRIMITIVE_TOPOLOGY_LINE_STRIP = 2
    | VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST = 3
    | VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP = 4
    | VK_PRIMITIVE_TOPOLOGY_TRIANGLE_FAN = 5
    | VK_PRIMITIVE_TOPOLOGY_LINE_LIST_WITH_ADJACENCY = 6
    | VK_PRIMITIVE_TOPOLOGY_LINE_STRIP_WITH_ADJACENCY = 7
    | VK_PRIMITIVE_TOPOLOGY_TRIANGLE_LIST_WITH_ADJACENCY = 8
    | VK_PRIMITIVE_TOPOLOGY_TRIANGLE_STRIP_WITH_ADJACENCY = 9
    | VK_PRIMITIVE_TOPOLOGY_PATCH_LIST = 10

type VkSharingMode =
    | VK_SHARING_MODE_EXCLUSIVE = 0
    | VK_SHARING_MODE_CONCURRENT = 1

type VkIndexType =
    | VK_INDEX_TYPE_UINT16 = 0
    | VK_INDEX_TYPE_UINT32 = 1

type VkFilter =
    | VK_FILTER_NEAREST = 0
    | VK_FILTER_LINEAR = 1

type VkSamplerMipmapMode =
    /// Choose nearest mip level
    | VK_SAMPLER_MIPMAP_MODE_NEAREST = 0
    /// Linear filter between mip levels
    | VK_SAMPLER_MIPMAP_MODE_LINEAR = 1

type VkSamplerAddressMode =
    | VK_SAMPLER_ADDRESS_MODE_REPEAT = 0
    | VK_SAMPLER_ADDRESS_MODE_MIRRORED_REPEAT = 1
    | VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_EDGE = 2
    | VK_SAMPLER_ADDRESS_MODE_CLAMP_TO_BORDER = 3

type VkCompareOp =
    | VK_COMPARE_OP_NEVER = 0
    | VK_COMPARE_OP_LESS = 1
    | VK_COMPARE_OP_EQUAL = 2
    | VK_COMPARE_OP_LESS_OR_EQUAL = 3
    | VK_COMPARE_OP_GREATER = 4
    | VK_COMPARE_OP_NOT_EQUAL = 5
    | VK_COMPARE_OP_GREATER_OR_EQUAL = 6
    | VK_COMPARE_OP_ALWAYS = 7

type VkPolygonMode =
    | VK_POLYGON_MODE_FILL = 0
    | VK_POLYGON_MODE_LINE = 1
    | VK_POLYGON_MODE_POINT = 2

[<Flags>]
type VkCullModeFlagBits =
    | VK_CULL_MODE_NONE = 0
    | VK_CULL_MODE_FRONT_BIT = 1
    | VK_CULL_MODE_BACK_BIT = 2

type VkFrontFace =
    | VK_FRONT_FACE_COUNTER_CLOCKWISE = 0
    | VK_FRONT_FACE_CLOCKWISE = 1

type VkBlendFactor =
    | VK_BLEND_FACTOR_ZERO = 0
    | VK_BLEND_FACTOR_ONE = 1
    | VK_BLEND_FACTOR_SRC_COLOR = 2
    | VK_BLEND_FACTOR_ONE_MINUS_SRC_COLOR = 3
    | VK_BLEND_FACTOR_DST_COLOR = 4
    | VK_BLEND_FACTOR_ONE_MINUS_DST_COLOR = 5
    | VK_BLEND_FACTOR_SRC_ALPHA = 6
    | VK_BLEND_FACTOR_ONE_MINUS_SRC_ALPHA = 7
    | VK_BLEND_FACTOR_DST_ALPHA = 8
    | VK_BLEND_FACTOR_ONE_MINUS_DST_ALPHA = 9
    | VK_BLEND_FACTOR_CONSTANT_COLOR = 10
    | VK_BLEND_FACTOR_ONE_MINUS_CONSTANT_COLOR = 11
    | VK_BLEND_FACTOR_CONSTANT_ALPHA = 12
    | VK_BLEND_FACTOR_ONE_MINUS_CONSTANT_ALPHA = 13
    | VK_BLEND_FACTOR_SRC_ALPHA_SATURATE = 14
    | VK_BLEND_FACTOR_SRC1_COLOR = 15
    | VK_BLEND_FACTOR_ONE_MINUS_SRC1_COLOR = 16
    | VK_BLEND_FACTOR_SRC1_ALPHA = 17
    | VK_BLEND_FACTOR_ONE_MINUS_SRC1_ALPHA = 18

type VkBlendOp =
    | VK_BLEND_OP_ADD = 0
    | VK_BLEND_OP_SUBTRACT = 1
    | VK_BLEND_OP_REVERSE_SUBTRACT = 2
    | VK_BLEND_OP_MIN = 3
    | VK_BLEND_OP_MAX = 4

type VkStencilOp =
    | VK_STENCIL_OP_KEEP = 0
    | VK_STENCIL_OP_ZERO = 1
    | VK_STENCIL_OP_REPLACE = 2
    | VK_STENCIL_OP_INCREMENT_AND_CLAMP = 3
    | VK_STENCIL_OP_DECREMENT_AND_CLAMP = 4
    | VK_STENCIL_OP_INVERT = 5
    | VK_STENCIL_OP_INCREMENT_AND_WRAP = 6
    | VK_STENCIL_OP_DECREMENT_AND_WRAP = 7

type VkLogicOp =
    | VK_LOGIC_OP_CLEAR = 0
    | VK_LOGIC_OP_AND = 1
    | VK_LOGIC_OP_AND_REVERSE = 2
    | VK_LOGIC_OP_COPY = 3
    | VK_LOGIC_OP_AND_INVERTED = 4
    | VK_LOGIC_OP_NO_OP = 5
    | VK_LOGIC_OP_XOR = 6
    | VK_LOGIC_OP_OR = 7
    | VK_LOGIC_OP_NOR = 8
    | VK_LOGIC_OP_EQUIVALENT = 9
    | VK_LOGIC_OP_INVERT = 10
    | VK_LOGIC_OP_OR_REVERSE = 11
    | VK_LOGIC_OP_COPY_INVERTED = 12
    | VK_LOGIC_OP_OR_INVERTED = 13
    | VK_LOGIC_OP_NAND = 14
    | VK_LOGIC_OP_SET = 15

type VkInternalAllocationType =
    | VK_INTERNAL_ALLOCATION_TYPE_EXECUTABLE = 0

type VkSystemAllocationScope =
    | VK_SYSTEM_ALLOCATION_SCOPE_COMMAND = 0
    | VK_SYSTEM_ALLOCATION_SCOPE_OBJECT = 1
    | VK_SYSTEM_ALLOCATION_SCOPE_CACHE = 2
    | VK_SYSTEM_ALLOCATION_SCOPE_DEVICE = 3
    | VK_SYSTEM_ALLOCATION_SCOPE_INSTANCE = 4

type VkPhysicalDeviceType =
    | VK_PHYSICAL_DEVICE_TYPE_OTHER = 0
    | VK_PHYSICAL_DEVICE_TYPE_INTEGRATED_GPU = 1
    | VK_PHYSICAL_DEVICE_TYPE_DISCRETE_GPU = 2
    | VK_PHYSICAL_DEVICE_TYPE_VIRTUAL_GPU = 3
    | VK_PHYSICAL_DEVICE_TYPE_CPU = 4

type VkVertexInputRate =
    | VK_VERTEX_INPUT_RATE_VERTEX = 0
    | VK_VERTEX_INPUT_RATE_INSTANCE = 1

/// Vulkan format definitions
type VkFormat =
    | VK_FORMAT_UNDEFINED = 0
    | VK_FORMAT_R4G4_UNORM_PACK8 = 1
    | VK_FORMAT_R4G4B4A4_UNORM_PACK16 = 2
    | VK_FORMAT_B4G4R4A4_UNORM_PACK16 = 3
    | VK_FORMAT_R5G6B5_UNORM_PACK16 = 4
    | VK_FORMAT_B5G6R5_UNORM_PACK16 = 5
    | VK_FORMAT_R5G5B5A1_UNORM_PACK16 = 6
    | VK_FORMAT_B5G5R5A1_UNORM_PACK16 = 7
    | VK_FORMAT_A1R5G5B5_UNORM_PACK16 = 8
    | VK_FORMAT_R8_UNORM = 9
    | VK_FORMAT_R8_SNORM = 10
    | VK_FORMAT_R8_USCALED = 11
    | VK_FORMAT_R8_SSCALED = 12
    | VK_FORMAT_R8_UINT = 13
    | VK_FORMAT_R8_SINT = 14
    | VK_FORMAT_R8_SRGB = 15
    | VK_FORMAT_R8G8_UNORM = 16
    | VK_FORMAT_R8G8_SNORM = 17
    | VK_FORMAT_R8G8_USCALED = 18
    | VK_FORMAT_R8G8_SSCALED = 19
    | VK_FORMAT_R8G8_UINT = 20
    | VK_FORMAT_R8G8_SINT = 21
    | VK_FORMAT_R8G8_SRGB = 22
    | VK_FORMAT_R8G8B8_UNORM = 23
    | VK_FORMAT_R8G8B8_SNORM = 24
    | VK_FORMAT_R8G8B8_USCALED = 25
    | VK_FORMAT_R8G8B8_SSCALED = 26
    | VK_FORMAT_R8G8B8_UINT = 27
    | VK_FORMAT_R8G8B8_SINT = 28
    | VK_FORMAT_R8G8B8_SRGB = 29
    | VK_FORMAT_B8G8R8_UNORM = 30
    | VK_FORMAT_B8G8R8_SNORM = 31
    | VK_FORMAT_B8G8R8_USCALED = 32
    | VK_FORMAT_B8G8R8_SSCALED = 33
    | VK_FORMAT_B8G8R8_UINT = 34
    | VK_FORMAT_B8G8R8_SINT = 35
    | VK_FORMAT_B8G8R8_SRGB = 36
    | VK_FORMAT_R8G8B8A8_UNORM = 37
    | VK_FORMAT_R8G8B8A8_SNORM = 38
    | VK_FORMAT_R8G8B8A8_USCALED = 39
    | VK_FORMAT_R8G8B8A8_SSCALED = 40
    | VK_FORMAT_R8G8B8A8_UINT = 41
    | VK_FORMAT_R8G8B8A8_SINT = 42
    | VK_FORMAT_R8G8B8A8_SRGB = 43
    | VK_FORMAT_B8G8R8A8_UNORM = 44
    | VK_FORMAT_B8G8R8A8_SNORM = 45
    | VK_FORMAT_B8G8R8A8_USCALED = 46
    | VK_FORMAT_B8G8R8A8_SSCALED = 47
    | VK_FORMAT_B8G8R8A8_UINT = 48
    | VK_FORMAT_B8G8R8A8_SINT = 49
    | VK_FORMAT_B8G8R8A8_SRGB = 50
    | VK_FORMAT_A8B8G8R8_UNORM_PACK32 = 51
    | VK_FORMAT_A8B8G8R8_SNORM_PACK32 = 52
    | VK_FORMAT_A8B8G8R8_USCALED_PACK32 = 53
    | VK_FORMAT_A8B8G8R8_SSCALED_PACK32 = 54
    | VK_FORMAT_A8B8G8R8_UINT_PACK32 = 55
    | VK_FORMAT_A8B8G8R8_SINT_PACK32 = 56
    | VK_FORMAT_A8B8G8R8_SRGB_PACK32 = 57
    | VK_FORMAT_A2R10G10B10_UNORM_PACK32 = 58
    | VK_FORMAT_A2R10G10B10_SNORM_PACK32 = 59
    | VK_FORMAT_A2R10G10B10_USCALED_PACK32 = 60
    | VK_FORMAT_A2R10G10B10_SSCALED_PACK32 = 61
    | VK_FORMAT_A2R10G10B10_UINT_PACK32 = 62
    | VK_FORMAT_A2R10G10B10_SINT_PACK32 = 63
    | VK_FORMAT_A2B10G10R10_UNORM_PACK32 = 64
    | VK_FORMAT_A2B10G10R10_SNORM_PACK32 = 65
    | VK_FORMAT_A2B10G10R10_USCALED_PACK32 = 66
    | VK_FORMAT_A2B10G10R10_SSCALED_PACK32 = 67
    | VK_FORMAT_A2B10G10R10_UINT_PACK32 = 68
    | VK_FORMAT_A2B10G10R10_SINT_PACK32 = 69
    | VK_FORMAT_R16_UNORM = 70
    | VK_FORMAT_R16_SNORM = 71
    | VK_FORMAT_R16_USCALED = 72
    | VK_FORMAT_R16_SSCALED = 73
    | VK_FORMAT_R16_UINT = 74
    | VK_FORMAT_R16_SINT = 75
    | VK_FORMAT_R16_SFLOAT = 76
    | VK_FORMAT_R16G16_UNORM = 77
    | VK_FORMAT_R16G16_SNORM = 78
    | VK_FORMAT_R16G16_USCALED = 79
    | VK_FORMAT_R16G16_SSCALED = 80
    | VK_FORMAT_R16G16_UINT = 81
    | VK_FORMAT_R16G16_SINT = 82
    | VK_FORMAT_R16G16_SFLOAT = 83
    | VK_FORMAT_R16G16B16_UNORM = 84
    | VK_FORMAT_R16G16B16_SNORM = 85
    | VK_FORMAT_R16G16B16_USCALED = 86
    | VK_FORMAT_R16G16B16_SSCALED = 87
    | VK_FORMAT_R16G16B16_UINT = 88
    | VK_FORMAT_R16G16B16_SINT = 89
    | VK_FORMAT_R16G16B16_SFLOAT = 90
    | VK_FORMAT_R16G16B16A16_UNORM = 91
    | VK_FORMAT_R16G16B16A16_SNORM = 92
    | VK_FORMAT_R16G16B16A16_USCALED = 93
    | VK_FORMAT_R16G16B16A16_SSCALED = 94
    | VK_FORMAT_R16G16B16A16_UINT = 95
    | VK_FORMAT_R16G16B16A16_SINT = 96
    | VK_FORMAT_R16G16B16A16_SFLOAT = 97
    | VK_FORMAT_R32_UINT = 98
    | VK_FORMAT_R32_SINT = 99
    | VK_FORMAT_R32_SFLOAT = 100
    | VK_FORMAT_R32G32_UINT = 101
    | VK_FORMAT_R32G32_SINT = 102
    | VK_FORMAT_R32G32_SFLOAT = 103
    | VK_FORMAT_R32G32B32_UINT = 104
    | VK_FORMAT_R32G32B32_SINT = 105
    | VK_FORMAT_R32G32B32_SFLOAT = 106
    | VK_FORMAT_R32G32B32A32_UINT = 107
    | VK_FORMAT_R32G32B32A32_SINT = 108
    | VK_FORMAT_R32G32B32A32_SFLOAT = 109
    | VK_FORMAT_R64_UINT = 110
    | VK_FORMAT_R64_SINT = 111
    | VK_FORMAT_R64_SFLOAT = 112
    | VK_FORMAT_R64G64_UINT = 113
    | VK_FORMAT_R64G64_SINT = 114
    | VK_FORMAT_R64G64_SFLOAT = 115
    | VK_FORMAT_R64G64B64_UINT = 116
    | VK_FORMAT_R64G64B64_SINT = 117
    | VK_FORMAT_R64G64B64_SFLOAT = 118
    | VK_FORMAT_R64G64B64A64_UINT = 119
    | VK_FORMAT_R64G64B64A64_SINT = 120
    | VK_FORMAT_R64G64B64A64_SFLOAT = 121
    | VK_FORMAT_B10G11R11_UFLOAT_PACK32 = 122
    | VK_FORMAT_E5B9G9R9_UFLOAT_PACK32 = 123
    | VK_FORMAT_D16_UNORM = 124
    | VK_FORMAT_X8_D24_UNORM_PACK32 = 125
    | VK_FORMAT_D32_SFLOAT = 126
    | VK_FORMAT_S8_UINT = 127
    | VK_FORMAT_D16_UNORM_S8_UINT = 128
    | VK_FORMAT_D24_UNORM_S8_UINT = 129
    | VK_FORMAT_D32_SFLOAT_S8_UINT = 130
    | VK_FORMAT_BC1_RGB_UNORM_BLOCK = 131
    | VK_FORMAT_BC1_RGB_SRGB_BLOCK = 132
    | VK_FORMAT_BC1_RGBA_UNORM_BLOCK = 133
    | VK_FORMAT_BC1_RGBA_SRGB_BLOCK = 134
    | VK_FORMAT_BC2_UNORM_BLOCK = 135
    | VK_FORMAT_BC2_SRGB_BLOCK = 136
    | VK_FORMAT_BC3_UNORM_BLOCK = 137
    | VK_FORMAT_BC3_SRGB_BLOCK = 138
    | VK_FORMAT_BC4_UNORM_BLOCK = 139
    | VK_FORMAT_BC4_SNORM_BLOCK = 140
    | VK_FORMAT_BC5_UNORM_BLOCK = 141
    | VK_FORMAT_BC5_SNORM_BLOCK = 142
    | VK_FORMAT_BC6H_UFLOAT_BLOCK = 143
    | VK_FORMAT_BC6H_SFLOAT_BLOCK = 144
    | VK_FORMAT_BC7_UNORM_BLOCK = 145
    | VK_FORMAT_BC7_SRGB_BLOCK = 146
    | VK_FORMAT_ETC2_R8G8B8_UNORM_BLOCK = 147
    | VK_FORMAT_ETC2_R8G8B8_SRGB_BLOCK = 148
    | VK_FORMAT_ETC2_R8G8B8A1_UNORM_BLOCK = 149
    | VK_FORMAT_ETC2_R8G8B8A1_SRGB_BLOCK = 150
    | VK_FORMAT_ETC2_R8G8B8A8_UNORM_BLOCK = 151
    | VK_FORMAT_ETC2_R8G8B8A8_SRGB_BLOCK = 152
    | VK_FORMAT_EAC_R11_UNORM_BLOCK = 153
    | VK_FORMAT_EAC_R11_SNORM_BLOCK = 154
    | VK_FORMAT_EAC_R11G11_UNORM_BLOCK = 155
    | VK_FORMAT_EAC_R11G11_SNORM_BLOCK = 156
    | VK_FORMAT_ASTC_4x4_UNORM_BLOCK = 157
    | VK_FORMAT_ASTC_4x4_SRGB_BLOCK = 158
    | VK_FORMAT_ASTC_5x4_UNORM_BLOCK = 159
    | VK_FORMAT_ASTC_5x4_SRGB_BLOCK = 160
    | VK_FORMAT_ASTC_5x5_UNORM_BLOCK = 161
    | VK_FORMAT_ASTC_5x5_SRGB_BLOCK = 162
    | VK_FORMAT_ASTC_6x5_UNORM_BLOCK = 163
    | VK_FORMAT_ASTC_6x5_SRGB_BLOCK = 164
    | VK_FORMAT_ASTC_6x6_UNORM_BLOCK = 165
    | VK_FORMAT_ASTC_6x6_SRGB_BLOCK = 166
    | VK_FORMAT_ASTC_8x5_UNORM_BLOCK = 167
    | VK_FORMAT_ASTC_8x5_SRGB_BLOCK = 168
    | VK_FORMAT_ASTC_8x6_UNORM_BLOCK = 169
    | VK_FORMAT_ASTC_8x6_SRGB_BLOCK = 170
    | VK_FORMAT_ASTC_8x8_UNORM_BLOCK = 171
    | VK_FORMAT_ASTC_8x8_SRGB_BLOCK = 172
    | VK_FORMAT_ASTC_10x5_UNORM_BLOCK = 173
    | VK_FORMAT_ASTC_10x5_SRGB_BLOCK = 174
    | VK_FORMAT_ASTC_10x6_UNORM_BLOCK = 175
    | VK_FORMAT_ASTC_10x6_SRGB_BLOCK = 176
    | VK_FORMAT_ASTC_10x8_UNORM_BLOCK = 177
    | VK_FORMAT_ASTC_10x8_SRGB_BLOCK = 178
    | VK_FORMAT_ASTC_10x10_UNORM_BLOCK = 179
    | VK_FORMAT_ASTC_10x10_SRGB_BLOCK = 180
    | VK_FORMAT_ASTC_12x10_UNORM_BLOCK = 181
    | VK_FORMAT_ASTC_12x10_SRGB_BLOCK = 182
    | VK_FORMAT_ASTC_12x12_UNORM_BLOCK = 183
    | VK_FORMAT_ASTC_12x12_SRGB_BLOCK = 184

/// Structure type enumerant
type VkStructureType =
    | VK_STRUCTURE_TYPE_APPLICATION_INFO = 0
    | VK_STRUCTURE_TYPE_INSTANCE_CREATE_INFO = 1
    | VK_STRUCTURE_TYPE_DEVICE_QUEUE_CREATE_INFO = 2
    | VK_STRUCTURE_TYPE_DEVICE_CREATE_INFO = 3
    | VK_STRUCTURE_TYPE_SUBMIT_INFO = 4
    | VK_STRUCTURE_TYPE_MEMORY_ALLOCATE_INFO = 5
    | VK_STRUCTURE_TYPE_MAPPED_MEMORY_RANGE = 6
    | VK_STRUCTURE_TYPE_BIND_SPARSE_INFO = 7
    | VK_STRUCTURE_TYPE_FENCE_CREATE_INFO = 8
    | VK_STRUCTURE_TYPE_SEMAPHORE_CREATE_INFO = 9
    | VK_STRUCTURE_TYPE_EVENT_CREATE_INFO = 10
    | VK_STRUCTURE_TYPE_QUERY_POOL_CREATE_INFO = 11
    | VK_STRUCTURE_TYPE_BUFFER_CREATE_INFO = 12
    | VK_STRUCTURE_TYPE_BUFFER_VIEW_CREATE_INFO = 13
    | VK_STRUCTURE_TYPE_IMAGE_CREATE_INFO = 14
    | VK_STRUCTURE_TYPE_IMAGE_VIEW_CREATE_INFO = 15
    | VK_STRUCTURE_TYPE_SHADER_MODULE_CREATE_INFO = 16
    | VK_STRUCTURE_TYPE_PIPELINE_CACHE_CREATE_INFO = 17
    | VK_STRUCTURE_TYPE_PIPELINE_SHADER_STAGE_CREATE_INFO = 18
    | VK_STRUCTURE_TYPE_PIPELINE_VERTEX_INPUT_STATE_CREATE_INFO = 19
    | VK_STRUCTURE_TYPE_PIPELINE_INPUT_ASSEMBLY_STATE_CREATE_INFO = 20
    | VK_STRUCTURE_TYPE_PIPELINE_TESSELLATION_STATE_CREATE_INFO = 21
    | VK_STRUCTURE_TYPE_PIPELINE_VIEWPORT_STATE_CREATE_INFO = 22
    | VK_STRUCTURE_TYPE_PIPELINE_RASTERIZATION_STATE_CREATE_INFO = 23
    | VK_STRUCTURE_TYPE_PIPELINE_MULTISAMPLE_STATE_CREATE_INFO = 24
    | VK_STRUCTURE_TYPE_PIPELINE_DEPTH_STENCIL_STATE_CREATE_INFO = 25
    | VK_STRUCTURE_TYPE_PIPELINE_COLOR_BLEND_STATE_CREATE_INFO = 26
    | VK_STRUCTURE_TYPE_PIPELINE_DYNAMIC_STATE_CREATE_INFO = 27
    | VK_STRUCTURE_TYPE_GRAPHICS_PIPELINE_CREATE_INFO = 28
    | VK_STRUCTURE_TYPE_COMPUTE_PIPELINE_CREATE_INFO = 29
    | VK_STRUCTURE_TYPE_PIPELINE_LAYOUT_CREATE_INFO = 30
    | VK_STRUCTURE_TYPE_SAMPLER_CREATE_INFO = 31
    | VK_STRUCTURE_TYPE_DESCRIPTOR_SET_LAYOUT_CREATE_INFO = 32
    | VK_STRUCTURE_TYPE_DESCRIPTOR_POOL_CREATE_INFO = 33
    | VK_STRUCTURE_TYPE_DESCRIPTOR_SET_ALLOCATE_INFO = 34
    | VK_STRUCTURE_TYPE_WRITE_DESCRIPTOR_SET = 35
    | VK_STRUCTURE_TYPE_COPY_DESCRIPTOR_SET = 36
    | VK_STRUCTURE_TYPE_FRAMEBUFFER_CREATE_INFO = 37
    | VK_STRUCTURE_TYPE_RENDER_PASS_CREATE_INFO = 38
    | VK_STRUCTURE_TYPE_COMMAND_POOL_CREATE_INFO = 39
    | VK_STRUCTURE_TYPE_COMMAND_BUFFER_ALLOCATE_INFO = 40
    | VK_STRUCTURE_TYPE_COMMAND_BUFFER_INHERITANCE_INFO = 41
    | VK_STRUCTURE_TYPE_COMMAND_BUFFER_BEGIN_INFO = 42
    | VK_STRUCTURE_TYPE_RENDER_PASS_BEGIN_INFO = 43
    | VK_STRUCTURE_TYPE_BUFFER_MEMORY_BARRIER = 44
    | VK_STRUCTURE_TYPE_IMAGE_MEMORY_BARRIER = 45
    | VK_STRUCTURE_TYPE_MEMORY_BARRIER = 46
    /// Reserved for internal use by the loader, layers, and ICDs
    | VK_STRUCTURE_TYPE_LOADER_INSTANCE_CREATE_INFO = 47
    /// Reserved for internal use by the loader, layers, and ICDs
    | VK_STRUCTURE_TYPE_LOADER_DEVICE_CREATE_INFO = 48

type VkSubpassContents =
    | VK_SUBPASS_CONTENTS_INLINE = 0
    | VK_SUBPASS_CONTENTS_SECONDARY_COMMAND_BUFFERS = 1

/// API result codes
type VkResult =
    /// Command completed successfully
    | VK_SUCCESS = 0
    /// A fence or query has not yet completed
    | VK_NOT_READY = 1
    /// A wait operation has not completed in the specified time
    | VK_TIMEOUT = 2
    /// An event is signaled
    | VK_EVENT_SET = 3
    /// An event is unsignaled
    | VK_EVENT_RESET = 4
    /// A return array was too small for the result
    | VK_INCOMPLETE = 5
    /// A host memory allocation has failed
    | VK_ERROR_OUT_OF_HOST_MEMORY = -1
    /// A device memory allocation has failed
    | VK_ERROR_OUT_OF_DEVICE_MEMORY = -2
    /// Initialization of a object has failed
    | VK_ERROR_INITIALIZATION_FAILED = -3
    /// The logical device has been lost. See <<devsandqueues-lost-device>>
    | VK_ERROR_DEVICE_LOST = -4
    /// Mapping of a memory object has failed
    | VK_ERROR_MEMORY_MAP_FAILED = -5
    /// Layer specified does not exist
    | VK_ERROR_LAYER_NOT_PRESENT = -6
    /// Extension specified does not exist
    | VK_ERROR_EXTENSION_NOT_PRESENT = -7
    /// Requested feature is not available on this device
    | VK_ERROR_FEATURE_NOT_PRESENT = -8
    /// Unable to find a Vulkan driver
    | VK_ERROR_INCOMPATIBLE_DRIVER = -9
    /// Too many objects of the type have already been created
    | VK_ERROR_TOO_MANY_OBJECTS = -10
    /// Requested format is not supported on this device
    | VK_ERROR_FORMAT_NOT_SUPPORTED = -11
    /// A requested pool allocation has failed due to fragmentation of the pool's memory
    | VK_ERROR_FRAGMENTED_POOL = -12

type VkDynamicState =
    | VK_DYNAMIC_STATE_VIEWPORT = 0
    | VK_DYNAMIC_STATE_SCISSOR = 1
    | VK_DYNAMIC_STATE_LINE_WIDTH = 2
    | VK_DYNAMIC_STATE_DEPTH_BIAS = 3
    | VK_DYNAMIC_STATE_BLEND_CONSTANTS = 4
    | VK_DYNAMIC_STATE_DEPTH_BOUNDS = 5
    | VK_DYNAMIC_STATE_STENCIL_COMPARE_MASK = 6
    | VK_DYNAMIC_STATE_STENCIL_WRITE_MASK = 7
    | VK_DYNAMIC_STATE_STENCIL_REFERENCE = 8

type VkDescriptorUpdateTemplateType =
    /// Create descriptor update template for descriptor set updates
    | VK_DESCRIPTOR_UPDATE_TEMPLATE_TYPE_DESCRIPTOR_SET = 0

/// Enums to track objects of various types
type VkObjectType =
    | VK_OBJECT_TYPE_UNKNOWN = 0
    /// VkInstance
    | VK_OBJECT_TYPE_INSTANCE = 1
    /// VkPhysicalDevice
    | VK_OBJECT_TYPE_PHYSICAL_DEVICE = 2
    /// VkDevice
    | VK_OBJECT_TYPE_DEVICE = 3
    /// VkQueue
    | VK_OBJECT_TYPE_QUEUE = 4
    /// VkSemaphore
    | VK_OBJECT_TYPE_SEMAPHORE = 5
    /// VkCommandBuffer
    | VK_OBJECT_TYPE_COMMAND_BUFFER = 6
    /// VkFence
    | VK_OBJECT_TYPE_FENCE = 7
    /// VkDeviceMemory
    | VK_OBJECT_TYPE_DEVICE_MEMORY = 8
    /// VkBuffer
    | VK_OBJECT_TYPE_BUFFER = 9
    /// VkImage
    | VK_OBJECT_TYPE_IMAGE = 10
    /// VkEvent
    | VK_OBJECT_TYPE_EVENT = 11
    /// VkQueryPool
    | VK_OBJECT_TYPE_QUERY_POOL = 12
    /// VkBufferView
    | VK_OBJECT_TYPE_BUFFER_VIEW = 13
    /// VkImageView
    | VK_OBJECT_TYPE_IMAGE_VIEW = 14
    /// VkShaderModule
    | VK_OBJECT_TYPE_SHADER_MODULE = 15
    /// VkPipelineCache
    | VK_OBJECT_TYPE_PIPELINE_CACHE = 16
    /// VkPipelineLayout
    | VK_OBJECT_TYPE_PIPELINE_LAYOUT = 17
    /// VkRenderPass
    | VK_OBJECT_TYPE_RENDER_PASS = 18
    /// VkPipeline
    | VK_OBJECT_TYPE_PIPELINE = 19
    /// VkDescriptorSetLayout
    | VK_OBJECT_TYPE_DESCRIPTOR_SET_LAYOUT = 20
    /// VkSampler
    | VK_OBJECT_TYPE_SAMPLER = 21
    /// VkDescriptorPool
    | VK_OBJECT_TYPE_DESCRIPTOR_POOL = 22
    /// VkDescriptorSet
    | VK_OBJECT_TYPE_DESCRIPTOR_SET = 23
    /// VkFramebuffer
    | VK_OBJECT_TYPE_FRAMEBUFFER = 24
    /// VkCommandPool
    | VK_OBJECT_TYPE_COMMAND_POOL = 25

[<Flags>]
type VkQueueFlagBits =
    /// Queue supports graphics operations
    | VK_QUEUE_GRAPHICS_BIT = 1
    /// Queue supports compute operations
    | VK_QUEUE_COMPUTE_BIT = 2
    /// Queue supports transfer operations
    | VK_QUEUE_TRANSFER_BIT = 4
    /// Queue supports sparse resource memory management operations
    | VK_QUEUE_SPARSE_BINDING_BIT = 8

[<Flags>]
type VkMemoryPropertyFlagBits =
    /// If otherwise stated, then allocate memory on device
    | VK_MEMORY_PROPERTY_DEVICE_LOCAL_BIT = 1
    /// Memory is mappable by host
    | VK_MEMORY_PROPERTY_HOST_VISIBLE_BIT = 2
    /// Memory will have i/o coherency. If not set, application may need to use vkFlushMappedMemoryRanges and vkInvalidateMappedMemoryRanges to flush/invalidate host cache
    | VK_MEMORY_PROPERTY_HOST_COHERENT_BIT = 4
    /// Memory will be cached by the host
    | VK_MEMORY_PROPERTY_HOST_CACHED_BIT = 8
    /// Memory may be allocated by the driver when it is required
    | VK_MEMORY_PROPERTY_LAZILY_ALLOCATED_BIT = 16

[<Flags>]
type VkMemoryHeapFlagBits =
    /// If set, heap represents device memory
    | VK_MEMORY_HEAP_DEVICE_LOCAL_BIT = 1

[<Flags>]
type VkAccessFlagBits =
    /// Controls coherency of indirect command reads
    | VK_ACCESS_INDIRECT_COMMAND_READ_BIT = 1
    /// Controls coherency of index reads
    | VK_ACCESS_INDEX_READ_BIT = 2
    /// Controls coherency of vertex attribute reads
    | VK_ACCESS_VERTEX_ATTRIBUTE_READ_BIT = 4
    /// Controls coherency of uniform buffer reads
    | VK_ACCESS_UNIFORM_READ_BIT = 8
    /// Controls coherency of input attachment reads
    | VK_ACCESS_INPUT_ATTACHMENT_READ_BIT = 16
    /// Controls coherency of shader reads
    | VK_ACCESS_SHADER_READ_BIT = 32
    /// Controls coherency of shader writes
    | VK_ACCESS_SHADER_WRITE_BIT = 64
    /// Controls coherency of color attachment reads
    | VK_ACCESS_COLOR_ATTACHMENT_READ_BIT = 128
    /// Controls coherency of color attachment writes
    | VK_ACCESS_COLOR_ATTACHMENT_WRITE_BIT = 256
    /// Controls coherency of depth/stencil attachment reads
    | VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_READ_BIT = 512
    /// Controls coherency of depth/stencil attachment writes
    | VK_ACCESS_DEPTH_STENCIL_ATTACHMENT_WRITE_BIT = 1024
    /// Controls coherency of transfer reads
    | VK_ACCESS_TRANSFER_READ_BIT = 2048
    /// Controls coherency of transfer writes
    | VK_ACCESS_TRANSFER_WRITE_BIT = 4096
    /// Controls coherency of host reads
    | VK_ACCESS_HOST_READ_BIT = 8192
    /// Controls coherency of host writes
    | VK_ACCESS_HOST_WRITE_BIT = 16384
    /// Controls coherency of memory reads
    | VK_ACCESS_MEMORY_READ_BIT = 32768
    /// Controls coherency of memory writes
    | VK_ACCESS_MEMORY_WRITE_BIT = 65536

[<Flags>]
type VkBufferUsageFlagBits =
    /// Can be used as a source of transfer operations
    | VK_BUFFER_USAGE_TRANSFER_SRC_BIT = 1
    /// Can be used as a destination of transfer operations
    | VK_BUFFER_USAGE_TRANSFER_DST_BIT = 2
    /// Can be used as TBO
    | VK_BUFFER_USAGE_UNIFORM_TEXEL_BUFFER_BIT = 4
    /// Can be used as IBO
    | VK_BUFFER_USAGE_STORAGE_TEXEL_BUFFER_BIT = 8
    /// Can be used as UBO
    | VK_BUFFER_USAGE_UNIFORM_BUFFER_BIT = 16
    /// Can be used as SSBO
    | VK_BUFFER_USAGE_STORAGE_BUFFER_BIT = 32
    /// Can be used as source of fixed-function index fetch (index buffer)
    | VK_BUFFER_USAGE_INDEX_BUFFER_BIT = 64
    /// Can be used as source of fixed-function vertex fetch (VBO)
    | VK_BUFFER_USAGE_VERTEX_BUFFER_BIT = 128
    /// Can be the source of indirect parameters (e.g. indirect buffer, parameter buffer)
    | VK_BUFFER_USAGE_INDIRECT_BUFFER_BIT = 256

[<Flags>]
type VkBufferCreateFlagBits =
    /// Buffer should support sparse backing
    | VK_BUFFER_CREATE_SPARSE_BINDING_BIT = 1
    /// Buffer should support sparse backing with partial residency
    | VK_BUFFER_CREATE_SPARSE_RESIDENCY_BIT = 2
    /// Buffer should support constent data access to physical memory ranges mapped into multiple locations of sparse buffers
    | VK_BUFFER_CREATE_SPARSE_ALIASED_BIT = 4

[<Flags>]
type VkShaderStageFlagBits =
    | VK_SHADER_STAGE_VERTEX_BIT = 1
    | VK_SHADER_STAGE_TESSELLATION_CONTROL_BIT = 2
    | VK_SHADER_STAGE_TESSELLATION_EVALUATION_BIT = 4
    | VK_SHADER_STAGE_GEOMETRY_BIT = 8
    | VK_SHADER_STAGE_FRAGMENT_BIT = 16
    | VK_SHADER_STAGE_COMPUTE_BIT = 32

[<Flags>]
type VkImageUsageFlagBits =
    /// Can be used as a source of transfer operations
    | VK_IMAGE_USAGE_TRANSFER_SRC_BIT = 1
    /// Can be used as a destination of transfer operations
    | VK_IMAGE_USAGE_TRANSFER_DST_BIT = 2
    /// Can be sampled from (SAMPLED_IMAGE and COMBINED_IMAGE_SAMPLER descriptor types)
    | VK_IMAGE_USAGE_SAMPLED_BIT = 4
    /// Can be used as storage image (STORAGE_IMAGE descriptor type)
    | VK_IMAGE_USAGE_STORAGE_BIT = 8
    /// Can be used as framebuffer color attachment
    | VK_IMAGE_USAGE_COLOR_ATTACHMENT_BIT = 16
    /// Can be used as framebuffer depth/stencil attachment
    | VK_IMAGE_USAGE_DEPTH_STENCIL_ATTACHMENT_BIT = 32
    /// Image data not needed outside of rendering
    | VK_IMAGE_USAGE_TRANSIENT_ATTACHMENT_BIT = 64
    /// Can be used as framebuffer input attachment
    | VK_IMAGE_USAGE_INPUT_ATTACHMENT_BIT = 128

[<Flags>]
type VkImageCreateFlagBits =
    /// Image should support sparse backing
    | VK_IMAGE_CREATE_SPARSE_BINDING_BIT = 1
    /// Image should support sparse backing with partial residency
    | VK_IMAGE_CREATE_SPARSE_RESIDENCY_BIT = 2
    /// Image should support constent data access to physical memory ranges mapped into multiple locations of sparse images
    | VK_IMAGE_CREATE_SPARSE_ALIASED_BIT = 4
    /// Allows image views to have different format than the base image
    | VK_IMAGE_CREATE_MUTABLE_FORMAT_BIT = 8
    /// Allows creating image views with cube type from the created image
    | VK_IMAGE_CREATE_CUBE_COMPATIBLE_BIT = 16

[<Flags>]
type VkPipelineCreateFlagBits =
    | VK_PIPELINE_CREATE_DISABLE_OPTIMIZATION_BIT = 1
    | VK_PIPELINE_CREATE_ALLOW_DERIVATIVES_BIT = 2
    | VK_PIPELINE_CREATE_DERIVATIVE_BIT = 4

[<Flags>]
type VkColorComponentFlagBits =
    | VK_COLOR_COMPONENT_R_BIT = 1
    | VK_COLOR_COMPONENT_G_BIT = 2
    | VK_COLOR_COMPONENT_B_BIT = 4
    | VK_COLOR_COMPONENT_A_BIT = 8

[<Flags>]
type VkFenceCreateFlagBits =
    | VK_FENCE_CREATE_SIGNALED_BIT = 1

[<Flags>]
type VkFormatFeatureFlagBits =
    /// Format can be used for sampled images (SAMPLED_IMAGE and COMBINED_IMAGE_SAMPLER descriptor types)
    | VK_FORMAT_FEATURE_SAMPLED_IMAGE_BIT = 1
    /// Format can be used for storage images (STORAGE_IMAGE descriptor type)
    | VK_FORMAT_FEATURE_STORAGE_IMAGE_BIT = 2
    /// Format supports atomic operations in case it is used for storage images
    | VK_FORMAT_FEATURE_STORAGE_IMAGE_ATOMIC_BIT = 4
    /// Format can be used for uniform texel buffers (TBOs)
    | VK_FORMAT_FEATURE_UNIFORM_TEXEL_BUFFER_BIT = 8
    /// Format can be used for storage texel buffers (IBOs)
    | VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_BIT = 16
    /// Format supports atomic operations in case it is used for storage texel buffers
    | VK_FORMAT_FEATURE_STORAGE_TEXEL_BUFFER_ATOMIC_BIT = 32
    /// Format can be used for vertex buffers (VBOs)
    | VK_FORMAT_FEATURE_VERTEX_BUFFER_BIT = 64
    /// Format can be used for color attachment images
    | VK_FORMAT_FEATURE_COLOR_ATTACHMENT_BIT = 128
    /// Format supports blending in case it is used for color attachment images
    | VK_FORMAT_FEATURE_COLOR_ATTACHMENT_BLEND_BIT = 256
    /// Format can be used for depth/stencil attachment images
    | VK_FORMAT_FEATURE_DEPTH_STENCIL_ATTACHMENT_BIT = 512
    /// Format can be used as the source image of blits with vkCmdBlitImage
    | VK_FORMAT_FEATURE_BLIT_SRC_BIT = 1024
    /// Format can be used as the destination image of blits with vkCmdBlitImage
    | VK_FORMAT_FEATURE_BLIT_DST_BIT = 2048
    /// Format can be filtered with VK_FILTER_LINEAR when being sampled
    | VK_FORMAT_FEATURE_SAMPLED_IMAGE_FILTER_LINEAR_BIT = 4096

[<Flags>]
type VkQueryControlFlagBits =
    /// Require precise results to be collected by the query
    | VK_QUERY_CONTROL_PRECISE_BIT = 1

[<Flags>]
type VkQueryResultFlagBits =
    /// Results of the queries are written to the destination buffer as 64-bit values
    | VK_QUERY_RESULT_64_BIT = 1
    /// Results of the queries are waited on before proceeding with the result copy
    | VK_QUERY_RESULT_WAIT_BIT = 2
    /// Besides the results of the query, the availability of the results is also written
    | VK_QUERY_RESULT_WITH_AVAILABILITY_BIT = 4
    /// Copy the partial results of the query even if the final results are not available
    | VK_QUERY_RESULT_PARTIAL_BIT = 8

[<Flags>]
type VkCommandBufferUsageFlagBits =
    | VK_COMMAND_BUFFER_USAGE_ONE_TIME_SUBMIT_BIT = 1
    | VK_COMMAND_BUFFER_USAGE_RENDER_PASS_CONTINUE_BIT = 2
    /// Command buffer may be submitted/executed more than once simultaneously
    | VK_COMMAND_BUFFER_USAGE_SIMULTANEOUS_USE_BIT = 4

[<Flags>]
type VkQueryPipelineStatisticFlagBits =
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_INPUT_ASSEMBLY_VERTICES_BIT = 1
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_INPUT_ASSEMBLY_PRIMITIVES_BIT = 2
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_VERTEX_SHADER_INVOCATIONS_BIT = 4
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_GEOMETRY_SHADER_INVOCATIONS_BIT = 8
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_GEOMETRY_SHADER_PRIMITIVES_BIT = 16
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_CLIPPING_INVOCATIONS_BIT = 32
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_CLIPPING_PRIMITIVES_BIT = 64
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_FRAGMENT_SHADER_INVOCATIONS_BIT = 128
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_TESSELLATION_CONTROL_SHADER_PATCHES_BIT = 256
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_TESSELLATION_EVALUATION_SHADER_INVOCATIONS_BIT = 512
    /// Optional
    | VK_QUERY_PIPELINE_STATISTIC_COMPUTE_SHADER_INVOCATIONS_BIT = 1024

[<Flags>]
type VkImageAspectFlagBits =
    | VK_IMAGE_ASPECT_COLOR_BIT = 1
    | VK_IMAGE_ASPECT_DEPTH_BIT = 2
    | VK_IMAGE_ASPECT_STENCIL_BIT = 4
    | VK_IMAGE_ASPECT_METADATA_BIT = 8

[<Flags>]
type VkSparseImageFormatFlagBits =
    /// Image uses a single mip tail region for all array layers
    | VK_SPARSE_IMAGE_FORMAT_SINGLE_MIPTAIL_BIT = 1
    /// Image requires mip level dimensions to be an integer multiple of the sparse image block dimensions for non-tail mip levels.
    | VK_SPARSE_IMAGE_FORMAT_ALIGNED_MIP_SIZE_BIT = 2
    /// Image uses a non-standard sparse image block dimensions
    | VK_SPARSE_IMAGE_FORMAT_NONSTANDARD_BLOCK_SIZE_BIT = 4

[<Flags>]
type VkSparseMemoryBindFlagBits =
    /// Operation binds resource metadata to memory
    | VK_SPARSE_MEMORY_BIND_METADATA_BIT = 1

[<Flags>]
type VkPipelineStageFlagBits =
    /// Before subsequent commands are processed
    | VK_PIPELINE_STAGE_TOP_OF_PIPE_BIT = 1
    /// Draw/DispatchIndirect command fetch
    | VK_PIPELINE_STAGE_DRAW_INDIRECT_BIT = 2
    /// Vertex/index fetch
    | VK_PIPELINE_STAGE_VERTEX_INPUT_BIT = 4
    /// Vertex shading
    | VK_PIPELINE_STAGE_VERTEX_SHADER_BIT = 8
    /// Tessellation control shading
    | VK_PIPELINE_STAGE_TESSELLATION_CONTROL_SHADER_BIT = 16
    /// Tessellation evaluation shading
    | VK_PIPELINE_STAGE_TESSELLATION_EVALUATION_SHADER_BIT = 32
    /// Geometry shading
    | VK_PIPELINE_STAGE_GEOMETRY_SHADER_BIT = 64
    /// Fragment shading
    | VK_PIPELINE_STAGE_FRAGMENT_SHADER_BIT = 128
    /// Early fragment (depth and stencil) tests
    | VK_PIPELINE_STAGE_EARLY_FRAGMENT_TESTS_BIT = 256
    /// Late fragment (depth and stencil) tests
    | VK_PIPELINE_STAGE_LATE_FRAGMENT_TESTS_BIT = 512
    /// Color attachment writes
    | VK_PIPELINE_STAGE_COLOR_ATTACHMENT_OUTPUT_BIT = 1024
    /// Compute shading
    | VK_PIPELINE_STAGE_COMPUTE_SHADER_BIT = 2048
    /// Transfer/copy operations
    | VK_PIPELINE_STAGE_TRANSFER_BIT = 4096
    /// After previous commands have completed
    | VK_PIPELINE_STAGE_BOTTOM_OF_PIPE_BIT = 8192
    /// Indicates host (CPU) is a source/sink of the dependency
    | VK_PIPELINE_STAGE_HOST_BIT = 16384
    /// All stages of the graphics pipeline
    | VK_PIPELINE_STAGE_ALL_GRAPHICS_BIT = 32768
    /// All stages supported on the queue
    | VK_PIPELINE_STAGE_ALL_COMMANDS_BIT = 65536

[<Flags>]
type VkCommandPoolCreateFlagBits =
    /// Command buffers have a short lifetime
    | VK_COMMAND_POOL_CREATE_TRANSIENT_BIT = 1
    /// Command buffers may release their memory individually
    | VK_COMMAND_POOL_CREATE_RESET_COMMAND_BUFFER_BIT = 2

[<Flags>]
type VkCommandPoolResetFlagBits =
    /// Release resources owned by the pool
    | VK_COMMAND_POOL_RESET_RELEASE_RESOURCES_BIT = 1

[<Flags>]
type VkCommandBufferResetFlagBits =
    /// Release resources owned by the buffer
    | VK_COMMAND_BUFFER_RESET_RELEASE_RESOURCES_BIT = 1

[<Flags>]
type VkSampleCountFlagBits =
    /// Sample count 1 supported
    | VK_SAMPLE_COUNT_1_BIT = 1
    /// Sample count 2 supported
    | VK_SAMPLE_COUNT_2_BIT = 2
    /// Sample count 4 supported
    | VK_SAMPLE_COUNT_4_BIT = 4
    /// Sample count 8 supported
    | VK_SAMPLE_COUNT_8_BIT = 8
    /// Sample count 16 supported
    | VK_SAMPLE_COUNT_16_BIT = 16
    /// Sample count 32 supported
    | VK_SAMPLE_COUNT_32_BIT = 32
    /// Sample count 64 supported
    | VK_SAMPLE_COUNT_64_BIT = 64

[<Flags>]
type VkAttachmentDescriptionFlagBits =
    /// The attachment may alias physical memory of another attachment in the same render pass
    | VK_ATTACHMENT_DESCRIPTION_MAY_ALIAS_BIT = 1

[<Flags>]
type VkStencilFaceFlagBits =
    /// Front face
    | VK_STENCIL_FACE_FRONT_BIT = 1
    /// Back face
    | VK_STENCIL_FACE_BACK_BIT = 2

[<Flags>]
type VkDescriptorPoolCreateFlagBits =
    /// Descriptor sets may be freed individually
    | VK_DESCRIPTOR_POOL_CREATE_FREE_DESCRIPTOR_SET_BIT = 1

[<Flags>]
type VkDependencyFlagBits =
    /// Dependency is per pixel region 
    | VK_DEPENDENCY_BY_REGION_BIT = 1

type VkPresentModeKHR =
    | VK_PRESENT_MODE_IMMEDIATE_KHR = 0
    | VK_PRESENT_MODE_MAILBOX_KHR = 1
    | VK_PRESENT_MODE_FIFO_KHR = 2
    | VK_PRESENT_MODE_FIFO_RELAXED_KHR = 3

type VkColorSpaceKHR =
    | VK_COLOR_SPACE_SRGB_NONLINEAR_KHR = 0

[<Flags>]
type VkDisplayPlaneAlphaFlagBitsKHR =
    | VK_DISPLAY_PLANE_ALPHA_OPAQUE_BIT_KHR = 1
    | VK_DISPLAY_PLANE_ALPHA_GLOBAL_BIT_KHR = 2
    | VK_DISPLAY_PLANE_ALPHA_PER_PIXEL_BIT_KHR = 4
    | VK_DISPLAY_PLANE_ALPHA_PER_PIXEL_PREMULTIPLIED_BIT_KHR = 8

[<Flags>]
type VkCompositeAlphaFlagBitsKHR =
    | VK_COMPOSITE_ALPHA_OPAQUE_BIT_KHR = 1
    | VK_COMPOSITE_ALPHA_PRE_MULTIPLIED_BIT_KHR = 2
    | VK_COMPOSITE_ALPHA_POST_MULTIPLIED_BIT_KHR = 4
    | VK_COMPOSITE_ALPHA_INHERIT_BIT_KHR = 8

[<Flags>]
type VkSurfaceTransformFlagBitsKHR =
    | VK_SURFACE_TRANSFORM_IDENTITY_BIT_KHR = 1
    | VK_SURFACE_TRANSFORM_ROTATE_90_BIT_KHR = 2
    | VK_SURFACE_TRANSFORM_ROTATE_180_BIT_KHR = 4
    | VK_SURFACE_TRANSFORM_ROTATE_270_BIT_KHR = 8
    | VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_BIT_KHR = 16
    | VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_90_BIT_KHR = 32
    | VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_180_BIT_KHR = 64
    | VK_SURFACE_TRANSFORM_HORIZONTAL_MIRROR_ROTATE_270_BIT_KHR = 128
    | VK_SURFACE_TRANSFORM_INHERIT_BIT_KHR = 256

[<Flags>]
type VkSwapchainImageUsageFlagBitsANDROID =
    | VK_SWAPCHAIN_IMAGE_USAGE_SHARED_BIT_ANDROID = 1

type VkTimeDomainEXT =
    | VK_TIME_DOMAIN_DEVICE_EXT = 0
    | VK_TIME_DOMAIN_CLOCK_MONOTONIC_EXT = 1
    | VK_TIME_DOMAIN_CLOCK_MONOTONIC_RAW_EXT = 2
    | VK_TIME_DOMAIN_QUERY_PERFORMANCE_COUNTER_EXT = 3

[<Flags>]
type VkDebugReportFlagBitsEXT =
    | VK_DEBUG_REPORT_INFORMATION_BIT_EXT = 1
    | VK_DEBUG_REPORT_WARNING_BIT_EXT = 2
    | VK_DEBUG_REPORT_PERFORMANCE_WARNING_BIT_EXT = 4
    | VK_DEBUG_REPORT_ERROR_BIT_EXT = 8
    | VK_DEBUG_REPORT_DEBUG_BIT_EXT = 16

type VkDebugReportObjectTypeEXT =
    | VK_DEBUG_REPORT_OBJECT_TYPE_UNKNOWN_EXT = 0
    | VK_DEBUG_REPORT_OBJECT_TYPE_INSTANCE_EXT = 1
    | VK_DEBUG_REPORT_OBJECT_TYPE_PHYSICAL_DEVICE_EXT = 2
    | VK_DEBUG_REPORT_OBJECT_TYPE_DEVICE_EXT = 3
    | VK_DEBUG_REPORT_OBJECT_TYPE_QUEUE_EXT = 4
    | VK_DEBUG_REPORT_OBJECT_TYPE_SEMAPHORE_EXT = 5
    | VK_DEBUG_REPORT_OBJECT_TYPE_COMMAND_BUFFER_EXT = 6
    | VK_DEBUG_REPORT_OBJECT_TYPE_FENCE_EXT = 7
    | VK_DEBUG_REPORT_OBJECT_TYPE_DEVICE_MEMORY_EXT = 8
    | VK_DEBUG_REPORT_OBJECT_TYPE_BUFFER_EXT = 9
    | VK_DEBUG_REPORT_OBJECT_TYPE_IMAGE_EXT = 10
    | VK_DEBUG_REPORT_OBJECT_TYPE_EVENT_EXT = 11
    | VK_DEBUG_REPORT_OBJECT_TYPE_QUERY_POOL_EXT = 12
    | VK_DEBUG_REPORT_OBJECT_TYPE_BUFFER_VIEW_EXT = 13
    | VK_DEBUG_REPORT_OBJECT_TYPE_IMAGE_VIEW_EXT = 14
    | VK_DEBUG_REPORT_OBJECT_TYPE_SHADER_MODULE_EXT = 15
    | VK_DEBUG_REPORT_OBJECT_TYPE_PIPELINE_CACHE_EXT = 16
    | VK_DEBUG_REPORT_OBJECT_TYPE_PIPELINE_LAYOUT_EXT = 17
    | VK_DEBUG_REPORT_OBJECT_TYPE_RENDER_PASS_EXT = 18
    | VK_DEBUG_REPORT_OBJECT_TYPE_PIPELINE_EXT = 19
    | VK_DEBUG_REPORT_OBJECT_TYPE_DESCRIPTOR_SET_LAYOUT_EXT = 20
    | VK_DEBUG_REPORT_OBJECT_TYPE_SAMPLER_EXT = 21
    | VK_DEBUG_REPORT_OBJECT_TYPE_DESCRIPTOR_POOL_EXT = 22
    | VK_DEBUG_REPORT_OBJECT_TYPE_DESCRIPTOR_SET_EXT = 23
    | VK_DEBUG_REPORT_OBJECT_TYPE_FRAMEBUFFER_EXT = 24
    | VK_DEBUG_REPORT_OBJECT_TYPE_COMMAND_POOL_EXT = 25
    | VK_DEBUG_REPORT_OBJECT_TYPE_SURFACE_KHR_EXT = 26
    | VK_DEBUG_REPORT_OBJECT_TYPE_SWAPCHAIN_KHR_EXT = 27
    | VK_DEBUG_REPORT_OBJECT_TYPE_DEBUG_REPORT_CALLBACK_EXT_EXT = 28
    | VK_DEBUG_REPORT_OBJECT_TYPE_DISPLAY_KHR_EXT = 29
    | VK_DEBUG_REPORT_OBJECT_TYPE_DISPLAY_MODE_KHR_EXT = 30
    | VK_DEBUG_REPORT_OBJECT_TYPE_OBJECT_TABLE_NVX_EXT = 31
    | VK_DEBUG_REPORT_OBJECT_TYPE_INDIRECT_COMMANDS_LAYOUT_NVX_EXT = 32
    | VK_DEBUG_REPORT_OBJECT_TYPE_VALIDATION_CACHE_EXT_EXT = 33

type VkRasterizationOrderAMD =
    | VK_RASTERIZATION_ORDER_STRICT_AMD = 0
    | VK_RASTERIZATION_ORDER_RELAXED_AMD = 1

[<Flags>]
type VkExternalMemoryHandleTypeFlagBitsNV =
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_WIN32_BIT_NV = 1
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_WIN32_KMT_BIT_NV = 2
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_IMAGE_BIT_NV = 4
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_IMAGE_KMT_BIT_NV = 8

[<Flags>]
type VkExternalMemoryFeatureFlagBitsNV =
    | VK_EXTERNAL_MEMORY_FEATURE_DEDICATED_ONLY_BIT_NV = 1
    | VK_EXTERNAL_MEMORY_FEATURE_EXPORTABLE_BIT_NV = 2
    | VK_EXTERNAL_MEMORY_FEATURE_IMPORTABLE_BIT_NV = 4

type VkValidationCheckEXT =
    | VK_VALIDATION_CHECK_ALL_EXT = 0
    | VK_VALIDATION_CHECK_SHADERS_EXT = 1

type VkValidationFeatureEnableEXT =
    | VK_VALIDATION_FEATURE_ENABLE_GPU_ASSISTED_EXT = 0
    | VK_VALIDATION_FEATURE_ENABLE_GPU_ASSISTED_RESERVE_BINDING_SLOT_EXT = 1
    | VK_VALIDATION_FEATURE_ENABLE_BEST_PRACTICES_EXT = 2

type VkValidationFeatureDisableEXT =
    | VK_VALIDATION_FEATURE_DISABLE_ALL_EXT = 0
    | VK_VALIDATION_FEATURE_DISABLE_SHADERS_EXT = 1
    | VK_VALIDATION_FEATURE_DISABLE_THREAD_SAFETY_EXT = 2
    | VK_VALIDATION_FEATURE_DISABLE_API_PARAMETERS_EXT = 3
    | VK_VALIDATION_FEATURE_DISABLE_OBJECT_LIFETIMES_EXT = 4
    | VK_VALIDATION_FEATURE_DISABLE_CORE_CHECKS_EXT = 5
    | VK_VALIDATION_FEATURE_DISABLE_UNIQUE_HANDLES_EXT = 6

[<Flags>]
type VkSubgroupFeatureFlagBits =
    /// Basic subgroup operations
    | VK_SUBGROUP_FEATURE_BASIC_BIT = 1
    /// Vote subgroup operations
    | VK_SUBGROUP_FEATURE_VOTE_BIT = 2
    /// Arithmetic subgroup operations
    | VK_SUBGROUP_FEATURE_ARITHMETIC_BIT = 4
    /// Ballot subgroup operations
    | VK_SUBGROUP_FEATURE_BALLOT_BIT = 8
    /// Shuffle subgroup operations
    | VK_SUBGROUP_FEATURE_SHUFFLE_BIT = 16
    /// Shuffle relative subgroup operations
    | VK_SUBGROUP_FEATURE_SHUFFLE_RELATIVE_BIT = 32
    /// Clustered subgroup operations
    | VK_SUBGROUP_FEATURE_CLUSTERED_BIT = 64
    /// Quad subgroup operations
    | VK_SUBGROUP_FEATURE_QUAD_BIT = 128

[<Flags>]
type VkIndirectCommandsLayoutUsageFlagBitsNVX =
    | VK_INDIRECT_COMMANDS_LAYOUT_USAGE_UNORDERED_SEQUENCES_BIT_NVX = 1
    | VK_INDIRECT_COMMANDS_LAYOUT_USAGE_SPARSE_SEQUENCES_BIT_NVX = 2
    | VK_INDIRECT_COMMANDS_LAYOUT_USAGE_EMPTY_EXECUTIONS_BIT_NVX = 4
    | VK_INDIRECT_COMMANDS_LAYOUT_USAGE_INDEXED_SEQUENCES_BIT_NVX = 8

[<Flags>]
type VkObjectEntryUsageFlagBitsNVX =
    | VK_OBJECT_ENTRY_USAGE_GRAPHICS_BIT_NVX = 1
    | VK_OBJECT_ENTRY_USAGE_COMPUTE_BIT_NVX = 2

type VkIndirectCommandsTokenTypeNVX =
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_PIPELINE_NVX = 0
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_DESCRIPTOR_SET_NVX = 1
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_INDEX_BUFFER_NVX = 2
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_VERTEX_BUFFER_NVX = 3
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_PUSH_CONSTANT_NVX = 4
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_DRAW_INDEXED_NVX = 5
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_DRAW_NVX = 6
    | VK_INDIRECT_COMMANDS_TOKEN_TYPE_DISPATCH_NVX = 7

type VkObjectEntryTypeNVX =
    | VK_OBJECT_ENTRY_TYPE_DESCRIPTOR_SET_NVX = 0
    | VK_OBJECT_ENTRY_TYPE_PIPELINE_NVX = 1
    | VK_OBJECT_ENTRY_TYPE_INDEX_BUFFER_NVX = 2
    | VK_OBJECT_ENTRY_TYPE_VERTEX_BUFFER_NVX = 3
    | VK_OBJECT_ENTRY_TYPE_PUSH_CONSTANT_NVX = 4

[<Flags>]
type VkExternalMemoryHandleTypeFlagBits =
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_FD_BIT = 1
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_WIN32_BIT = 2
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_OPAQUE_WIN32_KMT_BIT = 4
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_TEXTURE_BIT = 8
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D11_TEXTURE_KMT_BIT = 16
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D12_HEAP_BIT = 32
    | VK_EXTERNAL_MEMORY_HANDLE_TYPE_D3D12_RESOURCE_BIT = 64

[<Flags>]
type VkExternalMemoryFeatureFlagBits =
    | VK_EXTERNAL_MEMORY_FEATURE_DEDICATED_ONLY_BIT = 1
    | VK_EXTERNAL_MEMORY_FEATURE_EXPORTABLE_BIT = 2
    | VK_EXTERNAL_MEMORY_FEATURE_IMPORTABLE_BIT = 4

[<Flags>]
type VkExternalSemaphoreHandleTypeFlagBits =
    | VK_EXTERNAL_SEMAPHORE_HANDLE_TYPE_OPAQUE_FD_BIT = 1
    | VK_EXTERNAL_SEMAPHORE_HANDLE_TYPE_OPAQUE_WIN32_BIT = 2
    | VK_EXTERNAL_SEMAPHORE_HANDLE_TYPE_OPAQUE_WIN32_KMT_BIT = 4
    | VK_EXTERNAL_SEMAPHORE_HANDLE_TYPE_D3D12_FENCE_BIT = 8
    | VK_EXTERNAL_SEMAPHORE_HANDLE_TYPE_SYNC_FD_BIT = 16

[<Flags>]
type VkExternalSemaphoreFeatureFlagBits =
    | VK_EXTERNAL_SEMAPHORE_FEATURE_EXPORTABLE_BIT = 1
    | VK_EXTERNAL_SEMAPHORE_FEATURE_IMPORTABLE_BIT = 2

[<Flags>]
type VkSemaphoreImportFlagBits =
    | VK_SEMAPHORE_IMPORT_TEMPORARY_BIT = 1

[<Flags>]
type VkExternalFenceHandleTypeFlagBits =
    | VK_EXTERNAL_FENCE_HANDLE_TYPE_OPAQUE_FD_BIT = 1
    | VK_EXTERNAL_FENCE_HANDLE_TYPE_OPAQUE_WIN32_BIT = 2
    | VK_EXTERNAL_FENCE_HANDLE_TYPE_OPAQUE_WIN32_KMT_BIT = 4
    | VK_EXTERNAL_FENCE_HANDLE_TYPE_SYNC_FD_BIT = 8

[<Flags>]
type VkExternalFenceFeatureFlagBits =
    | VK_EXTERNAL_FENCE_FEATURE_EXPORTABLE_BIT = 1
    | VK_EXTERNAL_FENCE_FEATURE_IMPORTABLE_BIT = 2

[<Flags>]
type VkFenceImportFlagBits =
    | VK_FENCE_IMPORT_TEMPORARY_BIT = 1

[<Flags>]
type VkSurfaceCounterFlagBitsEXT =
    | VK_SURFACE_COUNTER_VBLANK_EXT = 1

type VkDisplayPowerStateEXT =
    | VK_DISPLAY_POWER_STATE_OFF_EXT = 0
    | VK_DISPLAY_POWER_STATE_SUSPEND_EXT = 1
    | VK_DISPLAY_POWER_STATE_ON_EXT = 2

type VkDeviceEventTypeEXT =
    | VK_DEVICE_EVENT_TYPE_DISPLAY_HOTPLUG_EXT = 0

type VkDisplayEventTypeEXT =
    | VK_DISPLAY_EVENT_TYPE_FIRST_PIXEL_OUT_EXT = 0

[<Flags>]
type VkPeerMemoryFeatureFlagBits =
    /// Can read with vkCmdCopy commands
    | VK_PEER_MEMORY_FEATURE_COPY_SRC_BIT = 1
    /// Can write with vkCmdCopy commands
    | VK_PEER_MEMORY_FEATURE_COPY_DST_BIT = 2
    /// Can read with any access type/command
    | VK_PEER_MEMORY_FEATURE_GENERIC_SRC_BIT = 4
    /// Can write with and access type/command
    | VK_PEER_MEMORY_FEATURE_GENERIC_DST_BIT = 8

[<Flags>]
type VkMemoryAllocateFlagBits =
    /// Force allocation on specific devices
    | VK_MEMORY_ALLOCATE_DEVICE_MASK_BIT = 1

[<Flags>]
type VkDeviceGroupPresentModeFlagBitsKHR =
    /// Present from local memory
    | VK_DEVICE_GROUP_PRESENT_MODE_LOCAL_BIT_KHR = 1
    /// Present from remote memory
    | VK_DEVICE_GROUP_PRESENT_MODE_REMOTE_BIT_KHR = 2
    /// Present sum of local and/or remote memory
    | VK_DEVICE_GROUP_PRESENT_MODE_SUM_BIT_KHR = 4
    /// Each physical device presents from local memory
    | VK_DEVICE_GROUP_PRESENT_MODE_LOCAL_MULTI_DEVICE_BIT_KHR = 8

type VkViewportCoordinateSwizzleNV =
    | VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_X_NV = 0
    | VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_X_NV = 1
    | VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_Y_NV = 2
    | VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_Y_NV = 3
    | VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_Z_NV = 4
    | VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_Z_NV = 5
    | VK_VIEWPORT_COORDINATE_SWIZZLE_POSITIVE_W_NV = 6
    | VK_VIEWPORT_COORDINATE_SWIZZLE_NEGATIVE_W_NV = 7

type VkDiscardRectangleModeEXT =
    | VK_DISCARD_RECTANGLE_MODE_INCLUSIVE_EXT = 0
    | VK_DISCARD_RECTANGLE_MODE_EXCLUSIVE_EXT = 1

type VkPointClippingBehavior =
    | VK_POINT_CLIPPING_BEHAVIOR_ALL_CLIP_PLANES = 0
    | VK_POINT_CLIPPING_BEHAVIOR_USER_CLIP_PLANES_ONLY = 1

type VkSamplerReductionModeEXT =
    | VK_SAMPLER_REDUCTION_MODE_WEIGHTED_AVERAGE_EXT = 0
    | VK_SAMPLER_REDUCTION_MODE_MIN_EXT = 1
    | VK_SAMPLER_REDUCTION_MODE_MAX_EXT = 2

type VkTessellationDomainOrigin =
    | VK_TESSELLATION_DOMAIN_ORIGIN_UPPER_LEFT = 0
    | VK_TESSELLATION_DOMAIN_ORIGIN_LOWER_LEFT = 1

type VkSamplerYcbcrModelConversion =
    | VK_SAMPLER_YCBCR_MODEL_CONVERSION_RGB_IDENTITY = 0
    /// just range expansion
    | VK_SAMPLER_YCBCR_MODEL_CONVERSION_YCBCR_IDENTITY = 1
    /// aka HD YUV
    | VK_SAMPLER_YCBCR_MODEL_CONVERSION_YCBCR_709 = 2
    /// aka SD YUV
    | VK_SAMPLER_YCBCR_MODEL_CONVERSION_YCBCR_601 = 3
    /// aka UHD YUV
    | VK_SAMPLER_YCBCR_MODEL_CONVERSION_YCBCR_2020 = 4

type VkSamplerYcbcrRange =
    /// Luma 0..1 maps to 0..255, chroma -0.5..0.5 to 1..255 (clamped)
    | VK_SAMPLER_YCBCR_RANGE_ITU_FULL = 0
    /// Luma 0..1 maps to 16..235, chroma -0.5..0.5 to 16..240
    | VK_SAMPLER_YCBCR_RANGE_ITU_NARROW = 1

type VkChromaLocation =
    | VK_CHROMA_LOCATION_COSITED_EVEN = 0
    | VK_CHROMA_LOCATION_MIDPOINT = 1

type VkBlendOverlapEXT =
    | VK_BLEND_OVERLAP_UNCORRELATED_EXT = 0
    | VK_BLEND_OVERLAP_DISJOINT_EXT = 1
    | VK_BLEND_OVERLAP_CONJOINT_EXT = 2

type VkCoverageModulationModeNV =
    | VK_COVERAGE_MODULATION_MODE_NONE_NV = 0
    | VK_COVERAGE_MODULATION_MODE_RGB_NV = 1
    | VK_COVERAGE_MODULATION_MODE_ALPHA_NV = 2
    | VK_COVERAGE_MODULATION_MODE_RGBA_NV = 3

type VkCoverageReductionModeNV =
    | VK_COVERAGE_REDUCTION_MODE_MERGE_NV = 0
    | VK_COVERAGE_REDUCTION_MODE_TRUNCATE_NV = 1

type VkValidationCacheHeaderVersionEXT =
    | VK_VALIDATION_CACHE_HEADER_VERSION_ONE_EXT = 1

type VkShaderInfoTypeAMD =
    | VK_SHADER_INFO_TYPE_STATISTICS_AMD = 0
    | VK_SHADER_INFO_TYPE_BINARY_AMD = 1
    | VK_SHADER_INFO_TYPE_DISASSEMBLY_AMD = 2

type VkQueueGlobalPriorityEXT =
    | VK_QUEUE_GLOBAL_PRIORITY_LOW_EXT = 128
    | VK_QUEUE_GLOBAL_PRIORITY_MEDIUM_EXT = 256
    | VK_QUEUE_GLOBAL_PRIORITY_HIGH_EXT = 512
    | VK_QUEUE_GLOBAL_PRIORITY_REALTIME_EXT = 1024

[<Flags>]
type VkDebugUtilsMessageSeverityFlagBitsEXT =
    | VK_DEBUG_UTILS_MESSAGE_SEVERITY_VERBOSE_BIT_EXT = 1
    | VK_DEBUG_UTILS_MESSAGE_SEVERITY_INFO_BIT_EXT = 16
    | VK_DEBUG_UTILS_MESSAGE_SEVERITY_WARNING_BIT_EXT = 256
    | VK_DEBUG_UTILS_MESSAGE_SEVERITY_ERROR_BIT_EXT = 4096

[<Flags>]
type VkDebugUtilsMessageTypeFlagBitsEXT =
    | VK_DEBUG_UTILS_MESSAGE_TYPE_GENERAL_BIT_EXT = 1
    | VK_DEBUG_UTILS_MESSAGE_TYPE_VALIDATION_BIT_EXT = 2
    | VK_DEBUG_UTILS_MESSAGE_TYPE_PERFORMANCE_BIT_EXT = 4

type VkConservativeRasterizationModeEXT =
    | VK_CONSERVATIVE_RASTERIZATION_MODE_DISABLED_EXT = 0
    | VK_CONSERVATIVE_RASTERIZATION_MODE_OVERESTIMATE_EXT = 1
    | VK_CONSERVATIVE_RASTERIZATION_MODE_UNDERESTIMATE_EXT = 2

[<Flags>]
type VkDescriptorBindingFlagBitsEXT =
    | VK_DESCRIPTOR_BINDING_UPDATE_AFTER_BIND_BIT_EXT = 1
    | VK_DESCRIPTOR_BINDING_UPDATE_UNUSED_WHILE_PENDING_BIT_EXT = 2
    | VK_DESCRIPTOR_BINDING_PARTIALLY_BOUND_BIT_EXT = 4
    | VK_DESCRIPTOR_BINDING_VARIABLE_DESCRIPTOR_COUNT_BIT_EXT = 8

type VkDriverIdKHR =
    /// Advanced Micro Devices, Inc.
    | VK_DRIVER_ID_AMD_PROPRIETARY_KHR = 1
    /// Advanced Micro Devices, Inc.
    | VK_DRIVER_ID_AMD_OPEN_SOURCE_KHR = 2
    /// Mesa open source project
    | VK_DRIVER_ID_MESA_RADV_KHR = 3
    /// NVIDIA Corporation
    | VK_DRIVER_ID_NVIDIA_PROPRIETARY_KHR = 4
    /// Intel Corporation
    | VK_DRIVER_ID_INTEL_PROPRIETARY_WINDOWS_KHR = 5
    /// Intel Corporation
    | VK_DRIVER_ID_INTEL_OPEN_SOURCE_MESA_KHR = 6
    /// Imagination Technologies
    | VK_DRIVER_ID_IMAGINATION_PROPRIETARY_KHR = 7
    /// Qualcomm Technologies, Inc.
    | VK_DRIVER_ID_QUALCOMM_PROPRIETARY_KHR = 8
    /// Arm Limited
    | VK_DRIVER_ID_ARM_PROPRIETARY_KHR = 9
    /// Google LLC
    | VK_DRIVER_ID_GOOGLE_SWIFTSHADER_KHR = 10
    /// Google LLC
    | VK_DRIVER_ID_GGP_PROPRIETARY_KHR = 11
    /// Broadcom Inc.
    | VK_DRIVER_ID_BROADCOM_PROPRIETARY_KHR = 12

[<Flags>]
type VkConditionalRenderingFlagBitsEXT =
    | VK_CONDITIONAL_RENDERING_INVERTED_BIT_EXT = 1

[<Flags>]
type VkResolveModeFlagBitsKHR =
    | VK_RESOLVE_MODE_NONE_KHR = 0
    | VK_RESOLVE_MODE_SAMPLE_ZERO_BIT_KHR = 1
    | VK_RESOLVE_MODE_AVERAGE_BIT_KHR = 2
    | VK_RESOLVE_MODE_MIN_BIT_KHR = 4
    | VK_RESOLVE_MODE_MAX_BIT_KHR = 8

type VkShadingRatePaletteEntryNV =
    | VK_SHADING_RATE_PALETTE_ENTRY_NO_INVOCATIONS_NV = 0
    | VK_SHADING_RATE_PALETTE_ENTRY_16_INVOCATIONS_PER_PIXEL_NV = 1
    | VK_SHADING_RATE_PALETTE_ENTRY_8_INVOCATIONS_PER_PIXEL_NV = 2
    | VK_SHADING_RATE_PALETTE_ENTRY_4_INVOCATIONS_PER_PIXEL_NV = 3
    | VK_SHADING_RATE_PALETTE_ENTRY_2_INVOCATIONS_PER_PIXEL_NV = 4
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_PIXEL_NV = 5
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X1_PIXELS_NV = 6
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_1X2_PIXELS_NV = 7
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X2_PIXELS_NV = 8
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_4X2_PIXELS_NV = 9
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_2X4_PIXELS_NV = 10
    | VK_SHADING_RATE_PALETTE_ENTRY_1_INVOCATION_PER_4X4_PIXELS_NV = 11

type VkCoarseSampleOrderTypeNV =
    | VK_COARSE_SAMPLE_ORDER_TYPE_DEFAULT_NV = 0
    | VK_COARSE_SAMPLE_ORDER_TYPE_CUSTOM_NV = 1
    | VK_COARSE_SAMPLE_ORDER_TYPE_PIXEL_MAJOR_NV = 2
    | VK_COARSE_SAMPLE_ORDER_TYPE_SAMPLE_MAJOR_NV = 3

[<Flags>]
type VkGeometryInstanceFlagBitsNV =
    | VK_GEOMETRY_INSTANCE_TRIANGLE_CULL_DISABLE_BIT_NV = 1
    | VK_GEOMETRY_INSTANCE_TRIANGLE_FRONT_COUNTERCLOCKWISE_BIT_NV = 2
    | VK_GEOMETRY_INSTANCE_FORCE_OPAQUE_BIT_NV = 4
    | VK_GEOMETRY_INSTANCE_FORCE_NO_OPAQUE_BIT_NV = 8

[<Flags>]
type VkGeometryFlagBitsNV =
    | VK_GEOMETRY_OPAQUE_BIT_NV = 1
    | VK_GEOMETRY_NO_DUPLICATE_ANY_HIT_INVOCATION_BIT_NV = 2

[<Flags>]
type VkBuildAccelerationStructureFlagBitsNV =
    | VK_BUILD_ACCELERATION_STRUCTURE_ALLOW_UPDATE_BIT_NV = 1
    | VK_BUILD_ACCELERATION_STRUCTURE_ALLOW_COMPACTION_BIT_NV = 2
    | VK_BUILD_ACCELERATION_STRUCTURE_PREFER_FAST_TRACE_BIT_NV = 4
    | VK_BUILD_ACCELERATION_STRUCTURE_PREFER_FAST_BUILD_BIT_NV = 8
    | VK_BUILD_ACCELERATION_STRUCTURE_LOW_MEMORY_BIT_NV = 16

type VkCopyAccelerationStructureModeNV =
    | VK_COPY_ACCELERATION_STRUCTURE_MODE_CLONE_NV = 0
    | VK_COPY_ACCELERATION_STRUCTURE_MODE_COMPACT_NV = 1

type VkAccelerationStructureTypeNV =
    | VK_ACCELERATION_STRUCTURE_TYPE_TOP_LEVEL_NV = 0
    | VK_ACCELERATION_STRUCTURE_TYPE_BOTTOM_LEVEL_NV = 1

type VkGeometryTypeNV =
    | VK_GEOMETRY_TYPE_TRIANGLES_NV = 0
    | VK_GEOMETRY_TYPE_AABBS_NV = 1

type VkAccelerationStructureMemoryRequirementsTypeNV =
    | VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_OBJECT_NV = 0
    | VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_BUILD_SCRATCH_NV = 1
    | VK_ACCELERATION_STRUCTURE_MEMORY_REQUIREMENTS_TYPE_UPDATE_SCRATCH_NV = 2

type VkRayTracingShaderGroupTypeNV =
    | VK_RAY_TRACING_SHADER_GROUP_TYPE_GENERAL_NV = 0
    | VK_RAY_TRACING_SHADER_GROUP_TYPE_TRIANGLES_HIT_GROUP_NV = 1
    | VK_RAY_TRACING_SHADER_GROUP_TYPE_PROCEDURAL_HIT_GROUP_NV = 2

type VkMemoryOverallocationBehaviorAMD =
    | VK_MEMORY_OVERALLOCATION_BEHAVIOR_DEFAULT_AMD = 0
    | VK_MEMORY_OVERALLOCATION_BEHAVIOR_ALLOWED_AMD = 1
    | VK_MEMORY_OVERALLOCATION_BEHAVIOR_DISALLOWED_AMD = 2

type VkScopeNV =
    | VK_SCOPE_DEVICE_NV = 1
    | VK_SCOPE_WORKGROUP_NV = 2
    | VK_SCOPE_SUBGROUP_NV = 3
    | VK_SCOPE_QUEUE_FAMILY_NV = 5

type VkComponentTypeNV =
    | VK_COMPONENT_TYPE_FLOAT16_NV = 0
    | VK_COMPONENT_TYPE_FLOAT32_NV = 1
    | VK_COMPONENT_TYPE_FLOAT64_NV = 2
    | VK_COMPONENT_TYPE_SINT8_NV = 3
    | VK_COMPONENT_TYPE_SINT16_NV = 4
    | VK_COMPONENT_TYPE_SINT32_NV = 5
    | VK_COMPONENT_TYPE_SINT64_NV = 6
    | VK_COMPONENT_TYPE_UINT8_NV = 7
    | VK_COMPONENT_TYPE_UINT16_NV = 8
    | VK_COMPONENT_TYPE_UINT32_NV = 9
    | VK_COMPONENT_TYPE_UINT64_NV = 10

[<Flags>]
type VkPipelineCreationFeedbackFlagBitsEXT =
    | VK_PIPELINE_CREATION_FEEDBACK_VALID_BIT_EXT = 1
    | VK_PIPELINE_CREATION_FEEDBACK_APPLICATION_PIPELINE_CACHE_HIT_BIT_EXT = 2
    | VK_PIPELINE_CREATION_FEEDBACK_BASE_PIPELINE_ACCELERATION_BIT_EXT = 4

type VkFullScreenExclusiveEXT =
    | VK_FULL_SCREEN_EXCLUSIVE_DEFAULT_EXT = 0
    | VK_FULL_SCREEN_EXCLUSIVE_ALLOWED_EXT = 1
    | VK_FULL_SCREEN_EXCLUSIVE_DISALLOWED_EXT = 2
    | VK_FULL_SCREEN_EXCLUSIVE_APPLICATION_CONTROLLED_EXT = 3

type VkPerformanceConfigurationTypeINTEL =
    | VK_PERFORMANCE_CONFIGURATION_TYPE_COMMAND_QUEUE_METRICS_DISCOVERY_ACTIVATED_INTEL = 0

type VkQueryPoolSamplingModeINTEL =
    | VK_QUERY_POOL_SAMPLING_MODE_MANUAL_INTEL = 0

type VkPerformanceOverrideTypeINTEL =
    | VK_PERFORMANCE_OVERRIDE_TYPE_NULL_HARDWARE_INTEL = 0
    | VK_PERFORMANCE_OVERRIDE_TYPE_FLUSH_GPU_CACHES_INTEL = 1

type VkPerformanceParameterTypeINTEL =
    | VK_PERFORMANCE_PARAMETER_TYPE_HW_COUNTERS_SUPPORTED_INTEL = 0
    | VK_PERFORMANCE_PARAMETER_TYPE_STREAM_MARKER_VALID_BITS_INTEL = 1

type VkPerformanceValueTypeINTEL =
    | VK_PERFORMANCE_VALUE_TYPE_UINT32_INTEL = 0
    | VK_PERFORMANCE_VALUE_TYPE_UINT64_INTEL = 1
    | VK_PERFORMANCE_VALUE_TYPE_FLOAT_INTEL = 2
    | VK_PERFORMANCE_VALUE_TYPE_BOOL_INTEL = 3
    | VK_PERFORMANCE_VALUE_TYPE_STRING_INTEL = 4

type VkPipelineExecutableStatisticFormatKHR =
    | VK_PIPELINE_EXECUTABLE_STATISTIC_FORMAT_BOOL32_KHR = 0
    | VK_PIPELINE_EXECUTABLE_STATISTIC_FORMAT_INT64_KHR = 1
    | VK_PIPELINE_EXECUTABLE_STATISTIC_FORMAT_UINT64_KHR = 2
    | VK_PIPELINE_EXECUTABLE_STATISTIC_FORMAT_FLOAT64_KHR = 3

type VkShaderFloatControlsIndependenceKHR =
    | VK_SHADER_FLOAT_CONTROLS_INDEPENDENCE_32_BIT_ONLY_KHR = 0
    | VK_SHADER_FLOAT_CONTROLS_INDEPENDENCE_ALL_KHR = 1
    | VK_SHADER_FLOAT_CONTROLS_INDEPENDENCE_NONE_KHR = 2

type VkLineRasterizationModeEXT =
    | VK_LINE_RASTERIZATION_MODE_DEFAULT_EXT = 0
    | VK_LINE_RASTERIZATION_MODE_RECTANGULAR_EXT = 1
    | VK_LINE_RASTERIZATION_MODE_BRESENHAM_EXT = 2
    | VK_LINE_RASTERIZATION_MODE_RECTANGULAR_SMOOTH_EXT = 3

type Display = nativeint

type VisualID = nativeint

type Window = nativeint

type RROutput = nativeint

type wl_display = nativeint

type wl_surface = nativeint

type HINSTANCE = nativeint

type HWND = nativeint

type HMONITOR = nativeint

type HANDLE = nativeint

type SECURITY_ATTRIBUTES = nativeint

type DWORD = nativeint

type LPCWSTR = nativeint

type xcb_connection_t = nativeint

type xcb_visualid_t = nativeint

type xcb_window_t = nativeint

type zx_handle_t = nativeint

type GgpStreamDescriptor = nativeint

type GgpFrameToken = nativeint

type VK_MAKE_VERSION = nativeint

type VK_VERSION_MAJOR = nativeint

type VK_VERSION_MINOR = nativeint

type VK_VERSION_PATCH = nativeint

type VK_API_VERSION = nativeint

type VK_API_VERSION_1_0 = nativeint

type VK_API_VERSION_1_1 = nativeint

type VK_HEADER_VERSION = nativeint

type VK_DEFINE_HANDLE = nativeint

type VK_NULL_HANDLE = nativeint

type ANativeWindow = nativeint

type AHardwareBuffer = nativeint

type CAMetalLayer = nativeint

type VkSampleMask = uint32

type VkBool32 = uint32

type VkFlags = uint32

type VkDeviceSize = uint64

type VkDeviceAddress = uint64

type VkFramebufferCreateFlags = VkFlags

type VkQueryPoolCreateFlags = VkFlags

type VkRenderPassCreateFlags = VkFlags

type VkSamplerCreateFlags = VkFlags

type VkPipelineLayoutCreateFlags = VkFlags

type VkPipelineCacheCreateFlags = VkFlags

type VkPipelineDepthStencilStateCreateFlags = VkFlags

type VkPipelineDynamicStateCreateFlags = VkFlags

type VkPipelineColorBlendStateCreateFlags = VkFlags

type VkPipelineMultisampleStateCreateFlags = VkFlags

type VkPipelineRasterizationStateCreateFlags = VkFlags

type VkPipelineViewportStateCreateFlags = VkFlags

type VkPipelineTessellationStateCreateFlags = VkFlags

type VkPipelineInputAssemblyStateCreateFlags = VkFlags

type VkPipelineVertexInputStateCreateFlags = VkFlags

type VkPipelineShaderStageCreateFlags = VkFlags

type VkDescriptorSetLayoutCreateFlags = VkFlags

type VkBufferViewCreateFlags = VkFlags

type VkInstanceCreateFlags = VkFlags

type VkDeviceCreateFlags = VkFlags

type VkDeviceQueueCreateFlags = VkFlags

type VkQueueFlags = VkFlags

type VkMemoryPropertyFlags = VkFlags

type VkMemoryHeapFlags = VkFlags

type VkAccessFlags = VkFlags

type VkBufferUsageFlags = VkFlags

type VkBufferCreateFlags = VkFlags

type VkShaderStageFlags = VkFlags

type VkImageUsageFlags = VkFlags

type VkImageCreateFlags = VkFlags

type VkImageViewCreateFlags = VkFlags

type VkPipelineCreateFlags = VkFlags

type VkColorComponentFlags = VkFlags

type VkFenceCreateFlags = VkFlags

type VkSemaphoreCreateFlags = VkFlags

type VkFormatFeatureFlags = VkFlags

type VkQueryControlFlags = VkFlags

type VkQueryResultFlags = VkFlags

type VkShaderModuleCreateFlags = VkFlags

type VkEventCreateFlags = VkFlags

type VkCommandPoolCreateFlags = VkFlags

type VkCommandPoolResetFlags = VkFlags

type VkCommandBufferResetFlags = VkFlags

type VkCommandBufferUsageFlags = VkFlags

type VkQueryPipelineStatisticFlags = VkFlags

type VkMemoryMapFlags = VkFlags

type VkImageAspectFlags = VkFlags

type VkSparseMemoryBindFlags = VkFlags

type VkSparseImageFormatFlags = VkFlags

type VkSubpassDescriptionFlags = VkFlags

type VkPipelineStageFlags = VkFlags

type VkSampleCountFlags = VkFlags

type VkAttachmentDescriptionFlags = VkFlags

type VkStencilFaceFlags = VkFlags

type VkCullModeFlags = VkFlags

type VkDescriptorPoolCreateFlags = VkFlags

type VkDescriptorPoolResetFlags = VkFlags

type VkDependencyFlags = VkFlags

type VkSubgroupFeatureFlags = VkFlags

type VkIndirectCommandsLayoutUsageFlagsNVX = VkFlags

type VkObjectEntryUsageFlagsNVX = VkFlags

type VkGeometryFlagsNV = VkFlags

type VkGeometryInstanceFlagsNV = VkFlags

type VkBuildAccelerationStructureFlagsNV = VkFlags

type VkDescriptorUpdateTemplateCreateFlags = VkFlags

type VkPipelineCreationFeedbackFlagsEXT = VkFlags

type VkPipelineCompilerControlFlagsAMD = VkFlags

type VkShaderCorePropertiesFlagsAMD = VkFlags

type VkCompositeAlphaFlagsKHR = VkFlags

type VkDisplayPlaneAlphaFlagsKHR = VkFlags

type VkSurfaceTransformFlagsKHR = VkFlags

type VkSwapchainCreateFlagsKHR = VkFlags

type VkDisplayModeCreateFlagsKHR = VkFlags

type VkDisplaySurfaceCreateFlagsKHR = VkFlags

type VkAndroidSurfaceCreateFlagsKHR = VkFlags

type VkViSurfaceCreateFlagsNN = VkFlags

type VkWaylandSurfaceCreateFlagsKHR = VkFlags

type VkWin32SurfaceCreateFlagsKHR = VkFlags

type VkXlibSurfaceCreateFlagsKHR = VkFlags

type VkXcbSurfaceCreateFlagsKHR = VkFlags

type VkIOSSurfaceCreateFlagsMVK = VkFlags

type VkMacOSSurfaceCreateFlagsMVK = VkFlags

type VkMetalSurfaceCreateFlagsEXT = VkFlags

type VkImagePipeSurfaceCreateFlagsFUCHSIA = VkFlags

type VkStreamDescriptorSurfaceCreateFlagsGGP = VkFlags

type VkHeadlessSurfaceCreateFlagsEXT = VkFlags

type VkPeerMemoryFeatureFlags = VkFlags

type VkMemoryAllocateFlags = VkFlags

type VkDeviceGroupPresentModeFlagsKHR = VkFlags

type VkDebugReportFlagsEXT = VkFlags

type VkCommandPoolTrimFlags = VkFlags

type VkExternalMemoryHandleTypeFlagsNV = VkFlags

type VkExternalMemoryFeatureFlagsNV = VkFlags

type VkExternalMemoryHandleTypeFlags = VkFlags

type VkExternalMemoryFeatureFlags = VkFlags

type VkExternalSemaphoreHandleTypeFlags = VkFlags

type VkExternalSemaphoreFeatureFlags = VkFlags

type VkSemaphoreImportFlags = VkFlags

type VkExternalFenceHandleTypeFlags = VkFlags

type VkExternalFenceFeatureFlags = VkFlags

type VkFenceImportFlags = VkFlags

type VkSurfaceCounterFlagsEXT = VkFlags

type VkPipelineViewportSwizzleStateCreateFlagsNV = VkFlags

type VkPipelineDiscardRectangleStateCreateFlagsEXT = VkFlags

type VkPipelineCoverageToColorStateCreateFlagsNV = VkFlags

type VkPipelineCoverageModulationStateCreateFlagsNV = VkFlags

type VkPipelineCoverageReductionStateCreateFlagsNV = VkFlags

type VkValidationCacheCreateFlagsEXT = VkFlags

type VkDebugUtilsMessageSeverityFlagsEXT = VkFlags

type VkDebugUtilsMessageTypeFlagsEXT = VkFlags

type VkDebugUtilsMessengerCreateFlagsEXT = VkFlags

type VkDebugUtilsMessengerCallbackDataFlagsEXT = VkFlags

type VkPipelineRasterizationConservativeStateCreateFlagsEXT = VkFlags

type VkDescriptorBindingFlagsEXT = VkFlags

type VkConditionalRenderingFlagsEXT = VkFlags

type VkResolveModeFlagsKHR = VkFlags

type VkPipelineRasterizationStateStreamCreateFlagsEXT = VkFlags

type VkPipelineRasterizationDepthClipStateCreateFlagsEXT = VkFlags

type VkSwapchainImageUsageFlagsANDROID = VkFlags

type VkInstance = nativeint

type VkPhysicalDevice = nativeint

type VkDevice = nativeint

type VkQueue = nativeint

type VkCommandBuffer = nativeint

type VkDeviceMemory = nativeint

type VkCommandPool = nativeint

type VkBuffer = nativeint

type VkBufferView = nativeint

type VkImage = nativeint

type VkImageView = nativeint

type VkShaderModule = nativeint

type VkPipeline = nativeint

type VkPipelineLayout = nativeint

type VkSampler = nativeint

type VkDescriptorSet = nativeint

type VkDescriptorSetLayout = nativeint

type VkDescriptorPool = nativeint

type VkFence = nativeint

type VkSemaphore = nativeint

type VkEvent = nativeint

type VkQueryPool = nativeint

type VkFramebuffer = nativeint

type VkRenderPass = nativeint

type VkPipelineCache = nativeint

type VkObjectTableNVX = nativeint

type VkIndirectCommandsLayoutNVX = nativeint

type VkDescriptorUpdateTemplate = nativeint

type VkSamplerYcbcrConversion = nativeint

type VkValidationCacheEXT = nativeint

type VkAccelerationStructureNV = nativeint

type VkPerformanceConfigurationINTEL = nativeint

type VkDisplayKHR = nativeint

type VkDisplayModeKHR = nativeint

type VkSurfaceKHR = nativeint

type VkSwapchainKHR = nativeint

type VkDebugReportCallbackEXT = nativeint

type VkDebugUtilsMessengerEXT = nativeint

type PFN_vkInternalAllocationNotification = delegate of nativeint * nativeint * VkInternalAllocationType * VkSystemAllocationScope -> unit

type PFN_vkInternalFreeNotification = delegate of nativeint * nativeint * VkInternalAllocationType * VkSystemAllocationScope -> unit

type PFN_vkReallocationFunction = delegate of nativeint * nativeint * nativeint * nativeint * VkSystemAllocationScope -> nativeint

type PFN_vkAllocationFunction = delegate of nativeint * nativeint * nativeint * VkSystemAllocationScope -> nativeint

type PFN_vkFreeFunction = delegate of nativeint * nativeint -> unit

type PFN_vkVoidFunction = delegate of unit -> unit

type PFN_vkDebugReportCallbackEXT = delegate of VkDebugReportFlagsEXT * VkDebugReportObjectTypeEXT * uint64 * nativeint * int * nativeptr<char> * nativeptr<char> * nativeint -> VkBool32

type PFN_vkDebugUtilsMessengerCallbackEXT = delegate of VkDebugUtilsMessageSeverityFlagBitsEXT * VkDebugUtilsMessageTypeFlagsEXT * nativeptr<VkDebugUtilsMessengerCallbackDataEXT> * nativeint -> VkBool32

[<Struct>]
type VkBaseOutStructure =
    val mutable sType: VkStructureType
    val mutable pNext: nativeptr<VkBaseOutStructure>

[<Struct>]
type VkBaseInStructure =
    val mutable sType: VkStructureType
    val pNext: nativeptr<VkBaseInStructure>

[<Struct>]
type VkOffset2D =
    val mutable x: int
    val mutable y: int

[<Struct>]
type VkOffset3D =
    val mutable x: int
    val mutable y: int
    val mutable z: int

[<Struct>]
type VkExtent2D =
    val mutable width: uint32
    val mutable height: uint32

[<Struct>]
type VkExtent3D =
    val mutable width: uint32
    val mutable height: uint32
    val mutable depth: uint32

[<Struct>]
type VkViewport =
    val mutable x: float32
    val mutable y: float32
    val mutable width: float32
    val mutable height: float32
    val mutable minDepth: float32
    val mutable maxDepth: float32

[<Struct>]
type VkRect2D =
    val mutable offset: VkOffset2D
    val mutable extent: VkExtent2D

[<Struct>]
type VkClearRect =
    val mutable rect: VkRect2D
    val mutable baseArrayLayer: uint32
    val mutable layerCount: uint32

[<Struct>]
type VkComponentMapping =
    val mutable r: VkComponentSwizzle
    val mutable g: VkComponentSwizzle
    val mutable b: VkComponentSwizzle
    val mutable a: VkComponentSwizzle

[<Struct>]
type VkPhysicalDeviceProperties =
    val mutable apiVersion: uint32
    val mutable driverVersion: uint32
    val mutable vendorID: uint32
    val mutable deviceID: uint32
    val mutable deviceType: VkPhysicalDeviceType
    val mutable deviceName: char
    val mutable pipelineCacheUUID: byte
    val mutable limits: VkPhysicalDeviceLimits
    val mutable sparseProperties: VkPhysicalDeviceSparseProperties

[<Struct>]
type VkExtensionProperties =
    /// extension name
    val mutable extensionName: char
    /// version of the extension specification implemented
    val mutable specVersion: uint32

[<Struct>]
type VkLayerProperties =
    /// layer name
    val mutable layerName: char
    /// version of the layer specification implemented
    val mutable specVersion: uint32
    /// build or release version of the layer's library
    val mutable implementationVersion: uint32
    /// Free-form description of the layer
    val mutable description: char

[<Struct>]
type VkApplicationInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val pApplicationName: nativeptr<char>
    val mutable applicationVersion: uint32
    val pEngineName: nativeptr<char>
    val mutable engineVersion: uint32
    val mutable apiVersion: uint32

[<Struct>]
type VkAllocationCallbacks =
    val mutable pUserData: nativeint
    val mutable pfnAllocation: PFN_vkAllocationFunction
    val mutable pfnReallocation: PFN_vkReallocationFunction
    val mutable pfnFree: PFN_vkFreeFunction
    val mutable pfnInternalAllocation: PFN_vkInternalAllocationNotification
    val mutable pfnInternalFree: PFN_vkInternalFreeNotification

[<Struct>]
type VkDeviceQueueCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDeviceQueueCreateFlags
    val mutable queueFamilyIndex: uint32
    val mutable queueCount: uint32
    val pQueuePriorities: nativeptr<float32>

[<Struct>]
type VkDeviceCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDeviceCreateFlags
    val mutable queueCreateInfoCount: uint32
    val pQueueCreateInfos: nativeptr<VkDeviceQueueCreateInfo>
    val mutable enabledLayerCount: uint32
    /// Ordered list of layer names to be enabled
    val ppEnabledLayerNames: nativeptr<char>
    val mutable enabledExtensionCount: uint32
    val ppEnabledExtensionNames: nativeptr<char>
    val pEnabledFeatures: nativeptr<VkPhysicalDeviceFeatures>

[<Struct>]
type VkInstanceCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkInstanceCreateFlags
    val pApplicationInfo: nativeptr<VkApplicationInfo>
    val mutable enabledLayerCount: uint32
    /// Ordered list of layer names to be enabled
    val ppEnabledLayerNames: nativeptr<char>
    val mutable enabledExtensionCount: uint32
    /// Extension names to be enabled
    val ppEnabledExtensionNames: nativeptr<char>

[<Struct>]
type VkQueueFamilyProperties =
    /// Queue flags
    val mutable queueFlags: VkQueueFlags
    val mutable queueCount: uint32
    val mutable timestampValidBits: uint32
    /// Minimum alignment requirement for image transfers
    val mutable minImageTransferGranularity: VkExtent3D

[<Struct>]
type VkPhysicalDeviceMemoryProperties =
    val mutable memoryTypeCount: uint32
    val mutable memoryTypes: VkMemoryType
    val mutable memoryHeapCount: uint32
    val mutable memoryHeaps: VkMemoryHeap

[<Struct>]
type VkMemoryAllocateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Size of memory allocation
    val mutable allocationSize: VkDeviceSize
    /// Index of the of the memory type to allocate from
    val mutable memoryTypeIndex: uint32

[<Struct>]
type VkMemoryRequirements =
    /// Specified in bytes
    val mutable size: VkDeviceSize
    /// Specified in bytes
    val mutable alignment: VkDeviceSize
    /// Bitmask of the allowed memory type indices into memoryTypes[] for this object
    val mutable memoryTypeBits: uint32

[<Struct>]
type VkSparseImageFormatProperties =
    val mutable aspectMask: VkImageAspectFlags
    val mutable imageGranularity: VkExtent3D
    val mutable flags: VkSparseImageFormatFlags

[<Struct>]
type VkSparseImageMemoryRequirements =
    val mutable formatProperties: VkSparseImageFormatProperties
    val mutable imageMipTailFirstLod: uint32
    /// Specified in bytes, must be a multiple of sparse block size in bytes / alignment
    val mutable imageMipTailSize: VkDeviceSize
    /// Specified in bytes, must be a multiple of sparse block size in bytes / alignment
    val mutable imageMipTailOffset: VkDeviceSize
    /// Specified in bytes, must be a multiple of sparse block size in bytes / alignment
    val mutable imageMipTailStride: VkDeviceSize

[<Struct>]
type VkMemoryType =
    /// Memory properties of this memory type
    val mutable propertyFlags: VkMemoryPropertyFlags
    /// Index of the memory heap allocations of this memory type are taken from
    val mutable heapIndex: uint32

[<Struct>]
type VkMemoryHeap =
    /// Available memory in the heap
    val mutable size: VkDeviceSize
    /// Flags for the heap
    val mutable flags: VkMemoryHeapFlags

[<Struct>]
type VkMappedMemoryRange =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Mapped memory object
    val mutable memory: VkDeviceMemory
    /// Offset within the memory object where the range starts
    val mutable offset: VkDeviceSize
    /// Size of the range within the memory object
    val mutable size: VkDeviceSize

[<Struct>]
type VkFormatProperties =
    /// Format features in case of linear tiling
    val mutable linearTilingFeatures: VkFormatFeatureFlags
    /// Format features in case of optimal tiling
    val mutable optimalTilingFeatures: VkFormatFeatureFlags
    /// Format features supported by buffers
    val mutable bufferFeatures: VkFormatFeatureFlags

[<Struct>]
type VkImageFormatProperties =
    /// max image dimensions for this resource type
    val mutable maxExtent: VkExtent3D
    /// max number of mipmap levels for this resource type
    val mutable maxMipLevels: uint32
    /// max array size for this resource type
    val mutable maxArrayLayers: uint32
    /// supported sample counts for this resource type
    val mutable sampleCounts: VkSampleCountFlags
    /// max size (in bytes) of this resource type
    val mutable maxResourceSize: VkDeviceSize

[<Struct>]
type VkDescriptorBufferInfo =
    /// Buffer used for this descriptor slot.
    val mutable buffer: VkBuffer
    /// Base offset from buffer start in bytes to update in the descriptor set.
    val mutable offset: VkDeviceSize
    /// Size in bytes of the buffer resource for this descriptor update.
    val mutable range: VkDeviceSize

[<Struct>]
type VkDescriptorImageInfo =
    /// Sampler to write to the descriptor in case it is a SAMPLER or COMBINED_IMAGE_SAMPLER descriptor. Ignored otherwise.
    val mutable sampler: VkSampler
    /// Image view to write to the descriptor in case it is a SAMPLED_IMAGE, STORAGE_IMAGE, COMBINED_IMAGE_SAMPLER, or INPUT_ATTACHMENT descriptor. Ignored otherwise.
    val mutable imageView: VkImageView
    /// Layout the image is expected to be in when accessed using this descriptor (only used if imageView is not VK_NULL_HANDLE).
    val mutable imageLayout: VkImageLayout

[<Struct>]
type VkWriteDescriptorSet =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Destination descriptor set
    val mutable dstSet: VkDescriptorSet
    /// Binding within the destination descriptor set to write
    val mutable dstBinding: uint32
    /// Array element within the destination binding to write
    val mutable dstArrayElement: uint32
    /// Number of descriptors to write (determines the size of the array pointed by pDescriptors)
    val mutable descriptorCount: uint32
    /// Descriptor type to write (determines which members of the array pointed by pDescriptors are going to be used)
    val mutable descriptorType: VkDescriptorType
    /// Sampler, image view, and layout for SAMPLER, COMBINED_IMAGE_SAMPLER, {SAMPLED,STORAGE}_IMAGE, and INPUT_ATTACHMENT descriptor types.
    val pImageInfo: nativeptr<VkDescriptorImageInfo>
    /// Raw buffer, size, and offset for {UNIFORM,STORAGE}_BUFFER[_DYNAMIC] descriptor types.
    val pBufferInfo: nativeptr<VkDescriptorBufferInfo>
    /// Buffer view to write to the descriptor for {UNIFORM,STORAGE}_TEXEL_BUFFER descriptor types.
    val pTexelBufferView: nativeptr<VkBufferView>

[<Struct>]
type VkCopyDescriptorSet =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Source descriptor set
    val mutable srcSet: VkDescriptorSet
    /// Binding within the source descriptor set to copy from
    val mutable srcBinding: uint32
    /// Array element within the source binding to copy from
    val mutable srcArrayElement: uint32
    /// Destination descriptor set
    val mutable dstSet: VkDescriptorSet
    /// Binding within the destination descriptor set to copy to
    val mutable dstBinding: uint32
    /// Array element within the destination binding to copy to
    val mutable dstArrayElement: uint32
    /// Number of descriptors to write (determines the size of the array pointed by pDescriptors)
    val mutable descriptorCount: uint32

[<Struct>]
type VkBufferCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Buffer creation flags
    val mutable flags: VkBufferCreateFlags
    /// Specified in bytes
    val mutable size: VkDeviceSize
    /// Buffer usage flags
    val mutable usage: VkBufferUsageFlags
    val mutable sharingMode: VkSharingMode
    val mutable queueFamilyIndexCount: uint32
    val pQueueFamilyIndices: nativeptr<uint32>

[<Struct>]
type VkBufferViewCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkBufferViewCreateFlags
    val mutable buffer: VkBuffer
    /// Optionally specifies format of elements
    val mutable format: VkFormat
    /// Specified in bytes
    val mutable offset: VkDeviceSize
    /// View size specified in bytes
    val mutable range: VkDeviceSize

[<Struct>]
type VkImageSubresource =
    val mutable aspectMask: VkImageAspectFlags
    val mutable mipLevel: uint32
    val mutable arrayLayer: uint32

[<Struct>]
type VkImageSubresourceLayers =
    val mutable aspectMask: VkImageAspectFlags
    val mutable mipLevel: uint32
    val mutable baseArrayLayer: uint32
    val mutable layerCount: uint32

[<Struct>]
type VkImageSubresourceRange =
    val mutable aspectMask: VkImageAspectFlags
    val mutable baseMipLevel: uint32
    val mutable levelCount: uint32
    val mutable baseArrayLayer: uint32
    val mutable layerCount: uint32

[<Struct>]
type VkMemoryBarrier =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Memory accesses from the source of the dependency to synchronize
    val mutable srcAccessMask: VkAccessFlags
    /// Memory accesses from the destination of the dependency to synchronize
    val mutable dstAccessMask: VkAccessFlags

[<Struct>]
type VkBufferMemoryBarrier =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Memory accesses from the source of the dependency to synchronize
    val mutable srcAccessMask: VkAccessFlags
    /// Memory accesses from the destination of the dependency to synchronize
    val mutable dstAccessMask: VkAccessFlags
    /// Queue family to transition ownership from
    val mutable srcQueueFamilyIndex: uint32
    /// Queue family to transition ownership to
    val mutable dstQueueFamilyIndex: uint32
    /// Buffer to sync
    val mutable buffer: VkBuffer
    /// Offset within the buffer to sync
    val mutable offset: VkDeviceSize
    /// Amount of bytes to sync
    val mutable size: VkDeviceSize

[<Struct>]
type VkImageMemoryBarrier =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Memory accesses from the source of the dependency to synchronize
    val mutable srcAccessMask: VkAccessFlags
    /// Memory accesses from the destination of the dependency to synchronize
    val mutable dstAccessMask: VkAccessFlags
    /// Current layout of the image
    val mutable oldLayout: VkImageLayout
    /// New layout to transition the image to
    val mutable newLayout: VkImageLayout
    /// Queue family to transition ownership from
    val mutable srcQueueFamilyIndex: uint32
    /// Queue family to transition ownership to
    val mutable dstQueueFamilyIndex: uint32
    /// Image to sync
    val mutable image: VkImage
    /// Subresource range to sync
    val mutable subresourceRange: VkImageSubresourceRange

[<Struct>]
type VkImageCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Image creation flags
    val mutable flags: VkImageCreateFlags
    val mutable imageType: VkImageType
    val mutable format: VkFormat
    val mutable extent: VkExtent3D
    val mutable mipLevels: uint32
    val mutable arrayLayers: uint32
    val mutable samples: VkSampleCountFlagBits
    val mutable tiling: VkImageTiling
    /// Image usage flags
    val mutable usage: VkImageUsageFlags
    /// Cross-queue-family sharing mode
    val mutable sharingMode: VkSharingMode
    /// Number of queue families to share across
    val mutable queueFamilyIndexCount: uint32
    /// Array of queue family indices to share across
    val pQueueFamilyIndices: nativeptr<uint32>
    /// Initial image layout for all subresources
    val mutable initialLayout: VkImageLayout

[<Struct>]
type VkSubresourceLayout =
    /// Specified in bytes
    val mutable offset: VkDeviceSize
    /// Specified in bytes
    val mutable size: VkDeviceSize
    /// Specified in bytes
    val mutable rowPitch: VkDeviceSize
    /// Specified in bytes
    val mutable arrayPitch: VkDeviceSize
    /// Specified in bytes
    val mutable depthPitch: VkDeviceSize

[<Struct>]
type VkImageViewCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkImageViewCreateFlags
    val mutable image: VkImage
    val mutable viewType: VkImageViewType
    val mutable format: VkFormat
    val mutable components: VkComponentMapping
    val mutable subresourceRange: VkImageSubresourceRange

[<Struct>]
type VkBufferCopy =
    /// Specified in bytes
    val mutable srcOffset: VkDeviceSize
    /// Specified in bytes
    val mutable dstOffset: VkDeviceSize
    /// Specified in bytes
    val mutable size: VkDeviceSize

[<Struct>]
type VkSparseMemoryBind =
    /// Specified in bytes
    val mutable resourceOffset: VkDeviceSize
    /// Specified in bytes
    val mutable size: VkDeviceSize
    val mutable memory: VkDeviceMemory
    /// Specified in bytes
    val mutable memoryOffset: VkDeviceSize
    val mutable flags: VkSparseMemoryBindFlags

[<Struct>]
type VkSparseImageMemoryBind =
    val mutable subresource: VkImageSubresource
    val mutable offset: VkOffset3D
    val mutable extent: VkExtent3D
    val mutable memory: VkDeviceMemory
    /// Specified in bytes
    val mutable memoryOffset: VkDeviceSize
    val mutable flags: VkSparseMemoryBindFlags

[<Struct>]
type VkSparseBufferMemoryBindInfo =
    val mutable buffer: VkBuffer
    val mutable bindCount: uint32
    val pBinds: nativeptr<VkSparseMemoryBind>

[<Struct>]
type VkSparseImageOpaqueMemoryBindInfo =
    val mutable image: VkImage
    val mutable bindCount: uint32
    val pBinds: nativeptr<VkSparseMemoryBind>

[<Struct>]
type VkSparseImageMemoryBindInfo =
    val mutable image: VkImage
    val mutable bindCount: uint32
    val pBinds: nativeptr<VkSparseImageMemoryBind>

[<Struct>]
type VkBindSparseInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable waitSemaphoreCount: uint32
    val pWaitSemaphores: nativeptr<VkSemaphore>
    val mutable bufferBindCount: uint32
    val pBufferBinds: nativeptr<VkSparseBufferMemoryBindInfo>
    val mutable imageOpaqueBindCount: uint32
    val pImageOpaqueBinds: nativeptr<VkSparseImageOpaqueMemoryBindInfo>
    val mutable imageBindCount: uint32
    val pImageBinds: nativeptr<VkSparseImageMemoryBindInfo>
    val mutable signalSemaphoreCount: uint32
    val pSignalSemaphores: nativeptr<VkSemaphore>

[<Struct>]
type VkImageCopy =
    val mutable srcSubresource: VkImageSubresourceLayers
    /// Specified in pixels for both compressed and uncompressed images
    val mutable srcOffset: VkOffset3D
    val mutable dstSubresource: VkImageSubresourceLayers
    /// Specified in pixels for both compressed and uncompressed images
    val mutable dstOffset: VkOffset3D
    /// Specified in pixels for both compressed and uncompressed images
    val mutable extent: VkExtent3D

[<Struct>]
type VkImageBlit =
    val mutable srcSubresource: VkImageSubresourceLayers
    /// Specified in pixels for both compressed and uncompressed images
    val mutable srcOffsets: VkOffset3D
    val mutable dstSubresource: VkImageSubresourceLayers
    /// Specified in pixels for both compressed and uncompressed images
    val mutable dstOffsets: VkOffset3D

[<Struct>]
type VkBufferImageCopy =
    /// Specified in bytes
    val mutable bufferOffset: VkDeviceSize
    /// Specified in texels
    val mutable bufferRowLength: uint32
    val mutable bufferImageHeight: uint32
    val mutable imageSubresource: VkImageSubresourceLayers
    /// Specified in pixels for both compressed and uncompressed images
    val mutable imageOffset: VkOffset3D
    /// Specified in pixels for both compressed and uncompressed images
    val mutable imageExtent: VkExtent3D

[<Struct>]
type VkImageResolve =
    val mutable srcSubresource: VkImageSubresourceLayers
    val mutable srcOffset: VkOffset3D
    val mutable dstSubresource: VkImageSubresourceLayers
    val mutable dstOffset: VkOffset3D
    val mutable extent: VkExtent3D

[<Struct>]
type VkShaderModuleCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkShaderModuleCreateFlags
    /// Specified in bytes
    val mutable codeSize: nativeint
    /// Binary code of size codeSize
    val pCode: nativeptr<uint32>

[<Struct>]
type VkDescriptorSetLayoutBinding =
    /// Binding number for this entry
    val mutable binding: uint32
    /// Type of the descriptors in this binding
    val mutable descriptorType: VkDescriptorType
    /// Number of descriptors in this binding
    val mutable descriptorCount: uint32
    /// Shader stages this binding is visible to
    val mutable stageFlags: VkShaderStageFlags
    /// Immutable samplers (used if descriptor type is SAMPLER or COMBINED_IMAGE_SAMPLER, is either NULL or contains count number of elements)
    val pImmutableSamplers: nativeptr<VkSampler>

[<Struct>]
type VkDescriptorSetLayoutCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDescriptorSetLayoutCreateFlags
    /// Number of bindings in the descriptor set layout
    val mutable bindingCount: uint32
    /// Array of descriptor set layout bindings
    val pBindings: nativeptr<VkDescriptorSetLayoutBinding>

[<Struct>]
type VkDescriptorPoolSize =
    val mutable typ: VkDescriptorType
    val mutable descriptorCount: uint32

[<Struct>]
type VkDescriptorPoolCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDescriptorPoolCreateFlags
    val mutable maxSets: uint32
    val mutable poolSizeCount: uint32
    val pPoolSizes: nativeptr<VkDescriptorPoolSize>

[<Struct>]
type VkDescriptorSetAllocateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable descriptorPool: VkDescriptorPool
    val mutable descriptorSetCount: uint32
    val pSetLayouts: nativeptr<VkDescriptorSetLayout>

[<Struct>]
type VkSpecializationMapEntry =
    /// The SpecConstant ID specified in the BIL
    val constantID: uint32
    /// Offset of the value in the data block
    val mutable offset: uint32
    /// Size in bytes of the SpecConstant
    val mutable size: nativeint

[<Struct>]
type VkSpecializationInfo =
    /// Number of entries in the map
    val mutable mapEntryCount: uint32
    /// Array of map entries
    val pMapEntries: nativeptr<VkSpecializationMapEntry>
    /// Size in bytes of pData
    val mutable dataSize: nativeint
    /// Pointer to SpecConstant data
    val pData: nativeint

[<Struct>]
type VkPipelineShaderStageCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineShaderStageCreateFlags
    /// Shader stage
    val mutable stage: VkShaderStageFlagBits
    /// Module containing entry point
    val mutable modul: VkShaderModule
    /// Null-terminated entry point name
    val pName: nativeptr<char>
    val pSpecializationInfo: nativeptr<VkSpecializationInfo>

[<Struct>]
type VkComputePipelineCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Pipeline creation flags
    val mutable flags: VkPipelineCreateFlags
    val mutable stage: VkPipelineShaderStageCreateInfo
    /// Interface layout of the pipeline
    val mutable layout: VkPipelineLayout
    /// If VK_PIPELINE_CREATE_DERIVATIVE_BIT is set and this value is nonzero, it specifies the handle of the base pipeline this is a derivative of
    val mutable basePipelineHandle: VkPipeline
    /// If VK_PIPELINE_CREATE_DERIVATIVE_BIT is set and this value is not -1, it specifies an index into pCreateInfos of the base pipeline this is a derivative of
    val mutable basePipelineIndex: int

[<Struct>]
type VkVertexInputBindingDescription =
    /// Vertex buffer binding id
    val mutable binding: uint32
    /// Distance between vertices in bytes (0 = no advancement)
    val mutable stride: uint32
    /// The rate at which the vertex data is consumed
    val mutable inputRate: VkVertexInputRate

[<Struct>]
type VkVertexInputAttributeDescription =
    /// location of the shader vertex attrib
    val mutable location: uint32
    /// Vertex buffer binding id
    val mutable binding: uint32
    /// format of source data
    val mutable format: VkFormat
    /// Offset of first element in bytes from base of vertex
    val mutable offset: uint32

[<Struct>]
type VkPipelineVertexInputStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineVertexInputStateCreateFlags
    /// number of bindings
    val mutable vertexBindingDescriptionCount: uint32
    val pVertexBindingDescriptions: nativeptr<VkVertexInputBindingDescription>
    /// number of attributes
    val mutable vertexAttributeDescriptionCount: uint32
    val pVertexAttributeDescriptions: nativeptr<VkVertexInputAttributeDescription>

[<Struct>]
type VkPipelineInputAssemblyStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineInputAssemblyStateCreateFlags
    val mutable topology: VkPrimitiveTopology
    val mutable primitiveRestartEnable: VkBool32

[<Struct>]
type VkPipelineTessellationStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineTessellationStateCreateFlags
    val mutable patchControlPoints: uint32

[<Struct>]
type VkPipelineViewportStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineViewportStateCreateFlags
    val mutable viewportCount: uint32
    val pViewports: nativeptr<VkViewport>
    val mutable scissorCount: uint32
    val pScissors: nativeptr<VkRect2D>

[<Struct>]
type VkPipelineRasterizationStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineRasterizationStateCreateFlags
    val mutable depthClampEnable: VkBool32
    val mutable rasterizerDiscardEnable: VkBool32
    /// optional (GL45)
    val mutable polygonMode: VkPolygonMode
    val mutable cullMode: VkCullModeFlags
    val mutable frontFace: VkFrontFace
    val mutable depthBiasEnable: VkBool32
    val mutable depthBiasConstantFactor: float32
    val mutable depthBiasClamp: float32
    val mutable depthBiasSlopeFactor: float32
    val mutable lineWidth: float32

[<Struct>]
type VkPipelineMultisampleStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineMultisampleStateCreateFlags
    /// Number of samples used for rasterization
    val mutable rasterizationSamples: VkSampleCountFlagBits
    /// optional (GL45)
    val mutable sampleShadingEnable: VkBool32
    /// optional (GL45)
    val mutable minSampleShading: float32
    /// Array of sampleMask words
    val pSampleMask: nativeptr<VkSampleMask>
    val mutable alphaToCoverageEnable: VkBool32
    val mutable alphaToOneEnable: VkBool32

[<Struct>]
type VkPipelineColorBlendAttachmentState =
    val mutable blendEnable: VkBool32
    val mutable srcColorBlendFactor: VkBlendFactor
    val mutable dstColorBlendFactor: VkBlendFactor
    val mutable colorBlendOp: VkBlendOp
    val mutable srcAlphaBlendFactor: VkBlendFactor
    val mutable dstAlphaBlendFactor: VkBlendFactor
    val mutable alphaBlendOp: VkBlendOp
    val mutable colorWriteMask: VkColorComponentFlags

[<Struct>]
type VkPipelineColorBlendStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineColorBlendStateCreateFlags
    val mutable logicOpEnable: VkBool32
    val mutable logicOp: VkLogicOp
    /// # of pAttachments
    val mutable attachmentCount: uint32
    val pAttachments: nativeptr<VkPipelineColorBlendAttachmentState>
    val mutable blendConstants: float32

[<Struct>]
type VkPipelineDynamicStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineDynamicStateCreateFlags
    val mutable dynamicStateCount: uint32
    val pDynamicStates: nativeptr<VkDynamicState>

[<Struct>]
type VkStencilOpState =
    val mutable failOp: VkStencilOp
    val mutable passOp: VkStencilOp
    val mutable depthFailOp: VkStencilOp
    val mutable compareOp: VkCompareOp
    val mutable compareMask: uint32
    val mutable writeMask: uint32
    val mutable reference: uint32

[<Struct>]
type VkPipelineDepthStencilStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineDepthStencilStateCreateFlags
    val mutable depthTestEnable: VkBool32
    val mutable depthWriteEnable: VkBool32
    val mutable depthCompareOp: VkCompareOp
    /// optional (depth_bounds_test)
    val mutable depthBoundsTestEnable: VkBool32
    val mutable stencilTestEnable: VkBool32
    val mutable front: VkStencilOpState
    val mutable back: VkStencilOpState
    val mutable minDepthBounds: float32
    val mutable maxDepthBounds: float32

[<Struct>]
type VkGraphicsPipelineCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Pipeline creation flags
    val mutable flags: VkPipelineCreateFlags
    val mutable stageCount: uint32
    /// One entry for each active shader stage
    val pStages: nativeptr<VkPipelineShaderStageCreateInfo>
    val pVertexInputState: nativeptr<VkPipelineVertexInputStateCreateInfo>
    val pInputAssemblyState: nativeptr<VkPipelineInputAssemblyStateCreateInfo>
    val pTessellationState: nativeptr<VkPipelineTessellationStateCreateInfo>
    val pViewportState: nativeptr<VkPipelineViewportStateCreateInfo>
    val pRasterizationState: nativeptr<VkPipelineRasterizationStateCreateInfo>
    val pMultisampleState: nativeptr<VkPipelineMultisampleStateCreateInfo>
    val pDepthStencilState: nativeptr<VkPipelineDepthStencilStateCreateInfo>
    val pColorBlendState: nativeptr<VkPipelineColorBlendStateCreateInfo>
    val pDynamicState: nativeptr<VkPipelineDynamicStateCreateInfo>
    /// Interface layout of the pipeline
    val mutable layout: VkPipelineLayout
    val mutable renderPass: VkRenderPass
    val mutable subpass: uint32
    /// If VK_PIPELINE_CREATE_DERIVATIVE_BIT is set and this value is nonzero, it specifies the handle of the base pipeline this is a derivative of
    val mutable basePipelineHandle: VkPipeline
    /// If VK_PIPELINE_CREATE_DERIVATIVE_BIT is set and this value is not -1, it specifies an index into pCreateInfos of the base pipeline this is a derivative of
    val mutable basePipelineIndex: int

[<Struct>]
type VkPipelineCacheCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineCacheCreateFlags
    /// Size of initial data to populate cache, in bytes
    val mutable initialDataSize: nativeint
    /// Initial data to populate cache
    val pInitialData: nativeint

[<Struct>]
type VkPushConstantRange =
    /// Which stages use the range
    val mutable stageFlags: VkShaderStageFlags
    /// Start of the range, in bytes
    val mutable offset: uint32
    /// Size of the range, in bytes
    val mutable size: uint32

[<Struct>]
type VkPipelineLayoutCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineLayoutCreateFlags
    /// Number of descriptor sets interfaced by the pipeline
    val mutable setLayoutCount: uint32
    /// Array of setCount number of descriptor set layout objects defining the layout of the
    val pSetLayouts: nativeptr<VkDescriptorSetLayout>
    /// Number of push-constant ranges used by the pipeline
    val pushConstantRangeCount: uint32
    /// Array of pushConstantRangeCount number of ranges used by various shader stages
    val pPushConstantRanges: nativeptr<VkPushConstantRange>

[<Struct>]
type VkSamplerCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkSamplerCreateFlags
    /// Filter mode for magnification
    val mutable magFilter: VkFilter
    /// Filter mode for minifiation
    val mutable minFilter: VkFilter
    /// Mipmap selection mode
    val mutable mipmapMode: VkSamplerMipmapMode
    val mutable addressModeU: VkSamplerAddressMode
    val mutable addressModeV: VkSamplerAddressMode
    val mutable addressModeW: VkSamplerAddressMode
    val mutable mipLodBias: float32
    val mutable anisotropyEnable: VkBool32
    val mutable maxAnisotropy: float32
    val mutable compareEnable: VkBool32
    val mutable compareOp: VkCompareOp
    val mutable minLod: float32
    val mutable maxLod: float32
    val mutable borderColor: VkBorderColor
    val mutable unnormalizedCoordinates: VkBool32

[<Struct>]
type VkCommandPoolCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Command pool creation flags
    val mutable flags: VkCommandPoolCreateFlags
    val mutable queueFamilyIndex: uint32

[<Struct>]
type VkCommandBufferAllocateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable commandPool: VkCommandPool
    val mutable level: VkCommandBufferLevel
    val mutable commandBufferCount: uint32

[<Struct>]
type VkCommandBufferInheritanceInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Render pass for secondary command buffers
    val mutable renderPass: VkRenderPass
    val mutable subpass: uint32
    /// Framebuffer for secondary command buffers
    val mutable framebuffer: VkFramebuffer
    /// Whether this secondary command buffer may be executed during an occlusion query
    val mutable occlusionQueryEnable: VkBool32
    /// Query flags used by this secondary command buffer, if executed during an occlusion query
    val mutable queryFlags: VkQueryControlFlags
    /// Pipeline statistics that may be counted for this secondary command buffer
    val mutable pipelineStatistics: VkQueryPipelineStatisticFlags

[<Struct>]
type VkCommandBufferBeginInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Command buffer usage flags
    val mutable flags: VkCommandBufferUsageFlags
    /// Pointer to inheritance info for secondary command buffers
    val pInheritanceInfo: nativeptr<VkCommandBufferInheritanceInfo>

[<Struct>]
type VkRenderPassBeginInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable renderPass: VkRenderPass
    val mutable framebuffer: VkFramebuffer
    val mutable renderArea: VkRect2D
    val mutable clearValueCount: uint32
    val pClearValues: nativeptr<VkClearValue>

/// // Union allowing specification of floating point, integer, or unsigned integer color data. Actual value selected is based on image/attachment being cleared.
[<Struct;StructLayout(LayoutKind.Explicit)>]
type VkClearColorValue =
    [<FieldOffset(0)>] val mutable float32: float32
    [<FieldOffset(0)>] val mutable int32: int
    [<FieldOffset(0)>] val mutable uint32: uint32

[<Struct>]
type VkClearDepthStencilValue =
    val mutable depth: float32
    val mutable stencil: uint32

/// // Union allowing specification of color or depth and stencil values. Actual value selected is based on attachment being cleared.
[<Struct;StructLayout(LayoutKind.Explicit)>]
type VkClearValue =
    [<FieldOffset(0)>] val mutable color: VkClearColorValue
    [<FieldOffset(0)>] val mutable depthStencil: VkClearDepthStencilValue

[<Struct>]
type VkClearAttachment =
    val mutable aspectMask: VkImageAspectFlags
    val mutable colorAttachment: uint32
    val mutable clearValue: VkClearValue

[<Struct>]
type VkAttachmentDescription =
    val mutable flags: VkAttachmentDescriptionFlags
    val mutable format: VkFormat
    val mutable samples: VkSampleCountFlagBits
    /// Load operation for color or depth data
    val mutable loadOp: VkAttachmentLoadOp
    /// Store operation for color or depth data
    val mutable storeOp: VkAttachmentStoreOp
    /// Load operation for stencil data
    val mutable stencilLoadOp: VkAttachmentLoadOp
    /// Store operation for stencil data
    val mutable stencilStoreOp: VkAttachmentStoreOp
    val mutable initialLayout: VkImageLayout
    val mutable finalLayout: VkImageLayout

[<Struct>]
type VkAttachmentReference =
    val mutable attachment: uint32
    val mutable layout: VkImageLayout

[<Struct>]
type VkSubpassDescription =
    val mutable flags: VkSubpassDescriptionFlags
    /// Must be VK_PIPELINE_BIND_POINT_GRAPHICS for now
    val mutable pipelineBindPoint: VkPipelineBindPoint
    val mutable inputAttachmentCount: uint32
    val pInputAttachments: nativeptr<VkAttachmentReference>
    val mutable colorAttachmentCount: uint32
    val pColorAttachments: nativeptr<VkAttachmentReference>
    val pResolveAttachments: nativeptr<VkAttachmentReference>
    val pDepthStencilAttachment: nativeptr<VkAttachmentReference>
    val mutable preserveAttachmentCount: uint32
    val pPreserveAttachments: nativeptr<uint32>

[<Struct>]
type VkSubpassDependency =
    val mutable srcSubpass: uint32
    val mutable dstSubpass: uint32
    val mutable srcStageMask: VkPipelineStageFlags
    val mutable dstStageMask: VkPipelineStageFlags
    /// Memory accesses from the source of the dependency to synchronize
    val mutable srcAccessMask: VkAccessFlags
    /// Memory accesses from the destination of the dependency to synchronize
    val mutable dstAccessMask: VkAccessFlags
    val mutable dependencyFlags: VkDependencyFlags

[<Struct>]
type VkRenderPassCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkRenderPassCreateFlags
    val mutable attachmentCount: uint32
    val pAttachments: nativeptr<VkAttachmentDescription>
    val mutable subpassCount: uint32
    val pSubpasses: nativeptr<VkSubpassDescription>
    val mutable dependencyCount: uint32
    val pDependencies: nativeptr<VkSubpassDependency>

[<Struct>]
type VkEventCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Event creation flags
    val mutable flags: VkEventCreateFlags

[<Struct>]
type VkFenceCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Fence creation flags
    val mutable flags: VkFenceCreateFlags

[<Struct>]
type VkPhysicalDeviceFeatures =
    /// out of bounds buffer accesses are well defined
    val mutable robustBufferAccess: VkBool32
    /// full 32-bit range of indices for indexed draw calls
    val mutable fullDrawIndexUint32: VkBool32
    /// image views which are arrays of cube maps
    val mutable imageCubeArray: VkBool32
    /// blending operations are controlled per-attachment
    val mutable independentBlend: VkBool32
    /// geometry stage
    val mutable geometryShader: VkBool32
    /// tessellation control and evaluation stage
    val mutable tessellationShader: VkBool32
    /// per-sample shading and interpolation
    val mutable sampleRateShading: VkBool32
    /// blend operations which take two sources
    val mutable dualSrcBlend: VkBool32
    /// logic operations
    val mutable logicOp: VkBool32
    /// multi draw indirect
    val mutable multiDrawIndirect: VkBool32
    /// indirect draws can use non-zero firstInstance
    val mutable drawIndirectFirstInstance: VkBool32
    /// depth clamping
    val mutable depthClamp: VkBool32
    /// depth bias clamping
    val mutable depthBiasClamp: VkBool32
    /// point and wireframe fill modes
    val mutable fillModeNonSolid: VkBool32
    /// depth bounds test
    val mutable depthBounds: VkBool32
    /// lines with width greater than 1
    val mutable wideLines: VkBool32
    /// points with size greater than 1
    val mutable largePoints: VkBool32
    /// the fragment alpha component can be forced to maximum representable alpha value
    val mutable alphaToOne: VkBool32
    /// viewport arrays
    val mutable multiViewport: VkBool32
    /// anisotropic sampler filtering
    val mutable samplerAnisotropy: VkBool32
    /// ETC texture compression formats
    val mutable textureCompressionETC2: VkBool32
    /// ASTC LDR texture compression formats
    val mutable textureCompressionASTC_LDR: VkBool32
    /// BC1-7 texture compressed formats
    val mutable textureCompressionBC: VkBool32
    /// precise occlusion queries returning actual sample counts
    val mutable occlusionQueryPrecise: VkBool32
    /// pipeline statistics query
    val mutable pipelineStatisticsQuery: VkBool32
    /// stores and atomic ops on storage buffers and images are supported in vertex, tessellation, and geometry stages
    val mutable vertexPipelineStoresAndAtomics: VkBool32
    /// stores and atomic ops on storage buffers and images are supported in the fragment stage
    val mutable fragmentStoresAndAtomics: VkBool32
    /// tessellation and geometry stages can export point size
    val mutable shaderTessellationAndGeometryPointSize: VkBool32
    /// image gather with run-time values and independent offsets
    val mutable shaderImageGatherExtended: VkBool32
    /// the extended set of formats can be used for storage images
    val mutable shaderStorageImageExtendedFormats: VkBool32
    /// multisample images can be used for storage images
    val mutable shaderStorageImageMultisample: VkBool32
    /// read from storage image does not require format qualifier
    val mutable shaderStorageImageReadWithoutFormat: VkBool32
    /// write to storage image does not require format qualifier
    val mutable shaderStorageImageWriteWithoutFormat: VkBool32
    /// arrays of uniform buffers can be accessed with dynamically uniform indices
    val mutable shaderUniformBufferArrayDynamicIndexing: VkBool32
    /// arrays of sampled images can be accessed with dynamically uniform indices
    val mutable shaderSampledImageArrayDynamicIndexing: VkBool32
    /// arrays of storage buffers can be accessed with dynamically uniform indices
    val mutable shaderStorageBufferArrayDynamicIndexing: VkBool32
    /// arrays of storage images can be accessed with dynamically uniform indices
    val mutable shaderStorageImageArrayDynamicIndexing: VkBool32
    /// clip distance in shaders
    val mutable shaderClipDistance: VkBool32
    /// cull distance in shaders
    val mutable shaderCullDistance: VkBool32
    /// 64-bit floats (doubles) in shaders
    val mutable shaderFloat64: VkBool32
    /// 64-bit integers in shaders
    val mutable shaderInt64: VkBool32
    /// 16-bit integers in shaders
    val mutable shaderInt16: VkBool32
    /// shader can use texture operations that return resource residency information (requires sparseNonResident support)
    val mutable shaderResourceResidency: VkBool32
    /// shader can use texture operations that specify minimum resource LOD
    val mutable shaderResourceMinLod: VkBool32
    /// Sparse resources support: Resource memory can be managed at opaque page level rather than object level
    val mutable sparseBinding: VkBool32
    /// Sparse resources support: GPU can access partially resident buffers 
    val mutable sparseResidencyBuffer: VkBool32
    /// Sparse resources support: GPU can access partially resident 2D (non-MSAA non-depth/stencil) images 
    val mutable sparseResidencyImage2D: VkBool32
    /// Sparse resources support: GPU can access partially resident 3D images 
    val mutable sparseResidencyImage3D: VkBool32
    /// Sparse resources support: GPU can access partially resident MSAA 2D images with 2 samples
    val mutable sparseResidency2Samples: VkBool32
    /// Sparse resources support: GPU can access partially resident MSAA 2D images with 4 samples
    val mutable sparseResidency4Samples: VkBool32
    /// Sparse resources support: GPU can access partially resident MSAA 2D images with 8 samples
    val mutable sparseResidency8Samples: VkBool32
    /// Sparse resources support: GPU can access partially resident MSAA 2D images with 16 samples
    val mutable sparseResidency16Samples: VkBool32
    /// Sparse resources support: GPU can correctly access data aliased into multiple locations (opt-in)
    val mutable sparseResidencyAliased: VkBool32
    /// multisample rate must be the same for all pipelines in a subpass
    val mutable variableMultisampleRate: VkBool32
    /// Queries may be inherited from primary to secondary command buffers
    val mutable inheritedQueries: VkBool32

[<Struct>]
type VkPhysicalDeviceSparseProperties =
    /// Sparse resources support: GPU will access all 2D (single sample) sparse resources using the standard sparse image block shapes (based on pixel format)
    val mutable residencyStandard2DBlockShape: VkBool32
    /// Sparse resources support: GPU will access all 2D (multisample) sparse resources using the standard sparse image block shapes (based on pixel format)
    val mutable residencyStandard2DMultisampleBlockShape: VkBool32
    /// Sparse resources support: GPU will access all 3D sparse resources using the standard sparse image block shapes (based on pixel format)
    val mutable residencyStandard3DBlockShape: VkBool32
    /// Sparse resources support: Images with mip level dimensions that are NOT a multiple of the sparse image block dimensions will be placed in the mip tail
    val mutable residencyAlignedMipSize: VkBool32
    /// Sparse resources support: GPU can consistently access non-resident regions of a resource, all reads return as if data is 0, writes are discarded
    val mutable residencyNonResidentStrict: VkBool32

[<Struct>]
type VkPhysicalDeviceLimits =
    /// max 1D image dimension
    val mutable maxImageDimension1D: uint32
    /// max 2D image dimension
    val mutable maxImageDimension2D: uint32
    /// max 3D image dimension
    val mutable maxImageDimension3D: uint32
    /// max cubemap image dimension
    val mutable maxImageDimensionCube: uint32
    /// max layers for image arrays
    val mutable maxImageArrayLayers: uint32
    /// max texel buffer size (fstexels)
    val mutable maxTexelBufferElements: uint32
    /// max uniform buffer range (bytes)
    val mutable maxUniformBufferRange: uint32
    /// max storage buffer range (bytes)
    val mutable maxStorageBufferRange: uint32
    /// max size of the push constants pool (bytes)
    val maxPushConstantsSize: uint32
    /// max number of device memory allocations supported
    val mutable maxMemoryAllocationCount: uint32
    /// max number of samplers that can be allocated on a device
    val mutable maxSamplerAllocationCount: uint32
    /// Granularity (in bytes) at which buffers and images can be bound to adjacent memory for simultaneous usage
    val mutable bufferImageGranularity: VkDeviceSize
    /// Total address space available for sparse allocations (bytes)
    val mutable sparseAddressSpaceSize: VkDeviceSize
    /// max number of descriptors sets that can be bound to a pipeline
    val mutable maxBoundDescriptorSets: uint32
    /// max number of samplers allowed per-stage in a descriptor set
    val mutable maxPerStageDescriptorSamplers: uint32
    /// max number of uniform buffers allowed per-stage in a descriptor set
    val mutable maxPerStageDescriptorUniformBuffers: uint32
    /// max number of storage buffers allowed per-stage in a descriptor set
    val mutable maxPerStageDescriptorStorageBuffers: uint32
    /// max number of sampled images allowed per-stage in a descriptor set
    val mutable maxPerStageDescriptorSampledImages: uint32
    /// max number of storage images allowed per-stage in a descriptor set
    val mutable maxPerStageDescriptorStorageImages: uint32
    /// max number of input attachments allowed per-stage in a descriptor set
    val mutable maxPerStageDescriptorInputAttachments: uint32
    /// max number of resources allowed by a single stage
    val mutable maxPerStageResources: uint32
    /// max number of samplers allowed in all stages in a descriptor set
    val mutable maxDescriptorSetSamplers: uint32
    /// max number of uniform buffers allowed in all stages in a descriptor set
    val mutable maxDescriptorSetUniformBuffers: uint32
    /// max number of dynamic uniform buffers allowed in all stages in a descriptor set
    val mutable maxDescriptorSetUniformBuffersDynamic: uint32
    /// max number of storage buffers allowed in all stages in a descriptor set
    val mutable maxDescriptorSetStorageBuffers: uint32
    /// max number of dynamic storage buffers allowed in all stages in a descriptor set
    val mutable maxDescriptorSetStorageBuffersDynamic: uint32
    /// max number of sampled images allowed in all stages in a descriptor set
    val mutable maxDescriptorSetSampledImages: uint32
    /// max number of storage images allowed in all stages in a descriptor set
    val mutable maxDescriptorSetStorageImages: uint32
    /// max number of input attachments allowed in all stages in a descriptor set
    val mutable maxDescriptorSetInputAttachments: uint32
    /// max number of vertex input attribute slots
    val mutable maxVertexInputAttributes: uint32
    /// max number of vertex input binding slots
    val mutable maxVertexInputBindings: uint32
    /// max vertex input attribute offset added to vertex buffer offset
    val mutable maxVertexInputAttributeOffset: uint32
    /// max vertex input binding stride
    val mutable maxVertexInputBindingStride: uint32
    /// max number of output components written by vertex shader
    val mutable maxVertexOutputComponents: uint32
    /// max level supported by tessellation primitive generator
    val mutable maxTessellationGenerationLevel: uint32
    /// max patch size (vertices)
    val mutable maxTessellationPatchSize: uint32
    /// max number of input components per-vertex in TCS
    val mutable maxTessellationControlPerVertexInputComponents: uint32
    /// max number of output components per-vertex in TCS
    val mutable maxTessellationControlPerVertexOutputComponents: uint32
    /// max number of output components per-patch in TCS
    val mutable maxTessellationControlPerPatchOutputComponents: uint32
    /// max total number of per-vertex and per-patch output components in TCS
    val mutable maxTessellationControlTotalOutputComponents: uint32
    /// max number of input components per vertex in TES
    val mutable maxTessellationEvaluationInputComponents: uint32
    /// max number of output components per vertex in TES
    val mutable maxTessellationEvaluationOutputComponents: uint32
    /// max invocation count supported in geometry shader
    val mutable maxGeometryShaderInvocations: uint32
    /// max number of input components read in geometry stage
    val mutable maxGeometryInputComponents: uint32
    /// max number of output components written in geometry stage
    val mutable maxGeometryOutputComponents: uint32
    /// max number of vertices that can be emitted in geometry stage
    val mutable maxGeometryOutputVertices: uint32
    /// max total number of components (all vertices) written in geometry stage
    val mutable maxGeometryTotalOutputComponents: uint32
    /// max number of input components read in fragment stage
    val mutable maxFragmentInputComponents: uint32
    /// max number of output attachments written in fragment stage
    val mutable maxFragmentOutputAttachments: uint32
    /// max number of output attachments written when using dual source blending
    val mutable maxFragmentDualSrcAttachments: uint32
    /// max total number of storage buffers, storage images and output buffers
    val mutable maxFragmentCombinedOutputResources: uint32
    /// max total storage size of work group local storage (bytes)
    val mutable maxComputeSharedMemorySize: uint32
    /// max num of compute work groups that may be dispatched by a single command (x,y,z)
    val mutable maxComputeWorkGroupCount: uint32
    /// max total compute invocations in a single local work group
    val mutable maxComputeWorkGroupInvocations: uint32
    /// max local size of a compute work group (x,y,z)
    val mutable maxComputeWorkGroupSize: uint32
    /// number bits of subpixel precision in screen x and y
    val mutable subPixelPrecisionBits: uint32
    /// number bits of precision for selecting texel weights
    val mutable subTexelPrecisionBits: uint32
    /// number bits of precision for selecting mipmap weights
    val mutable mipmapPrecisionBits: uint32
    /// max index value for indexed draw calls (for 32-bit indices)
    val mutable maxDrawIndexedIndexValue: uint32
    /// max draw count for indirect draw calls
    val mutable maxDrawIndirectCount: uint32
    /// max absolute sampler LOD bias
    val mutable maxSamplerLodBias: float32
    /// max degree of sampler anisotropy
    val mutable maxSamplerAnisotropy: float32
    /// max number of active viewports
    val mutable maxViewports: uint32
    /// max viewport dimensions (x,y)
    val mutable maxViewportDimensions: uint32
    /// viewport bounds range (min,max)
    val mutable viewportBoundsRange: float32
    /// number bits of subpixel precision for viewport
    val mutable viewportSubPixelBits: uint32
    /// min required alignment of pointers returned by MapMemory (bytes)
    val mutable minMemoryMapAlignment: nativeint
    /// min required alignment for texel buffer offsets (bytes) 
    val mutable minTexelBufferOffsetAlignment: VkDeviceSize
    /// min required alignment for uniform buffer sizes and offsets (bytes)
    val mutable minUniformBufferOffsetAlignment: VkDeviceSize
    /// min required alignment for storage buffer offsets (bytes)
    val mutable minStorageBufferOffsetAlignment: VkDeviceSize
    /// min texel offset for OpTextureSampleOffset
    val mutable minTexelOffset: int
    /// max texel offset for OpTextureSampleOffset
    val mutable maxTexelOffset: uint32
    /// min texel offset for OpTextureGatherOffset
    val mutable minTexelGatherOffset: int
    /// max texel offset for OpTextureGatherOffset
    val mutable maxTexelGatherOffset: uint32
    /// furthest negative offset for interpolateAtOffset
    val mutable minInterpolationOffset: float32
    /// furthest positive offset for interpolateAtOffset
    val mutable maxInterpolationOffset: float32
    /// number of subpixel bits for interpolateAtOffset
    val mutable subPixelInterpolationOffsetBits: uint32
    /// max width for a framebuffer
    val mutable maxFramebufferWidth: uint32
    /// max height for a framebuffer
    val mutable maxFramebufferHeight: uint32
    /// max layer count for a layered framebuffer
    val mutable maxFramebufferLayers: uint32
    /// supported color sample counts for a framebuffer
    val mutable framebufferColorSampleCounts: VkSampleCountFlags
    /// supported depth sample counts for a framebuffer
    val mutable framebufferDepthSampleCounts: VkSampleCountFlags
    /// supported stencil sample counts for a framebuffer
    val mutable framebufferStencilSampleCounts: VkSampleCountFlags
    /// supported sample counts for a framebuffer with no attachments
    val mutable framebufferNoAttachmentsSampleCounts: VkSampleCountFlags
    /// max number of color attachments per subpass
    val mutable maxColorAttachments: uint32
    /// supported color sample counts for a non-integer sampled image
    val mutable sampledImageColorSampleCounts: VkSampleCountFlags
    /// supported sample counts for an integer image
    val mutable sampledImageIntegerSampleCounts: VkSampleCountFlags
    /// supported depth sample counts for a sampled image
    val mutable sampledImageDepthSampleCounts: VkSampleCountFlags
    /// supported stencil sample counts for a sampled image
    val mutable sampledImageStencilSampleCounts: VkSampleCountFlags
    /// supported sample counts for a storage image
    val mutable storageImageSampleCounts: VkSampleCountFlags
    /// max number of sample mask words
    val mutable maxSampleMaskWords: uint32
    /// timestamps on graphics and compute queues
    val mutable timestampComputeAndGraphics: VkBool32
    /// number of nanoseconds it takes for timestamp query value to increment by 1
    val mutable timestampPeriod: float32
    /// max number of clip distances
    val mutable maxClipDistances: uint32
    /// max number of cull distances
    val mutable maxCullDistances: uint32
    /// max combined number of user clipping
    val mutable maxCombinedClipAndCullDistances: uint32
    /// distinct queue priorities available 
    val mutable discreteQueuePriorities: uint32
    /// range (min,max) of supported point sizes
    val mutable pointSizeRange: float32
    /// range (min,max) of supported line widths
    val mutable lineWidthRange: float32
    /// granularity of supported point sizes
    val mutable pointSizeGranularity: float32
    /// granularity of supported line widths
    val mutable lineWidthGranularity: float32
    /// line rasterization follows preferred rules
    val mutable strictLines: VkBool32
    /// supports standard sample locations for all supported sample counts
    val mutable standardSampleLocations: VkBool32
    /// optimal offset of buffer copies
    val mutable optimalBufferCopyOffsetAlignment: VkDeviceSize
    /// optimal pitch of buffer copies
    val mutable optimalBufferCopyRowPitchAlignment: VkDeviceSize
    /// minimum size and alignment for non-coherent host-mapped device memory access
    val mutable nonCoherentAtomSize: VkDeviceSize

[<Struct>]
type VkSemaphoreCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Semaphore creation flags
    val mutable flags: VkSemaphoreCreateFlags

[<Struct>]
type VkQueryPoolCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkQueryPoolCreateFlags
    val mutable queryType: VkQueryType
    val mutable queryCount: uint32
    /// Optional
    val mutable pipelineStatistics: VkQueryPipelineStatisticFlags

[<Struct>]
type VkFramebufferCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkFramebufferCreateFlags
    val mutable renderPass: VkRenderPass
    val mutable attachmentCount: uint32
    val pAttachments: nativeptr<VkImageView>
    val mutable width: uint32
    val mutable height: uint32
    val mutable layers: uint32

[<Struct>]
type VkDrawIndirectCommand =
    val mutable vertexCount: uint32
    val mutable instanceCount: uint32
    val mutable firstVertex: uint32
    val mutable firstInstance: uint32

[<Struct>]
type VkDrawIndexedIndirectCommand =
    val mutable indexCount: uint32
    val mutable instanceCount: uint32
    val mutable firstIndex: uint32
    val mutable vertexOffset: int
    val mutable firstInstance: uint32

[<Struct>]
type VkDispatchIndirectCommand =
    val mutable x: uint32
    val mutable y: uint32
    val mutable z: uint32

[<Struct>]
type VkSubmitInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable waitSemaphoreCount: uint32
    val pWaitSemaphores: nativeptr<VkSemaphore>
    val pWaitDstStageMask: nativeptr<VkPipelineStageFlags>
    val mutable commandBufferCount: uint32
    val pCommandBuffers: nativeptr<VkCommandBuffer>
    val mutable signalSemaphoreCount: uint32
    val pSignalSemaphores: nativeptr<VkSemaphore>

[<Struct>]
type VkDisplayPropertiesKHR =
    /// Handle of the display object
    val mutable display: VkDisplayKHR
    /// Name of the display
    val displayName: nativeptr<char>
    /// In millimeters?
    val mutable physicalDimensions: VkExtent2D
    /// Max resolution for CRT?
    val mutable physicalResolution: VkExtent2D
    /// one or more bits from VkSurfaceTransformFlagsKHR
    val mutable supportedTransforms: VkSurfaceTransformFlagsKHR
    /// VK_TRUE if the overlay plane's z-order can be changed on this display.
    val mutable planeReorderPossible: VkBool32
    /// VK_TRUE if this is a "smart" display that supports self-refresh/internal buffering.
    val mutable persistentContent: VkBool32

[<Struct>]
type VkDisplayPlanePropertiesKHR =
    /// Display the plane is currently associated with.  Will be VK_NULL_HANDLE if the plane is not in use.
    val mutable currentDisplay: VkDisplayKHR
    /// Current z-order of the plane.
    val mutable currentStackIndex: uint32

[<Struct>]
type VkDisplayModeParametersKHR =
    /// Visible scanout region.
    val mutable visibleRegion: VkExtent2D
    /// Number of times per second the display is updated.
    val mutable refreshRate: uint32

[<Struct>]
type VkDisplayModePropertiesKHR =
    /// Handle of this display mode.
    val mutable displayMode: VkDisplayModeKHR
    /// The parameters this mode uses.
    val mutable parameters: VkDisplayModeParametersKHR

[<Struct>]
type VkDisplayModeCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDisplayModeCreateFlagsKHR
    /// The parameters this mode uses.
    val mutable parameters: VkDisplayModeParametersKHR

[<Struct>]
type VkDisplayPlaneCapabilitiesKHR =
    /// Types of alpha blending supported, if any.
    val mutable supportedAlpha: VkDisplayPlaneAlphaFlagsKHR
    /// Does the plane have any position and extent restrictions?
    val mutable minSrcPosition: VkOffset2D
    val mutable maxSrcPosition: VkOffset2D
    val mutable minSrcExtent: VkExtent2D
    val mutable maxSrcExtent: VkExtent2D
    val mutable minDstPosition: VkOffset2D
    val mutable maxDstPosition: VkOffset2D
    val mutable minDstExtent: VkExtent2D
    val mutable maxDstExtent: VkExtent2D

[<Struct>]
type VkDisplaySurfaceCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDisplaySurfaceCreateFlagsKHR
    /// The mode to use when displaying this surface
    val mutable displayMode: VkDisplayModeKHR
    /// The plane on which this surface appears.  Must be between 0 and the value returned by vkGetPhysicalDeviceDisplayPlanePropertiesKHR() in pPropertyCount.
    val mutable planeIndex: uint32
    /// The z-order of the plane.
    val mutable planeStackIndex: uint32
    /// Transform to apply to the images as part of the scanout operation
    val mutable transform: VkSurfaceTransformFlagBitsKHR
    /// Global alpha value.  Must be between 0 and 1, inclusive.  Ignored if alphaMode is not VK_DISPLAY_PLANE_ALPHA_GLOBAL_BIT_KHR
    val mutable globalAlpha: float32
    /// What type of alpha blending to use.  Must be a bit from vkGetDisplayPlanePropertiesKHR::supportedAlpha.
    val mutable alphaMode: VkDisplayPlaneAlphaFlagBitsKHR
    /// size of the images to use with this surface
    val mutable imageExtent: VkExtent2D

[<Struct>]
type VkDisplayPresentInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Rectangle within the presentable image to read pixel data from when presenting to the display.
    val mutable srcRect: VkRect2D
    /// Rectangle within the current display mode's visible region to display srcRectangle in.
    val mutable dstRect: VkRect2D
    /// For smart displays, use buffered mode.  If the display properties member "persistentMode" is VK_FALSE, this member must always be VK_FALSE.
    val mutable persistent: VkBool32

[<Struct>]
type VkSurfaceCapabilitiesKHR =
    /// Supported minimum number of images for the surface
    val mutable minImageCount: uint32
    /// Supported maximum number of images for the surface, 0 for unlimited
    val mutable maxImageCount: uint32
    /// Current image width and height for the surface, (0, 0) if undefined
    val mutable currentExtent: VkExtent2D
    /// Supported minimum image width and height for the surface
    val mutable minImageExtent: VkExtent2D
    /// Supported maximum image width and height for the surface
    val mutable maxImageExtent: VkExtent2D
    /// Supported maximum number of image layers for the surface
    val mutable maxImageArrayLayers: uint32
    /// 1 or more bits representing the transforms supported
    val mutable supportedTransforms: VkSurfaceTransformFlagsKHR
    /// The surface's current transform relative to the device's natural orientation
    val mutable currentTransform: VkSurfaceTransformFlagBitsKHR
    /// 1 or more bits representing the alpha compositing modes supported
    val mutable supportedCompositeAlpha: VkCompositeAlphaFlagsKHR
    /// Supported image usage flags for the surface
    val mutable supportedUsageFlags: VkImageUsageFlags

[<Struct>]
type VkAndroidSurfaceCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkAndroidSurfaceCreateFlagsKHR
    val mutable window: nativeptr<ANativeWindow>

[<Struct>]
type VkViSurfaceCreateInfoNN =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkViSurfaceCreateFlagsNN
    val mutable window: nativeint

[<Struct>]
type VkWaylandSurfaceCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkWaylandSurfaceCreateFlagsKHR
    val mutable display: nativeptr<wl_display>
    val mutable surface: nativeptr<wl_surface>

[<Struct>]
type VkWin32SurfaceCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkWin32SurfaceCreateFlagsKHR
    val mutable hinstance: HINSTANCE
    val mutable hwnd: HWND

[<Struct>]
type VkXlibSurfaceCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkXlibSurfaceCreateFlagsKHR
    val mutable dpy: nativeptr<Display>
    val mutable window: Window

[<Struct>]
type VkXcbSurfaceCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkXcbSurfaceCreateFlagsKHR
    val mutable connection: nativeptr<xcb_connection_t>
    val mutable window: xcb_window_t

[<Struct>]
type VkImagePipeSurfaceCreateInfoFUCHSIA =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkImagePipeSurfaceCreateFlagsFUCHSIA
    val mutable imagePipeHandle: zx_handle_t

[<Struct>]
type VkStreamDescriptorSurfaceCreateInfoGGP =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkStreamDescriptorSurfaceCreateFlagsGGP
    val mutable streamDescriptor: GgpStreamDescriptor

[<Struct>]
type VkSurfaceFormatKHR =
    /// Supported pair of rendering format
    val mutable format: VkFormat
    /// and color space for the surface
    val mutable colorSpace: VkColorSpaceKHR

[<Struct>]
type VkSwapchainCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkSwapchainCreateFlagsKHR
    /// The swapchain's target surface
    val mutable surface: VkSurfaceKHR
    /// Minimum number of presentation images the application needs
    val mutable minImageCount: uint32
    /// Format of the presentation images
    val mutable imageFormat: VkFormat
    /// Colorspace of the presentation images
    val mutable imageColorSpace: VkColorSpaceKHR
    /// Dimensions of the presentation images
    val mutable imageExtent: VkExtent2D
    /// Determines the number of views for multiview/stereo presentation
    val mutable imageArrayLayers: uint32
    /// Bits indicating how the presentation images will be used
    val mutable imageUsage: VkImageUsageFlags
    /// Sharing mode used for the presentation images
    val mutable imageSharingMode: VkSharingMode
    /// Number of queue families having access to the images in case of concurrent sharing mode
    val mutable queueFamilyIndexCount: uint32
    /// Array of queue family indices having access to the images in case of concurrent sharing mode
    val pQueueFamilyIndices: nativeptr<uint32>
    /// The transform, relative to the device's natural orientation, applied to the image content prior to presentation
    val mutable preTransform: VkSurfaceTransformFlagBitsKHR
    /// The alpha blending mode used when compositing this surface with other surfaces in the window system
    val mutable compositeAlpha: VkCompositeAlphaFlagBitsKHR
    /// Which presentation mode to use for presents on this swap chain
    val mutable presentMode: VkPresentModeKHR
    /// Specifies whether presentable images may be affected by window clip regions
    val mutable clipped: VkBool32
    /// Existing swap chain to replace, if any
    val mutable oldSwapchain: VkSwapchainKHR

[<Struct>]
type VkPresentInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Number of semaphores to wait for before presenting
    val mutable waitSemaphoreCount: uint32
    /// Semaphores to wait for before presenting
    val pWaitSemaphores: nativeptr<VkSemaphore>
    /// Number of swapchains to present in this call
    val mutable swapchainCount: uint32
    /// Swapchains to present an image from
    val pSwapchains: nativeptr<VkSwapchainKHR>
    /// Indices of which presentable images to present
    val pImageIndices: nativeptr<uint32>
    /// Optional (i.e. if non-NULL) VkResult for each swapchain
    val mutable pResults: nativeptr<VkResult>

[<Struct>]
type VkDebugReportCallbackCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Indicates which events call this callback
    val mutable flags: VkDebugReportFlagsEXT
    /// Function pointer of a callback function
    val mutable pfnCallback: PFN_vkDebugReportCallbackEXT
    /// User data provided to callback function
    val mutable pUserData: nativeint

[<Struct>]
type VkValidationFlagsEXT =
    /// Must be VK_STRUCTURE_TYPE_VALIDATION_FLAGS_EXT
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Number of validation checks to disable
    val mutable disabledValidationCheckCount: uint32
    /// Validation checks to disable
    val pDisabledValidationChecks: nativeptr<VkValidationCheckEXT>

[<Struct>]
type VkValidationFeaturesEXT =
    /// Must be VK_STRUCTURE_TYPE_VALIDATION_FEATURES_EXT
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Number of validation features to enable
    val mutable enabledValidationFeatureCount: uint32
    /// Validation features to enable
    val pEnabledValidationFeatures: nativeptr<VkValidationFeatureEnableEXT>
    /// Number of validation features to disable
    val mutable disabledValidationFeatureCount: uint32
    /// Validation features to disable
    val pDisabledValidationFeatures: nativeptr<VkValidationFeatureDisableEXT>

[<Struct>]
type VkPipelineRasterizationStateRasterizationOrderAMD =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Rasterization order to use for the pipeline
    val mutable rasterizationOrder: VkRasterizationOrderAMD

[<Struct>]
type VkDebugMarkerObjectNameInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// The type of the object
    val mutable objectType: VkDebugReportObjectTypeEXT
    /// The handle of the object, cast to uint64_t
    val mutable object: uint64
    /// Name to apply to the object
    val pObjectName: nativeptr<char>

[<Struct>]
type VkDebugMarkerObjectTagInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// The type of the object
    val mutable objectType: VkDebugReportObjectTypeEXT
    /// The handle of the object, cast to uint64_t
    val mutable object: uint64
    /// The name of the tag to set on the object
    val mutable tagName: uint64
    /// The length in bytes of the tag data
    val mutable tagSize: nativeint
    /// Tag data to attach to the object
    val pTag: nativeint

[<Struct>]
type VkDebugMarkerMarkerInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Name of the debug marker
    val pMarkerName: nativeptr<char>
    /// Optional color for debug marker
    val mutable color: float32

[<Struct>]
type VkDedicatedAllocationImageCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Whether this image uses a dedicated allocation
    val mutable dedicatedAllocation: VkBool32

[<Struct>]
type VkDedicatedAllocationBufferCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Whether this buffer uses a dedicated allocation
    val mutable dedicatedAllocation: VkBool32

[<Struct>]
type VkDedicatedAllocationMemoryAllocateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Image that this allocation will be bound to
    val mutable image: VkImage
    /// Buffer that this allocation will be bound to
    val mutable buffer: VkBuffer

[<Struct>]
type VkExternalImageFormatPropertiesNV =
    val mutable imageFormatProperties: VkImageFormatProperties
    val mutable externalMemoryFeatures: VkExternalMemoryFeatureFlagsNV
    val mutable exportFromImportedHandleTypes: VkExternalMemoryHandleTypeFlagsNV
    val mutable compatibleHandleTypes: VkExternalMemoryHandleTypeFlagsNV

[<Struct>]
type VkExternalMemoryImageCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalMemoryHandleTypeFlagsNV

[<Struct>]
type VkExportMemoryAllocateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalMemoryHandleTypeFlagsNV

[<Struct>]
type VkImportMemoryWin32HandleInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalMemoryHandleTypeFlagsNV
    val mutable handle: HANDLE

[<Struct>]
type VkExportMemoryWin32HandleInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val pAttributes: nativeptr<SECURITY_ATTRIBUTES>
    val mutable dwAccess: DWORD

[<Struct>]
type VkWin32KeyedMutexAcquireReleaseInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable acquireCount: uint32
    val pAcquireSyncs: nativeptr<VkDeviceMemory>
    val pAcquireKeys: nativeptr<uint64>
    val pAcquireTimeoutMilliseconds: nativeptr<uint32>
    val mutable releaseCount: uint32
    val pReleaseSyncs: nativeptr<VkDeviceMemory>
    val pReleaseKeys: nativeptr<uint64>

[<Struct>]
type VkDeviceGeneratedCommandsFeaturesNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable computeBindingPointSupport: VkBool32

[<Struct>]
type VkDeviceGeneratedCommandsLimitsNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable maxIndirectCommandsLayoutTokenCount: uint32
    val mutable maxObjectEntryCounts: uint32
    val mutable minSequenceCountBufferOffsetAlignment: uint32
    val mutable minSequenceIndexBufferOffsetAlignment: uint32
    val mutable minCommandsTokenBufferOffsetAlignment: uint32

[<Struct>]
type VkIndirectCommandsTokenNVX =
    val mutable tokenType: VkIndirectCommandsTokenTypeNVX
    /// buffer containing tableEntries and additional data for indirectCommands
    val mutable buffer: VkBuffer
    /// offset from the base address of the buffer
    val mutable offset: VkDeviceSize

[<Struct>]
type VkIndirectCommandsLayoutTokenNVX =
    val mutable tokenType: VkIndirectCommandsTokenTypeNVX
    /// Binding unit for vertex attribute / descriptor set, offset for pushconstants
    val bindingUnit: uint32
    /// Number of variable dynamic values for descriptor set / push constants
    val dynamicCount: uint32
    /// Rate the which the array is advanced per element (must be power of 2, minimum 1)
    val mutable divisor: uint32

[<Struct>]
type VkIndirectCommandsLayoutCreateInfoNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable pipelineBindPoint: VkPipelineBindPoint
    val mutable flags: VkIndirectCommandsLayoutUsageFlagsNVX
    val mutable tokenCount: uint32
    val pTokens: nativeptr<VkIndirectCommandsLayoutTokenNVX>

[<Struct>]
type VkCmdProcessCommandsInfoNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable objectTable: VkObjectTableNVX
    val mutable indirectCommandsLayout: VkIndirectCommandsLayoutNVX
    val mutable indirectCommandsTokenCount: uint32
    val pIndirectCommandsTokens: nativeptr<VkIndirectCommandsTokenNVX>
    val mutable maxSequencesCount: uint32
    val mutable targetCommandBuffer: VkCommandBuffer
    val mutable sequencesCountBuffer: VkBuffer
    val mutable sequencesCountOffset: VkDeviceSize
    val mutable sequencesIndexBuffer: VkBuffer
    val mutable sequencesIndexOffset: VkDeviceSize

[<Struct>]
type VkCmdReserveSpaceForCommandsInfoNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable objectTable: VkObjectTableNVX
    val mutable indirectCommandsLayout: VkIndirectCommandsLayoutNVX
    val mutable maxSequencesCount: uint32

[<Struct>]
type VkObjectTableCreateInfoNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable objectCount: uint32
    val pObjectEntryTypes: nativeptr<VkObjectEntryTypeNVX>
    val pObjectEntryCounts: nativeptr<uint32>
    val pObjectEntryUsageFlags: nativeptr<VkObjectEntryUsageFlagsNVX>
    val mutable maxUniformBuffersPerDescriptor: uint32
    val mutable maxStorageBuffersPerDescriptor: uint32
    val mutable maxStorageImagesPerDescriptor: uint32
    val mutable maxSampledImagesPerDescriptor: uint32
    val mutable maxPipelineLayouts: uint32

[<Struct>]
type VkObjectTableEntryNVX =
    val mutable typ: VkObjectEntryTypeNVX
    val mutable flags: VkObjectEntryUsageFlagsNVX

[<Struct>]
type VkObjectTablePipelineEntryNVX =
    val mutable typ: VkObjectEntryTypeNVX
    val mutable flags: VkObjectEntryUsageFlagsNVX
    val mutable pipeline: VkPipeline

[<Struct>]
type VkObjectTableDescriptorSetEntryNVX =
    val mutable typ: VkObjectEntryTypeNVX
    val mutable flags: VkObjectEntryUsageFlagsNVX
    val mutable pipelineLayout: VkPipelineLayout
    val mutable descriptorSet: VkDescriptorSet

[<Struct>]
type VkObjectTableVertexBufferEntryNVX =
    val mutable typ: VkObjectEntryTypeNVX
    val mutable flags: VkObjectEntryUsageFlagsNVX
    val mutable buffer: VkBuffer

[<Struct>]
type VkObjectTableIndexBufferEntryNVX =
    val mutable typ: VkObjectEntryTypeNVX
    val mutable flags: VkObjectEntryUsageFlagsNVX
    val mutable buffer: VkBuffer
    val mutable indexType: VkIndexType

[<Struct>]
type VkObjectTablePushConstantEntryNVX =
    val mutable typ: VkObjectEntryTypeNVX
    val mutable flags: VkObjectEntryUsageFlagsNVX
    val mutable pipelineLayout: VkPipelineLayout
    val mutable stageFlags: VkShaderStageFlags

[<Struct>]
type VkPhysicalDeviceFeatures2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable features: VkPhysicalDeviceFeatures

type VkPhysicalDeviceFeatures2KHR = VkPhysicalDeviceFeatures2

[<Struct>]
type VkPhysicalDeviceProperties2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable properties: VkPhysicalDeviceProperties

type VkPhysicalDeviceProperties2KHR = VkPhysicalDeviceProperties2

[<Struct>]
type VkFormatProperties2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable formatProperties: VkFormatProperties

type VkFormatProperties2KHR = VkFormatProperties2

[<Struct>]
type VkImageFormatProperties2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable imageFormatProperties: VkImageFormatProperties

type VkImageFormatProperties2KHR = VkImageFormatProperties2

[<Struct>]
type VkPhysicalDeviceImageFormatInfo2 =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable format: VkFormat
    val mutable typ: VkImageType
    val mutable tiling: VkImageTiling
    val mutable usage: VkImageUsageFlags
    val mutable flags: VkImageCreateFlags

type VkPhysicalDeviceImageFormatInfo2KHR = VkPhysicalDeviceImageFormatInfo2

[<Struct>]
type VkQueueFamilyProperties2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable queueFamilyProperties: VkQueueFamilyProperties

type VkQueueFamilyProperties2KHR = VkQueueFamilyProperties2

[<Struct>]
type VkPhysicalDeviceMemoryProperties2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryProperties: VkPhysicalDeviceMemoryProperties

type VkPhysicalDeviceMemoryProperties2KHR = VkPhysicalDeviceMemoryProperties2

[<Struct>]
type VkSparseImageFormatProperties2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable properties: VkSparseImageFormatProperties

type VkSparseImageFormatProperties2KHR = VkSparseImageFormatProperties2

[<Struct>]
type VkPhysicalDeviceSparseImageFormatInfo2 =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable format: VkFormat
    val mutable typ: VkImageType
    val mutable samples: VkSampleCountFlagBits
    val mutable usage: VkImageUsageFlags
    val mutable tiling: VkImageTiling

type VkPhysicalDeviceSparseImageFormatInfo2KHR = VkPhysicalDeviceSparseImageFormatInfo2

[<Struct>]
type VkPhysicalDevicePushDescriptorPropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxPushDescriptors: uint32

[<Struct>]
type VkConformanceVersionKHR =
    val mutable major: byte
    val mutable minor: byte
    val mutable subminor: byte
    val mutable patch: byte

[<Struct>]
type VkPhysicalDeviceDriverPropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable driverID: VkDriverIdKHR
    val mutable driverName: char
    val mutable driverInfo: char
    val mutable conformanceVersion: VkConformanceVersionKHR

[<Struct>]
type VkPresentRegionsKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Copy of VkPresentInfoKHR::swapchainCount
    val mutable swapchainCount: uint32
    /// The regions that have changed
    val pRegions: nativeptr<VkPresentRegionKHR>

[<Struct>]
type VkPresentRegionKHR =
    /// Number of rectangles in pRectangles
    val mutable rectangleCount: uint32
    /// Array of rectangles that have changed in a swapchain's image(s)
    val pRectangles: nativeptr<VkRectLayerKHR>

[<Struct>]
type VkRectLayerKHR =
    /// upper-left corner of a rectangle that has not changed, in pixels of a presentation images
    val mutable offset: VkOffset2D
    /// Dimensions of a rectangle that has not changed, in pixels of a presentation images
    val mutable extent: VkExtent2D
    /// Layer of a swapchain's image(s), for stereoscopic-3D images
    val mutable layer: uint32

[<Struct>]
type VkPhysicalDeviceVariablePointersFeatures =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable variablePointersStorageBuffer: VkBool32
    val mutable variablePointers: VkBool32

type VkPhysicalDeviceVariablePointersFeaturesKHR = VkPhysicalDeviceVariablePointersFeatures

type VkPhysicalDeviceVariablePointerFeaturesKHR = VkPhysicalDeviceVariablePointersFeatures

type VkPhysicalDeviceVariablePointerFeatures = VkPhysicalDeviceVariablePointersFeatures

[<Struct>]
type VkExternalMemoryProperties =
    val mutable externalMemoryFeatures: VkExternalMemoryFeatureFlags
    val mutable exportFromImportedHandleTypes: VkExternalMemoryHandleTypeFlags
    val mutable compatibleHandleTypes: VkExternalMemoryHandleTypeFlags

type VkExternalMemoryPropertiesKHR = VkExternalMemoryProperties

[<Struct>]
type VkPhysicalDeviceExternalImageFormatInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits

type VkPhysicalDeviceExternalImageFormatInfoKHR = VkPhysicalDeviceExternalImageFormatInfo

[<Struct>]
type VkExternalImageFormatProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable externalMemoryProperties: VkExternalMemoryProperties

type VkExternalImageFormatPropertiesKHR = VkExternalImageFormatProperties

[<Struct>]
type VkPhysicalDeviceExternalBufferInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkBufferCreateFlags
    val mutable usage: VkBufferUsageFlags
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits

type VkPhysicalDeviceExternalBufferInfoKHR = VkPhysicalDeviceExternalBufferInfo

[<Struct>]
type VkExternalBufferProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable externalMemoryProperties: VkExternalMemoryProperties

type VkExternalBufferPropertiesKHR = VkExternalBufferProperties

[<Struct>]
type VkPhysicalDeviceIDProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable deviceUUID: byte
    val mutable driverUUID: byte
    val mutable deviceLUID: byte
    val mutable deviceNodeMask: uint32
    val mutable deviceLUIDValid: VkBool32

type VkPhysicalDeviceIDPropertiesKHR = VkPhysicalDeviceIDProperties

[<Struct>]
type VkExternalMemoryImageCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalMemoryHandleTypeFlags

type VkExternalMemoryImageCreateInfoKHR = VkExternalMemoryImageCreateInfo

[<Struct>]
type VkExternalMemoryBufferCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalMemoryHandleTypeFlags

type VkExternalMemoryBufferCreateInfoKHR = VkExternalMemoryBufferCreateInfo

[<Struct>]
type VkExportMemoryAllocateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalMemoryHandleTypeFlags

type VkExportMemoryAllocateInfoKHR = VkExportMemoryAllocateInfo

[<Struct>]
type VkImportMemoryWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits
    val mutable handle: HANDLE
    val mutable name: LPCWSTR

[<Struct>]
type VkExportMemoryWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val pAttributes: nativeptr<SECURITY_ATTRIBUTES>
    val mutable dwAccess: DWORD
    val mutable name: LPCWSTR

[<Struct>]
type VkMemoryWin32HandlePropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryTypeBits: uint32

[<Struct>]
type VkMemoryGetWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable memory: VkDeviceMemory
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits

[<Struct>]
type VkImportMemoryFdInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits
    val mutable fd: int

[<Struct>]
type VkMemoryFdPropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryTypeBits: uint32

[<Struct>]
type VkMemoryGetFdInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable memory: VkDeviceMemory
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits

[<Struct>]
type VkWin32KeyedMutexAcquireReleaseInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable acquireCount: uint32
    val pAcquireSyncs: nativeptr<VkDeviceMemory>
    val pAcquireKeys: nativeptr<uint64>
    val pAcquireTimeouts: nativeptr<uint32>
    val mutable releaseCount: uint32
    val pReleaseSyncs: nativeptr<VkDeviceMemory>
    val pReleaseKeys: nativeptr<uint64>

[<Struct>]
type VkPhysicalDeviceExternalSemaphoreInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalSemaphoreHandleTypeFlagBits

type VkPhysicalDeviceExternalSemaphoreInfoKHR = VkPhysicalDeviceExternalSemaphoreInfo

[<Struct>]
type VkExternalSemaphoreProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable exportFromImportedHandleTypes: VkExternalSemaphoreHandleTypeFlags
    val mutable compatibleHandleTypes: VkExternalSemaphoreHandleTypeFlags
    val mutable externalSemaphoreFeatures: VkExternalSemaphoreFeatureFlags

type VkExternalSemaphorePropertiesKHR = VkExternalSemaphoreProperties

[<Struct>]
type VkExportSemaphoreCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalSemaphoreHandleTypeFlags

type VkExportSemaphoreCreateInfoKHR = VkExportSemaphoreCreateInfo

[<Struct>]
type VkImportSemaphoreWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable semaphore: VkSemaphore
    val mutable flags: VkSemaphoreImportFlags
    val mutable handleType: VkExternalSemaphoreHandleTypeFlagBits
    val mutable handle: HANDLE
    val mutable name: LPCWSTR

[<Struct>]
type VkExportSemaphoreWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val pAttributes: nativeptr<SECURITY_ATTRIBUTES>
    val mutable dwAccess: DWORD
    val mutable name: LPCWSTR

[<Struct>]
type VkD3D12FenceSubmitInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable waitSemaphoreValuesCount: uint32
    val pWaitSemaphoreValues: nativeptr<uint64>
    val mutable signalSemaphoreValuesCount: uint32
    val pSignalSemaphoreValues: nativeptr<uint64>

[<Struct>]
type VkSemaphoreGetWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable semaphore: VkSemaphore
    val mutable handleType: VkExternalSemaphoreHandleTypeFlagBits

[<Struct>]
type VkImportSemaphoreFdInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable semaphore: VkSemaphore
    val mutable flags: VkSemaphoreImportFlags
    val mutable handleType: VkExternalSemaphoreHandleTypeFlagBits
    val mutable fd: int

[<Struct>]
type VkSemaphoreGetFdInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable semaphore: VkSemaphore
    val mutable handleType: VkExternalSemaphoreHandleTypeFlagBits

[<Struct>]
type VkPhysicalDeviceExternalFenceInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalFenceHandleTypeFlagBits

type VkPhysicalDeviceExternalFenceInfoKHR = VkPhysicalDeviceExternalFenceInfo

[<Struct>]
type VkExternalFenceProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable exportFromImportedHandleTypes: VkExternalFenceHandleTypeFlags
    val mutable compatibleHandleTypes: VkExternalFenceHandleTypeFlags
    val mutable externalFenceFeatures: VkExternalFenceFeatureFlags

type VkExternalFencePropertiesKHR = VkExternalFenceProperties

[<Struct>]
type VkExportFenceCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleTypes: VkExternalFenceHandleTypeFlags

type VkExportFenceCreateInfoKHR = VkExportFenceCreateInfo

[<Struct>]
type VkImportFenceWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable fence: VkFence
    val mutable flags: VkFenceImportFlags
    val mutable handleType: VkExternalFenceHandleTypeFlagBits
    val mutable handle: HANDLE
    val mutable name: LPCWSTR

[<Struct>]
type VkExportFenceWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val pAttributes: nativeptr<SECURITY_ATTRIBUTES>
    val mutable dwAccess: DWORD
    val mutable name: LPCWSTR

[<Struct>]
type VkFenceGetWin32HandleInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable fence: VkFence
    val mutable handleType: VkExternalFenceHandleTypeFlagBits

[<Struct>]
type VkImportFenceFdInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable fence: VkFence
    val mutable flags: VkFenceImportFlags
    val mutable handleType: VkExternalFenceHandleTypeFlagBits
    val mutable fd: int

[<Struct>]
type VkFenceGetFdInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable fence: VkFence
    val mutable handleType: VkExternalFenceHandleTypeFlagBits

[<Struct>]
type VkPhysicalDeviceMultiviewFeatures =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// Multiple views in a renderpass
    val mutable multiview: VkBool32
    /// Multiple views in a renderpass w/ geometry shader
    val mutable multiviewGeometryShader: VkBool32
    /// Multiple views in a renderpass w/ tessellation shader
    val mutable multiviewTessellationShader: VkBool32

type VkPhysicalDeviceMultiviewFeaturesKHR = VkPhysicalDeviceMultiviewFeatures

[<Struct>]
type VkPhysicalDeviceMultiviewProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// max number of views in a subpass
    val mutable maxMultiviewViewCount: uint32
    /// max instance index for a draw in a multiview subpass
    val mutable maxMultiviewInstanceIndex: uint32

type VkPhysicalDeviceMultiviewPropertiesKHR = VkPhysicalDeviceMultiviewProperties

[<Struct>]
type VkRenderPassMultiviewCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable subpassCount: uint32
    val pViewMasks: nativeptr<uint32>
    val mutable dependencyCount: uint32
    val pViewOffsets: nativeptr<int>
    val mutable correlationMaskCount: uint32
    val pCorrelationMasks: nativeptr<uint32>

type VkRenderPassMultiviewCreateInfoKHR = VkRenderPassMultiviewCreateInfo

[<Struct>]
type VkSurfaceCapabilities2EXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// Supported minimum number of images for the surface
    val mutable minImageCount: uint32
    /// Supported maximum number of images for the surface, 0 for unlimited
    val mutable maxImageCount: uint32
    /// Current image width and height for the surface, (0, 0) if undefined
    val mutable currentExtent: VkExtent2D
    /// Supported minimum image width and height for the surface
    val mutable minImageExtent: VkExtent2D
    /// Supported maximum image width and height for the surface
    val mutable maxImageExtent: VkExtent2D
    /// Supported maximum number of image layers for the surface
    val mutable maxImageArrayLayers: uint32
    /// 1 or more bits representing the transforms supported
    val mutable supportedTransforms: VkSurfaceTransformFlagsKHR
    /// The surface's current transform relative to the device's natural orientation
    val mutable currentTransform: VkSurfaceTransformFlagBitsKHR
    /// 1 or more bits representing the alpha compositing modes supported
    val mutable supportedCompositeAlpha: VkCompositeAlphaFlagsKHR
    /// Supported image usage flags for the surface
    val mutable supportedUsageFlags: VkImageUsageFlags
    val mutable supportedSurfaceCounters: VkSurfaceCounterFlagsEXT

[<Struct>]
type VkDisplayPowerInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable powerState: VkDisplayPowerStateEXT

[<Struct>]
type VkDeviceEventInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable deviceEvent: VkDeviceEventTypeEXT

[<Struct>]
type VkDisplayEventInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable displayEvent: VkDisplayEventTypeEXT

[<Struct>]
type VkSwapchainCounterCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable surfaceCounters: VkSurfaceCounterFlagsEXT

[<Struct>]
type VkPhysicalDeviceGroupProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable physicalDeviceCount: uint32
    val mutable physicalDevices: VkPhysicalDevice
    val mutable subsetAllocation: VkBool32

type VkPhysicalDeviceGroupPropertiesKHR = VkPhysicalDeviceGroupProperties

[<Struct>]
type VkMemoryAllocateFlagsInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkMemoryAllocateFlags
    val mutable deviceMask: uint32

type VkMemoryAllocateFlagsInfoKHR = VkMemoryAllocateFlagsInfo

[<Struct>]
type VkBindBufferMemoryInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable buffer: VkBuffer
    val mutable memory: VkDeviceMemory
    val mutable memoryOffset: VkDeviceSize

type VkBindBufferMemoryInfoKHR = VkBindBufferMemoryInfo

[<Struct>]
type VkBindBufferMemoryDeviceGroupInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable deviceIndexCount: uint32
    val pDeviceIndices: nativeptr<uint32>

type VkBindBufferMemoryDeviceGroupInfoKHR = VkBindBufferMemoryDeviceGroupInfo

[<Struct>]
type VkBindImageMemoryInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable image: VkImage
    val mutable memory: VkDeviceMemory
    val mutable memoryOffset: VkDeviceSize

type VkBindImageMemoryInfoKHR = VkBindImageMemoryInfo

[<Struct>]
type VkBindImageMemoryDeviceGroupInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable deviceIndexCount: uint32
    val pDeviceIndices: nativeptr<uint32>
    val mutable splitInstanceBindRegionCount: uint32
    val pSplitInstanceBindRegions: nativeptr<VkRect2D>

type VkBindImageMemoryDeviceGroupInfoKHR = VkBindImageMemoryDeviceGroupInfo

[<Struct>]
type VkDeviceGroupRenderPassBeginInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable deviceMask: uint32
    val mutable deviceRenderAreaCount: uint32
    val pDeviceRenderAreas: nativeptr<VkRect2D>

type VkDeviceGroupRenderPassBeginInfoKHR = VkDeviceGroupRenderPassBeginInfo

[<Struct>]
type VkDeviceGroupCommandBufferBeginInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable deviceMask: uint32

type VkDeviceGroupCommandBufferBeginInfoKHR = VkDeviceGroupCommandBufferBeginInfo

[<Struct>]
type VkDeviceGroupSubmitInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable waitSemaphoreCount: uint32
    val pWaitSemaphoreDeviceIndices: nativeptr<uint32>
    val mutable commandBufferCount: uint32
    val pCommandBufferDeviceMasks: nativeptr<uint32>
    val mutable signalSemaphoreCount: uint32
    val pSignalSemaphoreDeviceIndices: nativeptr<uint32>

type VkDeviceGroupSubmitInfoKHR = VkDeviceGroupSubmitInfo

[<Struct>]
type VkDeviceGroupBindSparseInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable resourceDeviceIndex: uint32
    val mutable memoryDeviceIndex: uint32

type VkDeviceGroupBindSparseInfoKHR = VkDeviceGroupBindSparseInfo

[<Struct>]
type VkDeviceGroupPresentCapabilitiesKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable presentMask: uint32
    val mutable modes: VkDeviceGroupPresentModeFlagsKHR

[<Struct>]
type VkImageSwapchainCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable swapchain: VkSwapchainKHR

[<Struct>]
type VkBindImageMemorySwapchainInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable swapchain: VkSwapchainKHR
    val mutable imageIndex: uint32

[<Struct>]
type VkAcquireNextImageInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable swapchain: VkSwapchainKHR
    val mutable timeout: uint64
    val mutable semaphore: VkSemaphore
    val mutable fence: VkFence
    val mutable deviceMask: uint32

[<Struct>]
type VkDeviceGroupPresentInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable swapchainCount: uint32
    val pDeviceMasks: nativeptr<uint32>
    val mutable mode: VkDeviceGroupPresentModeFlagBitsKHR

[<Struct>]
type VkDeviceGroupDeviceCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable physicalDeviceCount: uint32
    val pPhysicalDevices: nativeptr<VkPhysicalDevice>

type VkDeviceGroupDeviceCreateInfoKHR = VkDeviceGroupDeviceCreateInfo

[<Struct>]
type VkDeviceGroupSwapchainCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable modes: VkDeviceGroupPresentModeFlagsKHR

[<Struct>]
type VkDescriptorUpdateTemplateEntry =
    /// Binding within the destination descriptor set to write
    val mutable dstBinding: uint32
    /// Array element within the destination binding to write
    val mutable dstArrayElement: uint32
    /// Number of descriptors to write
    val mutable descriptorCount: uint32
    /// Descriptor type to write
    val mutable descriptorType: VkDescriptorType
    /// Offset into pData where the descriptors to update are stored
    val mutable offset: nativeint
    /// Stride between two descriptors in pData when writing more than one descriptor
    val mutable stride: nativeint

type VkDescriptorUpdateTemplateEntryKHR = VkDescriptorUpdateTemplateEntry

[<Struct>]
type VkDescriptorUpdateTemplateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDescriptorUpdateTemplateCreateFlags
    /// Number of descriptor update entries to use for the update template
    val mutable descriptorUpdateEntryCount: uint32
    /// Descriptor update entries for the template
    val pDescriptorUpdateEntries: nativeptr<VkDescriptorUpdateTemplateEntry>
    val mutable templateType: VkDescriptorUpdateTemplateType
    val mutable descriptorSetLayout: VkDescriptorSetLayout
    val mutable pipelineBindPoint: VkPipelineBindPoint
    /// If used for push descriptors, this is the only allowed layout
    val mutable pipelineLayout: VkPipelineLayout
    val mutable set: uint32

type VkDescriptorUpdateTemplateCreateInfoKHR = VkDescriptorUpdateTemplateCreateInfo

/// Chromaticity coordinate
[<Struct>]
type VkXYColorEXT =
    val mutable x: float32
    val mutable y: float32

[<Struct>]
type VkHdrMetadataEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Display primary's Red
    val mutable displayPrimaryRed: VkXYColorEXT
    /// Display primary's Green
    val mutable displayPrimaryGreen: VkXYColorEXT
    /// Display primary's Blue
    val mutable displayPrimaryBlue: VkXYColorEXT
    /// Display primary's Blue
    val mutable whitePoint: VkXYColorEXT
    /// Display maximum luminance
    val mutable maxLuminance: float32
    /// Display minimum luminance
    val mutable minLuminance: float32
    /// Content maximum luminance
    val mutable maxContentLightLevel: float32
    val mutable maxFrameAverageLightLevel: float32

[<Struct>]
type VkDisplayNativeHdrSurfaceCapabilitiesAMD =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable localDimmingSupport: VkBool32

[<Struct>]
type VkSwapchainDisplayNativeHdrCreateInfoAMD =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable localDimmingEnable: VkBool32

[<Struct>]
type VkRefreshCycleDurationGOOGLE =
    /// Number of nanoseconds from the start of one refresh cycle to the next
    val mutable refreshDuration: uint64

[<Struct>]
type VkPastPresentationTimingGOOGLE =
    /// Application-provided identifier, previously given to vkQueuePresentKHR
    val mutable presentID: uint32
    /// Earliest time an image should have been presented, previously given to vkQueuePresentKHR
    val mutable desiredPresentTime: uint64
    /// Time the image was actually displayed
    val mutable actualPresentTime: uint64
    /// Earliest time the image could have been displayed
    val mutable earliestPresentTime: uint64
    /// How early vkQueuePresentKHR was processed vs. how soon it needed to be and make earliestPresentTime
    val mutable presentMargin: uint64

[<Struct>]
type VkPresentTimesInfoGOOGLE =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Copy of VkPresentInfoKHR::swapchainCount
    val mutable swapchainCount: uint32
    /// The earliest times to present images
    val pTimes: nativeptr<VkPresentTimeGOOGLE>

[<Struct>]
type VkPresentTimeGOOGLE =
    /// Application-provided identifier
    val mutable presentID: uint32
    /// Earliest time an image should be presented
    val mutable desiredPresentTime: uint64

[<Struct>]
type VkIOSSurfaceCreateInfoMVK =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkIOSSurfaceCreateFlagsMVK
    val pView: nativeint

[<Struct>]
type VkMacOSSurfaceCreateInfoMVK =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkMacOSSurfaceCreateFlagsMVK
    val pView: nativeint

[<Struct>]
type VkMetalSurfaceCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkMetalSurfaceCreateFlagsEXT
    val pLayer: nativeptr<CAMetalLayer>

[<Struct>]
type VkViewportWScalingNV =
    val mutable xcoeff: float32
    val mutable ycoeff: float32

[<Struct>]
type VkPipelineViewportWScalingStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable viewportWScalingEnable: VkBool32
    val mutable viewportCount: uint32
    val pViewportWScalings: nativeptr<VkViewportWScalingNV>

[<Struct>]
type VkViewportSwizzleNV =
    val mutable x: VkViewportCoordinateSwizzleNV
    val mutable y: VkViewportCoordinateSwizzleNV
    val mutable z: VkViewportCoordinateSwizzleNV
    val mutable w: VkViewportCoordinateSwizzleNV

[<Struct>]
type VkPipelineViewportSwizzleStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineViewportSwizzleStateCreateFlagsNV
    val mutable viewportCount: uint32
    val pViewportSwizzles: nativeptr<VkViewportSwizzleNV>

[<Struct>]
type VkPhysicalDeviceDiscardRectanglePropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// max number of active discard rectangles
    val mutable maxDiscardRectangles: uint32

[<Struct>]
type VkPipelineDiscardRectangleStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineDiscardRectangleStateCreateFlagsEXT
    val mutable discardRectangleMode: VkDiscardRectangleModeEXT
    val mutable discardRectangleCount: uint32
    val pDiscardRectangles: nativeptr<VkRect2D>

[<Struct>]
type VkPhysicalDeviceMultiviewPerViewAttributesPropertiesNVX =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable perViewPositionAllComponents: VkBool32

[<Struct>]
type VkInputAttachmentAspectReference =
    val mutable subpass: uint32
    val mutable inputAttachmentIndex: uint32
    val mutable aspectMask: VkImageAspectFlags

type VkInputAttachmentAspectReferenceKHR = VkInputAttachmentAspectReference

[<Struct>]
type VkRenderPassInputAttachmentAspectCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable aspectReferenceCount: uint32
    val pAspectReferences: nativeptr<VkInputAttachmentAspectReference>

type VkRenderPassInputAttachmentAspectCreateInfoKHR = VkRenderPassInputAttachmentAspectCreateInfo

[<Struct>]
type VkPhysicalDeviceSurfaceInfo2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable surface: VkSurfaceKHR

[<Struct>]
type VkSurfaceCapabilities2KHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable surfaceCapabilities: VkSurfaceCapabilitiesKHR

[<Struct>]
type VkSurfaceFormat2KHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable surfaceFormat: VkSurfaceFormatKHR

[<Struct>]
type VkDisplayProperties2KHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable displayProperties: VkDisplayPropertiesKHR

[<Struct>]
type VkDisplayPlaneProperties2KHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable displayPlaneProperties: VkDisplayPlanePropertiesKHR

[<Struct>]
type VkDisplayModeProperties2KHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable displayModeProperties: VkDisplayModePropertiesKHR

[<Struct>]
type VkDisplayPlaneInfo2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable mode: VkDisplayModeKHR
    val mutable planeIndex: uint32

[<Struct>]
type VkDisplayPlaneCapabilities2KHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable capabilities: VkDisplayPlaneCapabilitiesKHR

[<Struct>]
type VkSharedPresentSurfaceCapabilitiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// Supported image usage flags if swapchain created using a shared present mode
    val mutable sharedPresentSupportedUsageFlags: VkImageUsageFlags

[<Struct>]
type VkPhysicalDevice16BitStorageFeatures =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// 16-bit integer/floating-point variables supported in BufferBlock
    val mutable storageBuffer16BitAccess: VkBool32
    /// 16-bit integer/floating-point variables supported in BufferBlock and Block
    val mutable uniformAndStorageBuffer16BitAccess: VkBool32
    /// 16-bit integer/floating-point variables supported in PushConstant
    val mutable storagePushConstant16: VkBool32
    /// 16-bit integer/floating-point variables supported in shader inputs and outputs
    val mutable storageInputOutput16: VkBool32

type VkPhysicalDevice16BitStorageFeaturesKHR = VkPhysicalDevice16BitStorageFeatures

[<Struct>]
type VkPhysicalDeviceSubgroupProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// The size of a subgroup for this queue.
    val mutable subgroupSize: uint32
    /// Bitfield of what shader stages support subgroup operations
    val mutable supportedStages: VkShaderStageFlags
    /// Bitfield of what subgroup operations are supported.
    val mutable supportedOperations: VkSubgroupFeatureFlags
    /// Flag to specify whether quad operations are available in all stages.
    val mutable quadOperationsInAllStages: VkBool32

[<Struct>]
type VkBufferMemoryRequirementsInfo2 =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable buffer: VkBuffer

type VkBufferMemoryRequirementsInfo2KHR = VkBufferMemoryRequirementsInfo2

[<Struct>]
type VkImageMemoryRequirementsInfo2 =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable image: VkImage

type VkImageMemoryRequirementsInfo2KHR = VkImageMemoryRequirementsInfo2

[<Struct>]
type VkImageSparseMemoryRequirementsInfo2 =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable image: VkImage

type VkImageSparseMemoryRequirementsInfo2KHR = VkImageSparseMemoryRequirementsInfo2

[<Struct>]
type VkMemoryRequirements2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryRequirements: VkMemoryRequirements

type VkMemoryRequirements2KHR = VkMemoryRequirements2

[<Struct>]
type VkSparseImageMemoryRequirements2 =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryRequirements: VkSparseImageMemoryRequirements

type VkSparseImageMemoryRequirements2KHR = VkSparseImageMemoryRequirements2

[<Struct>]
type VkPhysicalDevicePointClippingProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable pointClippingBehavior: VkPointClippingBehavior

type VkPhysicalDevicePointClippingPropertiesKHR = VkPhysicalDevicePointClippingProperties

[<Struct>]
type VkMemoryDedicatedRequirements =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable prefersDedicatedAllocation: VkBool32
    val mutable requiresDedicatedAllocation: VkBool32

type VkMemoryDedicatedRequirementsKHR = VkMemoryDedicatedRequirements

[<Struct>]
type VkMemoryDedicatedAllocateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Image that this allocation will be bound to
    val mutable image: VkImage
    /// Buffer that this allocation will be bound to
    val mutable buffer: VkBuffer

type VkMemoryDedicatedAllocateInfoKHR = VkMemoryDedicatedAllocateInfo

[<Struct>]
type VkImageViewUsageCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable usage: VkImageUsageFlags

type VkImageViewUsageCreateInfoKHR = VkImageViewUsageCreateInfo

[<Struct>]
type VkPipelineTessellationDomainOriginStateCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable domainOrigin: VkTessellationDomainOrigin

type VkPipelineTessellationDomainOriginStateCreateInfoKHR = VkPipelineTessellationDomainOriginStateCreateInfo

[<Struct>]
type VkSamplerYcbcrConversionInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable conversion: VkSamplerYcbcrConversion

type VkSamplerYcbcrConversionInfoKHR = VkSamplerYcbcrConversionInfo

[<Struct>]
type VkSamplerYcbcrConversionCreateInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable format: VkFormat
    val mutable ycbcrModel: VkSamplerYcbcrModelConversion
    val mutable ycbcrRange: VkSamplerYcbcrRange
    val mutable components: VkComponentMapping
    val mutable xChromaOffset: VkChromaLocation
    val mutable yChromaOffset: VkChromaLocation
    val mutable chromaFilter: VkFilter
    val forceExplicitReconstruction: VkBool32

type VkSamplerYcbcrConversionCreateInfoKHR = VkSamplerYcbcrConversionCreateInfo

[<Struct>]
type VkBindImagePlaneMemoryInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable planeAspect: VkImageAspectFlagBits

type VkBindImagePlaneMemoryInfoKHR = VkBindImagePlaneMemoryInfo

[<Struct>]
type VkImagePlaneMemoryRequirementsInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable planeAspect: VkImageAspectFlagBits

type VkImagePlaneMemoryRequirementsInfoKHR = VkImagePlaneMemoryRequirementsInfo

[<Struct>]
type VkPhysicalDeviceSamplerYcbcrConversionFeatures =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// Sampler color conversion supported
    val mutable samplerYcbcrConversion: VkBool32

type VkPhysicalDeviceSamplerYcbcrConversionFeaturesKHR = VkPhysicalDeviceSamplerYcbcrConversionFeatures

[<Struct>]
type VkSamplerYcbcrConversionImageFormatProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable combinedImageSamplerDescriptorCount: uint32

type VkSamplerYcbcrConversionImageFormatPropertiesKHR = VkSamplerYcbcrConversionImageFormatProperties

[<Struct>]
type VkTextureLODGatherFormatPropertiesAMD =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable supportsTextureGatherLODBiasAMD: VkBool32

[<Struct>]
type VkConditionalRenderingBeginInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable buffer: VkBuffer
    val mutable offset: VkDeviceSize
    val mutable flags: VkConditionalRenderingFlagsEXT

[<Struct>]
type VkProtectedSubmitInfo =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Submit protected command buffers
    val mutable protectedSubmit: VkBool32

[<Struct>]
type VkPhysicalDeviceProtectedMemoryFeatures =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable protectedMemory: VkBool32

[<Struct>]
type VkPhysicalDeviceProtectedMemoryProperties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable protectedNoFault: VkBool32

[<Struct>]
type VkDeviceQueueInfo2 =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDeviceQueueCreateFlags
    val mutable queueFamilyIndex: uint32
    val mutable queueIndex: uint32

[<Struct>]
type VkPipelineCoverageToColorStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineCoverageToColorStateCreateFlagsNV
    val mutable coverageToColorEnable: VkBool32
    val mutable coverageToColorLocation: uint32

[<Struct>]
type VkPhysicalDeviceSamplerFilterMinmaxPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable filterMinmaxSingleComponentFormats: VkBool32
    val mutable filterMinmaxImageComponentMapping: VkBool32

[<Struct>]
type VkSampleLocationEXT =
    val mutable x: float32
    val mutable y: float32

[<Struct>]
type VkSampleLocationsInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable sampleLocationsPerPixel: VkSampleCountFlagBits
    val mutable sampleLocationGridSize: VkExtent2D
    val mutable sampleLocationsCount: uint32
    val pSampleLocations: nativeptr<VkSampleLocationEXT>

[<Struct>]
type VkAttachmentSampleLocationsEXT =
    val mutable attachmentIndex: uint32
    val mutable sampleLocationsInfo: VkSampleLocationsInfoEXT

[<Struct>]
type VkSubpassSampleLocationsEXT =
    val mutable subpassIndex: uint32
    val mutable sampleLocationsInfo: VkSampleLocationsInfoEXT

[<Struct>]
type VkRenderPassSampleLocationsBeginInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable attachmentInitialSampleLocationsCount: uint32
    val pAttachmentInitialSampleLocations: nativeptr<VkAttachmentSampleLocationsEXT>
    val mutable postSubpassSampleLocationsCount: uint32
    val pPostSubpassSampleLocations: nativeptr<VkSubpassSampleLocationsEXT>

[<Struct>]
type VkPipelineSampleLocationsStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable sampleLocationsEnable: VkBool32
    val mutable sampleLocationsInfo: VkSampleLocationsInfoEXT

[<Struct>]
type VkPhysicalDeviceSampleLocationsPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable sampleLocationSampleCounts: VkSampleCountFlags
    val mutable maxSampleLocationGridSize: VkExtent2D
    val mutable sampleLocationCoordinateRange: float32
    val mutable sampleLocationSubPixelBits: uint32
    val mutable variableSampleLocations: VkBool32

[<Struct>]
type VkMultisamplePropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxSampleLocationGridSize: VkExtent2D

[<Struct>]
type VkSamplerReductionModeCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable reductionMode: VkSamplerReductionModeEXT

[<Struct>]
type VkPhysicalDeviceBlendOperationAdvancedFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable advancedBlendCoherentOperations: VkBool32

[<Struct>]
type VkPhysicalDeviceBlendOperationAdvancedPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable advancedBlendMaxColorAttachments: uint32
    val mutable advancedBlendIndependentBlend: VkBool32
    val mutable advancedBlendNonPremultipliedSrcColor: VkBool32
    val mutable advancedBlendNonPremultipliedDstColor: VkBool32
    val mutable advancedBlendCorrelatedOverlap: VkBool32
    val mutable advancedBlendAllOperations: VkBool32

[<Struct>]
type VkPipelineColorBlendAdvancedStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable srcPremultiplied: VkBool32
    val mutable dstPremultiplied: VkBool32
    val mutable blendOverlap: VkBlendOverlapEXT

[<Struct>]
type VkPhysicalDeviceInlineUniformBlockFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable inlineUniformBlock: VkBool32
    val mutable descriptorBindingInlineUniformBlockUpdateAfterBind: VkBool32

[<Struct>]
type VkPhysicalDeviceInlineUniformBlockPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxInlineUniformBlockSize: uint32
    val mutable maxPerStageDescriptorInlineUniformBlocks: uint32
    val mutable maxPerStageDescriptorUpdateAfterBindInlineUniformBlocks: uint32
    val mutable maxDescriptorSetInlineUniformBlocks: uint32
    val mutable maxDescriptorSetUpdateAfterBindInlineUniformBlocks: uint32

[<Struct>]
type VkWriteDescriptorSetInlineUniformBlockEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable dataSize: uint32
    val pData: nativeint

[<Struct>]
type VkDescriptorPoolInlineUniformBlockCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable maxInlineUniformBlockBindings: uint32

[<Struct>]
type VkPipelineCoverageModulationStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineCoverageModulationStateCreateFlagsNV
    val mutable coverageModulationMode: VkCoverageModulationModeNV
    val mutable coverageModulationTableEnable: VkBool32
    val mutable coverageModulationTableCount: uint32
    val pCoverageModulationTable: nativeptr<float32>

[<Struct>]
type VkImageFormatListCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable viewFormatCount: uint32
    val pViewFormats: nativeptr<VkFormat>

[<Struct>]
type VkValidationCacheCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkValidationCacheCreateFlagsEXT
    val mutable initialDataSize: nativeint
    val pInitialData: nativeint

[<Struct>]
type VkShaderModuleValidationCacheCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable validationCache: VkValidationCacheEXT

[<Struct>]
type VkPhysicalDeviceMaintenance3Properties =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxPerSetDescriptors: uint32
    val mutable maxMemoryAllocationSize: VkDeviceSize

type VkPhysicalDeviceMaintenance3PropertiesKHR = VkPhysicalDeviceMaintenance3Properties

[<Struct>]
type VkDescriptorSetLayoutSupport =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable supported: VkBool32

type VkDescriptorSetLayoutSupportKHR = VkDescriptorSetLayoutSupport

[<Struct>]
type VkPhysicalDeviceShaderDrawParametersFeatures =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderDrawParameters: VkBool32

type VkPhysicalDeviceShaderDrawParameterFeatures = VkPhysicalDeviceShaderDrawParametersFeatures

[<Struct>]
type VkPhysicalDeviceShaderFloat16Int8FeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderFloat16: VkBool32
    val mutable shaderInt8: VkBool32

type VkPhysicalDeviceFloat16Int8FeaturesKHR = VkPhysicalDeviceShaderFloat16Int8FeaturesKHR

[<Struct>]
type VkPhysicalDeviceFloatControlsPropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable denormBehaviorIndependence: VkShaderFloatControlsIndependenceKHR
    val mutable roundingModeIndependence: VkShaderFloatControlsIndependenceKHR
    val mutable shaderSignedZeroInfNanPreserveFloat16: VkBool32
    val mutable shaderSignedZeroInfNanPreserveFloat32: VkBool32
    val mutable shaderSignedZeroInfNanPreserveFloat64: VkBool32
    val mutable shaderDenormPreserveFloat16: VkBool32
    val mutable shaderDenormPreserveFloat32: VkBool32
    val mutable shaderDenormPreserveFloat64: VkBool32
    val mutable shaderDenormFlushToZeroFloat16: VkBool32
    val mutable shaderDenormFlushToZeroFloat32: VkBool32
    val mutable shaderDenormFlushToZeroFloat64: VkBool32
    val mutable shaderRoundingModeRTEFloat16: VkBool32
    val mutable shaderRoundingModeRTEFloat32: VkBool32
    val mutable shaderRoundingModeRTEFloat64: VkBool32
    val mutable shaderRoundingModeRTZFloat16: VkBool32
    val mutable shaderRoundingModeRTZFloat32: VkBool32
    val mutable shaderRoundingModeRTZFloat64: VkBool32

[<Struct>]
type VkPhysicalDeviceHostQueryResetFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable hostQueryReset: VkBool32

[<Struct>]
type VkNativeBufferUsage2ANDROID =
    val mutable consumer: uint64
    val mutable producer: uint64

[<Struct>]
type VkNativeBufferANDROID =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val handle: nativeint
    val mutable stride: int
    val mutable format: int
    val mutable usage: int
    val mutable usage2: VkNativeBufferUsage2ANDROID

[<Struct>]
type VkSwapchainImageCreateInfoANDROID =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable usage: VkSwapchainImageUsageFlagsANDROID

[<Struct>]
type VkPhysicalDevicePresentationPropertiesANDROID =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable sharedImage: VkBool32

[<Struct>]
type VkShaderResourceUsageAMD =
    val mutable numUsedVgprs: uint32
    val mutable numUsedSgprs: uint32
    val mutable ldsSizePerLocalWorkGroup: uint32
    val mutable ldsUsageSizeInBytes: nativeint
    val mutable scratchMemUsageInBytes: nativeint

[<Struct>]
type VkShaderStatisticsInfoAMD =
    val mutable shaderStageMask: VkShaderStageFlags
    val mutable resourceUsage: VkShaderResourceUsageAMD
    val mutable numPhysicalVgprs: uint32
    val mutable numPhysicalSgprs: uint32
    val mutable numAvailableVgprs: uint32
    val mutable numAvailableSgprs: uint32
    val mutable computeWorkGroupSize: uint32

[<Struct>]
type VkDeviceQueueGlobalPriorityCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable globalPriority: VkQueueGlobalPriorityEXT

[<Struct>]
type VkDebugUtilsObjectNameInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable objectType: VkObjectType
    val mutable objectHandle: uint64
    val pObjectName: nativeptr<char>

[<Struct>]
type VkDebugUtilsObjectTagInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable objectType: VkObjectType
    val mutable objectHandle: uint64
    val mutable tagName: uint64
    val mutable tagSize: nativeint
    val pTag: nativeint

[<Struct>]
type VkDebugUtilsLabelEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val pLabelName: nativeptr<char>
    val mutable color: float32

[<Struct>]
type VkDebugUtilsMessengerCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDebugUtilsMessengerCreateFlagsEXT
    val mutable messageSeverity: VkDebugUtilsMessageSeverityFlagsEXT
    val mutable messageType: VkDebugUtilsMessageTypeFlagsEXT
    val mutable pfnUserCallback: PFN_vkDebugUtilsMessengerCallbackEXT
    val mutable pUserData: nativeint

[<Struct>]
type VkDebugUtilsMessengerCallbackDataEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkDebugUtilsMessengerCallbackDataFlagsEXT
    val pMessageIdName: nativeptr<char>
    val mutable messageIdNumber: int
    val pMessage: nativeptr<char>
    val mutable queueLabelCount: uint32
    val pQueueLabels: nativeptr<VkDebugUtilsLabelEXT>
    val mutable cmdBufLabelCount: uint32
    val pCmdBufLabels: nativeptr<VkDebugUtilsLabelEXT>
    val mutable objectCount: uint32
    val pObjects: nativeptr<VkDebugUtilsObjectNameInfoEXT>

[<Struct>]
type VkImportMemoryHostPointerInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable handleType: VkExternalMemoryHandleTypeFlagBits
    val mutable pHostPointer: nativeint

[<Struct>]
type VkMemoryHostPointerPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryTypeBits: uint32

[<Struct>]
type VkPhysicalDeviceExternalMemoryHostPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable minImportedHostPointerAlignment: VkDeviceSize

[<Struct>]
type VkPhysicalDeviceConservativeRasterizationPropertiesEXT =
    val mutable sType: VkStructureType
    /// Pointer to next structure
    val mutable pNext: nativeint
    /// The size in pixels the primitive is enlarged at each edge during conservative rasterization
    val mutable primitiveOverestimationSize: float32
    /// The maximum additional overestimation the client can specify in the pipeline state
    val mutable maxExtraPrimitiveOverestimationSize: float32
    /// The granularity of extra overestimation sizes the implementations supports between 0 and maxExtraOverestimationSize
    val mutable extraPrimitiveOverestimationSizeGranularity: float32
    /// true if the implementation supports conservative rasterization underestimation mode
    val mutable primitiveUnderestimation: VkBool32
    /// true if conservative rasterization also applies to points and lines
    val mutable conservativePointAndLineRasterization: VkBool32
    /// true if degenerate triangles (those with zero area after snap) are rasterized
    val mutable degenerateTrianglesRasterized: VkBool32
    /// true if degenerate lines (those with zero length after snap) are rasterized
    val mutable degenerateLinesRasterized: VkBool32
    /// true if the implementation supports the FullyCoveredEXT SPIR-V builtin fragment shader input variable
    val mutable fullyCoveredFragmentShaderInputVariable: VkBool32
    /// true if the implementation supports both conservative rasterization and post depth coverage sample coverage mask
    val mutable conservativeRasterizationPostDepthCoverage: VkBool32

[<Struct>]
type VkCalibratedTimestampInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable timeDomain: VkTimeDomainEXT

[<Struct>]
type VkPhysicalDeviceShaderCorePropertiesAMD =
    val mutable sType: VkStructureType
    /// Pointer to next structure
    val mutable pNext: nativeint
    /// number of shader engines
    val mutable shaderEngineCount: uint32
    /// number of shader arrays
    val mutable shaderArraysPerEngineCount: uint32
    /// number of physical CUs per shader array
    val mutable computeUnitsPerShaderArray: uint32
    /// number of SIMDs per compute unit
    val mutable simdPerComputeUnit: uint32
    /// number of wavefront slots in each SIMD
    val mutable wavefrontsPerSimd: uint32
    /// maximum number of threads per wavefront
    val mutable wavefrontSize: uint32
    /// number of physical SGPRs per SIMD
    val mutable sgprsPerSimd: uint32
    /// minimum number of SGPRs that can be allocated by a wave
    val mutable minSgprAllocation: uint32
    /// number of available SGPRs
    val mutable maxSgprAllocation: uint32
    /// SGPRs are allocated in groups of this size
    val mutable sgprAllocationGranularity: uint32
    /// number of physical VGPRs per SIMD
    val mutable vgprsPerSimd: uint32
    /// minimum number of VGPRs that can be allocated by a wave
    val mutable minVgprAllocation: uint32
    /// number of available VGPRs
    val mutable maxVgprAllocation: uint32
    /// VGPRs are allocated in groups of this size
    val mutable vgprAllocationGranularity: uint32

[<Struct>]
type VkPhysicalDeviceShaderCoreProperties2AMD =
    val mutable sType: VkStructureType
    /// Pointer to next structure
    val mutable pNext: nativeint
    /// features supported by the shader core
    val mutable shaderCoreFeatures: VkShaderCorePropertiesFlagsAMD
    /// number of active compute units across all shader engines/arrays
    val mutable activeComputeUnitCount: uint32

[<Struct>]
type VkPipelineRasterizationConservativeStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineRasterizationConservativeStateCreateFlagsEXT
    val mutable conservativeRasterizationMode: VkConservativeRasterizationModeEXT
    val mutable extraPrimitiveOverestimationSize: float32

[<Struct>]
type VkPhysicalDeviceDescriptorIndexingFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderInputAttachmentArrayDynamicIndexing: VkBool32
    val mutable shaderUniformTexelBufferArrayDynamicIndexing: VkBool32
    val mutable shaderStorageTexelBufferArrayDynamicIndexing: VkBool32
    val mutable shaderUniformBufferArrayNonUniformIndexing: VkBool32
    val mutable shaderSampledImageArrayNonUniformIndexing: VkBool32
    val mutable shaderStorageBufferArrayNonUniformIndexing: VkBool32
    val mutable shaderStorageImageArrayNonUniformIndexing: VkBool32
    val mutable shaderInputAttachmentArrayNonUniformIndexing: VkBool32
    val mutable shaderUniformTexelBufferArrayNonUniformIndexing: VkBool32
    val mutable shaderStorageTexelBufferArrayNonUniformIndexing: VkBool32
    val mutable descriptorBindingUniformBufferUpdateAfterBind: VkBool32
    val mutable descriptorBindingSampledImageUpdateAfterBind: VkBool32
    val mutable descriptorBindingStorageImageUpdateAfterBind: VkBool32
    val mutable descriptorBindingStorageBufferUpdateAfterBind: VkBool32
    val mutable descriptorBindingUniformTexelBufferUpdateAfterBind: VkBool32
    val mutable descriptorBindingStorageTexelBufferUpdateAfterBind: VkBool32
    val mutable descriptorBindingUpdateUnusedWhilePending: VkBool32
    val mutable descriptorBindingPartiallyBound: VkBool32
    val mutable descriptorBindingVariableDescriptorCount: VkBool32
    val mutable runtimeDescriptorArray: VkBool32

[<Struct>]
type VkPhysicalDeviceDescriptorIndexingPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxUpdateAfterBindDescriptorsInAllPools: uint32
    val mutable shaderUniformBufferArrayNonUniformIndexingNative: VkBool32
    val mutable shaderSampledImageArrayNonUniformIndexingNative: VkBool32
    val mutable shaderStorageBufferArrayNonUniformIndexingNative: VkBool32
    val mutable shaderStorageImageArrayNonUniformIndexingNative: VkBool32
    val mutable shaderInputAttachmentArrayNonUniformIndexingNative: VkBool32
    val mutable robustBufferAccessUpdateAfterBind: VkBool32
    val mutable quadDivergentImplicitLod: VkBool32
    val mutable maxPerStageDescriptorUpdateAfterBindSamplers: uint32
    val mutable maxPerStageDescriptorUpdateAfterBindUniformBuffers: uint32
    val mutable maxPerStageDescriptorUpdateAfterBindStorageBuffers: uint32
    val mutable maxPerStageDescriptorUpdateAfterBindSampledImages: uint32
    val mutable maxPerStageDescriptorUpdateAfterBindStorageImages: uint32
    val mutable maxPerStageDescriptorUpdateAfterBindInputAttachments: uint32
    val mutable maxPerStageUpdateAfterBindResources: uint32
    val mutable maxDescriptorSetUpdateAfterBindSamplers: uint32
    val mutable maxDescriptorSetUpdateAfterBindUniformBuffers: uint32
    val mutable maxDescriptorSetUpdateAfterBindUniformBuffersDynamic: uint32
    val mutable maxDescriptorSetUpdateAfterBindStorageBuffers: uint32
    val mutable maxDescriptorSetUpdateAfterBindStorageBuffersDynamic: uint32
    val mutable maxDescriptorSetUpdateAfterBindSampledImages: uint32
    val mutable maxDescriptorSetUpdateAfterBindStorageImages: uint32
    val mutable maxDescriptorSetUpdateAfterBindInputAttachments: uint32

[<Struct>]
type VkDescriptorSetLayoutBindingFlagsCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable bindingCount: uint32
    val pBindingFlags: nativeptr<VkDescriptorBindingFlagsEXT>

[<Struct>]
type VkDescriptorSetVariableDescriptorCountAllocateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable descriptorSetCount: uint32
    val pDescriptorCounts: nativeptr<uint32>

[<Struct>]
type VkDescriptorSetVariableDescriptorCountLayoutSupportEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxVariableDescriptorCount: uint32

[<Struct>]
type VkAttachmentDescription2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkAttachmentDescriptionFlags
    val mutable format: VkFormat
    val mutable samples: VkSampleCountFlagBits
    /// Load operation for color or depth data
    val mutable loadOp: VkAttachmentLoadOp
    /// Store operation for color or depth data
    val mutable storeOp: VkAttachmentStoreOp
    /// Load operation for stencil data
    val mutable stencilLoadOp: VkAttachmentLoadOp
    /// Store operation for stencil data
    val mutable stencilStoreOp: VkAttachmentStoreOp
    val mutable initialLayout: VkImageLayout
    val mutable finalLayout: VkImageLayout

[<Struct>]
type VkAttachmentReference2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable attachment: uint32
    val mutable layout: VkImageLayout
    val mutable aspectMask: VkImageAspectFlags

[<Struct>]
type VkSubpassDescription2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkSubpassDescriptionFlags
    val mutable pipelineBindPoint: VkPipelineBindPoint
    val mutable viewMask: uint32
    val mutable inputAttachmentCount: uint32
    val pInputAttachments: nativeptr<VkAttachmentReference2KHR>
    val mutable colorAttachmentCount: uint32
    val pColorAttachments: nativeptr<VkAttachmentReference2KHR>
    val pResolveAttachments: nativeptr<VkAttachmentReference2KHR>
    val pDepthStencilAttachment: nativeptr<VkAttachmentReference2KHR>
    val mutable preserveAttachmentCount: uint32
    val pPreserveAttachments: nativeptr<uint32>

[<Struct>]
type VkSubpassDependency2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable srcSubpass: uint32
    val mutable dstSubpass: uint32
    val mutable srcStageMask: VkPipelineStageFlags
    val mutable dstStageMask: VkPipelineStageFlags
    val mutable srcAccessMask: VkAccessFlags
    val mutable dstAccessMask: VkAccessFlags
    val mutable dependencyFlags: VkDependencyFlags
    val mutable viewOffset: int

[<Struct>]
type VkRenderPassCreateInfo2KHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkRenderPassCreateFlags
    val mutable attachmentCount: uint32
    val pAttachments: nativeptr<VkAttachmentDescription2KHR>
    val mutable subpassCount: uint32
    val pSubpasses: nativeptr<VkSubpassDescription2KHR>
    val mutable dependencyCount: uint32
    val pDependencies: nativeptr<VkSubpassDependency2KHR>
    val mutable correlatedViewMaskCount: uint32
    val pCorrelatedViewMasks: nativeptr<uint32>

[<Struct>]
type VkSubpassBeginInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable contents: VkSubpassContents

[<Struct>]
type VkSubpassEndInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint

[<Struct>]
type VkVertexInputBindingDivisorDescriptionEXT =
    val mutable binding: uint32
    val mutable divisor: uint32

[<Struct>]
type VkPipelineVertexInputDivisorStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable vertexBindingDivisorCount: uint32
    val pVertexBindingDivisors: nativeptr<VkVertexInputBindingDivisorDescriptionEXT>

[<Struct>]
type VkPhysicalDeviceVertexAttributeDivisorPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// max value of vertex attribute divisor
    val mutable maxVertexAttribDivisor: uint32

[<Struct>]
type VkPhysicalDevicePCIBusInfoPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable pciDomain: uint32
    val mutable pciBus: uint32
    val mutable pciDevice: uint32
    val mutable pciFunction: uint32

[<Struct>]
type VkImportAndroidHardwareBufferInfoANDROID =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable buffer: nativeptr<AHardwareBuffer>

[<Struct>]
type VkAndroidHardwareBufferUsageANDROID =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable androidHardwareBufferUsage: uint64

[<Struct>]
type VkAndroidHardwareBufferPropertiesANDROID =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable allocationSize: VkDeviceSize
    val mutable memoryTypeBits: uint32

[<Struct>]
type VkMemoryGetAndroidHardwareBufferInfoANDROID =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable memory: VkDeviceMemory

[<Struct>]
type VkAndroidHardwareBufferFormatPropertiesANDROID =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable format: VkFormat
    val mutable externalFormat: uint64
    val mutable formatFeatures: VkFormatFeatureFlags
    val mutable samplerYcbcrConversionComponents: VkComponentMapping
    val mutable suggestedYcbcrModel: VkSamplerYcbcrModelConversion
    val mutable suggestedYcbcrRange: VkSamplerYcbcrRange
    val mutable suggestedXChromaOffset: VkChromaLocation
    val mutable suggestedYChromaOffset: VkChromaLocation

[<Struct>]
type VkCommandBufferInheritanceConditionalRenderingInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Whether this secondary command buffer may be executed during an active conditional rendering
    val mutable conditionalRenderingEnable: VkBool32

[<Struct>]
type VkExternalFormatANDROID =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable externalFormat: uint64

[<Struct>]
type VkPhysicalDevice8BitStorageFeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// 8-bit integer variables supported in StorageBuffer
    val mutable storageBuffer8BitAccess: VkBool32
    /// 8-bit integer variables supported in StorageBuffer and Uniform
    val mutable uniformAndStorageBuffer8BitAccess: VkBool32
    /// 8-bit integer variables supported in PushConstant
    val mutable storagePushConstant8: VkBool32

[<Struct>]
type VkPhysicalDeviceConditionalRenderingFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable conditionalRendering: VkBool32
    val mutable inheritedConditionalRendering: VkBool32

[<Struct>]
type VkPhysicalDeviceVulkanMemoryModelFeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable vulkanMemoryModel: VkBool32
    val mutable vulkanMemoryModelDeviceScope: VkBool32
    val mutable vulkanMemoryModelAvailabilityVisibilityChains: VkBool32

[<Struct>]
type VkPhysicalDeviceShaderAtomicInt64FeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderBufferInt64Atomics: VkBool32
    val mutable shaderSharedInt64Atomics: VkBool32

[<Struct>]
type VkPhysicalDeviceVertexAttributeDivisorFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable vertexAttributeInstanceRateDivisor: VkBool32
    val mutable vertexAttributeInstanceRateZeroDivisor: VkBool32

[<Struct>]
type VkQueueFamilyCheckpointPropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable checkpointExecutionStageMask: VkPipelineStageFlags

[<Struct>]
type VkCheckpointDataNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable stage: VkPipelineStageFlagBits
    val mutable pCheckpointMarker: nativeint

[<Struct>]
type VkPhysicalDeviceDepthStencilResolvePropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// supported depth resolve modes
    val mutable supportedDepthResolveModes: VkResolveModeFlagsKHR
    /// supported stencil resolve modes
    val mutable supportedStencilResolveModes: VkResolveModeFlagsKHR
    /// depth and stencil resolve modes can be set independently if one of them is none
    val mutable independentResolveNone: VkBool32
    /// depth and stencil resolve modes can be set independently
    val mutable independentResolve: VkBool32

[<Struct>]
type VkSubpassDescriptionDepthStencilResolveKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// depth resolve mode
    val mutable depthResolveMode: VkResolveModeFlagBitsKHR
    /// stencil resolve mode
    val mutable stencilResolveMode: VkResolveModeFlagBitsKHR
    /// depth/stencil resolve attachment
    val pDepthStencilResolveAttachment: nativeptr<VkAttachmentReference2KHR>

[<Struct>]
type VkImageViewASTCDecodeModeEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable decodeMode: VkFormat

[<Struct>]
type VkPhysicalDeviceASTCDecodeFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable decodeModeSharedExponent: VkBool32

[<Struct>]
type VkPhysicalDeviceTransformFeedbackFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable transformFeedback: VkBool32
    val mutable geometryStreams: VkBool32

[<Struct>]
type VkPhysicalDeviceTransformFeedbackPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxTransformFeedbackStreams: uint32
    val mutable maxTransformFeedbackBuffers: uint32
    val mutable maxTransformFeedbackBufferSize: VkDeviceSize
    val mutable maxTransformFeedbackStreamDataSize: uint32
    val mutable maxTransformFeedbackBufferDataSize: uint32
    val mutable maxTransformFeedbackBufferDataStride: uint32
    val mutable transformFeedbackQueries: VkBool32
    val mutable transformFeedbackStreamsLinesTriangles: VkBool32
    val mutable transformFeedbackRasterizationStreamSelect: VkBool32
    val mutable transformFeedbackDraw: VkBool32

[<Struct>]
type VkPipelineRasterizationStateStreamCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineRasterizationStateStreamCreateFlagsEXT
    val mutable rasterizationStream: uint32

[<Struct>]
type VkPhysicalDeviceRepresentativeFragmentTestFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable representativeFragmentTest: VkBool32

[<Struct>]
type VkPipelineRepresentativeFragmentTestStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable representativeFragmentTestEnable: VkBool32

[<Struct>]
type VkPhysicalDeviceExclusiveScissorFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable exclusiveScissor: VkBool32

[<Struct>]
type VkPipelineViewportExclusiveScissorStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable exclusiveScissorCount: uint32
    val pExclusiveScissors: nativeptr<VkRect2D>

[<Struct>]
type VkPhysicalDeviceCornerSampledImageFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable cornerSampledImage: VkBool32

[<Struct>]
type VkPhysicalDeviceComputeShaderDerivativesFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable computeDerivativeGroupQuads: VkBool32
    val mutable computeDerivativeGroupLinear: VkBool32

[<Struct>]
type VkPhysicalDeviceFragmentShaderBarycentricFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable fragmentShaderBarycentric: VkBool32

[<Struct>]
type VkPhysicalDeviceShaderImageFootprintFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable imageFootprint: VkBool32

[<Struct>]
type VkPhysicalDeviceDedicatedAllocationImageAliasingFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable dedicatedAllocationImageAliasing: VkBool32

[<Struct>]
type VkShadingRatePaletteNV =
    val mutable shadingRatePaletteEntryCount: uint32
    val pShadingRatePaletteEntries: nativeptr<VkShadingRatePaletteEntryNV>

[<Struct>]
type VkPipelineViewportShadingRateImageStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable shadingRateImageEnable: VkBool32
    val mutable viewportCount: uint32
    val pShadingRatePalettes: nativeptr<VkShadingRatePaletteNV>

[<Struct>]
type VkPhysicalDeviceShadingRateImageFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shadingRateImage: VkBool32
    val mutable shadingRateCoarseSampleOrder: VkBool32

[<Struct>]
type VkPhysicalDeviceShadingRateImagePropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shadingRateTexelSize: VkExtent2D
    val mutable shadingRatePaletteSize: uint32
    val mutable shadingRateMaxCoarseSamples: uint32

[<Struct>]
type VkCoarseSampleLocationNV =
    val mutable pixelX: uint32
    val mutable pixelY: uint32
    val mutable sample: uint32

[<Struct>]
type VkCoarseSampleOrderCustomNV =
    val mutable shadingRate: VkShadingRatePaletteEntryNV
    val mutable sampleCount: uint32
    val mutable sampleLocationCount: uint32
    val pSampleLocations: nativeptr<VkCoarseSampleLocationNV>

[<Struct>]
type VkPipelineViewportCoarseSampleOrderStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable sampleOrderType: VkCoarseSampleOrderTypeNV
    val mutable customSampleOrderCount: uint32
    val pCustomSampleOrders: nativeptr<VkCoarseSampleOrderCustomNV>

[<Struct>]
type VkPhysicalDeviceMeshShaderFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable taskShader: VkBool32
    val mutable meshShader: VkBool32

[<Struct>]
type VkPhysicalDeviceMeshShaderPropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable maxDrawMeshTasksCount: uint32
    val mutable maxTaskWorkGroupInvocations: uint32
    val mutable maxTaskWorkGroupSize: uint32
    val mutable maxTaskTotalMemorySize: uint32
    val mutable maxTaskOutputCount: uint32
    val mutable maxMeshWorkGroupInvocations: uint32
    val mutable maxMeshWorkGroupSize: uint32
    val mutable maxMeshTotalMemorySize: uint32
    val mutable maxMeshOutputVertices: uint32
    val mutable maxMeshOutputPrimitives: uint32
    val mutable maxMeshMultiviewViewCount: uint32
    val mutable meshOutputPerVertexGranularity: uint32
    val mutable meshOutputPerPrimitiveGranularity: uint32

[<Struct>]
type VkDrawMeshTasksIndirectCommandNV =
    val mutable taskCount: uint32
    val mutable firstTask: uint32

[<Struct>]
type VkRayTracingShaderGroupCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable typ: VkRayTracingShaderGroupTypeNV
    val mutable generalShader: uint32
    val mutable closestHitShader: uint32
    val mutable anyHitShader: uint32
    val mutable intersectionShader: uint32

[<Struct>]
type VkRayTracingPipelineCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Pipeline creation flags
    val mutable flags: VkPipelineCreateFlags
    val mutable stageCount: uint32
    /// One entry for each active shader stage
    val pStages: nativeptr<VkPipelineShaderStageCreateInfo>
    val mutable groupCount: uint32
    val pGroups: nativeptr<VkRayTracingShaderGroupCreateInfoNV>
    val mutable maxRecursionDepth: uint32
    /// Interface layout of the pipeline
    val mutable layout: VkPipelineLayout
    /// If VK_PIPELINE_CREATE_DERIVATIVE_BIT is set and this value is nonzero, it specifies the handle of the base pipeline this is a derivative of
    val mutable basePipelineHandle: VkPipeline
    /// If VK_PIPELINE_CREATE_DERIVATIVE_BIT is set and this value is not -1, it specifies an index into pCreateInfos of the base pipeline this is a derivative of
    val mutable basePipelineIndex: int

[<Struct>]
type VkGeometryTrianglesNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable vertexData: VkBuffer
    val mutable vertexOffset: VkDeviceSize
    val mutable vertexCount: uint32
    val mutable vertexStride: VkDeviceSize
    val mutable vertexFormat: VkFormat
    val mutable indexData: VkBuffer
    val mutable indexOffset: VkDeviceSize
    val mutable indexCount: uint32
    val mutable indexType: VkIndexType
    /// Optional reference to array of floats representing a 3x4 row major affine transformation matrix.
    val mutable transformData: VkBuffer
    val mutable transformOffset: VkDeviceSize

[<Struct>]
type VkGeometryAABBNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable aabbData: VkBuffer
    val mutable numAABBs: uint32
    /// Stride in bytes between AABBs
    val mutable stride: uint32
    /// Offset in bytes of the first AABB in aabbData
    val mutable offset: VkDeviceSize

[<Struct>]
type VkGeometryDataNV =
    val mutable triangles: VkGeometryTrianglesNV
    val mutable aabbs: VkGeometryAABBNV

[<Struct>]
type VkGeometryNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable geometryType: VkGeometryTypeNV
    val mutable geometry: VkGeometryDataNV
    val mutable flags: VkGeometryFlagsNV

[<Struct>]
type VkAccelerationStructureInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable typ: VkAccelerationStructureTypeNV
    val mutable flags: VkBuildAccelerationStructureFlagsNV
    val mutable instanceCount: uint32
    val mutable geometryCount: uint32
    val pGeometries: nativeptr<VkGeometryNV>

[<Struct>]
type VkAccelerationStructureCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable compactedSize: VkDeviceSize
    val mutable info: VkAccelerationStructureInfoNV

[<Struct>]
type VkBindAccelerationStructureMemoryInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable accelerationStructure: VkAccelerationStructureNV
    val mutable memory: VkDeviceMemory
    val mutable memoryOffset: VkDeviceSize
    val mutable deviceIndexCount: uint32
    val pDeviceIndices: nativeptr<uint32>

[<Struct>]
type VkWriteDescriptorSetAccelerationStructureNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable accelerationStructureCount: uint32
    val pAccelerationStructures: nativeptr<VkAccelerationStructureNV>

[<Struct>]
type VkAccelerationStructureMemoryRequirementsInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable typ: VkAccelerationStructureMemoryRequirementsTypeNV
    val mutable accelerationStructure: VkAccelerationStructureNV

[<Struct>]
type VkPhysicalDeviceRayTracingPropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderGroupHandleSize: uint32
    val mutable maxRecursionDepth: uint32
    val mutable maxShaderGroupStride: uint32
    val mutable shaderGroupBaseAlignment: uint32
    val mutable maxGeometryCount: uint64
    val mutable maxInstanceCount: uint64
    val mutable maxTriangleCount: uint64
    val mutable maxDescriptorSetAccelerationStructures: uint32

[<Struct>]
type VkDrmFormatModifierPropertiesListEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable drmFormatModifierCount: uint32
    val mutable pDrmFormatModifierProperties: nativeptr<VkDrmFormatModifierPropertiesEXT>

[<Struct>]
type VkDrmFormatModifierPropertiesEXT =
    val mutable drmFormatModifier: uint64
    val mutable drmFormatModifierPlaneCount: uint32
    val mutable drmFormatModifierTilingFeatures: VkFormatFeatureFlags

[<Struct>]
type VkPhysicalDeviceImageDrmFormatModifierInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable drmFormatModifier: uint64
    val mutable sharingMode: VkSharingMode
    val mutable queueFamilyIndexCount: uint32
    val pQueueFamilyIndices: nativeptr<uint32>

[<Struct>]
type VkImageDrmFormatModifierListCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable drmFormatModifierCount: uint32
    val pDrmFormatModifiers: nativeptr<uint64>

[<Struct>]
type VkImageDrmFormatModifierExplicitCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable drmFormatModifier: uint64
    val mutable drmFormatModifierPlaneCount: uint32
    val pPlaneLayouts: nativeptr<VkSubresourceLayout>

[<Struct>]
type VkImageDrmFormatModifierPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable drmFormatModifier: uint64

[<Struct>]
type VkImageStencilUsageCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable stencilUsage: VkImageUsageFlags

[<Struct>]
type VkDeviceMemoryOverallocationCreateInfoAMD =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable overallocationBehavior: VkMemoryOverallocationBehaviorAMD

[<Struct>]
type VkPhysicalDeviceFragmentDensityMapFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable fragmentDensityMap: VkBool32
    val mutable fragmentDensityMapDynamic: VkBool32
    val mutable fragmentDensityMapNonSubsampledImages: VkBool32

[<Struct>]
type VkPhysicalDeviceFragmentDensityMapPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable minFragmentDensityTexelSize: VkExtent2D
    val mutable maxFragmentDensityTexelSize: VkExtent2D
    val mutable fragmentDensityInvocations: VkBool32

[<Struct>]
type VkRenderPassFragmentDensityMapCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable fragmentDensityMapAttachment: VkAttachmentReference

[<Struct>]
type VkPhysicalDeviceScalarBlockLayoutFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable scalarBlockLayout: VkBool32

[<Struct>]
type VkSurfaceProtectedCapabilitiesKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Represents if surface can be protected
    val mutable supportsProtected: VkBool32

[<Struct>]
type VkPhysicalDeviceUniformBufferStandardLayoutFeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable uniformBufferStandardLayout: VkBool32

[<Struct>]
type VkPhysicalDeviceDepthClipEnableFeaturesEXT =
    val mutable sType: VkStructureType
    /// Pointer to next structure
    val mutable pNext: nativeint
    val mutable depthClipEnable: VkBool32

[<Struct>]
type VkPipelineRasterizationDepthClipStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineRasterizationDepthClipStateCreateFlagsEXT
    val mutable depthClipEnable: VkBool32

[<Struct>]
type VkPhysicalDeviceMemoryBudgetPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable heapBudget: VkDeviceSize
    val mutable heapUsage: VkDeviceSize

[<Struct>]
type VkPhysicalDeviceMemoryPriorityFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable memoryPriority: VkBool32

[<Struct>]
type VkMemoryPriorityAllocateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable priority: float32

[<Struct>]
type VkPhysicalDeviceBufferDeviceAddressFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable bufferDeviceAddress: VkBool32
    val mutable bufferDeviceAddressCaptureReplay: VkBool32
    val mutable bufferDeviceAddressMultiDevice: VkBool32

type VkPhysicalDeviceBufferAddressFeaturesEXT = VkPhysicalDeviceBufferDeviceAddressFeaturesEXT

[<Struct>]
type VkBufferDeviceAddressInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable buffer: VkBuffer

[<Struct>]
type VkBufferDeviceAddressCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable deviceAddress: VkDeviceAddress

[<Struct>]
type VkPhysicalDeviceImageViewImageFormatInfoEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable imageViewType: VkImageViewType

[<Struct>]
type VkFilterCubicImageViewImageFormatPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable filterCubic: VkBool32
    val mutable filterCubicMinmax: VkBool32

[<Struct>]
type VkPhysicalDeviceImagelessFramebufferFeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable imagelessFramebuffer: VkBool32

[<Struct>]
type VkFramebufferAttachmentsCreateInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable attachmentImageInfoCount: uint32
    val pAttachmentImageInfos: nativeptr<VkFramebufferAttachmentImageInfoKHR>

[<Struct>]
type VkFramebufferAttachmentImageInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Image creation flags
    val mutable flags: VkImageCreateFlags
    /// Image usage flags
    val mutable usage: VkImageUsageFlags
    val mutable width: uint32
    val mutable height: uint32
    val mutable layerCount: uint32
    val mutable viewFormatCount: uint32
    val pViewFormats: nativeptr<VkFormat>

[<Struct>]
type VkRenderPassAttachmentBeginInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable attachmentCount: uint32
    val pAttachments: nativeptr<VkImageView>

[<Struct>]
type VkPhysicalDeviceTextureCompressionASTCHDRFeaturesEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable textureCompressionASTC_HDR: VkBool32

[<Struct>]
type VkPhysicalDeviceCooperativeMatrixFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable cooperativeMatrix: VkBool32
    val mutable cooperativeMatrixRobustBufferAccess: VkBool32

[<Struct>]
type VkPhysicalDeviceCooperativeMatrixPropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable cooperativeMatrixSupportedStages: VkShaderStageFlags

[<Struct>]
type VkCooperativeMatrixPropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable MSize: uint32
    val mutable NSize: uint32
    val mutable KSize: uint32
    val mutable AType: VkComponentTypeNV
    val mutable BType: VkComponentTypeNV
    val mutable CType: VkComponentTypeNV
    val mutable DType: VkComponentTypeNV
    val mutable scope: VkScopeNV

[<Struct>]
type VkPhysicalDeviceYcbcrImageArraysFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable ycbcrImageArrays: VkBool32

[<Struct>]
type VkImageViewHandleInfoNVX =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable imageView: VkImageView
    val mutable descriptorType: VkDescriptorType
    val mutable sampler: VkSampler

[<Struct>]
type VkPresentFrameTokenGGP =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable frameToken: GgpFrameToken

[<Struct>]
type VkPipelineCreationFeedbackEXT =
    val mutable flags: VkPipelineCreationFeedbackFlagsEXT
    val mutable duration: uint64

[<Struct>]
type VkPipelineCreationFeedbackCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    /// Output pipeline creation feedback.
    val mutable pPipelineCreationFeedback: nativeptr<VkPipelineCreationFeedbackEXT>
    val mutable pipelineStageCreationFeedbackCount: uint32
    /// One entry for each shader stage specified in the parent Vk*PipelineCreateInfo struct
    val mutable pPipelineStageCreationFeedbacks: nativeptr<VkPipelineCreationFeedbackEXT>

[<Struct>]
type VkSurfaceFullScreenExclusiveInfoEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable fullScreenExclusive: VkFullScreenExclusiveEXT

[<Struct>]
type VkSurfaceFullScreenExclusiveWin32InfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable hmonitor: HMONITOR

[<Struct>]
type VkSurfaceCapabilitiesFullScreenExclusiveEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable fullScreenExclusiveSupported: VkBool32

[<Struct>]
type VkHeadlessSurfaceCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkHeadlessSurfaceCreateFlagsEXT

[<Struct>]
type VkPhysicalDeviceCoverageReductionModeFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable coverageReductionMode: VkBool32

[<Struct>]
type VkPipelineCoverageReductionStateCreateInfoNV =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable flags: VkPipelineCoverageReductionStateCreateFlagsNV
    val mutable coverageReductionMode: VkCoverageReductionModeNV

[<Struct>]
type VkFramebufferMixedSamplesCombinationNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable coverageReductionMode: VkCoverageReductionModeNV
    val mutable rasterizationSamples: VkSampleCountFlagBits
    val mutable depthStencilSamples: VkSampleCountFlags
    val mutable colorSamples: VkSampleCountFlags

[<Struct>]
type VkPhysicalDeviceShaderIntegerFunctions2FeaturesINTEL =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderIntegerFunctions2: VkBool32

[<Struct;StructLayout(LayoutKind.Explicit)>]
type VkPerformanceValueDataINTEL =
    [<FieldOffset(0)>] val mutable value32: uint32
    [<FieldOffset(0)>] val mutable value64: uint64
    [<FieldOffset(0)>] val mutable valueFloat: float32
    [<FieldOffset(0)>] val mutable valueBool: VkBool32
    [<FieldOffset(0)>] val valueString: nativeptr<char>

[<Struct>]
type VkPerformanceValueINTEL =
    val mutable typ: VkPerformanceValueTypeINTEL
    val mutable data: VkPerformanceValueDataINTEL

[<Struct>]
type VkInitializePerformanceApiInfoINTEL =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable pUserData: nativeint

[<Struct>]
type VkQueryPoolCreateInfoINTEL =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable performanceCountersSampling: VkQueryPoolSamplingModeINTEL

[<Struct>]
type VkPerformanceMarkerInfoINTEL =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable marker: uint64

[<Struct>]
type VkPerformanceStreamMarkerInfoINTEL =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable marker: uint32

[<Struct>]
type VkPerformanceOverrideInfoINTEL =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable typ: VkPerformanceOverrideTypeINTEL
    val mutable enable: VkBool32
    val mutable parameter: uint64

[<Struct>]
type VkPerformanceConfigurationAcquireInfoINTEL =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable typ: VkPerformanceConfigurationTypeINTEL

[<Struct>]
type VkPhysicalDeviceIndexTypeUint8FeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable indexTypeUint8: VkBool32

[<Struct>]
type VkPhysicalDeviceShaderSMBuiltinsPropertiesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderSMCount: uint32
    val mutable shaderWarpsPerSM: uint32

[<Struct>]
type VkPhysicalDeviceShaderSMBuiltinsFeaturesNV =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderSMBuiltins: VkBool32

[<Struct>]
type VkPhysicalDeviceFragmentShaderInterlockFeaturesEXT =
    val mutable sType: VkStructureType
    /// Pointer to next structure
    val mutable pNext: nativeint
    val mutable fragmentShaderSampleInterlock: VkBool32
    val mutable fragmentShaderPixelInterlock: VkBool32
    val mutable fragmentShaderShadingRateInterlock: VkBool32

[<Struct>]
type VkPhysicalDevicePipelineExecutablePropertiesFeaturesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable pipelineExecutableInfo: VkBool32

[<Struct>]
type VkPipelineInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable pipeline: VkPipeline

[<Struct>]
type VkPipelineExecutablePropertiesKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable stages: VkShaderStageFlags
    val mutable name: char
    val mutable description: char
    val mutable subgroupSize: uint32

[<Struct>]
type VkPipelineExecutableInfoKHR =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable pipeline: VkPipeline
    val mutable executableIndex: uint32

[<Struct;StructLayout(LayoutKind.Explicit)>]
type VkPipelineExecutableStatisticValueKHR =
    [<FieldOffset(0)>] val mutable b32: VkBool32
    [<FieldOffset(0)>] val mutable i64: int64
    [<FieldOffset(0)>] val mutable u64: uint64
    [<FieldOffset(0)>] val mutable f64: float

[<Struct>]
type VkPipelineExecutableStatisticKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable name: char
    val mutable description: char
    val mutable format: VkPipelineExecutableStatisticFormatKHR
    val mutable value: VkPipelineExecutableStatisticValueKHR

[<Struct>]
type VkPipelineExecutableInternalRepresentationKHR =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable name: char
    val mutable description: char
    val mutable isText: VkBool32
    val mutable dataSize: nativeint
    val mutable pData: nativeint

[<Struct>]
type VkPhysicalDeviceShaderDemoteToHelperInvocationFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable shaderDemoteToHelperInvocation: VkBool32

[<Struct>]
type VkPhysicalDeviceTexelBufferAlignmentFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable texelBufferAlignment: VkBool32

[<Struct>]
type VkPhysicalDeviceTexelBufferAlignmentPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable storageTexelBufferOffsetAlignmentBytes: VkDeviceSize
    val mutable storageTexelBufferOffsetSingleTexelAlignment: VkBool32
    val mutable uniformTexelBufferOffsetAlignmentBytes: VkDeviceSize
    val mutable uniformTexelBufferOffsetSingleTexelAlignment: VkBool32

[<Struct>]
type VkPhysicalDeviceSubgroupSizeControlFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable subgroupSizeControl: VkBool32
    val mutable computeFullSubgroups: VkBool32

[<Struct>]
type VkPhysicalDeviceSubgroupSizeControlPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    /// The minimum subgroup size supported by this device
    val mutable minSubgroupSize: uint32
    /// The maximum subgroup size supported by this device
    val mutable maxSubgroupSize: uint32
    /// The maximum number of subgroups supported in a workgroup
    val mutable maxComputeWorkgroupSubgroups: uint32
    /// The shader stages that support specifying a subgroup size
    val mutable requiredSubgroupSizeStages: VkShaderStageFlags

[<Struct>]
type VkPipelineShaderStageRequiredSubgroupSizeCreateInfoEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable requiredSubgroupSize: uint32

[<Struct>]
type VkPhysicalDeviceLineRasterizationFeaturesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable rectangularLines: VkBool32
    val mutable bresenhamLines: VkBool32
    val mutable smoothLines: VkBool32
    val mutable stippledRectangularLines: VkBool32
    val mutable stippledBresenhamLines: VkBool32
    val mutable stippledSmoothLines: VkBool32

[<Struct>]
type VkPhysicalDeviceLineRasterizationPropertiesEXT =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable lineSubPixelPrecisionBits: uint32

[<Struct>]
type VkPipelineRasterizationLineStateCreateInfoEXT =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable lineRasterizationMode: VkLineRasterizationModeEXT
    val mutable stippledLineEnable: VkBool32
    val mutable lineStippleFactor: uint32
    val mutable lineStipplePattern: uint16

[<Struct>]
type VkPipelineCompilerControlCreateInfoAMD =
    val mutable sType: VkStructureType
    val pNext: nativeint
    val mutable compilerControlFlags: VkPipelineCompilerControlFlagsAMD

[<Struct>]
type VkPhysicalDeviceCoherentMemoryFeaturesAMD =
    val mutable sType: VkStructureType
    val mutable pNext: nativeint
    val mutable deviceCoherentMemory: VkBool32

let VK_MAX_PHYSICAL_DEVICE_NAME_SIZE = 256
let VK_UUID_SIZE = 16
let VK_LUID_SIZE = 8
let VK_LUID_SIZE_KHR = VK_LUID_SIZE
let VK_MAX_EXTENSION_NAME_SIZE = 256
let VK_MAX_DESCRIPTION_SIZE = 256
let VK_MAX_MEMORY_TYPES = 32
/// The maximum number of unique memory heaps, each of which supporting 1 or more memory types
let VK_MAX_MEMORY_HEAPS = 16
let VK_LOD_CLAMP_NONE = 1000.0f
let VK_REMAINING_MIP_LEVELS = ~~~0u
let VK_REMAINING_ARRAY_LAYERS = ~~~0u
let VK_WHOLE_SIZE = ~~~0UL
let VK_ATTACHMENT_UNUSED = ~~~0u
let VK_TRUE = 1
let VK_FALSE = 0
let VK_QUEUE_FAMILY_IGNORED = ~~~0u
let VK_QUEUE_FAMILY_EXTERNAL = ~~~0u-1u
let VK_QUEUE_FAMILY_EXTERNAL_KHR = VK_QUEUE_FAMILY_EXTERNAL
let VK_QUEUE_FAMILY_FOREIGN_EXT = ~~~0u-2u
let VK_SUBPASS_EXTERNAL = ~~~0u
let VK_MAX_DEVICE_GROUP_SIZE = 32
let VK_MAX_DEVICE_GROUP_SIZE_KHR = VK_MAX_DEVICE_GROUP_SIZE
let VK_MAX_DRIVER_NAME_SIZE_KHR = 256
let VK_MAX_DRIVER_INFO_SIZE_KHR = 256
let VK_SHADER_UNUSED_NV = ~~~0u