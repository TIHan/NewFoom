﻿namespace Foom.Net

open System
open System.Net
open Foom.IO.Message
open Foom.IO.Packet

[<Sealed>]
type ConnectedClient(msgFactory: MessageFactory, udpServer: UdpServer, endPoint: IPEndPoint) as this =
    let stream = PacketStream()
    let netChannel = NetChannel(stream, msgFactory, msgFactory.CreateChannelLookup())

    let heartbeat () =
        if not this.IsChallenging then
            let msg = msgFactory.CreateMessage<Heartbeat>()
            netChannel.SendMessage(msg, willRecycle = true)

    member __.SendMessage(msg, willRecycle) =
        netChannel.SendMessage(msg, willRecycle)

    member __.ReceivePacket(packet) =
        netChannel.ReceivePacket(packet)

    member __.ProcessReceivedMessages(f) =
        netChannel.ProcessReceivedMessages(f)

    member __.SendPackets() =
        heartbeat ()
        netChannel.SendPackets(fun packet ->
            udpServer.Send(packet, endPoint)
        )

    member __.Time
        with get () = stream.Time
        and set value = stream.Time <- value

    member val EndPoint = endPoint

    member val IsChallenging = true with get, set