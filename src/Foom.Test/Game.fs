namespace Foom.Game

open System

[<Sealed>]
type Game(svGame: AbstractServerGame, clGame: AbstractClientGame, interval) =

    member __.Start() =
        GameLoop.start
            (float interval)
            id
            (fun time interval ->
                let time = TimeSpan.FromTicks(time)
                let interval = TimeSpan.FromTicks(interval)

                clGame.PreUpdate(time, interval)
                let svWillQuit = svGame.Update(time, interval)
                let clWillQuit = clGame.Update(time, interval)

                svWillQuit || clWillQuit
            )
            (fun time deltaTime ->
                let time = TimeSpan.FromTicks(time)

                clGame.Render(time, deltaTime)
            )
