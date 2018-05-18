namespace Foom.Net

open Foom.IO.Message

[<Sealed>]
type Network(networkChannels: NetworkChannel list) =

    member __.CreateServer(port, maxClients) =
        let msgFactory = createMessageFactory networkChannels maxClients
        new Server(msgFactory, port, maxClients) :> IServer

    member __.CreateClient() =
        let msgFactory = createMessageFactory networkChannels 1
        new Client(msgFactory) :> IClient

    member __.CreateBackgroundServer(port, maxClients) =
        new BackgroundServer(networkChannels, port, maxClients)
     