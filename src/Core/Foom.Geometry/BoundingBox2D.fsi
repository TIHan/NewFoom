namespace Foom.Geometry

open System.Numerics

[<Struct>]
type BoundingBox2D =

    val mutable Center : Vector2
    val mutable Extents : Vector2

    new : center: Vector2 * extents: Vector2 -> BoundingBox2D

    member Min : unit -> Vector2

    member Max : unit -> Vector2

    member Intersects : BoundingBox2D -> bool

    member Contains : point: Vector2 -> bool

    member Merge : BoundingBox2D -> BoundingBox2D

    static member FromMinAndMax : min: Vector2 * max: Vector2 -> BoundingBox2D
