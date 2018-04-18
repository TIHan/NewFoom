namespace Foom.IO.Message.Channel

open System
open Foom.IO.Message

type internal ReceiverType =
    | Normal = 0
    | Sequenced = 1
    | Ordered = 2

[<Sealed>]
type internal Receiver =

    new : ReceiverType * lookup: MessagePoolBase [] -> Receiver

    member Enqueue : Span<byte> -> int

    member Process : (Message -> unit) -> unit