module FSharp.Window

open System
open GameLoop

type MouseButtonType =
    | Left = 1
    | Middle = 2
    | Right = 3
    | X1 = 4
    | X2 = 5

[<AbstractClass>]
type AbstractWindow() =

    abstract OnInitialized: unit -> unit
    default _.OnInitialized() = ()

    abstract OnClosing: unit -> unit
    default _.OnClosing() = ()

    abstract OnSized: width: int * height: int -> unit
    default _.OnSized(_, _) = ()

    abstract OnMoved: x: int * y: int -> unit
    default _.OnMoved(_, _) = ()

    abstract OnKeyPressed: char -> unit
    default _.OnKeyPressed _ = ()

    abstract OnKeyReleased: char -> unit
    default _.OnKeyReleased _ = ()

    abstract OnMouseButtonPressed: MouseButtonType -> unit
    default _.OnMouseButtonPressed _ = ()

    abstract OnMouseButtonReleased: MouseButtonType -> unit
    default _.OnMouseButtonReleased _ = ()

    abstract OnMouseWheelScrolled: x: int * y: int -> unit
    default _.OnMouseWheelScrolled(_, _) = ()

    abstract OnMouseMoved: x: int * y: int * xrel: int * yrel: int -> unit
    default _.OnMouseMoved(_, _, _, _) = ()

    abstract OnFixedUpdate: time: TimeSpan * deltaTime: TimeSpan -> bool
    default _.OnFixedUpdate(_, _) = false

    abstract OnUpdate: time: TimeSpan * deltaTime: TimeSpan -> bool
    default _.OnUpdate(_, _) = false

    abstract HideCursor: unit -> unit
    default _.HideCursor() = ()

    abstract ShowCursor: unit -> unit
    default _.ShowCursor() = ()

    abstract ClipCursor: unit -> unit
    default _.ClipCursor() = ()

    abstract UnclipCursor: unit -> unit
    default _.UnclipCursor() = ()