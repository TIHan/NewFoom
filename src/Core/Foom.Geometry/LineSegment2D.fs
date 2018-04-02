namespace Foom.Geometry

open System.Numerics

[<Struct>]
type LineSegment2D = 

    val mutable A : Vector2
    val mutable B : Vector2

    new (a, b) = { A = a; B = b }

    // From Book: Real-Time Collision Detection
    // Modified, so it might not work :(
    member seg.Intersects(aabb: BoundingBox2D) =
        let a = seg.A
        let b = seg.B

        let c = aabb.Center
        let e = aabb.Extents
        let m = (a + b) * 0.5f
        let d = b - m
        let m = m - c

        let adx = abs d.X
        if abs m.X > e.X + adx then false
        else

        let ady = abs d.Y
        if abs m.Y > e.Y + ady then false
        else

        let adx = adx + System.Single.Epsilon
        let ady = ady + System.Single.Epsilon

        if abs(m.X * d.Y - m.Y * d.X) > e.X * ady + e.Y * adx then false
        else
            true

    member seg.BoundingBox() =
        let a = seg.A
        let b = seg.B

        let mutable minX = a.X
        let mutable maxX = a.X
        let mutable minY = a.Y
        let mutable maxY = a.Y

        if b.X < minX then minX <- b.X
        if b.X > maxX then maxX <- b.X
        if b.Y < minY then minY <- b.Y
        if b.Y > maxY then maxY <- b.Y

        BoundingBox2D.FromMinAndMax(Vector2(minX, minY), Vector2(maxX, maxY))

    member seg.FindClosestPoint(point: Vector2) =
        let a = seg.A
        let b = seg.B

        let ab = b - a

        let t = Vector2.Dot(point - a, ab)
        if (t <= 0.f) then
            struct(0.f, a)
        else
            let denom = Vector2.Dot(ab, ab)
            if (t >= denom) then
                struct(1.f, b)
            else
                let t = t / denom
                struct(t, a + (t * ab))

    member seg.Normal() =
        let dx = seg.B.X - seg.A.X
        let dy = seg.B.Y - seg.A.Y

        Vector2(dy, -dx)
        |> Vector2.Normalize

    member seg.CheckIsOnLeftSide(point: Vector2) =
        let v1 = seg.A
        let v2 = seg.B
        (v2.X - v1.X) * (point.Y - v1.Y) - (v2.Y - v1.Y) * (point.X - v1.X) > 0.f
