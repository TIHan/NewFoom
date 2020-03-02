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
    |> Array.iter writer.WriteBE<byte>

let writeMidiEventValue (midiEventTypeValue: byte) (midiChannelValue: byte) (writer: BinaryWriter) =
    writer.WriteBE<byte> (midiEventTypeValue ||| midiChannelValue)

let writeMidiParameters (parm1: byte) (parm2: byte) (writer: BinaryWriter) =
    writer.WriteBE<byte> parm1
    writer.WriteBE<byte> parm2

let writeMidiEvent deltaTime (midiEventTypeValue: byte) (midiChannelValue: byte) (parm1: byte) (parm2: byte) (writer: BinaryWriter) =
    writeMidiVariableLength deltaTime writer
    writeMidiEventValue midiEventTypeValue midiChannelValue writer
    writeMidiParameters parm1 parm2 writer

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
                        [||]

                length <- length - int (stream.Position - prev)
                prev <- stream.Position

                if midiEventType <> MidiEventType.Invalid then
                    ()

            writer.BaseStream.Position <- 0L
            let res = (new BinaryReader(writer.BaseStream)).ReadBytes(int writer.BaseStream.Length)
            writer.Dispose()
            res
    )

let parse lumpHeader writer =
    pHeader lumpHeader >>= fun musHeader ->
        pBody lumpHeader.Offset musHeader writer

let Parse lumpHeader (stream: Stream) =
    let ms = new MemoryStream()
    let writer = new BinaryWriter(ms, Text.Encoding.UTF8, leaveOpen=false)
    u_run (parse lumpHeader writer) (ReadStream stream)

//// MIDI

//type MThdHeader =
//    {
//        Length: uint32
//        Type: uint16
//        TrackCount: uint16
//        TicksPerQuarterNote: uint16
//    }

//type MidiEventType =
//    | NoteOff = 0x80uy
//    | NoteOn = 0x90uy
//    | PolyphonicKeyPressure = 0xA0uy
//    | Controller = 0xB0uy
//    | InstrumentChange = 0xC0uy
//    | ChannelPressure = 0xD0uy
//    | PitchBend = 0xE0uy
//    | EndOfTrack = 0x2Fuy

//[<RequireQualifiedAccess>]
//type MidiEvent =
//    | NoteOff of noteNumber: uint8 * velocity: uint8
//    | NoteOn of noteNumber: uint8 * velocity: uint8
//    | PolyphonicKeyPressure of pressureValue: uint8 * noteNumber: uint8 
//    | Controller of controllerNumber: uint8 * value: uint8
//    | InstrumentChange of instrumentNumber: uint8
//    | PitchBend of lsb: uint8 * msb: uint8
//    | EndOfTrack

//    member this.Value =
//        match this with
//        | MidiEvent.NoteOff _ -> 0x80
//        | MidiEvent.NoteOn _ -> 0x90
//        | MidiEvent.PolyphonicKeyPressure _ -> 0xA0
//        | MidiEvent.Controller _ -> 0xB0
//        | MidiEvent.InstrumentChange _ -> 0xC0
//        | MidiEvent.PitchBend _ -> 0xE0
//        | MidiEvent.EndOfTrack -> 0x2F

//type MTrkBlock =
//    {
//        Length: uint32
//        Events: MidiEvent []
//    }

//let writeMThdHeader (writer: BinaryWriter) (header: MThdHeader) =
//    writer.Write([|byte 'M';byte 'T';byte 'h';byte 'd'|])
//    writer.WriteBE<uint32> header.Length
//    writer.WriteBE<uint16> header.Type
//    writer.WriteBE<uint16> header.TrackCount
//    writer.WriteBE<uint16> header.TicksPerQuarterNote

//let writeMidiEvent (writer: BinaryWriter) (midiEvent: MidiEvent) =
//    match midiEvent with
//    | MidiEvent.NoteOff(n, v) -> 0x80
//    | MidiEvent.NoteOn _ -> 0x90
//    | MidiEvent.PolyphonicKeyPressure _ -> 0xA0
//    | MidiEvent.Controller _ -> 0xB0
//    | MidiEvent.InstrumentChange _ -> 0xC0
//    | MidiEvent.PitchBend _ -> 0xE0
//    | MidiEvent.EndOfTrack -> 0x2F

//let tryConvertMusEventToMidiEvent (prevVolume: byref<byte>) channelCount (musChannelEvent: MusChannelEvent) =
//    match musChannelEvent.Event with
//    | MusEvent.ReleaseNote n -> 
//        MidiEvent.NoteOff(n, 127uy) 
//        |> ValueSome
//    | MusEvent.PlayNote(n, volumeOpt) ->
//        let volume =
//            match volumeOpt with
//            | ValueSome volume -> 
//                prevVolume <- volume
//                volume
//            | _ -> prevVolume
//        MidiEvent.NoteOn(n, volume) 
//        |> ValueSome
//    | MusEvent.PitchBend n ->
//        MidiEvent.PitchBend((n &&& 1uy) <<< 6, (n >>> 1) &&& 127uy) 
//        |> ValueSome
//    | MusEvent.System midiController -> 
//        MidiEvent.Controller(byte midiController, if midiController = MusController.Mono then channelCount else 0uy) 
//        |> ValueSome
//    | MusEvent.Controller(midiController, value) ->
//        if midiController = MusController.ChangeInstrumentEvent then
//            MidiEvent.InstrumentChange(value) |> ValueSome
//        else
//            let value =
//                if midiController = MusController.Volume then
//                    if value > 127uy then 127uy
//                    else 0uy
//                else
//                    value
//            MidiEvent.Controller(byte midiController, if midiController = MusController.Mono then channelCount else value)
//            |> ValueSome
//    | MusEvent.EndOfMeasure ->
//        ValueNone
//    | MusEvent.Finish ->
//        MidiEvent.EndOfTrack
//        |> ValueSome
//    | MusEvent.Unused ->
//        ValueNone

//let convertMusToMidi (mus: Mus) =
//    let events =
//        let mutable prevVolume = 127uy
//        let events = ResizeArray()
//        for musEvent in mus.Events do
//            let res = tryConvertMusEventToMidiEvent &prevVolume (byte mus.Header.PrimaryChannelCount + byte mus.Header.SecondaryChannelCount) musEvent
//            match res with
//            | ValueSome midiEvent -> events.Add midiEvent
//            | _ -> ()
//        events.ToArray()
//    events

//let GetMidiStream lumpHeader stream =
//    let mus = Parse lumpHeader stream
//    convertMusToMidi mus
//    ()