namespace Foom.IO.Packet

open System
    
[<Sealed>]
type internal AckManager() =

    [<Literal>]
    let MaxSeq = 65536

    [<Literal>]
    let MaxSeqId = 65535us

    let halfMaxSeq = MaxSeq / 2

    let halfMaxSeqId = MaxSeqId / 2us

    let packetAcked = Event<uint16>()
    let packetResendRequestedEvent = Event<uint16>()
    let acks = Array.init MaxSeq (fun _ -> true)
    let acksTime = Array.init MaxSeq (fun _ -> TimeSpan.Zero)
    let receivedAcks = Array.init MaxSeq (fun _ -> false)
    let mutable receivedLatestAck = MaxSeqId
    let mutable currentTime = TimeSpan.Zero

    let getAckIndex ack i =
        let index = int ack - i
        if index < 0 then
            receivedAcks.Length + index
        else
            index

    member __.CalculateAckBits(ack) =
        let mutable ackBits = 0

        for i = 0 to 31 do
            let index = getAckIndex ack i
            let isOn = if receivedAcks.[index] then 1 else 0
            ackBits <- ackBits ||| (isOn <<< i)
        ackBits

    member this.Ack(ack: uint16, ackBits: int) =
        for i = 0 to 31 do
            let index = getAckIndex ack i
            if not acks.[index] then
                if ackBits &&& (1 <<< i) <> 0 then
                    acks.[index] <- true
                    packetAcked.Trigger(uint16 index)

    //member __.Ack(ack: uint16) =
        //if not acks.[int ack] then
            //acks.[int ack] <- true
            //packetAcked.Trigger(ack)

    member __.ResendAck(ack: uint16) =
        packetResendRequestedEvent.Trigger(ack)

    member __.Mark(seqId: uint16) =
        let index = int seqId
        acks.[index] <- false
        acksTime.[index] <- currentTime

    member __.Acks = acks
    member __.ReceivedAcks = receivedAcks
    member __.ReceivedLatestAck
        with get() = receivedLatestAck
        and set value = 
            if value < receivedLatestAck then
                Array.Clear(receivedAcks, 0, halfMaxSeq)
            if value > halfMaxSeqId && receivedLatestAck <= halfMaxSeqId then
                Array.Clear(receivedAcks, halfMaxSeq, halfMaxSeq)
            receivedAcks.[int value] <- true
            receivedLatestAck <- value

    member __.PacketAcked = packetAcked.Publish

    member __.PacketResendRequested = packetResendRequestedEvent.Publish
