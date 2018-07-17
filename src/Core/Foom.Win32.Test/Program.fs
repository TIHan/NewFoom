open System
open Foom.Game
open Foom.Win32

type Win32ServerGame() =
    inherit AbstractServerGame()

    override __.Update(_, _) = false
    
type Win32ClientGame() =
    inherit AbstractClientGame()

    override __.PreUpdate(_, _) = ()

    override __.Update(_, _) = false

    override __.Render(_, _) = ()

[<EntryPoint>]
let main argv =

    let game = Win32Game("F# Game", Win32ServerGame(), Win32ClientGame(), 30.)
    game.Start()
    0
