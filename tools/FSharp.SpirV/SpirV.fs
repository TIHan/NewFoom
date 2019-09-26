namespace FSharp.SpirV

open Pickle

[<RequireQualifiedAccess>]
module SpirV =

    let deserialize stream =
        let stream =
            {
                stream = stream
                remaining = 0
                buffer128 = Array.zeroCreate 128
                isReader = true
            }
        Module.read stream

    let serialize stream spvModule =
        let stream =
            {
                stream = stream
                remaining = 0
                buffer128 = Array.zeroCreate 128
                isReader = false
            }
        Module.write stream spvModule
