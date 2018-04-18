namespace Foom.Net

open System
open System.Net
open System.Net.Sockets
open System.Runtime.InteropServices

[<AbstractClass>]
type Udp =

    member IsDataAvailable : bool

    member ReceiveBufferSize : int with get, set

    member SendBufferSize : int with get, set

    member Close : unit -> unit

    interface IDisposable

[<Sealed>]
type UdpClient =
    inherit Udp

    new : unit -> UdpClient

    member Connect : address: string * port: int -> bool

    member Disconnect : unit -> unit

    member RemoteEndPoint : IPEndPoint

    member Receive : unit -> Span<byte>

    member Send : payload: Span<byte> -> unit

    member IsConnected : bool

[<Sealed>]
type UdpServer =
    inherit Udp

    new : port: int -> UdpServer

    member Send : payload: Span<byte> * remoteEndPoint: IPEndPoint -> unit

    member Receive : remoteEndPoint: byref<IPEndPoint> -> Span<byte>

    member BytesSentSinceLastCall : unit -> int

    member CanForceDataLoss : bool with get, set

    member CanForceDataLossEveryOtherCall : bool with get, set
