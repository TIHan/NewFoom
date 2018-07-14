[<RequireQualifiedAccess>]
module Foom.Geometry.EarClipping

open System
open System.Numerics

type Ray =
    {
        Origin: Vector2
        Direction: Vector2
    }

    member this.GetPoint (distance: float32) =
        this.Origin + (this.Direction * distance)

let inline isReflexVertex (prev: Vector2) (next: Vector2) (vertex: Vector2) =
    let p1 = prev - vertex
    let p2 = next - vertex
    Vector3.Cross(Vector3 (p1.X, p1.Y, 0.f), Vector3 (p2.X, p2.Y, 0.f)).Z < 0.f

let inline perpDot (left: Vector2) (right: Vector2) = left.X * right.Y - left.Y * right.X

let inline isPointOnLeftSide (v1: Vector2) (v2: Vector2) (p: Vector2) =
    (v2.X - v1.X) * (p.Y - v1.Y) - (v2.Y - v1.Y) * (p.X - v1.X) > 0.f

// https://rootllama.wordpress.com/2014/06/20/ray-line-segment-intersection-test-in-2d/
let rayIntersection (ray: Ray) (vertices: Vector2 []) =
    let mutable i = vertices.Length - 1
    let mutable j = 0

    let segmentsHit = ResizeArray<int * int * Vector2> ()

    while (j < vertices.Length) do

        let start' = vertices.[i]
        let end' = vertices.[j]
        let dir' = end' - start' |> Vector2.Normalize

        let v1 = ray.Origin - start'
        let v2 = end' - start'
        let v3 = Vector2 (-ray.Direction.Y, ray.Direction.X)

        let t1 = perpDot v2 v1 / Vector2.Dot (v2, v3)
        let t2 = Vector2.Dot (v1, v3) / Vector2.Dot (v2, v3)

        if (t1 >= 0.f && t2 >= 0.f && t2 <= 1.f) then
            segmentsHit.Add(i, j, ray.GetPoint (t1))

        i <- j
        j <- j + 1

    if segmentsHit.Count = 0 then
        None
    else

        let yopac =
            segmentsHit
            |> Seq.filter (fun (i, j, p) ->
                isPointOnLeftSide vertices.[j] vertices.[i] ray.Origin
            )
            |> Seq.toArray


        let jopac =
            yopac
            |> Seq.sortByDescending (fun (_, j, _) ->
                j
            )
            |> Seq.toArray

        jopac
        |> Seq.sortBy (fun (i, j, p) ->
            Vector2.Dot (vertices.[i] - vertices.[j] |> Vector2.Normalize, ray.Direction)
        )
        |> Seq.minBy (fun (i, j, p) ->
            (ray.Origin - p).Length()
        )
        |> Some

let computeVertices (vertices: Vector2 seq) f =

    let rec computeVertices (recursiveSteps: int) (vertices: Vector2 ResizeArray) currentIndex = 
        if recursiveSteps > vertices.Count then
            failwith "Unable to triangulate"
            ()
        else

        if vertices.Count < 3 then
            ()
        elif vertices.Count = 3 then
            f vertices.[2] vertices.[1] vertices.[0]
        else

        let pPrev =
            if currentIndex = 0 then
                vertices.[vertices.Count - 1]
            else
                vertices.[currentIndex - 1]

        let pCur = vertices.[currentIndex]

        let pNext =
            if currentIndex = (vertices.Count - 1) then
                vertices.[0]
            else
                vertices.[currentIndex + 1]

        let triangle = Triangle2D(pNext, pCur, pPrev)

        let anyPointsInsideTriangle =
            vertices
            |> Seq.exists (fun x ->
                (x <> pPrev) && (x <> pCur) && (x <> pNext) && triangle.Contains x
            )

        if isReflexVertex pPrev pNext pCur || anyPointsInsideTriangle then
            let nextIndex =
                if currentIndex >= (vertices.Count - 1) then
                    0
                else
                    currentIndex + 1
            computeVertices (recursiveSteps + 1) (vertices) nextIndex
        else
            vertices.RemoveAt(currentIndex)

            let nextIndex =
                if currentIndex >= (vertices.Count - 1) then
                    0
                else
                    currentIndex + 1

            f pNext pCur pPrev
            computeVertices 0 vertices nextIndex

    computeVertices 0 (ResizeArray (vertices)) 0

