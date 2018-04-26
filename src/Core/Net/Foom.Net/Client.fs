﻿namespace Foom.Net

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
    let stream = PacketStream()
    let netChannel = NetChannel(stream, msgFactory, msgFactory.CreateChannelLookup())

    // Client state
    let mutable currentTime = TimeSpan.Zero
    let mutable isConnected = false

    let mutable clientId = ClientId()

    // Internal client messages
    let sendPackets () =
        netChannel.SendPackets(fun packet -> udpClient.Send(packet))

    let heartbeat () =
        if isConnected then
            let msg = msgFactory.CreateMessage<Heartbeat>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, willRecycle = true)

    let connectionRequest () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionRequested>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, willRecycle = true)

    let connectionChallengeAccepted () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionChallengeAccepted>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, willRecycle = true)

    let disconnectRequest () =
        if isConnected then
            let msg = msgFactory.CreateMessage<DisconnectRequested>()
            msg.IncrementRefCount()
            netChannel.SendMessage(msg, willRecycle = true)

    let senderTaskQueue = TaskQueue()
    let receivePacketsTask = 
        new Task((fun () ->
            while true do
                if udpClient.IsConnected then
                    while udpClient.IsDataAvailable do
                        let packet = udpClient.Receive()
                        netChannel.ReceivePacket(packet)), TaskCreationOptions.LongRunning)

    do
        receivePacketsTask.Start()

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
            netChannel.SendMessage(msg, willRecycle)
        else
            msgFactory.RecycleMessage(msg)

    member __.Update(interval, clientUpdate: IClientUpdate) =

        netChannel.ProcessReceivedMessages (fun msg ->
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

        if udpClient.IsConnected then
            senderTaskQueue.Enqueue(sendPackets) |> ignore

        stream.Time <- stream.Time + interval

    member __.IsConnected = isConnected

    member __.CreateMessage() =
        msgFactory.CreateMessage()

    member __.RecycleMessage(msg) =
        msgFactory.RecycleMessage(msg)

    member __.GetBeforeSerializedEvent(typeId) = netChannel.GetBeforeSerializedEvent(typeId)

    member __.GetBeforeDeserializedEvent(typeId) = netChannel.GetBeforeDeserializedEvent(typeId)

    interface IDisposable with

        member __.Dispose() =
            (udpClient :> IDisposable).Dispose()
