namespace Foom.Math

[<AbstractClass; Sealed>]
type Mathf =

    static member Clamp : value: float32 * min: float32 * max: float32 -> float32

    static member Hermite : value1: float32 * tangent1: float32 * value2: float32 * tangent2: float32 * amount: float32 -> float32

    static member SmoothStep : value1: float32 * value2: float32 * amount: float32 -> float32

    static member Lerp : x: float32 * y: float32 * t: float32 -> float32