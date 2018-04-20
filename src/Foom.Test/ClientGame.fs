namespace Foom.Test

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

type ClientEvent =
    | Connected
    | LoadMap of name: string

[<Sealed>]
type ClientGame(input: IInput, renderer: IRenderer, client: IBackgroundClient) =
    inherit AbstractClientGame()

    let eventQueue = ConcurrentQueue()

    let projection = 
        Matrix4x4.CreatePerspectiveFieldOfView (56.25f * 0.0174533f, ((16.f + 16.f * 0.25f) / 9.f), 16.f, 100000.f)
    let camera = renderer.CreateCamera(Matrix4x4.Identity, projection, 0)

    // hard coded shit
    let mutable zombiemanSpriteBatchOpt : ISpriteBatch option = None
    let mutable mov = Movement.Default
    let mutable inputState = Unchecked.defaultof<InputState>

    let mutable playerCount = 0
    let spriteStates = Array.zeroCreate<int> 64
    let mutable clientId = ClientId.Local

    let mutable latestSnap = 31us
    let snapshotHistory = Array.init 32 (fun _ -> Array.zeroCreate<Player> 64)
    let sortedList = SortedList()
    let queue = Queue()

    do
        client.ListenForMessage<Snapshot>()
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
        client.ProcessMessages(function
            | ClientMessage.ConnectionAccepted(x) -> clientId <- x
            | ClientMessage.DisconnectAccepted _ -> ()
            | ClientMessage.Message(msg) -> 
                match msg with
                | :? Snapshot as snapshotMsg ->
                    playerCount <- snapshotMsg.playerCount

                    latestSnap <- (latestSnap + 1us) % 32us

                    let playerStates = snapshotHistory.[int latestSnap]

                    for i = 0 to playerCount - 1 do
                        playerStates.[i] <- snapshotMsg.playerState.[i]

                    if sortedList.ContainsKey(snapshotMsg.snapshotId) |> not then
                        sortedList.Add(snapshotMsg.snapshotId, struct(playerStates, time))

                | _ -> ()
        )

        let mutable evt = Unchecked.defaultof<ClientEvent>
        while eventQueue.TryDequeue(&evt) do
            match evt with
            | LoadMap name ->
                camera.Rotation <- Quaternion.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
                zombiemanSpriteBatchOpt <- Some(loadMap name camera renderer)
                client.Connect("localhost", 27015)
            | _ -> ()
        
        // end events
        if sortedList.Count > 0 then
            let struct(playerStates, snapTime) = sortedList.Values.[0]
            if time >= snapTime + TimeSpan.FromMilliseconds(100.) || clientId.IsLocal then
                sortedList.RemoveAt(0)
                for i = 0 to playerCount - 1 do
                    let player = &playerStates.[i]

                    let sprite = &spriteStates.[i]
                    if sprite = 0 && zombiemanSpriteBatchOpt.IsSome then
                        sprite <- zombiemanSpriteBatchOpt.Value.CreateSprite()

                    if zombiemanSpriteBatchOpt.IsSome then
                        zombiemanSpriteBatchOpt.Value.SetSpritePosition(sprite, playerStates.[i].translation)

                    if player.clientId = clientId then
                        camera.Translation <- player.translation
                        camera.Rotation <- player.rotation

        inputState.Events
        |> List.exists (function KeyReleased '\027' -> true | _ -> false)

    override __.Render(time, deltaTime) =
        renderer.Draw(deltaTime)

    static member Create(client) =
        let app = Backend.init()
        let input = Foom.Input.Desktop.DesktopInput(app.Window) :> IInput

        let desktopGL = DesktopGL(app)
        let renderer = Renderer(desktopGL) :> IRenderer

        ClientGame(input, renderer, client)
