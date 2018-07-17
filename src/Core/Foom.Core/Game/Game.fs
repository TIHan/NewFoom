namespace Foom.Game

open System
open Foom.Game.Input

[<AbstractClass>]
type AbstractServerGame() =

    abstract Update : time: TimeSpan * interval: TimeSpan -> bool

[<AbstractClass>]
type AbstractClientGame() =

    abstract PreUpdate : time: TimeSpan * interval: TimeSpan * inputEvents: InputEvent list -> unit

    abstract Update : time: TimeSpan * interval: TimeSpan * inputEvents: InputEvent list -> bool

    abstract Render : time: TimeSpan * deltaTime: float32 -> unit

[<Sealed>]
type Game(svGame: AbstractServerGame, clGame: AbstractClientGame, interval) =

    member __.Start() =
        GameLoop.start
            (float interval)
            id
            (fun time interval ->
                let time = TimeSpan.FromTicks(time)
                let interval = TimeSpan.FromTicks(interval)

                clGame.PreUpdate(time, interval, [])
                let svWillQuit = svGame.Update(time, interval)
                let clWillQuit = clGame.Update(time, interval, [])

                svWillQuit || clWillQuit
            )
            (fun time deltaTime ->
                let time = TimeSpan.FromTicks(time)

                clGame.Render(time, deltaTime)
            )
