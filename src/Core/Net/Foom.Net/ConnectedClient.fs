namespace Foom.Net

open System
open System.Net
open Foom.IO.Message
open Foom.IO.Packet

[<Sealed>]
type ConnectedClient(msgFactory: MessageFactory, channelLookup, udpServer: UdpServer, endPoint: IPEndPoint, clientId: int) =
    let stream = PacketStream()
    let sender = Sender(stream, channelLookup)
    let receiver = Receiver(stream, channelLookup)

    member __.SendMessage(msg: Message, channelId, willRecycle) =
        sender.EnqueueMessage(msg, channelId, willRecycle)

    member __.OnReceivePacket(packet) =
        receiver.EnqueuePacket(packet)

    member __.TryReceiveMessages(f) =
        receiver.ProcessMessages(fun msg -> f clientId msg)

    member __.SendMessages() =
        let msg = msgFactory.CreateMessage<Heartbeat>()
        sender.EnqueueMessage(msg, DefaultChannelIds.Heartbeat, willRecycle = true)

        sender.SendPackets(fun packet ->
            udpServer.Send(Span.op_Implicit packet, endPoint)
        )

    member __.Time
        with get () = stream.Time
        and set value = stream.Time <- value

    member val EndPoint = endPoint

    member val ClientId = clientId