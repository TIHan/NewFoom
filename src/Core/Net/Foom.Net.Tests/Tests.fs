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
type TextMessage =
    inherit NetMessage

    val mutable text : string

    new () = { text = String.Empty }

    override this.NetSerialize(writer, stream) =
        writer.WriteString(stream, &this.text)

    override this.NetReset() =
        this.text <- String.Empty

[<Fact>]
let ``Udp Client and Server`` () =
    use udpServer = new UdpServer(27017)
    use udpClient = new UdpClient()

    Assert.True(udpClient.Connect("::1", 27017))

    let pool = MessagePool<TextMessage>(0uy, 64) :> MessagePoolBase
    let lookup = Array.zeroCreate 65536

    lookup.[int 0us] <- pool

    let channel = Channel(lookup)

    let msg = pool.Create() :?> TextMessage
    msg.IncrementRefCount()
    msg.text <- "BEEF"

    let msg2 = pool.Create() :?> TextMessage
    msg2.IncrementRefCount()
    msg2.text <- "BEEF2"

    let mutable text = ""

    let data = channel.SerializeMessage(msg2, true, fun data -> udpClient.Send(data))

    System.Threading.Thread.Sleep(700)

    Assert.True(udpServer.IsDataAvailable)

    let mutable endPoint = Unchecked.defaultof<IPEndPoint>
    channel.Receive(udpServer.Receive(&endPoint)) |> ignore

    channel.ProcessReceived(fun msg ->
        match msg with
        | :? TextMessage as msg ->
            text <- msg.text
        | _ -> ()
    )

    Assert.Equal("BEEF2", text)

[<Fact>]
let ``Udp Client and Server Simple`` () =
    let clientUpdateNoOp =
        { new IClientUpdate with

            member __.OnMessageReceived(_) = ()

            member __.OnAfterMessagesReceived() = () }

    let network = Network([])

    use server = network.CreateServer(27015, 8)
    use client = network.CreateClient()

    server.Start()

    client.Connect("::1", 27015)

    client.Update(TimeSpan.Zero, clientUpdateNoOp)

    System.Threading.Thread.Sleep(100)
    server.ReceivePackets()
    server.ProcessMessages(fun _ -> ())
    server.SendPackets()

    System.Threading.Thread.Sleep(100)
    client.Update(TimeSpan.Zero, clientUpdateNoOp)

    // Added extra two below because we added challenge requests.
    System.Threading.Thread.Sleep(100)
    server.ReceivePackets()
    server.ProcessMessages(fun _ -> ())
    server.SendPackets()

    System.Threading.Thread.Sleep(100)
    client.Update(TimeSpan.Zero, clientUpdateNoOp)

    Assert.True(client.IsConnected)

[<Fact>]
let ``Udp Client and Server Simple Big Message`` () =

    let network = 
        Network(
            [
                NetworkChannel.create ChannelType.Unreliable
                |> NetworkChannel.register<TextMessage> 64
            ]
        )

    //

    use server = network.CreateServer(27015, 8)
    use client = network.CreateClient()

    let mutable finalText = ""
    let mutable clientId = ClientId.Local

    let serverProcess = function
        | ServerMessage.ClientConnected(clientId') ->
            clientId <- clientId'
        | ServerMessage.ClientDisconnected(_) -> ()
        | ServerMessage.Message(_) -> ()

    let clientUpdate =
        { new IClientUpdate with

            member __.OnMessageReceived(msg) =
                match msg with
                | ClientMessage.Message(msg) ->
                    match msg with
                    | :? TextMessage as msg ->
                        finalText <- msg.text
                    |  _ -> ()
                | _ -> ()
                
            member __.OnAfterMessagesReceived() = () }

    //

    let stopwatch = System.Diagnostics.Stopwatch.StartNew()
    let mutable time = TimeSpan.Zero

    server.Start()

    client.Connect("::1", 27015)

    time <- stopwatch.Elapsed
    client.Update(TimeSpan.Zero, clientUpdate)

    System.Threading.Thread.Sleep(100)
    server.Time <- (stopwatch.Elapsed)
    server.ReceivePackets()
    server.ProcessMessages serverProcess
    server.SendPackets()

    System.Threading.Thread.Sleep(100)
    time <- stopwatch.Elapsed - time
    client.Update(TimeSpan.Zero, clientUpdate)

    // Added extra two below because we added challenge requests.
    System.Threading.Thread.Sleep(100)
    server.Time <- (stopwatch.Elapsed)
    server.ReceivePackets()
    server.ProcessMessages serverProcess
    server.SendPackets()

    System.Threading.Thread.Sleep(100)
    time <- stopwatch.Elapsed - time
    client.Update(TimeSpan.Zero, clientUpdate)


    Assert.True(client.IsConnected)
    Assert.NotEqual(ClientId.Local, clientId)

    //

    let expectedText = String(Array.init 5000 (fun i -> char i))
    let msg = server.CreateMessage<TextMessage>()
    msg.text <- expectedText

    server.SendMessage(msg, clientId, willRecycle = true)

    server.Time <- (stopwatch.Elapsed)
    server.ReceivePackets()
    server.ProcessMessages serverProcess
    server.SendPackets()

    System.Threading.Thread.Sleep(100)
    time <- stopwatch.Elapsed - time
    client.Update(TimeSpan.Zero, clientUpdate)

    Assert.Equal(expectedText, finalText)
