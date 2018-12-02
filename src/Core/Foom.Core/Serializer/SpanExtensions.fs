[<AutoOpen>]
module Foom.IO.Serializer.SpanExtensions

open System
open System.Runtime.CompilerServices
open System.Collections.Generic
open System.Runtime.InteropServices
open FSharp.NativeInterop

// Disable native interop warnings
#nowarn "9"
#nowarn "51"

type SpanDelegate = delegate of Span<byte> -> unit

[<RequireQualifiedAccess>]
module LittleEndian =

    let inline write8 (data: Span<byte>) offset value =
        data.[offset] <- byte value

    let inline write16 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)

    let inline write32 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)
        data.[offset + 2] <- byte (value >>> 16)
        data.[offset + 3] <- byte (value >>> 24)

    let inline write64 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)
        data.[offset + 2] <- byte (value >>> 16)
        data.[offset + 3] <- byte (value >>> 24)
        data.[offset + 4] <- byte (value >>> 32)
        data.[offset + 5] <- byte (value >>> 40)
        data.[offset + 6] <- byte (value >>> 48)
        data.[offset + 7] <- byte (value >>> 56)

    let inline read8 (data: ReadOnlySpan<byte>) offset =
        data.[offset]

    let inline read16 (data: ReadOnlySpan<byte>) offset =
        ( uint16 data.[offset]) |||
        ((uint16 data.[offset + 1]) <<< 8)

    let inline read32 (data: ReadOnlySpan<byte>) offset =
        ( uint32 data.[offset]) |||
        ((uint32 data.[offset + 1]) <<< 8) |||
        ((uint32 data.[offset + 2]) <<< 16) |||
        ((uint32 data.[offset + 3]) <<< 24)

    let inline read64 (data: ReadOnlySpan<byte>) offset =
        ( uint64 data.[offset]) |||
        ((uint64 data.[offset + 1]) <<< 8) |||
        ((uint64 data.[offset + 2]) <<< 16) |||
        ((uint64 data.[offset + 3]) <<< 24) |||
        ((uint64 data.[offset + 4]) <<< 32) |||
        ((uint64 data.[offset + 5]) <<< 40) |||
        ((uint64 data.[offset + 6]) <<< 48) |||
        ((uint64 data.[offset + 7]) <<< 56)

[<Struct; StructLayout (LayoutKind.Explicit)>]
type SingleUnion =

    [<FieldOffset (0)>]
    val mutable Value : uint32

    [<FieldOffset (0)>]
    val mutable SingleValue : single

type Span<'T> with

    member inline this.AsReadOnly : ReadOnlySpan<'T> = Span.op_Implicit(this)

