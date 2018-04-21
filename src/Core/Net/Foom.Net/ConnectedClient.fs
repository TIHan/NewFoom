namespace Foom.Net

open System
open System.Net
open Foom.IO.Message
open Foom.IO.Packet

[<Sealed>]
type ConnectedClient(msgFactory: MessageFactory, typeToChannelMap, channelLookup, udpServer: UdpServer, endPoint: IPEndPoint) =
    let stream = PacketStream()
    let netChannel = NetChannel(stream, typeToChannelMap, channelLookup)

    let heartbeat () =
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