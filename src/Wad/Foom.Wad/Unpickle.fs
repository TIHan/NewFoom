module internal Foom.Wad.Unpickle

open System
open System.Numerics
open System.Collections.Generic
open Foom.Geometry
open Foom.Wad.Unpickler

type Header = { IsPwad: bool; LumpCount: int; LumpOffset: int }
 
type LumpHeader = { Offset: int32; Size: int32; Name: string }
 
type WadData = { Header: Header; LumpHeaders: LumpHeader [] }

type ThingFormat =
    | Doom = 0
    | Hexen = 1

type LumpThings = { Things: Thing [] }
type LumpLinedefs = { Linedefs: Linedef [] }
type LumpSidedefs = { Sidedefs: Sidedef [] }
type LumpVertices = { Vertices: Vector2 [] }
type LumpSectors = { Sectors: Sector [] }

type PaletteData = { Pixels: Pixel [] }

type TextureHeader =
    {
        Count: int
        Offsets: int []
    }

type TexturePatch =
    {
        OriginX: int
        OriginY: int
        PatchNumber: int
    }

type TextureInfo =
    {
        Name: string
        IsMasked: bool
        Width: int
        Height: int
        Patches: TexturePatch []
    }

type DoomPictureHeader =
    {
        Width: int
        Height: int
        Top: int
        Left: int
    }

type DoomPicture =
    {
        Width: int
        Height: int
        Top: int
        Left: int
        Data: Pixel [,]
    }

type MusHeader =
    {
        ScoreLength: int
        ScoreStart: int
        PrimaryChannelCount: int
        SecondaryChannelCount: int
        InstrumentCount: int
        Instruments: int []
    }

