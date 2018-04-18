namespace Foom.Net

open System
open System.Net
open System.Net.Sockets
open System.Runtime.InteropServices

[<RequireQualifiedAccess>]
module internal UdpConstants =

    [<Literal>]
    let DefaultReceiveBufferSize = 645120

    [<Literal>]
    let DefaultSendBufferSize = 645120

[<AbstractClass>]
type Udp =

    val UdpClient : UdpClient

    val UdpClientV6 : UdpClient

    val Buffer : byte []

    val mutable receiveBufferSize : int

    val mutable sendBufferSize : int

    new() =
        let udpClient = new UdpClient(AddressFamily.InterNetwork)
        let udpClientV6 = new UdpClient(AddressFamily.InterNetworkV6)

        udpClient.Client.Blocking <- false
        udpClientV6.Client.Blocking <- false

        udpClient.Client.ReceiveBufferSize <- UdpConstants.DefaultReceiveBufferSize
        udpClientV6.Client.ReceiveBufferSize <- UdpConstants.DefaultReceiveBufferSize
        udpClient.Client.SendBufferSize <- UdpConstants.DefaultSendBufferSize
        udpClientV6.Client.SendBufferSize <- UdpConstants.DefaultSendBufferSize

        { 
            UdpClient = udpClient
            UdpClientV6 = udpClientV6
            Buffer = Array.zeroCreate 65536
            receiveBufferSize = UdpConstants.DefaultReceiveBufferSize
            sendBufferSize = UdpConstants.DefaultSendBufferSize
        }

    new(port) =
        let udpClient = new UdpClient(port, AddressFamily.InterNetwork)
        let udpClientV6 = new UdpClient(port, AddressFamily.InterNetworkV6)

        udpClient.Client.Blocking <- false
        udpClientV6.Client.Blocking <- false

        udpClient.Client.ReceiveBufferSize <- UdpConstants.DefaultReceiveBufferSize
        udpClientV6.Client.ReceiveBufferSize <- UdpConstants.DefaultReceiveBufferSize
        udpClient.Client.SendBufferSize <- UdpConstants.DefaultSendBufferSize
        udpClientV6.Client.SendBufferSize <- UdpConstants.DefaultSendBufferSize

        { 
            UdpClient = udpClient
            UdpClientV6 = udpClientV6
            Buffer = Array.zeroCreate 65536
            receiveBufferSize = UdpConstants.DefaultReceiveBufferSize
            sendBufferSize = UdpConstants.DefaultSendBufferSize
        }

    member this.IsDataAvailable = 
        this.UdpClient.Available > 0 || this.UdpClientV6.Available > 0

    member this.ReceiveBufferSize
        with get() = this.receiveBufferSize
        and set value =
            this.receiveBufferSize <- value
            this.UdpClient.Client.ReceiveBufferSize <- value
            this.UdpClientV6.Client.ReceiveBufferSize <- value

    member this.SendBufferSize
        with get() = this.sendBufferSize
        and set value =
            this.sendBufferSize <- value
            this.UdpClient.Client.SendBufferSize <- value
            this.UdpClientV6.Client.SendBufferSize <- value

    member this.Close() =
        this.UdpClient.Close()
        this.UdpClientV6.Close()

    member this.Receive([<Out>] remoteEndPoint: byref<IPEndPoint>) =
        if this.UdpClient.Available > 0 then

            let ipEndPoint = IPEndPoint(IPAddress.Any, 0)
            let mutable endPoint = ipEndPoint :> EndPoint

            let result =
                match this.UdpClient.Client.ReceiveFrom(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, &endPoint) with
                | 0 -> Span.Empty
                | byteCount -> Span(this.Buffer, 0, byteCount)

            remoteEndPoint <- endPoint :?> IPEndPoint
            result

        elif this.UdpClientV6.Available > 0 then

            let ipEndPoint = IPEndPoint(IPAddress.IPv6Any, 0)
            let mutable endPoint = ipEndPoint :> EndPoint

            let result =
                match this.UdpClientV6.Client.ReceiveFrom(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, &endPoint) with
                | 0 -> Span.Empty
                | byteCount -> Span(this.Buffer, 0, byteCount)

            remoteEndPoint <- endPoint :?> IPEndPoint
            result

        else 
            remoteEndPoint <- IPEndPoint(IPAddress.None, 0)
            Span.Empty

    interface IDisposable with

        member this.Dispose() =
            this.Close()
            (this.UdpClient :> IDisposable).Dispose()
            (this.UdpClientV6 :> IDisposable).Dispose()

