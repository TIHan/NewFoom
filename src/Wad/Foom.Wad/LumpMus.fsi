[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickle

val Parse : LumpHeader -> Stream -> byte[]
