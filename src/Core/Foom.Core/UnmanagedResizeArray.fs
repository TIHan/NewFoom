namespace Foom.Core

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

#nowarn "9"
#nowarn "42"

module NativePtrExtension = begin
    [<NoDynamicInvocation>]
    let inline toByref (x: nativeptr<'T>) = (# "" x : 'T byref  #)
end

[<Sealed>]
type UnmanagedResizeArray<'T when 'T : unmanaged>(capacity) =

    let mutable length = sizeof<'T> * capacity
    let mutable count = 0
    let mutable buffer = 
        if capacity <= 0 then
            failwith "Capacity must be greater than 0"
        Marshal.AllocHGlobal(length)
        |> NativePtr.ofNativeInt<'T>

    member __.Buffer = buffer

    member __.IncreaseCapacity () =
        let mutable newLength = uint32 length * 2u
        if newLength >= uint32 Int32.MaxValue then
            failwith "Length is bigger than the maximum number of elements in the array"

        length <- int newLength

        let newBuffer = Marshal.ReAllocHGlobal(NativePtr.toNativeInt buffer, nativeint length) 
                        |> NativePtr.ofNativeInt
        buffer <- newBuffer

    member this.Add(item) =
        if count * sizeof<'T> >= length then
            this.IncreaseCapacity ()
        
        NativePtr.set buffer count item
        count <- count + 1

    member this.AddDefault() =
        if count * sizeof<'T> >= length then
            this.IncreaseCapacity ()
        
        NativePtr.set buffer count (Unchecked.defaultof<'T>)
        count <- count + 1

    member __.LastItem = NativePtr.get buffer (count - 1)

    member __.SwapRemoveAt index =
        if index >= count then
            failwith "Index out of bounds"

        let lastIndex = count - 1
        NativePtr.set buffer index (NativePtr.get buffer lastIndex)
        count <- lastIndex

    member inline this.Item
        with get index = NativePtrExtension.toByref (NativePtr.add this.Buffer index)

    member __.Count = count

    interface IDisposable with

        member __.Dispose() =
            Marshal.FreeHGlobal(NativePtr.toNativeInt buffer)

[<Struct>]
type UnmanagedArray<'T when 'T : unmanaged> =

    val Buffer : nativeptr<'T>
    val Length : int

    new (length) =
        let size = sizeof<'T> * length
        {
            Buffer = Marshal.AllocHGlobal(size) |> NativePtr.ofNativeInt<'T>
            Length = length
        }

    member inline this.Item
        with get index = NativePtrExtension.toByref (NativePtr.add this.Buffer index)

    static member Create(length, init) =
        let arr = new UnmanagedArray<_>(length)

        for i = 0 to arr.Length - 1 do
            let ref = arr.[i]
            ref <- init i

        arr

    interface IDisposable with

        member this.Dispose() =
            Marshal.FreeHGlobal(NativePtr.toNativeInt this.Buffer)