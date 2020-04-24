open Foom.Game
open Foom.Win32
open FSharp.Vulkan.Interop
open FSharp.Window
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open System
open System.Numerics
open System.Drawing
open System.Drawing.Imaging
open FSharp.NativeInterop
open Foom.Wad
open FsGame.Graphics.FreeType
open FsGame.Graphics.Vulkan
open FsGame.Renderer
open System.Text

#nowarn "9"
#nowarn "51"

let loadMusic (wad: Wad) =    
    let fmodCheckResult res =
        if res <> FMOD.RESULT.OK then
            failwithf "FMOD error! (%A) %s\n" res (FMOD.Error.String res)
    
    let music = wad.TryFindMusic "d_e1m2"
    
    let res, fmodSystem = FMOD.Factory.System_Create()
    fmodCheckResult res
    fmodSystem.init(512, FMOD.INITFLAGS.NORMAL, 0n) |> fmodCheckResult
    
    let res, soundGroup = fmodSystem.createSoundGroup("wad")
    fmodCheckResult res
    
    let mutable info = FMOD.CREATESOUNDEXINFO()
    info.format <- FMOD.SOUND_FORMAT.PCM16
    info.cbsize <- sizeof<FMOD.CREATESOUNDEXINFO>
    info.length <- uint32 music.Value.Length
    let res, sound = fmodSystem.createSound(music.Value, FMOD.MODE.OPENMEMORY ||| FMOD.MODE.LOOP_NORMAL, &info)
    Array.Clear(music.Value, 0, music.Value.Length)
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
    ()

   // let freeType = FreeType.Create()
   // let face = freeType.Load("fonts/OpenSans/OpenSans-Regular.ttf")
   //// face.SetCharSize(0, 16*64, 300u, 300u)
   // use doot = face.GetCharBitmap('$', 1024)
   // let bmp = doot.Bitmap
   // let shader = textShader instance

   // let verticesBuffer = instance.CreateBuffer<Vector3>(6, FalkanBufferFlags.None, VertexBuffer)
   // let vertices =
   //     let xratio = (1024 - bmp.Width + doot.Left)

   //     [|
   //         Vector3(-0.5f * 500.f, -0.5f * 500.f, -1.f)
   //         Vector3(0.5f * 500.f, -0.5f * 500.f, -1.f)
   //         Vector3(0.5f * 500.f, 0.5f * 500.f, -1.f)

   //         Vector3(0.5f * 500.f, 0.5f * 500.f, -1.f)
   //         Vector3(-0.5f * 500.f, 0.5f * 500.f, -1.f)
   //         Vector3(-0.5f * 500.f, -0.5f * 500.f, -1.f)

   //     |]
   // instance.FillBuffer(verticesBuffer, ReadOnlySpan vertices)

   // let uvBuffer = instance.CreateBuffer<Vector2>(6, FalkanBufferFlags.None, VertexBuffer)
   // let uv =
   //     [|
   //         Vector2(0.f, 0.f)
   //         Vector2(1.f, 0.f)          
   //         Vector2(1.f, -1.f)

   //         Vector2(1.f, -1.f)
   //         Vector2(0.f, -1.f)
   //         Vector2(0.f, 0.f)
   //     |]
   // instance.FillBuffer(uvBuffer, ReadOnlySpan uv)

   // let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
   // let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)
   // let image = instance.CreateImage(bmp.Width, bmp.Height)
   // let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
   // instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
   // bmp.UnlockBits(data)

   // let ortho = Matrix4x4.CreateOrthographic(1280.f, 720.f, 0.1f, 100.f)
   // let orthoUniform = instance.CreateBuffer<Projection>(1, FalkanBufferFlags.None, FalkanBufferKind.UniformBuffer)
   // orthoUniform.SetData(ReadOnlySpan [|{ proj = ortho }|])

   // let draw = shader.CreateDrawBuilder()
   // let draw = draw.AddDescriptorImage(image)
   // let draw = draw.AddDescriptorBuffer(orthoUniform, 1)
   // let draw = draw.Next.AddVertexBuffer(verticesBuffer)
   // let draw = draw.AddVertexBuffer(uvBuffer)
   // shader.AddDraw(draw, uint32 vertices.Length, 1u) |> ignore

//let createMvp (start: Vector2) =
//    let mutable quat =  Matrix4x4.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
//    quat.Translation <- Vector3(start.X, start.Y, 28.f)
//    {
//        model = Matrix4x4.Identity
//        view = quat
//        proj = Matrix4x4.CreatePerspectiveFieldOfView(radians 45.f, 1280.f / 720.f, 0.1f, 1000000.f)
//    }

open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler.Interactive.Shell
open System.IO

[<Sealed>]
type FSharpInteractive private (inStream, outStream, errStream) =

    // Build command line arguments & start FSI session
    let argv = [| "C:\\fsi.exe" |]
    #if NETCOREAPP
    let args = Array.append argv [|"--noninteractive"; "--targetprofile:netcore"|]
    #else
    let args = Array.append argv [|"--noninteractive"|]
    #endif
    let allArgs = args

    let fsiConfig = FsiEvaluationSession.GetDefaultConfiguration()
    let fsiSession = FsiEvaluationSession.Create(fsiConfig, allArgs, inStream, outStream, errStream, collectible = true)

    member _.SubmitInteraction code =
        let _, errors = fsiSession.EvalInteractionNonThrowing(code)
        if errors.Length = 0 then
            String.Empty
        else
            sprintf "%A" errors

    interface IDisposable with

        member _.Dispose() =
            (fsiSession :> IDisposable).Dispose()
            inStream.Dispose()
            outStream.Dispose()
            errStream.Dispose()

    static member Initialize(inStream, outStream, errStream) = new FSharpInteractive(inStream, outStream, errStream)


