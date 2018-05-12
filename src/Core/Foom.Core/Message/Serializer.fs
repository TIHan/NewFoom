namespace Foom.IO.Message.Channel

open System
open System.Collections.Concurrent
open Foom.IO.Message
open Foom.IO.Serializer

[<Sealed>]
type internal Serializer(lookup: MessagePoolBase []) =

    let beforeSerializedEvents = ConcurrentDictionary<byte, Event<Message>>()

    let buffer = Array.zeroCreate<byte> 65536
    let mutable nextSeqId = 0us

    member __.GetBeforeSerializedEvent(typeId: byte) =
        match beforeSerializedEvents.TryGetValue(typeId) with
        | (true, evt) -> evt.Publish
        | _ ->
            let evt = Event<Message>()
            if beforeSerializedEvents.TryAdd(typeId, evt) |> not then
                beforeSerializedEvents.[typeId].Publish
            else
                evt.Publish

    member __.SerializeMessage(msg: Message, willRecycle: bool, f: SpanDelegate) =
        match lookup.[int msg.TypeId] with
        | null -> failwith "Invalid message type."
        | pool ->
            msg.sequenceId <- nextSeqId
            nextSeqId <- nextSeqId + 1us

            let numBytesWritten = 
                try
                    match beforeSerializedEvents.TryGetValue(msg.TypeId) with
                    | (true, evt) -> evt.Trigger(msg)
                    | _ -> ()
                    msg.StartSerialize(Span(buffer))
                finally 
                    pool.Recycle(msg)

            f.Invoke(Span(buffer, 0, numBytesWritten))
