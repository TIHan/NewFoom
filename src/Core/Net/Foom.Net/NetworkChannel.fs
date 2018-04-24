namespace Foom.Net

type internal TypeId = byte
type internal ChannelId = byte
type internal NetworkRegistrationFunc = (TypeId -> ChannelId -> MessageFactory -> unit)

type NetworkChannel = internal NetworkChannel of ChannelType * NetworkRegistrationFunc list

[<RequireQualifiedAccess>]
module NetworkChannel =

    let create channelType = NetworkChannel(channelType, [])

    let register<'T when 'T :> NetMessage and 'T : (new : unit -> 'T)> (poolAmount: int) = function
        | NetworkChannel(channelType, funcs) ->
            let func : NetworkRegistrationFunc = 
                fun (typeId: TypeId) (channelId: ChannelId) msgFactory -> 
                    msgFactory.RegisterMessage<'T>(typeId, channelId, poolAmount)
            NetworkChannel(channelType, func :: funcs)

[<AutoOpen>]
module internal NetworkHelpers =

    let defaultRegister (msgFactory: MessageFactory) =
        msgFactory.RegisterChannel(DefaultChannelIds.Connection, ChannelType.ReliableOrdered)
        msgFactory.RegisterChannel(DefaultChannelIds.Heartbeat, ChannelType.Reliable)
        msgFactory.RegisterMessage<Heartbeat>(Heartbeat.DefaultTypeId, DefaultChannelIds.Heartbeat, Heartbeat.DefaultPoolAmount)
        msgFactory.RegisterMessage<ConnectionRequested>(ConnectionRequested.DefaultTypeId, DefaultChannelIds.Connection, ConnectionRequested.DefaultPoolAmount)
        msgFactory.RegisterMessage<ConnectionChallengeRequested>(ConnectionChallengeRequested.DefaultTypeId, DefaultChannelIds.Connection, ConnectionChallengeRequested.DefaultPoolAmount)
        msgFactory.RegisterMessage<ConnectionChallengeAccepted>(ConnectionChallengeAccepted.DefaultTypeId, DefaultChannelIds.Connection, ConnectionChallengeAccepted.DefaultPoolAmount)
        msgFactory.RegisterMessage<ConnectionAccepted>(ConnectionAccepted.DefaultTypeId, DefaultChannelIds.Connection, ConnectionAccepted.DefaultPoolAmount)
        msgFactory.RegisterMessage<DisconnectRequested>(DisconnectRequested.DefaultTypeId, DefaultChannelIds.Connection, DisconnectRequested.DefaultPoolAmount)
        msgFactory.RegisterMessage<DisconnectAccepted>(DisconnectAccepted.DefaultTypeId, DefaultChannelIds.Connection, DisconnectAccepted.DefaultPoolAmount)
        msgFactory.RegisterMessage<ClientDisconnected>(ClientDisconnected.DefaultTypeId, DefaultChannelIds.Connection, ClientDisconnected.DefaultPoolAmount)

    let rec processNetworkChannels (msgFactory: MessageFactory) typeId channelId = function
        | [] -> ()
        | NetworkChannel(channelType, funcs) :: networkChannels ->
            msgFactory.RegisterChannel(channelId, channelType)

            let funcs =
                funcs
                |> List.rev

            let nextTypeId =
                (typeId, funcs)
                ||> List.fold (fun typeId func ->
                    func typeId channelId msgFactory
                    typeId + 1uy
                )
            processNetworkChannels msgFactory nextTypeId (channelId + 1uy) networkChannels

    let createMessageFactory networkChannels poolMultiply =
        let msgFactory = MessageFactory(poolMultiply)
        defaultRegister msgFactory
        processNetworkChannels msgFactory 0uy 0uy networkChannels
        msgFactory