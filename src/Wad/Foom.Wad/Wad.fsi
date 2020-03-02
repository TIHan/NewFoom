namespace Foom.Wad

open System
open System.IO

type Texture =
    {
        Data: Pixel [,]
        Name: string
    }

    member Width : int

    member Height : int

[<Sealed>]
type Wad =

    member TryFindFlatTexture : name: string -> Texture option

    member TryFindTexture : name: string -> Texture option

    member TryFindSpriteTexture : name: string -> Texture option

    member FindMap : name: string -> Map

    member TryFindMusic : name: string -> byte[] option

    member OverrideFromFile : fileName: string -> unit

    static member FromFile : fileName: string -> Wad

    static member FromStream : stream: Stream -> Wad

    interface IDisposable

