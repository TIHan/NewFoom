namespace Foom.Net

open System.Net
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Message
open Foom.IO.Message.Channel
open Foom.IO.Packet
open Foom.Core

type ChannelType =
    | Unreliable = 0
    | UnreliableSequenced = 1
    | Reliable = 3
    | ReliableSequenced = 4
    | ReliableOrdered = 5

type TypeToTypeId = ConcurrentDictionary<System.Type, byte>
type TypeIdToChannelId = byte []
type TypeIdToPool = MessagePoolBase []

[<Sealed>]
type MessageFactory(poolMultiply) =
    let lookupTypeId = TypeToTypeId()
    let lookupChannelId : TypeIdToChannelId = Array.zeroCreate 256
    let lookupPool : TypeIdToPool = Array.zeroCreate 256

    let channelLookup = ConcurrentDictionary<byte, MessagePoolBase [] -> struct(AbstractChannel * PacketDeliveryType)>()
    let channelTypeLookup = ConcurrentDictionary<byte, ChannelType>()

    member __.RegisterMessage<'T when 'T :> Message and 'T : (new : unit -> 'T)>(typeId: byte, channelId: byte, poolAmount: int) =
        if lookupTypeId.ContainsKey(typeof<'T>) then
            failwithf "Message, %A, already registered." typeof<'T>.Name

        lookupTypeId.[typeof<'T>] <- typeId
        lookupChannelId.[int typeId] <- channelId
        lookupPool.[int typeId] <- MessagePool<'T>(typeId, poolMultiply * poolAmount) :> MessagePoolBase

    member __.CreateChannelLookup() =
        let result = Dictionary()
        channelLookup
        |> Seq.iter (fun pair ->
            result.Add(pair.Key, pair.Value lookupPool)
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

    member __.GetTypeId(typ) = lookupTypeId.[typ]

    member inline this.GetTypeId<'T>() = this.GetTypeId(typeof<'T>)

    member __.GetChannelId(typeId: byte) = lookupChannelId.[int typeId]

    member __.GetPool(typeId: byte) = lookupPool.[int typeId]

    member inline this.GetPool<'T>() = this.GetPool(this.GetTypeId<'T>())

    member inline this.CreateMessage<'T when 'T :> Message>() =
        this.GetPool<'T>().Create() :?> 'T

    member __.RecycleMessage(msg: NetMessage) : unit =
        lookupPool.[int msg.TypeId].Recycle(msg)
