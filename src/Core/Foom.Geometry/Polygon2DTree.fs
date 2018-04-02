namespace Foom.Geometry

open System.Numerics
open System.Collections.Immutable

[<Sealed>]
type Polygon2DTree = 

    val Polygon : Polygon2D
    val Children : ImmutableArray<Polygon2DTree>

    new(polygon) = { Polygon = polygon; Children = ImmutableArray.Create() }

    new(polygon, children: Polygon2DTree seq) = 
        { Polygon = polygon; Children = children.ToImmutableArray() }
