[<AutoOpen>]
module Foom.Wad.Extensions

open System.Numerics
open System.Collections.Generic
open System.Collections.Immutable
open Foom.Geometry

let computeSectorTriangle map linedefLookup sectorIndex =
    let polyTrees = LinedefTracer.compute map linedefLookup sectorIndex
    try
        let results =
            polyTrees
            |> List.map EarClipping.computeTree

        if List.isEmpty results then
            Array.empty
        else
            results
            |> List.reduce Seq.append
            |> Seq.reduce Array.append
    with | ex ->
        failwithf "Failed sector %i. %A" sectorIndex ex

let createLinedefLookup (map: Map) =
    let dict = Dictionary()

    let add sectorIndex linedef =
        let linedefs =
            match dict.TryGetValue sectorIndex with
            | false, _ -> 
                let linedefs = ResizeArray()
                dict.Add(sectorIndex, linedefs)
                linedefs
            | _, linedefs ->
                linedefs
      
        linedefs.Add linedef

    map.Linedefs
    |> Seq.iter(fun linedef ->
        match linedef.FrontSidedefIndex with
        | Some i -> add map.Sidedefs.[i].SectorNumber linedef
        | _ -> ()

        match linedef.BackSidedefIndex with
        | Some i -> add map.Sidedefs.[i].SectorNumber linedef
        | _ -> ()
    )

    dict

let computeSectorFloorVertices sector (triangles: Triangle2D []) =
    let z = float32 sector.FloorHeight
    let vertices = Array.zeroCreate (triangles.Length * 3)

    let mutable index = 0
    triangles
    |> Array.iter(fun tri ->
        vertices.[index] <-     Vector3(tri.P1, z)
        vertices.[index + 1] <- Vector3(tri.P2, z)
        vertices.[index + 2] <- Vector3(tri.P3, z)
        index <- index + 3
    )

    vertices

let computeSectorCeilingVertices sector (triangles: Triangle2D []) =
    let z = float32 sector.CeilingHeight
    let vertices = Array.zeroCreate (triangles.Length * 3)

    let mutable index = 0
    triangles
    |> Array.iter(fun tri ->
        vertices.[index + 2] <- Vector3(tri.P1, z)
        vertices.[index + 1] <- Vector3(tri.P2, z)
        vertices.[index] <-     Vector3(tri.P3, z)
        index <- index + 3
    )

    vertices

let inline updateSectorUv width height (vertices: Vector3 []) (uv: Vector2 []) =
    let width = single width
    let height = single height * -1.f

    let mutable i = 0
    while i < vertices.Length do

        let v1 = vertices.[i]
        let v2 = vertices.[i + 1]
        let v3 = vertices.[i + 2]

        uv.[i] <- Vector2 (v1.X / width, v1.Y / height)
        uv.[i + 1] <- Vector2 (v2.X / width, v2.Y / height)
        uv.[i + 2] <- Vector2 (v3.X / width, v3.Y / height)

        i <- i + 3

let inline createSectorUv width height (vertices: Vector3 []) =
    let uv = Array.zeroCreate (vertices.Length)
    updateSectorUv width height vertices uv
    uv

