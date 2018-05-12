namespace Foom.IO.Packet

open System
open System.Collections.Generic
open Foom.IO.Serializer

[<AutoOpen>]
module internal PacketReceiverHelpers =

    let sequenceMoreRecent (s1 : uint16) (s2 : uint16) =
        (s1 > s2) &&
        (s1 - s2 <= UInt16.MaxValue / 2us)
            ||
        (s2 > s1) &&
        (s2 - s1 > UInt16.MaxValue / 2us)

[<Sealed>]
type internal PacketReceiver(acks: AckManager) =

    let pool = PacketPool()
    let frags = Array.zeroCreate PacketConstants.PacketPoolAmount

    let queue = Queue()

    let buffer = Array.zeroCreate<byte> (1024 * 1024)
    let mutable bufferPosition = 0

    let receiveAck seqId =
        acks.ReceivedAcks.[int seqId] <- true

        if sequenceMoreRecent seqId acks.ReceivedLatestAck then
            acks.ReceivedLatestAck <- seqId

        // TODO: This may need to be put somewhere else
        elif not (sequenceMoreRecent seqId (acks.ReceivedLatestAck - 32us)) then
            printfn "resending ack: %A" seqId
            acks.ResendAck(seqId)

    let process' (f: SpanDelegate) =
        while queue.Count > 0 do
            let seqId = queue.Dequeue()

            let packet = pool.TryGet(seqId)
            if packet.IsEmpty then ()
            else

            let mutable header = Unchecked.defaultof<PacketHeader>
            Writer(true).Write(packet, &header)
            if not packet.IsEmpty then
                let packetData = packet.Slice(sizeof<PacketHeader>)
                let packetData = packetData.Slice(0, packetData.Length - pool.GetRemainingLength(seqId))
                if header.IsFragmented then
                    packetData.CopyTo(Span(buffer, bufferPosition, packetData.Length))
                    bufferPosition <- bufferPosition + packetData.Length
                    if header.IsLastFragment then
                        f.Invoke(Span(buffer, 0, bufferPosition))
                        bufferPosition <- 0
                else
                    f.Invoke(packetData)
        bufferPosition <- 0

    // TODO: We need to validate this packet to make sure it isn't malicious. 
    //     e.g. older packets, duplicate packets
    member this.Receive(incomingPacket: Span<byte>, f) =
        let mutable header = Unchecked.defaultof<PacketHeader>
        Writer(true).Write(incomingPacket, &header)

        let packetType' = header.PacketType
        let seqId = header.SequenceId
        let fragId = int header.FragmentId
        let fragCount = int header.FragmentCount

        // Validation
        if fragId >= 32 || fragCount >= 32 then
            failwith "Invalid fragment header."
        
        let packet = pool.Create(&header)
        incomingPacket.CopyTo(packet)
        pool.DecreaseRemainingLength(seqId, incomingPacket.Slice(sizeof<PacketHeader>).Length)

        if packetType' = PacketType.Reliable then
            receiveAck seqId

        // Unreliable and reliable packets will send ack info for reliable packets
        acks.Ack(header.Ack, header.AckBits)

        // Ack packets don't do anything with data. They are purely for acks.
        if packetType' = PacketType.Ack then ()
        else

        if header.IsFragmented then
            let mainSeqId = header.MainSequenceId
            let index = getIndex mainSeqId

            let fragMask = frags.[index] ||| (1 <<< fragId)
            let finalMask = ~~~(-1 <<< (fragCount + 1))

            frags.[index] <- fragMask
            // We have all fragments
            if fragMask = finalMask then
                frags.[index] <- 0
                for i = int mainSeqId to int mainSeqId + fragCount do
                    queue.Enqueue(uint16 i)
        else
            queue.Enqueue(seqId)        

        process' f