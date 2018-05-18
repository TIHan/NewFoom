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
    | Connected
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

    override this.Update(time, interval) =
        client.Update(time, {
            new IClientUpdate with

                member __.OnMessageReceived(msg) = 
                    match msg with
                    | ClientMessage.ConnectionAccepted(_) ->
                        eventQueue.Enqueue(Connected)
                    | ClientMessage.DisconnectAccepted ->
                        eventQueue.Enqueue(Disconnected)
                    | ClientMessage.Message msg ->
                        eventQueue.Enqueue(ClientMessageReceived(msg))

                member __.OnAfterMessagesReceived() = ()
        })

        let mutable evt = Unchecked.defaultof<ClientGameEvent<'Event>>
        let mutable willQuit = false
        while eventQueue.TryDequeue(&evt) && not willQuit do
            match evt with
            | ConnectionRequested(address, port) ->
                client.Connect(address, port)
            | DisconnectionRequested ->
                client.Disconnect()
            | _ -> ()
            willQuit <- this.OnEvent(time, interval, evt)
        willQuit



