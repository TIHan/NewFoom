﻿[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickler
open Foom.Wad.Unpickle
open System
open FSharp.NativeInterop
open System
open System.Runtime.CompilerServices

#nowarn "9"

[<NoEquality;NoComparison>]
type MusHeader =
    {
        Length: int
        Offset: int
        PrimaryChannelCount: int
        SecondaryChannelCount: int
        InstrumentCount: int
        InstrumentPatches: int []
    }

type MusControllerType =
    | BankSelect = 1uy
    | BankSelect2 = 32uy
    | Modulation = 2uy
    | Volume = 3uy
    | Pan = 4uy
    | Expression = 5uy
    | ReverbDepth = 6uy
    | ChorusDepth = 7uy
    | SustainPedalHold = 8uy
    | SoftPedal = 9uy
    | AllSoundsOff = 10uy
    | AllNotesFadeOff = 11uy
    | Mono = 12uy
    | Poly = 13uy
    | ResetAllControllersOnChannel = 14uy
    | ChangeInstrumentEvent = 0uy

type MidiControllerType =
    | BankSelect = 0uy
    | Modulation = 1uy
    | Volume = 7uy
    | Pan = 10uy
    | Expression = 11uy
    | ReverbDepth = 91uy
    | ChorusDepth = 93uy
    | SustainPedalHold = 64uy
    | SoftPedal = 67uy
    | AllSoundsOff = 120uy
    | AllNotesFadeOff = 123uy
    | Mono = 126uy
    | Poly = 127uy
    | ResetAllControllersOnChannel = 121uy

type MusEventType =
    | ReleaseNote = 0uy
    | PlayNote = 1uy
    | PitchBend = 2uy
    | System = 3uy
    | Controller = 4uy
    | EndOfMeasure = 5uy
    | Finish = 6uy
    | Unused = 7uy

type MidiEventType =
    | NoteOff = 0x80uy
    | NoteOn = 0x90uy
    | Controller = 0xB0uy
    | InstrumentChange = 0xC0uy
    | ChannelPressure = 0xD0uy
    | PitchBend = 0xE0uy
    | SystemReset = 0xFFuy
    | Invalid = 254uy

type MidiMetaEventType =
    | EndOfTrack = 0x2Fuy

let musControllerTypeToMidiControllerType musCtrlTy =
    match musCtrlTy with
    | MusControllerType.BankSelect -> MidiControllerType.BankSelect
    | MusControllerType.Modulation -> MidiControllerType.Modulation
    | MusControllerType.Volume -> MidiControllerType.Volume
    | MusControllerType.Pan -> MidiControllerType.Pan
    | MusControllerType.Expression -> MidiControllerType.Expression
    | MusControllerType.ReverbDepth -> MidiControllerType.ReverbDepth
    | MusControllerType.ChorusDepth -> MidiControllerType.ChorusDepth
    | MusControllerType.SustainPedalHold -> MidiControllerType.SustainPedalHold
    | MusControllerType.SoftPedal -> MidiControllerType.SoftPedal
    | _ -> failwithf "Invalid controller: %A" musCtrlTy

let systemEventToMidiControllerType sysEvent =
    match sysEvent with
    | MusControllerType.AllSoundsOff -> MidiControllerType.AllSoundsOff
    | MusControllerType.AllNotesFadeOff -> MidiControllerType.AllNotesFadeOff
    | MusControllerType.Mono -> MidiControllerType.Mono
    | MusControllerType.Poly -> MidiControllerType.Poly
    | MusControllerType.ResetAllControllersOnChannel -> MidiControllerType.ResetAllControllersOnChannel
    | _ -> musControllerTypeToMidiControllerType sysEvent

type BinaryWriter with

    [<RequiresExplicitTypeArguments>]
    member inline this.WriteBE<'T when 'T : unmanaged>(value: 'T) =
        let stack = Span<byte>(Unsafe.AsPointer(&Unsafe.AsRef &value), sizeof<'T>)
        stack.Reverse()
        this.Write(Span.op_Implicit stack)

let writeMidiEventValue (midiEventTypeValue: byte) (midiChannelValue: byte) (writer: BinaryWriter) =
    writer.Write (midiEventTypeValue ||| midiChannelValue)

let writeMidiEvent (deltaTime: uint32) (midiEventTypeValue: byte) (midiChannelValue: byte) (parm1: byte[] voption) (parm2: byte[] voption) (writer: BinaryWriter) =
    let writeDeltaTimeByte i =
        if i = 0 then
            byte ((deltaTime >>> (7 * i))) &&& 127uy
            |> writer.Write
        else
            let value = byte ((deltaTime >>> (7 * i))) &&& 127uy
            if value <> 0uy then
                writer.Write(value ||| 0x80uy)
    writeDeltaTimeByte 4
    writeDeltaTimeByte 3
    writeDeltaTimeByte 2
    writeDeltaTimeByte 1
    writeDeltaTimeByte 0
    writeMidiEventValue midiEventTypeValue midiChannelValue writer
    parm1 |> ValueOption.iter writer.Write
    parm2 |> ValueOption.iter writer.Write

let writeMidiHeader formatType trackCount ticksPerQuarterNote (writer: BinaryWriter) =
    [|byte 'M';byte 'T';byte 'h';byte 'd'|] |> Array.iter writer.Write
    writer.WriteBE<uint32> 6u
    writer.WriteBE<uint16> formatType
    writer.WriteBE<uint16> trackCount
    writer.WriteBE<uint16> ticksPerQuarterNote

let writeMidiTrack (trackEventData: byte[]) (writer: BinaryWriter) =
    [|byte 'M';byte 'T';byte 'r';byte 'k'|] |> Array.iter writer.Write
    writer.WriteBE<uint32> (uint32 trackEventData.Length)
    trackEventData |> Array.iter writer.Write

let pHeader lumpHeader =
    goToLump lumpHeader (
        (
            u_pipe7 
                (u_string 4)
                u_uint16
                u_uint16
                u_uint16
                u_uint16
                u_uint16
                u_uint16
            <| fun id length offset primaryChannelCount secondaryChannelCount instrumentCount _reserved ->
                let isIdMus = id.StartsWith("mus", StringComparison.OrdinalIgnoreCase)
                if not isIdMus then
                    failwith "Not a MUS lump."
                    
                {
                    Length = int length
                    Offset = int offset
                    PrimaryChannelCount = int primaryChannelCount
                    SecondaryChannelCount = int secondaryChannelCount
                    InstrumentCount = int instrumentCount
                    InstrumentPatches = [||]
                }
        )
        >>= fun header ->
            u_array header.InstrumentCount u_uint16 
            |>> fun instruments -> 
                { header with InstrumentPatches = instruments |> Array.map int }
    )

// http://www.shikadi.net/moddingwiki/MUS_Format
let pBody offset musHeader (writer: BinaryWriter) =
    u_lookAhead (
        u_skipBytes (int64 musHeader.Offset + int64 offset) >>.
        fun stream ->
            let channelCount = byte (musHeader.PrimaryChannelCount + musHeader.SecondaryChannelCount)
            let mutable length = musHeader.Length
            let mutable prev = stream.Position
            let musChannelVolumes = System.Collections.Generic.Dictionary()
            let mutable nextTime = 0u
            while length > 0 do
                let eventDescr = stream.ReadByte()

                let last = (eventDescr) >>> 7
                let eventType = LanguagePrimitives.EnumOfValue(byte ((eventDescr) <<< 1) >>> 5)
                let musChannel = byte ((eventDescr <<< 4) >>> 4)
                if not (musChannelVolumes.ContainsKey musChannel) then
                    musChannelVolumes.[musChannel] <- 127uy

                let midiEventType, midiParm1, midiParm2 =
                    match eventType with
                    | MusEventType.ReleaseNote ->
                        let b = stream.ReadByte()
                        let noteNumber = (b <<< 1) >>> 1
                        MidiEventType.NoteOff, ValueSome [|noteNumber|], ValueSome [|0uy|]
                    | MusEventType.PlayNote ->
                        let b = stream.ReadByte()
                        let volFlag = b >>> 7
                        let noteNumber = (b <<< 1) >>> 1
                        let volume =
                            if volFlag <> 0uy then
                                let b = stream.ReadByte()
                                let volume = (b <<< 1) >>> 1
                                let volume = if volume > 127uy then 127uy else volume
                                musChannelVolumes.[musChannel] <- volume
                                volume
                            else
                                musChannelVolumes.[musChannel]
                        MidiEventType.NoteOn, ValueSome [|noteNumber|], ValueSome [|volume|]
                    | MusEventType.PitchBend ->
                        let n = uint16 (stream.ReadByte()) * 64us
                        MidiEventType.PitchBend, ValueSome([|byte ((n &&& 127us))|]), ValueSome([|byte ((n >>> 7) &&& 127us)|])
                    | MusEventType.System ->
                        let b = stream.ReadByte()
                        let midiCtrlTy = (b <<< 1) >>> 1 |> LanguagePrimitives.EnumOfValue |> systemEventToMidiControllerType
                        MidiEventType.Controller, ValueSome([|byte midiCtrlTy|]), ValueSome [|0uy|]
                    | MusEventType.Controller ->
                        let b = stream.ReadByte()
                        let musCtrlTy = (b <<< 1) >>> 1 |> LanguagePrimitives.EnumOfValue
                        let b = stream.ReadByte()
                        let value = (b <<< 1) >>> 1
                        if musCtrlTy = MusControllerType.ChangeInstrumentEvent then
                            if musHeader.InstrumentPatches |> Array.exists (fun x -> byte x = value) then
                                MidiEventType.InstrumentChange, ValueSome [|value|], ValueNone
                            else
                                MidiEventType.Invalid, ValueNone, ValueNone
                        else
                            let midiCtrlTy = musControllerTypeToMidiControllerType musCtrlTy
                            MidiEventType.Controller, ValueSome([|byte midiCtrlTy|]), ValueSome [|value|]
                    | MusEventType.EndOfMeasure ->
                        MidiEventType.Invalid, ValueNone, ValueNone
                    | MusEventType.Finish ->

                        use ms = new MemoryStream()
                        use seWriter = new BinaryWriter(ms)

                        seWriter.Write(byte MidiMetaEventType.EndOfTrack)
                        seWriter.Write 0uy

                        let bytes = ms.ToArray()

                        MidiEventType.SystemReset, ValueSome bytes, ValueNone
                    | MusEventType.Unused ->
                        MidiEventType.Invalid, ValueNone, ValueNone
                    | _ ->
                        failwithf "Invalid mus event: %A" eventType

                let deltaTime =
                    if last <> 0uy then
                        let mutable time = 0u
                        let mutable stop = false
                        while not stop do
                            let b = stream.ReadByte()
                            if b &&& 0x80uy = 0uy then
                                stop <- true
                            time <- time * 128u + uint32 ((b <<< 1) >>> 1)
                        time
                    else
                        0u

                length <- length - int (stream.Position - prev)
                prev <- stream.Position

                let channel =
                    if musChannel = 15uy then 9uy
                    elif musChannel >= 9uy then musChannel + 1uy
                    else musChannel

                if midiEventType <> MidiEventType.Invalid then
                    writeMidiEvent nextTime (byte midiEventType) channel midiParm1 midiParm2 writer

                nextTime <- deltaTime

            writer.BaseStream.Position <- 0L
            let res = (new BinaryReader(writer.BaseStream)).ReadBytes(int writer.BaseStream.Length)
            res
    )

let parse lumpHeader writer =
    pHeader lumpHeader >>= fun musHeader ->
        pBody lumpHeader.Offset musHeader writer

let Parse lumpHeader (stream: Stream) =
    let ms = new MemoryStream()
    let writer = new BinaryWriter(ms, Text.Encoding.UTF8, leaveOpen=false)
    let bytes = u_run (parse lumpHeader writer) (ReadStream stream)
    writer.Dispose()
    let ms = new MemoryStream()
    let writer = new BinaryWriter(ms, Text.Encoding.UTF8, leaveOpen=false)
    writeMidiHeader 0us 1us 70us writer
    writeMidiTrack bytes writer
    let bytes = ms.ToArray()
    writer.Dispose()
    bytes