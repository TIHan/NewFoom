[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.SpirvGen

open FSharp.Spirv
open TypedTree

val GenModule: SpirvGenInfo -> SpirvTopLevelExpr -> SpirvModule
