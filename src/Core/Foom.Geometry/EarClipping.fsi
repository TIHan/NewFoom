[<RequireQualifiedAccess>]
module Foom.Geometry.EarClipping

val compute : Polygon2D -> Triangle2D []

val computeTree : Polygon2DTree -> Triangle2D [] seq