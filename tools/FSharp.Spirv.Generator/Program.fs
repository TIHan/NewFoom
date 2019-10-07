open System
open System.IO
open System.Reflection
open FSharp.Data

type SpirvSpec = JsonProvider<"spirv.core.grammar.json">
let spec = SpirvSpec.Load "spirv.core.grammar.json"

let genKind (kind: SpirvSpec.OperandKind) =
    let comment =
        match kind.Doc with
        | Some doc -> """/// """ + doc + "\n"
        | _ -> String.Empty

    comment +
    match kind.Category with
    | "Id" ->
        "type " + kind.Kind + " = uint32\n"
    | "Literal" ->
        let tyName =
            match kind.Kind with
            | "LiteralString" -> "string"
            | "LiteralExtInstInteger" -> "uint32"
            | "LiteralSpecConstantOpInteger" -> "uint32"
            | _ -> "uint32 list"
        "type " + kind.Kind + " = " + tyName + "\n"
    | "ValueEnum" ->
        "type " + kind.Kind + " =\n" +
        (kind.Enumerants
            |> Array.map (fun case ->
            let name =
                match case.Enumerant with
                | "1D" -> "One"
                | "2D" -> "Two"
                | "3D" -> "Three"
                | _ -> case.Enumerant
            "   | " + name + " = " + if case.Value.Number.IsSome then string case.Value.Number.Value else case.Value.String.Value)
            |> Array.reduce (fun case1 case2 -> case1 + "\n" + case2)) + "\n"
    | "BitEnum" ->
        "type " + kind.Kind + " =\n" +
        (kind.Enumerants
            |> Array.map (fun case ->
            "   | " + case.Enumerant + " = " + case.Value.String.Value)
            |> Array.reduce (fun case1 case2 -> case1 + "\n" + case2)) + "\n"
    | "Composite" ->
        "type " + kind.Kind + " = " + kind.Kind + " of " + (kind.Bases |> Array.reduce (fun x y -> x + " * " + y)) + "\n"
    | _ ->
        String.Empty
    
let genKinds () =
    spec.OperandKinds
    |> Array.map genKind
    |> Array.filter (fun x -> not (String.IsNullOrWhiteSpace x))
    |> Array.reduce (fun x y -> x + "\n" + y)

let genOperand (operand: SpirvSpec.Operand) =
    operand.Kind

let genInstruction (instr: SpirvSpec.Instruction) =
    "   | " + instr.Opname +
    if not (Array.isEmpty instr.Operands) then
        " of " +
        (instr.Operands
         |> Array.map genOperand
         |> Array.reduce (fun x y -> x + " * " + y))
    else
        String.Empty

let genInstructionMemberOpcodeCase (instr: SpirvSpec.Instruction) =
    let underscore () =
        if Array.isEmpty instr.Operands then
            String.Empty
        else
            " _"
    "       | " + instr.Opname + underscore () + " -> " + string instr.Opcode + "u"

let genInstructionMemberOpcodeMember () =
    "   member x.Opcode =
       match x with\n" +
    (spec.Instructions
     |> Array.map genInstructionMemberOpcodeCase
     |> Array.reduce (fun x y -> x + "\n" + y))

let genInstructions () =
    """type Instruction =
""" +
    (spec.Instructions
     |> Array.map genInstruction
     |> Array.reduce (fun x y -> x + "\n" + y)) + "\n\n" +
    genInstructionMemberOpcodeMember ()

let genSource () =
    """// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System
""" + "\n" + 
    genKinds () + "\n" +
    genInstructions ()

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
