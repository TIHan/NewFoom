namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message

[<Sealed>]
type ClientManager(msgFactory, channelLookupFactory: ChannelLookupFactory, maxClients: int) =
    let endPointLookup = Dictionary<EndPoint, int>()
    let clientLookup = Array.init (maxClients + 1) (fun _ -> -1)
    let clients = ResizeArray(maxClients)

    // 0 is reserved
    let clientIdQueue = Queue([1..maxClients])

    member __.AddClient(udpServer, currentTime, endPoint) =
        let clientId = clientIdQueue.Dequeue()

        let client = ConnectedClient(msgFactory, channelLookupFactory.CreateChannelLookup(msgFactory.PoolLookup), udpServer, endPoint, clientId)
        client.Time <- currentTime

        let index = clients.Count
        endPointLookup.Add(endPoint, clientId)
        clientLookup.[clientId] <- index
        clients.Add(struct(clientId, client))

        client

    member __.RemoveClient(clientId) =
        if clientLookup.[clientId] <> -1 then
            let swappingIndex = clients.Count - 1
            let struct(swappingClientId, swappingClient) = clients.[swappingIndex]

            let index = clientLookup.[clientId]
            let struct(_, client) = clients.[index]
            clients.[index] <- struct(swappingClientId, swappingClient)
            clientLookup.[swappingClientId] <- index

            clients.RemoveAt(swappingIndex)
            endPointLookup.Remove(client.EndPoint) |> ignore

            clientLookup.[clientId] <- -1
            clientIdQueue.Enqueue(clientId)

    member __.Clear() =
        endPointLookup.Clear()
        for i = 0 to clientLookup.Length - 1 do clientLookup.[i] <- -1
        clients.Clear()

    member __.TryGetClient(endPoint) =
        match endPointLookup.TryGetValue(endPoint) with
        | true, clientId ->
            let struct(_, client) = clients.[clientLookup.[clientId]]
            Some client
        | _ -> None

    member __.SendMessage(msg, channelId, clientId, willRecycle) =
        let struct(_, client) = clients.[clientLookup.[clientId]]
        client.SendMessage(msg, channelId, willRecycle)

    member __.SendMessage(msg, channelId, willRecycle) =
        // TODO: We could optimize this by serializing the msg once.
        for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]
            client.SendMessage(msg, channelId, willRecycle)

    member __.SendMessages() =
        for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]
            client.SendMessages()

    member this.ReceiveMessages(f) =
        for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]     
            client.TryReceiveMessages(f)
            // TODO: Do something here.
            //if not didSucceed then
              //  this.RemoveClient(client.ClientId)
                // TODO: Add Ban client or something for malformed packet.

    member this.IsClientConnected(clientId) = clientLookup.[clientId] <> -1

    member this.SetAllClientsTime(time) =
         for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]
            client.Time <- time

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
            clients.SendMessage(msg, channelId, clientId, willRecycle)

    member this.SendMessage(msg: Message, channelId, willRecycle) =
        if udpServerOpt.IsSome then
            clients.SendMessage(msg, channelId, willRecycle)

    member __.SendMessages() = // TODO: Rename to SendPackets
        if udpServerOpt.IsSome then
            clients.SendMessages()

    member this.ReceivePackets() =
        match udpServerOpt with
        | None -> () // server not started
        | Some udpServer ->
            
            while udpServer.IsDataAvailable do
                let mutable endPoint = Unchecked.defaultof<IPEndPoint>
                let packet = udpServer.Receive(&endPoint)

                // NEED TO ADD CHECKS TO PREVENT MANY CLIENTS TRYING TO CONNECT

                match clients.TryGetClient(endPoint) with
                | Some client -> client.OnReceivePacket packet
                | _ -> 
                    let client = clients.AddClient(udpServerOpt.Value, currentTime, endPoint)

                    client.OnReceivePacket packet

                    client.TryReceiveMessages(fun clientId msg -> 
                        if msg :? ConnectionRequested then
                            this.Publish(msg, clientId)
                        else
                            failwith "Client sent an invalid message before it was connected."
                    )
                    // TODO: Do something here.
                //    if not didSucceed then
                  //      clients.RemoveClient(client.ClientId)
                        // BAN CLIENT

    member __.PumpMessages() =
        clients.ReceiveMessages(fun clientId msg -> this.Publish(msg, clientId))

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