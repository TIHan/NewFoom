module Foom.Win32

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop
open Foom.Game
open Foom.Win32Internal

#nowarn "9"

[<Sealed>]
type Win32Game (windowTitle: string, svGame: AbstractServerGame, clGame: AbstractClientGame, interval) =

    member __.Start() =
        use title = fixed (System.Text.Encoding.UTF8.GetBytes(windowTitle))
        let title = title |> NativePtr.toNativeInt |> NativePtr.ofNativeInt
        let wndProc = WndProcDelegate(fun hWnd msg wParam lParam -> 
                DefWindowProc(hWnd, msg, wParam, lParam)
        )
        let hInstance = Marshal.GetHINSTANCE(typeof<WNDCLASSEXW>.Module)

        let mutable windowClass = Unchecked.defaultof<WNDCLASSEXW>
        windowClass.cbSize <- uint32 sizeof<WNDCLASSEXW>
        windowClass.style <- uint32((* CS_HREDRAW *) 0x0002 ||| (* CS_VREDRAW *) 0x0001)
        windowClass.lpfnWndProc <- Marshal.GetFunctionPointerForDelegate(wndProc)
        windowClass.hInstance <- hInstance
        windowClass.lpszClassName <- title
        windowClass.hCursor <- LoadCursor(IntPtr.Zero, (* IDC_ARROW *)32512)

        if (RegisterClassExW(&windowClass) = 0) then
            printfn "Failed Registering Window Class"
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
        else

        printfn "- Created Window"

        printfn "ShowWindow Result: %A" <| ShowWindow(hwnd, 1)
        printfn "UpdateWindow Result: %A" <| UpdateWindow(hwnd)

        let mutable msg = Unchecked.defaultof<MSG>
        let mutable gl = GameLoop.create interval
        while not gl.WillQuit do
            while PeekMessage(&&msg, nativeint 0, 0u, 0u, 0x0001u) <> 0uy do
                TranslateMessage(&msg) |> ignore
                DispatchMessage(&msg) |> ignore

            gl <- GameLoop.tick
                    id
                    (fun time interval ->
                        let time = TimeSpan.FromTicks(time)
                        let interval = TimeSpan.FromTicks(interval)

                        clGame.PreUpdate(time, interval, [])
                        let svWillQuit = svGame.Update(time, interval)
                        let clWillQuit = clGame.Update(time, interval, [])

                        svWillQuit || clWillQuit
                    )
                    (fun time deltaTime ->
                        let time = TimeSpan.FromTicks(time)

                        clGame.Render(time, deltaTime)
                    )
                    gl