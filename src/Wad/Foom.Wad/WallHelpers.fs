[<AutoOpen>]
module internal Foom.Wad.WallHelpers

open System.Numerics

let updateUpperFront (bottom: float32) (top: float32) (index: int) (vertices: Vector3 []) =
    if top > bottom then
        vertices.[index + 0].Z <- bottom
        vertices.[index + 1].Z <- bottom
        vertices.[index + 2].Z <- top

        vertices.[index + 3].Z <- top
        vertices.[index + 4].Z <- top
        vertices.[index + 5].Z <- bottom
    else
        vertices.[index + 0].Z <- bottom
        vertices.[index + 1].Z <- bottom
        vertices.[index + 2].Z <- bottom

        vertices.[index + 3].Z <- bottom
        vertices.[index + 4].Z <- bottom
        vertices.[index + 5].Z <- bottom

let createUpperFront (p1: Vector2) (p2: Vector2) bottom top =
    let vertices =
        [|
            Vector3(p1, 0.f)
            Vector3(p2, 0.f)
            Vector3(p2, 0.f)

            Vector3(p2, 0.f)
            Vector3(p1, 0.f)
            Vector3(p1, 0.f)
        |]
    updateUpperFront bottom top 0 vertices
    vertices

let (|FrontBack|) (map: Map, linedef: Linedef) =
    let frontSideSectorOpt =
        linedef.FrontSidedefIndex
        |> Option.map(fun x -> map.Sidedefs.[x])
        |> Option.map(fun x -> map.Sectors.[x.SectorNumber])

    let backSideSectorOpt =
        linedef.BackSidedefIndex
        |> Option.map(fun x -> map.Sidedefs.[x])
        |> Option.map(fun x -> map.Sectors.[x.SectorNumber])

    (frontSideSectorOpt, backSideSectorOpt)

let tryInitializeUpperFront map linedef =
    match (map, linedef) with
    | FrontBack(Some frontSideSector, Some backSideSector) ->
        let p1, p2 = linedef.Segment.A, linedef.Segment.B
        let vertices = createUpperFront p1 p2 (float32 backSideSector.CeilingHeight) (float32 frontSideSector.CeilingHeight)
        Some vertices
    | _ ->
        None

let tryInitializeMiddleFront map linedef =
    let p1, p2 = linedef.Segment.A, linedef.Segment.B

    match (map, linedef) with
    | FrontBack(frontSideSectorOpt, backSideSectorOpt) ->
        match frontSideSectorOpt, backSideSectorOpt with

        | Some frontSideSector, Some backSideSector ->
            let bottom =
                if backSideSector.FloorHeight > frontSideSector.FloorHeight then
                    backSideSector.FloorHeight
                else
                    frontSideSector.FloorHeight
                |> float32

            let top =
                if backSideSector.CeilingHeight < frontSideSector.CeilingHeight then
                    backSideSector.CeilingHeight
                else
                    frontSideSector.CeilingHeight
                |> float32

            Some(createUpperFront p1 p2 bottom top)

        | Some frontSideSector, _ ->
            let bottom = float32 frontSideSector.FloorHeight
            let top = float32 frontSideSector.CeilingHeight

            Some(createUpperFront p1 p2 bottom top)

        | _ ->
            None

let tryInitializeLowerFront map linedef =
    let p1, p2 = linedef.Segment.A, linedef.Segment.B

    match (map, linedef) with
    | FrontBack(frontSideSectorOpt, backSideSectorOpt) ->
        match frontSideSectorOpt, backSideSectorOpt with

        | Some(frontSideSector), Some(backSideSector) ->
            if frontSideSector.FloorHeight < backSideSector.FloorHeight then

                let bottom, top = float32 frontSideSector.FloorHeight, float32 backSideSector.FloorHeight
                Some(createUpperFront p1 p2 bottom top)
            else
                None
        | _ ->
            None

let createFrontWall map linedef =
    {
        Upper = tryInitializeUpperFront map linedef
        Middle = tryInitializeMiddleFront map linedef
        Lower = tryInitializeLowerFront map linedef
    }

