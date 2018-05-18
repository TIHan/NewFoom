namespace Foom.Net

open System
open Foom.IO.Message

[<RequireQualifiedAccess>]
type ServerMessage =
    | ClientConnected of ClientId
    | ClientDisconnected of ClientId
    | Message of ClientId * NetMessage

type IServer =

    abstract Start : unit -> unit

    abstract Stop : unit -> unit

    abstract SendMessage : NetMessage * ClientId * willRecycle: bool -> unit

    abstract SendMessage : NetMessage * willRecycle: bool -> unit

    abstract SendPackets : unit -> unit
    
    abstract ProcessMessages : (ServerMessage -> unit) -> unit

    abstract DisconnectClient : ClientId -> unit

    abstract CreateMessage : unit -> #Message

    abstract RecycleMessage : #NetMessage -> unit

    inherit IDisposable