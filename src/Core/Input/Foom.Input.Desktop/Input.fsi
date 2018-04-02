namespace Foom.Input.Desktop

open Foom.Input

module Input =
    val private dispatchKeyboardEvent : KeyboardEvent -> unit
    val private dispatchMouseButtonEvent : MouseButtonEvent -> unit
    val private dispatchMouseWheelEvent : MouseWheelEvent -> unit
    val private dispatchMouseMoveEvent : MouseMoveEvent -> unit

    val pollEvents : window: nativeint -> unit
    val getMousePosition : unit -> MousePosition
    val getState : unit -> InputState

type DesktopInput =

    new : window : nativeint -> DesktopInput

    interface IInput