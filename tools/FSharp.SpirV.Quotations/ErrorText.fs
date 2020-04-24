[<RequireQualifiedAccess>]
module internal FSharp.Spirv.Quotations.ErrorText

open FSharp.Quotations

let QuotationExpressionNotSupportedAtTopLevel (expr: Expr) = sprintf "Quotation expression not supported at top-level: %A" expr