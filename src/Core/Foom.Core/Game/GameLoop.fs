// http://gafferongames.com/game-physics/fix-your-timestep/
module Foom.Game.GameLoop

    open System.Diagnostics
    open System.Threading
    open System.Collections.Concurrent

    type GameLoop<'T> = 
        private { 
            LastTime: int64
            UpdateTime: int64
            UpdateAccumulator: int64
            willQuit: bool 

            Stopwatch : Stopwatch
            Skip : int64
            TargetUpdateInterval : int64
        }

        member this.WillQuit = this.willQuit

    let create updateInterval =
        let targetUpdateInterval = (1000. / updateInterval) * 10000. |> int64
        let skip = (1000. / 5.) * 10000. |> int64

        let stopwatch = Stopwatch.StartNew ()

        {
            LastTime = 0L
            UpdateTime = 0L
            UpdateAccumulator = targetUpdateInterval
            willQuit = false

            Stopwatch = stopwatch
            Skip = skip
            TargetUpdateInterval = targetUpdateInterval
        }

    let tick (alwaysUpdate: unit -> unit) (update: int64 -> int64 -> bool) (render: int64 -> int64 -> bool) gl =
        let stopwatch = gl.Stopwatch
        let skip = gl.Skip
        let targetUpdateInterval = gl.TargetUpdateInterval

        let currentTime = stopwatch.Elapsed.Ticks
        let deltaTime =
            match currentTime - gl.LastTime with
            | x when x > skip -> skip
            | x -> x

        let updateAcc = gl.UpdateAccumulator + deltaTime

        let rec processUpdate gl =
            alwaysUpdate ()

            if gl.UpdateAccumulator >= targetUpdateInterval
            then
                let willQuit = update gl.UpdateTime targetUpdateInterval

                processUpdate
                    { gl with 
                        UpdateTime = gl.UpdateTime + targetUpdateInterval
                        UpdateAccumulator = gl.UpdateAccumulator - targetUpdateInterval
                        willQuit = willQuit
                    }
            else
                gl

        let processRender gl =
            let willQuit = render currentTime (gl.UpdateAccumulator / targetUpdateInterval)

            { gl with 
                LastTime = currentTime
                willQuit = willQuit
            }

        { gl with UpdateAccumulator = updateAcc }
        |> processUpdate
        |> processRender

    let start updateInterval (alwaysUpdate: unit -> unit) (update: int64 -> int64 -> bool) (render: int64 -> int64 -> bool) : unit =
        let gl = create updateInterval

        let rec loop gl =
            if not gl.willQuit then
                gl
                |> tick alwaysUpdate update render
                |> loop

        loop gl