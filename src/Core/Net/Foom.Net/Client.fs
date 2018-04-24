namespace Foom.Net

open System
open Foom.IO.Message
open Foom.IO.Packet

[<Struct;RequireQualifiedAccess>]
type ClientMessage =
    | ConnectionAccepted of clientId: ClientId
    | DisconnectAccepted
    | Message of NetMessage
    
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

    // Internal client messages
    let sendPackets () =
        netChannel.SendPackets(fun packet -> udpClient.Send(packet))

    let heartbeat () =
        let msg = msgFactory.CreateMessage<Heartbeat>()
        netChannel.SendMessage(msg, willRecycle = true)

    let connectionRequest () =
        let msg = msgFactory.CreateMessage<ConnectionRequested>()
        netChannel.SendMessage(msg, willRecycle = true)

    let disconnectRequest () =
        let msg = msgFactory.CreateMessage<DisconnectRequested>()
        netChannel.SendMessage(msg, willRecycle = true)

    member __.Connect(address: string, port: int) =
        if not isConnected && not udpClient.IsConnected then
            udpClient.Connect(address, port) |> ignore
            connectionRequest ()

    member __.Disconnect() =
        if udpClient.IsConnected && isConnected then
            disconnectRequest ()

    member __.SendMessage(msg, willRecycle) =
        if udpClient.IsConnected && isConnected then
            netChannel.SendMessage(msg, willRecycle)

    member __.SendPackets() =
        if udpClient.IsConnected && isConnected then
            heartbeat ()

        if udpClient.IsConnected then
            sendPackets ()

    member __.ReceivePackets() =
        if udpClient.IsConnected then
            while udpClient.IsDataAvailable do
                let packet = udpClient.Receive()
                netChannel.ReceivePacket(packet)

    /// Thread safe
    member __.ProcessMessages(f) =
        netChannel.ProcessReceivedMessages (fun msg ->
            match msg with
            | :? ConnectionAccepted as msg -> 
                isConnected <- true
                f (ClientMessage.ConnectionAccepted(msg.clientId))

            | :? DisconnectAccepted ->
                isConnected <- false
                udpClient.Disconnect()
                f ClientMessage.DisconnectAccepted

            | :? Heartbeat -> ()

            | _ -> 
                f (ClientMessage.Message(msg))

            msgFactory.RecycleMessage(msg)
        )

    member __.Time 
        with get () = currentTime
        and set value =
            currentTime <- value
            stream.Time <- currentTime

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
