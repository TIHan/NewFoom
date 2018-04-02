[<RequireQualifiedAccess>]
module internal Foom.Wad.LinedefTracer

open System.Collections.Generic
open Foom.Geometry 

val compute : Map -> IDictionary<int, ResizeArray<Linedef>> -> sectorId: int -> Polygon2DTree list
