[<RequireQualifiedAccess>]
module internal Foom.Wad.LumpMus

open System.IO
open Foom.Wad.Unpickler
open Foom.Wad.Unpickle
open System

type MusHeader =
    {
        ScoreLength: int
        ScoreStart: int
        PrimaryChannelCount: int
        SecondaryChannelCount: int
        InstrumentCount: int
        Instruments: int []
    }

type Mus =
    {
        Events: int [] // placeholder
    }

let pHeader lumpHeader =
    goToLump lumpHeader (
        (
            u_pipe7 
                (u_string 4)
                u_uint16 // scoreLen
                u_uint16 // scoreStart
                u_uint16 // channels
                u_uint16 // sec_channels
                u_uint16 // instrCnt
                u_uint16 // dummy
            <| fun id scoreLen scoreStart channel secChannels instrCount _ ->
                let isIdMus = id.StartsWith("mus", StringComparison.OrdinalIgnoreCase)
                if not isIdMus then
                    failwith "Not a MUS lump."
                    
                {
                    ScoreLength = int scoreLen
                    ScoreStart = int scoreStart
                    PrimaryChannelCount = int channel
                    SecondaryChannelCount = int secChannels
                    InstrumentCount = int instrCount
                    Instruments = [||]
                }
        )
        >>= fun header ->
            u_array header.InstrumentCount u_uint16 
            |>> fun instruments -> 
                { header with Instruments = instruments |> Array.map int }
    )

// http://www.shikadi.net/moddingwiki/MUS_Format
let pBody musHeader =
    u_lookAhead (
        u_skipBytes (int64 musHeader.ScoreStart) >>.
        fun stream ->
            let eventDescr = stream.ReadByte()

            let last = (eventDescr) >>> 7
            let eventType = ((eventDescr) <<< 1) >>> 5
            let channel = ((eventDescr <<< 4) >>> 4)

            { Events = [||] }
    )

let parse lumpHeader =
    pHeader lumpHeader >>= fun musHeader ->
        pBody musHeader

let Parse lumpHeader (stream: Stream) =
    u_run (parse lumpHeader) (ReadStream stream) 