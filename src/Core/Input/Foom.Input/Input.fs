namespace Foom.Input

type MouseButtonType =
    | Left = 1
    | Middle = 2
    | Right = 3
    | X1 = 4
    | X2 = 5

type InputEvent =
    | KeyPressed of char
    | KeyReleased of char
    | MouseButtonPressed of MouseButtonType
    | MouseButtonReleased of MouseButtonType
    | MouseWheelScrolled of 
        x: int * y: int
    | MouseMoved of
        x: int * y: int * xrel: int * yrel: int

[<RequireQualifiedAccess>]
type InputState = { Events: InputEvent list }

[<Struct>]
type KeyboardEvent =
    val IsPressed : int
    val KeyCode : int

[<Struct>]
type MouseButtonEvent =
    val IsPressed : int
    val Clicks : int
    val Button : MouseButtonType
    val X : int
    val Y : int

[<Struct>]
type MouseWheelEvent =
    val X : int
    val Y : int

[<Struct>]
type MouseMoveEvent =
    val X : int
    val Y : int
    val XRel : int
    val YRel : int

[<Struct>]
type MousePosition =
    val X : int
    val Y : int
    val XRel : int
    val YRel : int

type IInput =

    abstract PollEvents : unit -> unit

    abstract GetMousePosition : unit -> MousePosition

    abstract GetState : unit -> InputState        