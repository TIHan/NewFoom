﻿[<AutoOpen>]
module Foom.IO.FastPacket

open System
open System.Buffers
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Threading

open Foom.IO.Serializer
open System.Runtime.InteropServices

[<RequireQualifiedAccess>]
module PacketConstants =

    [<Literal>]
    let MaxFragmentDataSize = 1024

    [<Literal>]
    let PoolAmount = 1024

[<Struct>]
type PacketHeader =
    {
        SequenceId: uint32
        SequenceVersion: uint32
        DataSize: int
        FragmentIndex: int
        FragmentCount: int
        FragmentDataSize: int
    }

    member this.MainSequenceId = this.SequenceId - uint32 this.FragmentIndex

    member this.IsDataFragmented = this.FragmentCount > 1

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
        packetBytes: byte []
    }

    member this.Header 
        with get () =
            ReadOnlySpan(this.packetBytes, 0, sizeof<PacketHeader>).Read<PacketHeader>(0)

    member this.AsSpan = 
        Span(this.packetBytes, 0, this.Header.FragmentDataSize + sizeof<PacketHeader>)

type ForEachSpanDelegate = delegate of Span<byte> -> unit

[<Struct>]
type Packets =
    private {
        packets: Packet []
    }

    member this.MainHeader = this.packets.[0].Header

    member private this.LastPacket = this.packets.[this.packets.Length - 1]

    member this.ExtraSpaceAmount = PacketConstants.MaxFragmentDataSize - this.LastPacket.Header.FragmentDataSize

    member this.TryFillExtraSpace(data: ReadOnlySpan<byte>) =
        if data.Length <= this.ExtraSpaceAmount then
            let lastPacket = this.LastPacket
            let header = lastPacket.Header
            data.CopyTo(this.LastPacket.AsSpan.Slice(sizeof<PacketHeader> + header.FragmentDataSize, sizeof<PacketHeader> + PacketConstants.MaxFragmentDataSize))
            let header =  { header with FragmentDataSize = header.FragmentDataSize + data.Length }
            lastPacket.AsSpan.Write<PacketHeader>(0, &header)
            true
        else
            false
            

    member this.Count = this.MainHeader.FragmentCount

    member this.ForEach(f) =
        for i = 0 to this.Count - 1 do
            f this.packets.[i]

let createPacketHeader seqId seqVer dataSize fragIndex fragCount =
    let fragSize =
        if fragIndex = fragCount - 1 then dataSize - ((fragCount - 1) * PacketConstants.MaxFragmentDataSize)
        else PacketConstants.MaxFragmentDataSize

    {
        SequenceId = seqId
        SequenceVersion = seqVer
        DataSize = dataSize
        FragmentIndex = fragIndex
        FragmentCount = fragCount
        FragmentDataSize = fragSize
    }

let createPacket (packetByteArrayPool: ArrayPool<byte>) (packetHeader: PacketHeader) (data: ReadOnlySpan<byte>) =
    let start = packetHeader.FragmentIndex * PacketConstants.MaxFragmentDataSize
    let dataSize = packetHeader.FragmentDataSize

    let packetBytes = packetByteArrayPool.Rent(PacketConstants.MaxFragmentDataSize + sizeof<PacketHeader>)
    let packetBytesSpan = Span(packetBytes)

    // Write header and data
    packetBytesSpan.Write<PacketHeader>(0, &packetHeader)
    data.Slice(start, dataSize).CopyTo(packetBytesSpan.Slice(sizeof<PacketHeader>))

    { packetBytes = packetBytes }

/// Thread safe and atomic
[<Sealed>]
type PacketPool() =

    //let packetByteArrayPool = ArrayPool<byte>.Create(sizeof<PacketHeader> * PacketConstants.MaxFragmentDataSize, PacketConstants.PoolAmount)
    //let packetArrayPool = ArrayPool<Packet>.Create(64, PacketConstants.PoolAmount)

    member __.AssignHeaders(byteStream: ChunkedByteStream, seqId, seqVer) =
        if byteStream.ChunkLength <> (sizeof<PacketHeader> + PacketConstants.MaxFragmentDataSize) then
            failwith "Invalid chunk length"

        let fragCount = (byteStream.Length / (sizeof<PacketHeader> + PacketConstants.MaxFragmentDataSize)) + 1

        for fragIndex = 0 to fragCount - 1 do
            let seqId = seqId + uint32 fragIndex
            let mutable packetHeader = createPacketHeader seqId seqVer byteStream.Length fragIndex fragCount
            byteStream.Chunks.[fragIndex].Span.Write(0, &packetHeader)

        ()

    //member __.Rent(packetData: ReadOnlySpan<byte>) =
    //    if packetData.Length > (PacketConstants.MaxFragmentDataSize + sizeof<PacketHeader>) then
    //        failwith "Packet data too big."

    //    let packetBytes = packetByteArrayPool.Rent(packetData.Length)
    //    packetData.CopyTo(Span(packetBytes, 0, packetData.Length))

    //    // TODO: Add validation

    //    { packetBytes = packetBytes }

    //member __.ReturnPackets(packets: Packets) =
    //    packetArrayPool.Return(packets.packets)

    //member __.Return(packet: Packet) =
    //    packetByteArrayPool.Return(packet.packetBytes)
     
