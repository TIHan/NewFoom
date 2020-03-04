open System
open Foom.Wad
open FSharp.NativeInterop
open System.Text
open System.Runtime.InteropServices
open System.Runtime.CompilerServices

#nowarn "9"
#nowarn "51"

let fmodCheckResult res =
    if res <> FMOD.RESULT.OK then
        failwithf "FMOD error! (%A) %s\n" res (FMOD.Error.String res)

[<EntryPoint>]
let main argv =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    let music = wad.TryFindMusic "d_e1m2"
    System.IO.File.WriteAllBytes("test.mid", music.Value)

    let res, fmodSystem = FMOD.Factory.System_Create()
    fmodCheckResult res
    fmodSystem.init(512, FMOD.INITFLAGS.NORMAL, 0n) |> fmodCheckResult

    let res, soundGroup = fmodSystem.createSoundGroup("wad")
    fmodCheckResult res

    //let mutable info = FMOD.CREATESOUNDEXINFO()
    //info.format <- FMOD.SOUND_FORMAT.PCM16
    //info.cbsize <- sizeof<FMOD.CREATESOUNDEXINFO>
    //let res, sound = fmodSystem.createSound(music.Value, FMOD.MODE.LOOP_NORMAL, &info)
    //fmodCheckResult res

    let res, sound = fmodSystem.createSound("test.mid", FMOD.MODE.LOOP_NORMAL)
    fmodCheckResult res

    sound.setSoundGroup soundGroup |> fmodCheckResult

    let res, channelGroup = fmodSystem.createChannelGroup("music")
    fmodCheckResult res

    let res, channel = fmodSystem.playSound(sound, channelGroup, false)
    fmodCheckResult res

    let res, isPlaying = channel.isPlaying()
    fmodCheckResult res

    fmodSystem.update() |> fmodCheckResult

    let res, v = channel.getVolume()
    printfn "volume: %A" v

    printfn "isPlaying: %A" isPlaying
   // fmodSystem.Play

 //   System.IO.File.WriteAllBytes("test.mid", music.Value)
   // let deviceId = getDeviceId ()

   /// let filePath = """C:\NewFoom\src\Wad\Foom.Wad.MusicExtractor\D_DEAD.mid"""
  //  playFile filePath

//    printfn "%A" (isFileFinished deviceId)

    System.Threading.Thread.Sleep(3000000)
    0
