namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message
open Foom.Core

[<Sealed>]
type ClientManager(msgFactory, channelLookupFactory: ChannelLookupFactory, maxClients: int) =
    let endPointLookup = Dictionary<EndPoint, ClientId>()

    let manager = Manager<ConnectedClient>(maxClients)

    let lockObj = obj ()

    member __.AddClient(udpServer, currentTime, endPoint) =
        let client = ConnectedClient(msgFactory, channelLookupFactory.CreateChannelLookup(msgFactory.PoolLookup), udpServer, endPoint)
        client.Time <- currentTime

        let clientId = { id = manager.Add(client) }

        lock lockObj
        |> fun _ ->
            endPointLookup.Add(endPoint, clientId)

        clientId

    member __.RemoveClient(clientId) =
        lock lockObj
        |> fun _ ->
            manager.Remove(clientId.id)

    member __.TryGetClientId(endPoint) =
        match endPointLookup.TryGetValue(endPoint) with
        | true, clientId -> Some clientId
        | _ -> None

    member __.SendMessage(clientId, msg, channelId, willRecycle) =
        let client = manager.Get(clientId.id)
        client.SendMessage(msg, channelId, willRecycle)

    member __.SendMessage(msg, channelId, willRecycle) =
        manager.ForEach(fun _ client ->
            client.SendMessage(msg, channelId, willRecycle)
        )

    member __.ReceivePacket(clientId, packet) =
        let client = manager.Get(clientId.id)
        client.ReceivePacket(packet)

    member __.SendPackets() =
        manager.ForEach(fun _ client ->
            client.SendPackets()
        )

    /// Thread safe
    member this.ProcessReceivedMessages(f) =
        lock lockObj
        |> fun _ ->
            manager.ForEach(fun id client ->
                client.ProcessReceivedMessages(fun msg -> f { id = id } msg)
            )
            // TODO: Do something here.
            //if not didSucceed then
              //  this.RemoveClient(client.ClientId)
                // TODO: Add Ban client or something for malformed packet.

    member this.IsClientConnected(clientId) = manager.IsValid(clientId.id)

    member this.SetAllClientsTime(time) =
        manager.ForEach(fun _ client ->
            client.Time <- time
        )
