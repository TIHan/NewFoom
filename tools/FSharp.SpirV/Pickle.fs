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

let LiteralStrings: SPVPickle<LiteralStrings> =
    {
        read = (fun stream ->
            let ress = ResizeArray ()
            while stream.remaining > 0 do
                let mutable res = Unchecked.defaultof<string>
                stream.String &res
                ress.Add res
            ress |> List.ofSeq
        )
        write = (fun stream ress ->
            for res in ress do
                let mutable res = res
                stream.String &res
        )
    }

let LiteralNumber = Ids

let LiteralNumberLimitOne: SPVPickle<LiteralNumberLimitOne> = Word

let Literal: SPVPickle<Literal> = Ids

let Literals: SPVPickle<Literals> = Ids

let p0 (instr: SPVInstruction) : SPVPickle<SPVInstruction> =
    {
        read = (fun _ -> instr)
        write = (fun _ _ -> ())
    }

let p1 (f: ('Arg1 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (g: SPVInstruction -> 'Arg1) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream)
        )
        write = (fun stream res ->
            let arg1 = g res
            v1.write stream arg1
        )
    }

let p2 (f: ('Arg1 * 'Arg2 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (v2: SPVPickle<'Arg2>) (g: SPVInstruction -> 'Arg1 * 'Arg2) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream, v2.read stream)
        )
        write = (fun stream res ->
            let (arg1, arg2) = g res
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
            let (arg1, arg2, arg3) = g res
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
            let (arg1, arg2, arg3, arg4) = g res
            v1.write stream arg1
            v2.write stream arg2
            v3.write stream arg3
            v4.write stream arg4
        )
    }

let p5 (f: ('Arg1 * 'Arg2 * 'Arg3 * 'Arg4 * 'Arg5 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (v2: SPVPickle<'Arg2>) (v3: SPVPickle<'Arg3>) (v4: SPVPickle<'Arg4>) (v5: SPVPickle<'Arg5>) (g: SPVInstruction -> 'Arg1 * 'Arg2 * 'Arg3 * 'Arg4 * 'Arg5) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream, v2.read stream, v3.read stream, v4.read stream, v5.read stream)
        )
        write = (fun stream res ->
            let (arg1, arg2, arg3, arg4, arg5) = g res
            v1.write stream arg1
            v2.write stream arg2
            v3.write stream arg3
            v4.write stream arg4
            v5.write stream arg5

        )
    }

