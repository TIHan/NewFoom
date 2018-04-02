namespace Foom.Math

[<AbstractClass; Sealed>]
type Math private () =

    // http://stackoverflow.com/questions/3407012/c-rounding-up-to-the-nearest-multiple-of-a-number
    static member RoundUp(numToRound, multiple) =
        if (multiple = 0) then 
            numToRound
        else
            let remainder = abs(numToRound) % multiple;
            if (remainder = 0) then 
                numToRound
            else

                if (numToRound < 0) then
                    -(abs(numToRound) - remainder)
                else
                    numToRound + multiple - remainder

    static member inline RoundDown(numToRound, multiple) =
        Math.RoundUp(numToRound, multiple) - multiple