let compute (polygon: Polygon2D) =

    let triangles = ResizeArray<Triangle2D> (polygon.Vertices.Length / 3)

    computeVertices polygon.Vertices (fun x y z ->
        triangles.Add (Triangle2D (x, y, z))
    )

    triangles
    |> Seq.toArray

let computeMultiple polygons =
    polygons
    |> List.map compute

let rec decomposeTree (tree: Polygon2DTree) =

    let mutable vertices = tree.Polygon.Vertices

    let polygons = ResizeArray<Polygon2D list> ()

    tree.Children
    |> Seq.sortByDescending (fun childTree ->
        let v =
            childTree.Polygon.Vertices
            |> Array.maxBy(fun x -> x.X)
        v.X
    )
    |> Seq.iteri (fun i childTree ->

        childTree.Children 
        |> Seq.map decomposeTree
        |> polygons.AddRange

        if true then

            let childMax =
                childTree.Polygon.Vertices
                |> Array.maxBy (fun x -> x.X)

            let ray = { Origin = childMax; Direction = Vector2.UnitX }

            match rayIntersection ray vertices with
            | Some (edge1Index, edge2Index, pt) ->

                let mutable edge2Index =
                    if vertices.[edge1Index].X > vertices.[edge2Index].X then
                        edge1Index
                    else
                        edge2Index

                let mutable replaceIndex = ValueNone
                let childMaxIndex = 
                    childTree.Polygon.Vertices |> Array.findIndex (fun x -> x = childMax)

                let v1 = pt
                let v2 = childTree.Polygon.Vertices.[childMaxIndex]
                let v3 = vertices.[edge2Index]

                let reflexes = Array.zeroCreate vertices.Length
                for i = 0 to vertices.Length - 1 do
                    let prev =
                        if i = 0 then
                            vertices.Length - 1
                        else
                            i - 1

                    let next =
                        if i = vertices.Length - 1 then
                            0
                        else
                            i + 1

                    reflexes.[i] <- isReflexVertex vertices.[prev] vertices.[next] vertices.[i]

                let mutable minValue = System.Single.MaxValue
                for i = 0 to vertices.Length - 1 do
                    let x = vertices.[i]
                    if x <> v1 && x <> v2 && x <> v3 && Triangle2D.Contains(v1, v2, v3, x) && reflexes.[i] then
                        let possibleMinValue = (ray.Origin - x).Length()
                        if possibleMinValue < minValue then
                            minValue <- possibleMinValue
                            replaceIndex <- ValueSome i

                let linkedList = vertices |> System.Collections.Generic.List

                if not (childTree.Polygon.IsArrangedClockwise()) then
                    failwith "butt"

                let mutable ii = childMaxIndex
                let mutable count = 0
                let linkedList2 = System.Collections.Generic.List ()

                match replaceIndex with
                | ValueNone ->
                    linkedList2.Add(vertices.[edge2Index])
                | ValueSome index ->
                    linkedList2.Add(vertices.[index])

                let childCount = childTree.Polygon.Count
                while (count < childCount) do
                    linkedList2.Add(childTree.Polygon.Vertices.[ii])
                    ii <-
                        if ii + 1 >= childCount then
                            0
                        else
                            ii + 1
                    count <- count + 1


                linkedList2.Add(childMax)
                match replaceIndex with
                | ValueNone ->
                    linkedList.InsertRange(edge2Index, linkedList2)
                | ValueSome index ->
                    linkedList.InsertRange(index, linkedList2)

                vertices <- (linkedList |> Seq.toArray)


            | _ -> ()
    )  

    [ Polygon2D vertices ]
    |> polygons.Add

    polygons |> Seq.reduce List.append

let computeTree (tree: Polygon2DTree) =

    if tree.Children.IsEmpty then
        [ compute tree.Polygon ] |> List.toSeq
    else
        let vertices = 
            match computeMultiple (decomposeTree tree) with
            | [] -> [||]
            | polygons ->
                polygons
                |> List.reduce Array.append
                |> Array.map (fun x -> [|x.P1;x.P2;x.P3|])
                |> Array.reduce Array.append

        let triangles = ResizeArray<Triangle2D> (vertices.Length / 3)
        let mutable i = 0
        while (i < vertices.Length) do
            Triangle2D (
                vertices.[i],
                vertices.[i + 1],
                vertices.[i + 2]
            )
            |> triangles.Add
            i <- i + 3

        [
            triangles
            |> Seq.toArray
        ]
        |> List.toSeq
