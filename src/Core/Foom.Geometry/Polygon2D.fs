namespace Foom.Geometry

open System.Numerics

[<AutoOpen>]
module Polygon2DHelpers =

    let sign = function
        | x when x <= 0.f -> false
        | _ -> true

    let cross v1 v2 =
        Vector3.Cross(Vector3(v1, 0.f), Vector3(v2, 0.f)).Z

[<Sealed>]
type Polygon2D =

    val Vertices : Vector2 []

    new(vertices: Vector2 seq) = 
        if Seq.length vertices <= 2 then
            failwith "Polygon2D: A polygon requires 3 or more vertices."
        { Vertices = Seq.toArray vertices }

    member poly.IsArrangedClockwise() =
        let vertices = poly.Vertices
        let length = vertices.Length

        vertices
        |> Array.mapi(fun i y ->
            let x =
                match i with
                | 0 -> vertices.[length - 1]
                | _ -> vertices.[i - 1]
            cross x y
        )                
        |> Array.reduce(+)
        |> sign

    // http://alienryderflex.com/polygon/
    member poly.Contains(point: Vector2) =
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

    member this.Count = this.Vertices.Length