type InteractiveCommand =
    | SubmitInteraction of code: string

let globalCommands = System.Collections.Concurrent.ConcurrentQueue()

[<EntryPoint>]
let main argv =
    let wad = Wad.FromFile("../../../../../../Foom-deps/testwads/doom1.wad")
    loadMusic wad

    let mutable mvp = Unchecked.defaultof<_>

    let mutable yaw = 0.f
    let mutable pitch = 0.f

    let eventQueue = System.Collections.Generic.Queue<FalGraphics -> unit>()

    let mutable mvpUniform = Unchecked.defaultof<_>

    eventQueue.Enqueue(fun graphics ->
        let mvpUniform2, mvp2 = FsGame.Renderer.Vulkan.loadMap "e1m1" wad graphics
        graphics.SetupCommands()
        mvpUniform <- mvpUniform2
        mvp <- mvp2
    )

    let sbOut = new StringBuilder()
    let sbErr = new StringBuilder()
    let inStream = new StringReader("")
    let outStream = new StringWriter(sbOut)
    let errStream = new StringWriter(sbErr)

    use fsi = FSharpInteractive.Initialize(inStream, outStream, errStream)

    let lineQueue = System.Collections.Concurrent.ConcurrentQueue()

    async {
        let rec loop () =
            let lineStr = Console.ReadLine()
            lineQueue.Enqueue(lineStr)
            loop ()
        loop ()
    } |> Async.Start

    let windowEvents = 
        { new FsGame.Renderer.Vulkan.IVulkanWindowEvents with

            member __.OnWindowClosing _ = ()

            member __.OnInputEvents(_, events) = 
                let mutable acc = Vector3.Zero

                let mutable view = mvp.view

                let getRotation () =
                    Quaternion.CreateFromRotationMatrix view

                let setRotation quat =
                    let mutable m = Matrix4x4.CreateFromQuaternion quat
                    m.Translation <- view.Translation
                    view <- m

                let rotation = getRotation ()

                let mutable xrel = 0.f
                let mutable yrel = 0.f
                events
                |> List.iter (fun x ->
                    let v =
                        match x with
                        | InputEvent.KeyPressed 'W' -> Vector3.Transform (-Vector3.UnitZ, rotation)
                        | InputEvent.KeyPressed 'S' -> Vector3.Transform (Vector3.UnitZ, rotation)
                        | InputEvent.KeyPressed 'A' -> Vector3.Transform (-Vector3.UnitX, rotation)
                        | InputEvent.KeyPressed 'D' -> Vector3.Transform (Vector3.UnitX, rotation)
                        | InputEvent.MouseMoved(_, _, xrel2, yrel2) ->
                            xrel <- float32 xrel2
                            yrel <- float32 yrel2
                            Vector3.Zero
                        | _ -> Vector3.Zero
                    acc <- acc + (Vector3(v.X, v.Y, v.Z))
                )

                acc <-
                    if acc <> Vector3.Zero then
                        (acc |> Vector3.Normalize) * 50.f
                    else
                        acc

                if xrel <> 0.f || yrel <> 0.f then
                    yaw <- yaw + (xrel * -0.25f) * (MathF.PI / 180.f)
                    pitch <- pitch + (yrel * -0.25f) * (MathF.PI / 180.f)
                    let rotation = Quaternion.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
                    setRotation (rotation * Quaternion.CreateFromYawPitchRoll(yaw * 0.25f, pitch * 0.25f, 0.f))

                view.Translation <- view.Translation + acc
                mvp <-
                    { mvp with view = view }               

            member __.OnUpdateFrame (graphics, time, interval) =

                let mutable canPrintArrow = false
                let mutable lineStr = ""
                while lineQueue.TryDequeue &lineStr do
                    canPrintArrow <- true
                    let errorString = fsi.SubmitInteraction lineStr
                    if not (String.IsNullOrWhiteSpace(errorString)) then
                        Console.ForegroundColor <- ConsoleColor.Red
                        printfn "%s\n" errorString
                        Console.ResetColor()

                if sbOut.Length > 0 then
                    printf "%s" (sbOut.ToString())

                sbOut.Clear() |> ignore
                sbErr.Clear() |> ignore

                if canPrintArrow then
                    printf "> "

                match eventQueue.TryDequeue() with
                | true, f -> f graphics
                | _ -> ()
                if not (obj.ReferenceEquals(mvpUniform, null)) then
                    mvpUniform.Upload(ReadOnlySpan [|mvp.InvertedView|])
                false 
                
            member _.OnRenderFrame(graphics, _, _, _, _) =
                graphics.DrawFrame() }

    let window, graphics = FsGame.Renderer.Vulkan.createVulkanWin32Window "F# Vulkan" "HealthyCore" 30. 1280 720 windowEvents
    use graphics = graphics
    window.Start()
    printfn "F# Vulkan ended...."
    0
