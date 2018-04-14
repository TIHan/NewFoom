namespace Foom.Net

open System
open System.Collections.Generic
open System.Collections.Concurrent
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel

[<Sealed>]
type Sender(stream: PacketStream, channelLookup: Dictionary<byte, struct(AbstractChannel * PacketDeliveryType)>) =

    let queue = ConcurrentQueue()

    member __.EnqueueMessage(msg: Message, channelId, willRecycle) =
        // TODO: Remove ChannelId setter
        msg.ChannelId <- channelId
        queue.Enqueue struct(msg, willRecycle)

    member __.SendPackets(f) =
        let mutable msg = Unchecked.defaultof<struct(Message * bool)>
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
                let struct(channel, _) = channelLookup.[Message.GetChannelId(Span.op_Implicit data)]
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
