namespace Foom.Test

open System
open System.Numerics
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.Game
open Foom.Net
open Foom.Wad

[<Sealed>]
type EmptyServerGame() =
    inherit AbstractServerGame()

    override __.Update(_, _) = false

[<AutoOpen>]
module private BestHelpers =
    let player1StartPosition () =
        use wad = Wad.FromFile("../../../../../Foom-deps/testwads/doom1.wad")
        let map = wad.FindMap "e1m1"

        let player1Start = map.TryFindPlayer1Start()
        player1Start
        |> Option.map (fun doomThing ->
            Vector3(single doomThing.X, single doomThing.Y, 28.f)
        )

open Foom.EntityManager

[<Sealed>]
type ServerGame(server: BackgroundServer) =
    inherit AbstractServerGame()

    let eventQueue = ConcurrentQueue()

    let playerLookup = Dictionary<ClientId, Entity>()

    let player1StartPosition = player1StartPosition ()

    let mutable snapshotId = 0L


    let em = new EntityManager(100)

    do
        server.ListenForMessage<UserInfo>()
        em.RegisterComponent<Player>()
        em.RegisterComponent<Movement>()

    override __.Update(time, interval) =
        server.ProcessClientConnected(fun clientId ->

            let ent = em.Spawn()

            let player = em.Add<Player>(ent)
            let movement = em.Add<Movement>(ent)

            player.clientId <- clientId
            match player1StartPosition with
            | Some pos -> player.translation <- pos
            | _ -> ()

            playerLookup.Add(clientId, ent)

            printfn "Client Connected: %A" clientId
        )

        server.ProcessMessages(fun struct(clientId, msg) ->
            match msg with
            | :? UserInfo as userInfoMsg when playerLookup.ContainsKey(clientId) ->
                em.TryGetComponent<Movement>(playerLookup.[clientId], fun movement ->
                    movement <- userInfoMsg.Movement
                )
            | _ -> ()
        )

        em.ForEach<Player, Movement>(fun _ player movement ->
            updatePlayer movement &player
        )

        let snapshotMsg = server.CreateMessage<Snapshot>()

        let mutable i = 0

        em.ForEach<Player>(fun _ player ->
            snapshotMsg.playerState.[i] <- player
            i <- i + 1
        )

        snapshotMsg.playerCount <- i

        snapshotMsg.snapshotId <- snapshotId
        snapshotId <- snapshotId + 1L
        server.SendMessage(snapshotMsg, 1uy)
        false

    static member Create(server) =
        ServerGame(server)
