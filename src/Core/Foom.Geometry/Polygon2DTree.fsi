namespace Foom.Geometry

open System.Numerics
open System.Collections.Immutable

[<Sealed>]
type Polygon2DTree = 

    val Polygon : Polygon2D
    val Children : ImmutableArray<Polygon2DTree>

    new : Polygon2D -> Polygon2DTree

    new : Polygon2D * children: Polygon2DTree seq -> Polygon2DTree
