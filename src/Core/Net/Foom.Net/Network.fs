namespace Foom.Net

open Foom.IO.Message

[<Sealed>]
type Network() =
    let msgReg = MessageRegistration()
    let channelLookupFactory = ChannelLookupFactory()

    let mutable didCreateServerOrClient = false

    do
        channelLookupFactory.RegisterChannel(DefaultChannelIds.Connection, ChannelType.ReliableOrdered)
        channelLookupFactory.RegisterChannel(DefaultChannelIds.Heartbeat, ChannelType.Reliable)
        msgReg.RegisterMessage<Heartbeat>(Heartbeat.DefaultTypeId, Heartbeat.DefaultPoolAmount)
        msgReg.RegisterMessage<ConnectionRequested>(ConnectionRequested.DefaultTypeId, ConnectionRequested.DefaultPoolAmount)
        msgReg.RegisterMessage<ConnectionAccepted>(ConnectionAccepted.DefaultTypeId, ConnectionAccepted.DefaultPoolAmount)
        msgReg.RegisterMessage<DisconnectRequested>(DisconnectRequested.DefaultTypeId, DisconnectRequested.DefaultPoolAmount)
        msgReg.RegisterMessage<DisconnectAccepted>(DisconnectAccepted.DefaultTypeId, DisconnectAccepted.DefaultPoolAmount)
        msgReg.RegisterMessage<ClientDisconnected>(ClientDisconnected.DefaultTypeId, ClientDisconnected.DefaultPoolAmount)

    member __.RegisterMessage<'T when 'T :> Message and 'T : (new : unit -> 'T)>(typeId, poolAmount) =
        if didCreateServerOrClient then
            failwith "Cannot register messages after a server or client has been created."
        msgReg.RegisterMessage<'T>(typeId, poolAmount)

    member __.RegisterChannel(channelId, channelType) =
        if didCreateServerOrClient then
            failwith "Cannot register channels after a server or client has been created."
        channelLookupFactory.RegisterChannel(channelId, channelType)

    member __.CreateServer(port, maxClients) =
        didCreateServerOrClient <- true
        new Server(msgReg, channelLookupFactory, port, maxClients)

    member __.CreateClient() =
        didCreateServerOrClient <- true
        new Client(msgReg, channelLookupFactory)

    member __.CreateBackgroundServer(port, maxClients) =
        didCreateServerOrClient <- true
        new BackgroundServer(msgReg, channelLookupFactory, port, maxClients)

    member __.CreateBackgroundClient() =
        didCreateServerOrClient <- true
        new BackgroundClient(msgReg, channelLookupFactory) :> IBackgroundClient 
     