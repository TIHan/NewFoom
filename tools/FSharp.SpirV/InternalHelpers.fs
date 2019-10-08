module internal FSharp.Spirv.InternalHelpers

open System
open System.IO

[<RequireQualifiedAccess>]
module private LittleEndian =

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

[<NoEquality;NoComparison>]
type SpirvStream =
    {
        stream: Stream
        mutable remaining: int
        buffer128: byte []
    }

    member x.WriteUInt16 (v: uint16) = ()

    member x.WriteUInt32 (v: uint32) = ()

    member x.WriteString (v: string) = ()

    member x.WriteEnum<'T when 'T : enum<uint32>> (v: 'T) = ()

    member x.WriteOption (v: 'T option, f: 'T -> unit) = ()

    member x.WriteList (v: 'T list, f: 'T -> unit) = ()

    member x.ReadUInt16 () = Unchecked.defaultof<uint16>

    member x.ReadUInt32 () = Unchecked.defaultof<uint32>

    member x.ReadString () = Unchecked.defaultof<string>

    member x.ReadEnum<'T when 'T : enum<uint32>> () = Unchecked.defaultof<'T>

    member x.ReadOption (f: unit -> 'T) = Unchecked.defaultof<'T option>

    member x.ReadList (f: unit -> 'T) = Unchecked.defaultof<'T list>