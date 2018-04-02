namespace Foom.Geometry

open System.Numerics

[<Struct>]
type LineSegment2D = 

    val mutable A : Vector2
    val mutable B : Vector2

    new : Vector2 * Vector2 -> LineSegment2D

    member Intersects : BoundingBox2D -> bool

    member BoundingBox : unit -> BoundingBox2D

    member FindClosestPoint : point: Vector2 -> struct(float32 * Vector2)

    member Normal : unit -> Vector2

    member CheckIsOnLeftSide : point: Vector2 -> bool
