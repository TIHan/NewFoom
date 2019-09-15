namespace Foom.Input.Desktop

open Foom.Input

module Input =

    val pollEvents : window: nativeint -> unit
    val getMousePosition : unit -> MousePosition
    val getState : unit -> InputState

type DesktopInput =

    new : window : nativeint -> DesktopInput

    interface IInput