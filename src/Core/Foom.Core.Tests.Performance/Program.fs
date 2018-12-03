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

    let bytes = ArrayPool<byte>.Shared.Rent(size)

    let bytesSpan = Span(bytes, 0, size)

    let mutable w = Writer(WriterType.Default)
    for _i = 1 to count do
        w.WriteString(bytesSpan, &str)

    let s = System.Diagnostics.Stopwatch.StartNew()
    let packetPool = PacketPool()
    
    let packets = packetPool.RentPackets(bytesSpan.AsReadOnly, 0u, 0u)
    s.Stop()
    printfn "Packets - Time: %A ms" s.Elapsed.TotalMilliseconds

    let s = System.Diagnostics.Stopwatch.StartNew()
    let defragmenter = DataDefragmenter()

    let data = defragmenter.TryRentData(packets)
    s.Stop()
    printfn "Data - Time: %A ms" s.Elapsed.TotalMilliseconds
    printfn "====="

    defragmenter.ReturnData(data.Value)
    packetPool.ReturnPackets(packets)

    ArrayPool<byte>.Shared.Return(bytes) 

[<EntryPoint>]
let main argv =
    for i = 0 to 10 do
        stringTest 1000


    0
