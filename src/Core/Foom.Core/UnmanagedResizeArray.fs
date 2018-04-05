﻿namespace Foom.Core

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

    member __.IncreaseCapacity () =
        let mutable newLength = uint32 length * 2u
        if newLength >= uint32 Int32.MaxValue then
            failwith "Length is bigger than the maximum number of elements in the array"

        length <- int newLength
        buffer <- 
            Marshal.ReAllocHGlobal(NativePtr.toNativeInt buffer, NativePtr.toNativeInt &&length) 
            |> NativePtr.ofNativeInt

    member this.Add item =
        if count >= length then
            this.IncreaseCapacity ()
        
        NativePtr.set buffer count item
        count <- count + 1

    member __.LastItem = NativePtr.get buffer (count - 1)

    member __.SwapRemoveAt index =
        if index >= count then
            failwith "Index out of bounds"

        let lastIndex = count - 1

        NativePtr.set buffer index (NativePtr.get buffer lastIndex)
        NativePtr.set buffer lastIndex Unchecked.defaultof<'T>
        count <- lastIndex

    member __.Item
        with get index = NativePtrExtension.toByref (NativePtr.add buffer index)

    member __.Count = count

    interface IDisposable with

        member __.Dispose() =
            Marshal.FreeHGlobal(NativePtr.toNativeInt buffer)