namespace Foom.Geometry

open System.Numerics

[<Sealed>]
type Polygon2D =

    val Vertices : Vector2 []

    new : vertices: Vector2 seq -> Polygon2D

    member IsArrangedClockwise : unit -> bool

    member Contains : point: Vector2 -> bool

    member Count : int