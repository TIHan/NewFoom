module Tests

open System
open System.Collections.Generic
open Xunit
open Foom.IO.Message
open Foom.IO.Message.Channel

[<Sealed>]
type TextMessage() =
    inherit Message()

    member val Text = String.Empty with get, set

    override this.Serialize(writer, stream) =
        writer.WriteString(stream, this.Text)

    override this.Deserialize(reader, stream) =
        this.Text <- reader.ReadString stream

    override this.Reset() =
        this.Text <- String.Empty

[<Fact>]
let ``Normal`` () =
    let pool = MessagePool<TextMessage>(0us, 64) :> MessagePoolBase
    let lookup = Array.zeroCreate 65536

    lookup.[int 0us] <- pool

    let channel = Channel(lookup)

    let msg = pool.Create() :?> TextMessage
    msg.Text <- "BEEF"

    let msg2 = pool.Create() :?> TextMessage
    msg2.Text <- "BEEF2"

    channel.SerializeMessage(msg2, true, fun data -> channel.Receive(data) |> ignore)
    channel.SerializeMessage(msg, true, fun data -> channel.Receive(data) |> ignore)

    let results = ResizeArray()
    channel.ProcessReceived(fun msg ->
        match msg with
        | :? TextMessage as msg ->
            results.Add(msg.Text)
        | _ -> ()
    )

    Assert.Equal(2, results.Count)
    Assert.Equal("BEEF2", results.[0])
    Assert.Equal("BEEF", results.[1])

[<Fact>]
let ``Sequenced`` () =
    let pool = MessagePool<TextMessage>(0us, 64) :> MessagePoolBase
    let lookup = Array.zeroCreate 65536

    lookup.[int 0us] <- pool

    let channel = SequencedChannel(lookup)

    let msg = pool.Create() :?> TextMessage
    msg.Text <- "BEEF"

    let msg2 = pool.Create() :?> TextMessage
    msg2.Text <- "BEEF2"

    channel.SerializeMessage(msg2, true, fun data -> 
        // This modifies the bytes to set the sequence id for testing
        let x = data.[2]
        x <- 1uy
        channel.Receive(data) |> ignore)
    channel.SerializeMessage(msg, true, fun data -> 
        // This modifies the bytes to set the sequence id for testing
        let x = data.[2]
        x <- 0uy
        channel.Receive(data) |> ignore)

    let results = ResizeArray()
    channel.ProcessReceived(fun msg ->
        match msg with
        | :? TextMessage as msg ->
            results.Add(msg.Text)
        | _ -> ()
    )

    Assert.Equal(1, results.Count)
    Assert.Equal("BEEF2", results.[0])

[<Fact>]
let ``Ordered`` () =
    let pool = MessagePool<TextMessage>(0us, 64) :> MessagePoolBase
    let lookup = Array.zeroCreate 65536

    lookup.[int 0us] <- pool

    let channel = OrderedChannel(lookup)

    let msg = pool.Create() :?> TextMessage
    msg.Text <- "BEEF"

    let msg2 = pool.Create() :?> TextMessage
    msg2.Text <- "BEEF2"

    channel.SerializeMessage(msg2, true, fun data -> 
        // This modifies the bytes to set the sequence id for testing
        let x = data.[2]
        x <- 1uy
        channel.Receive(data) |> ignore)
    channel.SerializeMessage(msg, true, fun data -> 
        // This modifies the bytes to set the sequence id for testing
        let x = data.[2]
        x <- 0uy
        channel.Receive(data) |> ignore)

    let results = ResizeArray()
    channel.ProcessReceived(fun msg ->
        match msg with
        | :? TextMessage as msg ->
            results.Add(msg.Text)
        | _ -> ()
    )

    Assert.Equal(2, results.Count)
    Assert.Equal("BEEF", results.[0])
    Assert.Equal("BEEF2", results.[1])
