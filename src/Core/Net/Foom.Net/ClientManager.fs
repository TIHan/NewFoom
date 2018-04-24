namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message
open Foom.Core

[<Sealed>]
type ClientManager(msgFactory, maxClients: int) =
    let endPointLookup = Dictionary<EndPoint, ClientId>()

    let manager = Manager<ConnectedClient>(maxClients)

    let lockObj = obj ()

    member __.AddClient(udpServer, currentTime, endPoint) =
        let client = ConnectedClient(msgFactory, udpServer, endPoint)
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
        if not client.IsChallenging then
            client.SendMessage(msg, willRecycle)
        else
            msgFactory.RecycleMessage(msg)

    member __.SendMessage(msg, willRecycle) =
        manager.ForEach(fun _ client ->
            if not client.IsChallenging then
                client.SendMessage(msg, willRecycle)
        )

        if msg.refCount <= 0 then
            msgFactory.RecycleMessage(msg)

    member this.ReceivePacket(clientId: ClientId, packet) =
        let client = manager.Get(clientId.Id)
        try client.ReceivePacket(packet)
        with | ex -> 
            printfn "Tried to receive packet from client. Client removed because: %A" ex
            this.RemoveClient(clientId)

    member this.SendPackets() =
        manager.ForEach(fun id client ->
            try client.SendPackets()
            with | ex -> 
                printfn "Tried to send packets to client. Client removed because: %A" ex
                this.RemoveClient(ClientId(id))
        )

    /// Thread safe
    member this.ProcessReceivedMessages(f) =
        lock lockObj
        |> fun _ ->
            manager.ForEach(fun id client ->
                client.ProcessReceivedMessages(fun msg -> 
                    if client.IsChallenging then
                        match msg with
                        | :? ConnectionRequested ->
                            let sendingMsg = msgFactory.CreateMessage<ConnectionChallengeRequested>()
                            sendingMsg.clientId <- ClientId(id)
                            client.SendMessage(sendingMsg, willRecycle = true)
                        | :? ConnectionChallengeAccepted ->
                            let sendingMsg = msgFactory.CreateMessage<ConnectionAccepted>()
                            client.SendMessage(sendingMsg, willRecycle = true)
                            client.IsChallenging <- false
                            f (ClientId(id)) msg
                        | _ -> () // TODO: Disconnect challenging client.
                    else
                        f (ClientId(id)) msg
                )
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
