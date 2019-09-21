module Foom.Win32

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop
open Foom.Game
open Foom.Game.Input
open Foom.Win32Internal

#nowarn "9"

open System.Collections.Generic

let private createWin32Window (windowTitle: string) (del: WndProcDelegate) =
    use title = fixed (System.Text.Encoding.UTF8.GetBytes(windowTitle))
    let title = title |> NativePtr.toNativeInt |> NativePtr.ofNativeInt
    let hInstance = Marshal.GetHINSTANCE(typeof<WNDCLASSEXW>.Module)

    let mutable windowClass = Unchecked.defaultof<WNDCLASSEXW>
    windowClass.cbSize <- uint32 sizeof<WNDCLASSEXW>
    windowClass.style <- uint32((* CS_HREDRAW *) 0x0002 ||| (* CS_VREDRAW *) 0x0001)
    windowClass.lpfnWndProc <- Marshal.GetFunctionPointerForDelegate(del)
    windowClass.hInstance <- hInstance
    windowClass.lpszClassName <- title
    windowClass.hCursor <- LoadCursor(IntPtr.Zero, (* IDC_ARROW *)32512)

    if (RegisterClassExW(&windowClass) = 0) then
        printfn "Failed Registering Window Class"
        IntPtr.Zero, IntPtr.Zero
    else

    printfn "- Window Class Registered"

    let hwnd =
        CreateWindowExW(
            0u,
            windowClass.lpszClassName,
            title,
            WS_OVERLAPPEDWINDOW,
            0,
            0,
            1280,
            720,
            IntPtr.Zero,
            IntPtr.Zero,
            hInstance,
            IntPtr.Zero
        )

    if hwnd = IntPtr.Zero then
        printfn "Failed Creating Window"
        IntPtr.Zero, IntPtr.Zero
    else

    printfn "- Created Window"

    printfn "ShowWindow Result: %A" <| ShowWindow(hwnd, 1)
    printfn "UpdateWindow Result: %A" <| UpdateWindow(hwnd)
    hwnd, hInstance

[<Sealed>]
type Win32Game (windowTitle: string, svGame: AbstractServerGame, clGame: AbstractClientGame, interval) =

    let del = WndProcDelegate(Win32Game.WndProc)
    let hwnd, hinstance = createWin32Window windowTitle del

    // Store this so it doesn't get collected cause a System.ExecutionEngineException.
    member val private WndProcDelegate = del

    member __.Hwnd = hwnd

    member __.Hinstance = hinstance

    member __.Width = 1280

    member __.Height = 720

    static member private WndProc hWnd msg wParam lParam =
        DefWindowProc(hWnd, msg, wParam, lParam)

    member __.Start() =
        let mutable msg = Unchecked.defaultof<MSG>
        let mutable gl = GameLoop.create interval
        let inputs = ResizeArray()
        let hashKey = HashSet()
        while not gl.WillQuit do
            while PeekMessage(&&msg, nativeint 0, 0u, 0u, 0x0001u) <> 0uy do
                TranslateMessage(&msg) |> ignore
                DispatchMessage(&msg) |> ignore

                match msg.message with
                | x when int x = WM_KEYDOWN ->
                    if hashKey.Add(char msg.lParam) then
                        inputs.Add(KeyPressed(char msg.lParam))
                | x when int x = WM_KEYUP ->
                    if hashKey.Remove(char msg.lParam) then
                        inputs.Add(KeyReleased(char msg.lParam))
                | _ -> ()

            gl <- GameLoop.tick
                    id
                    (fun time interval ->
                        let time = TimeSpan.FromTicks(time)
                        let interval = TimeSpan.FromTicks(interval)

                        let inputList = inputs |> Seq.toList
                        inputs.Clear()

                        clGame.PreUpdate(time, interval, inputList)
                        let svWillQuit = svGame.Update(time, interval)
                        let clWillQuit = clGame.Update(time, interval, inputList)

                        svWillQuit || clWillQuit
                    )
                    (fun time deltaTime ->
                        let time = TimeSpan.FromTicks(time)

                        clGame.Render(time, deltaTime)
                    )
                    gl