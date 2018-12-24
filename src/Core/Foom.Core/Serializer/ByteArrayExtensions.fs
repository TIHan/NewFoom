[<AutoOpen>]
module Foom.IO.Serializer.ByteArrayExtensions

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Collections.Generic
open System.Collections.Immutable
open System.Runtime.InteropServices
open FSharp.NativeInterop
open System.Buffers

[<Sealed>]
type ChunkedArray<'T> =

    val private chunks : 'T[][]
    val private chunkSize : int
    val Length : int

    new (chunkSize, length) =
        let chunkCount = (length / chunkSize) + 1
        let chunks = Array.init chunkCount (fun _ -> Array.zeroCreate<'T> chunkSize)
        {
            chunks = chunks
            chunkSize = chunkSize
            Length = length
        }

    member this.Item 
        with get i =
            this.chunks.[i / this.chunkSize].[(i % this.chunkSize)]
        and set i value =
            this.chunks.[i / this.chunkSize].[(i % this.chunkSize)] <- value
    

[<Struct>]
type ChunkedByteStreamState =
    {
        position: int
        chunkIndex: int
    }

/// Thread safe but not atomic
[<Sealed>]
type ChunkedByteStream =
    
    val mutable private state : ChunkedByteStreamState
    val private chunks : byte [][]
    val private chunkSize : int
    val private writeSize : int
    val private size : int
    val private startOffset : int

    new (startOffset, chunkSize, size) =
        let chunkCount = (size / chunkSize) + 1
        let chunks = Array.init chunkCount (fun _ -> Array.zeroCreate<byte> chunkSize)
        {
            state = { position = 0; chunkIndex = 0}
            chunks = chunks
            chunkSize = chunkSize
            writeSize = chunkSize - startOffset
            size = size
            startOffset = startOffset
        }

    member this.Item 
        with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get i =
            this.chunks.[i / this.writeSize].[(i % this.writeSize) + this.startOffset]
        and [<MethodImpl(MethodImplOptions.AggressiveInlining)>] set i value =
            this.chunks.[i / this.writeSize].[(i % this.writeSize) + this.startOffset] <- value

    member this.Chunks =
        let builder = ImmutableArray.CreateBuilder()
        for chunk in this.chunks do
            builder.Add(Memory(chunk, 0, this.chunkSize))
        builder.ToImmutable()

    member this.Length = this.size

    member this.ChunkLength = this.chunkSize

    member this.WritingPosition = this.state.position

    member this.WriteInt(value: byref<int>) =
        let value = value
        let state = this.state

        let position1 = state.position
        let position2 = position1 + 1
        let position3 = position2 + 1
        let position4 = position3 + 1
        let nextPosition = position4 + 1

        if position4 >= this.writeSize then
            this.[position1] <- byte value
            this.[position2] <- byte (value >>> 8)
            this.[position3] <- byte (value >>> 16)
            this.[position4] <- byte (value >>> 24)

            this.state <- { position = (nextPosition % this.writeSize); chunkIndex = state.chunkIndex + 1 }
        else
            let data = this.chunks.[state.chunkIndex]
            data.[position1] <- byte value
            data.[position2] <- byte (value >>> 8)
            data.[position3] <- byte (value >>> 16)
            data.[position4] <- byte (value >>> 24)

            this.state <- { state with position = nextPosition }

    member this.WriteBytes(bytes: byte []) =
        let state = this.state

        let leftOver = this.writeSize - state.position
        if leftOver > 0 then
            let data = Span(this.chunks.[state.chunkIndex])
            let data = data.Slice(this.startOffset + state.position, this.chunkSize - this.startOffset - state.position)
            if bytes.Length < leftOver then
                Span(bytes, 0, bytes.Length).CopyTo(data)
            else
                Span(bytes, 0, leftOver).CopyTo(data)

        let extraCount = ((bytes.Length - leftOver) / this.writeSize)
        if extraCount > 0 then
            let mutable chunkIndex = state.chunkIndex + 1
            let mutable position = this.writeSize - leftOver
            for i = 0 to extraCount - 1 do
                let data = Span(this.chunks.[chunkIndex])
                if i = extraCount then
                    let leftOver = bytes.Length - leftOver - ((chunkIndex - 1) * (extraCount - 1))
                    if leftOver > 0 then
                        Span(bytes, 0, leftOver).CopyTo(data.Slice(this.startOffset, this.writeSize))
                    position <- this.writeSize - leftOver
                else
                    Span(bytes, 0, this.writeSize).CopyTo(data.Slice(this.startOffset, this.writeSize))
        
            this.state <- { position = position; chunkIndex = chunkIndex }

    member this.WriteString(str: byref<string>) =
        let bytes = System.Text.Encoding.Unicode.GetBytes(str)
        let mutable length = bytes.Length
        this.WriteInt(&length) // write length
        this.WriteBytes(bytes)

    interface IDisposable with

        member this.Dispose() =
            ()

/// Thread safe but not atomic
[<Sealed>]
type CountOnlyByteStream =
    
    val mutable position : int

    new () =
        {
            position = 0
        }

    member this.WriteInt(_value: byref<int>) =
        this.position <- this.position + 4