let p9 (f: ('Arg1 * 'Arg2 * 'Arg3 * 'Arg4 * 'Arg5 * 'Arg6 * 'Arg7 * 'Arg8 * 'Arg9 -> SPVInstruction)) (v1: SPVPickle<'Arg1>) (v2: SPVPickle<'Arg2>) (v3: SPVPickle<'Arg3>) (v4: SPVPickle<'Arg4>) (v5: SPVPickle<'Arg5>) (v6: SPVPickle<'Arg6>) (v7: SPVPickle<'Arg7>) (v8: SPVPickle<'Arg8>) (v9: SPVPickle<'Arg9>) (g: SPVInstruction -> 'Arg1 * 'Arg2 * 'Arg3 * 'Arg4 * 'Arg5 * 'Arg6 * 'Arg7 * 'Arg8 * 'Arg9) : SPVPickle<SPVInstruction> =
    {
        read = (fun stream ->
            f (v1.read stream, v2.read stream, v3.read stream, v4.read stream, v5.read stream, v6.read stream, v7.read stream, v8.read stream, v9.read stream)
        )
        write = (fun stream res ->
            let (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) = g res
            v1.write stream arg1
            v2.write stream arg2
            v3.write stream arg3
            v4.write stream arg4
            v5.write stream arg5
            v6.write stream arg6
            v7.write stream arg7
            v8.write stream arg8
            v9.write stream arg9
        )
    }

module Instructions =

    type private IMarker = interface end

    // Miscellaneous Instructions
        
    let OpNop =                  p0 OpNop
    let OpUndef =                p2 OpUndef Id ResultId                                                                   (function OpUndef (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid") 
    let OpSizeOf =               p3 OpSizeOf Id ResultId Id                                                               (function OpSizeOf (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    
    // Debug Instructions
    
    let OpSourceContinued =      p1 OpSourceContinued LiteralString                                                       (function OpSourceContinued arg1 -> arg1 | _ -> failwith "invalid")
    let OpSource =               p4 OpSource Enum<SourceLanguage> LiteralNumberLimitOne (Opt Id) (Opt LiteralString)      (function OpSource (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpSourceExtension =      p1 OpSourceExtension LiteralString                                                       (function OpSourceExtension arg1 -> arg1 | _ -> failwith "invalid")
    let OpName =                 p2 OpName Id LiteralString                                                               (function OpName (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpString =               p2 OpString ResultId LiteralString                                                       (function OpString (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpLine =                 p3 OpLine Id LiteralNumberLimitOne LiteralNumberLimitOne                                 (function OpLine (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpNoLine =               p0 OpNoLine                                                                              
    let OpModuleProcessed =      p1 OpModuleProcessed LiteralString                                                       (function OpModuleProcessed arg1 -> arg1 | _ -> failwith "invalid")
    
    // Annotation Instructions
    
    let OpDecorate =             p3 OpDecorate Id Enum<Decoration> Literal                                                (function OpDecorate (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpMemberDecorate =       p4 OpMemberDecorate Id LiteralNumber Enum<Decoration> Literal                            (function OpMemberDecorate (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpDecorationGroup =      p1 OpDecorationGroup ResultId                                                            (function OpDecorationGroup arg1 -> arg1 | _ -> failwith "invalid")
    let OpGroupDecorate =        p2 OpGroupDecorate Id Ids                                                                (function OpGroupDecorate (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpGroupMemberDecorate =  p2 OpGroupMemberDecorate Id Ids                                                          (function OpGroupMemberDecorate (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpDecorateId =           p3 OpDecorateId Id Enum<Decoration> Ids                                                  (function OpDecorateId (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpDecorateString =       p4 OpDecorateString Id Enum<Decoration> LiteralString LiteralStrings                     (function OpDecorateString (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpMemberDecorateString = p5 OpMemberDecorateString Id LiteralNumber Enum<Decoration> LiteralString LiteralStrings (function OpMemberDecorateString (arg1, arg2, arg3, arg4, arg5) -> (arg1, arg2, arg3, arg4, arg5) | _ -> failwith "invalid")
    
    // Extension Instructions
    
    let OpExtension =            p1 OpExtension LiteralString                                                             (function OpExtension arg1 -> arg1 | _ -> failwith "invalid")
    let OpExtInstImport =        p2 OpExtInstImport ResultId LiteralString                                                (function OpExtInstImport (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpExtInst =              p5 OpExtInst Id ResultId Id LiteralNumber Ids                                            (function OpExtInst (arg1, arg2, arg3, arg4, arg5) -> (arg1, arg2, arg3, arg4, arg5) | _ -> failwith "invalid")
    
    // Mode-Settings Instructions
    
    let OpMemoryModel =          p2 OpMemoryModel Enum<AddressingModel> Enum<MemoryModel>                                 (function OpMemoryModel (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpEntryPoint =           p4 OpEntryPoint Enum<ExecutionModel> Id LiteralString Ids                                (function OpEntryPoint (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpExecutionMode =        p3 OpExecutionMode Id Enum<ExecutionMode> LiteralNumber                                  (function OpExecutionMode (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpCapability =           p1 OpCapability Enum<Capability>                                                         (function OpCapability arg1 -> arg1 | _ -> failwith "invalid")
    let OpExecutionModeId =      p3 OpExecutionModeId Id Enum<ExecutionMode> Ids                                          (function OpExecutionModeId (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")

    // Type-Declaration Instructions
    
    let OpTypeVoid =             p1 OpTypeVoid ResultId                                                                                                                                                     (function OpTypeVoid arg1 -> arg1 | _ -> failwith "invalid")
    let OpTypeBool =             p1 OpTypeBool ResultId                                                                                                                                                     (function OpTypeBool arg1 -> arg1 | _ -> failwith "invalid")
    let OpTypeInt =              p3 OpTypeInt ResultId LiteralNumberLimitOne LiteralNumber                                                                                                                  (function OpTypeInt (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpTypeFloat =            p2 OpTypeFloat ResultId LiteralNumber                                                                                                                                      (function OpTypeFloat (arg1, arg2) -> (arg1, arg2) | _ -> failwith "invalid")
    let OpTypeVector =           p3 OpTypeVector ResultId Id LiteralNumber                                                                                                                                  (function OpTypeVector (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpTypeMatrix =           p3 OpTypeMatrix ResultId Id LiteralNumber                                                                                                                                  (function OpTypeMatrix (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpTypeImage =            p9 OpTypeImage ResultId Id Enum<Dim> LiteralNumberLimitOne LiteralNumberLimitOne LiteralNumberLimitOne LiteralNumberLimitOne Enum<ImageFormat> (Opt Enum<AccessQualifier>) (function OpTypeImage (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) -> (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9) | _ -> failwith "invalid")
    let OpTypeSampler =          p1 OpTypeSampler ResultId                                                                                                                                                  (function OpTypeSampler arg1 -> arg1 | _ -> failwith "invalid")
    
    let OpTypePointer =          p3 OpTypePointer ResultId Enum<StorageClass> Id (function OpTypePointer (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpTypeFunction =         p3 OpTypeFunction ResultId Id Ids (function OpTypeFunction (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")

    // Constant-Create Instructions

    let OpConstant = p3 OpConstant Id ResultId Literal (function OpConstant (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")

    // Memory Instructions

    let OpVariable = p4 OpVariable Id ResultId Enum<StorageClass> (Opt Id) (function OpVariable (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpLoad =     p4 OpLoad Id ResultId Id (Opt Enum<MemoryAccessMask>) (function OpLoad (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpStore =    p3 OpStore Id Id (Opt Enum<MemoryAccessMask>) (function OpStore (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")

    // Function Instructions

    let OpFunction =    p4 OpFunction Id ResultId Enum<FunctionControlMask> Id (function OpFunction (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")
    let OpFunctionEnd = p0 OpFunctionEnd

    // Composite Instructions

    let OpCompositeConstruct = p3 OpCompositeConstruct Id ResultId Ids (function OpCompositeConstruct (arg1, arg2, arg3) -> (arg1, arg2, arg3) | _ -> failwith "invalid")
    let OpCompositeExtract = p4 OpCompositeExtract Id ResultId Id Literals (function OpCompositeExtract (arg1, arg2, arg3, arg4) -> (arg1, arg2, arg3, arg4) | _ -> failwith "invalid")

    // Relational and Logical Instructions

    let OpReturn = p0 OpReturn

    // Control-Flow Instructions

    let OpLabel = p1 OpLabel ResultId (function OpLabel arg1 -> arg1 | _ -> failwith "invalid")

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
