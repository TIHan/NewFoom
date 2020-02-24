[<AutoOpen>]
module FSharp.Spirv.Quotations.Intrinsics

open System
open System.Numerics
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Spirv

[<Literal>]
let private ErrorMessage = "Do not call outside the code quotation."

[<RequiresExplicitTypeArguments>]
let Variable<'T> (_decorations: Decoration list) (_storageClass: StorageClass) : 'T = failwith ErrorMessage

[<Struct;NoComparison;NoEquality>]
type Sampler2d = private Sampler2d of unit


