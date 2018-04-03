[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickle

type Mus =
    {
        Events: int [] // placeholder
    }

val Parse : LumpHeader -> Stream -> Mus
