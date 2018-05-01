﻿namespace Foom.Net

open System
open System.Threading.Tasks
open Foom.Core
open Foom.IO.Message
open Foom.IO.Packet
    
[<Sealed>]
type Client(msgFactory: MessageFactory) =

    // Events
    let exRaisedEvent = Event<Exception>()

    // UDP
    let udpClient = new UdpClient()

    // Packet streams and channels
    let stream = PacketStream()
    let channelLookup = msgFactory.CreateChannelLookup()
    let netChannel = NetChannel(stream, msgFactory, channelLookup)
    let sendTaskQueue = new TaskQueue()

    let sendPacket(packet) =
        if udpClient.IsConnected then
            udpClient.Send(packet)

    let receivePacket() =
        try
            if udpClient.IsConnected && udpClient.IsDataAvailable then
                udpClient.Receive()
            else
                Packet.Empty
        with | ex ->
            printfn "%A" ex
            Packet.Empty

    let sendPackets() =
        sendTaskQueue.Enqueue(fun () ->
            netChannel.SendPackets(fun packet ->
                if udpClient.IsConnected then
                    udpClient.Send(packet)
            )
        )

    let receivePacketsTask = 
        new Task((fun () ->
            while true do

                match receivePacket() with
                | packet when packet.IsEmpty -> ()
                | packet ->

                    try
                        stream.Receive(packet, fun data -> 
                            let mutable data = data
                            while data.Length > 0 do
                                let typeId = data.[0]
                                let channelId = data.[3] // This gets the channelId - don't change.

                                if msgFactory.GetChannelId(typeId) <> channelId then
                                    failwith "Message received with invalid channel."

                                let struct(channel, _) = channelLookup.[channelId]
                                let numBytesRead = channel.Receive(data)

                                if numBytesRead = 0 then
                                    failwith "Unable to receive message."

                                data <- data.Slice(numBytesRead)
                        )
                    with | ex ->
                        printfn "MessageReceiver threw: %A" ex
        ), TaskCreationOptions.LongRunning)

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

            netChannel.SendPackets(fun packet ->
                if udpClient.IsConnected then
                    udpClient.Send(packet)
            )

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
