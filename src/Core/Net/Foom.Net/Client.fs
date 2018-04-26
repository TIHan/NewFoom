namespace Foom.Net

open System
open System.Threading.Tasks
open Foom.Core
open Foom.IO.Message
open Foom.IO.Packet

[<Struct;RequireQualifiedAccess>]
type ClientMessage =
    | ConnectionAccepted of clientId: ClientId
    | DisconnectAccepted
    | Message of NetMessage

type IClientUpdate =

    abstract OnMessageReceived : ClientMessage -> unit

    abstract OnAfterMessagesReceived : unit -> unit
    
[<Sealed>]
type Client(msgFactory: MessageFactory) =

    // Events
    let exRaisedEvent = Event<Exception>()

    // UDP
    let udpClient = new UdpClient()

    // Packet streams and channels
    let sendMsgQueue = MessageQueue()
    let stream = PacketStream()
    let channelLookup = msgFactory.CreateChannelLookup()
    let taskQueue = new TaskQueue()
    let packetSender = new PacketSender(stream, channelLookup, msgFactory, taskQueue, 10, fun packet -> 
        if udpClient.IsConnected then
            udpClient.Send(packet)
    ) 
    let messageReceiver = new MessageReceiver([|stream|], [|channelLookup|], msgFactory, fun streamIndex ->
        streamIndex <- 0
        try
            if udpClient.IsConnected && udpClient.IsDataAvailable then
                udpClient.Receive()
            else
                Packet.Empty
        with | ex ->
            printfn "%A" ex
            Packet.Empty
    )

    // Client state
    let mutable currentTime = TimeSpan.Zero
    let mutable isConnected = false
    let mutable clientId = ClientId()

    let processReceivedMessages f =
        channelLookup
        |> Seq.iter (fun pair -> 
            let struct(channel, _) = pair.Value
            channel.ProcessReceived(fun msg ->
                match msg with
                | :? NetMessage as msg -> f msg
                | _ -> failwith "Invalid NetMessage"
            )
        )

    let heartbeat () =
        if isConnected then
            let msg = msgFactory.CreateMessage<Heartbeat>()
            msg.IncrementRefCount()
            sendMsgQueue.Enqueue(msg)

    let connectionRequest () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionRequested>()
            msg.IncrementRefCount()
            sendMsgQueue.Enqueue(msg)

    let connectionChallengeAccepted () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionChallengeAccepted>()
            msg.IncrementRefCount()
            sendMsgQueue.Enqueue(msg)

    let disconnectRequest () =
        if isConnected then
            let msg = msgFactory.CreateMessage<DisconnectRequested>()
            msg.IncrementRefCount()
            sendMsgQueue.Enqueue(msg)

    member __.Connect(address: string, port: int) =
        if not isConnected && not udpClient.IsConnected then
            udpClient.Connect(address, port) |> ignore
            connectionRequest ()

    member __.Disconnect() =
        if udpClient.IsConnected && isConnected then
            disconnectRequest ()

    member __.SendMessage(msg: NetMessage, willRecycle) =

        msg.IncrementRefCount()

        if udpClient.IsConnected && isConnected then
            sendMsgQueue.Enqueue(msg)
        else
            msgFactory.RecycleMessage(msg)

    member __.Update(interval, clientUpdate: IClientUpdate) =

        processReceivedMessages (fun msg ->
            match msg with
            | :? ConnectionChallengeRequested as msg ->
                clientId <- msg.clientId
                connectionChallengeAccepted ()

            | :? ConnectionAccepted as msg -> 
                isConnected <- true
                clientUpdate.OnMessageReceived(ClientMessage.ConnectionAccepted(clientId))

            | :? DisconnectAccepted ->
                isConnected <- false
                udpClient.Disconnect()
                clientUpdate.OnMessageReceived(ClientMessage.DisconnectAccepted)

            | :? Heartbeat -> ()

            | _ -> 
                clientUpdate.OnMessageReceived(ClientMessage.Message(msg))

            msgFactory.RecycleMessage(msg)
        )

        clientUpdate.OnAfterMessagesReceived()

        if udpClient.IsConnected && isConnected then
            heartbeat ()

        packetSender.SendPacketsAsync(sendMsgQueue) |> ignore

        stream.Time <- stream.Time + interval

    member __.IsConnected = isConnected

    member __.CreateMessage() =
        msgFactory.CreateMessage()

    member __.RecycleMessage(msg) =
        msgFactory.RecycleMessage(msg)

    member __.GetBeforeSerializedEvent(typeId) =
        match channelLookup.TryGetValue(typeId) with
        | (true, struct(channel, _)) -> channel.GetBeforeSerializedEvent(typeId)
        | _ -> failwithf "TypeId, %i, has not been registered to a channel. Unable to get event." typeId

    member __.GetBeforeDeserializedEvent(typeId) =
        match channelLookup.TryGetValue(typeId) with
        | (true, struct(channel, _)) -> channel.GetBeforeDeserializedEvent(typeId)
        | _ -> failwithf "TypeId, %i, has not been registered to a channel. Unable to get event." typeId

    interface IDisposable with

        member __.Dispose() =
            (udpClient :> IDisposable).Dispose()
