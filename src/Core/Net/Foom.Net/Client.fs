namespace Foom.Net

open System
open System.Threading.Tasks
open Foom.Tasks
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
                        netChannel.ReceivePacket(packet)
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
            netChannel.SendMessage(msg, false)

    let connectionRequest () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionRequested>()
            netChannel.SendMessage(msg, false)

    let connectionChallengeAccepted () =
        if not isConnected then
            let msg = msgFactory.CreateMessage<ConnectionChallengeAccepted>()
            netChannel.SendMessage(msg, false)

    let disconnectRequest () =
        if isConnected then
            let msg = msgFactory.CreateMessage<DisconnectRequested>()
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
