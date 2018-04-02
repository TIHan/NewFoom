[<RequireQualifiedAccess>]
module Foom.Wad.LinedefTracer

open System.Numerics
open System.Collections.Generic
open System.Collections.Immutable
open Foom.Geometry

[<Struct>]
type LineTrace =
    {
        Segment: LineSegment2D
        IsFrontSide: bool
    }

type LinedefTracer = 
    { 
        endVertex: Vector2
        currentVertex: Vector2
        currentLinedef: LineTrace
        linedefs: LineTrace list
        visitedLinedefs: ImmutableHashSet<LineTrace>
        path: LineTrace list 
        sectorId: int
    }

// http://stackoverflow.com/questions/1560492/how-to-tell-whether-a-point-is-to-the-right-or-left-side-of-a-line
let inline isPointOnLeftSide (v1: Vector2) (v2: Vector2) (p: Vector2) =
    (v2.X - v1.X) * (p.Y - v1.Y) - (v2.Y - v1.Y) * (p.X - v1.X) > 0.f

let isPointOnFrontSide (p: Vector2) (linedef: LineTrace) (tracer: LinedefTracer) =
    let seg = linedef.Segment
    let a = seg.A
    let b = seg.B

    if linedef.IsFrontSide
    then isPointOnLeftSide b a p
    else isPointOnLeftSide a b p

let toVertices sectorId (linedefs: LineTrace seq) =
    linedefs
    |> Seq.map (fun x -> 
        if x.IsFrontSide then x.Segment.A else x.Segment.B) 
    |> Array.ofSeq

let findClosestLinedef (linedefs: LineTrace list) =
    let s = linedefs |> List.minBy (fun x -> x.Segment.A.X)
    let e = linedefs |> List.minBy (fun x -> x.Segment.B.X)

    let v =
        if s.Segment.A.X <= e.Segment.B.X 
        then s.Segment.A
        else e.Segment.B

    match linedefs |> List.tryFind (fun x -> x.Segment.A.Equals v) with
    | None -> linedefs |> List.find (fun x -> x.Segment.B.Equals v)
    | Some linedef -> linedef

let inline nonVisitedLinedefs tracer = 
    tracer.linedefs |> List.filter (not << tracer.visitedLinedefs.Contains)

let visit (linedef: LineTrace) (tracer: LinedefTracer) =
    { tracer with
        currentVertex = if linedef.IsFrontSide then linedef.Segment.B else linedef.Segment.A
        currentLinedef = linedef
        visitedLinedefs = tracer.visitedLinedefs.Add linedef
        path = tracer.path @ [linedef] }  

let create sectorId linedefs =
    let linedef = findClosestLinedef linedefs

    { 
        endVertex = if linedef.IsFrontSide then linedef.Segment.A else linedef.Segment.B
        currentVertex = if linedef.IsFrontSide then linedef.Segment.B else linedef.Segment.A
        currentLinedef = linedef
        linedefs = linedefs
        visitedLinedefs = ImmutableHashSet<LineTrace>.Empty.Add linedef
        path = [linedef]
        sectorId = sectorId
    }

let inline isFinished tracer = tracer.currentVertex.Equals tracer.endVertex && tracer.path.Length >= 3

let inline currentDirection tracer =
    let v =
        if tracer.currentLinedef.IsFrontSide
        then tracer.currentLinedef.Segment.A
        else tracer.currentLinedef.Segment.B

    Vector2.Normalize (v - tracer.currentVertex)

let tryVisitNextLinedef tracer =
    if isFinished tracer
    then tracer, false
    else
        match
            tracer.linedefs
            |> List.filter (fun l -> 
                (tracer.currentVertex.Equals (if l.IsFrontSide then l.Segment.A else l.Segment.B)) && 
                not (tracer.visitedLinedefs.Contains l)) with
        | [] -> tracer, false
        | [linedef] -> visit linedef tracer, true
        | linedefs ->
            let dir = currentDirection tracer

            let linedef =
                linedefs
                |> List.minBy (fun l ->
                    let v = if l.IsFrontSide then l.Segment.B else l.Segment.A                       
                    let result = Vector2.Dot (dir, Vector2.Normalize (tracer.currentVertex - v))

                    if isPointOnFrontSide v tracer.currentLinedef tracer
                    then result
                    else 2.f + (result * -1.f))

            visit linedef tracer, true

let compute (map: Map) (linedefLookup: IDictionary<int, ResizeArray<Linedef>>) sectorId = 
    let rec f  (polygons: Polygon2DTree list) (originalTracer: LinedefTracer) (tracer: LinedefTracer) =
        match tryVisitNextLinedef tracer with
        | tracer, true -> f polygons originalTracer tracer
        | tracer, _ ->
            if isFinished tracer then

                let polygon = Polygon2DTree(Polygon2D(toVertices tracer.sectorId tracer.path))

                match nonVisitedLinedefs tracer with
                | [] -> polygon :: polygons
                | linedefs ->

                    let innerLinedefs, linedefs =
                        linedefs
                        |> List.partition (fun linedef ->
                            polygon.Polygon.Contains linedef.Segment.A &&
                            polygon.Polygon.Contains linedef.Segment.B
                        )

                      

                    let polygon =
                        if innerLinedefs.Length > 0 then
                            let tracer = create tracer.sectorId innerLinedefs
                            Polygon2DTree(polygon.Polygon, f [] tracer tracer)
                        else
                            polygon


                    match linedefs with
                    | [] -> polygon :: polygons
                    | linedefs ->
                        let tracer = create tracer.sectorId linedefs
                        f (polygon :: polygons) tracer tracer


            else
                match nonVisitedLinedefs originalTracer with
                | [] -> polygons // Used to return "polygons". Return nothing because something broke.
                           // We might need to handle it a better way.
                | linedefs ->
                    let tracer = create originalTracer.sectorId linedefs
                    f polygons tracer tracer

    let linedefs = ResizeArray ()
    linedefLookup.[sectorId] |> Seq.iter (fun x ->
        let canUseLinedef =
            if (x.FrontSidedefIndex.IsSome && x.BackSidedefIndex.IsSome) then
                let front = map.Sidedefs.[x.FrontSidedefIndex.Value]
                let back = map.Sidedefs.[x.BackSidedefIndex.Value]
                not (front.SectorNumber = back.SectorNumber)
            else
                not (x.FrontSidedefIndex.IsNone && x.BackSidedefIndex.IsNone) 

        if canUseLinedef then
            match x.FrontSidedefIndex with
            | Some frontIndex ->
                let front = map.Sidedefs.[frontIndex]
                linedefs.Add ({ Segment = x.Segment; IsFrontSide = front.SectorNumber = sectorId })
            | _ ->
                linedefs.Add ({ Segment = x.Segment; IsFrontSide = false })
    )

    let tracer =
        linedefs
        |> List.ofSeq
        |> create sectorId
                    
    f [] tracer tracer  
