module FSharp.Window

open System
open GameLoop

type MouseButtonType =
    | Left = 1
    | Middle = 2
    | Right = 3
    | X1 = 4
    | X2 = 5

type IWindowEvents =

    abstract OnWin32Initialized: hwnd: nativeint * hinstance: nativeint -> unit

    abstract OnClosing: unit -> unit

    abstract OnChanged: width: int * height: int * x: int * y: int -> unit

    abstract OnKeyPressed: char -> unit

    abstract OnKeyReleased: char -> unit

    abstract OnMouseButtonPressed: MouseButtonType -> unit

    abstract OnMouseButtonReleased: MouseButtonType -> unit

    abstract OnMouseWheelScrolled: x: int * y: int -> unit

    abstract OnMouseMoved: x: int * y: int * xrel: int * yrel: int -> unit

    abstract OnFixedUpdate: time: TimeSpan * deltaTime: TimeSpan -> bool

    abstract OnUpdate: time: TimeSpan * deltaTime: TimeSpan -> bool