[<Extension;Sealed>]
type SpanByteExtensions =

    [<Extension>]
    static member inline WriteByte(this: Span<byte>, offset, value: byte) =
        LittleEndian.write8 this offset value

    [<Extension>]
    static member inline WriteSByte(this: Span<byte>, offset, value: sbyte) =
        LittleEndian.write8 this offset value

    [<Extension>]
    static member inline WriteUInt16(this: Span<byte>, offset, value: uint16) =
        LittleEndian.write16 this offset value

    [<Extension>]
    static member inline WriteInt16(this: Span<byte>, offset, value: int16) =
        LittleEndian.write16 this offset value

    [<Extension>]
    static member inline WriteUInt32(this: Span<byte>, offset, value: uint32) =
        LittleEndian.write32 this offset value

    [<Extension>]
    static member inline WriteInt(this: Span<byte>, offset, value: int) =
        LittleEndian.write32 this offset value

    [<Extension>]
    static member inline WriteUInt64(this: Span<byte>, offset, value: uint64) =
        LittleEndian.write64 this offset value

    [<Extension>]
    static member inline WriteInt64(this: Span<byte>, offset, value: int64) =
        LittleEndian.write64 this offset value

    [<Extension>]
    static member inline WriteSingle(this: Span<byte>, offset, value: single) =
        let mutable s = SingleUnion ()
        s.SingleValue <- value
        this.WriteUInt32(offset, s.Value)

    [<Extension>]
    static member inline Write<'T when 'T : unmanaged>(this: Span<byte>, offset, value: inref<'T>) =
        let size = sizeof<'T>
        Span(NativePtr.toVoidPtr &&value, size).CopyTo(this.Slice(offset, size))

    [<Extension>]
    static member WriteString(this: Span<byte>, offset, str: string) =
        let bytes = System.Text.Encoding.Unicode.GetBytes(str)
        this.WriteInt(offset, bytes.Length) // write length
        Span(bytes).CopyTo(this.Slice(offset + 4)) // now write the string

    [<Extension>]
    static member inline ReadByte(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read8 this offset

    [<Extension>]
    static member inline ReadSByte(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read8 this offset |> sbyte

    [<Extension>]
    static member inline ReadUInt16(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read16 this offset

    [<Extension>]
    static member inline ReadInt16(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read16 this offset |> int16

    [<Extension>]
    static member inline ReadUInt32(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read32 this offset

    [<Extension>]
    static member inline ReadInt(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read32 this offset |> int

    [<Extension>]
    static member inline ReadUInt64(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read64 this offset

    [<Extension>]
    static member inline ReadInt64(this: ReadOnlySpan<byte>, offset) =
        LittleEndian.read64 this offset |> int64

    [<Extension>]
    static member inline ReadSingle(this: ReadOnlySpan<byte>, offset) =
        let mutable s = SingleUnion ()
        s.Value <- this.ReadUInt32(offset)
        s.SingleValue

    [<Extension>]
    static member inline Read<'T when 'T : unmanaged>(this: ReadOnlySpan<byte>, offset, value: outref<'T>) =
        let size = sizeof<'T>
        this.Slice(offset, size).CopyTo(Span(NativePtr.toVoidPtr &&value, size))

    [<Extension>]
    static member ReadString(this: ReadOnlySpan<byte>, offset) =
        let length = this.ReadInt(offset)
        let ptr = &this.Slice(offset + 4).GetPinnableReference()
        System.Text.Encoding.Unicode.GetString(&&ptr, length)

type WriterType =
    | Default = 0
    | ReadOnly = 1
    | CountOnly = 2
    
[<Struct>]
type Writer =

    val Type : WriterType
    val mutable position : int

    member inline this.WriteByte(data: Span<byte>, value: byref<byte>) =
        match this.Type with
        | WriterType.Default -> data.WriteByte(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadByte(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 1

    member inline this.WriteSByte(data: Span<byte>, value: byref<sbyte>) =
        match this.Type with
        | WriterType.Default -> data.WriteSByte(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadSByte(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 1

    member inline this.WriteUInt16(data: Span<byte>, value: byref<uint16>) =
        match this.Type with
        | WriterType.Default -> data.WriteUInt16(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadUInt16(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 2

    member inline this.WriteInt(data: Span<byte>, value: byref<int>) =
        match this.Type with
        | WriterType.Default -> data.WriteInt(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadInt(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 4

    member inline this.WriteUInt32(data: Span<byte>, value: byref<uint32>) =
        match this.Type with
        | WriterType.Default -> data.WriteUInt32(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadUInt32(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 4

    member inline this.WriteInt64(data: Span<byte>, value: byref<int64>) =
        match this.Type with
        | WriterType.Default -> data.WriteInt64(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadInt64(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 8

    member this.WriteSingle(data: Span<byte>, value: byref<single>) =
        match this.Type with
        | WriterType.Default -> data.WriteSingle(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadSingle(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + 4

    member this.Write<'T when 'T : unmanaged>(data: Span<byte>, value: byref<'T>) =
        match this.Type with
        | WriterType.Default -> data.Write<'T>(this.position, &value)
        | WriterType.ReadOnly -> data.AsReadOnly.Read<'T>(this.position, &value)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + sizeof<'T>

    member this.WriteString(data: Span<byte>, value: byref<string>) =
        match this.Type with
        | WriterType.Default -> data.WriteString(this.position, value)
        | WriterType.ReadOnly -> value <- data.AsReadOnly.ReadString(this.position)
        | WriterType.CountOnly -> ()
        | _ -> failwith "invalid writer type"
        this.position <- this.position + (4 + System.Text.Encoding.Unicode.GetByteCount(value))

    new (isReading) = 
        { 
            Type = if isReading then WriterType.ReadOnly else WriterType.Default
            position = 0 
        }
