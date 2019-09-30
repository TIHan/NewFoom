[<AutoOpen>]
module FSharp.SpirV.Quotations.Intrinsics

let input<'T> = Unchecked.defaultof<'T>
let output<'T> = ref Unchecked.defaultof<'T>