module internal Falkan.FreeTypeInterop

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

[<Literal>]
let FreeTypeDllName = "freetype.dll"

type FT_Error = int
type FT_Pointer = nativeint
type FT_Library = nativeint
type FT_New_Face = nativeint
type FT_Long = int
type FT_Face = nativeint

[<DllImport(FreeTypeDllName)>]
extern FT_Error FT_Init_FreeType(FT_Library *alibrary)

[<DllImport(FreeTypeDllName)>]
extern FT_Error FT_New_Face(FT_Library library, string filepathname, FT_Long face_index, FT_Face *aface)