[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickle

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

val Parse : LumpHeader -> Stream -> Mus
