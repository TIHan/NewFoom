namespace Foom.IO.Packet

open System
open System.Collections.Generic
open Foom.IO.Serializer

[<AutoOpen>]
module PacketSenderHelpers =

    module ErrorStrings =

        let DataTooBigForFragments() = "Data is too big for fragmented packets."

        let DataIsEmpty() = "Data is empty."

        let PacketOverflow() = "Too much data was collected. Packet overflow"

        let ReliablePacketTimedOut() = "Reliable packet timed out."

    let inline addDataToPacket (data: Span<byte>) start length (packet: Span<byte>) startPacket =
        data.Slice(start, length).CopyTo(packet.Slice(sizeof<PacketHeader> + startPacket))

    let inline processSeqId (pool: PacketPool) (loseEveryOtherPacket: bool) (f: SpanDelegate) seqId =
        let packet = pool.TryGet(seqId)
        if packet.IsEmpty then 
            failwith (ErrorStrings.PacketOverflow())
        else
            let header = Reader().Read<PacketHeader> (packet)
            if not packet.IsEmpty then
                if not (loseEveryOtherPacket && (seqId % 2us = 0us)) then
                    f.Invoke (packet.Slice(0, packet.Length - pool.GetRemainingLength seqId))

[<Sealed>]
type internal PacketSender(acks: AckManager, packetType) =
    let pool = PacketPool()
    let queue = Queue()
    let resendQueue = Queue()

    let add seqId data start length packet startPacket =
        addDataToPacket data start length packet startPacket
        pool.DecreaseRemainingLength(seqId, length)

        if packetType = PacketType.Reliable then
            acks.Mark(seqId)

    let resendTime = TimeSpan.FromSeconds(1.)
    let timeout = TimeSpan.FromSeconds(3.)
    let mutable time = TimeSpan.Zero

    let fragment seqId (data: Span<byte>) fragCount =
        for i = 0 to fragCount do
            let seqId = seqId + uint16 i
            let buf = pool.Create(packetType, seqId, byte i, byte fragCount, acks.ReceivedLatestAck, acks.CalculateAckBits(acks.ReceivedLatestAck))

            let start = i * PacketConstants.PacketDataSize
            let length =
                if i = fragCount then data.Length - (fragCount * PacketConstants.PacketDataSize)
                else PacketConstants.PacketDataSize

            add seqId data start length buf 0

            queue.Enqueue(seqId)

        uint16 fragCount + 1us

    let rec send seqId (data: Span<byte>) =
        let fragmentCount = (data.Length / PacketConstants.PacketDataSize)

        // Validation
        if fragmentCount >= 32 then 
            failwith (ErrorStrings.DataTooBigForFragments())

        match pool.GetRemainingLength seqId, fragmentCount with

        // New Packets
        | PacketConstants.PacketDataSize, 0 ->
            let packet = pool.Create(packetType, seqId, 0uy, 0uy, acks.ReceivedLatestAck, acks.CalculateAckBits(acks.ReceivedLatestAck))

            let start = 0
            let length = data.Length

            add seqId data start length packet 0

            queue.Enqueue(seqId)
            1us

        | PacketConstants.PacketDataSize, fragCount ->
            fragment seqId data fragCount

        // Looks like we need a new packet.
        | n, 0 when data.Length > n -> 
            send (seqId + 1us) data

        // Re-use existing packet because it has space
        | n, 0 ->
            System.Diagnostics.Debug.Assert(n >= 0)

            let packet = pool.TryGet(seqId)
            if packet.IsEmpty then
                pool.Reset(seqId)
                send seqId data
            else

            let start = 0
            let length = data.Length

            System.Diagnostics.Debug.Assert(not packet.IsEmpty)
            add seqId data start length packet (PacketConstants.PacketDataSize - n)
            1us

        | n, fragCount ->
            // For fragments, we want new packets.
            fragment (seqId + 1us) data fragCount

    let mutable nextSeqId = 0us

    member this.Send(data) = 
        let count = send nextSeqId data 
        nextSeqId <- nextSeqId + count

    member this.SendAck(ack) =
        let seqId = nextSeqId
        let _ = pool.Create(PacketType.Ack, seqId, 0uy, 0uy, ack, acks.CalculateAckBits(ack))
        nextSeqId <- nextSeqId + 1us
        queue.Enqueue(seqId)

    member this.Time
        with get () = time
        and set value = time <- value

    member val LoseEveryOtherPacket = false with get, set

    member this.Process (f: SpanDelegate) =
        // Resending packets have priority
        let resendCount = resendQueue.Count 
        let mutable processedCount = 0
        while processedCount < resendCount do
            processedCount <- processedCount + 1

            let struct(seqId, t) as req = resendQueue.Dequeue()

            // If we have an ack, then we are finished, don't send.
            if not acks.Acks.[int seqId] then

                if time >= t + timeout then
                    failwith (ErrorStrings.ReliablePacketTimedOut())

                if time >= t + resendTime then
                    printfn "resentin shit %A %A %A %A" seqId time t resendTime
                    processSeqId pool this.LoseEveryOtherPacket f seqId

                resendQueue.Enqueue(req)

        while queue.Count > 0 do
            let seqId = queue.Dequeue()

            if packetType = PacketType.Reliable then
                resendQueue.Enqueue(struct(seqId, time))

            processSeqId pool this.LoseEveryOtherPacket f seqId
