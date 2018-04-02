[<AutoOpen>]
module Foom.IO.Packet.Packet

open System
open Foom.IO.Serializer

type PacketType =
    | Unreliable = 0uy
    | Reliable = 1uy
    | Ack = 2uy

[<Struct>]
type PacketHeader =
    {
        PacketType: PacketType
        SequenceId: uint16
        FragmentId: byte
        FragmentCount: byte
        Ack: uint16
        AckBits: int
    }

    member this.MainSequenceId = this.SequenceId - uint16 this.FragmentId

    member this.IsFragmented = this.FragmentCount > 0uy

    member this.IsLastFragment = this.FragmentId = this.FragmentCount

let PacketSize = PacketConstants.PacketDataSize + sizeof<PacketHeader>

let inline getIndex(seqId: uint16) = int seqId % PacketConstants.PacketPoolAmount

let sequenceMoreRecent (s1 : uint16) (s2 : uint16) =
    (s1 > s2) &&
    (s1 - s2 <= UInt16.MaxValue / 2us)
        ||
    (s2 > s1) &&
    (s2 - s1 > UInt16.MaxValue / 2us)

[<Sealed>]
type PacketPool() =

    let buffer =          Array.zeroCreate<byte> (PacketSize * PacketConstants.PacketPoolAmount)
    let seqIds =          Array.zeroCreate PacketConstants.PacketPoolAmount
    let remainingLength = Array.init PacketConstants.PacketPoolAmount (fun _ -> PacketConstants.PacketDataSize)

    member this.Create(packetType, seqId: uint16, fragId: byte, fragCount: byte, ack: uint16, ackBits: int) =
        let mutable header = 
            {
                PacketType = packetType
                SequenceId = seqId
                FragmentId = fragId
                FragmentCount = fragCount
                Ack = ack
                AckBits = ackBits
            }
        this.Create(&header)

    member __.Create(header: byref<PacketHeader>) =
        let i = getIndex header.SequenceId
        seqIds.[i] <- header.SequenceId
        remainingLength.[i] <- PacketConstants.PacketDataSize

        let data = Span(buffer, i * PacketSize, PacketSize)
        let mutable writer = Writer()
        writer.Write(data, &header)
        data

    member __.TryGet(seqId) =
        let i = getIndex seqId
        if seqIds.[i] = seqId then
            Span(buffer, i * PacketSize, PacketSize)
        else
            Span.Empty

    member __.Reset(seqId) =
        let i = getIndex seqId
        seqIds.[i] <- seqId
        remainingLength.[i] <- PacketConstants.PacketDataSize

    member __.GetRemainingLength seqId =
        let index = getIndex seqId
        remainingLength.[index]

    member __.DecreaseRemainingLength(seqId: uint16, amount: int) =
        let index = getIndex seqId
        remainingLength.[index] <- remainingLength.[index] - amount