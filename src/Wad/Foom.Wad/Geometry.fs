namespace Foom.Wad.Geometry

open System
open System.Numerics

// https://github.com/MonoGame/MonoGame
// Some source from MonoGame converted to F#. 
// Thanks MonoGame for your hard work.

[<Struct>]
type Triangle2D =

    val X : Vector2

    val Y : Vector2

    val Z : Vector2

    new (x, y, z) = { X = x; Y = y; Z = z }

type Polygon2D =
    {
        Vertices: Vector2 []
    }

[<CompilationRepresentationAttribute (CompilationRepresentationFlags.ModuleSuffix)>]
module Polygon2D =

    let create vertices = { Vertices = vertices }

    let sign = function
        | x when x <= 0.f -> false
        | _ -> true

    let inline cross v1 v2 = (Vector3.Cross (Vector3(v1, 0.f), Vector3(v2, 0.f))).Z
        
    let isArrangedClockwise poly =
        let vertices = poly.Vertices
        let length = vertices.Length

        vertices
        |> Array.mapi (fun i y ->
            let x =
                match i with
                | 0 -> vertices.[length - 1]
                | _ -> vertices.[i - 1]
            cross x y)                
        |> Array.reduce ((+))
        |> sign

    // http://alienryderflex.com/polygon/
    let isPointInside (point: Vector2) poly =
        let vertices = poly.Vertices
        let mutable j = vertices.Length - 1
        let mutable c = false

        for i = 0 to vertices.Length - 1 do
            let xp1 = vertices.[i].X
            let xp2 = vertices.[j].X
            let yp1 = vertices.[i].Y
            let yp2 = vertices.[j].Y

            if
                ((yp1 > point.Y) <> (yp2 > point.Y)) &&
                (point.X < (xp2 - xp1) * (point.Y - yp1) / (yp2 - yp1) + xp1) then
                c <- not c
            else ()

            j <- i
        c

type Polygon2DTree = 
    {
        Polygon: Polygon2D
        Children: Polygon2DTree list
    }

type ContainmentType =
    | Disjoint
    | Contains
    | Intersects

type BoundingBox2D =
    {
        Min: Vector2
        Max: Vector2
    }

    member this.Contains (point: Vector2) =
        //first we get if point is out of box
        if (point.X < this.Min.X
            || point.X > this.Max.X
            || point.Y < this.Min.Y
            || point.Y > this.Max.Y) then
            ContainmentType.Disjoint

        //or if point is on box because coordonate of point is lesser or equal
        elif (point.X = this.Min.X
            || point.X = this.Max.X
            || point.Y = this.Min.Y
            || point.Y = this.Max.Y) then
            ContainmentType.Intersects
        else
            ContainmentType.Contains

    member this.Intersects b =
        //test if all corner is in the same side of a face by just checking min and max
        if (b.Max.X < this.Min.X
            || b.Min.X > this.Max.X
            || b.Max.Y < this.Min.Y
            || b.Min.Y > this.Max.Y) then
            ContainmentType.Disjoint

        elif (b.Min.X >= this.Min.X
            && b.Max.X <= this.Max.X
            && b.Min.Y >= this.Min.Y
            && b.Max.Y <= this.Max.Y) then
            ContainmentType.Contains

        else
            ContainmentType.Intersects

