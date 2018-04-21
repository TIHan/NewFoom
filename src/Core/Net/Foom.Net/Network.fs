﻿namespace Foom.Net

open Foom.IO.Message

type internal TypeId = byte
type internal ChannelId = byte
type internal NetworkRegistrationFunc = (TypeId -> ChannelId -> MessageFactory -> unit)

type NetworkChannel = internal NetworkChannel of ChannelType * NetworkRegistrationFunc list

[<RequireQualifiedAccess>]
module NetworkChannel =

    let create channelType = NetworkChannel(channelType, [])

    let register<'T when 'T :> Message and 'T : (new : unit -> 'T)> (poolAmount: int) = function
        | NetworkChannel(channelType, funcs) ->
            let func : NetworkRegistrationFunc = 
                fun (typeId: TypeId) (channelId: ChannelId) msgFactory -> 
                    msgFactory.RegisterMessage<'T>(typeId, channelId, poolAmount)
            NetworkChannel(channelType, func :: funcs)

[<AutoOpen>]
module private NetworkHelpers =

    let defaultRegister (msgFactory: MessageFactory) =
        msgFactory.RegisterChannel(DefaultChannelIds.Connection, ChannelType.ReliableOrdered)
        msgFactory.RegisterChannel(DefaultChannelIds.Heartbeat, ChannelType.Reliable)
        msgFactory.RegisterMessage<Heartbeat>(Heartbeat.DefaultTypeId, DefaultChannelIds.Heartbeat, Heartbeat.DefaultPoolAmount)
        msgFactory.RegisterMessage<ConnectionRequested>(ConnectionRequested.DefaultTypeId, DefaultChannelIds.Connection, ConnectionRequested.DefaultPoolAmount)
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

[<Sealed>]
type Network(networkChannels: NetworkChannel list) =

    let createMessageFactory poolMultiply =
        let msgFactory = MessageFactory(poolMultiply)
        defaultRegister msgFactory
        processNetworkChannels msgFactory 0uy 0uy networkChannels
        msgFactory

    member __.CreateServer(port, maxClients) =
        let msgFactory = createMessageFactory maxClients
        new Server(msgFactory, port, maxClients)

    member __.CreateClient() =
        let msgFactory = createMessageFactory 1
        new Client(msgFactory)

    member __.CreateBackgroundServer(port, maxClients) =
        let msgFactory = createMessageFactory maxClients
        new BackgroundServer(msgFactory, port, maxClients)

    member __.CreateBackgroundClient() =
        let msgFactory = createMessageFactory 1
        new BackgroundClient(msgFactory) :> IBackgroundClient 
     