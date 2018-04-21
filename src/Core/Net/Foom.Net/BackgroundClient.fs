namespace Foom.Net

open System
open System.Net
open System.Diagnostics
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Message

type internal BackgroundClientInternalMessage =
    | Dispose of AsyncReplyChannel<unit>

type internal BackgroundClient(msgReg, channelLookupFactory) =
    let client = new Client(msgReg, channelLookupFactory)
    let exceptionEvent = Event<Exception>()

    let msgHash = HashSet()

    let mp = MailboxProcessor<BackgroundClientInternalMessage>.Start(fun inbox -> 

        let rec start (stopwatch: Stopwatch) =
            async {
                let mutable replyDispose = Unchecked.defaultof<AsyncReplyChannel<unit>>
                let mutable willDispose = false
                while not willDispose do
                    let time = stopwatch.Elapsed
                    client.Time <- time

                    try
                        client.ReceivePackets()
                        client.SendPackets()

                        if inbox.CurrentQueueLength > 0 then
                            let! msg = inbox.Receive()
                            match msg with
                            | Dispose(reply) ->
                                replyDispose <- reply
                                willDispose <- true
                        else
                            do! Async.Sleep(1)
                    with
                    | ex -> 
                        client.Disconnect()
                        exceptionEvent.Trigger(ex)
                        do! Async.Sleep(1)

                (client :> IDisposable).Dispose()
                replyDispose.Reply()
            }

        start (Stopwatch.StartNew())
    )

    interface IBackgroundClient with

        member __.Connect(address, port) =
            client.Connect(address, port)

        member __.Disconnect() =
            client.Disconnect()

        member __.SendMessage(msg) =
            client.SendMessage(msg, willRecycle = true)

        member __.CreateMessage() =
            client.CreateMessage()

        member __.ProcessMessages(f) =
            client.ProcessMessages(f)

        member __.OnException = exceptionEvent.Publish

    interface IDisposable with

        member __.Dispose() =
            mp.PostAndReply(fun x -> Dispose x)
            (mp :> IDisposable).Dispose()
