namespace Foom.IO.Message

open System
open Foom.IO.Serializer
open System.Collections
open System.Collections.Concurrent

[<AbstractClass;AllowNullLiteral>]
type Message() =

    [<DefaultValue>] val mutable typeId : byte

    [<DefaultValue>] val mutable sequenceId : uint16

    [<DefaultValue>] val mutable refCount : int

    member this.TypeId = this.typeId

    member this.SequenceId = this.sequenceId

    member val IsRecyclable = false with get, set

    member val IsRecycled = true with get, set

    member this.StartSerialize(stream: Span<byte>) =
        let mutable writer = Writer()

        this.MainSerialize(&writer, stream)

    member this.StartDeserialize(stream: Span<byte>) =
        let mutable writer = Writer(true)

        this.MainSerialize(&writer, stream)

    member this.MainSerialize(writer: byref<Writer>, stream: Span<byte>) =
        writer.Write(stream, &this.typeId)
        writer.Write(stream, &this.sequenceId)

        this.Serialize(&writer, stream)

        writer.position

    member this.MainReset() =
        this.typeId <- 0uy
        this.sequenceId <- 0us
        this.refCount <- 0
        this.IsRecyclable <- false
        this.IsRecycled <- true

        this.Reset()

    member this.IncrementRefCount() =
        System.Threading.Interlocked.Increment(&this.refCount) |> ignore

    abstract Serialize : byref<Writer> * Span<byte> -> unit

    default __.Serialize(_writer, _stream) = ()

    abstract Reset : unit -> unit

    default __.Reset() = ()

[<AbstractClass;AllowNullLiteral>]
type MessagePoolBase() =

    abstract Create : unit -> Message

    abstract Recycle : Message -> unit

[<Sealed>]
type MessagePool<'T when 'T :> Message and 'T : (new : unit -> 'T)>(typeId, poolAmount) =
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
        msg.typeId <- typeId
        msg.IsRecycled <- false
        msg.IncrementRefCount()
        msg

    override __.Recycle msg =
        if msg.IsRecyclable then
            let refCount = System.Threading.Interlocked.Decrement(&msg.refCount)
            if refCount < 0 then
                failwithf "RefCount on message, %A, is below zero." (msg.GetType().Name)

            if refCount = 0 then
                msg.MainReset()
                msgs.Push(msg :?> 'T)
