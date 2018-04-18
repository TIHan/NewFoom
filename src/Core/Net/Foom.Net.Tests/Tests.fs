module Tests

open System
open System.Net
open Xunit
open Foom.IO.Packet
open Foom.IO.Message
open Foom.IO.Message.Channel
open Foom.IO.Serializer
open Foom.Net
open System.Collections.Generic


[<Sealed>]
type TextMessage() =
    inherit NetMessage()

    member val Text = String.Empty with get, set

    override this.Serialize(writer, stream) =
        base.Serialize(&writer, stream)
        writer.WriteString(stream, this.Text)

    override this.Deserialize(reader, stream) =
        base.Deserialize(&reader, stream)
        this.Text <- reader.ReadString stream

    override this.Reset() =
        base.Reset()
        this.Text <- String.Empty

[<Fact>]
let ``Udp Client and Server`` () =
    use udpServer = new UdpServer(27015)
    use udpClient = new UdpClient()

    Assert.True(udpClient.Connect("::1", 27015))

    let pool = MessagePool<TextMessage>(0us, 64) :> MessagePoolBase
    let lookup = Array.zeroCreate 65536

    lookup.[int 0us] <- pool

    let channel = Channel(lookup)

    let msg = pool.Create() :?> TextMessage
    msg.Text <- "BEEF"

    let msg2 = pool.Create() :?> TextMessage
    msg2.Text <- "BEEF2"

    let mutable text = ""

    let data = channel.SerializeMessage(msg2, true, fun data -> udpClient.Send(data))

    System.Threading.Thread.Sleep(100)

    Assert.True(udpServer.IsDataAvailable)

    let mutable endPoint = Unchecked.defaultof<IPEndPoint>
    channel.Receive(udpServer.Receive(&endPoint)) |> ignore

    channel.ProcessReceived(fun msg ->
        match msg with
        | :? TextMessage as msg ->
            text <- msg.Text
        | _ -> ()
    )

    Assert.Equal("BEEF2", text)

[<Fact>]
let ``Udp Client and Server Simple`` () =
    let network = Network()

    use server = network.CreateServer(27015, 8)
    use client = network.CreateClient()

    server.Start()

    client.Connect("::1", 27015)

    client.ReceivePackets()
    client.ProcessMessages(fun _ -> ())
    client.SendPackets()

    System.Threading.Thread.Sleep(100)
    server.ReceivePackets()
    server.PumpMessages()
    server.SendMessages()

    System.Threading.Thread.Sleep(100)
    client.ReceivePackets()
    client.ProcessMessages(fun _ -> ())
    client.SendPackets()

    Assert.True(client.IsConnected)

[<Fact>]
let ``Udp Client and Server Simple Big Message`` () =
    let network = Network()

    network.RegisterMessage<TextMessage>(0us, 64)
    network.RegisterChannel(0uy, ChannelType.Unreliable)

    //

    use server = network.CreateServer(27015, 8)
    use client = network.CreateClient()

    let mutable finalText = ""
    let mutable clientId = ClientId.Local

    server.ClientConnected.Add(fun x -> clientId <- x)

    client.MessageReceived<TextMessage>().Add(fun struct(_, msg) ->
        match msg with
        | :? TextMessage as msg ->
            finalText <- msg.Text
        | _ -> ()
    )

    let clientProcess msg =
        match msg with
        | ClientMessage.Message(msg) ->
            match msg with
            | :? TextMessage as msg ->
                finalText <- msg.Text
            |  _ -> ()
        | _ -> ()

    //

    let stopwatch = System.Diagnostics.Stopwatch.StartNew()

    server.Start()

    client.Connect("::1", 27015)

    client.Time <- (stopwatch.Elapsed)
    client.ReceivePackets()
    client.ProcessMessages clientProcess
    client.SendPackets()

    System.Threading.Thread.Sleep(100)
    server.Time <- (stopwatch.Elapsed)
    server.ReceivePackets()
    server.PumpMessages()
    server.SendMessages()

    System.Threading.Thread.Sleep(100)
    client.Time <- (stopwatch.Elapsed)
    client.ReceivePackets()
    client.ProcessMessages clientProcess
    client.SendPackets()

    Assert.True(client.IsConnected)
    Assert.NotEqual(ClientId.Local, clientId)

    //

    let expectedText = String(Array.init 5000 (fun i -> char i))
    let msg = server.CreateMessage<TextMessage>()
    msg.Text <- expectedText

    server.SendMessage(msg, 0uy, clientId, willRecycle = true)

    server.Time <- (stopwatch.Elapsed)
    server.ReceivePackets()
    server.PumpMessages()
    server.SendMessages()

    System.Threading.Thread.Sleep(100)
    client.Time <- (stopwatch.Elapsed)
    client.ReceivePackets()
    client.ProcessMessages clientProcess
    client.SendPackets()

    Assert.Equal(expectedText, finalText)

[<Fact>]
let ``Udp Client and Server Simple - Background`` () =
    let network = Network()

    network.RegisterMessage<TextMessage>(0us, 64)
    network.RegisterChannel(0uy, ChannelType.Unreliable)

    use server = network.CreateBackgroundServer(27015, 8)
    use client = network.CreateBackgroundClient()

    let mutable serverHadException = false
    server.OnException.Add(fun ex ->
        serverHadException <- true
    )

    let mutable clientHadException = false
    client.OnException.Add(fun ex ->
        clientHadException <- true
    )

    server.ListenForMessage<TextMessage>()
    client.ListenForMessage<TextMessage>()

    server.Start()
    client.Connect("::1", 27015)

    System.Threading.Thread.Sleep(700)

    let mutable text = ""

    for i = 0 to 31 do
        text <- String.Empty
        let msg = client.CreateMessage<TextMessage>()
        msg.Text <- String.init 15000 (fun _ -> "c")

        client.ProcessMessages(fun _ -> ())
        client.SendMessage(msg, 0uy)

        System.Threading.Thread.Sleep(700)
        server.ProcessMessages(fun struct(_, msg) ->
            match msg with
            | :? TextMessage as msg ->
                text <- msg.Text
            | _ -> ()
        )

    System.Threading.Thread.Sleep(700)

    Assert.False(serverHadException)
    Assert.False(clientHadException)
    Assert.Equal(String.init 15000 (fun _ -> "c"), text)


