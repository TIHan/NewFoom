[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickler
open Foom.Wad.Unpickle
open System

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

type MusController =
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
    | ChangeInstrumentEvent = 255uy

[<RequireQualifiedAccess;NoEquality;NoComparison>]
type MusEvent =
    | ReleaseNote of number: byte
    | PlayNote of number: byte * volume: byte voption
    | PitchBend of amount: byte
    | System of MusController
    | Controller of MusController * value: byte
    | EndOfMeasure
    | Finish
    | Unused

[<NoEquality;NoComparison>]
type MusChannelEvent =
    {
        Channel: byte
        Event: MusEvent
        Delayed: byte []
    }

[<NoEquality;NoComparison>]
type Mus =
    {
        Header: MusHeader
        Events: MusChannelEvent []
    }

let musController number =
    match number with
    | 0uy -> MusController.ChangeInstrumentEvent
    | 1uy -> MusController.BankSelect
    | 2uy -> MusController.Modulation
    | 3uy -> MusController.Volume
    | 4uy -> MusController.Pan
    | 5uy -> MusController.Expression
    | 6uy -> MusController.ReverbDepth
    | 7uy -> MusController.ChorusDepth
    | 8uy -> MusController.SustainPedalHold
    | 9uy -> MusController.SoftPedal
    | _ -> failwithf "Invalid controller: %A" number

let systemEventToMusController sysEvent =
    match sysEvent with
    | 10uy -> MusController.AllSoundsOff
    | 11uy -> MusController.AllNotesFadeOff
    | 12uy -> MusController.Mono
    | 13uy -> MusController.Poly
    | 14uy -> MusController.ResetAllControllersOnChannel
    | _ -> musController sysEvent

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
let pBody offset musHeader =
    u_lookAhead (
        u_skipBytes (int64 musHeader.Offset + int64 offset) >>.
        fun stream ->
            let mutable length = musHeader.Length
            let events = ResizeArray()
            let mutable prev = stream.Position
            while length > 0 do
                let eventDescr = stream.ReadByte()

                let last = (eventDescr) >>> 7
                let eventType = ((eventDescr) <<< 1) >>> 5
                let channel = ((eventDescr <<< 4) >>> 4)

                let event =
                    match eventType with
                    | 0uy ->
                        let b = stream.ReadByte()
                        let noteNumber = (b <<< 1) >>> 1
                        MusEvent.ReleaseNote noteNumber
                    | 1uy ->
                        let b = stream.ReadByte()
                        let volFlag = b >>> 7
                        let noteNumber = (b <<< 1) >>> 1
                        let volume =
                            if volFlag <> 0uy then
                                let b = stream.ReadByte()
                                let volume = (b <<< 1) >>> 1
                                ValueSome volume
                            else
                                ValueNone
                        MusEvent.PlayNote(noteNumber, volume)
                    | 2uy ->
                        let amount = stream.ReadByte()
                        MusEvent.PitchBend amount
                    | 3uy ->
                        let b = stream.ReadByte()
                        let sysEvent = (b <<< 1) >>> 1
                        MusEvent.System(systemEventToMusController sysEvent)
                    | 4uy ->
                        let b = stream.ReadByte()
                        let controllerNumber = (b <<< 1) >>> 1 >>> 2
                        let b = stream.ReadByte()
                        let value = (b <<< 1) >>> 1
                        MusEvent.Controller(musController controllerNumber, value)
                    | 5uy ->
                        MusEvent.EndOfMeasure
                    | 6uy ->
                        MusEvent.Finish
                    | 7uy ->
                        MusEvent.Unused
                    | _ ->
                        failwithf "Invalid mus event: %A" eventType

                let delayed =
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
                events.Add { Channel = channel; Event = event; Delayed = delayed }

            { Header = musHeader; Events = events.ToArray() }
    )

let parse lumpHeader =
    pHeader lumpHeader >>= fun musHeader ->
        pBody lumpHeader.Offset musHeader

let Parse lumpHeader (stream: Stream) =
    u_run (parse lumpHeader) (ReadStream stream)

// MIDI

open FSharp.NativeInterop
open System
open System.Runtime.CompilerServices

#nowarn "9"

type BinaryWriter with

    [<RequiresExplicitTypeArguments>]
    member inline this.WriteBE<'T when 'T : unmanaged>(value: 'T) =
        let stack = Span<byte>(Unsafe.AsPointer(&Unsafe.AsRef &value), 4)
        stack.Reverse()
        this.Write(Span.op_Implicit stack)

type MThdHeader =
    {
        Length: uint32
        Type: uint16
        TrackCount: uint16
        TicksPerQuarterNote: uint16
    }

type MidiEventType =
    | NoteOff = 0x80
    | NoteOn = 0x90
    | PolyphonicKeyPressure = 0xA0
    | Controller = 0xB0
    | InstrumentChange = 0xC0
    | ChannelPressure = 0xD0
    | PitchBend = 0xE0

[<RequireQualifiedAccess>]
type MidiEvent =
    | NoteOff of noteNumber: uint8 * velocity: uint8
    | NoteOn of noteNumber: uint8 * velocity: uint8
    | PolyphonicKeyPressure of pressureValue: uint8 * noteNumber: uint8 
    | Controller of controllerNumber: uint8 * value: uint8
    | InstrumentChange of instrumentNumber: uint8
    | PitchBend of lsb: uint8 * msb: uint8
    | EndOfTrack

    member this.Value =
        match this with
        | MidiEvent.NoteOff _ -> 0x80
        | MidiEvent.NoteOn _ -> 0x90
        | MidiEvent.PolyphonicKeyPressure _ -> 0xA0
        | MidiEvent.Controller _ -> 0xB0
        | MidiEvent.InstrumentChange _ -> 0xC0
        | MidiEvent.PitchBend _ -> 0xE0
        | MidiEvent.EndOfTrack -> 0x2F

type MTrkBlock =
    {
        Length: uint32
        Events: MidiEvent []
    }

let writeMThdHeader (writer: BinaryWriter) (header: MThdHeader) =
    writer.Write([|byte 'M';byte 'T';byte 'h';byte 'd'|])
    writer.WriteBE<uint32> header.Length
    writer.WriteBE<uint16> header.Type
    writer.WriteBE<uint16> header.TrackCount
    writer.WriteBE<uint16> header.TicksPerQuarterNote

let writeMidiEvent (writer: BinaryWriter) (midiEvent: MidiEvent) =
    match midiEvent with
    | MidiEvent.NoteOff(n, v) -> 0x80
    | MidiEvent.NoteOn _ -> 0x90
    | MidiEvent.PolyphonicKeyPressure _ -> 0xA0
    | MidiEvent.Controller _ -> 0xB0
    | MidiEvent.InstrumentChange _ -> 0xC0
    | MidiEvent.PitchBend _ -> 0xE0
    | MidiEvent.EndOfTrack -> 0x2F

let tryConvertMusEventToMidiEvent (prevVolume: byref<byte>) channelCount (musChannelEvent: MusChannelEvent) =
    match musChannelEvent.Event with
    | MusEvent.ReleaseNote n -> 
        MidiEvent.NoteOff(n, 127uy) 
        |> ValueSome
    | MusEvent.PlayNote(n, volumeOpt) ->
        let volume =
            match volumeOpt with
            | ValueSome volume -> 
                prevVolume <- volume
                volume
            | _ -> prevVolume
        MidiEvent.NoteOn(n, volume) 
        |> ValueSome
    | MusEvent.PitchBend n ->
        MidiEvent.PitchBend((n &&& 1uy) <<< 6, (n >>> 1) &&& 127uy) 
        |> ValueSome
    | MusEvent.System midiController -> 
        MidiEvent.Controller(byte midiController, if midiController = MusController.Mono then channelCount else 0uy) 
        |> ValueSome
    | MusEvent.Controller(midiController, value) ->
        if midiController = MusController.ChangeInstrumentEvent then
            MidiEvent.InstrumentChange(value) |> ValueSome
        else
            let value =
                if midiController = MusController.Volume then
                    if value > 127uy then 127uy
                    else 0uy
                else
                    value
            MidiEvent.Controller(byte midiController, if midiController = MusController.Mono then channelCount else value)
            |> ValueSome
    | MusEvent.EndOfMeasure ->
        ValueNone
    | MusEvent.Finish ->
        MidiEvent.EndOfTrack
        |> ValueSome
    | MusEvent.Unused ->
        ValueNone

let convertMusToMidi (mus: Mus) =
    let events =
        let mutable prevVolume = 127uy
        let events = ResizeArray()
        for musEvent in mus.Events do
            let res = tryConvertMusEventToMidiEvent &prevVolume (byte mus.Header.PrimaryChannelCount + byte mus.Header.SecondaryChannelCount) musEvent
            match res with
            | ValueSome midiEvent -> events.Add midiEvent
            | _ -> ()
        events.ToArray()
    events

let GetMidiStream lumpHeader stream =
    let mus = Parse lumpHeader stream
    convertMusToMidi mus
    ()