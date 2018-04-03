open System
open Foom.Wad

[<EntryPoint>]
let main argv =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let music = wad.FindMusic "d_e1m1"
    0
