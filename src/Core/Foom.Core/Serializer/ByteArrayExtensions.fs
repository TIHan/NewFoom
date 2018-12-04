[<AutoOpen>]
module Foom.IO.Serializer.ByteArrayExtensions

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Collections.Generic
open System.Runtime.InteropServices
open FSharp.NativeInterop

[<Sealed>]
type ChunkedByteStream =
    
    val mutable private position : int
    val private chunks : byte [][]
    val private chunkSize : int
    val private size : int
    val private offsetSize : int

    new (chunks, chunkSize, size) =
        {
            position = 0
            chunks = chunks
            chunkSize = chunkSize
            size = size
            offsetSize = chunkSize
        }

    member private this.Item 
        with get i =
            this.chunks.[i / this.chunkSize].[i % this.chunkSize]
        and [<MethodImpl(MethodImplOptions.AggressiveInlining)>] set i value =
            this.chunks.[i / this.chunkSize].[i % this.chunkSize] <- value

    member this.WriteByte(value: byte) =
        let position = Interlocked.Add(&this.position, 1)

        if position >= this.size then
            failwith "outside of the byteStream"

        this.chunks.[position / this.chunkSize].[position] <- value

    member this.WriteSByte(value: sbyte) =
        let position = Interlocked.Add(&this.position, 1)

        if position >= this.size then
            failwith "outside of the byteStream"

        this.chunks.[position / this.chunkSize].[position] <- byte value

    member this.WriteUInt16(value: uint16) =
        let position2 = Interlocked.Add(&this.position, 2)
        let position1 = position2 - 1

        if position2 >= this.size then
            failwith "outside of the byteStream"

        this.chunks.[position1 / this.chunkSize].[position1] <- byte value
        this.chunks.[position2 / this.chunkSize].[position2] <- byte (value >>> 8)

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.WriteInt(value: int) =
        let position4 = Interlocked.Add(&this.position, 4) - 1
        let position3 = position4 - 1
        let position2 = position3 - 1
        let position1 = position2 - 1

        this.[position1] <- byte value
        this.[position2] <- byte (value >>> 8)
        this.[position3] <- byte (value >>> 16)
        this.[position4] <-     byte (value >>> 24)

        //let position = Interlocked.Add(&this.position, 4) - 1

        //this.[position - 3] <- byte value
        //this.[position - 2] <- byte (value >>> 8)
        //this.[position - 1] <- byte (value >>> 16)
        //this.[position] <-     byte (value >>> 24)

        //let position4 = Interlocked.Add(&this.position, 4) - 1
        //let position3 = position4 - 1
        //let position2 = position3 - 1
        //let position1 = position2 - 1
        //this.[position1] <- byte value
        //this.[position2] <- byte (value >>> 8)
        //this.[position3] <- byte (value >>> 16)
        //this.[position4] <-     byte (value >>> 24)

    member this.WriteUInt32(value: uint32) =
        let position4 = Interlocked.Add(&this.position, 4)
        let position3 = position4 - 1
        let position2 = position3 - 1
        let position1 = position2 - 1

        if position4 >= this.size then
            failwith "outside of the byteStream"

        this.chunks.[position1 / this.chunkSize].[position1] <- byte value
        this.chunks.[position2 / this.chunkSize].[position2] <- byte (value >>> 8)
        this.chunks.[position3 / this.chunkSize].[position3] <- byte (value >>> 16)
        this.chunks.[position4 / this.chunkSize].[position4] <- byte (value >>> 24)

    member this.WriteInt64(value: int64) =
        let position8 = Interlocked.Add(&this.position, 8)
        let position7 = position8 - 1
        let position6 = position7 - 1
        let position5 = position6 - 1
        let position4 = position5 - 1
        let position3 = position4 - 1
        let position2 = position3 - 1
        let position1 = position2 - 1

        if position4 >= this.size then
            failwith "outside of the byteStream"

        this.chunks.[position1 / this.chunkSize].[position1] <- byte value
        this.chunks.[position2 / this.chunkSize].[position2] <- byte (value >>> 8)
        this.chunks.[position3 / this.chunkSize].[position3] <- byte (value >>> 16)
        this.chunks.[position4 / this.chunkSize].[position4] <- byte (value >>> 24)
        this.chunks.[position5 / this.chunkSize].[position5] <- byte (value >>> 32)
        this.chunks.[position6 / this.chunkSize].[position6] <- byte (value >>> 40)
        this.chunks.[position7 / this.chunkSize].[position7] <- byte (value >>> 48)
        this.chunks.[position8 / this.chunkSize].[position8] <- byte (value >>> 56)

    //member this.WriteSingle(value: byref<single>) =
    //    let position = Interlocked.Add(&this.position, 4)
    //    let data = this.chunks.[position % this.chunkAmount]
    //    match this.Type with
    //    | WriterType.Default -> data.WriteSingle(this.position, value)
    //    | WriterType.ReadOnly -> value <- data.ReadSingle(this.position)
    //    | WriterType.CountOnly -> ()
    //    | _ -> failwith "invalid writer type"

    //member this.Write<'T when 'T : unmanaged>(value: byref<'T>) =
    //    let position = Interlocked.Add(&this.position, sizeof<'T>)
    //    let data = this.chunks.[position % this.chunkAmount]
    //    match this.Type with
    //    | WriterType.Default -> data.Write<'T>(this.position, &value)
    //    | WriterType.ReadOnly -> data.Read<'T>(this.position, &value)
    //    | WriterType.CountOnly -> ()
    //    | _ -> failwith "invalid writer type"

    //member this.WriteString(data: byte [], value: byref<string>) =
    //    let position = Interlocked.Add(&this.position, (4 + System.Text.Encoding.Unicode.GetByteCount(value)))
    //    let data = this.chunks.[position % this.chunkAmount]
    //    match this.Type with
    //    | WriterType.Default -> data.WriteString(this.position, value)
    //    | WriterType.ReadOnly -> value <- data.ReadString(this.position)
    //    | WriterType.CountOnly -> ()
    //    | _ -> failwith "invalid writer type"