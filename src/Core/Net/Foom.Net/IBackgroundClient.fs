namespace Foom.Net

open System
open Foom.IO.Message

type IBackgroundClient =

    abstract Connect : address: string * port: int -> unit

    abstract Disconnect : unit -> unit

    abstract SendMessage : NetMessage -> unit

    abstract CreateMessage<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)> : unit -> 'T

    abstract ListenForMessage<'T when 'T :> NetMessage> : unit -> unit

    abstract ProcessMessages : (ClientMessage -> unit) -> unit

    abstract OnException : IEvent<Exception>

    inherit IDisposable