[<AutoOpen>]
module Implementation =

    let inline u_arrayi n (p: int -> Unpickle<'a>) =
        fun stream ->
            match n with
            | 0 -> [||]
            | _ -> Array.init n (fun i -> p i stream)

    let u_header : Unpickle<Header> =
        u_pipe3 (u_string 4) u_int32 u_int32 <|
        fun id lumpCount lumpOffset ->
            { IsPwad = if id = "IWAD" then false else true
              LumpCount = lumpCount
              LumpOffset = lumpOffset }

    let u_lumpHeader : Unpickle<LumpHeader> =
        u_pipe3 u_int32 u_int32 (u_string 8) <| 
        fun offset size name -> 
            { 
                Offset = offset
                Size = size
                Name = name.Replace("\\", "").Trim().Trim('\000') 
            }

    let u_lumpHeaders count offset : Unpickle<LumpHeader []> =
        u_skipBytes offset >>. u_array count u_lumpHeader

    let u_wad : Unpickle<WadData> =
        u_lookAhead u_header >>= fun header ->
            (u_lookAhead <| (u_lumpHeaders header.LumpCount (int64 header.LumpOffset)) |>> (fun lumpHeaders -> { Header = header; LumpHeaders = lumpHeaders }))

    [<Literal>]
    let doomThingSize = 10
    [<Literal>]
    let hexenThingSize = 20
    let u_thing format : Unpickle<Thing> =
        match format with
        | ThingFormat.Doom ->
            u_pipe5 u_int16 u_int16 u_int16 u_int16 u_int16 <|
            fun x y angle typ flags ->
                Thing.Doom 
                    { 
                        X = int x 
                        Y = int y
                        Angle = int angle
                        Type = enum<ThingType> (int typ)
                        Flags = enum<DoomThingFlags> (int flags) 
                    }
        | _ -> failwith "Not supported."

    let u_things format count offset : Unpickle<Thing []> =
        u_skipBytes offset >>. u_array count (u_thing format)

    let u_lumpThings format size offset : Unpickle<LumpThings> =
        match format with
        | ThingFormat.Doom ->
            u_lookAhead (u_things format (size / doomThingSize) offset) |>> fun things -> { Things = things }
        | _ -> failwith "Not supported."

    [<Literal>]
    let vertexSize = 4
    let u_vertex : Unpickle<Vector2> =
        u_pipe2 u_int16 u_int16 <|
        fun x y -> Vector2 (single x, single y)

    let u_vertices count offset : Unpickle<Vector2 []> =
        u_skipBytes offset >>. u_array count u_vertex

    let u_lumpVertices size offset : Unpickle<LumpVertices> =
        u_lookAhead (u_vertices (size / vertexSize) offset) |>> fun vertices -> { Vertices = vertices }

    let tryParseTextureName name =
        if String.IsNullOrWhiteSpace (name) || name = "-" then
            None
        else
            Some (name.ToUpper())

    [<Literal>]
    let sidedefSize = 30
    let u_sidedef : Unpickle<Sidedef> =
        u_pipe6 u_int16 u_int16 (u_string 8) (u_string 8) (u_string 8) u_int16 <|
        fun offsetX offsetY upperTexName lowerTexName middleTexName sectorNumber ->
            { 
                OffsetX = int offsetX
                OffsetY = int offsetY
                UpperTextureName = upperTexName.Trim().Trim('\000') |> tryParseTextureName
                LowerTextureName = lowerTexName.Trim().Trim('\000') |> tryParseTextureName
                MiddleTextureName = middleTexName.Trim().Trim('\000') |> tryParseTextureName
                SectorNumber = int sectorNumber
            }

    let u_sidedefs count offset : Unpickle<Sidedef []> =
        u_skipBytes offset >>. u_array count u_sidedef

    let u_lumpSidedefs size offset : Unpickle<LumpSidedefs> =
        u_lookAhead (u_sidedefs (size / sidedefSize) offset) |>> fun sidedefs -> { Sidedefs = sidedefs }

    [<Literal>]
    let linedefSize = 14
    let u_linedef (vertices: Vector2 []) (sidedefs: Sidedef []) : Unpickle<Linedef> =
        u_pipe7 u_uint16 u_uint16 u_int16 u_int16 u_int16 u_uint16 u_uint16 <|
        fun startVertex endVertex flags specialType sectorTag rightSidedef leftSidedef ->
            let f = match int rightSidedef with | n when n <> 65535 -> Some n | _ -> None
            let b = match int leftSidedef with | n when n <> 65535 -> Some n | _ -> None
            {
                Segment = LineSegment2D(vertices.[int startVertex], vertices.[int endVertex])
                FrontSidedefIndex = f
                BackSidedefIndex = b
                Flags = enum<LinedefFlags> (int flags)
                SpecialType = int specialType
                SectorTag = int sectorTag
            }

    let u_linedefs (vertices: Vector2 []) (sidedefs: Sidedef []) count offset =
        u_skipBytes offset >>. u_array count (u_linedef vertices sidedefs) 
        |>> (fun linedefs -> { Linedefs = linedefs })
        
    let u_lumpLinedefs (vertices: Vector2 []) (sidedefs: Sidedef []) size offset : Unpickle<LumpLinedefs> =
        u_lookAhead (u_linedefs vertices sidedefs (size / linedefSize) offset)

    [<Literal>]
    let sectorSize = 26
    let u_sector (sectorId: int) : Unpickle<Sector> =
        u_pipe7 u_int16 u_int16 (u_string 8) (u_string 8) u_int16 u_int16 u_int16 <|
        fun floorHeight ceilingHeight floorTexName ceilingTexName lightLevel typ tag ->
            { Id = sectorId
              FloorHeight = int floorHeight
              CeilingHeight = int ceilingHeight
              FloorTextureName = floorTexName.Trim().Trim('\000').ToUpper()
              CeilingTextureName = ceilingTexName.Trim().Trim('\000').ToUpper()
              LightLevel = int lightLevel
              Type = enum<SectorType> (int typ)
              Tag = int tag
            }

    let u_sectors count offset : Unpickle<Sector []> =
        u_skipBytes offset >>. u_arrayi count (u_sector)

    let u_lumpSectors size offset : Unpickle<LumpSectors> =
        u_lookAhead (u_sectors (size / sectorSize) offset) |>> fun sectors -> { Sectors = sectors }

    [<Literal>]
    let flatSize = 4096

    [<Literal>]
    let paletteSize = 768
    let u_pixel =
        u_pipe3 u_byte u_byte u_byte <| fun r g b -> Pixel (r, g, b)

    let u_pixels count = u_array count u_pixel

    let u_palette = (u_pixels (paletteSize / sizeof<Pixel>)) |>> fun pixels -> { Pixels = pixels }

    let u_palettes count offset : Unpickle<PaletteData []> =
        u_skipBytes offset >>. u_array count (u_palette)

    let u_lumpPalettes size offset : Unpickle<PaletteData []> =
        u_lookAhead (u_palettes (size / paletteSize) offset)

    let u_lumpRaw size offset : Unpickle<byte []> =
        u_lookAhead (u_skipBytes offset >>. u_bytes size)

    let inline goToLump lumpHeader u =
        u_lookAhead (u_skipBytes (int64 lumpHeader.Offset) >>. u)

    let uTextureHeader lumpHeader : Unpickle<TextureHeader> =
        goToLump lumpHeader (
            u_int32 >>= fun count ->
                (u_array count u_int32) |>> fun offsets ->
                    {
                        Count = count
                        Offsets = offsets
                    }
        )

    let uTextureInfos lumpHeader (textureHeader: TextureHeader) : Unpickle<TextureInfo []> =


        let uPatch =
            u_pipe5 u_int16 u_int16 u_int16 u_int16 u_int16 <|
            fun originX originY patch _ _ ->
                {
                    OriginX = int originX
                    OriginY = int originY
                    PatchNumber = int patch
                }

        let uInfo = 
            u_pipe6 (u_string 8) (u_int32) (u_int16) (u_int16) (u_int32) (u_int16) 
                (fun name masked width height _ patchCount -> (name, masked, width, height, patchCount)) 
                    >>= fun (name, masked, width, height, patchCount) ->
                        u_array (int patchCount) uPatch |>> fun patches ->
                            {
                                Name = name.Trim().Trim('\000').ToUpper()
                                IsMasked = if masked = 1 then true else false
                                Width = int width
                                Height = int height
                                Patches = patches
                            }

        goToLump lumpHeader (
            u_arrayi textureHeader.Offsets.Length (fun i ->
                u_lookAhead (u_skipBytes (int64 textureHeader.Offsets.[i]) >>. uInfo)   
            )
        )

    let uPatchNames lumpHeader : Unpickle<string []> =
        goToLump lumpHeader (
            u_int32 >>= fun count ->
                u_array count (u_string 8) 
                |>> fun names -> 
                    names 
                    |> Array.map (fun name -> name.Trim().Trim('\000').ToUpper())
        )

    let uTexture : Unpickle<DoomPictureHeader> =
        u_pipe4 u_uint16 u_uint16 u_int16 u_int16 <| 
        fun width height top left ->
            {
                Width = int width
                Height = int height
                Top = int top
                Left = int left
            }

    let uDoomPicture (lumpHeader: LumpHeader)  (palette: PaletteData) : Unpickle<DoomPicture> =

        goToLump lumpHeader (
            u_lookAhead (
                uTexture >>= fun texture -> 
                    u_lookAhead (u_array texture.Width (u_uint32)) 
                    |>> fun columnArray -> (columnArray, texture)) >>= fun (columnArray, texture) ->

                        let data = Array2D.init texture.Width texture.Height (fun _ _ -> Pixel (0uy, 255uy, 255uy))

                        u_arrayi texture.Width (fun i ->
                            u_lookAhead (
                                fun stream ->

                                    stream.Skip(int64 columnArray.[i])

                                    let mutable rowStart = 0uy

                                    while rowStart <> 255uy do
                                        rowStart <- stream.ReadByte()

                                        if rowStart <> 255uy then

                                            let count = stream.ReadByte() |> int
                                            stream.ReadByte() |> ignore

                                            for j = 0 to count - 1 do
                                                let p = stream.ReadByte()
                                                let j = j + int rowStart
                                                if i < texture.Width && j < texture.Height then
                                                    data.[i,j] <- palette.Pixels.[int p]
                                            
                                            stream.ReadByte() |> ignore


                                    ()
                                //u_skipBytes (int64 columnArray.[i]) >>= fun () ->
                                //    u_pipe3 u_byte u_byte u_byte (fun rowStart count _ -> (rowStart, count)) >>= fun (rowStart, count) ->

                                //        if rowStart <> 255uy then
                                //            u_arrayi (int count + 4) (fun j ->
                                //                let j = j + int rowStart
                                //                u_byte |>> fun p ->
                                //                    if i < texture.Width && j < texture.Height then
                                //                        data.[i,j] <- palette.Pixels.[int p]                                    
                                //            )
                                //        else
                                //            fun _ -> Array.empty

                            )
                        ) |>> fun columns ->

                            {
                                Width = texture.Width
                                Height = texture.Height
                                Top = texture.Top
                                Left = texture.Left
                                Data = data
                            }

        )

    let uMusHeader lumpHeader =
        goToLump lumpHeader (
            (
                u_pipe7 
                    (u_array 4 u_byte)
                    u_uint16 // scoreLen
                    u_uint16 // scoreStart
                    u_uint16 // channels
                    u_uint16 // sec_channels
                    u_uint16 // instrCnt
                    u_uint16 // dummy
                <| fun _ scoreLen scoreStart channel secChannels instrCount _ ->
                    {
                        ScoreLength = int scoreLen
                        ScoreStart = int scoreStart
                        PrimaryChannelCount = int channel
                        SecondaryChannelCount = int secChannels
                        InstrumentCount = int instrCount
                        Instruments = [||]
                    }
            )
            >>= fun header ->
                u_array header.InstrumentCount u_uint16 
                |>> fun instruments -> 
                    { header with Instruments = instruments |> Array.map int }
        )

    // http://www.shikadi.net/moddingwiki/MUS_Format
    let uMusBody musHeader =
        u_lookAhead (
            u_skipBytes (int64 musHeader.ScoreStart) >>.
            fun stream ->
                let eventDescr = stream.ReadByte()

                let last = (eventDescr) >>> 7
                let eventType = ((eventDescr) <<< 1) >>> 5
                let channel = ((eventDescr <<< 4) >>> 4)

                match eventType with
                | 0uy -> // Release Note
                    let noteNumber = stream.ReadByte()
                    (last, eventType, noteNumber)
                | _ ->

                    (last, eventType, channel)
        )
      