namespace Foom.Tasks

open System
open System.Diagnostics
open System.Threading
open System.Threading.Tasks
open System.Collections.Concurrent

type internal InternalMessage =
    | Stop of AsyncReplyChannel<unit>
    | Work of (unit -> unit)

type Reactor(f: unit -> unit) =

    let mutable mbp = None

    member __.IsRunning = mbp.IsSome

    member __.Start() =
        if mbp.IsSome then
            failwith "Reactor already running."

        mbp <-
            MailboxProcessor<InternalMessage>.Start(fun inbox ->
                let stopwatch = Stopwatch()
                let rec loop () = async {

                    f ()

                    if inbox.CurrentQueueLength = 0 then
                        if not stopwatch.IsRunning then
                            stopwatch.Start()
                        elif stopwatch.Elapsed.TotalMilliseconds >= 10. then
                            do! Async.Sleep(10)
                            
                        return! loop ()
                    else
                        stopwatch.Restart()

                        let! msg = inbox.Receive()
                        match msg with
                        | Stop(reply) -> reply.Reply()
                        | Work(f) -> 
                            f ()
                            return! loop ()
                }

                loop ()
            )
            |> Some

    member __.Stop() =
        mbp
        |> Option.iter (fun mbp ->
            mbp.PostAndReply(fun reply -> InternalMessage.Stop(reply))
        )

    member __.Enqueue(f) =
        mbp
        |> Option.iter (fun mbp ->
            mbp.Post(InternalMessage.Work(f))
        )
