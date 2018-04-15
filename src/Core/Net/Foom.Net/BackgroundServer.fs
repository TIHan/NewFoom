namespace Foom.Net

open System
open System.Collections.Generic 
open System.Collections.Concurrent
open System.Diagnostics
open Foom.IO.Message

type BackgroundServerInternalMessage =
    | Start
    | Stop
    | SendMessageAll of Message * channelId: byte * willRecycle: bool
    | SendMessage of Message * channelId: byte * clientId: ClientId * willRecycle: bool
    | Dispose of AsyncReplyChannel<unit>

type BackgroundServer(msgReg, channelLookupFactory, port, maxClients) =
    let clientConnectedQueue = ConcurrentQueue()
    let msgHash = HashSet()
    let msgQueue = ConcurrentQueue()

    let server = new Server(msgReg, channelLookupFactory, port, maxClients)
    let exceptionEvent = Event<Exception>()

    let mutable localClientOpt : (Peer * ConcurrentQueue<struct(ClientId * ClientMessage)>) option = None

    let mp = MailboxProcessor<BackgroundServerInternalMessage>.Start(fun inbox -> 

        let rec start (stopwatch: Stopwatch) =
            let sendRate = TimeSpan.FromMilliseconds(10.)
            let mutable sendTime = TimeSpan.Zero
            async {
                let mutable replyDispose = Unchecked.defaultof<AsyncReplyChannel<unit>>
                let mutable willDispose = false
                while not willDispose do
                    let time = stopwatch.Elapsed

                    try 
                        server.ReceivePackets()

                        if time > sendTime + sendRate then
                            server.PumpMessages()
                            server.SendMessages()
                            sendTime <- sendTime + sendRate

                        if inbox.CurrentQueueLength > 0 then
                            let! msg = inbox.Receive()
                            match msg with
                            | Start -> 
                                server.Start()
                            | Stop -> 
                                server.Stop()
                            | SendMessageAll(msg, channelId, willRecycle) -> 
                                server.SendMessage(msg, channelId, willRecycle)
                            | SendMessage(msg, channelId, clientId, willRecycle) ->
                                server.SendMessage(msg, channelId, clientId, willRecycle)
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

    do
        server.ClientConnected.Add(clientConnectedQueue.Enqueue)

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
          
        mp.Post(SendMessageAll(msg, channelId, willRecycle))

    member __.SendMessage(msg, channelId, clientId: ClientId) =
        match localClientOpt with
        | Some(_, localClientReceiveQueue) when clientId.IsLocal ->
            localClientReceiveQueue.Enqueue struct(ClientId.Local, ClientMessage.Message(msg))
        | _ ->
            mp.Post(SendMessage(msg, channelId, clientId, willRecycle = true))

    member __.ListenForMessage<'T when 'T :> Message>() =
        if msgHash.Add(typeof<'T>) then
            server.MessageReceived<'T>().Add(msgQueue.Enqueue)
           
    member __.ProcessClientConnected(f) =
        let mutable clientId = Unchecked.defaultof<ClientId>
        while clientConnectedQueue.TryDequeue(&clientId) do
            f clientId

    member __.ProcessMessages(f) =
        let mutable fullMsg = Unchecked.defaultof<_>
        while msgQueue.TryDequeue(&fullMsg) do
            let struct(clientId, msg) = fullMsg

            if clientId.IsLocal then
                match localClientOpt with
                | Some(localClient, _) ->
                    f fullMsg
                    // We received a message from the client directly, cleanup from the client.
                    localClient.RecycleMessage(msg)
                | _ ->
                    failwith "Should not happen. Local client doesn't exist yet we received a message with client id, 0."
            else
                f fullMsg
                server.RecycleMessage(msg)

    member __.OnException = exceptionEvent.Publish

    member __.CreateLocalBackgroundClient() =
        let localClient = Peer(msgReg, 1)
        let localClientReceiveHash = HashSet()
        let localClientReceiveQueue = ConcurrentQueue()
        let onLocalClientException = Event<Exception>()

        let mutable connectedQueue = None

        localClientOpt <- Some(localClient, localClientReceiveQueue)
        {
            new IBackgroundClient with

                member __.Connect(_, _) =
                    localClientReceiveQueue.Enqueue(struct(Unchecked.defaultof<ClientId>, ClientMessage.ConnectionAccepted(Unchecked.defaultof<ClientId>)))
                    clientConnectedQueue.Enqueue(Unchecked.defaultof<ClientId>)

                member __.Disconnect() = ()

                member __.SendMessage(msg, _) =
                    server.Publish(msg, Unchecked.defaultof<ClientId>)

                member __.CreateMessage() =
                    localClient.CreateMessage()

                member __.ListenForMessage<'T when 'T :> Message>() =
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
