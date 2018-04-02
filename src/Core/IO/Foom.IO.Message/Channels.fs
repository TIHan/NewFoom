namespace Foom.IO.Message.Channel

[<AbstractClass>]
type AbstractChannel internal (serializer: Serializer, receiver: Receiver) =

    /// Not thread safe.
    member __.SerializeMessage(msg, willRecycle, f) =
        serializer.SerializeMessage(msg, willRecycle, f)

    /// Not thread safe.
    member __.Receive(data) =
        receiver.Enqueue(data)

    /// Thread safe.
    member __.ProcessReceived(f) =
        receiver.Process(f)

[<Sealed>]
type Channel(lookup) =
    inherit AbstractChannel(Serializer(lookup), Receiver(ReceiverType.Normal, lookup))

[<Sealed>]
type SequencedChannel(lookup) =
    inherit AbstractChannel(Serializer(lookup), Receiver(ReceiverType.Sequenced, lookup))

[<Sealed>]
type OrderedChannel(lookup) =
    inherit AbstractChannel(Serializer(lookup), Receiver(ReceiverType.Ordered, lookup))
    