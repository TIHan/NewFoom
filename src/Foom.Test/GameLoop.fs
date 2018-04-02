// http://gafferongames.com/game-physics/fix-your-timestep/
module GameLoop

    open System.Diagnostics
    open System.Threading
    open System.Collections.Concurrent

    type private GameLoopSynchronizationContext () =
        inherit SynchronizationContext ()

        let queue = ConcurrentQueue<unit -> unit> ()

        override this.Post (d, state) =
            queue.Enqueue (fun () -> d.Invoke (state))

        member this.Process () =
            let mutable msg = Unchecked.defaultof<unit -> unit>
            while queue.TryDequeue (&msg) do
                msg ()

    type GameLoop<'T> = 
        private { 
            LastTime: int64
            UpdateTime: int64
            UpdateAccumulator: int64
            WillQuit: bool 

            Context : GameLoopSynchronizationContext
            Stopwatch : Stopwatch
            Skip : int64
            TargetUpdateInterval : int64
        }

    let create updateInterval =
        let targetUpdateInterval = (1000. / updateInterval) * 10000. |> int64
        let skip = (1000. / 5.) * 10000. |> int64

        let stopwatch = Stopwatch.StartNew ()

        let ctx = GameLoopSynchronizationContext ()

        SynchronizationContext.SetSynchronizationContext ctx

        {
            LastTime = 0L
            UpdateTime = 0L
            UpdateAccumulator = targetUpdateInterval
            WillQuit = false

            Context = ctx
            Stopwatch = stopwatch
            Skip = skip
            TargetUpdateInterval = targetUpdateInterval
        }

    let tick (alwaysUpdate: unit -> unit) (update: int64 -> int64 -> bool) (render: int64 -> float32 -> unit) gl =
        let ctx = gl.Context
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
                ctx.Process ()
                let willQuit = update gl.UpdateTime targetUpdateInterval

                processUpdate
                    { gl with 
                        UpdateTime = gl.UpdateTime + targetUpdateInterval
                        UpdateAccumulator = gl.UpdateAccumulator - targetUpdateInterval
                        WillQuit = willQuit
                    }
            else
                gl

        let processRender gl =
            render currentTime (single gl.UpdateAccumulator / single targetUpdateInterval)

            { gl with 
                LastTime = currentTime
            }

        { gl with UpdateAccumulator = updateAcc }
        |> processUpdate
        |> processRender

    let start updateInterval (alwaysUpdate: unit -> unit) (update: int64 -> int64 -> bool) (render: int64 -> float32 -> unit) : unit =
        let gl = create updateInterval

        let rec loop gl =
            if not gl.WillQuit then
                gl
                |> tick alwaysUpdate update render
                |> loop

        loop gl