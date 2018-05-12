namespace Foom.NativeCollections

open System
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

#nowarn "9"
#nowarn "42"

[<Sealed>]
type NativeResizeArray<'T when 'T : unmanaged>(capacity) =

    let mutable length = 
        if capacity <= 0 then
            failwith "Capacity must be greater than 0"
        sizeof<'T> * capacity
    let mutable count = 0
    let mutable buffer = new NativeArray<'T>(capacity)

    member __.Count = count

    member __.Buffer = buffer

    member __.IncreaseCapacity () =
        let mutable newLength = uint32 length * 2u
        if newLength >= uint32 Int32.MaxValue then
            failwith "Length is bigger than the maximum number of elements in the array"

        length <- int newLength
        buffer <- NativeArray.resize length buffer

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Add(item: 'T) =
        if count * sizeof<'T> >= length then
            this.IncreaseCapacity ()
        
        NativePtr.set this.Buffer.Buffer count item
        count <- count + 1

    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    member this.Add(item: byref<'T>) =
        if count * sizeof<'T> >= length then
            this.IncreaseCapacity ()
        
        NativePtr.set buffer.Buffer count item
        count <- count + 1

    member this.LastItem = NativePtr.get this.Buffer.Buffer (count - 1)

    member this.SwapRemoveAt index =
        if index >= count then
            failwith "Index out of bounds"

        let lastIndex = count - 1
        NativePtr.set buffer.Buffer index (NativePtr.get this.Buffer.Buffer lastIndex)
        count <- lastIndex

    member inline this.Item
        with get index = NativePtr.read (NativePtr.add this.Buffer.Buffer index)
        and set index value = NativePtr.set this.Buffer.Buffer index value

    member inline this.GetByRef(index) =
        NativePtrExtension.toByref (NativePtr.add this.Buffer.Buffer index)

    member inline this.ToSpan() =
        Span<'T>((this.Buffer.Buffer |> NativePtr.toNativeInt).ToPointer(), this.Count * sizeof<'T>)

    interface IDisposable with

        member __.Dispose() =
            Marshal.FreeHGlobal(NativePtr.toNativeInt buffer.Buffer)