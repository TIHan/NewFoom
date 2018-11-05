namespace Foom.NativeCollections

open System
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

#nowarn "9"
#nowarn "42"

[<Sealed>]
type NativeQueue<'T when 'T : unmanaged>(capacity) =

    let mutable length = 
        if capacity <= 0 then
            failwith "Capacity must be greater than 0"
        sizeof<'T> * capacity
    let mutable count = 0
    let mutable startIndex = 0
    let mutable buffer = new NativeArray<'T>(capacity)
    let mutable ptr = buffer.Buffer

    member __.IncreaseCapacity() =
        let mutable newLength = uint32 length * 2u
        if newLength >= uint32 Int32.MaxValue then
            failwith "Length is bigger than the maximum number of elements in the array"

        let newBuffer = new NativeArray<'T>(buffer, length)

        (buffer :> IDisposable).Dispose()

        length <- int newLength
        buffer <- newBuffer
        ptr <- buffer.Buffer

    member this.Enqueue(item: inref<'T>) =
        if count * sizeof<'T> >= length then
            this.IncreaseCapacity ()
        let index = (startIndex + count) % capacity
        buffer.[index] <- item
        count <- count + 1

    member inline this.Enqueue(item: 'T) =
        this.Enqueue(&item)

    member __.Dequeue(item: outref<'T>) =
        let index = startIndex
        startIndex <- (startIndex + 1) % capacity
        count <- count - 1
        item <- buffer.[index]