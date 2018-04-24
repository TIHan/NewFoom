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
open Foom.Core

[<Sealed>]
type ServerGame(em: EntityManager, server: BackgroundServer) =
    inherit AbstractServerGame()

    let eventQueue = ConcurrentQueue()

    let playerLookup = Dictionary<ClientId, Entity>()

    let player1StartPosition = player1StartPosition ()

    let mutable snapshotId = 0L

    let masterPlayerSnapshots = Array.init<UnmanagedArray<PlayerSnapshot>> 30 (fun _ -> UnmanagedArray<PlayerSnapshot>.Create(32768, fun _ -> PlayerSnapshot()))

    override __.Update(time, interval) =
        server.ProcessMessages(function
            | ServerMessage.ClientConnected(clientId) ->
                let ent = em.Spawn()

                let transform = em.Add<Transform>(ent)
                let _direction = em.Add<Direction>(ent)
                let _render = em.Add<Render>(ent)
                let userControlled = em.Add<UserControlled>(ent)
                let _spectatorTag = em.Add<SpectatorTag>(ent)

                userControlled.clientId <- clientId
                match player1StartPosition with
                | Some pos -> transform.position <- pos
                | _ -> ()

                playerLookup.Add(clientId, ent)

                printfn "Client Connected: %A" clientId

            | ServerMessage.ClientDisconnected(_) -> () // TODO:
            | ServerMessage.Message(clientId, msg) ->
                match msg with
                | :? UserInfo as userInfoMsg when playerLookup.ContainsKey(clientId) ->
                    em.TryGetComponent<UserControlled>(playerLookup.[clientId], fun userControlled ->
                        userControlled.movement <- userInfoMsg.Movement
                    )
                | _ -> ()
        )

        em.ForEach<UserControlled, Transform>(fun _ userControlled transform ->
            updatePlayer userControlled.movement &transform
        )

        let snapshotMsg = server.CreateMessage<Snapshot>()

        let mutable i = 0

        let playerSnapshots = masterPlayerSnapshots.[int snapshotId % 30]

        em.ForEach<Transform, Direction, Render, UserControlled, SpectatorTag>(fun ent transform direction render userControlled _ ->
            let r = playerSnapshots.GetByRef(i)
            r.entity <- ent
            r.position <- transform.position
            r.rotation <- transform.rotation
            r.renderIndex <- render.index
            r.renderFrame <- render.frame
            r.direction <- direction.value
            r.clientId <- userControlled.clientId
            i <- i + 1
        )

        snapshotMsg.playerCount <- i
        snapshotMsg.playerSnapshots <- playerSnapshots
        snapshotMsg.snapshotId <- snapshotId
        snapshotMsg.serverTime <- time
        snapshotId <- snapshotId + 1L
        server.SendMessage(snapshotMsg)
        server.SendPackets()
        false

    static member Create(em, server) =
        ServerGame(em, server)
