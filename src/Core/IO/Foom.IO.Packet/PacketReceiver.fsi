namespace Foom.IO.Packet

open System
open Foom.IO.Serializer

[<Sealed>]
type internal PacketReceiver =

    new : AckManager -> PacketReceiver

    member Receive : Span<byte> * SpanDelegate -> unit
