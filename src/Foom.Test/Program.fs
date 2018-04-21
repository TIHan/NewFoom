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
open System.Collections.Concurrent
open System.Numerics
open SkiaSharp
open Foom.Wad
open Foom.Wad.Extensions
open Foom.Geometry
open Foom.Renderer
open Foom.Renderer.GL
open Foom.Renderer.GL.Desktop
open Foom.Input
open Foom.Input.Desktop
open Foom.Test
open Foom.Net
open Foom.IO.Message
open Foom.Game

[<EntryPoint>]
let main argv =

    let netChans =
        [
            NetworkChannel.create ChannelType.Unreliable
            |> NetworkChannel.register<Snapshot> 20

            NetworkChannel.create ChannelType.UnreliableSequenced
            |> NetworkChannel.register<UserInfo> 20
        ]

    let network = Network(netChans)

    let serverOpt =
        if argv.Length = 0 then
            let server = network.CreateBackgroundServer(27015, 8)
            server.OnException.Add(fun ex -> printfn "%A" ex.Message; printfn "%A" ex.StackTrace)
            server.Start()
            printfn "Server started"
            Some server
        else
            None

    let clientOpt =
        match serverOpt with
        | None ->
            let client = network.CreateBackgroundClient()
            client.OnException.Add(fun ex -> printfn "%A" ex.Message)
            printfn "Client started, connecting at %s." argv.[0]
            Some client
        | Some server -> 
            let client = server.CreateLocalBackgroundClient()
            client.OnException.Add(fun ex -> printfn "%A" ex.Message)
            printfn "Local client started."
            Some client

    let svGame = 
        match serverOpt with
        | Some server -> ServerGame.Create(server) :> AbstractServerGame
        | _ -> EmptyServerGame() :> AbstractServerGame
    let clGame = ClientGame.Create(clientOpt.Value)
    let game = Game(svGame, clGame, 30)

    game.Start()

    if serverOpt.IsSome then (serverOpt.Value :> IDisposable).Dispose()
    if clientOpt.IsSome then (clientOpt.Value :> IDisposable).Dispose()

    //sunderStream (fun stream ->
    //    let wad = Wad.FromStream stream

    //    allMapGeometry wad
    //        [ "map01"; "map02"; "map03"; "map04"; "map05"; "map06"; 
    //          "map07"; "map08"; "map09"; "map10"; "map11"; "map12"; "map13"; "map14" ]
    //)
    0
