namespace Foom.IO.Message

open System
open Foom.IO.Serializer
open System.Collections
open System.Collections.Concurrent

[<Struct>]
type internal MessageHeader =
    {
        TypeId: uint16
        SequenceId: uint16
    }
   
[<AbstractClass;AllowNullLiteral>]
type Message =

    new : unit -> Message

    [<DefaultValue>] val mutable internal sequenceId : uint16

    [<DefaultValue>] val mutable internal typeId : uint16

    member SequenceId : uint16 with get

    member TypeId : uint16 with get

    member IsRecycled : bool with get

    member internal IsRecycled : bool with set

    member internal IsRecyclable : bool with get, set

    member internal StartSerialize : Span<byte> -> int

    member internal StartDeserialize : Span<byte> -> int

    member internal MainSerialize : byref<Writer> * Span<byte> -> int

    member internal MainReset : unit -> unit

    abstract Serialize : byref<Writer> * Span<byte> -> unit

    default Serialize : byref<Writer> * Span<byte> -> unit

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