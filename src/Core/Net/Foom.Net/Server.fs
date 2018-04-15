namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message

[<Sealed>]
type Server(msgReg, channelLookupFactory, port: int, maxClients) as this =
    inherit Peer(msgReg, maxClients / 4) // conversative, perhaps revisit

    let mutable udpServerOpt = None
    let mutable currentTime = TimeSpan.Zero

    let clients = ClientManager(this.MessageFactory, channelLookupFactory, maxClients)

    let clientConnected = Event<int>()
    let clientDisconnected = Event<int>()

    do
        this.MessageReceived<ConnectionRequested>().Add(fun (struct(clientId, msg)) ->
            // TODO: We need to handle this differently.
            clientConnected.Trigger(clientId)

            let msg = this.CreateMessage<ConnectionAccepted>()
            msg.ClientId <- clientId

            this.SendMessage(msg, DefaultChannelIds.Connection, clientId, willRecycle = true)
        )

        this.MessageReceived<Heartbeat>().Add(fun struct(_, msg) -> this.RecycleMessage(msg))

    member val UdpServerOption : UdpServer option = udpServerOpt

    member __.Start() =
        match udpServerOpt with
        | None -> udpServerOpt <- Some(new UdpServer(port))
        | _ -> () // Server already started

    member __.Stop() =
        match udpServerOpt with
        | Some udpServer -> 
            clients.Clear()
            udpServer.Close()
            udpServerOpt <- None
        | _ ->
            () // Server not started

    member __.SendMessage(msg: Message, channelId, clientId, willRecycle) =
        if udpServerOpt.IsSome then
            clients.SendMessage(clientId, msg, channelId, willRecycle)

    member this.SendMessage(msg: Message, channelId, willRecycle) =
        if udpServerOpt.IsSome then
            clients.SendMessage(msg, channelId, willRecycle)

    member __.SendMessages() = // TODO: Rename to SendPackets
        if udpServerOpt.IsSome then
            clients.SendPackets()

    member this.ReceivePackets() =
        match udpServerOpt with
        | None -> () // server not started
        | Some udpServer ->
            
            while udpServer.IsDataAvailable do
                let mutable endPoint = Unchecked.defaultof<IPEndPoint>
                let packet = udpServer.Receive(&endPoint)

                // NEED TO ADD CHECKS TO PREVENT MANY CLIENTS TRYING TO CONNECT

                match clients.TryGetClientId(endPoint) with
                | Some client -> 
                    clients.ReceivePacket(client, packet)
                | _ -> 

                    let clientId = clients.AddClient(udpServerOpt.Value, currentTime, endPoint)

                    try
                        clients.ReceivePacket(clientId, packet)
                    with | _ ->
                        // TODO: ban client
                        printfn "Client connection refused."
                        clients.RemoveClient(clientId)

    member __.PumpMessages() =
        clients.ProcessReceivedMessages(fun clientId msg -> this.Publish(msg, clientId))

    member this.DisconnectClient(clientId) =
        if clients.IsClientConnected(clientId) then
            clients.RemoveClient(clientId)
            clientDisconnected.Trigger(clientId)

            let msg = this.CreateMessage<ClientDisconnected>()
            msg.Reason <- "Client timed out."
            this.SendMessage(msg, DefaultChannelIds.Connection, willRecycle = true)

    member __.Time
        with get () = currentTime
        and set value =
            currentTime <- value
            clients.SetAllClientsTime(currentTime)

    member __.ClientConnected = clientConnected.Publish

    member __.ClientDisconnected = clientDisconnected.Publish

    interface IDisposable with

        member this.Dispose() =
            match udpServerOpt with
            | Some _ -> this.Stop()
            | _ -> ()