module internal FSharp.Spirv.Quotations.Checker

open FSharp.Quotations
open ErrorLogger
open TypedTree

val CheckVariable: Var -> Expr list -> SpirvVar * Expr list

val Check: Expr -> SpirvTopLevelExpr * Error list