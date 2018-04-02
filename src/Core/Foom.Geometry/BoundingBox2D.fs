namespace Foom.Geometry

open System.Numerics

[<Struct>]
type BoundingBox2D =

    val mutable Center : Vector2
    val mutable Extents : Vector2

    new(center, extents) = { Center = center; Extents = extents }

    member this.Min() = this.Center - this.Extents

    member this.Max() = this.Center + this.Extents

    member a.Intersects(b: BoundingBox2D) =
        if      abs (a.Center.X - b.Center.X) > (a.Extents.X + b.Extents.X) then false
        elif    abs (a.Center.Y - b.Center.Y) > (a.Extents.Y + b.Extents.Y) then false
        else    true

    member b.Contains(point: Vector2) =
        let d = b.Center - point

        abs d.X <= b.Extents.X &&
        abs d.Y <= b.Extents.Y

    // TODO: Optimize this.
    member a.Merge(b: BoundingBox2D) =
        let minA = a.Min ()
        let maxA = a.Max ()
        let minB = b.Min ()
        let maxB = b.Max ()

        let mutable minX = minA.X
        let mutable maxX = maxA.X
        let mutable minY = minA.Y
        let mutable maxY = maxA.Y

        if (minB.X < minX) then minX <- minB.X
        if (minB.Y < minY) then minY <- minB.Y
        if (maxB.X > maxX) then maxX <- maxB.X
        if (maxB.Y > maxY) then maxY <- maxB.Y

        BoundingBox2D.FromMinAndMax(Vector2(minX, minY), Vector2(maxX, maxY))

    static member FromMinAndMax(min: Vector2, max: Vector2) =
        BoundingBox2D((min + max) * 0.5f, (min - max) * 0.5f |> abs)
