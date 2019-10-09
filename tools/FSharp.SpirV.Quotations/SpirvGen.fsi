[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.SpirvGen

open FSharp.Spirv
open Tast

val GenModule: SpirvGenInfo -> SpirvTopLevelExpr -> SpirvModuleOld
