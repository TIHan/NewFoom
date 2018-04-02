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

    member Receive : unit -> ReadOnlySpan<byte>

    member Send : payload: ReadOnlySpan<byte> -> unit

    member IsConnected : bool

[<Sealed>]
type UdpServer =
    inherit Udp

    new : port: int -> UdpServer

    member Send : payload: ReadOnlySpan<byte> * remoteEndPoint: IPEndPoint -> unit

    member Receive : remoteEndPoint: byref<IPEndPoint> -> ReadOnlySpan<byte>

    member BytesSentSinceLastCall : unit -> int

    member CanForceDataLoss : bool with get, set

    member CanForceDataLossEveryOtherCall : bool with get, set
