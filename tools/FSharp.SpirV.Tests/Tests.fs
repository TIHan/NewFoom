module Tests

open System
open System.IO
open Xunit
open FSharp.SpirV

[<Fact>]
let ``Deserialize triangle_vertex`` () =
    use file = File.OpenRead("triangle_vertex.spv")
    let spv = SpirV.deserialize file
    Assert.Equal(Specification.MagicNumber, spv.MagicNumber)

[<Fact>]
let ``Deserialize triangle_fragment`` () =
    use file = File.OpenRead("triangle_fragment.spv")
    let spv = SpirV.deserialize file
    Assert.Equal(Specification.MagicNumber, spv.MagicNumber)
