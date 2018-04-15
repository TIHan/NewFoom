namespace Foom.Net

open System
open System.Net
open Foom.IO.Message
open Foom.IO.Packet

[<Sealed>]
type ConnectedClient(msgFactory: MessageFactory, channelLookup, udpServer: UdpServer, endPoint: IPEndPoint) =
    let stream = PacketStream()
    let sender = Sender(stream, channelLookup)
    let receiver = Receiver(stream, channelLookup)

    member __.SendMessage(msg: Message, channelId, willRecycle) =
        sender.EnqueueMessage(msg, channelId, willRecycle)

    member __.ReceivePacket(packet) =
        receiver.EnqueuePacket(packet)

    member __.ProcessReceivedMessages(f) =
        receiver.ProcessMessages(fun msg -> f msg)

    member __.SendPackets() =
        sender.SendPackets(fun packet ->
            udpServer.Send(Span.op_Implicit packet, endPoint)
        )

    member __.Time
        with get () = stream.Time
        and set value = stream.Time <- value

    member val EndPoint = endPoint