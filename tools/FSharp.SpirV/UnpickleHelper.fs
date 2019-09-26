module internal FSharp.SpirV.UnpickleHelper

open System.IO
open System.Text
open System.Collections.Generic
open Microsoft.FSharp.NativeInterop

#nowarn "9"
#nowarn "51"

type StringKind =
    | BigEndianUnicode
    | Unicode
    | UTF8

[<Sealed>]
type ReadStream =

    val mutable private bytes : byte []
    val mutable private position : int
    val private stream : Stream option

    new(bytes) = { bytes = bytes; position = 0; stream = None }

    new(stream) = { bytes = Array.empty; position = -1; stream = Some stream }

    member this.Length =
        match this.stream with
        | None -> int64 this.bytes.Length
        | Some stream -> stream.Length

    member this.Position =
        match this.stream with
        | None -> int64 this.position
        | Some stream -> stream.Position

    member this.Seek offset =
        match this.stream with
        | None -> this.position <- int offset
        | Some stream -> stream.Position <- offset 

    member this.Skip n =
        match this.stream with
        | None -> this.position <- this.position + int n
        | Some stream -> stream.Position <- stream.Position + n

    member this.ReadByte() =
        match this.stream with
        | None ->
            let result = this.bytes.[int this.position]
            this.position <- this.position + 1
            result
        | Some stream ->
            match stream.ReadByte() with
            | -1 -> failwith "Unable to read byte from stream."
            | result -> byte result

    member this.ReadBytes n =
        match this.stream with
        | None ->
            let i = this.position
            this.position <- this.position + n
            this.bytes.[int i..int this.position]
        | Some stream ->
            let mutable bytes = Array.zeroCreate<byte> n
            stream.Read(bytes, 0, n) |> ignore
            bytes

    member this.ReadString n =
        match this.stream with
        | None ->
            let enc = System.Text.UTF8Encoding(true, true)
            let result = enc.GetString(this.bytes, this.position, n)
            this.position <- this.position + n
            result
        | Some stream ->
            let mutable bytes = Array.zeroCreate<byte> n
            stream.Read(bytes, 0, n) |> ignore
            let enc = System.Text.UTF8Encoding(true, true)
            enc.GetString(bytes, 0, n)

    member this.Read<'T when 'T : unmanaged>() =
        match this.stream with
        | None ->
            let result = NativePtr.read(NativePtr.ofNativeInt<'T> <| NativePtr.toNativeInt &&this.bytes.[int this.position])
            this.position <- this.position + sizeof<'T>
            result
        | Some stream ->
            let n = sizeof<'T>
            let mutable bytes = Array.zeroCreate<byte> n 
            stream.Read(bytes, 0, n) |> ignore
            NativePtr.read(NativePtr.ofNativeInt<'T> <| NativePtr.toNativeInt &&bytes.[0])

type Unpickle<'a> = ReadStream -> 'a

let u_byte : Unpickle<byte> =
    fun stream -> stream.ReadByte()

let u_bytes n : Unpickle<byte []> =
    fun stream -> stream.ReadBytes n

let u_int16 : Unpickle<int16> =
    fun stream -> stream.Read()

let u_uint16 : Unpickle<uint16> =
    fun stream -> stream.Read()

let u_int32 : Unpickle<int> =
    fun stream -> stream.Read()

let u_uint32 : Unpickle<uint32> =
    fun stream -> stream.Read()

let u_single : Unpickle<single> =
    fun stream -> stream.Read()

let u_int64 : Unpickle<int64> =
    fun stream -> stream.Read()

let u_uint64 : Unpickle<uint64> =
    fun stream -> stream.Read()

let inline u_string n : Unpickle<string> =
    fun stream -> stream.ReadString n

let inline u_pipe2 a b f : Unpickle<_> =
    fun stream -> f (a stream) (b stream)

let inline u_pipe3 a b c f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream)

let inline u_pipe4 a b c d f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream)

let inline u_pipe5 a b c d e f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream)

let inline u_pipe6 a b c d e g f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream)

let inline u_pipe7 a b c d e g h f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream)

let inline u_pipe8 a b c d e g h i f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream) (i stream)

let inline u_pipe9 a b c d e g h i j f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream) (i stream) (j stream)

let inline u_pipe10 a b c d e g h i j k f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream) (i stream) (j stream) (k stream)

let inline u_pipe11 a b c d e g h i j k l f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream) (i stream) (j stream) (k stream) (l stream)

let inline u_pipe12 a b c d e g h i j k l m f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream) (i stream) (j stream) (k stream) (l stream) (m stream)

let inline u_pipe13 a b c d e g h i j k l m n f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) (d stream) (e stream) (g stream) (h stream) (i stream) (j stream) (k stream) (l stream) (m stream) (n stream)

let inline u_bpipe2 (a: Unpickle<'a>) (b: Unpickle<'b>) (f: 'a -> 'b -> Unpickle<'c>) : Unpickle<_> =
    fun stream -> f (a stream) (b stream) stream

let inline u_bpipe3 a b c f : Unpickle<_> =
    fun stream -> f (a stream) (b stream) (c stream) stream

let inline u<'a when 'a : unmanaged> : Unpickle<_> =
    fun stream -> stream.Read<'a>()

let inline u_array n (p: Unpickle<'a>) =
    fun stream ->
        match n with
        | 0 -> [||]
        | _ -> Array.init n (fun _ -> p stream)

let inline u_list n (p: Unpickle<'a>) =
    fun stream ->
        match n with
        | 0 -> []
        | _ -> List.init n (fun _ -> p stream)

let inline u_dictionary n f (p: Unpickle<'a>) =
    fun stream ->
        match n with
        | 0 -> Dictionary ()
        | _ -> 
            let dict = Dictionary ()
            for i = 0 to n - 1 do
                f dict (p stream)
            dict

let inline u_skipBytes n : Unpickle<_> =
    fun stream -> stream.Skip n

let inline u_lookAhead (p: Unpickle<'a>) : Unpickle<'a> =
    fun stream ->
        let prevPosition = stream.Position
        let result = p stream
        stream.Seek prevPosition
        result

let u_streamPosition: Unpickle<int64> =
    fun stream -> stream.Position

// fmap
let inline (|>>) (u: Unpickle<'a>) (f: 'a -> 'b) : Unpickle<'b> =
    fun stream -> f (u stream)

let inline (<*>) (u1: Unpickle<'a -> 'b>) (u2: Unpickle<'a>) : Unpickle<'b> =
    fun stream -> u1 stream (u2 stream)

let inline (>>=) (u: Unpickle<'a>) (f: 'a -> Unpickle<'b>) : Unpickle<'b> =
    fun stream -> f (u stream) stream

let inline (>>.) (u1: Unpickle<'a>) (u2: Unpickle<'b>) =
    fun stream ->
        u1 stream |> ignore
        u2 stream

let inline (.>>) (u1: Unpickle<'a>) (u2: Unpickle<'b>) =
    fun stream ->
        let result = u1 stream
        u2 stream |> ignore
        result

let inline (.>>.) (u1: Unpickle<'a>) (u2: Unpickle<'b>) =
    fun stream ->
        u1 stream,
        u2 stream

let inline u_return x : Unpickle<_> = 
    fun stream -> x

let inline u_run (p: Unpickle<_>) x = p x
