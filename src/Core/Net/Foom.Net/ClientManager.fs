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

        clientId

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

    member __.TryGetClientId(endPoint) =
        match endPointLookup.TryGetValue(endPoint) with
        | true, clientId ->
            Some clientId
        | _ -> None

    member __.SendMessage(clientId, msg, channelId, willRecycle) =
        let struct(_, client) = clients.[clientLookup.[clientId]]
        client.SendMessage(msg, channelId, willRecycle)

    member __.SendMessage(msg, channelId, willRecycle) =
        // TODO: We could optimize this by serializing the msg once.
        for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]
            client.SendMessage(msg, channelId, willRecycle)

    member __.ReceivePacket(clientId, packet) =
        let struct(_, client) = clients.[clientLookup.[clientId]]
        client.ReceivePacket(packet)

    member __.SendPackets() =
        for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]
            client.SendPackets()

    member this.ProcessReceivedMessages(f) =
        for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]     
            client.ProcessReceivedMessages(f)
            // TODO: Do something here.
            //if not didSucceed then
              //  this.RemoveClient(client.ClientId)
                // TODO: Add Ban client or something for malformed packet.

    member this.IsClientConnected(clientId) = clientLookup.[clientId] <> -1

    member this.SetAllClientsTime(time) =
         for i = 0 to clients.Count - 1 do
            let struct(_, client) = clients.[i]
            client.Time <- time
