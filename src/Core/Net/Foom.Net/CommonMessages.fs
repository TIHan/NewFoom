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
    inherit Message()

    static member DefaultTypeId = UInt16.MaxValue

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionRequested() =
    inherit Message()

    static member DefaultTypeId = UInt16.MaxValue - 1us

    static member DefaultPoolAmount = 20

[<Sealed>]
type ConnectionAccepted() =
    inherit Message()

    member val ClientId = -1 with get, set

    override this.Serialize(writer, stream) =
        writer.WriteInt(stream, this.ClientId)

    override this.Deserialize(reader, stream) =
        this.ClientId <- reader.ReadInt(stream)

    override this.Reset() =
        this.ClientId <- -1

    static member DefaultTypeId = UInt16.MaxValue - 2us

    static member DefaultPoolAmount = 20

[<Sealed>]
type DisconnectRequested() =
    inherit Message()

    static member DefaultTypeId = UInt16.MaxValue - 3us

    static member DefaultPoolAmount = 20

[<Sealed>]
type DisconnectAccepted() =
    inherit Message()

    static member DefaultTypeId = UInt16.MaxValue - 4us

    static member DefaultPoolAmount = 20

[<Sealed>]
type ClientDisconnected() =
    inherit Message()

    member val Reason = String.Empty with get, set

    override this.Serialize(writer, stream) =
        writer.WriteString(stream, this.Reason)
    
    override this.Deserialize(reader, stream) =
        this.Reason <- reader.ReadString(stream)

    override this.Reset() =
        this.Reason <- String.Empty

    static member DefaultTypeId = UInt16.MaxValue - 5us

    static member DefaultPoolAmount = 20
