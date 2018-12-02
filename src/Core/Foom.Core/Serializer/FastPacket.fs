[<AutoOpen>]
module Foom.IO.FastPacket

open System
open System.Buffers
open System.Collections.Generic
open System.Collections.Concurrent

open Foom.IO.Serializer
open System.Runtime.InteropServices

[<RequireQualifiedAccess>]
module PacketConstants =

    [<Literal>]
    let MaxFragmentDataSize = 1024

    [<Literal>]
    let PoolAmount = 1024

    [<Literal>]
    let MaxFragments = 32

[<Struct>]
type PacketHeader(
                    sequenceId: uint16,
                    dataSize: uint32,
                    timeStamp: TimeSpan,
                    flags: uint32,
                    fragmentId: byte,
                    fragmentCount: byte,
                    fragmentDataSize: uint32) =

    member __.SequenceId = sequenceId

    member __.DataSize = dataSize

    member __.TimeStamp = timeStamp

    member __.Flags = flags

    member __.FragmentId = fragmentId

    member this.FragmentIndex = int (this.FragmentId - 1uy)

    member __.FragmentCount = fragmentCount

    member __.FragmentDataSize = fragmentDataSize

    member this.MainSequenceId = this.SequenceId - uint16 this.FragmentId

    member this.IsDataFragmented = this.FragmentCount > 1uy

    member this.IsLastFragment = this.FragmentId = this.FragmentCount

[<AutoOpen>]
module PacketSenderHelpers =

    module ErrorStrings =

        let DataTooBigForFragments() = "Data is too big for fragmented packets."

        let DataIsEmpty() = "Data is empty."

        let PacketOverflow() = "Too much data was collected. Packet overflow"

        let ReliablePacketTimedOut() = "Reliable packet timed out."

[<Struct>]
type Packet =
    private {
        length: int
        packetBytes: byte []
    }

    member this.AsSpan = Span(this.packetBytes, 0, this.length)

type ForEachSpanDelegate = delegate of Span<byte> -> unit

[<Struct>]
type Packets =
    private {
        count: int
        packets: Packet []
    }

    member this.Count = this.count

    member this.ForEachReadOnlySpan(f: ForEachSpanDelegate) =
        for i = 0 to this.count do
            f.Invoke(this.packets.[i].AsSpan)

let createPacketHeader seqId dataLength timeStamp flags fragId fragCount =
    let length =
        if fragId = fragCount then dataLength - ((fragCount - 1) * PacketConstants.MaxFragmentDataSize)
        else PacketConstants.MaxFragmentDataSize

    PacketHeader(seqId, uint32 dataLength, timeStamp, flags, byte fragId, byte fragCount, uint32 length)

let createPacket (packetBytesPool: ArrayPool<byte>) (packetHeader: PacketHeader) (data: ReadOnlySpan<byte>) =
    let start = packetHeader.FragmentIndex * PacketConstants.MaxFragmentDataSize
    let length = int packetHeader.FragmentDataSize
    let packetBytes = packetBytesPool.Rent(length + sizeof<PacketHeader>)
    let packetBytesSpan = Span(packetBytes)

    // Write header and data
    packetBytesSpan.Write<PacketHeader>(0, &packetHeader)
    data.Slice(start, length).CopyTo(packetBytesSpan.Slice(sizeof<PacketHeader>))

    { length = length + sizeof<PacketHeader>; packetBytes = packetBytes }

