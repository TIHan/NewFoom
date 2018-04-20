namespace Foom.Net

open System
open System.Collections.Generic 
open System.Collections.Concurrent
open System.Diagnostics
open Foom.IO.Message

type BackgroundServerInternalMessage =
    | Start
    | Stop
    | Dispose of AsyncReplyChannel<unit>

type BackgroundServer(msgReg, channelLookupFactory, port, maxClients) =
    let msgHash = HashSet()

    let server = new Server(msgReg, channelLookupFactory, port, maxClients)
    let exceptionEvent = Event<Exception>()

    let mutable localClientOpt : (Peer * ConcurrentQueue<struct(ClientId * ClientMessage)>) option = None

    let mutable receivedLocalClientMsgsOpt : ConcurrentQueue<ServerMessage> option = None

    let mp = MailboxProcessor<BackgroundServerInternalMessage>.Start(fun inbox -> 

        let rec start (stopwatch: Stopwatch) =
            let mutable sendTime = TimeSpan.Zero
            async {
                let mutable replyDispose = Unchecked.defaultof<AsyncReplyChannel<unit>>
                let mutable willDispose = false
                while not willDispose do
                    try 
                        server.ReceivePackets()
                        server.SendPackets()

                        if inbox.CurrentQueueLength > 0 then
                            let! msg = inbox.Receive()
                            match msg with
                            | Start -> 
                                server.Start()
                            | Stop -> 
                                server.Stop()
                            | Dispose(reply) ->
                                replyDispose <- reply
                                willDispose <- true
                        else
                            do! Async.Sleep(1)
                    with
                    | ex -> 
                        server.Stop()
                        exceptionEvent.Trigger(ex)
                        do! Async.Sleep(1)

                (server :> IDisposable).Dispose()
                replyDispose.Reply()
            }

        start (Stopwatch.StartNew())
    )

    member __.Start() =
        mp.Post(Start)

    member __.Stop() =
        mp.Post(Stop)

    member __.CreateMessage<'T when 'T :> Message>() =
        server.CreateMessage<'T>()

    member __.SendMessage(msg, channelId) =
        let willRecycle =
            match localClientOpt with
            | Some(_, localClientReceiveQueue) -> 
                localClientReceiveQueue.Enqueue struct(ClientId.Local, ClientMessage.Message(msg))
                false
            | _ ->
                true

        server.SendMessage(msg, channelId, willRecycle)

    member __.SendMessage(msg, channelId, clientId: ClientId) =
        match localClientOpt with
        | Some(_, localClientReceiveQueue) when clientId.IsLocal ->
            localClientReceiveQueue.Enqueue struct(ClientId.Local, ClientMessage.Message(msg))
        | _ ->
            server.SendMessage(msg, channelId, clientId, willRecycle = true)

    member __.ListenForMessage<'T when 'T :> Message>() =
        if msgHash.Add(typeof<'T>) then
            server.MessageReceived<'T>().Add(fun _ -> ())

    member __.ProcessMessages(f) =
        let mutable msg = Unchecked.defaultof<_>
        match receivedLocalClientMsgsOpt with
        | Some(receivedLocalClientMsgs) ->
            while receivedLocalClientMsgs.TryDequeue(&msg) do
                f msg
                match msg, localClientOpt with
                | ServerMessage.Message(_, msg), Some(localClient, _) -> localClient.RecycleMessage(msg)
                | _ -> ()
        | _ -> ()

        server.ProcessMessages(f)

    member __.OnException = exceptionEvent.Publish

    member __.CreateLocalBackgroundClient() =
        let localClient = Peer(msgReg, 1)
        let localClientReceiveHash = HashSet()
        let localClientReceiveQueue = ConcurrentQueue()
        let receivedLocalClientMsgs = ConcurrentQueue()
        let onLocalClientException = Event<Exception>()

        let mutable connectedQueue = None

        localClientOpt <- Some(localClient, localClientReceiveQueue)
        receivedLocalClientMsgsOpt <- Some(receivedLocalClientMsgs)
        {
            new IBackgroundClient with

                member __.Connect(_, _) =
                    localClientReceiveQueue.Enqueue(struct(Unchecked.defaultof<ClientId>, ClientMessage.ConnectionAccepted(Unchecked.defaultof<ClientId>)))
                    receivedLocalClientMsgs.Enqueue(ServerMessage.ClientConnected(ClientId()))

                member __.Disconnect() = 
                    receivedLocalClientMsgs.Enqueue(ServerMessage.ClientDisconnected(ClientId()))

                member __.SendMessage(msg) =
                    receivedLocalClientMsgs.Enqueue(ServerMessage.Message(ClientId(), msg))

                member __.CreateMessage() =
                    localClient.CreateMessage()

                member __.ListenForMessage<'T when 'T :> NetMessage>() =
                    if localClientReceiveHash.Add(typeof<'T>) then
                        localClient.MessageReceived<'T>().Add(fun _ -> ())

                member __.ProcessMessages(f) =
                    let mutable fullMsg = Unchecked.defaultof<_>
                    while localClientReceiveQueue.TryDequeue(&fullMsg) do
                        let struct(_, cl_msg) = fullMsg
                        f cl_msg

                        match cl_msg with
                        | ClientMessage.Message msg ->
                            server.RecycleMessage(msg)
                        | _ -> ()

                member __.OnException = onLocalClientException.Publish

            interface IDisposable with

                member __.Dispose() = ()
        }

    interface IDisposable with

        member __.Dispose() =
            mp.PostAndReply(fun x -> Dispose x)
            (mp :> IDisposable).Dispose()
