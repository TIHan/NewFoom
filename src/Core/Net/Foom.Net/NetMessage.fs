namespace Foom.Net

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel
open Foom.IO.Serializer

[<AbstractClass>]
type NetMessage =
    inherit Message

    val mutable channelId : byte

    val mutable internal refCount : int

    new () = { channelId = 0uy; refCount = 0 }

    override this.Serialize(writer, stream) =
        writer.WriteByte(stream, &this.channelId) 
        this.NetSerialize(&writer, stream)

    override this.Reset() =
        this.channelId <- 0uy
        this.refCount <- 0
        this.NetReset()

    abstract NetSerialize : byref<Writer> * Span<byte> -> unit

    default __.NetSerialize(_, _) = ()

    abstract NetReset : unit -> unit

    default this.NetReset() = ()