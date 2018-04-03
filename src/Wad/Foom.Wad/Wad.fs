namespace Foom.Wad

open System
open System.IO
open System.Numerics
open System.Runtime.InteropServices
open System.Collections.Generic
open System.Diagnostics
open Microsoft.FSharp.NativeInterop
open Foom.Wad.Unpickler
open Foom.Wad.Unpickle

#nowarn "9"

type Texture =
    {
        Data: Pixel [,]
        Name: string
    }

    member this.Width = Array2D.length1 this.Data

    member this.Height = Array2D.length2 this.Data

[<AutoOpen>]
module WadHelpers =

    let runUnpickle u (stream: Stream) =
        u_run u (ReadStream stream) 

    let runUnpickles us (stream: Stream) =
        let stream = ReadStream stream
        us |> Array.map (fun u -> u_run u stream)

    let loadLump u (header: LumpHeader) fileName = 
        runUnpickle (u header.Size (int64 header.Offset)) fileName

    let loadLumpMarker u (markerStart: LumpHeader) (markerEnd: LumpHeader) fileName =
        runUnpickle (u (markerEnd.Offset - markerStart.Offset) (int64 markerStart.Offset)) fileName

    let loadLumps u (headers: LumpHeader []) fileName =
        let us =
            headers
            |> Array.map (fun h -> u h.Size (int64 h.Offset))
        runUnpickles us fileName

    let loadPalettes stream wadData =
        wadData.LumpHeaders |> Array.tryFind (fun x -> x.Name.ToUpper () = "PLAYPAL")
        |> Option.bind (fun lumpPaletteHeader ->
            let lumpPalettes = loadLump u_lumpPalettes lumpPaletteHeader stream
            Some lumpPalettes.[0]
        )

    let filterLumpHeaders (headers: LumpHeader []) =
        headers
        |> Array.filter (fun x ->
            match x.Name.ToUpper () with
            | "F1_START" -> false
            | "F2_START" -> false
            | "F3_START" -> false
            | "F1_END" -> false
            | "F2_END" -> false
            | "F3_END" -> false
            | _ -> true)

    let tryFindLump (str: string) (headers: LumpHeader []) =
        headers
        |> Array.tryFind (fun x -> x.Name.ToUpper() = str.ToUpper())

    [<RequireQualifiedAccess>]
    type PatchTexture =
        | DoomPicture of DoomPicture
        | Flat of Texture

