module Foom.Win32

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop
open Foom.Game
open Foom.Game.Input
open Foom.Win32Internal

#nowarn "9"
#nowarn "51"

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
                TranslateMessage(&&msg) |> ignore
                DispatchMessage(&&msg) |> ignore

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
                        let deltaTime = TimeSpan.FromTicks(deltaTime)
                        clGame.Render(time, single deltaTime.TotalMilliseconds)
                        false
                    )
                    gl

open FSharp.Window

let private hideCursor hWnd =
    let mutable rect = Unchecked.defaultof<RECT>
    GetWindowRect(hWnd, &&rect) |> ignore
    ClipCursor(&&rect) |> ignore
    ShowCursor(0uy) |> ignore

let private centerCursor hWnd =
    let mutable rect = Unchecked.defaultof<RECT>
    GetWindowRect(hWnd, &&rect) |> ignore
    SetCursorPos(int rect.right / 2, int rect.bottom / 2) |> ignore

let private tryGetCursorPos hWnd =
    let mutable point = Unchecked.defaultof<_>
    if GetCursorPos(&&point) = 1uy then
        if ScreenToClient(hWnd, &&point) = 1uy then
            ValueSome point
        else
            ValueNone
    else
        ValueNone

[<Sealed>]
type Win32Window(title: string, width: int, weight: int, fixedUpdateInterval: float, events: IWindowEvents) as this =

    let mutable del = Unchecked.defaultof<_>
    let mutable hwnd = nativeint 0
    let mutable hinstance = nativeint 0

    let closing = Event<unit> ()
    let resized = Event<unit> ()

    let mutable prevPoint = POINT()

    let poll () =
        let hashKey = HashSet ()

        let mutable msg = MSG ()
        while PeekMessage(&&msg, nativeint 0, 0u, 0u, PM_REMOVE) <> 0uy do
            TranslateMessage(&&msg) |> ignore
            DispatchMessage(&&msg) |> ignore

            match int msg.message with
            | x when x = WM_KEYDOWN || x = WM_SYSKEYDOWN ->
                if hashKey.Add(char msg.lParam) then
                    events.OnKeyPressed(char msg.lParam)

            | x when x = WM_KEYUP || x = WM_SYSKEYUP ->
                if hashKey.Remove(char msg.lParam) then
                    events.OnKeyReleased(char msg.lParam)

            | x when int x = WM_MOUSEMOVE ->
                match tryGetCursorPos hwnd with
                | ValueSome point ->
                    let xrel = point.x - prevPoint.x
                    let yrel = point.y - prevPoint.y
                    centerCursor hwnd
                    events.OnMouseMoved(int point.x, int point.y, int xrel, int yrel)
                | _ ->
                    ()

            | x when int x = WM_LBUTTONDOWN ->
                events.OnMouseButtonPressed MouseButtonType.Left

            | x when int x = WM_LBUTTONUP ->
                events.OnMouseButtonReleased MouseButtonType.Left

            | x when int x = WM_MBUTTONDOWN ->
                events.OnMouseButtonPressed MouseButtonType.Middle

            | x when int x = WM_MBUTTONUP ->
                events.OnMouseButtonReleased MouseButtonType.Middle

            | x when int x = WM_RBUTTONDOWN ->
                events.OnMouseButtonPressed MouseButtonType.Right

            | x when int x = WM_RBUTTONUP ->
                events.OnMouseButtonReleased MouseButtonType.Right

            | x when int x = WM_QUIT ->
                CloseWindow(hwnd) |> ignore

            | _ -> ()

    member private __.WndProc hWnd msg wParam lParam =
        hideCursor hWnd
        match int msg with
        | x when x = WM_SYSCOMMAND ->
            match int wParam with
            | x when x = SC_KEYMENU -> nativeint 0
            | x when x = SC_CLOSE -> 
                events.OnClosing()
                DefWindowProc(hWnd, msg, wParam, lParam)
            | _ ->
                DefWindowProc(hWnd, msg, wParam, lParam)
        | x when x = WM_SIZE ->
            let mutable rect = Unchecked.defaultof<RECT>
            GetWindowRect(hWnd, &&rect) |> ignore
            events.OnChanged(int rect.left - int rect.right, int rect.top - int rect.bottom, int rect.top, int rect.left)
            DefWindowProc(hWnd, msg, wParam, lParam)
        | _ ->
            DefWindowProc(hWnd, msg, wParam, lParam)

    // Store this so it doesn't get collected cause a System.ExecutionEngineException.
    member private _.WndProcDelegate = del

    member __.Closing = closing.Publish

    member __.Resized = resized.Publish

    member __.Start() =
        del <- WndProcDelegate(this.WndProc)

        let hwnd2, hinstance2 = createWin32Window title del
        hwnd <- hwnd2
        hinstance <- hinstance2

        events.OnWin32Initialized(hwnd, hinstance)

        centerCursor hwnd
        prevPoint <- 
            match tryGetCursorPos hwnd with
            | ValueSome point -> point
            | _ -> POINT()

        GameLoop.start
            fixedUpdateInterval
            poll
            (fun ticks deltaTicks ->
                let time = TimeSpan.FromTicks ticks
                let deltaTime = TimeSpan.FromTicks deltaTicks

                events.OnFixedUpdate(time, deltaTime)
            )
            (fun ticks deltaTicks ->
                let time = TimeSpan.FromTicks ticks
                let deltaTime = TimeSpan.FromTicks deltaTicks

                events.OnUpdate(time, deltaTime)
            )

    member __.Hwnd = hwnd

    member __.Hinstance = hinstance