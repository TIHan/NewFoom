[<RequireQualifiedAccess>]
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
    | EndOfTrack = 0x2Fuy
    | Invalid = 255uy

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
        let stack = Span<byte>(Unsafe.AsPointer(&Unsafe.AsRef &value), 4)
        stack.Reverse()
        this.Write(Span.op_Implicit stack)

let writeMidiVariableLength (bytes: byte[]) (writer: BinaryWriter) =
    if bytes.Length = 0 then
        failwith "Data required for variable length."

    bytes
    |> Array.iter writer.Write

let writeMidiEventValue (midiEventTypeValue: byte) (midiChannelValue: byte) (writer: BinaryWriter) =
    writer.Write (midiEventTypeValue ||| midiChannelValue)

let writeMidiParameters (parm1: byte) (parm2: byte) (writer: BinaryWriter) =
    writer.Write parm1
    writer.Write parm2

let writeMidiEvent deltaTime (midiEventTypeValue: byte) (midiChannelValue: byte) (parm1: byte) (parm2: byte) (writer: BinaryWriter) =
    writeMidiVariableLength deltaTime writer
    writeMidiEventValue midiEventTypeValue midiChannelValue writer
    writeMidiParameters parm1 parm2 writer

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
            let mutable prevVolume = 127uy
            while length > 0 do
                let eventDescr = stream.ReadByte()

                let last = (eventDescr) >>> 7
                let eventType = LanguagePrimitives.EnumOfValue(byte ((eventDescr) <<< 1) >>> 5)
                let channel = byte ((eventDescr <<< 4) >>> 4)

                let midiEventType, midiParm1, midiParm2 =
                    match eventType with
                    | MusEventType.ReleaseNote ->
                        let b = stream.ReadByte()
                        let noteNumber = (b <<< 1) >>> 1
                        MidiEventType.NoteOff, noteNumber, 0uy
                    | MusEventType.PlayNote ->
                        let b = stream.ReadByte()
                        let volFlag = b >>> 7
                        let noteNumber = (b <<< 1) >>> 1
                        let volume =
                            if volFlag <> 0uy then
                                let b = stream.ReadByte()
                                let volume = (b <<< 1) >>> 1
                                prevVolume <- volume
                                volume
                            else
                                prevVolume
                        MidiEventType.NoteOn, noteNumber, volume
                    | MusEventType.PitchBend ->
                        let n = stream.ReadByte()
                        MidiEventType.PitchBend, (n &&& 1uy) <<< 6, (n >>> 1) &&& 127uy
                    | MusEventType.System ->
                        let b = stream.ReadByte()
                        let midiCtrlTy = (b <<< 1) >>> 1 |> LanguagePrimitives.EnumOfValue |> systemEventToMidiControllerType
                        MidiEventType.Controller, byte midiCtrlTy, 0uy
                    | MusEventType.Controller ->
                        let b = stream.ReadByte()
                        let musCtrlTy = (b <<< 1) >>> 1 >>> 2 |> LanguagePrimitives.EnumOfValue
                        let b = stream.ReadByte()
                        let value = (b <<< 1) >>> 1
                        if musCtrlTy = MusControllerType.ChangeInstrumentEvent then
                            MidiEventType.InstrumentChange, value, 0uy
                        else
                            let midiCtrlTy = musControllerTypeToMidiControllerType musCtrlTy
                            MidiEventType.Controller, byte midiCtrlTy, value
                    | MusEventType.EndOfMeasure ->
                        MidiEventType.Invalid, 0uy, 0uy
                    | MusEventType.Finish ->
                        MidiEventType.EndOfTrack, 0uy, 0uy
                    | MusEventType.Unused ->
                        MidiEventType.Invalid, 0uy, 0uy
                    | _ ->
                        failwithf "Invalid mus event: %A" eventType

                let deltaTime =
                    if last <> 0uy then
                        let delayed = ResizeArray()
                        let mutable stop = false
                        while not stop do
                            let b = stream.ReadByte()
                            if b &&& 0x80uy = 0uy then
                                stop <- true
                            delayed.Add b
                        delayed.ToArray()
                    else
                        [|0uy|]

                length <- length - int (stream.Position - prev)
                prev <- stream.Position

                if midiEventType <> MidiEventType.Invalid then
                    writeMidiEvent deltaTime (byte midiEventType) channel midiParm1 midiParm2 writer

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
    writeMidiHeader 0us 1us 120us writer
    writeMidiTrack bytes writer
    let bytes = ms.ToArray()
    writer.Dispose()
    bytes