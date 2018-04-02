[<AutoOpen>]
module Foom.IO.Serializer.SpanExtensions

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open FSharp.NativeInterop

// Disable native interop warnings
#nowarn "9"
#nowarn "51"

type SpanDelegate = delegate of Span<byte> -> unit

module internal LittleEndian =

    let inline (<--) (r: byref<byte>) (v: byte) =
        r <- v

    let inline write8 (data: Span<byte>) offset value =
        data.[offset] <-- byte value

    let inline write16 (data: Span<byte>) offset value =
        data.[offset] <-- byte value
        data.[offset + 1] <-- byte (value >>> 8)

    let inline write32 (data: Span<byte>) offset value =
        data.[offset] <-- byte value
        data.[offset + 1] <-- byte (value >>> 8)
        data.[offset + 2] <-- byte (value >>> 16)
        data.[offset + 3] <-- byte (value >>> 24)

    let inline read8 (data: ReadOnlySpan<byte>) offset =
        data.[offset]

    let inline read16 (data: ReadOnlySpan<byte>) offset =
        (uint16 data.[offset]) |||
        ((uint16 data.[offset + 1]) <<< 8)

    let inline read32 (data: ReadOnlySpan<byte>) offset =
        (uint32 data.[offset]) |||
        ((uint32 data.[offset + 1]) <<< 8) |||
        ((uint32 data.[offset + 2]) <<< 16) |||
        ((uint32 data.[offset + 3]) <<< 24)

[<Struct; StructLayout (LayoutKind.Explicit)>]
type internal SingleUnion =

    [<FieldOffset (0)>]
    val mutable Value : uint32

    [<FieldOffset (0)>]
    val mutable SingleValue : single

[<Struct>]
type Writer =

    val mutable position : int

    member inline this.WriteByte(data: Span<byte>, value: byte) =
        let result = LittleEndian.write8 data this.position value
        this.position <- this.position + 1
        result

    member inline this.WriteSByte(data: Span<byte>, value: sbyte) =
        let result = LittleEndian.write8 data this.position value
        this.position <- this.position + 1
        result

    member inline this.WriteInt16(data: Span<byte>, value: int16) =
        let result = LittleEndian.write16 data this.position value
        this.position <- this.position + 2
        result

    member inline this.WriteUInt16(data: Span<byte>, value: uint16) =
        let result = LittleEndian.write16 data this.position value
        this.position <- this.position + 2
        result

    member inline this.WriteInt(data: Span<byte>, value: int) =
        let result = LittleEndian.write32 data this.position value
        this.position <- this.position + 4
        result

    member inline this.WriteUInt32(data: Span<byte>, value: uint32) =
        let result = LittleEndian.write32 data this.position value
        this.position <- this.position + 4
        result

    member this.WriteSingle(data, value: single) =
        let mutable s = SingleUnion ()
        s.SingleValue <- value
        this.WriteUInt32(data, s.Value)

    member this.Write<'T when 'T : unmanaged>(data: Span<byte>, value: byref<'T>) =
        let size = sizeof<'T>
        Span((NativePtr.toNativeInt &&value).ToPointer(), size).CopyTo(data.Slice(this.position, size))
        this.position <- this.position + size

    member this.WriteString(data: Span<byte>, str: string) =
        let bytes = System.Text.Encoding.Unicode.GetBytes(str)
        this.WriteInt(data, bytes.Length)
        Span(bytes).CopyTo(data.Slice(this.position))
        this.position <- this.position + bytes.Length

[<Struct>]
type Reader =

    val mutable position : int

    member inline this.ReadByte data =
        let value = LittleEndian.read8 data this.position
        this.position <- this.position + 1
        value

    member inline this.ReadSByte data =
        let value = LittleEndian.read8 data this.position
        this.position <- this.position + 1
        sbyte value

    member inline this.ReadInt16 data =
        let value = LittleEndian.read16 data this.position
        this.position <- this.position + 2
        int16 value

    member inline this.ReadUInt16 data =
        let value = LittleEndian.read16 data this.position
        this.position <- this.position + 2
        uint16 value

    member inline this.ReadInt data =
        let value = LittleEndian.read32 data this.position
        this.position <- this.position + 4
        int value

    member inline this.ReadUInt32 data =
        let value = LittleEndian.read32 data this.position
        this.position <- this.position + 4
        uint32 value

    member this.ReadSingle data =
        let value = this.ReadUInt32 data
        let mutable s = SingleUnion()
        s.Value <- value
        s.SingleValue

    member this.Read<'T when 'T : unmanaged>(data: ReadOnlySpan<byte>) : 'T =
        let size = sizeof<'T>
        let mutable value = Unchecked.defaultof<'T>
        data.Slice(this.position, size).CopyTo(Span((NativePtr.toNativeInt &&value).ToPointer(), size))
        this.position <- this.position + size
        value

    member this.ReadString(data: ReadOnlySpan<byte>) : string =
        let length = this.ReadInt data
        let mutable ptr = data.Slice(this.position).DangerousGetPinnableReference()
        this.position <- this.position + length
        System.Text.Encoding.Unicode.GetString(&&ptr, length)
