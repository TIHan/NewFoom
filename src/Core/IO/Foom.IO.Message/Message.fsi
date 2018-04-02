namespace Foom.IO.Message

open System
open Foom.IO.Serializer
open System.Collections
open System.Collections.Concurrent

[<Struct>]
type internal MessageHeader =
    {
        ChannelId: byte
        SequenceId: uint16
        TypeId: uint16
    }
   
[<AbstractClass;AllowNullLiteral>]
type Message =

    new : unit -> Message

    static member GetChannelId : ReadOnlySpan<byte> -> byte

    member ChannelId : byte with get

    member ChannelId : byte with set // TODO: Don't make this public if we can.

    member SequenceId : uint16 with get

    member internal SequenceId : uint16 with set

    member TypeId : uint16 with get

    member internal TypeId : uint16 with set

    member IsRecycled : bool with get

    member internal IsRecycled : bool with set

    member internal IsRecyclable : bool with get, set

    member internal MainSerialize : Span<byte> -> int

    member internal MainDeserialize : ReadOnlySpan<byte> -> int

    member internal MainReset : unit -> unit

    abstract Serialize : byref<Writer> * Span<byte> -> unit

    default Serialize : byref<Writer> * Span<byte> -> unit

    abstract Deserialize : byref<Reader> * ReadOnlySpan<byte> -> unit

    default Deserialize : byref<Reader> * ReadOnlySpan<byte> -> unit

    abstract Reset : unit -> unit

    default Reset : unit -> unit

[<AbstractClass;AllowNullLiteral>]
type MessagePoolBase =

    new : unit -> MessagePoolBase

    abstract Create : unit -> Message

    abstract Recycle : Message -> unit

[<Sealed>]
type MessagePool<'T when 'T :> Message and 'T : (new : unit -> 'T)> =
    inherit MessagePoolBase

    new : typeId: uint16 * poolAmount: int -> MessagePool<'T>

[<Sealed>]
type TextMessage =
    inherit Message

    new : unit -> TextMessage

    member Text : string with get, set