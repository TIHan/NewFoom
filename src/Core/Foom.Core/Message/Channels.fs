namespace Foom.IO.Message.Channel

open System

[<AbstractClass>]
type AbstractChannel internal (lookup, receiverType) =

    let serializer = new Serializer(lookup)
    let receiver = Receiver(receiverType, lookup)

    /// Not thread safe.
    member __.SerializeMessage(msg, willRecycle, f) =
        serializer.SerializeMessage(msg, willRecycle, f)

    /// Not thread safe.
    member __.Receive(data) =
        receiver.Enqueue(data)

    /// Thread safe.
    member __.ProcessReceived(f) =
        receiver.Process(f)

    /// Thread safe.
    member __.GetBeforeSerializedEvent(typeId) = serializer.GetBeforeSerializedEvent(typeId)

    /// Thread safe.
    member __.GetBeforeDeserializedEvent(typeId) = receiver.GetBeforeDeserializedEvent(typeId)

    interface IDisposable with

        member __.Dispose() =
            (serializer :> IDisposable).Dispose()

[<Sealed>]
type Channel(lookup) =
    inherit AbstractChannel(lookup, ReceiverType.Normal)

[<Sealed>]
type SequencedChannel(lookup) =
    inherit AbstractChannel(lookup, ReceiverType.Sequenced)

[<Sealed>]
type OrderedChannel(lookup) =
    inherit AbstractChannel(lookup, ReceiverType.Ordered)
    