[<Sealed>]
type UdpClient () =
    inherit Udp ()

    let mutable isConnected = false
    let mutable isIpV6 = false
       
    member this.Connect(address, port) =

        match IPAddress.TryParse(address) with
        | true, ipAddress -> 
            if ipAddress.AddressFamily = AddressFamily.InterNetwork then
                this.UdpClient.Connect(ipAddress, port)
                isConnected <- true
                isIpV6 <- false
                true
            elif ipAddress.AddressFamily = AddressFamily.InterNetworkV6 then
                this.UdpClientV6.Connect(ipAddress, port)
                isConnected <- true
                isIpV6 <- true
                true
            else
                false
        | _ ->
            if address.Equals("localhost", StringComparison.OrdinalIgnoreCase) then
                try
                    this.UdpClientV6.Connect(IPAddress.IPv6Loopback, port)
                    isConnected <- true
                    isIpV6 <- true
                with | _ ->
                    this.UdpClient.Connect(IPAddress.Loopback, port)
                    isConnected <- true
                    isIpV6 <- false
                true
            else
                false

    member this.Disconnect() =
        try
            this.UdpClient.Client.Disconnect true
            this.UdpClientV6.Client.Disconnect true
        with | _ -> ()
        isConnected <- false

    member this.RemoteEndPoint =
        if not isConnected then
            failwith "Remote End Point is invalid because we haven't tried to connect."

        if isIpV6 then
            this.UdpClientV6.Client.RemoteEndPoint :?> IPEndPoint
        else
            this.UdpClient.Client.RemoteEndPoint :?> IPEndPoint

    member this.Receive() =
        if not isConnected then
            failwith "Receive is invalid because we haven't tried to connect."

        let mutable remoteEndPoint = Unchecked.defaultof<IPEndPoint>
        this.Receive(&remoteEndPoint)

    member this.Send(payload: Span<byte>) =
        if not isConnected then
            failwith "Send is invalid because we haven't tried to connect."

        payload.CopyTo(Span(this.Buffer, 0, payload.Length))

        if isIpV6 then
            this.UdpClientV6.Send (this.Buffer, payload.Length) |> ignore
        else
            this.UdpClient.Send (this.Buffer, payload.Length) |> ignore

    member __.IsConnected = isConnected

[<Sealed>]
type UdpServer (port) =
    inherit Udp (port)

    let mutable bytesSentSinceLastCall = 0

    let mutable dataLossEveryOtherCall = false

    member this.Send(payload: Span<byte>, remoteEndPoint: IPEndPoint) =

        payload.CopyTo(Span(this.Buffer, 0, payload.Length))

        if remoteEndPoint.AddressFamily = AddressFamily.InterNetwork then
            let actualSize = 
                if this.CanForceDataLoss || (dataLossEveryOtherCall && this.CanForceDataLossEveryOtherCall) then
                    0
                else
                    this.UdpClient.Send (this.Buffer, payload.Length, remoteEndPoint)

            bytesSentSinceLastCall <- bytesSentSinceLastCall + actualSize
            dataLossEveryOtherCall <- not dataLossEveryOtherCall

        elif remoteEndPoint.AddressFamily = AddressFamily.InterNetworkV6 then
            let actualSize = 
                if this.CanForceDataLoss || (dataLossEveryOtherCall && this.CanForceDataLossEveryOtherCall) then
                    0
                else
                    this.UdpClientV6.Send (this.Buffer, payload.Length, remoteEndPoint)

            dataLossEveryOtherCall <- not dataLossEveryOtherCall
            bytesSentSinceLastCall <- bytesSentSinceLastCall + actualSize

    member this.Receive([<Out>] remoteEndPoint: byref<IPEndPoint>) =
        (this :> Udp).Receive(&remoteEndPoint)

    member this.BytesSentSinceLastCall () =
        let count = bytesSentSinceLastCall
        bytesSentSinceLastCall <- 0
        count

    member val CanForceDataLoss = false with get, set

    member val CanForceDataLossEveryOtherCall = false with get, set