type Map with

    member this.ComputeAllSectorGeometry() =
        let linedefLookup = createLinedefLookup this
        let geos =
            this.Sectors
            |> Seq.mapi(fun i sector ->
                let triangles = computeSectorTriangle this linedefLookup i 

                let floorVertices =
                    computeSectorFloorVertices sector triangles

                let ceilingVertices =
                    computeSectorCeilingVertices sector triangles

                {
                    FloorVertices = floorVertices
                    CeilingVertices = ceilingVertices
                }
            )
       
        geos.ToImmutableArray()

    member this.ComputeFrontWallGeometry(linedef: Linedef) =
        createFrontWall this linedef

    //member this.CreateWallGeometry (linedef: Linedef) : (Vector3 [] * Vector3 [] * Vector3 []) * (Vector3 [] * Vector3 [] * Vector3 []) =
        //let seg = linedef.Segment
        //let a = seg.A
        //let b = seg.B

        //let getSector = fun sectorNumber -> this.Sectors.[sectorNumber]

        //// Upper Front
        //let upperFront = createUpperFront a b linedef getSector 

        //// Middle Front
        //let mutable middleFront = [||]
        //linedef.FrontSidedef
        //|> Option.iter (fun frontSide ->
        //    let frontSideSector = getSector frontSide.SectorNumber

        //    let floorHeight, ceilingHeight =
        //        match linedef.BackSidedef with
        //        | Some backSide ->
        //            let backSideSector = getSector backSide.SectorNumber

        //            (
        //                (
        //                    if backSideSector.FloorHeight > frontSideSector.FloorHeight then
        //                        backSideSector.FloorHeight
        //                    else
        //                        frontSideSector.FloorHeight
        //                ),
        //                (
        //                    if backSideSector.CeilingHeight < frontSideSector.CeilingHeight then
        //                        backSideSector.CeilingHeight
        //                    else
        //                        frontSideSector.CeilingHeight
        //                )
        //            )

        //        | _ -> 
        //            frontSideSector.FloorHeight, frontSideSector.CeilingHeight

        //    middleFront <-
        //        [|
        //            Vector3 (a, single floorHeight)
        //            Vector3 (b, single floorHeight)
        //            Vector3 (b, single ceilingHeight)

        //            Vector3 (b, single ceilingHeight)
        //            Vector3 (a, single ceilingHeight)
        //            Vector3 (a, single floorHeight)
        //        |]
        //)

        //// Lower Front
        //let mutable lowerFront = [||]
        //linedef.FrontSidedef
        //|> Option.iter (fun frontSide ->
        //    linedef.BackSidedef
        //    |> Option.iter (fun backSide ->
        //        let frontSideSector = getSector frontSide.SectorNumber
        //        let backSideSector = getSector backSide.SectorNumber

        //        if frontSideSector.FloorHeight < backSideSector.FloorHeight then

        //            let floorHeight, ceilingHeight = frontSideSector.FloorHeight, backSideSector.FloorHeight

        //            lowerFront <-
        //                [|
        //                    Vector3 (a, single floorHeight)
        //                    Vector3 (b, single floorHeight)
        //                    Vector3 (b, single ceilingHeight)

        //                    Vector3 (b, single ceilingHeight)
        //                    Vector3 (a, single ceilingHeight)
        //                    Vector3 (a, single floorHeight)
        //                |]
        //    )
        //)

        //// Upper Back
        //let mutable upperBack = [||]
        //linedef.BackSidedef
        //|> Option.iter (fun backSide ->
        //    linedef.FrontSidedef
        //    |> Option.iter (fun frontSide ->
        //        let backSideSector = getSector backSide.SectorNumber
        //        let frontSideSector = getSector frontSide.SectorNumber

        //        if frontSideSector.CeilingHeight < backSideSector.CeilingHeight then

        //            let floorHeight, ceilingHeight = frontSideSector.CeilingHeight, backSideSector.CeilingHeight

        //            upperBack <-
        //                [|
        //                    Vector3 (b, single floorHeight)
        //                    Vector3 (a, single floorHeight)
        //                    Vector3 (a, single ceilingHeight)

        //                    Vector3 (a, single ceilingHeight)
        //                    Vector3 (b, single ceilingHeight)
        //                    Vector3 (b, single floorHeight)
        //                |]
        //    )
        //)

        //// Middle Back
        //let mutable middleBack = [||]
        //linedef.BackSidedef
        //|> Option.iter (fun backSide ->
        //    let backSideSector = getSector backSide.SectorNumber

        //    let floorHeight, ceilingHeight =
        //        match linedef.FrontSidedef with
        //        | Some frontSide ->
        //            let frontSideSector = getSector frontSide.SectorNumber

        //            (
        //                (
        //                    if frontSideSector.FloorHeight > backSideSector.FloorHeight then
        //                        frontSideSector.FloorHeight
        //                    else
        //                        backSideSector.FloorHeight
        //                ),
        //                (
        //                    if frontSideSector.CeilingHeight < backSideSector.CeilingHeight then
        //                        frontSideSector.CeilingHeight
        //                    else
        //                        backSideSector.CeilingHeight
        //                )
        //            )

        //        | _ -> backSideSector.FloorHeight, backSideSector.CeilingHeight

        //    middleBack <-
        //        [|
        //            Vector3 (b, single floorHeight)
        //            Vector3 (a, single floorHeight)
        //            Vector3 (a, single ceilingHeight)

        //            Vector3 (a, single ceilingHeight)
        //            Vector3 (b, single ceilingHeight)
        //            Vector3 (b, single floorHeight)
        //        |]
        //)

        //// Lower Front
        //let mutable lowerBack = [||]
        //linedef.BackSidedef
        //|> Option.iter (fun backSide ->
        //    linedef.FrontSidedef
        //    |> Option.iter (fun frontSide ->
        //        let backSideSector = getSector backSide.SectorNumber
        //        let frontSideSector = getSector frontSide.SectorNumber

        //        if frontSideSector.FloorHeight > backSideSector.FloorHeight then

        //            let floorHeight, ceilingHeight = backSideSector.FloorHeight, frontSideSector.FloorHeight

        //            lowerBack <-
        //                [|
        //                    Vector3 (b, single floorHeight)
        //                    Vector3 (a, single floorHeight)
        //                    Vector3 (a, single ceilingHeight)

        //                    Vector3 (a, single ceilingHeight)
        //                    Vector3 (b, single ceilingHeight)
        //                    Vector3 (b, single floorHeight)
        //                |]
        //    )
        //)

        //(
        //    (upperFront, middleFront, lowerFront),
        //    (upperBack, middleBack, lowerBack)
        //)

    member this.CreateUpperWallUv(linedef, sidedef, width, height, vertices) =
        createWallUv this linedef sidedef width height vertices WallSection.Upper

    member this.CreateMiddleWallUv(linedef, sidedef, width, height, vertices) =
        createWallUv this linedef sidedef width height vertices WallSection.Middle

    member this.CreateLowerWallUv(linedef, sidedef, width, height, vertices) =
        createWallUv this linedef sidedef width height vertices WallSection.Lower

    static member CreateSectorUv(width, height, vertices) =
        createSectorUv width height vertices

    static member CreateColors(lightLevel, length) =
        Array.init length (fun _ -> Vector4(single lightLevel / 255.f, single lightLevel / 255.f, single lightLevel / 255.f, 255.f))
