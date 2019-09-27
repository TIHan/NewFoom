module internal FSharp.SpirV.Pickle

open System
open System.IO
open FSharp.SpirV.Specification

[<RequireQualifiedAccess>]
module LittleEndian =

    let inline write8 (data: Span<byte>) offset value =
        data.[offset] <- byte value

    let inline write16 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)

    let inline write32 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)
        data.[offset + 2] <- byte (value >>> 16)
        data.[offset + 3] <- byte (value >>> 24)

    let inline write64 (data: Span<byte>) offset value =
        data.[offset] <- byte value
        data.[offset + 1] <- byte (value >>> 8)
        data.[offset + 2] <- byte (value >>> 16)
        data.[offset + 3] <- byte (value >>> 24)
        data.[offset + 4] <- byte (value >>> 32)
        data.[offset + 5] <- byte (value >>> 40)
        data.[offset + 6] <- byte (value >>> 48)
        data.[offset + 7] <- byte (value >>> 56)

    let inline read8 (data: ReadOnlySpan<byte>) offset =
        data.[offset]

    let inline read16 (data: ReadOnlySpan<byte>) offset =
        ( uint16 data.[offset]) |||
        ((uint16 data.[offset + 1]) <<< 8)

    let inline read32 (data: ReadOnlySpan<byte>) offset =
        ( uint32 data.[offset]) |||
        ((uint32 data.[offset + 1]) <<< 8) |||
        ((uint32 data.[offset + 2]) <<< 16) |||
        ((uint32 data.[offset + 3]) <<< 24)

    let inline read64 (data: ReadOnlySpan<byte>) offset =
        ( uint64 data.[offset]) |||
        ((uint64 data.[offset + 1]) <<< 8) |||
        ((uint64 data.[offset + 2]) <<< 16) |||
        ((uint64 data.[offset + 3]) <<< 24) |||
        ((uint64 data.[offset + 4]) <<< 32) |||
        ((uint64 data.[offset + 5]) <<< 40) |||
        ((uint64 data.[offset + 6]) <<< 48) |||
        ((uint64 data.[offset + 7]) <<< 56)

type SPVPickleStream =
    {
        stream: Stream
        mutable remaining: int
        buffer128: byte []
        isReader: bool
    }

    member inline x.ReadOnlyBuffer len = ReadOnlySpan(x.buffer128, 0, len)

    member inline x.Buffer len = Span(x.buffer128, 0, len)

    member inline x.Position = int x.stream.Position

    member inline x.Seek (offset, origin) = x.stream.Seek (int64 offset, origin) |> ignore

    member x.Int (res: byref<int>) =
        if x.isReader then
            let buf = x.Buffer 4
            x.stream.Read buf |> ignore
            res <- int (LittleEndian.read32 (Span.op_Implicit buf) 0)
        else
            let buf = x.Buffer 4
            LittleEndian.write32 buf 0 res
            x.stream.Write (Span.op_Implicit buf)

    member x.UInt16 (res: byref<uint16>) =
        if x.isReader then
            let buf = x.Buffer 2
            x.stream.Read buf |> ignore
            res <- LittleEndian.read16 (Span.op_Implicit buf) 0
        else
            let buf = x.Buffer 2
            LittleEndian.write16 buf 0 res
            x.stream.Write (Span.op_Implicit buf)

    member x.UInt32 (res: byref<uint32>) =
        if x.isReader then
            let buf = x.Buffer 4
            x.stream.Read buf |> ignore
            res <- LittleEndian.read32 (Span.op_Implicit buf) 0
            if x.remaining > 0 then
                x.remaining <- x.remaining - 1
        else
            let buf = x.Buffer 4
            LittleEndian.write32 buf 0 res
            x.stream.Write (Span.op_Implicit buf)

    member x.String (res: byref<LiteralString>) =
        if x.isReader then
            let startPos = x.Position
            let mutable length = 0

            while not (x.stream.ReadByte() = 0 && length % sizeof<Word> = 0) do
                length <- length + 1

            x.Seek(startPos, SeekOrigin.Begin)

            let bytes = Array.zeroCreate length
            x.stream.Read(bytes, 0, bytes.Length) |> ignore

            // Padding
            x.Seek(4, SeekOrigin.Current)

            res <- Text.UTF8Encoding.UTF8.GetString(bytes)
            let endPos = x.Position

            let wordCount = int (endPos - startPos) / sizeof<Word>
            if x.remaining > 0 then
                x.remaining <- x.remaining - wordCount

        else
            let bytes = Text.UTF8Encoding.UTF8.GetBytes res
            let remainder = bytes.Length % sizeof<Word>

            for i = 0 to remainder - 1 do
                x.buffer128.[i] <- 0uy

            x.buffer128.[remainder + 0] <- 0uy
            x.buffer128.[remainder + 1] <- 0uy
            x.buffer128.[remainder + 2] <- 0uy
            x.buffer128.[remainder + 3] <- 0uy

            x.stream.Write(bytes, 0, bytes.Length)
            x.stream.Write(x.buffer128, 0, remainder + sizeof<Word>)

