namespace Foom.Net

open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel

type ChannelType =
    | Unreliable = 0
    | UnreliableSequenced = 1
    | Reliable = 2
    | ReliableSequenced = 3
    | ReliableOrdered = 4

[<Sealed>]
type ChannelLookupFactory() =

    let channelLookup = ConcurrentDictionary<byte, MessagePoolBase [] -> struct(AbstractChannel * PacketDeliveryType)>()
    let channelTypeLookup = ConcurrentDictionary<byte, ChannelType>()

    member __.GetChannelType(channelId: byte) = channelTypeLookup.[channelId]

    member __.CreateChannelLookup(lookup) =
        let result = Dictionary()
        channelLookup
        |> Seq.iter (fun pair ->
            result.Add(pair.Key, pair.Value lookup)
        )
        result

    member __.RegisterChannel(channelId, channelType: ChannelType) =
        if channelLookup.ContainsKey(channelId) then
            failwithf "ChannelId, %i, already registered." channelId

        channelTypeLookup.[channelId] <- channelType

        match channelType with
        | ChannelType.Unreliable ->
            channelLookup.[channelId] <- 
                fun lookup -> struct(Channel(lookup) :> AbstractChannel, PacketDeliveryType.Unreliable)
        | ChannelType.UnreliableSequenced ->
            channelLookup.[channelId] <- 
                fun lookup -> struct(SequencedChannel(lookup) :> AbstractChannel, PacketDeliveryType.Unreliable)
        | ChannelType.Reliable ->
            channelLookup.[channelId] <- 
                fun lookup -> struct(Channel(lookup) :> AbstractChannel, PacketDeliveryType.Reliable)
        | ChannelType.ReliableSequenced ->
            channelLookup.[channelId] <- 
                fun lookup -> struct(SequencedChannel(lookup) :> AbstractChannel, PacketDeliveryType.Reliable)
        | ChannelType.ReliableOrdered ->
            channelLookup.[channelId] <-
                fun lookup -> struct(OrderedChannel(lookup) :> AbstractChannel, PacketDeliveryType.Reliable)
        | _ -> failwith "Invalid channel type."
