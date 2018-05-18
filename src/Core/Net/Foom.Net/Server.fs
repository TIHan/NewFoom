namespace Foom.Net

open System
open System.Collections.Generic
open System.Net
open System
open Foom.IO.Packet
open Foom.IO.Message
open Foom.Tasks

[<Sealed>]
type Server(msgFactory, port: int, maxClients) =

    let mutable udpServerOpt : UdpServer option = None
    let mutable currentTime = TimeSpan.Zero

    let clients = ClientManager(msgFactory, maxClients)

    let receive () =
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
                    clients.ReceivePacket(clientId, packet)

    let reactor = Reactor(receive)

    member val UdpServerOption : UdpServer option = udpServerOpt

    interface IServer with

        member __.Start() =
            match udpServerOpt with
            | None -> 
                udpServerOpt <- Some(new UdpServer(port))
                reactor.Start()
            | _ -> () // Server already started

        member __.Stop() =
            match udpServerOpt with
            | Some udpServer -> 
                reactor.Stop()
                udpServer.Close()
                udpServerOpt <- None
            | _ ->
                () // Server not started

        member __.SendMessage(msg: NetMessage, clientId: ClientId, willRecycle) =
            if udpServerOpt.IsSome && reactor.IsRunning then
                reactor.Enqueue(fun () -> clients.SendMessage(clientId, msg, willRecycle))
            else
                msgFactory.RecycleMessage(msg)

        member this.SendMessage(msg: NetMessage, willRecycle) =
            if udpServerOpt.IsSome && reactor.IsRunning then
                reactor.Enqueue(fun () -> clients.SendMessage(msg, willRecycle))
            else
                msgFactory.RecycleMessage(msg)

        member __.SendPackets() =
            if udpServerOpt.IsSome && reactor.IsRunning then
                reactor.Enqueue(fun () -> clients.SendPackets())

        member this.ProcessMessages(f) =
            clients.ProcessReceivedMessages(fun clientId msg ->
                match msg with
                | :? ConnectionChallengeAccepted -> 
                    f (ServerMessage.ClientConnected(clientId))

                | :? ConnectionRequested -> () // TODO: Revisit
                | :? DisconnectRequested -> () // TODO: Revisit

                | :? Heartbeat -> ()

                | _ -> 
                    f (ServerMessage.Message(clientId, msg))

                msgFactory.RecycleMessage(msg)
            )

        member this.DisconnectClient(clientId) =
            if reactor.IsRunning then
                reactor.Enqueue(fun () ->
                    if clients.IsClientConnected(clientId) then
                        clients.RemoveClient(clientId)

                        let msg = msgFactory.CreateMessage<ClientDisconnected>()
                        msg.reason <- "Client timed out."
                        (this :> IServer).SendMessage(msg, willRecycle = true)
                )

        member __.Time
            with get () = currentTime
            and set value =
                currentTime <- value
                reactor.Enqueue(fun () -> clients.SetAllClientsTime(currentTime))

        member __.CreateMessage() =
            msgFactory.CreateMessage()

        member __.RecycleMessage(msg) =
            msgFactory.RecycleMessage(msg)

    interface IDisposable with

        member this.Dispose() =
            match udpServerOpt with
            | Some _ -> (this :> IServer).Stop()
            | _ -> ()