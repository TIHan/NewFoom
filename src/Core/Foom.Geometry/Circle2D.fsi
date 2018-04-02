namespace Foom.Geometry

open System.Numerics

[<Struct>]
type Circle2D =

    val mutable Center : Vector2
    val mutable Radius : float32

    new : center: Vector2 * radius: float32 -> Circle2D

    member BoundingBox : unit -> BoundingBox2D
