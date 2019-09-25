module Tests

open System
open System.IO
open Xunit
open FSharp.SpirV.Specification

[<Fact>]
let ``Deserialize triangle_vertex`` () =
    use file = File.OpenRead("triangle_vertex.spv")
    let spv = SPVModule.Deserialize(file)
    Assert.Equal(MagicNumber, spv.MagicNumber)
