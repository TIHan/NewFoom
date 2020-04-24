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

    ShowWindow(hwnd, 1) |> ignore
    UpdateWindow(hwnd) |> ignore
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
    ShowCursor(0uy) |> ignore

let private showCursor hWnd =
    let mutable rect = Unchecked.defaultof<RECT>
    GetWindowRect(hWnd, &&rect) |> ignore
    ShowCursor(1uy) |> ignore

let private clipCursor hWnd =
    let mutable rect = Unchecked.defaultof<RECT>
    GetWindowRect(hWnd, &&rect) |> ignore
    ClipCursor(&&rect) |> ignore

let private unclipCursor () =
    ClipCursor(nativeint 0 |> NativePtr.ofNativeInt) |> ignore

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

type Win32Window(title: string, fixedUpdateInterval: float) as this =
    inherit AbstractWindow()

    let mutable del = Unchecked.defaultof<_>
    let mutable hwnd = nativeint 0
    let mutable hinstance = nativeint 0

    let mutable isCursorHidden = false
    let mutable isCursorClipped = false

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
                    this.OnKeyPressed(char msg.lParam)

            | x when x = WM_KEYUP || x = WM_SYSKEYUP ->
                if hashKey.Remove(char msg.lParam) then
                    this.OnKeyReleased(char msg.lParam)

            | x when int x = WM_MOUSEMOVE ->
                match tryGetCursorPos hwnd with
                | ValueSome point ->
                    let xrel = point.x - prevPoint.x
                    let yrel = point.y - prevPoint.y
                    if isCursorHidden then
                        centerCursor hwnd
                    this.OnMouseMoved(int point.x, int point.y, int xrel, int yrel)
                | _ ->
                    ()

            | x when int x = WM_LBUTTONDOWN ->
                this.OnMouseButtonPressed MouseButtonType.Left

            | x when int x = WM_LBUTTONUP ->
                this.OnMouseButtonReleased MouseButtonType.Left

            | x when int x = WM_MBUTTONDOWN ->
                this.OnMouseButtonPressed MouseButtonType.Middle

            | x when int x = WM_MBUTTONUP ->
                this.OnMouseButtonReleased MouseButtonType.Middle

            | x when int x = WM_RBUTTONDOWN ->
                this.OnMouseButtonPressed MouseButtonType.Right

            | x when int x = WM_RBUTTONUP ->
                this.OnMouseButtonReleased MouseButtonType.Right

            | x when int x = WM_QUIT ->
                CloseWindow(hwnd) |> ignore

            | _ -> ()

    member private __.WndProc hWnd msg wParam lParam =
        if isCursorHidden then
            hideCursor hWnd
        if isCursorClipped then
            clipCursor hWnd
        match int msg with
        | x when x = WM_SYSCOMMAND ->
            match int wParam with
            | x when x = SC_KEYMENU -> nativeint 0
            | x when x = SC_CLOSE -> 
                this.OnClosing()
                DefWindowProc(hWnd, msg, wParam, lParam)
            | _ ->
                DefWindowProc(hWnd, msg, wParam, lParam)
        | x when x = WM_SIZE ->
            let mutable rect = Unchecked.defaultof<RECT>
            GetWindowRect(hWnd, &&rect) |> ignore
            this.OnSized(int rect.right - int rect.left, int rect.bottom - int rect.top)
            DefWindowProc(hWnd, msg, wParam, lParam)
        | x when x = WM_MOVE ->
            let mutable rect = Unchecked.defaultof<RECT>
            GetWindowRect(hWnd, &&rect) |> ignore
            this.OnMoved(int rect.left, int rect.top)
            DefWindowProc(hWnd, msg, wParam, lParam)
        | _ ->
            DefWindowProc(hWnd, msg, wParam, lParam)

    // Store this so it doesn't get collected cause a System.ExecutionEngineException.
    member private _.WndProcDelegate = del

    override _.OnInitialized() =
        del <- WndProcDelegate(this.WndProc)

        let hwnd2, hinstance2 = createWin32Window title del
        hwnd <- hwnd2
        hinstance <- hinstance2

        prevPoint <- 
            match tryGetCursorPos hwnd with
            | ValueSome point -> point
            | _ -> POINT()

    override _.ShowCursor() =
        if isCursorHidden then
            isCursorHidden <- false
            showCursor hwnd

    override _.HideCursor() =
        if not isCursorHidden then
            isCursorHidden <- true
            hideCursor hwnd

    override _.ClipCursor() =
        if not isCursorClipped then
            isCursorClipped <- true
            clipCursor hwnd

    override _.UnclipCursor() =
        if isCursorClipped then
            isCursorClipped <- false
            unclipCursor ()

    member this.Start() =
        this.OnInitialized()
        GameLoop.start
            fixedUpdateInterval
            poll
            (fun ticks deltaTicks ->
                let time = TimeSpan.FromTicks ticks
                let deltaTime = TimeSpan.FromTicks deltaTicks

                this.OnFixedUpdate(time, deltaTime)
            )
            (fun ticks deltaTicks ->
                let time = TimeSpan.FromTicks ticks
                let deltaTime = TimeSpan.FromTicks deltaTicks

                this.OnUpdate(time, deltaTime)
            )

    member __.Hwnd = hwnd

    member __.Hinstance = hinstance

let SetConsolePosition(x: int, y: int) =
    let ptr = GetConsoleWindow()
    SetWindowPos(ptr, nativeint 0, x, y, 0, 0, 0x0001 ||| 0x0010) |> ignore