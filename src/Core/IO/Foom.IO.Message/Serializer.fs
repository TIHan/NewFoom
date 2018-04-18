namespace Foom.IO.Message.Channel

open System
open Foom.IO.Message
open Foom.IO.Serializer

[<Sealed>]
type internal Serializer(lookup: MessagePoolBase []) =

    let buffer = Array.zeroCreate<byte> 65536
    let mutable nextSeqId = 0us

    member __.SerializeMessage(msg: Message, willRecycle: bool, f: SpanDelegate) =
        match lookup.[int msg.TypeId] with
        | null -> failwith "Invalid message type."
        | pool ->
            msg.sequenceId <- nextSeqId
            nextSeqId <- nextSeqId + 1us

            let numBytesWritten = msg.StartSerialize(Span(buffer))
            f.Invoke(Span(buffer, 0, numBytesWritten))

            if willRecycle then
                pool.Recycle(msg)