[<Sealed>]
type PacketFactory() =

    let packetBytesPool = ArrayPool<byte>.Create(sizeof<PacketHeader> * PacketConstants.MaxFragmentDataSize, PacketConstants.PoolAmount)
    let packetsPool = ArrayPool<Packet>.Create(PacketConstants.MaxFragments, PacketConstants.PoolAmount)

    member __.CreatePackets(data: ReadOnlySpan<byte>, seqId, timeStamp, flags) =
        let fragCount = (data.Length / PacketConstants.MaxFragmentDataSize) + 1

        // Validation
        if fragCount > PacketConstants.MaxFragments then 
            failwith (ErrorStrings.DataTooBigForFragments())

        let packets = packetsPool.Rent(fragCount)

        for fragId = 1 to fragCount do
            let seqId = seqId + uint16 fragId
            let packetHeader = createPacketHeader seqId data.Length timeStamp flags fragId fragCount
            packets.[packetHeader.FragmentIndex] <- createPacket packetBytesPool packetHeader data

        { count = fragCount; packets = packets }

    member __.RecyclePackets(packets: Packets) =
        for i = 0 to packets.count - 1 do
            packetBytesPool.Return(packets.packets.[i].packetBytes)
        packetsPool.Return(packets.packets)
     
[<Struct>]
type Data =
    private {
        length: int
        dataBytes: byte []
        packets: Packet []
    }

    member this.AsSpan = Span(this.dataBytes, 0, this.length)

    member this.ForEachPacket(f) =
        for packet in this.packets do
            f packet

[<Sealed>]
type DataDefragmenter() =

    let dataBytesPool = ArrayPool<byte>.Shared
    let packetsPool = ArrayPool<Packet>.Create(PacketConstants.MaxFragments, PacketConstants.PoolAmount)

    let frags = ConcurrentDictionary<uint16, struct (int * Packet [])>()

    member this.TryGetData(packets: Packets) =
        let mutable result = ValueNone
        for i = 0 to packets.count - 1 do
            result <- this.TryGetData(packets.packets.[i])
        result

    member __.TryGetData(packet: Packet) =
        let header = packet.AsSpan.AsReadOnly.Read<PacketHeader>(0)

        let fragId = int header.FragmentId
        let fragCount = int header.FragmentCount

        // Validation
        if fragId > PacketConstants.MaxFragments || fragCount > PacketConstants.MaxFragments || fragId <= 0 || fragCount <= 0 then
            failwith "Invalid fragment header."

        if header.IsDataFragmented then
            let mainSeqId = header.MainSequenceId

            let struct(frag, packets) =
                match frags.TryGetValue(mainSeqId) with
                | true, x -> x
                | _ -> 
                    let frag = 0
                    let packets = packetsPool.Rent(fragCount)
                    frags.[mainSeqId] <- struct(frag, packets)
                    struct(frag, packets)

            let fragIndex = fragId - 1
            let fragMask = frag ||| (1 <<< fragIndex)
            let finalMask = ~~~(-1 <<< (fragCount))

            packets.[fragIndex] <- packet
            frags.[mainSeqId] <- struct(fragMask, packets)

            // We have all fragments
            if fragMask = finalMask then
                frags.TryRemove(mainSeqId) |> ignore

                let dataBytes = dataBytesPool.Rent(int header.DataSize)
                let dataBytesSpan = Span(dataBytes, 0, int header.DataSize)

                for i = 0 to fragCount - 1 do
                    let packet = packets.[i]
                    let packetHeader = packet.AsSpan.AsReadOnly.Read<PacketHeader>(0)

                    let start = packetHeader.FragmentIndex * PacketConstants.MaxFragmentDataSize
                    let packetSpan = packet.AsSpan
                    packetSpan.Slice(sizeof<PacketHeader>, packetSpan.Length - sizeof<PacketHeader>).CopyTo(dataBytesSpan.Slice(start, int packetHeader.FragmentDataSize))

                ValueSome({ length = int header.DataSize; dataBytes = dataBytes; packets = packets })
            else
                ValueNone
        else
            let dataBytes = dataBytesPool.Rent(int header.DataSize)
            let packets = packetsPool.Rent(1)
            ValueSome({ length = int header.DataSize; dataBytes = dataBytes; packets = packets })
            
    member __.RecycleData(data: Data) =
        dataBytesPool.Return(data.dataBytes)
        packetsPool.Return(data.packets)
