[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickle

[<Sealed;NoEquality;NoComparison>]
type Mus

val Parse : LumpHeader -> Stream -> Mus

val GetMidiStream: LumpHeader -> Stream -> unit