type WallSection =
    | Upper
    | Middle
    | Lower

type TextureAlignment =
    | UpperUnpegged of offsetY: int
    | LowerUnpegged

let getTextureAlignment (map: Map) (linedef: Linedef) (sidedef: Sidedef) (isFrontSide: bool) (section: WallSection) =
    let isLowerUnpegged = linedef.Flags.HasFlag(LinedefFlags.LowerTextureUnpegged)
    let isUpperUnpegged = linedef.Flags.HasFlag(LinedefFlags.UpperTextureUnpegged)

    let isTwoSided = linedef.FrontSidedefIndex.IsSome && linedef.BackSidedefIndex.IsSome

    match section with
    | Upper ->
        if not isUpperUnpegged then
            LowerUnpegged
        else
            UpperUnpegged 0
    | _ ->
        if isLowerUnpegged then
            if isTwoSided && section = WallSection.Lower then
                let frontSideSector = 
                    map.Sectors.[map.Sidedefs.[linedef.FrontSidedefIndex.Value].SectorNumber]

                let backSideSector =
                    map.Sectors.[map.Sidedefs.[linedef.BackSidedefIndex.Value].SectorNumber]


                if isFrontSide then
                    UpperUnpegged (abs (frontSideSector.CeilingHeight - backSideSector.FloorHeight))
                else
                    UpperUnpegged (abs (backSideSector.CeilingHeight - frontSideSector.FloorHeight))
            else
                LowerUnpegged
        else
            UpperUnpegged 0

let updateWallUv (sidedef: Sidedef) width height (vertices: Vector3 []) (textureAlignment: TextureAlignment) (uv: Vector2 []) =
    let textureOffsetX = sidedef.OffsetX
    let textureOffsetY = sidedef.OffsetY

    let mutable i = 0
    while (i < vertices.Length) do
        let p1 = vertices.[i]
        let p2 = vertices.[i + 1]
        let p3 = vertices.[i + 2]

        let width = single width
        let height = single height

        let v1 = Vector2 (p1.X, p1.Y)
        let v2 = Vector2 (p2.X, p2.Y)
        let v3 = Vector2 (p3.X, p3.Y)

        let one = 0.f + single textureOffsetX
        let two = (v2 - v1).Length ()

        let x, y, z1, z3 =

            // lower unpeg
            match textureAlignment with
            | LowerUnpegged ->
                let ofsY = single textureOffsetY / height * -1.f
                if p3.Z < p1.Z then
                    (one + two) / width, 
                    one / width, 
                    0.f - ofsY,
                    ((abs (p1.Z - p3.Z)) / height * -1.f) - ofsY
                else
                    one / width, 
                    (one + two) / width, 
                    ((abs (p1.Z - p3.Z)) / height * -1.f) - ofsY,
                    0.f - ofsY

            // upper unpeg
            | UpperUnpegged offsetY ->
                let z = single offsetY / height * -1.f
                let ofsY = single textureOffsetY / height * -1.f
                if p3.Z < p1.Z then
                    (one + two) / width, 
                    one / width, 
                    (1.f - ((abs (p1.Z - p3.Z)) / height * -1.f)) - z - ofsY,
                    1.f - z - ofsY
                else
                    one / width, 
                    (one + two) / width, 
                    1.f - z - ofsY,
                    (1.f - ((abs (p1.Z - p3.Z)) / height * -1.f)) - z - ofsY

        uv.[i] <- Vector2 (x, z3)
        uv.[i + 1] <- Vector2(y, z3)
        uv.[i + 2] <- Vector2(y, z1)

        i <- i + 3

let createWallUv (map: Map) (linedef: Linedef) (sidedef: Sidedef) width height (vertices: Vector3 []) (section: WallSection) =
    let textureAlignment = getTextureAlignment map linedef sidedef linedef.FrontSidedefIndex.IsSome section

    let uv = Array.zeroCreate vertices.Length
    updateWallUv sidedef width height vertices textureAlignment uv
    uv