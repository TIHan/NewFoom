namespace Foom.IO.Packet

open System

type PacketDeliveryType =
    | Unreliable = 0
    | Reliable = 1

[<Sealed>]
type PacketStream() =
    let acks = AckManager()

    let unreliableSender = PacketSender(acks, PacketType.Unreliable)
    let reliableSender = PacketSender(acks, PacketType.Reliable)

    let unreliableReceiver = PacketReceiver(acks)
    let reliableReceiver = PacketReceiver(acks)

    do
        acks.PacketResendRequested.Add(fun ack ->
            unreliableSender.SendAck(ack)
        )

    member __.Send(data, packetDeliveryType) =
        match packetDeliveryType with
        | PacketDeliveryType.Unreliable -> unreliableSender.Send(data)
        | PacketDeliveryType.Reliable -> reliableSender.Send(data)
        | _ -> failwith "Invalid packet type."

    member __.ProcessSending(f) =
        reliableSender.Process(f)
        unreliableSender.Process(f)

    member __.Receive(packet: Span<byte>, f) =
        let typ = packet.[0]
        match typ with
        | 0uy -> unreliableReceiver.Receive(packet, f)
        | 1uy -> reliableReceiver.Receive(packet, f)
        | 2uy -> unreliableReceiver.Receive(packet, f)
        | _ -> failwith "Invalid packet type."

    member __.LoseEveryOtherPacket
        with get () = unreliableSender.LoseEveryOtherPacket
        and set value = 
            reliableSender.LoseEveryOtherPacket <- value
            unreliableSender.LoseEveryOtherPacket <- value

    member __.Time
        with get () = unreliableSender.Time
        and set value = 
            reliableSender.Time <- value
            unreliableSender.Time <- value
