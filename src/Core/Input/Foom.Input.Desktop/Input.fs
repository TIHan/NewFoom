namespace Foom.Input.Desktop

open System.Runtime.InteropServices
open Foom.Input

open OpenTK.Input

module Input =

    let inputEvents = ResizeArray<InputEvent> ()

    let dispatchKeyboardEvent (kbEvt: KeyboardEvent) : unit =
        inputEvents.Add (
            if kbEvt.IsPressed = 0 then 
                InputEvent.KeyReleased (char kbEvt.KeyCode) 
            else 
                InputEvent.KeyPressed (char kbEvt.KeyCode))

    let dispatchMouseButtonEvent (mbEvt: MouseButtonEvent) : unit =
        inputEvents.Add (
            if mbEvt.IsPressed = 0 then
                InputEvent.MouseButtonReleased (mbEvt.Button)
            else
                InputEvent.MouseButtonPressed (mbEvt.Button))

    let dispatchMouseWheelEvent (evt: MouseWheelEvent) : unit =
        inputEvents.Add (InputEvent.MouseWheelScrolled (evt.X, evt.Y))

    let dispatchMouseMoveEvent (evt: MouseMoveEvent) : unit =
        inputEvents.Add (InputEvent.MouseMoved (evt.X, evt.Y, evt.XRel, evt.YRel))

    let getMousePosition () : MousePosition =
        let state = OpenTK.Input.Mouse.GetState()
        MousePosition(state.X, state.Y, 0, 0)

    let getState () : InputState =
        let events = inputEvents |> List.ofSeq
        inputEvents.Clear ()
        { Events = events }

    let pollEvents (window: nativeint) = ()

type DesktopInput (window : nativeint) =

    interface IInput with

        member x.PollEvents () =
            ()

        member x.GetMousePosition () =
            Input.getMousePosition ()

        member x.GetState () =
            Input.getState ()