namespace FsGame.Resources

open System
open System.IO
open System.Collections.Generic
open Foom.Wad
open FsGame.Core.Collections

[<NoEquality;NoComparison;RequireQualifiedAccess>]
type internal Resource =
    | Wad of Wad

type ResourceManager internal () =

    let resources = Dictionary<string, Resource>()

    member _.ImportResource(filePath: string, name: string) =
        resources.[name] <- Resource.Wad(Wad.FromFile filePath)

    member internal _.GetResource(name: string) =
        resources.[name]
