module Tests

open System
open System.IO
open Xunit
open FSharp.Spirv

[<Fact>]
let ``Deserialize triangle_vertex`` () =
    use file = File.OpenRead("triangle_vertex.spv")
    let spv = SpirvModuleOld.Deserialize file
    ()

[<Fact>]
let ``Deserialize triangle_fragment`` () =
    use file = File.OpenRead("triangle_fragment.spv")
    let spv = SpirvModule.Deserialize file
    ()

[<Fact>]
let ``Serialize triangle_vertex`` () =
    use file = File.OpenRead("triangle_vertex.spv")
    let spv = SpirvModule.Deserialize file

    let tmp = Path.GetTempFileName()
    try
        use file2 = File.Open(tmp, FileMode.OpenOrCreate)
        SpirvModule.Serialize (file2, spv)

        Assert.Equal(file.Length, file2.Length)

        file2.Position <- 0L

        let spv2 = SpirvModule.Deserialize file2
        ()
    finally
        try File.Delete tmp with | _ -> () 

[<Fact>]
let ``Serialize triangle_fragment`` () =
    use file = File.OpenRead("triangle_fragment.spv")
    let spv = SpirvModule.Deserialize file

    let tmp = Path.GetTempFileName()
    try
        use file2 = File.Open(tmp, FileMode.OpenOrCreate)
        SpirvModule.Serialize (file2, spv)

        file2.Position <- 0L

        let spv2 = SpirvModule.Deserialize file2
        ()
    finally
        try File.Delete tmp with | _ -> () 
