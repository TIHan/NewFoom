namespace Foom.Test

open System
open System.Numerics
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.Game
open Foom.Net
open Foom.Wad

[<ReferenceEquality>]
type UnsafeResizeArray<'T> =
    {
        mutable count: int
        mutable buffer: 'T []
    }

    static member Create capacity =
        if capacity <= 0 then
            failwith "Capacity must be greater than 0"

        {
            count = 0
            buffer = Array.zeroCreate<'T> capacity
        }

    static member Create (buffer: 'T []) =
        {
            count = buffer.Length
            buffer = buffer
        }

    member this.IncreaseCapacity () =
        let newLength = uint32 this.buffer.Length * 2u
        if newLength >= uint32 Int32.MaxValue then
            failwith "Length is bigger than the maximum number of elements in the array"

        let newBuffer = Array.zeroCreate<'T> (int newLength)
        Array.Copy (this.buffer, newBuffer, this.count)
        this.buffer <- newBuffer
         

    member inline this.Add item =
        if this.count >= this.buffer.Length then
            this.IncreaseCapacity ()
        
        this.buffer.[this.count] <- item
        this.count <- this.count + 1

    member inline this.LastItem = this.buffer.[this.count - 1]

    member inline this.SwapRemoveAt index =
        if index >= this.count then
            failwith "Index out of bounds"

        let lastIndex = this.count - 1

        this.buffer.[index] <- this.buffer.[lastIndex]
        this.buffer.[lastIndex] <- Unchecked.defaultof<'T>
        this.count <- lastIndex

    member inline this.Count = this.count
    member inline this.Buffer = this.buffer

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

    let playerLookup = Array.zeroCreate 64
    let mutable playerId = 0
    let playerIdQueue = Queue()
    let getNextPlayerId () =
        if playerIdQueue.Count > 0 then
            playerIdQueue.Dequeue()
        else
            let id = playerId
            playerId <- playerId + 1
            id

    let playerStates = UnsafeResizeArray<Player>.Create(64)
    let playerMovStates = UnsafeResizeArray<Movement>.Create(64)

    let player1StartPosition = player1StartPosition ()

    let mutable snapshotId = 0L

    do
        server.ListenForMessage<UserInfo>()

    override __.Update(time, interval) =
        server.ProcessClientConnected(fun clientId ->
            let index = playerStates.Count
            let mutable player = Unchecked.defaultof<Player>

            match player1StartPosition with
            | Some pos -> player.translation <- pos
            | _ -> ()

            playerStates.Add(player)
            playerLookup.[clientId] <- index

            printfn "Client Connected: %i" clientId
        )

        server.ProcessMessages(fun struct(clientId, msg) ->
            match msg with
            | :? UserInfo as userInfoMsg ->
                let mov = userInfoMsg.Movement
                let mutable player = playerStates.Buffer.[playerLookup.[clientId]]
                playerMovStates.Buffer.[playerLookup.[clientId]] <- mov
                playerStates.Buffer.[playerLookup.[clientId]] <- player
            |  _ -> ()
        )

        for i = 0 to playerStates.Count - 1 do
            updatePlayer playerMovStates.Buffer.[i] &playerStates.Buffer.[i]

        let snapshotMsg = server.CreateMessage<Snapshot>()
        snapshotMsg.PlayerCount <- playerStates.Count;

        for i = 0 to playerStates.Count - 1 do
            snapshotMsg.PlayerState.[i] <- playerStates.Buffer.[i]

        snapshotMsg.SnapshotId <- snapshotId
        snapshotId <- snapshotId + 1L
        server.SendMessage(snapshotMsg, 1uy)
        false

    static member Create(server) =
        ServerGame(server)
