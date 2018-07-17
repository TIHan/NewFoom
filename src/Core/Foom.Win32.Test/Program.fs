open System
open Foom.Game
open Foom.Win32

[<EntryPoint>]
let main argv =

    match Win32.CreateWindow "F# Win32" with
    | Ok(window) ->
        GameLoop.start 30. (fun _ -> ()) (fun _ _ -> false) (fun _ _ -> window.Update())
    | Error(id) -> printfn "Error %A" id
    0
