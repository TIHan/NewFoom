﻿namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message

[<RequireQualifiedAccess>]
type ServerMessage =
    | ClientConnected of ClientId
    | ClientDisconnected of ClientId
    | Message of ClientId * NetMessage

[<Sealed>]
type Server(msgReg, channelLookupFactory, port: int, maxClients) as this =
    inherit Peer(msgReg, maxClients / 4) // conversative, perhaps revisit

    let mutable udpServerOpt = None
    let mutable currentTime = TimeSpan.Zero

    let clients = ClientManager(this.MessageFactory, msgReg.LookupChannelId, channelLookupFactory, maxClients)

    member val UdpServerOption : UdpServer option = udpServerOpt

    member __.Start() =
        match udpServerOpt with
        | None -> udpServerOpt <- Some(new UdpServer(port))
        | _ -> () // Server already started

    member __.Stop() =
        match udpServerOpt with
        | Some udpServer -> 
          //  clients.Clear()
            udpServer.Close()
            udpServerOpt <- None
        | _ ->
            () // Server not started

    member __.SendMessage(msg: NetMessage, clientId: ClientId, willRecycle) =
        if udpServerOpt.IsSome then
            clients.SendMessage(clientId, msg, willRecycle)

    member this.SendMessage(msg, willRecycle) =
        if udpServerOpt.IsSome then
            clients.SendMessage(msg, willRecycle)

    member __.SendPackets() =
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
                | Some clientId -> 
                    clients.ReceivePacket(clientId, packet)
                | _ -> 

                    let clientId = clients.AddClient(udpServerOpt.Value, currentTime, endPoint)

                    try
                        clients.ReceivePacket(clientId, packet)
                    with | _ ->
                        // TODO: ban client
                        printfn "Client connection refused."
                        clients.RemoveClient(clientId)

    member this.ProcessMessages(f) =
        clients.ProcessReceivedMessages(fun clientId msg ->
            match msg with
            | :? ConnectionRequested -> 
                let msg = this.CreateMessage<ConnectionAccepted>()
                msg.clientId <- clientId

                this.SendMessage(msg, clientId, willRecycle = true)
                f (ServerMessage.ClientConnected(clientId))

            | :? DisconnectRequested ->
                // TODO: Revisit
                f (ServerMessage.ClientDisconnected(clientId))

            | :? Heartbeat -> ()

            | _ -> 
                f (ServerMessage.Message(clientId, msg))

            this.RecycleMessage(msg)
        )

    member this.DisconnectClient(clientId) =
        if clients.IsClientConnected(clientId) then
            clients.RemoveClient(clientId)

            let msg = this.CreateMessage<ClientDisconnected>()
            msg.reason <- "Client timed out."
            this.SendMessage(msg, willRecycle = true)

    member __.Time
        with get () = currentTime
        and set value =
            currentTime <- value
            clients.SetAllClientsTime(currentTime)

    interface IDisposable with

        member this.Dispose() =
            match udpServerOpt with
            | Some _ -> this.Stop()
            | _ -> ()