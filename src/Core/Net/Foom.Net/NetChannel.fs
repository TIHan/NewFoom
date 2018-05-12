namespace Foom.Net

open System
open System.Threading
open System.Threading.Tasks
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel
open Foom.IO.Serializer

type Packet = Span<byte>
type MessageQueue = ConcurrentQueue<NetMessage>
type ChannelLookup = Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>

[<Sealed>]
type Sender(stream: PacketStream, msgFactory: MessageFactory, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =

    let queue = ConcurrentQueue()

    member __.EnqueueMessage(msg: NetMessage, willRecycle) =
        msg.channelId <- msgFactory.GetChannelId(msg.TypeId)
        queue.Enqueue(msg)

    member __.SendPackets(f) =
        let mutable msg = Unchecked.defaultof<NetMessage>
        while queue.TryDequeue(&msg) do
            let struct(channel, packetDeliveryType) = channelLookup.[msg.channelId]
            channel.SerializeMessage(msg, true, fun data -> stream.Send(data, packetDeliveryType) |> ignore)
        stream.ProcessSending(f)

[<Sealed>]
type Receiver(stream: PacketStream, msgFactory: MessageFactory, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =

    member this.EnqueuePacket(packet: Span<byte>) =

        stream.Receive(packet, fun data -> 
            let mutable data = data
            while data.Length > 0 do
                let typeId = data.[0]
                let channelId = data.[3] // This gets the channelId - don't change.

                if msgFactory.GetChannelId(typeId) <> channelId then
                    failwith "Message received with invalid channel."

                let struct(channel, _) = channelLookup.[channelId]
                let numBytesRead = channel.Receive(data)

                if numBytesRead = 0 then
                    failwith "Unable to receive message."

                data <- data.Slice(numBytesRead)
        )

    /// Thread safe
    member this.ProcessMessages(f: NetMessage -> unit) =
        channelLookup
        |> Seq.iter (fun pair -> 
            let struct(channel, _) = pair.Value
            channel.ProcessReceived(fun msg ->
                match msg with
                | :? NetMessage as msg -> f msg
                | _ -> failwith "Invalid NetMessage"
            )
        )

[<Sealed>]
type NetChannel(stream, msgFactory, channelLookup) =

    let sender = Sender(stream, msgFactory, channelLookup)
    let receiver = Receiver(stream, msgFactory, channelLookup)

    /// Thread safe
    member __.SendMessage(msg, willRecycle) =
        sender.EnqueueMessage(msg, willRecycle)

    /// Not thread safe
    member __.SendPackets(f) =
        sender.SendPackets(f)

    /// Not thread safe
    member __.ReceivePacket(packet: Span<byte>) =
        receiver.EnqueuePacket(packet)

    /// Thread safe
    member __.ProcessReceivedMessages(f) =
        receiver.ProcessMessages(f)

    /// Thread safe
    member __.GetBeforeSerializedEvent(typeId: byte) = 
        match channelLookup.TryGetValue(typeId) with
        | (true, struct(channel, _)) -> channel.GetBeforeSerializedEvent(typeId)
        | _ -> failwithf "TypeId, %i, has not been registered to a channel. Unable to get event." typeId

    /// Thread safe
    member __.GetBeforeDeserializedEvent(typeId: byte) = 
        match channelLookup.TryGetValue(typeId) with
        | (true, struct(channel, _)) -> channel.GetBeforeDeserializedEvent(typeId)
        | _ -> failwithf "TypeId, %i, has not been registered to a channel. Unable to get event." typeId
