// Learn more about F# at http://fsharp.org

open System
open System.Buffers
open Foom.IO.FastPacket
open Foom.IO.Serializer

let stringTest count =
    let mutable str = "BEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEF"

    let mutable w = Writer(WriterType.CountOnly)
    for _i = 1 to count do
        w.WriteString(Span.Empty, &str)
    let size = w.position

    printfn "Gigs of data: %A" (single size / 1024.f / 1024.f / 1024.f)

    let bytes = ArrayPool<byte>.Shared.Rent(size)

    let bytesSpan = Span(bytes, 0, size)

    let mutable w = Writer(WriterType.Default)
    for _i = 1 to count do
        w.WriteString(bytesSpan, &str)

    let packetFactory = PacketFactory()
    
    let packets = packetFactory.CreatePackets(bytesSpan.AsReadOnly, 0u, TimeSpan.Zero, 0u)

    let defragmenter = DataDefragmenter()

    let data = defragmenter.TryGetData(packets)

    defragmenter.RecycleData(data.Value)
    packetFactory.RecyclePackets(packets)

    ArrayPool<byte>.Shared.Return(bytes) 

[<EntryPoint>]
let main argv =
    for i = 0 to 10 do
        let s = System.Diagnostics.Stopwatch.StartNew()
        stringTest 10000000
        s.Stop()
        printfn "Time: %A ms" s.Elapsed.TotalMilliseconds


    0
