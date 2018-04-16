namespace Foom.Net

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel

[<AbstractClass>]
type NetMessage() =
    inherit Message()

    member val ChannelId = 0uy with get, set

    override this.Serialize(writer, stream) =
        writer.WriteByte(stream, this.ChannelId) 

    override this.Deserialize(reader, stream) =
        this.ChannelId <- reader.ReadByte(stream)

    override this.Reset() =
        this.ChannelId <- 0uy

[<Sealed>]
type Sender(stream: PacketStream, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =

    let queue = ConcurrentQueue()

    member __.EnqueueMessage(msg: NetMessage, channelId, willRecycle) =
        msg.ChannelId <- channelId
        queue.Enqueue struct(msg, willRecycle)

    member __.SendPackets(f) =
        let mutable msg = Unchecked.defaultof<struct(NetMessage * bool)>
        while queue.TryDequeue(&msg) do
            let struct(msg, willRecycle) = msg
            let struct(channel, packetDeliveryType) = channelLookup.[msg.ChannelId]
            channel.SerializeMessage(msg, willRecycle, fun data -> stream.Send(data, packetDeliveryType) |> ignore)
        stream.ProcessSending(f)

[<Sealed>]
type Receiver(stream: PacketStream, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =
    member this.EnqueuePacket(packet: ReadOnlySpan<byte>) =

        stream.Receive(packet, fun data -> 
            let mutable data = data
            while data.Length > 0 do
                let channelId = data.[4]
                let struct(channel, _) = channelLookup.[channelId]
                let numBytesRead = channel.Receive(Span.op_Implicit data)

                if numBytesRead = 0 then
                    failwith "Unable to receive message."

                data <- data.Slice(numBytesRead)
        )

    /// Thread safe
    member this.ProcessMessages(f) =
        channelLookup
        |> Seq.iter (fun pair -> 
            let struct(channel, _) = pair.Value
            channel.ProcessReceived(f)
        )

[<Sealed>]
type NetChannel(stream, channelLookup) =

    let sender = Sender(stream, channelLookup)
    let receiver = Receiver(stream, channelLookup)

    /// Thread safe
    member __.SendMessage(msg, channelId, willRecycle) =
        sender.EnqueueMessage(msg, channelId, willRecycle)

    member __.SendPackets(f) =
        sender.SendPackets(f)

    member __.ReceivePacket(packet: ReadOnlySpan<byte>) =
        receiver.EnqueuePacket(packet)

    /// Thread safe
    member __.ProcessReceivedMessages(f) =
        receiver.ProcessMessages(f)