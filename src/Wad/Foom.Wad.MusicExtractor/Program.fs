open System
open Foom.Wad
open FSharp.NativeInterop
open System.Text
open System.Runtime.InteropServices
open System.Runtime.CompilerServices

#nowarn "9"
#nowarn "51"

module MciInterop =

    type DWORD = uint32
    type DWORD_PTR = nativeint
    type MCIDEVICEID = int
    type LPCTSTR = nativeint
    type BOOL = byte
    type LPTSTR = StringBuilder
    type UINT = uint32
    type MCIERROR = uint32

    //let MCI_OPEN = 0x0803u
    //let MCI_STATUS = 0x0814u
    //let MCI_NOTIFY = 0x00000001u
    //let MCI_STRING_OFFSET = 512u
    //let MCI_MODE_PLAY = MCI_STRING_OFFSET + 14u
    //let MCI_OPEN_TYPE = 0x00002000u
    //let MCI_OPEN_SHAREABLE = 0x00000100u
    //let MCI_DEVTYPE_SEQUENCER = MCI_STRING_OFFSET + 11u

    //[<Struct;NoEquality;NoComparison;StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)>]
    //type MCI_OPEN_PARMS =
    //    val mutable dwCallback: DWORD_PTR
    //    val mutable wDeviceID: MCIDEVICEID
    //    val mutable lpstrDeviceType: LPCTSTR
    //    val mutable lpstrElementName: LPCTSTR
    //    val mutable lpstrAlias: LPCTSTR
    //    val mutable x: nativeint

    //[<Struct;NoEquality;NoComparison>]
    //type MCI_STATUS_PARMS =
    //    val mutable dwCallback: DWORD_PTR
    //    val mutable dwReturn: DWORD_PTR
    //    val mutable dwItem: DWORD
    //    val mutable dwTrack: DWORD

    [<DllImport("winmm.dll")>]
    extern MCIERROR mciSendString(string command, String buffer, int bufferSize, nativeint hwndCallback)

    //[<DllImport("winmm.dll")>]
    //extern MCIERROR mciSendCommand(MCIDEVICEID deviceId, uint32 msg, uint32 flags, nativeint parms)

    [<DllImport("winmm.dll")>]
    extern BOOL mciGetErrorString(DWORD fdwError, LPTSTR lpszErrorText, UINT cchErrorText)

open MciInterop

let checkResult x =
    if x <> 0u then
        let buffer = StringBuilder 256
        if mciGetErrorString(x, buffer, uint32 buffer.Capacity) <> 1uy then
            failwith "mciGetErrorString: Failed"
        failwithf "%s" (buffer.ToString())

//let getDeviceId () =
//    let mutable openParms = MCI_OPEN_PARMS()
//    openParms.lpstrDeviceType <- Marshal.StringToHGlobalAnsi "cdaudio"
//    mciSendCommand(0, MCI_OPEN, MCI_OPEN_TYPE ||| MCI_OPEN_SHAREABLE, &&openParms |> NativePtr.toNativeInt) |> checkResult
//    printfn "%A" openParms.lpstrDeviceType
//    printfn "%A" openParms.dwCallback
//    openParms.wDeviceID

//let isFileFinished deviceId =
//    let mutable statusParms = MCI_STATUS_PARMS()
//    mciSendCommand(deviceId, MCI_STATUS, MCI_NOTIFY, &&statusParms |> NativePtr.toNativeInt) |> checkResult
//    statusParms.dwReturn <> nativeint MCI_MODE_PLAY

let stopFile () =
    mciSendString("stop track", "", 0, nativeint 0) |> ignore
    mciSendString("close track", "", 0, nativeint 0) |> ignore

let playFile (filePath: string) =
    stopFile ()
    
    mciSendString("open " + filePath + " alias track", "", 0, nativeint 0) |> checkResult
    mciSendString("play track", "", 0, nativeint 0) |> checkResult

let closeAudio () =
    mciSendString("close all", null, 0, nativeint 0) |> checkResult

[<EntryPoint>]
let main argv =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let music = wad.TryFindMusic "d_e1m1"
    System.IO.File.WriteAllBytes("test.mid", music.Value)
   // let deviceId = getDeviceId ()

   /// let filePath = """C:\NewFoom\src\Wad\Foom.Wad.MusicExtractor\D_DEAD.mid"""
  //  playFile filePath

//    printfn "%A" (isFileFinished deviceId)

   // System.Threading.Thread.Sleep(3000000)
    0
