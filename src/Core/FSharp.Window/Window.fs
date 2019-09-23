module FSharp.Window

open System
open GameLoop

type MouseButtonType =
    | Left = 1
    | Middle = 2
    | Right = 3
    | X1 = 4
    | X2 = 5

type WindowEvent =
    | KeyPressed of char
    | KeyReleased of char
    | MouseButtonPressed of MouseButtonType
    | MouseButtonReleased of MouseButtonType
    | MouseWheelScrolled of 
        x: int * y: int
    | MouseMoved of
        x: int * y: int * xrel: int * yrel: int
    | WindowClosed

type IWindowEvents =

    abstract OnEvent: WindowEvent list -> unit

    abstract OnUpdateFrame: time: float * interval: float -> bool

    abstract OnRenderFrame: time: float * delta: float * width: int * height: int -> unit

type IWindowState =

    abstract ShowWindow: unit -> unit

    abstract PollInput: unit -> WindowEvent list

[<Sealed>]
type CreateStateArgs internal (title: string, width: int, height: int) =

    member __.Title = title

    member __.Width = width

    member __.Height = height

[<Sealed>]
type Window (title: string, updateInterval: float, width: int, height: int, events: IWindowEvents, state: IWindowState) =

    member __.Start () =
        GameLoop.start
            updateInterval
            (fun () -> events.OnEvent (state.PollInput ()))
            (fun ticks intervalTicks ->
                let time = TimeSpan.FromTicks ticks
                let interval = TimeSpan.FromTicks intervalTicks

                events.OnUpdateFrame (time.TotalMilliseconds, interval.TotalMilliseconds)
            )
            (fun ticks delta ->
                let time = TimeSpan.FromTicks ticks

                events.OnRenderFrame (time.TotalMilliseconds, delta, width, height)
            )
    
