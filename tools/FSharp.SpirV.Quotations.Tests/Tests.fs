module Tests

open System.Numerics
open FSharp.SpirV
open FSharp.SpirV.Specification
open FSharp.SpirV.Quotations
open Xunit

[<Fact>]
let ``Compiler Fragment`` () =
    let fragment = 
        <@ 
            let fragColor = input<Vector3>
            let outColor = output<Vector4>
    
            outColor := Vector4(fragColor, 1.f)
        @>

    let spv = SPVGen.GenFragment fragment
    ()
