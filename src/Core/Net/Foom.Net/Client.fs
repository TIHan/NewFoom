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
type Client(msgReg, channelLookupFactory: ChannelLookupFactory) as this =
    inherit Peer(msgReg, 1)

    // Events
    let exRaisedEvent = Event<Exception>()

    // UDP
    let udpClient = new UdpClient()

    // Packet streams and channels
    let stream = PacketStream()
    let channelLookup = channelLookupFactory.CreateChannelLookup(this.MessageFactory.PoolLookup)
    let sender = Sender(stream, channelLookup)
    let receiver = Receiver(stream, channelLookup)

    // Client state
    let mutable currentTime = TimeSpan.Zero
    let mutable isConnected = false

    // Internal client messages
    let sendPackets () =
        sender.SendPackets(fun packet -> udpClient.Send(packet))

    let heartbeat () =
        let msg = this.CreateMessage<Heartbeat>()
        sender.EnqueueMessage(msg, DefaultChannelIds.Heartbeat, willRecycle = true)

    let connectionRequest () =
        let msg = this.CreateMessage<ConnectionRequested>()
        sender.EnqueueMessage(msg, DefaultChannelIds.Connection, willRecycle = true)

    let disconnectRequest () =
        let msg = this.CreateMessage<DisconnectRequested>()
        sender.EnqueueMessage(msg, DefaultChannelIds.Connection, willRecycle = true)

    member this.Connect(address: string, port: int) =
        if not isConnected && not udpClient.IsConnected then
            udpClient.Connect(address, port) |> ignore
            connectionRequest ()

    member this.Disconnect() =
        if udpClient.IsConnected && isConnected then
            disconnectRequest ()

    member this.SendMessage(msg, channelId, willRecycle) =
        if udpClient.IsConnected && isConnected then
            sender.EnqueueMessage(msg, channelId, willRecycle)

    member this.SendPackets() =
        if udpClient.IsConnected && isConnected then
            heartbeat ()

        if udpClient.IsConnected then
            sendPackets ()

    member this.ReceivePackets() =
        if udpClient.IsConnected then
            while udpClient.IsDataAvailable do
                let packet = udpClient.Receive()
                receiver.EnqueuePacket packet

    /// Thread safe
    member this.ProcessMessages(f) =
        receiver.ProcessMessages (fun msg ->
            match msg with
            | :? ConnectionAccepted as msg -> 
                isConnected <- true
                f (ClientMessage.ConnectionAccepted(msg.ClientId))

            | :? DisconnectAccepted ->
                isConnected <- false
                udpClient.Disconnect()
                f ClientMessage.DisconnectAccepted

            | :? Heartbeat -> ()

            | _ -> 
                f (ClientMessage.Message(msg))

            this.RecycleMessage(msg)
        )

    member __.Time 
        with get () = currentTime
        and set value =
            currentTime <- value
            stream.Time <- currentTime

    member __.IsConnected = isConnected

    interface IDisposable with

        member __.Dispose() =
            (udpClient :> IDisposable).Dispose()
