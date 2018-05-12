namespace Foom.IO.Packet

[<Sealed>]
type internal AckManager =

    new : unit -> AckManager

    member CalculateAckBits : ack: uint16 -> int

    member Ack : ack: uint16 * ackBits: int -> unit

    member ResendAck : ack: uint16 -> unit

    member Mark : seqId: uint16 -> unit

    member Acks : bool []

    member ReceivedAcks : bool []

    member ReceivedLatestAck : uint16 with get, set

    member PacketAcked : IEvent<uint16>

    member PacketResendRequested : IEvent<uint16>