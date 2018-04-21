namespace Foom.Net

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel
open Foom.IO.Serializer

type TypeToChannelMap = ConcurrentDictionary<uint16, byte>

[<AbstractClass>]
type NetMessage =
    inherit Message

    val mutable channelId : byte

    new () = { channelId = 0uy }

    override this.Serialize(writer, stream) =
        writer.WriteByte(stream, &this.channelId) 
        this.NetSerialize(&writer, stream)

    override this.Reset() =
        this.channelId <- 0uy
        this.NetReset()

    abstract NetSerialize : byref<Writer> * Span<byte> -> unit

    default __.NetSerialize(_, _) = ()

    abstract NetReset : unit -> unit

    default __.NetReset() = ()

[<Sealed>]
type Sender(stream: PacketStream, typeToChannelMap: TypeToChannelMap, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =

    let queue = ConcurrentQueue()

    member __.EnqueueMessage(msg: NetMessage, willRecycle) =
        msg.channelId <- typeToChannelMap.[msg.TypeId]
        queue.Enqueue struct(msg, willRecycle)

    member __.SendPackets(f) =
        let mutable msg = Unchecked.defaultof<struct(NetMessage * bool)>
        while queue.TryDequeue(&msg) do
            let struct(msg, willRecycle) = msg
            let struct(channel, packetDeliveryType) = channelLookup.[msg.channelId]
            channel.SerializeMessage(msg, willRecycle, fun data -> stream.Send(data, packetDeliveryType) |> ignore)
        stream.ProcessSending(f)

[<Sealed>]
type Receiver(stream: PacketStream, typeToChannelMap: TypeToChannelMap, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =
    member this.EnqueuePacket(packet: Span<byte>) =

        stream.Receive(packet, fun data -> 
            let mutable data = data
            while data.Length > 0 do
                let typeId = LittleEndian.read16 data 0
                let channelId = data.[4] // This gets the channelId - don't change.

                if typeToChannelMap.[typeId] <> channelId then
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
type NetChannel(stream, channelIdMap, channelLookup) =

    let sender = Sender(stream, channelIdMap, channelLookup)
    let receiver = Receiver(stream, channelIdMap, channelLookup)

    /// Thread safe
    member __.SendMessage(msg, willRecycle) =
        sender.EnqueueMessage(msg, willRecycle)

    member __.SendPackets(f) =
        sender.SendPackets(f)

    member __.ReceivePacket(packet: Span<byte>) =
        receiver.EnqueuePacket(packet)

    /// Thread safe
    member __.ProcessReceivedMessages(f) =
        receiver.ProcessMessages(f)