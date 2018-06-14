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
open Foom.EntityManager

[<EntryPoint>]
let main argv =

    let createEntityManager () =
        let em = new EntityManager(32768)

        em.RegisterComponent<Transform>()
        em.RegisterComponent<Direction>()
        em.RegisterComponent<Render>()
        em.RegisterComponent<UserControlled>()
        em.RegisterComponent<SpectatorTag>()
        em

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
            let server = network.CreateServer(27015, 8)
            server.Start()
            printfn "Server started"
            Some server
        else
            None

    let svGame = 
        match serverOpt with
        | Some server -> ServerGame.Create(createEntityManager(), server) :> AbstractServerGame
        | _ -> EmptyServerGame() :> AbstractServerGame
    let clGame = ClientGame.Create(createEntityManager(), network)
    let game = Game(svGame, clGame, 30)


    game.Start()

    if serverOpt.IsSome then (serverOpt.Value :> IDisposable).Dispose()
    0
