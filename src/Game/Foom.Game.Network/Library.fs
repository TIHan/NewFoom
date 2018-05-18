namespace Foom.Game.Network

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.Game
open Foom.Net

type ServerGameEvent<'T> =
    | StartRequested of port: int
    | Started
    | StopRequested
    | Stopped
    | ServerMessageReceived of NetMessage

type ClientGameEvent<'Custom> =
    | ConnectionRequested of address: string * port: int
    | Connected of ClientId
    | DisconnectionRequested
    | Disconnected
    | ClientMessageReceived of NetMessage
    | Custom of 'Custom

[<AbstractClass>]
type AbstractNetworkClientGame<'Event>(network: Network) =
    inherit AbstractClientGame()

    let client = network.CreateClient()
    let eventQueue = ConcurrentQueue()

    abstract OnEvent : time: TimeSpan * interval: TimeSpan * ClientGameEvent<'Event> -> bool

    member this.Enqueue(evt) =
        eventQueue.Enqueue(Custom(evt))

    member this.Connect(address, port) =
        eventQueue.Enqueue(ConnectionRequested(address, port))

    member this.Disconnect() =
        eventQueue.Enqueue(DisconnectionRequested)

    member this.SendMessage(msg) = client.SendMessage(msg)

    member this.CreateMessage() = client.CreateMessage()

    member this.GetBeforeDeserializedEvent() = client.GetBeforeDeserializedEvent()

    override this.Update(time, interval) =
        let mutable willQuit = false
        client.Update(interval, {
            new IClientUpdate with

                member __.OnMessageReceived(msg) = 
                    match msg with
                    | ClientMessage.ConnectionAccepted(clientId) ->
                        if not willQuit && this.OnEvent(time, interval, Connected(clientId)) |> not then
                            willQuit <- true
                    | ClientMessage.DisconnectAccepted ->
                        if not willQuit && this.OnEvent(time, interval, Disconnected) |> not then
                            willQuit <- true
                    | ClientMessage.Message msg ->
                        if not willQuit && this.OnEvent(time, interval, ClientMessageReceived(msg)) |> not then
                            willQuit <- true

                member __.OnAfterMessagesReceived() = ()
        })

        let mutable evt = Unchecked.defaultof<ClientGameEvent<'Event>>
        while eventQueue.TryDequeue(&evt) && not willQuit do
            match evt with
            | ConnectionRequested(address, port) ->
                client.Connect(address, port)
            | DisconnectionRequested ->
                client.Disconnect()
            | _ -> ()
            willQuit <- this.OnEvent(time, interval, evt)
        willQuit



