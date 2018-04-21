namespace Foom.Net

open Foom.IO.Message

type internal TypeId = uint16
type internal ChannelId = byte
type internal NetworkRegistrationFunc = (TypeId -> ChannelId -> MessageRegistration -> unit)

type NetworkChannel = internal NetworkChannel of ChannelType * NetworkRegistrationFunc list

[<RequireQualifiedAccess>]
module NetworkChannel =

    let create channelType = NetworkChannel(channelType, [])

    let register<'T when 'T :> Message and 'T : (new : unit -> 'T)> (poolAmount: int) = function
        | NetworkChannel(channelType, funcs) ->
            let func : NetworkRegistrationFunc = 
                fun (typeId: TypeId) (channelId: ChannelId) msgReg -> 
                    msgReg.RegisterMessage<'T>(typeId, channelId, poolAmount)
            NetworkChannel(channelType, func :: funcs)

[<Sealed>]
type Network(networkChannels: NetworkChannel list) =
    let msgReg = MessageRegistration()
    let channelLookupFactory = ChannelLookupFactory()

    let mutable didCreateServerOrClient = false

    let rec processNetworkChannels typeId channelId = function
        | [] -> ()
        | NetworkChannel(channelType, funcs) :: networkChannels ->
            channelLookupFactory.RegisterChannel(channelId, channelType)

            let funcs =
                funcs
                |> List.rev

            let nextTypeId =
                (typeId, funcs)
                ||> List.fold (fun typeId func ->
                    func typeId channelId msgReg
                    typeId + 1us
                )
            processNetworkChannels nextTypeId (channelId + 1uy) networkChannels

    do
        channelLookupFactory.RegisterChannel(DefaultChannelIds.Connection, ChannelType.ReliableOrdered)
        channelLookupFactory.RegisterChannel(DefaultChannelIds.Heartbeat, ChannelType.Reliable)
        msgReg.RegisterMessage<Heartbeat>(Heartbeat.DefaultTypeId, DefaultChannelIds.Heartbeat, Heartbeat.DefaultPoolAmount)
        msgReg.RegisterMessage<ConnectionRequested>(ConnectionRequested.DefaultTypeId, DefaultChannelIds.Connection, ConnectionRequested.DefaultPoolAmount)
        msgReg.RegisterMessage<ConnectionAccepted>(ConnectionAccepted.DefaultTypeId, DefaultChannelIds.Connection, ConnectionAccepted.DefaultPoolAmount)
        msgReg.RegisterMessage<DisconnectRequested>(DisconnectRequested.DefaultTypeId, DefaultChannelIds.Connection, DisconnectRequested.DefaultPoolAmount)
        msgReg.RegisterMessage<DisconnectAccepted>(DisconnectAccepted.DefaultTypeId, DefaultChannelIds.Connection, DisconnectAccepted.DefaultPoolAmount)
        msgReg.RegisterMessage<ClientDisconnected>(ClientDisconnected.DefaultTypeId, DefaultChannelIds.Connection, ClientDisconnected.DefaultPoolAmount)

        processNetworkChannels 0us 0uy networkChannels

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
     