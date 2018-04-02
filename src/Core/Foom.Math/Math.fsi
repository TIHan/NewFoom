namespace Foom.Math

[<AbstractClass; Sealed>]
type Math =

    static member RoundUp : numToRound: int * multiple: int -> int

    static member inline RoundDown : numToRound: int * multiple: int -> int