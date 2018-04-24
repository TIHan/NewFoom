namespace Foom.Net

open System
open System.Net
open System.Diagnostics
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Message

type internal BackgroundClientInternalMessage =
    | SendPackets
    | Dispose of AsyncReplyChannel<unit>

type internal BackgroundClient(networkChannels) =
    let msgFactory = createMessageFactory networkChannels 1
    let client = new Client(msgFactory)
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

                        if inbox.CurrentQueueLength > 0 then
                            let! msg = inbox.Receive()
                            match msg with
                            | SendPackets ->
                                client.SendPackets()
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

        member __.SendPackets() =
            mp.Post(SendPackets)

        member __.CreateMessage() =
            msgFactory.CreateMessage()

        member __.ProcessMessages(f) =
            client.ProcessMessages(f)

        member __.OnException = exceptionEvent.Publish

        member __.GetBeforeSerializedEvent<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)>() =
            let typeId = msgFactory.GetTypeId<'T>()
            client.GetBeforeSerializedEvent(typeId)
            |> Event.map (fun msg -> 
                match msg with
                | :? 'T as msg -> msg
                | _ -> failwith "Should not happen. Message is of incorrect type."
            )

        member __.GetBeforeDeserializedEvent<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)>() =
            let typeId = msgFactory.GetTypeId<'T>()
            client.GetBeforeDeserializedEvent(typeId)
            |> Event.map (fun msg -> 
                match msg with
                | :? 'T as msg -> msg
                | _ -> failwith "Should not happen. Message is of incorrect type."
            )

    interface IDisposable with

        member __.Dispose() =
            mp.PostAndReply(fun x -> Dispose x)
            (mp :> IDisposable).Dispose()
