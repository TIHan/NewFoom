// Learn more about F# at http://fsharp.org

open System
open System.IO
open System.Reflection
open FSharp.Data

type SpirvSpec = JsonProvider<"spirv.core.grammar.json">
let spec = SpirvSpec.Load "spirv.core.grammar.json"

let genInstructions () =
    ""

let genSource () =
    """// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System
open Specification
open Types
""" + "\n" + genInstructions ()

[<EntryPoint>]
let main argv =
    let src = genSource()

    let asm = Assembly.GetExecutingAssembly()
    let path1 = (Path.GetDirectoryName asm.Location) + """\..\..\..\..\FSharp.Spirv\SpirvGen.fs"""
    if File.Exists path1 then
        File.WriteAllText(path1, src)
        printfn "Success"
    else
        failwith "Path not found."
    0
