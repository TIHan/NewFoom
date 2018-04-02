namespace Foom.Geometry

open System.Numerics

[<Struct>]
type Circle2D =

    val mutable Center : Vector2
    val mutable Radius : float32

    new(center, radius) = 
        if radius < 0.f then
            failwith "Circle2D: Radius cannot be less than 0."
        { Center = center; Radius = radius }

    member circle.BoundingBox() =
        BoundingBox2D(circle.Center, Vector2(circle.Radius, circle.Radius))