type SPVPickle<'T> = 
    {
        read: SPVPickleStream -> 'T
        write: SPVPickleStream -> 'T -> unit
    }

let Opt (p: SPVPickle<_>) =
    {
        read = (fun stream ->
            if stream.remaining > 0 then
                Some (p.read stream)
            else
                None
        )
        write = (fun stream resOpt ->
            match resOpt with
            | Some res -> p.write stream res
            | _ -> ()
        )
    }

let OpCode =
    {
        read = (fun stream ->
            let mutable res = 0us
            stream.UInt16 &res
            let value = LanguagePrimitives.EnumOfValue<int, Op> (int res)
            if Enum.GetName (typeof<Op>, value) = null then
                failwith "Invalid op."
            value
        )
        write = (fun stream res ->
            let mutable res = uint16 res
            stream.UInt16 &res
        )
    }

let WordCount =
    {
        read = (fun stream ->
            let mutable res = 0us
            stream.UInt16 &res
            int res
        )
        write = (fun stream res ->
            let mutable res = uint16 res
            stream.UInt16 &res
        )
    }

let Word: SPVPickle<Word> =
    {
        read = (fun stream ->
            let mutable res = 0u
            stream.UInt32 &res
            res
        )
        write = (fun stream res ->
            let mutable res = res
            stream.UInt32 &res
        )
    }

let Words count =
    {
        read = (fun stream ->
            List.init count (fun _ -> Word.read stream)
        )
        write = (fun stream res ->
            res.[..(count - 1)]
            |> List.iter (Word.write stream)
        )
    }

let Id: SPVPickle<id> = Word

let ResultId: SPVPickle<Result_id> = Word

let Ids: SPVPickle<id list> = 
    {
        read = (fun stream ->
            (Words stream.remaining).read stream
        )
        write = (fun stream res ->
            (Words res.Length).write stream res
        )
    }

let Enum<'T when 'T : enum<int>> =
    {
        read = (fun stream ->
            let mutable res = 0
            stream.Int &res
            let value = LanguagePrimitives.EnumOfValue<int, 'T> res
            if Enum.GetName (typeof<'T>, value) = null then
                failwithf "Invalid enum value for %s." typeof<'T>.Name
            if stream.remaining > 0 then
                stream.remaining <- stream.remaining - 1
            value
        )
        write = (fun stream res ->
            let mutable res = LanguagePrimitives.EnumToValue res
            stream.Int &res
        )
    }

let LiteralString: SPVPickle<LiteralString> =
    {
        read = (fun stream ->
            let mutable res = Unchecked.defaultof<string>
            stream.String &res
            res
        )
        write = (fun stream res ->
            let mutable res = res
            stream.String &res
        )
    }

let LiteralNumber = Ids

let LiteralNumberLimitOne: SPVPickle<LiteralNumberLimitOne> = Word

let p1 (f: ('Arg1 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (g: SPVInstruction -> 'Arg1) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream)
        )
        write = (fun stream res ->
            let mutable arg1 = g res
            v1.write stream arg1
        )
    }

let p2 (f: ('Arg1 * 'Arg2 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (v2: SPVPickle<'Arg2>) (g: SPVInstruction -> 'Arg1 * 'Arg2) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream, v2.read stream)
        )
        write = (fun stream res ->
            let mutable (arg1, arg2) = g res
            v1.write stream arg1
            v2.write stream arg2
        )
    }

let p3 (f: ('Arg1 * 'Arg2 * 'Arg3 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (v2: SPVPickle<'Arg2>) (v3: SPVPickle<'Arg3>) (g: SPVInstruction -> 'Arg1 * 'Arg2 * 'Arg3) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream, v2.read stream, v3.read stream)
        )
        write = (fun stream res ->
            let mutable (arg1, arg2, arg3) = g res
            v1.write stream arg1
            v2.write stream arg2
            v3.write stream arg3
        )
    }

