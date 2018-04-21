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

[<RequireQualifiedAccess>]
module LittleEndian =

    let inline internal (<--) (r: byref<byte>) (v: byte) =
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

    let inline write64 (data: Span<byte>) offset value =
        data.[offset] <-- byte value
        data.[offset + 1] <-- byte (value >>> 8)
        data.[offset + 2] <-- byte (value >>> 16)
        data.[offset + 3] <-- byte (value >>> 24)
        data.[offset + 4] <-- byte (value >>> 32)
        data.[offset + 5] <-- byte (value >>> 40)
        data.[offset + 6] <-- byte (value >>> 48)
        data.[offset + 7] <-- byte (value >>> 56)

    let inline read8 (data: Span<byte>) offset =
        let x0 = data.[offset]
        x0

    let inline read16 (data: Span<byte>) offset =
        let x0 = data.[offset]
        let x1 = data.[offset + 1]
        (uint16 x0) |||
        ((uint16 x1) <<< 8)

    let inline read32 (data: Span<byte>) offset =
        let x0 = data.[offset]
        let x1 = data.[offset + 1]
        let x2 = data.[offset + 2]
        let x3 = data.[offset + 3]
        (uint32 x0) |||
        ((uint32 x1) <<< 8) |||
        ((uint32 x2) <<< 16) |||
        ((uint32 x3) <<< 24)

    let inline read64 (data: Span<byte>) offset =
        let x0 = data.[offset]
        let x1 = data.[offset + 1]
        let x2 = data.[offset + 2]
        let x3 = data.[offset + 3]
        let x4 = data.[offset + 4]
        let x5 = data.[offset + 5]
        let x6 = data.[offset + 6]
        let x7 = data.[offset + 7]
        (uint64 x0) |||
        ((uint64 x1) <<< 8) |||
        ((uint64 x2) <<< 16) |||
        ((uint64 x3) <<< 24) |||
        ((uint64 x4) <<< 32) |||
        ((uint64 x5) <<< 40) |||
        ((uint64 x6) <<< 48) |||
        ((uint64 x7) <<< 56)

[<Struct; StructLayout (LayoutKind.Explicit)>]
type internal SingleUnion =

    [<FieldOffset (0)>]
    val mutable Value : uint32

    [<FieldOffset (0)>]
    val mutable SingleValue : single

[<Struct>]
type Writer =

    val isReading : bool
    val mutable position : int

    member inline this.WriteByte(data: Span<byte>, value: byref<byte>) =
        let p = this.position
        this.position <- p + 1
        if this.isReading then
            value <- LittleEndian.read8 data p
        else
            LittleEndian.write8 data p value

    member inline this.WriteSByte(data: Span<byte>, value: byref<sbyte>) =
        let p = this.position
        this.position <- p + 1
        if this.isReading then
            value <- LittleEndian.read8 data p |> sbyte
        else
            LittleEndian.write8 data p value

    member inline this.WriteUInt16(data: Span<byte>, value: byref<uint16>) =
        let p = this.position
        this.position <- p + 2
        if this.isReading then
            value <- LittleEndian.read16 data p
        else
            LittleEndian.write16 data p value

    member inline this.WriteInt(data: Span<byte>, value: byref<int>) =
        let p = this.position
        this.position <- p + 4
        if this.isReading then
            value <- LittleEndian.read32 data p |> int
        else
            LittleEndian.write32 data p value

    member inline this.WriteUInt32(data: Span<byte>, value: byref<uint32>) =
        let p = this.position
        this.position <- p + 4
        if this.isReading then
            value <- LittleEndian.read32 data p
        else
            LittleEndian.write32 data p value

    member inline this.WriteInt64(data: Span<byte>, value: byref<int64>) =
        let p = this.position
        this.position <- p + 8
        if this.isReading then
            value <- LittleEndian.read64 data p |> int64
        else
            LittleEndian.write64 data p value

    member this.WriteSingle(data, value: byref<single>) =
        let mutable s = SingleUnion ()
        if not this.isReading then
            s.SingleValue <- value
        this.WriteUInt32(data, &s.Value)
        if this.isReading then
            value <- s.SingleValue

    member this.Write<'T when 'T : unmanaged>(data: Span<byte>, value: byref<'T>) =
        let size = sizeof<'T>
        if this.isReading then
            data.Slice(this.position, size).CopyTo(Span((NativePtr.toNativeInt &&value).ToPointer(), size))
            this.position <- this.position + size
        else
            Span((NativePtr.toNativeInt &&value).ToPointer(), size).CopyTo(data.Slice(this.position, size))
            this.position <- this.position + size

    member this.WriteString(data: Span<byte>, str: byref<string>) =
        if this.isReading then
            let mutable length = 0
            this.WriteInt(data, &length)
            let mutable ptr = data.Slice(this.position).DangerousGetPinnableReference()
            this.position <- this.position + length
            str <- System.Text.Encoding.Unicode.GetString(&&ptr, length)
        else
            let bytes = System.Text.Encoding.Unicode.GetBytes(str)
            let mutable length = bytes.Length
            this.WriteInt(data, &length)
            Span(bytes).CopyTo(data.Slice(this.position))
            this.position <- this.position + bytes.Length

    new (isReading) = { isReading = isReading; position = 0 }

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

    member inline this.ReadInt64 data =
        let value = LittleEndian.read64 data this.position
        this.position <- this.position + 8
        int64 value

    member this.ReadSingle data =
        let value = this.ReadUInt32 data
        let mutable s = SingleUnion()
        s.Value <- value
        s.SingleValue

    member this.Read<'T when 'T : unmanaged>(data: Span<byte>) : 'T =
        let size = sizeof<'T>
        let mutable value = Unchecked.defaultof<'T>
        data.Slice(this.position, size).CopyTo(Span((NativePtr.toNativeInt &&value).ToPointer(), size))
        this.position <- this.position + size
        value

    member this.ReadString(data: Span<byte>) : string =
        let length = this.ReadInt data
        let mutable ptr = data.Slice(this.position).DangerousGetPinnableReference()
        this.position <- this.position + length
        System.Text.Encoding.Unicode.GetString(&&ptr, length)
