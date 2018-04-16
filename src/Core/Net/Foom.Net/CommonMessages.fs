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
type ConnectionAccepted() =
    inherit NetMessage()

    member val ClientId = Unchecked.defaultof<ClientId> with get, set

    override this.Serialize(writer, stream) =
        base.Serialize(&writer, stream)
        writer.Write(stream, this.ClientId)

    override this.Deserialize(reader, stream) =
        base.Deserialize(&reader, stream)
        this.ClientId <- reader.Read(stream)

    override this.Reset() =
        base.Reset()
        this.ClientId <- Unchecked.defaultof<ClientId>

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
type ClientDisconnected() =
    inherit NetMessage()

    member val Reason = String.Empty with get, set

    override this.Serialize(writer, stream) =
        base.Serialize(&writer, stream)
        writer.WriteString(stream, this.Reason)
    
    override this.Deserialize(reader, stream) =
        base.Deserialize(&reader, stream)
        this.Reason <- reader.ReadString(stream)

    override this.Reset() =
        base.Reset()
        this.Reason <- String.Empty

    static member DefaultTypeId = UInt16.MaxValue - 5us

    static member DefaultPoolAmount = 20
