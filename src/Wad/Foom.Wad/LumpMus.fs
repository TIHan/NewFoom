[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickler
open Foom.Wad.Unpickle
open System

type MusHeader =
    {
        Length: int
        Offset: int
        PrimaryChannelCount: int
        SecondaryChannelCount: int
        InstrumentCount: int
        InstrumentPatches: int []
    }

type MidiController =
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

let midiController controllerNumber =
    match controllerNumber with
    | 0uy -> Unchecked.defaultof<MidiController>
    | 1uy -> MidiController.BankSelect
    | 2uy -> MidiController.Modulation
    | 3uy -> MidiController.Volume
    | 4uy -> MidiController.Pan
    | 5uy -> MidiController.Expression
    | 6uy -> MidiController.ReverbDepth
    | 7uy -> MidiController.ChorusDepth
    | 8uy -> MidiController.SustainPedalHold
    | 9uy -> MidiController.SoftPedal
    | _ -> failwithf "Invalid controller: %A" controllerNumber

let systemEventToMidiController sysEvent =
    match sysEvent with
    | 10uy -> MidiController.AllSoundsOff
    | 11uy -> MidiController.AllNotesFadeOff
    | 12uy -> MidiController.Mono
    | 13uy -> MidiController.Poly
    | 14uy -> MidiController.ResetAllControllersOnChannel
    | _ -> midiController sysEvent

[<RequireQualifiedAccess>]
type MusEvent =
    | ReleaseNote of noteNumber: byte
    | PlayNote of noteNumber: byte * volume: byte option
    | PitchBend of amount: byte
    | System of MidiController
    | Controller of MidiController * value: byte
    | EndOfMeasure
    | Finish
    | Unused

type MusChannelEvent =
    {
        Channel: byte
        Event: MusEvent
        Delayed: byte []
    }

type Mus =
    {
        Events: MusChannelEvent []
    }

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
                                Some volume
                            else
                                None
                        MusEvent.PlayNote(noteNumber, volume)
                    | 2uy ->
                        let amount = stream.ReadByte()
                        MusEvent.PitchBend amount
                    | 3uy ->
                        let b = stream.ReadByte()
                        let sysEvent = (b <<< 1) >>> 1
                        MusEvent.System(systemEventToMidiController sysEvent)
                    | 4uy ->
                        let b = stream.ReadByte()
                        let controllerNumber = (b <<< 1) >>> 1 >>> 2
                        let b = stream.ReadByte()
                        let value = (b <<< 1) >>> 1
                        MusEvent.Controller(midiController controllerNumber, value)
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

            { Events = events.ToArray() }
    )

let parse lumpHeader =
    pHeader lumpHeader >>= fun musHeader ->
        pBody lumpHeader.Offset musHeader

let Parse lumpHeader (stream: Stream) =
    u_run (parse lumpHeader) (ReadStream stream) 