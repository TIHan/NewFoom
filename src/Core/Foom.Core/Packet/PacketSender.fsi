namespace Foom.IO.Packet

open System
open Foom.IO.Serializer

[<Sealed>]
type internal PacketSender =

    new : AckManager * PacketType -> PacketSender

    member Send : Span<byte> -> unit

    member SendAck : ack: uint16 -> unit

    member Time : TimeSpan with get, set

    member LoseEveryOtherPacket : bool with get, set

    // Is it possible to make process thread safe?
    member Process : SpanDelegate -> unit