namespace Foom.IO.Packet

open System
open Foom.IO.Serializer

type PacketDeliveryType =
    | Unreliable = 0
    | Reliable = 1

[<Sealed>]
type PacketStream =

    new : unit -> PacketStream

    member Send : Span<byte> * PacketDeliveryType -> unit

    member ProcessSending : SpanDelegate -> unit

    member Receive : Span<byte> * SpanDelegate -> unit

    member LoseEveryOtherPacket : bool with get, set

    member Time : TimeSpan with get, set