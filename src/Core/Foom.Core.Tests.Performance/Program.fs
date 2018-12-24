// Learn more about F# at http://fsharp.org

open System
open System.Buffers
open Foom.IO
open Foom.IO.FastPacket
open Foom.IO.Serializer
open System.Threading.Tasks
open BenchmarkDotNet.Attributes
open BenchmarkDotNet.Running

[<MemoryDiagnoser>]
type FoomCore() =

    let arr = Array.init 1000000 (fun _ -> 0uy)
    let chunkedArr = ChunkedArray<byte>(1024, 1000000)

    [<Benchmark>]
    member __.Array() =
        for i = 0 to arr.Length - 1 do
            arr.[i] |> ignore

    [<Benchmark>]
    member __.ChunkedArray() =
        for i = 0 to chunkedArr.Length - 1 do
            chunkedArr.[i] |> ignore

[<EntryPoint>]
let main argv =
    let _ = BenchmarkRunner.Run<FoomCore>()
    0
