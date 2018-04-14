module Foom.IO.Packet.Tests

open System
open System.Collections.Generic
open Xunit
open Foom.IO.Serializer

[<Fact>]
let ``Unreliable - Basic Test`` () =
    let stream = PacketStream()

    let clientStream = PacketStream()

    let buffer = Array.zeroCreate<byte> 65536
    let bufferSpan = Span buffer

    let mutable writer = Writer()

    for i = 0 to 7000 do
        writer.WriteInt(bufferSpan, i)

    stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Unreliable) |> ignore

    let mutable start = 0
    let outputBuffer = Array.zeroCreate<byte> 65536
    let outputBufferSpan = Span outputBuffer
    let run () =
        stream.ProcessSending(fun packet ->
            clientStream.Receive(Span.op_Implicit(packet), fun data ->
                data.CopyTo(Span(outputBuffer, start))
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
        writer.WriteInt(bufferSpan, i)

    stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Reliable) |> ignore

    let outputBuffer = Array.zeroCreate<byte> 65536
    let outputBufferSpan = Span outputBuffer

    let mutable start = 0
    stream.ProcessSending(fun packet ->
        stream.Receive(Span.op_Implicit(packet), fun data ->
            data.CopyTo(Span(outputBuffer, start))
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
        writer.WriteInt(bufferSpan, i)

    stream.Send(bufferSpan.Slice(0, writer.position), PacketDeliveryType.Unreliable) |> ignore

    let mutable start = 0
    let outputBuffer = Array.zeroCreate<byte> 65536
    let outputBufferSpan = Span outputBuffer
    let run () =
        let outputBuffer = Array.zeroCreate<byte> 65536
        let outputBufferSpan = Span outputBuffer

        stream.ProcessSending(fun packet ->
            stream.Receive(Span.op_Implicit(packet), fun data ->
                data.CopyTo(Span(outputBuffer, start))
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
        writer.WriteInt(bufferSpan, i)

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
                clientStream.Receive(Span.op_Implicit(packet), fun data ->
                    data.CopyTo(Span(outputBuffer, 0))
                    start <- start + data.Length)
            )

            clientStream.Send(Span([||]), PacketDeliveryType.Unreliable) |> ignore
            clientStream.ProcessSending(fun packet ->
                stream.Receive(Span.op_Implicit(packet), fun data ->
                    data.CopyTo(Span(outputBuffer, 0))
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