let p4 (f: ('Arg1 * 'Arg2 * 'Arg3 * 'Arg4 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (v2: SPVPickle<'Arg2>) (v3: SPVPickle<'Arg3>) (v4: SPVPickle<'Arg4>) (g: SPVInstruction -> 'Arg1 * 'Arg2 * 'Arg3 * 'Arg4) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream, v2.read stream, v3.read stream, v4.read stream)
        )
        write = (fun stream res ->
            let mutable (arg1, arg2, arg3, arg4) = g res
            v1.write stream arg1
            v2.write stream arg2
            v3.write stream arg3
            v4.write stream arg4
        )
    }

module Instructions =

    type private IMarker = interface end
        
    let OpSource =          p4 OpSource Enum<SourceLanguage> LiteralNumberLimitOne (Opt Id) (Opt LiteralString) (function OpSource (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpExtInstImport =   p2 OpExtInstImport ResultId LiteralString                                           (function OpExtInstImport (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpMemoryModel =     p2 OpMemoryModel Enum<AddressingModel> Enum<MemoryModel>                            (function OpMemoryModel (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpEntryPoint =      p4 OpEntryPoint Enum<ExecutionModel> Id LiteralString Ids                           (function OpEntryPoint (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpExecutionMode =   p3 OpExecutionMode Id Enum<ExecutionMode> LiteralNumber                             (function OpExecutionMode (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpExecutionModeId = p3 OpExecutionModeId Id Enum<ExecutionMode> Ids                                     (function OpExecutionModeId (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpCapability =      p1 OpCapability Enum<Capability>                                                    (function OpCapability arg1 -> arg1 | _ -> failwith "invalid")

    let InstructionsType = typeof<IMarker>.DeclaringType

    let tryGetPickle (op: Op) =
        let name = System.Enum.GetName (typeof<Op>, op)
        match InstructionsType.GetProperty (name, Reflection.BindingFlags.NonPublic ||| Reflection.BindingFlags.Static) with
        | null -> None
        | prop -> Some (prop.GetValue null :?> SPVPickle<SPVInstruction>)

    let getPickle (op: Op) =
        match tryGetPickle op with
        | Some x -> x
        | _ -> failwith "Unable to find pickle."

    let getOp (instr: SPVInstruction) =
        let name = instr.GetType().Name
        System.Enum.Parse<Op> name

let Instruction =
    {
        read = (fun stream ->
            let op = OpCode.read stream
            let wordCount = WordCount.read stream
            //let p = Instructions.getPickle op
            //stream.remaining <- wordCount - 1
            //p.read stream
            match Instructions.tryGetPickle op with
            | Some p ->
                stream.remaining <- wordCount - 1
                p.read stream
            | _ ->
                UnhandledOp (op, (Words (wordCount - 1)).read stream)
        )
        write = (fun stream res ->
            let op = Instructions.getOp res
            let p = Instructions.getPickle op

            OpCode.write stream op

            let wordCountPos = stream.Position
            stream.Seek(2, SeekOrigin.Current)

            let startPos = stream.Position
            p.write stream res 
            let endPos = stream.Position

            let length = endPos - startPos
            if length % sizeof<Word> <> 0 then
                failwith "Invalid instruction."

            stream.Seek(wordCountPos, SeekOrigin.Begin)

            let mutable wordCount = uint16 (length / sizeof<Word>)
            stream.UInt16 &wordCount

            stream.Seek(endPos, SeekOrigin.Begin)              
        )
    }

let Instructions =
    {
        read = (fun stream ->
            let xs = ResizeArray()
            while stream.stream.Position < stream.stream.Length do
                xs.Add(Instruction.read stream)
            xs |> List.ofSeq
        )
        write = (fun stream res ->
            res
            |> List.iter (fun instr ->
                Instruction.write stream instr
            )
        )
    }

let Module =
    {
        read = (fun stream ->
            let magicNumber = Word.read stream
            let versionNumber = Word.read stream
            let genMagicNumber = Word.read stream
            let bound = Word.read stream
            let _reserved = Word.read stream

            let instrs = Instructions.read stream

            {
                magicNumber = magicNumber
                versionNumber = versionNumber
                genMagicNumber = genMagicNumber
                bound = bound
                instrs = instrs
            }
        )
        write = (fun stream res ->
            Word.write stream res.magicNumber
            Word.write stream res.versionNumber
            Word.write stream res.genMagicNumber
            Word.write stream res.bound
            Word.write stream (let reserved = 0u in reserved)

            Instructions.write stream res.instrs
        )
    }
