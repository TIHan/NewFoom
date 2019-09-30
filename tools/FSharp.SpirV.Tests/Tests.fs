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

[<Fact>]
let ``Serialize triangle_vertex`` () =
    use file = File.OpenRead("triangle_vertex.spv")
    let spv = SpirV.deserialize file

    let tmp = Path.GetTempFileName()
    try
        use file2 = File.Open(tmp, FileMode.OpenOrCreate)
        SpirV.serialize file2 spv

        Assert.Equal(file.Length, file2.Length)

        file2.Position <- 0L

        let spv2 = SpirV.deserialize file2
        Assert.Equal(spv.MagicNumber, spv2.MagicNumber)
    finally
        try File.Delete tmp with | _ -> () 

[<Fact>]
let ``Serialize triangle_fragment`` () =
    use file = File.OpenRead("triangle_fragment.spv")
    let spv = SpirV.deserialize file

    let tmp = Path.GetTempFileName()
    try
        use file2 = File.Open(tmp, FileMode.OpenOrCreate)
        SpirV.serialize file2 spv

        file2.Position <- 0L

        let spv2 = SpirV.deserialize file2
        Assert.Equal(spv.MagicNumber, spv2.MagicNumber)
    finally
        try File.Delete tmp with | _ -> () 
