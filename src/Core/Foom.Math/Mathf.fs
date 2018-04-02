namespace Foom.Math

[<AbstractClass; Sealed>]
type Mathf private () =

    static member Clamp (value: float32, min, max) =
        if value < min then min
        elif value > max then max
        else value

    // Thank You MonoGame
    static member Hermite (value1: float32, tangent1: float32, value2: float32, tangent2: float32, amount: float32) =
        // All transformed to double not to lose precission
        // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
        let v1 = double value1
        let v2 = double value2
        let t1 = double tangent1
        let t2 = double tangent2
        let s = double amount
        let mutable result = 0.

        let sCubed = s * s * s;
        let sSquared = s * s;

        if (amount = 0.f) then
            result <- double value1
        elif (amount = 1.f) then
            result <- double value2
        else
            result <- (2. * v1 - 2. * v2 + t2 + t1) * sCubed +
                (3. * v2 - 3. * v1 - 2. * t1 - t2) * sSquared +
                t1 * s +
                v1
        float32 result

    static member SmoothStep (value1: float32, value2, amount) =
        // It is expected that 0 < amount < 1
        // If amount < 0, return value1
        // If amount > 1, return value2
        let result = Mathf.Clamp(amount, 0.f, 1.f)
        Mathf.Hermite(value1, 0.f, value2, 0.f, result)

    static member Lerp (x: float32, y, t) = x + (y - x) * t
