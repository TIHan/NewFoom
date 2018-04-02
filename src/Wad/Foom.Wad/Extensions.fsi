[<AutoOpen>]
module Foom.Wad.Extensions

open System.Numerics
open System.Collections.Immutable

type Map with

    member ComputeAllSectorGeometry : unit -> ImmutableArray<SectorGeometry>

    member ComputeFrontWallGeometry : Linedef -> WallGeometry

    member CreateUpperWallUv : Linedef * Sidedef * width: int * height: int * vertices: Vector3 [] -> Vector2 []

    member CreateMiddleWallUv : Linedef * Sidedef * width: int * height: int * vertices: Vector3 [] -> Vector2 []

    member CreateLowerWallUv : Linedef * Sidedef * width: int * height: int * vertices: Vector3 [] -> Vector2 []

    static member CreateSectorUv : width: int * height: int * vertices: Vector3 [] -> Vector2 []

    static member CreateColors : lightLevel: int * length: int -> Vector4 []
