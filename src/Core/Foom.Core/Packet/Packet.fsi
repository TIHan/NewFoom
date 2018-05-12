[<AutoOpen>]
module internal Foom.IO.Packet.Packet

open System

type PacketType =
    | Unreliable = 0uy
    | Reliable = 1uy
    | Ack = 2uy

[<Struct>]
type PacketHeader =
    {
        PacketType: PacketType
        SequenceId: uint16
        FragmentId: byte
        FragmentCount: byte
        Ack: uint16
        AckBits: int
    }

    member MainSequenceId : uint16

    member IsFragmented : bool

    member IsLastFragment : bool

val inline getIndex : seqId: uint16 -> int

val sequenceMoreRecent : uint16 -> uint16 -> bool

[<Sealed>]
type PacketPool =

    new : unit -> PacketPool

    member Create : PacketType * seqId: uint16 * fragId: byte * fragCount: byte * ack: uint16 * ackBits: int -> Span<byte>

    member Create : byref<PacketHeader> -> Span<byte>

    member TryGet : seqId: uint16 -> Span<byte>

    member Reset : seqId: uint16 -> unit

    member GetRemainingLength : seqId: uint16 -> int

    member DecreaseRemainingLength : seqId: uint16 * amount: int -> unit