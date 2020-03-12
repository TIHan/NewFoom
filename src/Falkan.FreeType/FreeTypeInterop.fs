module internal rec Falkan.FreeTypeInterop

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

type FT_Error = int
type FT_Pointer = nativeint
type FT_Library = nativeint
type FT_New_Face = nativeint
type FT_Long = int
type FT_F26Dot6 = int
type FT_UInt = uint32
type FT_ULong = uint32
type FT_Int32 = int
type FT_Int = int
type FT_String = sbyte
type FT_Pos = int
type FT_UShort = uint16
type FT_Short = int16

[<Struct;NoEquality;NoComparison>]
type FT_Generic =
    
    [<DefaultValue(false)>] val mutable data : nativeint // voidptr
    [<DefaultValue(false)>] val mutable finalizer : nativeint // FT_Generic_Finalizer

[<Struct;NoEquality;NoComparison>]
type FT_BBox =

    [<DefaultValue(false)>] val mutable xMin : FT_Pos
    [<DefaultValue(false)>] val mutable yMin : FT_Pos
    [<DefaultValue(false)>] val mutable xMax : FT_Pos
    [<DefaultValue(false)>] val mutable yMax : FT_Pos

type FT_Size = nativeint
type FT_CharMap = nativeint
type FT_Driver = nativeint
type FT_Memory = nativeint
type FT_Stream = nativeint
type FT_ListNode = nativeint
type FT_Face_Internal = nativeint
type FT_Fixed = int

[<Struct;NoEquality;NoComparison>]
type FT_Vector =

    [<DefaultValue(false)>] val mutable x : FT_Pos
    [<DefaultValue(false)>] val mutable y : FT_Pos

[<Struct;NoEquality;NoComparison>]
type FT_ListRec =

    [<DefaultValue(false)>] val mutable head : FT_ListNode
    [<DefaultValue(false)>] val mutable tail : FT_ListNode

type FT_Slot_Internal = nativeint

[<Struct;NoEquality;NoComparison>]
type FT_Glyph_Metrics =

    [<DefaultValue(false)>] val mutable width : FT_Pos
    [<DefaultValue(false)>] val mutable height : FT_Pos

    [<DefaultValue(false)>] val mutable horiBearingX : FT_Pos
    [<DefaultValue(false)>] val mutable horiBearingY : FT_Pos
    [<DefaultValue(false)>] val mutable horiAdvance : FT_Pos

    [<DefaultValue(false)>] val mutable vertBearingX : FT_Pos
    [<DefaultValue(false)>] val mutable vertBearingY : FT_Pos
    [<DefaultValue(false)>] val mutable vertAdvance : FT_Pos

[<Struct;NoEquality;NoComparison>]
type FT_Bitmap =

  [<DefaultValue(false)>] val mutable rows : uint32
  [<DefaultValue(false)>] val mutable width : uint32
  [<DefaultValue(false)>] val mutable pitch : int
  [<DefaultValue(false)>] val mutable buffer : nativeint // unsigned char*
  [<DefaultValue(false)>] val mutable num_grays : uint16
  [<DefaultValue(false)>] val mutable pixel_mode : byte
  [<DefaultValue(false)>] val mutable palette_mode : byte
  [<DefaultValue(false)>] val mutable palette : nativeint

type FT_Glyph_Format = int

[<Struct;NoEquality;NoComparison>]
type FT_GlyphSlotRec =

  [<DefaultValue(false)>] val mutable library : FT_Library
  [<DefaultValue(false)>] val mutable face : FT_Face //  FT_Face           face;
  [<DefaultValue(false)>] val mutable next : FT_GlyphSlot //  FT_GlyphSlot      next;
  [<DefaultValue(false)>] val mutable glyph_index :  FT_UInt //           glyph_index; /* new in 2.10; was reserved previously */
  [<DefaultValue(false)>] val mutable generic : FT_Generic //        generic;

  [<DefaultValue(false)>] val mutable metrics : FT_Glyph_Metrics //  metrics;
  [<DefaultValue(false)>] val mutable linearHoriAdvance :  FT_Fixed          //linearHoriAdvance;
  [<DefaultValue(false)>] val mutable linearVertAdvance :  FT_Fixed         // linearVertAdvance;
  [<DefaultValue(false)>] val mutable advance : FT_Vector        // advance;

  [<DefaultValue(false)>] val mutable format : FT_Glyph_Format //  format;

  [<DefaultValue(false)>] val mutable bitmap : FT_Bitmap        // bitmap;
  [<DefaultValue(false)>] val mutable bitmap_left : FT_Int           // bitmap_left;
  [<DefaultValue(false)>] val mutable bitmap_top : FT_Int        //    bitmap_top;

  //[<DefaultValue(false)>] val mutable outline : FT_Outline      //  outline;

  //[<DefaultValue(false)>] val mutable num_subglyphs : FT_UInt          // num_subglyphs;
  //[<DefaultValue(false)>] val mutable subglyphs : FT_SubGlyph     //  subglyphs;

  //[<DefaultValue(false)>] val mutable control_data : nativeint // void*             control_data;
  //[<DefaultValue(false)>] val mutable control_len : int // long              control_len;

  //[<DefaultValue(false)>] val mutable lsb_delta : FT_Pos         //   lsb_delta;
  //[<DefaultValue(false)>] val mutable resb_delta : FT_Pos          //  rsb_delta;

  //[<DefaultValue(false)>] val mutable other : nativeint // void*             other;

  //[<DefaultValue(false)>] val mutable internal' : FT_Slot_Internal // internal';

