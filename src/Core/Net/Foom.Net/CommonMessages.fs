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

    static member DefaultTypeId = Byte.MaxValue

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionRequested() =
    inherit NetMessage()

    static member DefaultTypeId = Byte.MaxValue - 1uy

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionChallengeRequested =
    inherit NetMessage

    val mutable clientId : ClientId

    new () = { clientId = ClientId() }

    override this.NetSerialize(writer, stream) =
        writer.Write(stream, &this.clientId)

    override this.NetReset() =
        this.clientId <- ClientId()

    static member DefaultTypeId = Byte.MaxValue - 2uy

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionChallengeAccepted() =
    inherit NetMessage()

    static member DefaultTypeId = Byte.MaxValue - 3uy

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionAccepted() =
    inherit NetMessage()

    static member DefaultTypeId = Byte.MaxValue - 4uy

    static member DefaultPoolAmount = 20

[<Sealed>]
type DisconnectRequested() =
    inherit NetMessage()

    static member DefaultTypeId = Byte.MaxValue - 5uy

    static member DefaultPoolAmount = 20

[<Sealed>]
type DisconnectAccepted() =
    inherit NetMessage()

    static member DefaultTypeId = Byte.MaxValue - 6uy

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

    static member DefaultTypeId = Byte.MaxValue - 7uy

    static member DefaultPoolAmount = 20
