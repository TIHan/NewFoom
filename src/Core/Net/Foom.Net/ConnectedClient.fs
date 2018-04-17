namespace Foom.Net

open System
open System.Net
open Foom.IO.Message
open Foom.IO.Packet

[<Sealed>]
type ConnectedClient(msgFactory: MessageFactory, channelLookup, udpServer: UdpServer, endPoint: IPEndPoint) =
    let stream = PacketStream()
    let netChannel = NetChannel(stream, channelLookup)

    let heartbeat () =
        let msg = msgFactory.CreateMessage<Heartbeat>()
        netChannel.SendMessage(msg, DefaultChannelIds.Heartbeat, willRecycle = true)

    member __.SendMessage(msg, channelId, willRecycle) =
        netChannel.SendMessage(msg, channelId, willRecycle)

    member __.ReceivePacket(packet) =
        netChannel.ReceivePacket(packet)

    member __.ProcessReceivedMessages(f) =
        netChannel.ProcessReceivedMessages(f)

    member __.SendPackets() =
        heartbeat ()
        netChannel.SendPackets(fun packet ->
            udpServer.Send(Span.op_Implicit packet, endPoint)
        )

    member __.Time
        with get () = stream.Time
        and set value = stream.Time <- value

    member val EndPoint = endPoint