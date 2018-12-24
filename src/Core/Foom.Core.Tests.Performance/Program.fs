// Learn more about F# at http://fsharp.org

open System
open System.Buffers
open Foom.IO
open Foom.IO.FastPacket
open Foom.IO.Serializer
open System.Threading.Tasks

let stringTest count =
    let mutable str = "BEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEF"

    let mutable w = Writer(WriterType.CountOnly)
    for _i = 1 to count do
        w.WriteString(Span.Empty, &str)
    let size = w.position

    let s = System.Diagnostics.Stopwatch.StartNew()
    use byteStream = new ChunkedByteStream(sizeof<PacketHeader>, 1024 + sizeof<PacketHeader>, size)
    for _i = 1 to count do
        byteStream.WriteString(&str)

    let packetPool = PacketPool()
    
    packetPool.AssignHeaders(byteStream, 0u, 0u)
   // let packets = packetPool.RentPackets(bytesSpan.AsReadOnly, 0u, 0u)
    s.Stop()
    printfn "Packets - Time: %A ms" s.Elapsed.TotalMilliseconds

    //let s = System.Diagnostics.Stopwatch.StartNew()
    //let defragmenter = DataDefragmenter()

    //let data = defragmenter.TryRentData(packets)
    //s.Stop()
    //printfn "Data - Time: %A ms" s.Elapsed.TotalMilliseconds
    //printfn "====="

    //defragmenter.ReturnData(data.Value)

    //packets.ForEach(packetPool.Return)
    //packetPool.ReturnPackets(packets)

    //ArrayPool<byte>.Shared.Return(bytes) 



[<EntryPoint>]
let main argv =
    for i = 0 to 100 do
        stringTest 1000000

    //let x = Array.init (1024 * 200) (fun _ -> Array.zeroCreate<byte> 1024)
    //for i = 0 to 100 do
    //    let s = System.Diagnostics.Stopwatch.StartNew()
    //  //  GC.Collect()
    //    let countOnly = CountOnlyByteStream()

    //    for j = 0 to 200 - 1 do
    //        for i = 0 to 262143 do
    //            let mutable i = 0
    //            countOnly.WriteInt(&i)

    //    use xChunk = new ChunkedByteStream(0, 1024, countOnly.position)

    ////    Parallel.For(0, 262144, fun i ->
    //    for j = 0 to 200 - 1 do
    //        for i = 0 to 262143 do
    //            let mutable i = 0
    //            xChunk.WriteInt(&i)
    // //   ) |> ignore

    //    s.Stop()
    //    printfn "Data - Time: %A ms" s.Elapsed.TotalMilliseconds


    0
