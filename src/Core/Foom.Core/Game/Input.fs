namespace Foom.Game.Input

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
    | MouseWheelScrolled of x: int * y: int
    | MouseMoved of x: int * y: int * xrel: int * yrel: int
