[<AutoOpen>]
module FSharp.Spirv.Extensions

open Pickle

type SpirvModuleOld with

    static member Deserialize stream =
        let stream =
            {
                stream = stream
                remaining = 0
                buffer128 = Array.zeroCreate 128
                isReader = true
            }
        Module.read stream

    static member Serialize (stream, spvModule) =
        let stream =
            {
                stream = stream
                remaining = 0
                buffer128 = Array.zeroCreate 128
                isReader = false
            }
        Module.write stream spvModule
