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
        let wad = Wad.FromFile("../../../../../Foom-deps/testwads/doom1.wad")
        let map = wad.FindMap "e1m1"

        let player1Start = map.TryFindPlayer1Start()
        player1Start
        |> Option.map (fun doomThing ->
            Vector3(single doomThing.X, single doomThing.Y, 28.f)
        )

[<Sealed>]
type ServerGame(server: BackgroundServer) =
    inherit AbstractServerGame()

    let eventQueue = ConcurrentQueue()

    let playerLookup = Dictionary<ClientId, Player ref * Movement ref>()

    let player1StartPosition = player1StartPosition ()

    let mutable snapshotId = 0L

    do
        server.ListenForMessage<UserInfo>()

    override __.Update(time, interval) =
        server.ProcessClientConnected(fun clientId ->
            let mutable player = Unchecked.defaultof<Player>
            player.clientId <- clientId
            match player1StartPosition with
            | Some pos -> player.translation <- pos
            | _ -> ()

            let player = ref player
            let movement = ref (Unchecked.defaultof<Movement>)

            playerLookup.Add(clientId, (player, movement))

            printfn "Client Connected: %A" clientId
        )

        server.ProcessMessages(fun struct(clientId, msg) ->
            match playerLookup.TryGetValue(clientId) with
            | true, (player, movement) ->
                match msg with
                | :? UserInfo as userInfoMsg ->
                    movement.contents <- userInfoMsg.Movement
                |  _ -> ()
            | _ -> ()
        )

        playerLookup
        |> Seq.iter (fun pair ->
            let (player, movement) = pair.Value
            updatePlayer movement.contents &player.contents
        )

        let snapshotMsg = server.CreateMessage<Snapshot>()
        snapshotMsg.playerCount <- playerLookup.Count;

        playerLookup
        |> Seq.iteri (fun i pair ->
            let (player, _) = pair.Value
            snapshotMsg.playerState.[i] <- player.contents
        )

        snapshotMsg.snapshotId <- snapshotId
        snapshotId <- snapshotId + 1L
        server.SendMessage(snapshotMsg, 1uy)
        false

    static member Create(server) =
        ServerGame(server)