[<Sealed>]
type Wad(stream: Stream) = 

    let wadData = runUnpickle u_wad stream
    let defaultPaletteData = loadPalettes stream wadData

    let textureInfoLookup = Dictionary<string, TextureInfo>()
    let flatHeaderLookup = Dictionary<string, LumpHeader>()

    let mutable wads : Wad list = []
    let mutable isDisposed = false

    let assertNotDisposed() =
        if isDisposed then failwith "Wad is disposed."

    member __.ReloadFlatHeaders() =
        assertNotDisposed()

        flatHeaderLookup.Clear()

        let stream = stream
        let lumpHeaders = wadData.LumpHeaders

        let lumpFlatsHeaderStartIndex = lumpHeaders |> Array.tryFindIndex (fun x -> x.Name.ToUpper () = "F_START" || x.Name.ToUpper () = "FF_START")
        let lumpFlatsHeaderEndIndex = lumpHeaders |> Array.tryFindIndex (fun x -> x.Name.ToUpper () = "F_END" || x.Name.ToUpper () = "FF_END")

        match lumpFlatsHeaderStartIndex, lumpFlatsHeaderEndIndex with
        | None, None -> ()

        | Some _, None ->
            Debug.WriteLine """Warning: Unable to load flat textures because "F_END" lump was not found."""

        | None, Some _ ->
            Debug.WriteLine """Warning: Unable to load flat textures because "F_START" lump was not found."""

        | Some lumpFlatsHeaderStartIndex, Some lumpFlatsHeaderEndIndex ->
            let lumpFlatHeaders =
                lumpHeaders.[(lumpFlatsHeaderStartIndex + 1)..(lumpFlatsHeaderEndIndex - 1)]
                |> filterLumpHeaders

            let dict = flatHeaderLookup

            lumpFlatHeaders
            |> Array.iter (fun x ->
                dict.[x.Name.ToUpper ()] <- x
            )

    member __.ReloadTextureInfos() =
        assertNotDisposed()

        textureInfoLookup.Clear()

        let readTextureLump (textureLump: LumpHeader) =
            let textureHeader = runUnpickle (uTextureHeader textureLump) stream
            let textureInfos = runUnpickle (uTextureInfos textureLump textureHeader) stream
            textureInfos
            |> Array.iter (fun x -> 
                textureInfoLookup.[x.Name.ToUpper ()] <- x
            )

        wadData.LumpHeaders
        |> Array.filter (fun x ->
            let name = x.Name.ToUpper ()
            name = "TEXTURE1" || name = "TEXTURE2"
        )
        |> Array.iter (readTextureLump)

    member this.TryFindPatch patchName =
        assertNotDisposed()

        let result =
            match this.TryFindFlatTexture patchName with
            | Some texture -> Some (PatchTexture.Flat texture)
            | _ ->
                match tryFindLump patchName wadData.LumpHeaders with
                | Some header ->
                    (
                        runUnpickle (uDoomPicture header defaultPaletteData.Value) stream 
                    )
                    |> PatchTexture.DoomPicture
                    |> Some
                | _ -> None

        match result with
        | Some result -> Some result
        | None ->
            if wads.IsEmpty then
                None
            else
                wads
                |> List.map(fun x -> x.TryFindPatch patchName)
                |> List.last

    member this.TryFindFlatTexture(name: string) =
        assertNotDisposed()

        let name = name.ToUpper ()
        if Seq.isEmpty flatHeaderLookup then
            this.ReloadFlatHeaders()

        let result =
            match defaultPaletteData with
            | None ->
                Debug.WriteLine "Warning: Unable to load flat textures because there is no default palette."
                None
            | Some palette ->
                match flatHeaderLookup.TryGetValue (name) with
                | false, _ -> None
                | true, h ->

                    // Assert Flat Headers are valid
                    if h.Offset.Equals 0 then failwithf "Invalid flat header, %A. Offset is 0." h
                    if not (h.Size.Equals 4096) then failwithf "Invalid flat header, %A. Size is not 4096." h

                    let bytes = loadLump u_lumpRaw h stream

                    {
                        Name = h.Name
                        Data =
                            let pixels = Array2D.zeroCreate<Pixel> 64 64
                            for i = 0 to 64 - 1 do
                                for j = 0 to 64 - 1 do
                                    pixels.[i, j] <- palette.Pixels.[int bytes.[i + j * 64]]
                            pixels
                    } |> Some

        match result with
        | Some result -> Some result
        | None ->
            if wads.IsEmpty then
                None
            else
                wads
                |> List.map(fun x -> x.TryFindFlatTexture(name))
                |> List.last

    member this.TryFindTexture(name: string) =
        assertNotDisposed()

        let name = name.ToUpper ()

        let result =
            if Seq.isEmpty textureInfoLookup then
                this.ReloadTextureInfos()

            match textureInfoLookup.TryGetValue (name) with
            | false, _ -> None
            | true, info ->

                let mutable tex = Array2D.init info.Width info.Height (fun _ _ -> Pixel.Cyan)

                let pnamesLump =
                    wadData.LumpHeaders
                    |> Array.find (fun x -> x.Name.ToUpper() = "PNAMES")

                let patchNames = runUnpickle (uPatchNames pnamesLump) stream

                info.Patches
                |> Array.iter (fun patch ->
                    let patchName = patchNames.[patch.PatchNumber]
                    match this.TryFindPatch patchName with
                    | Some ptex ->

                        let data =
                            match ptex with
                            | PatchTexture.DoomPicture pic -> pic.Data
                            | PatchTexture.Flat tex -> tex.Data

                        // If the patchName is equal to the name of the texture we are trying to find and patch count is one,
                        //     then just use the data retrieved directly instead of going through the patching process.
                        if patchName = name && info.Patches.Length = 1 then
                            tex <- data
                        else
                            data
                            |> Array2D.iteri (fun i j pixel ->
                                let i = i + patch.OriginX
                                let j = j + patch.OriginY

                                if i < info.Width && j < info.Height && i >= 0 && j >= 0 && pixel <> Pixel.Cyan then
                                    tex.[i, j] <- pixel
                            )

                    | _ -> ()
                )

                {
                    Data = tex
                    Name = info.Name
                } |> Some

        match result with
        | Some result -> Some result
        | None ->
            if wads.IsEmpty then
                None
            else
                wads
                |> List.map(fun x -> x.TryFindTexture(name))
                |> List.last

    member this.TryFindSpriteTexture(textureName: string) =
        assertNotDisposed()

        let result =
            let lumpHeaders = wadData.LumpHeaders

            let lumpFlatsHeaderStartIndex = lumpHeaders |> Array.tryFindIndex (fun x -> x.Name.ToUpper () = "S_START")
            let lumpFlatsHeaderEndIndex = lumpHeaders |> Array.tryFindIndex (fun x -> x.Name.ToUpper () = "S_END")

            match lumpFlatsHeaderStartIndex, lumpFlatsHeaderEndIndex with
            | None, None -> None

            | Some _, None ->
                Debug.WriteLine """Warning: Unable to load flat textures because "S_END" lump was not found."""
                None

            | None, Some _ ->
                Debug.WriteLine """Warning: Unable to load flat textures because "S_START" lump was not found."""
                None

            | Some lumpFlatsHeaderStartIndex, Some lumpFlatsHeaderEndIndex ->
                let lumpFlatHeaders =
                    lumpHeaders.[(lumpFlatsHeaderStartIndex + 1)..(lumpFlatsHeaderEndIndex - 1)]
                    |> filterLumpHeaders

                match lumpFlatHeaders |> Array.tryFind (fun x -> x.Name.ToLower() = textureName.ToLower()), defaultPaletteData with
                | Some h, Some palette ->
                    let pic = stream |> runUnpickle (uDoomPicture h palette)
                    Some
                        {
                            Data = pic.Data
                            Name = textureName
                        }
                | _ -> None

        match result with
        | Some result -> Some result
        | None ->
            if wads.IsEmpty then
                None
            else
                wads
                |> List.map(fun x -> x.TryFindSpriteTexture(textureName))
                |> List.last

    member this.FindMap(name: string) =
        assertNotDisposed()

        let name = name.ToLower ()

        match
            wadData.LumpHeaders
            |> Array.tryFindIndex (fun x -> x.Name.ToLower () = name.ToLower ()) with
        | None -> failwithf "Unable to find level, %s." name
        | Some lumpLevelStartIndex ->

        // printfn "Found Level: %s" name
        let lumpHeaders = wadData.LumpHeaders.[lumpLevelStartIndex..]

        // Note: This seems to work, but may be possible to get invalid data for the level.
        let lumpThingsHeader = lumpHeaders |> Array.find (fun x -> x.Name.ToLower () = "THINGS".ToLower ())
        let lumpLinedefsHeader = lumpHeaders |> Array.find (fun x -> x.Name.ToLower () = "LINEDEFS".ToLower ())
        let lumpSidedefsHeader = lumpHeaders |> Array.find (fun x -> x.Name.ToLower () = "SIDEDEFS".ToLower ())
        let lumpVerticesHeader = lumpHeaders |> Array.find (fun x -> x.Name.ToLower () = "VERTEXES".ToLower ())
        let lumpSectorsHeader = lumpHeaders |> Array.find (fun x -> x.Name.ToLower () = "SECTORS".ToLower ())

        let lumpThings = loadLump (u_lumpThings ThingFormat.Doom) lumpThingsHeader stream
        let lumpVertices = loadLump u_lumpVertices lumpVerticesHeader stream
        let lumpSidedefs = loadLump u_lumpSidedefs lumpSidedefsHeader stream
        let lumpLinedefs = loadLump (u_lumpLinedefs lumpVertices.Vertices lumpSidedefs.Sidedefs) lumpLinedefsHeader stream
        let lumpSectors = loadLump (u_lumpSectors) lumpSectorsHeader stream

        Map.Create (lumpSectors.Sectors, lumpThings.Things, lumpLinedefs.Linedefs, lumpSidedefs.Sidedefs)

    member __.FindMusic(name: string) =
        let lumpHeader = 
            let lumpHeaderOpt =
                wadData.LumpHeaders
                |> Array.tryFind (fun x -> x.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            match lumpHeaderOpt with
            | Some x -> x
            | _ -> failwithf "Unable to find lump header, %s." name

        let mus = LumpMus.Parse lumpHeader stream
        printfn "%A" mus
        ()
            

    member this.OverrideFromFile(fileName: string) =
        let wad = Wad.FromFile(fileName)
        wads <- wad :: wads

    static member FromFile(fileName: string) =
        let stream = File.OpenRead fileName
        Wad.FromStream(stream)

    static member FromStream(stream: Stream) =
        let wad = new Wad(stream)
        wad.ReloadFlatHeaders()
        wad.ReloadTextureInfos()
        wad

    interface IDisposable with

        member this.Dispose() =
            wads
            |> List.iter(fun x -> (x :> IDisposable).Dispose())
            stream.Dispose()
            isDisposed <- true

