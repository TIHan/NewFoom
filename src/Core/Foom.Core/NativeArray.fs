namespace Foom.NativeCollections

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop

#nowarn "9"
#nowarn "42"

[<AutoOpen>]
module private NativeArrayHelpers =

    let inline assertLengthNonNegative length =
        if length < 0 then failwithf "Length cannot be negative. Length: %i" length

    let inline fastAdd<'T when 'T : unmanaged> (address: nativeptr<'T>) (offset: int32) = (# "add" address offset: nativeptr<'T> #)

[<Struct>]
type NativeArray<'T when 'T : unmanaged> =

    val Buffer : nativeptr<'T>
    val Length : int

    new (length) =
        assertLengthNonNegative length
        let size = sizeof<'T> * length
        let buffer =
            if size = 0 then IntPtr.Zero |> NativePtr.ofNativeInt<'T>
            else
                Marshal.AllocHGlobal(size) 
                |> NativePtr.ofNativeInt<'T>
        {
            Buffer = buffer
            Length = length
        }

    new (narr: NativeArray<'T>, length) =
        if narr.Buffer |> NativePtr.toNativeInt = IntPtr.Zero then failwith "Buffer is zero."
        assertLengthNonNegative length
        {
            Buffer = 
                Marshal.ReAllocHGlobal(NativePtr.toNativeInt narr.Buffer, nativeint length) 
                |> NativePtr.ofNativeInt<'T>
            Length = length
        }

    member this.Item
        with get index = 
            //&Unsafe.Add<'T>(&Unsafe.AsRef<'T>(NativePtr.toVoidPtr this.Buffer), int index)
            NativePtr.toByRef (NativePtr.add this.Buffer index)

    member inline this.ToSpan() =
        Span<'T>((this.Buffer |> NativePtr.toNativeInt).ToPointer(), this.Length * sizeof<'T>)

    member inline this.ToPointer() =
        (this.Buffer |> NativePtr.toNativeInt).ToPointer()

    interface IDisposable with

        member this.Dispose() =
            Marshal.FreeHGlobal(NativePtr.toNativeInt this.Buffer)
