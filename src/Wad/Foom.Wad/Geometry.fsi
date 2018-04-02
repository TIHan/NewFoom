namespace Foom.Wad.Geometry

open System.Numerics

[<Struct>]
type Triangle2D =

    val X : Vector2

    val Y : Vector2

    val Z : Vector2

    new : Vector2 * Vector2 * Vector2 -> Triangle2D

type Polygon2D =
    {
        Vertices: Vector2 []
    } 

[<CompilationRepresentationAttribute (CompilationRepresentationFlags.ModuleSuffix)>]
module Polygon2D =

    val create : vertices: Vector2 [] -> Polygon2D

    val isArrangedClockwise : poly: Polygon2D -> bool

    val isPointInside : point: Vector2 -> poly: Polygon2D -> bool

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

    member Contains : Vector2 -> ContainmentType

    member Intersects : BoundingBox2D -> ContainmentType
