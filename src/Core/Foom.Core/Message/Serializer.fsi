﻿namespace Foom.IO.Message.Channel

open System
open Foom.IO.Message
open Foom.IO.Serializer

[<Sealed>]
type internal Serializer =

    new : lookup: MessagePoolBase [] -> Serializer

    member GetBeforeSerializedEvent : typeId: byte -> IEvent<Message>

    /// This will serialize a message to a byte span.
    member SerializeMessage : Message * willRecycle: bool * SpanDelegate -> unit

    interface IDisposable