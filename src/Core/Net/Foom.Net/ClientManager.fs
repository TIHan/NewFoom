namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message
open Foom.Core

[<Sealed>]
type ClientManager(msgFactory, typeToChannelMap, channelLookupFactory: ChannelLookupFactory, maxClients: int) =
    let endPointLookup = Dictionary<EndPoint, ClientId>()

    let manager = Manager<ConnectedClient>(maxClients)

    let lockObj = obj ()

    member __.AddClient(udpServer, currentTime, endPoint) =
        let client = ConnectedClient(msgFactory, typeToChannelMap, channelLookupFactory.CreateChannelLookup(msgFactory.PoolLookup), udpServer, endPoint)
        client.Time <- currentTime

        let clientId = lock lockObj |> fun _ -> ClientId(manager.Add(client))
        endPointLookup.Add(endPoint, clientId)
        clientId

    member __.RemoveClient(clientId: ClientId) =
        let endPoint = manager.Get(clientId.Id).EndPoint
        endPointLookup.Remove(endPoint) |> ignore
        lock lockObj
        |> fun _ ->
            manager.Remove(clientId.Id)

    member this.TryRemoveClientByEndPoint(endPoint) =
        match endPointLookup.TryGetValue(endPoint) with
        | true, clientId ->
            this.RemoveClient(clientId)
            true
        | _ -> false

    member __.TryGetClientId(endPoint) =
        match endPointLookup.TryGetValue(endPoint) with
        | true, clientId -> Some clientId
        | _ -> None

    member __.SendMessage(clientId: ClientId, msg, willRecycle) =
        let client = manager.Get(clientId.Id)
        client.SendMessage(msg, willRecycle)

    member __.SendMessage(msg, willRecycle) =
        manager.ForEach(fun _ client ->
            client.SendMessage(msg, willRecycle)
        )

    member this.ReceivePacket(clientId: ClientId, packet) =
        let client = manager.Get(clientId.Id)
        try client.ReceivePacket(packet)
        with | _ -> this.RemoveClient(clientId)

    member this.SendPackets() =
        manager.ForEach(fun id client ->
            try client.SendPackets()
            with | _ -> this.RemoveClient(ClientId(id))
        )

    /// Thread safe
    member this.ProcessReceivedMessages(f) =
        lock lockObj
        |> fun _ ->
            manager.ForEach(fun id client ->
                client.ProcessReceivedMessages(fun msg -> f (ClientId(id)) msg)
            )
            // TODO: Do something here.
            //if not didSucceed then
              //  this.RemoveClient(client.ClientId)
                // TODO: Add Ban client or something for malformed packet.

    member this.IsClientConnected(clientId: ClientId) = manager.IsValid(clientId.Id)

    member this.SetAllClientsTime(time) =
        manager.ForEach(fun _ client ->
            client.Time <- time
        )
