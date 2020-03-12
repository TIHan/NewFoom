module Falkan.FreeType

open System
open System.Drawing
open FSharp.NativeInterop
open Falkan.FreeTypeInterop

#nowarn "9"
#nowarn "51"

[<AutoOpen>]
module internal Helpers =

    let internal checkResult n =
        if n <> 0 then
            failwith "Failed FreeType call"

[<Sealed>]
type FreeTypeBitmap internal (bmp: Bitmap, top: int, left: int) =

    member _.Bitmap = bmp

    member _.Top = top

    member _.Left = left

    interface IDisposable with

        member _.Dispose() =
            bmp.Dispose()

[<Sealed>]
type FreeTypeFace(lib: FT_Library, face: FT_Face) =
    
    member _.SetCharSize(width, height, horizontalResolution, verticalResolution) =
        FT_Set_Char_Size(face, width, height, horizontalResolution, verticalResolution) |> checkResult

    member _.GetCharBitmap(charCode: char, size: int) =
        FT_Set_Pixel_Sizes(face, 0u, uint32 size) |> checkResult

        let index = FT_Get_Char_Index(face, uint32 charCode)

        FT_Load_Glyph(face, index, 0) |> checkResult

        let derefFace = (NativePtr.get face 0)
        let glyphSlot = derefFace.glyph
        FT_Render_Glyph(glyphSlot, FT_Render_Mode.FT_RENDER_MODE_NORMAL) |> checkResult

        let glyph = (NativePtr.get glyphSlot 0)
        let bitmap = glyph.bitmap
        let left = glyph.bitmap_left |> int
        let top = glyph.bitmap_top |> int
        let width = int bitmap.width
        let height = int bitmap.rows
        let bmp = new Bitmap(width, height, Imaging.PixelFormat.Format32bppArgb)

        for x = 0 to width - 1 do
            for y = 0 to height - 1 do
                let v = NativePtr.get (NativePtr.ofNativeInt<byte> bitmap.buffer) (y * int bitmap.width + x)
                let a =
                    if v = 0uy then 0
                    else 255
                bmp.SetPixel(x, y, Color.FromArgb(a, int v, int v, int v))
        
        new FreeTypeBitmap(bmp, top, left)

[<Sealed>]
type FreeType private (lib: FT_Library) =

    member _.Load(filePath: string) =
        let mutable face = Unchecked.defaultof<_>
        FT_New_Face(lib, filePath, 0, &&face) |> checkResult
        FreeTypeFace(lib, face)

    static member Create() =
        let mutable lib = Unchecked.defaultof<_>
        FT_Init_FreeType &&lib |> checkResult
        FreeType lib