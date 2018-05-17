namespace Foom.Core.Tasks

open System
open System.Threading
open System.Threading.Tasks
open System.Collections.Concurrent

type internal InternalMessage<'Message> =
    | Stop of AsyncReplyChannel<unit>
    | Message of 'Message

type TaskAgent<'Message>(handle: 'Message -> unit) =

    let mutable mbp = None

    member __.IsRunning = mbp.IsSome

    member __.Start() =
        if mbp.IsSome then
            failwith "Agent already running."

        mbp <-
            new MailboxProcessor<InternalMessage<'Message>>(fun inbox ->
                let rec loop () = async {
                    let! msg = inbox.Receive()
                    match msg with
                    | Stop(reply) -> reply.Reply()
                    | Message(msg) -> 
                        handle msg
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

    member __.Post(msg) =
        mbp
        |> Option.iter (fun mbp ->
            mbp.Post(InternalMessage.Message(msg))
        )
