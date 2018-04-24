﻿namespace Foom.Test

open System
open System.Numerics
open System.Collections.Concurrent
open System.Collections.Generic
open System.Linq
open Foom.Game
open Foom.Renderer
open Foom.Renderer.GL
open Foom.Renderer.GL.Desktop
open Foom.Input
open Foom.Net
open Foom.EntityManager
open Foom.Core

type ClientEvent =
    | Connected
    | LoadMap of name: string

[<Sealed>]
type ClientGame(em: EntityManager, input: IInput, renderer: IRenderer, client: IBackgroundClient) =
    inherit AbstractClientGame()

    let eventQueue = ConcurrentQueue()

    let projection = 
        Matrix4x4.CreatePerspectiveFieldOfView (56.25f * 0.0174533f, ((16.f + 16.f * 0.25f) / 9.f), 16.f, 100000.f)
    let camera = renderer.CreateCamera(Matrix4x4.Identity, projection, 0)

    // hard coded shit
    let mutable zombiemanSpriteBatchOpt : ISpriteBatch option = None
    let mutable mov = Movement.Default
    let mutable inputState = Unchecked.defaultof<InputState>

    let spriteStates = Array.zeroCreate<int> 64
    let mutable clientId = ClientId.Local

    let mutable latestSnap = 31us
    let masterPlayerSnapshots = Array.init<UnmanagedArray<PlayerSnapshot>> 30 (fun _ -> UnmanagedArray<PlayerSnapshot>.Create(32768, fun _ -> PlayerSnapshot()))
    let sortedList = SortedList()

    let mutable renderTime = None

    let processMessages time =
        client.ProcessMessages(function
            | ClientMessage.ConnectionAccepted(x) -> clientId <- x
            | ClientMessage.DisconnectAccepted _ -> ()
            | ClientMessage.Message(msg) -> 
                match msg with
                | :? Snapshot as snapshotMsg ->

                    let playerSnapshots = 
                        let playerSnapshots = masterPlayerSnapshots.[int snapshotMsg.snapshotId % 30]
                        playerSnapshots

                    latestSnap <- (latestSnap + 1us) % 30us

                    if sortedList.ContainsKey(snapshotMsg.snapshotId) |> not then
                        if sortedList.Count > 3 then
                            printfn "deleting snapshot"
                            sortedList.RemoveAt(0)
                        sortedList.Add(snapshotMsg.snapshotId, struct(snapshotMsg.playerCount, playerSnapshots, time, snapshotMsg.serverTime))

                | _ -> ()
        )

    do
        eventQueue.Enqueue(LoadMap "e1m1")

    override __.PreUpdate(time, interval) =
      //  client.ProcessMessages(fun _ -> ())
        input.PollEvents()

        inputState <- input.GetState()

        camera.ViewLerp <- camera.View

        mov <- getMovement inputState mov

        let userInfoMsg = client.CreateMessage<UserInfo>()
        userInfoMsg.Movement <- mov
        client.SendMessage(userInfoMsg)

        //if clientId <> -1 then
            //updatePlayer mov &playerStates.[clientId]

    override __.Update(time, interval) = 
        processMessages time

        let mutable evt = Unchecked.defaultof<ClientEvent>
        while eventQueue.TryDequeue(&evt) do
            match evt with
            | LoadMap name ->
                camera.Rotation <- Quaternion.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
                client.Connect("localhost", 27015)

                let mutable beef = false

                let connection = async {
                    while beef = false do
                        processMessages time
                        client.SendPackets()
                        do! Async.Sleep(10) } |> Async.StartAsTask
                zombiemanSpriteBatchOpt <- Some(loadMap name camera renderer)
                renderer.Draw(0.f)
                beef <- true
                connection.Wait()
            | _ -> ()

        if renderTime.IsSome then
            renderTime <- Some(renderTime.Value + interval)

        // end events
        if sortedList.Count > 0 then
            let struct(playerCount, playerSnapshots, snapTime, serverTime) = sortedList.Values.[0]
            let rTime = 
                match renderTime with
                | None -> 
                    renderTime <- Some(serverTime)
                    renderTime.Value
                | Some renderTime -> renderTime
           // printfn "snapshot count: %A" sortedList.Count
            if (rTime - interval - interval >= serverTime) || clientId.IsLocal then

                sortedList.RemoveAt(0)
                for i = 0 to playerCount - 1 do
                    let player = playerSnapshots.GetByRef(i)

                    let sprite = &spriteStates.[i]
                    if sprite = 0 && zombiemanSpriteBatchOpt.IsSome then
                        sprite <- zombiemanSpriteBatchOpt.Value.CreateSprite()

                    if zombiemanSpriteBatchOpt.IsSome then
                        zombiemanSpriteBatchOpt.Value.SetSpritePosition(sprite, player.position)

                    if player.clientId = clientId then
                        camera.Translation <- player.position
                        camera.Rotation <- player.rotation
            else
                printfn "no updating snapshot"
        else
            printfn "no snapshots available"

        client.SendPackets()

        inputState.Events
        |> List.exists (function KeyReleased '\027' -> true | _ -> false)

    override __.Render(time, deltaTime) =
        renderer.Draw(deltaTime)

    static member Create(em, client) =
        let app = Backend.init()
        let input = Foom.Input.Desktop.DesktopInput(app.Window) :> IInput

        let desktopGL = DesktopGL(app)
        let renderer = Renderer(desktopGL) :> IRenderer

        ClientGame(em, input, renderer, client)
