[<AutoOpen>]
module internal Foom.Net.CommonMessages

open System
open Foom.IO.Message

[<RequireQualifiedAccess>]
module DefaultChannelIds =

    [<Literal>]
    let Connection = Byte.MaxValue

    let Heartbeat = Byte.MaxValue - 1uy

[<Sealed>]
type Heartbeat() =
    inherit NetMessage()

    static member DefaultTypeId = UInt16.MaxValue

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionRequested() =
    inherit NetMessage()

    static member DefaultTypeId = UInt16.MaxValue - 1us

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionAccepted =
    inherit NetMessage

    val mutable clientId : ClientId

    new () = { clientId = ClientId() }

    override this.NetSerialize(writer, stream) =
        writer.Write(stream, &this.clientId)

    override this.NetReset() =
        this.clientId <- ClientId()

    static member DefaultTypeId = UInt16.MaxValue - 2us

    static member DefaultPoolAmount = 20

[<Sealed>]
type DisconnectRequested() =
    inherit NetMessage()

    static member DefaultTypeId = UInt16.MaxValue - 3us

    static member DefaultPoolAmount = 20

[<Sealed>]
type DisconnectAccepted() =
    inherit NetMessage()

    static member DefaultTypeId = UInt16.MaxValue - 4us

    static member DefaultPoolAmount = 20

[<Sealed>]
type ClientDisconnected =
    inherit NetMessage

    val mutable reason : string

    new () = { reason = String.Empty }

    override this.NetSerialize(writer, stream) =
        writer.WriteString(stream, &this.reason)

    override this.NetReset() =
        this.reason <- String.Empty

    static member DefaultTypeId = UInt16.MaxValue - 5us

    static member DefaultPoolAmount = 20
