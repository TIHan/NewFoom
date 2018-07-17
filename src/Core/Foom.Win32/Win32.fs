module Foom.Win32

open System
open System.Runtime.InteropServices
open FSharp.NativeInterop

open Foom.Win32Internal

#nowarn "9"

[<NoEquality;NoComparison>]
type Window = Window of nativeint with

    member this.Update() =
        match this with
        | Window(hWnd) -> UpdateWindow(hWnd) |> ignore

module Win32 =

    let CreateWindow (title: string) =
        use title = fixed (System.Text.Encoding.UTF8.GetBytes(title))
        let title = title |> NativePtr.toNativeInt |> NativePtr.ofNativeInt
        let wndProc = WndProcDelegate(fun hWnd msg wParam lParam -> 
            //match int msg with
            //| msg when msg = WM_PAINT -> nativeint 0
            //| _ ->
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
            Error(GetLastError())
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
            Error(GetLastError())
        else

        printfn "- Created Window"

        printfn "ShowWindow Result: %A" <| ShowWindow(hwnd, 1)
        printfn "UpdateWindow Result: %A" <| UpdateWindow(hwnd)

        Ok(Window(hwnd))