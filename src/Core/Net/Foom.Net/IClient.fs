namespace Foom.Net

open System
open Foom.IO.Message

[<Struct;RequireQualifiedAccess>]
type ClientMessage =
    | ConnectionAccepted of clientId: ClientId
    | DisconnectAccepted
    | Message of NetMessage

type IClientUpdate =

    abstract OnMessageReceived : ClientMessage -> unit

    abstract OnAfterMessagesReceived : unit -> unit

type IClient =

    abstract IsConnected : bool

    abstract Connect : address: string * port: int -> unit

    abstract Disconnect : unit -> unit

    abstract SendMessage : NetMessage -> unit

    abstract CreateMessage<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)> : unit -> 'T

    abstract Update : interval: TimeSpan * IClientUpdate -> unit

    abstract GetBeforeSerializedEvent<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)> : unit -> IEvent<'T>

    abstract GetBeforeDeserializedEvent<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)> : unit -> IEvent<'T>

    inherit IDisposable

