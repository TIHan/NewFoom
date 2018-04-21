﻿namespace Foom.IO.Message.Channel

open System
open System.Threading
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Message
open Foom.IO.Serializer

type internal ReceiverType =
    | Normal = 0
    | Sequenced = 1
    | Ordered = 2

[<Sealed>]
type internal Receiver(receiverType: ReceiverType, lookup: MessagePoolBase []) =

    let sequenceMoreRecent (s1 : uint16) (s2 : uint16) =
        (s1 > s2) &&
        (s1 - s2 <= UInt16.MaxValue / 2us)
            ||
        (s2 > s1) &&
        (s2 - s1 > UInt16.MaxValue / 2us)

    let msgQueue = ConcurrentQueue()
    let msgQueueLock = obj ()
    let mutable latest = 65535us
    let mutable latestSequencedMsg = null

    // TODO: This is LOH, find a better way
    let msgs = Array.init 65536 (fun _ -> null)
    let mutable nextOrdered = 0us

    let receive data (pool: MessagePoolBase) =
        let msg = pool.Create()

        let numBytesRead = msg.StartDeserialize(data)
        let seqId = msg.SequenceId

        match receiverType with
        | ReceiverType.Ordered ->
            msgs.[int seqId] <- msg

            if nextOrdered = seqId then
                msgs.[int nextOrdered] <- null
                nextOrdered <- nextOrdered + 1us
                msgQueue.Enqueue(msg)

                let mutable canBreakOut = false
                while not canBreakOut do
                    let msg = msgs.[int nextOrdered]
                    if msg = null then
                        canBreakOut <- true
                    else
                        msgs.[int nextOrdered] <- null
                        nextOrdered <- nextOrdered + 1us
                        msgQueue.Enqueue(msg)
        | ReceiverType.Sequenced ->
            if msg.SequenceId = latest then
                let msgToRecycle = Interlocked.Exchange<Message>(&latestSequencedMsg, msg)
                if msgToRecycle <> null then
                    pool.Recycle(msgToRecycle)

            else
                pool.Recycle(msg)

        | _ -> 
            msgQueue.Enqueue(msg)
        numBytesRead

    member this.Enqueue(data: Span<byte>) =
        let typeId = data.[0]
        match lookup.[int typeId] with
        | null -> failwithf "Can't find message type with TypeId, %i." typeId
        | pool ->
            let seqId = LittleEndian.read16 data 1

            if sequenceMoreRecent seqId latest then
                latest <- seqId

            receive data pool

    member __.Process(f: Message -> unit) =
        if receiverType = ReceiverType.Sequenced then
            let msg = Interlocked.Exchange<Message>(&latestSequencedMsg, null)

            // It is possible for a message to be null if no message has been queued up.
            if msg <> null then
                f msg
        else
            let maxCount = msgQueue.Count
            let mutable count = 0
            let mutable msg = null
            while msgQueue.TryDequeue(&msg) && count < maxCount do
                f msg
                count <- count + 1