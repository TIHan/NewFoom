[<AutoOpen>]
module Foom.IO.Serializer.ByteArrayExtensions

open System
open System.Runtime.CompilerServices
open System.Threading
open System.Collections.Generic
open System.Runtime.InteropServices
open FSharp.NativeInterop

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
    val private size : int
    val private offsetSize : int

    new (chunks, chunkSize, size) =
        {
            state = { position = 0; chunkIndex = 0}
            chunks = chunks
            chunkSize = chunkSize
            size = size
            offsetSize = chunkSize
        }

    member this.Item 
        with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get i =
            this.chunks.[i / this.chunkSize].[i % this.chunkSize]
        and [<MethodImpl(MethodImplOptions.AggressiveInlining)>] set i value =
            this.chunks.[i / this.chunkSize].[i % this.chunkSize] <- value

    member this.WriteInt(value: int) =
        let state = this.state

        let position1 = state.position
        let position2 = position1 + 1
        let position3 = position2 + 1
        let position4 = position3 + 1
        let nextPosition = position4 + 1

        if position4 >= this.chunkSize then
            this.[position1] <- byte value
            this.[position2] <- byte (value >>> 8)
            this.[position3] <- byte (value >>> 16)
            this.[position4] <- byte (value >>> 24)

            this.state <- { position = nextPosition % this.chunkSize; chunkIndex = state.chunkIndex + 1 }
        else
            let data = this.chunks.[state.chunkIndex]
            data.[position1] <- byte value
            data.[position2] <- byte (value >>> 8)
            data.[position3] <- byte (value >>> 16)
            data.[position4] <- byte (value >>> 24)

            this.state <- { state with position = nextPosition }