[<Struct>]
type Data =
    private {
        dataBytes: byte []
        dataSize: int
    }

    member this.AsSpan = Span(this.dataBytes, 0, this.dataSize)

type FragMask = int64

/// Thread safe and atomic
[<Sealed>]
type DataDefragmenter() =

    let dataByteArrayPool = ArrayPool<byte>.Shared
    let packetArrayPool = ArrayPool<Packet>.Create(64, PacketConstants.PoolAmount)
    let fragMaskArrayPool = ArrayPool<FragMask>.Create(64, PacketConstants.PoolAmount)

    let frags = ConcurrentDictionary<uint32, Lazy<struct (FragMask [] * Packet [])>>()

    let gate = obj ()

    let hasAllFragments (fragMasks: FragMask []) fragCount =
        let mutable hasAll = true
        let maskCount = fragCount / 64
        for i = 0 to maskCount do
            let fragMask = fragMasks.[i]
            let finalMask = if maskCount = i then ~~~(-1L <<< fragCount) else -1L
            if fragMask <> finalMask then
                hasAll <- false
        hasAll

    member this.TryRentData(packets: Packets) =
        let mutable result = ValueNone
        for i = 0 to packets.Count - 1 do
            result <- this.TryRentData(packets.packets.[i])
        result

    member __.TryRentData(packet: Packet) =
        let header = packet.Header

        let fragIndex = header.FragmentIndex
        let fragCount = header.FragmentCount

        // Validation
        if fragIndex < 0 || fragCount <= 0 then
            failwith "Invalid fragment header."

        let mainSeqId = header.MainSequenceId

        let delayed =
            let add = fun _ ->
                lazy
                    let packets = packetArrayPool.Rent(fragCount)
                    let fragMasks = fragMaskArrayPool.Rent((fragCount / 64) + 1)
                    struct(fragMasks, packets)

            frags.GetOrAdd(mainSeqId, add)

        let struct(fragMasks, packets) = delayed.Value

        packets.[header.FragmentIndex] <- packet

        let index = fragIndex / 64
        let indexMod = fragIndex % 64
        
        let hasAllFragments = lock gate (fun () -> 
            fragMasks.[index] <- fragMasks.[index] ||| (1L <<< indexMod)
            hasAllFragments fragMasks fragCount
        )

        // We have all fragments
        if hasAllFragments then
            frags.TryRemove(mainSeqId) |> ignore
            fragMaskArrayPool.Return(fragMasks)

            let dataBytes = dataByteArrayPool.Rent(header.DataSize)
            let dataBytesSpan = Span(dataBytes, 0, header.DataSize)

            for i = 0 to fragCount - 1 do
                let packet = packets.[i]

                if packet.packetBytes = null then
                    failwith "there are no packet bytes"

                let packetHeader = packet.Header
                let start = packetHeader.FragmentIndex * PacketConstants.MaxFragmentDataSize
                let packetSpan = packet.AsSpan

                packetSpan.Slice(sizeof<PacketHeader>, packetHeader.FragmentDataSize).CopyTo(dataBytesSpan.Slice(start, packetHeader.FragmentDataSize))

            let mainHeader = packets.[0].Header

            packetArrayPool.Return(packets)

            ValueSome({ dataBytes = dataBytes; dataSize = mainHeader.DataSize })
        else
            ValueNone
            
    member __.ReturnData(data: Data) =
        dataByteArrayPool.Return(data.dataBytes)

//type PacketChannel(channelNumber: int) =

//    let ackMaskArrayPool = ArrayPool<int64>.Create(128, PacketConstants.PoolAmount)

//    let packetPool = PacketPool()

//    let acks = ConcurrentDictionary()

//    member __.CreatePacket(data) =
//        let packets = packetPool.RentPackets(data, 0u, 0u)
//        ()
