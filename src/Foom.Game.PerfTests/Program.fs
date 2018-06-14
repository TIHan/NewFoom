// These are sectors to look for and test to ensure things are working as they should.
// 568 - map10 sunder
// 4 - map10  sunder
// 4371 - map14 sunder
// 28 - e1m1 doom
// 933 - map10 sunder
// 20 - map10 sunder
// 151 - map10 sunder
// 439 - map08 sunder
// 271 - map03 sunder
// 663 - map05 sunder
// 506 - map04 sunder
// 3 - map02 sunder
// 3450 - map11 sunder
// 1558 - map11 sunder
// 240 - map07 sunder
// 2021 - map11 sunder

open System
open System.IO
open System.IO.Compression
open System.Diagnostics
open Foom.Wad
open System.Threading.Tasks

let allMapGeometry (wad: Wad) mapNames =

    let stopwatch = Stopwatch.StartNew()

    let levels =
        mapNames
        |> Seq.map (fun mapName ->
            wad.FindMap mapName
        )
        |> Seq.toArray

    stopwatch.Stop()
    let deserializationTime = stopwatch.Elapsed.TotalMilliseconds
    printfn "\tDeserializing Success! Time: %f ms" deserializationTime

    stopwatch.Reset()
    stopwatch.Start()

    //levels
  //  |> Array.map(fun map ->
    let options = ParallelOptions ()

    options.MaxDegreeOfParallelism <- 4

    let beef = ResizeArray()
    Parallel.For(0, levels.Length, options, fun i _ ->
        levels.[i].ComputeAllSectorGeometry() |> beef.Add
    ) |> ignore

    stopwatch.Stop()
    let geometryTime = stopwatch.Elapsed.TotalMilliseconds
    printfn "\tGeometry Success! Time: %f ms" geometryTime

    printfn "Total Time: %f ms" (deserializationTime + geometryTime)

let sunderStream f =
    if not <| File.Exists ("sunder.wad") then
        use zip = ZipFile.Open("../../../../../Foom-deps/testwads/sunder.zip", ZipArchiveMode.Read)
        let sunderEntry = zip.GetEntry("sunder/sunder.wad")

        sunderEntry.ExtractToFile("sunder.wad")

    let stream = File.Open("sunder.wad", FileMode.Open)
    f stream
    stream.Dispose()

    File.Delete("sunder.wad")

[<EntryPoint>]
let main argv =
    sunderStream (fun stream ->
        let wad = Wad.FromStream stream

        allMapGeometry wad
            <| List.init 50 (fun _ -> "map14")
    )
    0
