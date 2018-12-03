module PacketTests

open System
open System.Collections.Generic
open Xunit
open Foom.IO.Serializer
open Foom.IO.Packet

[<Fact>]
let ``Unreliable - Basic Test`` () =
    let stream = PacketStream()

    let clientStream = PacketStream()

    let buffer = Array.zeroCreate<byte> 65536
    let bufferSpan = Span buffer

    let mutable writer = Writer()

    for i = 0 to 7000 do
        writer.WriteInt(bufferSpan, ref i)

    stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Unreliable) |> ignore

    let mutable start = 0
    let outputBuffer = Array.zeroCreate<byte> 65536
    let outputBufferSpan = Span outputBuffer
    let run () =
        stream.ProcessSending(fun packet ->
            clientStream.Receive(packet, fun data ->
                data.CopyTo(Span(outputBuffer, start, outputBuffer.Length))
                start <- start + data.Length
            )
        )

        clientStream.Send(Span([||]), PacketDeliveryType.Unreliable) |> ignore
    //    clientStream.ProcessSending(fun packet -> stream.Receive(Span.op_Implicit(packet)))

    run ()
    Assert.True(start > 0)

    for i = 0 to buffer.Length - 1 do
        if buffer.[i] <> outputBuffer.[i] then
            failwithf "Index %i, doesn't match. Expected: %A Actual: %A" i buffer.[i] outputBuffer.[i]
        Assert.Equal(buffer.[i], outputBuffer.[i])


[<Fact>]
let ``Reliable - Basic Test`` () =
    let stream = PacketStream()

    let buffer = Array.zeroCreate<byte> 65536
    let bufferSpan = Span buffer

    let mutable writer = Writer()

    for i = 0 to 7000 do
        writer.WriteInt(bufferSpan, ref i)

    stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Reliable) |> ignore

    let outputBuffer = Array.zeroCreate<byte> 65536
    let outputBufferSpan = Span outputBuffer

    let mutable start = 0
    stream.ProcessSending(fun packet ->
        stream.Receive(packet, fun data ->
            data.CopyTo(Span(outputBuffer, start, outputBuffer.Length))
            start <- start + data.Length)
    )

    Assert.True(start > 0)

    for i = 0 to buffer.Length - 1 do
        if buffer.[i] <> outputBuffer.[i] then
            failwithf "Index %i, doesn't match. Expected: %A Actual: %A" i buffer.[i] outputBuffer.[i]
        Assert.Equal(buffer.[i], outputBuffer.[i])

[<Fact>]
let ``Unreliable - Packet Loss - No Recovery`` () =
    let stream = PacketStream()
    stream.LoseEveryOtherPacket <- true

    let buffer = Array.zeroCreate<byte> 65536
    let bufferSpan = Span buffer

    let mutable writer = Writer()

    for i = 0 to 7000 do
        writer.WriteInt(bufferSpan, ref i)

    stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Unreliable) |> ignore

    let mutable start = 0
    let outputBuffer = Array.zeroCreate<byte> 65536
    let outputBufferSpan = Span outputBuffer
    let run () =
        let outputBuffer = Array.zeroCreate<byte> 65536
        let outputBufferSpan = Span outputBuffer

        stream.ProcessSending(fun packet ->
            stream.Receive(packet, fun data ->
                data.CopyTo(Span(outputBuffer, start, outputBuffer.Length))
                start <- start + data.Length)
        )

    run ()
    Assert.Equal(0, start)
    stream.LoseEveryOtherPacket <- false
    stream.Time <- TimeSpan.FromSeconds(1.5)
    run ()
    Assert.Equal(0, start)

[<Fact>]
let ``Reliable - Packet Loss - Recovery - Soak`` () =
    let stream = PacketStream()

    let clientStream = PacketStream()

    let buffer = Array.zeroCreate<byte> 65536
    let bufferSpan = Span buffer

    let mutable writer = Writer()

    for i = 0 to 7000 do
        writer.WriteInt(bufferSpan, ref i)

    for iteration = 0 to 10000 do
        GC.Collect(2, GCCollectionMode.Forced)
        stream.Time <- TimeSpan.Zero
        stream.LoseEveryOtherPacket <- true
        stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Reliable) |> ignore

        let mutable start = 0
        let outputBuffer = Array.zeroCreate<byte> 65536
        let outputBufferSpan = Span outputBuffer
        let run () =
            stream.ProcessSending(fun packet ->
                clientStream.Receive(packet, fun data ->
                    data.CopyTo(Span(outputBuffer, 0, outputBuffer.Length))
                    start <- start + data.Length)
            )

            clientStream.Send(Span([||]), PacketDeliveryType.Unreliable) |> ignore
            clientStream.ProcessSending(fun packet ->
                stream.Receive(packet, fun data ->
                    data.CopyTo(Span(outputBuffer, 0, outputBuffer.Length))
                    start <- start + data.Length)
            )

        run ()
        Assert.Equal(0, start)
        stream.LoseEveryOtherPacket <- false
        stream.Time <- TimeSpan.FromSeconds(1.5)
        run ()

        Assert.True(start > 0)

        for i = 0 to buffer.Length - 1 do
            if buffer.[i] <> outputBuffer.[i] then
                failwithf "Index %i, doesn't match. Expected: %A Actual: %A. Iteration: %i" i buffer.[i] outputBuffer.[i] iteration
            Assert.Equal(buffer.[i], outputBuffer.[i])

open Foom.IO.FastPacket
open System.Buffers

[<AutoOpen>]
module TestHelpers =

    let stringTest count =
        let mutable str = "BEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEF"

        let mutable w = Writer(WriterType.CountOnly)
        for _i = 1 to count do
            w.WriteString(Span.Empty, &str)
        let size = w.position

        let bytes = ArrayPool<byte>.Shared.Rent(size)

        let bytesSpan = Span(bytes, 0, size)

        let mutable w = Writer(WriterType.Default)
        for _i = 1 to count do
            w.WriteString(bytesSpan, &str)

        let packetPool = PacketPool()
    
        let packets = packetPool.RentPackets(bytesSpan.AsReadOnly, 0u, 0u)
        Assert.Equal((bytesSpan.Length / PacketConstants.MaxFragmentDataSize) + 1, packets.Count)

        let defragmenter = DataDefragmenter()

        let data = defragmenter.TryRentData(packets)

        let hasData = match data with | ValueSome(_) -> true | _ -> false
        Assert.True(hasData)

        Assert.Equal(bytesSpan.Length, data.Value.AsSpan.Length)

        for i = 0 to bytesSpan.Length - 1 do
            Assert.Equal(bytesSpan.[i], data.Value.AsSpan.[i])

        defragmenter.ReturnData(data.Value)

        packets.ForEach(packetPool.Return)
        packetPool.ReturnPackets(packets)

        ArrayPool<byte>.Shared.Return(bytes)   

[<Fact>]
let ``Fast Packet - 100 strings`` () =
    stringTest 100

[<Fact>]
let ``Fast Packet - 1000 strings`` () =
    stringTest 1000

[<Fact>]
let ``Fast Packet - 1000000 strings`` () =
    stringTest 1000000