type FT_GlyphSlot = nativeptr<FT_GlyphSlotRec>

type FT_Render_Mode =
    | FT_RENDER_MODE_NORMAL = 0
    | FT_RENDER_MODE_LIGHT = 1
    | FT_RENDER_MODE_MONO = 2
    | FT_RENDER_MODE_LCD = 3
    | FT_RENDER_MODE_LCD_V = 4
    | FT_RENDER_MODE_MAX = 5

[<Struct;NoEquality;NoComparison>]
type FT_FaceRec =
    
    [<DefaultValue(false)>] val mutable num_faces : FT_Long
    [<DefaultValue(false)>] val mutable face_index : FT_Long

    [<DefaultValue(false)>] val mutable face_flags : FT_Long
    [<DefaultValue(false)>] val mutable style_flags : FT_Long

    [<DefaultValue(false)>] val mutable num_glyphs : FT_Long

    [<DefaultValue(false)>] val mutable family_name : nativeint // FT_String*
    [<DefaultValue(false)>] val mutable style_name : nativeint // FT_String*

    [<DefaultValue(false)>] val mutable num_fixed_sizes : FT_Int
    [<DefaultValue(false)>] val mutable available_sizes : nativeint // FT_Bitmap_Size*

    [<DefaultValue(false)>] val mutable num_charmaps : FT_Int
    [<DefaultValue(false)>] val mutable charmaps : nativeint // FT_CharMap*

    [<DefaultValue(false)>] val mutable generic : FT_Generic

    [<DefaultValue(false)>] val mutable bbox : FT_BBox

    [<DefaultValue(false)>] val mutable units_per_EM : FT_UShort
    [<DefaultValue(false)>] val mutable ascender : FT_Short
    [<DefaultValue(false)>] val mutable descender : FT_Short
    [<DefaultValue(false)>] val mutable height : FT_Short

    [<DefaultValue(false)>] val mutable max_advance_width : FT_Short
    [<DefaultValue(false)>] val mutable max_advance_height : FT_Short

    [<DefaultValue(false)>] val mutable underline_position : FT_Short
    [<DefaultValue(false)>] val mutable underline_thickness : FT_Short

    [<DefaultValue(false)>] val mutable glyph : FT_GlyphSlot
    [<DefaultValue(false)>] val mutable size : FT_Size
    [<DefaultValue(false)>] val mutable charmap : FT_CharMap

    [<DefaultValue(false)>] val mutable driver : FT_Driver
    [<DefaultValue(false)>] val mutable memory : FT_Memory
    [<DefaultValue(false)>] val mutable stream : FT_Stream

    [<DefaultValue(false)>] val mutable sizes_list : FT_ListRec

    [<DefaultValue(false)>] val mutable autohint : FT_Generic
    [<DefaultValue(false)>] val mutable extensions : nativeint

    [<DefaultValue(false)>] val mutable internal' : FT_Face_Internal

type FT_Face = nativeptr<FT_FaceRec>

[<DllImport("freetype.dll")>]
extern FT_Error FT_Init_FreeType(FT_Library *alibrary)

[<DllImport("freetype.dll")>]
extern FT_Error FT_New_Face(FT_Library library, string filepathname, FT_Long face_index, FT_Face *aface)

[<DllImport("freetype.dll")>]
extern FT_Error FT_Set_Char_Size(FT_Face face, FT_F26Dot6 char_width, FT_F26Dot6 char_height, FT_UInt horz_resolution, FT_UInt vert_resolution)

[<DllImport("freetype.dll")>]
extern FT_Error FT_Set_Pixel_Sizes(FT_Face face, FT_UInt pixel_width, FT_UInt pixel_height)

[<DllImport("freetype.dll")>]
extern FT_UInt FT_Get_Char_Index(FT_Face face, FT_ULong charcode)

[<DllImport("freetype.dll")>]
extern FT_Error FT_Load_Glyph(FT_Face face, FT_UInt glyph_index, FT_Int32 load_flags)

[<DllImport("freetype.dll")>]
extern FT_Error FT_Render_Glyph(FT_GlyphSlot slot, FT_Render_Mode render_mode)