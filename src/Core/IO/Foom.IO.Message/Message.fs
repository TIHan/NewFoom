namespace Foom.IO.Message

open System
open Foom.IO.Serializer
open System.Collections
open System.Collections.Concurrent

[<Struct>]
type MessageHeader =
    {
        TypeId: uint16
        SequenceId: uint16
    }

[<AbstractClass;AllowNullLiteral>]
type Message() =

    member val SequenceId = 0us with get, set

    member val TypeId = 0us with get, set

    member val IsRecyclable = false with get, set

    member val IsRecycled = true with get, set

    member this.MainSerialize(stream: Span<byte>) =
        let mutable writer = Writer()

        let mutable header = 
            {
                SequenceId = this.SequenceId
                TypeId = uint16 this.TypeId
            }
        writer.Write(stream, &header)

        this.Serialize(&writer, stream)

        writer.position

    member this.MainDeserialize(stream: Span<byte>) =
        let mutable reader = Reader()

        let header = reader.Read<MessageHeader> stream
        this.SequenceId <- header.SequenceId
        this.TypeId <- header.TypeId

        this.Deserialize(&reader, stream)

        reader.position

    member this.MainReset() =
        this.SequenceId <- 0us
        this.TypeId <- 0us
        this.IsRecyclable <- false
        this.IsRecycled <- true

        this.Reset()

    abstract Serialize : byref<Writer> * Span<byte> -> unit

    default __.Serialize(_writer, _stream) = ()

    abstract Deserialize : byref<Reader> * Span<byte> -> unit

    default __.Deserialize(_reader, _stream) = ()

    abstract Reset : unit -> unit

    default __.Reset() = ()

[<AbstractClass;AllowNullLiteral>]
type MessagePoolBase() =

    abstract Create : unit -> Message

    abstract Recycle : Message -> unit

[<Sealed>]
type MessagePool<'T when 'T :> Message and 'T : (new : unit -> 'T)>(typeId: uint16, poolAmount) =
    inherit MessagePoolBase()

    let msgs = ConcurrentStack(Array.init poolAmount (fun _ -> new 'T()))

    override __.Create() = 
        let msg = 
            match msgs.TryPop() with
            | (true, msg) ->
                let msg = msg :> Message
                msg.IsRecyclable <- true
                msg
            | _ ->
                printfn "MessagePool, %s, reached its pool count." typeof<'T>.FullName
                let msg = new 'T() :> Message
                msg.IsRecyclable <- false
                msg
        msg.TypeId <- typeId
        msg.IsRecycled <- false
        msg

    override __.Recycle msg =
        if msg.IsRecyclable && (not msg.IsRecycled) then
            msg.MainReset()
            msgs.Push(msg :?> 'T)
