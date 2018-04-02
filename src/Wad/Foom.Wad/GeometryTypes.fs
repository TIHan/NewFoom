namespace Foom.Wad

open System.Numerics

[<Struct>]
type WallGeometry =
    {
        Upper: Vector3 [] option
        Middle: Vector3 [] option
        Lower: Vector3 [] option
    }

[<Struct>]
type SectorGeometry =
    {
        FloorVertices: Vector3 []
        CeilingVertices: Vector3 []
    }
