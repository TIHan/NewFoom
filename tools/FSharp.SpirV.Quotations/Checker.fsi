[<RequireQualifiedAccess>]
module FSharp.Spirv.Quotations.Checker

open FSharp.Quotations
open TypedTree

val CheckVariable: Var -> Expr list -> SpirvVar * Expr list

val Check: Expr -> SpirvTopLevelExpr