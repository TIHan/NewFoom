namespace Foom.Net

open Foom.IO.Message

[<Sealed>]
type Network(networkChannels: NetworkChannel list) =

    member __.CreateServer(port, maxClients) =
        let msgFactory = createMessageFactory networkChannels maxClients
        new Server(msgFactory, port, maxClients)

    member __.CreateClient() =
        let msgFactory = createMessageFactory networkChannels 1
        new Client(msgFactory)

    member __.CreateBackgroundServer(port, maxClients) =
        new BackgroundServer(networkChannels, port, maxClients)

    member __.CreateBackgroundClient() =
        new BackgroundClient(networkChannels) :> IBackgroundClient 
     