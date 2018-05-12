namespace Foom.NativeCollections

open System
open System.Collections.Generic
open System.Runtime.InteropServices
open FSharp.NativeInterop

#nowarn "9"
#nowarn "42"

module NativePtrExtension =
    [<NoDynamicInvocation>]
    let inline toByref (x: nativeptr<'T>) = (# "" x : 'T byref  #)

[<AutoOpen>]
module private NativeArrayHelpers =

    let inline assertLengthNonNegative length =
        if length < 0 then failwithf "Length cannot be negative. Length: %i" length

[<Sealed;AllowNullLiteral>]
type NativeArray<'T when 'T : unmanaged> =

    val Buffer : nativeptr<'T>
    val Length : int

    new (length) =
        assertLengthNonNegative length
        let size = sizeof<'T> * length
        {
            Buffer = 
                Marshal.AllocHGlobal(size) 
                |> NativePtr.ofNativeInt<'T>
            Length = length
        }

    new (narr: NativeArray<'T>, length) =
        if narr = null then nullArg "narr"
        assertLengthNonNegative length
        {
            Buffer = 
                Marshal.ReAllocHGlobal(NativePtr.toNativeInt narr.Buffer, nativeint length) 
                |> NativePtr.ofNativeInt<'T>
            Length = length
        }


    member inline this.Item
        with get index = NativePtr.read (NativePtr.add this.Buffer index)
        and set index value = NativePtr.set this.Buffer index value

    member inline this.GetByRef(index) =
        NativePtrExtension.toByref (NativePtr.add this.Buffer index)

    member inline this.ToSpan() =
        Span<'T>((this.Buffer |> NativePtr.toNativeInt).ToPointer(), this.Length * sizeof<'T>)

    interface IDisposable with

        member this.Dispose() =
            Marshal.FreeHGlobal(NativePtr.toNativeInt this.Buffer)

module NativeArray =

    let inline zeroCreate count =
        new NativeArray<_>(count)

    let inline init count f =
        let arr = new NativeArray<_>(count)

        for i = 0 to arr.Length - 1 do
            arr.[i] <- f i

        arr

    let inline iter f (arr: NativeArray<_>) =
        for i = 0 to arr.Length - 1 do
            f arr.[i]

    let inline iteri f (arr: NativeArray<_>) =
        for i = 0 to arr.Length - 1 do
            f i arr.[i]

    let inline resize length (narr: NativeArray<_>) =
        new NativeArray<_>(narr, length)
