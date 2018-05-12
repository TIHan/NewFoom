namespace Foom.Net

open System
open System.Threading.Tasks
open Foom.Tasks
open Foom.IO.Message
open Foom.IO.Packet

[<AutoOpen>]
module Helpers =

    let inline receivePacket (netChannel: NetChannel) receive =
        let packet =
            try
                receive ()
            with | ex ->
                printfn "%A" ex
                Packet.Empty

        match packet with
        | packet when packet.IsEmpty -> ()
        | packet ->

            try
                netChannel.ReceivePacket(packet)
            with | ex ->
                printfn "Receive packet failed: %A" ex

    let inline sendPackets (taskQueue: TaskQueue) (netChannel: NetChannel) sendPacket =
        taskQueue.Enqueue(fun () ->
            netChannel.SendPackets(sendPacket)
        )

[<AutoOpen>]
module ClientHelpers =

    let receivePacket (udpClient: UdpClient) netChannel =
        receivePacket netChannel (fun () ->
            if udpClient.IsConnected && udpClient.IsDataAvailable then
                udpClient.Receive()
            else
                Packet.Empty
        )

    let sendPackets (udpClient: UdpClient) taskQueue netChannel =
        sendPackets taskQueue netChannel <| Foom.IO.Serializer.SpanExtensions.SpanDelegate(fun packet ->
            if udpClient.IsConnected then
                udpClient.Send(packet)
        )
        
    
[<Sealed>]
type Client(msgFactory: MessageFactory) =

    // UDP
    let udpClient = new UdpClient()

    // Packet streams and channels
    let stream = PacketStream()
    let channelLookup = msgFactory.CreateChannelLookup()
    let netChannel = NetChannel(stream, msgFactory, channelLookup)

    let receivePacketsTask = 
        new Task((fun () -> while true do receivePacket udpClient netChannel), TaskCreationOptions.LongRunning)

    let sendTaskQueue = new TaskQueue()

    // Client state
    let mutable currentTime = TimeSpan.Zero
    let mutable isConnected = false
    let mutable clientId = ClientId()

    let heartbeat () =
        if isConnected then
            let msg = msgFactory.CreateMessage<Heartbeat>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, false)

    let connectionRequest () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionRequested>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, false)

    let connectionChallengeAccepted () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionChallengeAccepted>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, false)

    let disconnectRequest () =
        if isConnected then
            let msg = msgFactory.CreateMessage<DisconnectRequested>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, false)

    do
        receivePacketsTask.Start()

    interface IClient with

        member __.Connect(address: string, port: int) =
            if not isConnected && not udpClient.IsConnected then
                udpClient.Connect(address, port) |> ignore
                connectionRequest ()

        member __.Disconnect() =
            if udpClient.IsConnected && isConnected then
                disconnectRequest ()

        member __.SendMessage(msg: NetMessage) =

            msg.IncrementRefCount()

            if udpClient.IsConnected && isConnected then
                netChannel.SendMessage(msg, false)
            else
                msgFactory.RecycleMessage(msg)

        member __.Update(interval, clientUpdate: IClientUpdate) =

            netChannel.ProcessReceivedMessages(fun msg ->
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

            sendPackets udpClient sendTaskQueue netChannel

            stream.Time <- stream.Time + interval

        member __.IsConnected = isConnected

        member __.CreateMessage() =
            msgFactory.CreateMessage()

        member __.GetBeforeSerializedEvent<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)>() =
            let typeId = msgFactory.GetTypeId<'T>()
            match channelLookup.TryGetValue(typeId) with
            | (true, struct(channel, _)) -> channel.GetBeforeSerializedEvent(typeId) |> Event.map (fun x -> x :?> 'T)
            | _ -> failwithf "TypeId, %i, has not been registered to a channel. Unable to get event." typeId

        member __.GetBeforeDeserializedEvent<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)>() =
            let typeId = msgFactory.GetTypeId<'T>()
            match channelLookup.TryGetValue(typeId) with
            | (true, struct(channel, _)) -> channel.GetBeforeDeserializedEvent(typeId) |> Event.map (fun x -> x :?> 'T)
            | _ -> failwithf "TypeId, %i, has not been registered to a channel. Unable to get event." typeId

    interface IDisposable with

        member __.Dispose() =
            (udpClient :> IDisposable).Dispose()
