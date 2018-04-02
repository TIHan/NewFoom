namespace Foom.Math

[<AbstractClass; Sealed>]
type Vector2 private () =

    static member inline PerpDot (value1: System.Numerics.Vector2, value2: System.Numerics.Vector2) =
        value1.X * value2.Y - value1.Y * value2.X
