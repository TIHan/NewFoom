open System
open System.IO
open System.Reflection
open System.Collections.Generic
open FSharp.Data

type SpirvSpec = JsonProvider<"spirv.core.grammar.json">
let spec = SpirvSpec.Load "spirv.core.grammar.json"

let instructions =
    spec.Instructions
    |> Array.distinctBy (fun x -> x.Opcode)

[<RequireQualifiedAccess>]
type OperandType =
    | UInt16
    | UInt32
    | String
    | Composite of name: string * OperandType list
    | Enum of name: string
    | Option of OperandType
    | List of OperandType

    member x.Name =
        match x with
        | UInt16 -> "uint16"
        | UInt32 -> "uint32"
        | String -> "string"
        | Composite (name, _) -> name
        | Enum name -> name
        | Option ty -> ty.Name + " option"
        | List ty -> ty.Name + " list"

let typeLookup = Dictionary<string, OperandType>()

let rec getType (name: string) (category: string) (bases: string []) =
    match category with
    | "Id" -> OperandType.UInt32
    | "Literal" ->
        match name with
        | "LiteralString" ->
            OperandType.String
        | _ ->
            OperandType.UInt32
    | "ValueEnum" ->
        OperandType.Enum name
    | "BitEnum" ->
        OperandType.Enum name
    | "Composite" ->
        OperandType.Composite (name, bases |> List.ofArray |> List.map (fun x -> getType String.Empty x [||]))
    | _ ->
        match name with
        | "Opcode" ->
            OperandType.UInt16
        | _ ->
            OperandType.UInt32

let genKind (kind: SpirvSpec.OperandKind) =
    let comment =
        match kind.Doc with
        | Some doc -> """/// """ + doc + "\n"
        | _ -> String.Empty

    typeLookup.[kind.Kind] <- getType kind.Kind kind.Category kind.Bases

    comment +
    match kind.Category with
    | "Id" ->
        "type " + kind.Kind + " = uint32\n"
    | "Literal" ->
        let tyName =
            match kind.Kind with
            | "LiteralString" -> 
                "string"
            | _ -> 
                "uint32"
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
            "   | " + name + " = " + (if case.Value.Number.IsSome then string case.Value.Number.Value else case.Value.String.Value) + "u")
            |> Array.reduce (fun case1 case2 -> case1 + "\n" + case2)) + "\n"
    | "BitEnum" ->
        "type " + kind.Kind + " =\n" +
        (kind.Enumerants
            |> Array.map (fun case ->
            "   | " + case.Enumerant + " = " + case.Value.String.Value + "u")
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

let getOperandType (operand: SpirvSpec.Operand) =
    let ty = typeLookup.[operand.Kind]
    match operand.Quantifier with
    | Some "?" -> OperandType.Option ty
    | Some "*" -> OperandType.List ty
    | _ -> ty

let genOperand (operand: SpirvSpec.Operand) =
    let name =
        match operand.Name with
        | Some name ->
            let results = name.Split(''')
            if results.Length >= 2 then
                results.[1].Replace(" ", "").Replace("~", "").Replace(",","").Replace(".","").Replace(">","") + ": "
            else
                String.Empty
        | _ ->
            String.Empty
    name + (getOperandType operand).Name

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
    "       | " + instr.Opname + underscore () + " -> " + string instr.Opcode + "us"

let genInstructionMemberOpcodeMember () =
    "    member x.Opcode =
       match x with\n" +
    (instructions
     |> Array.map genInstructionMemberOpcodeCase
     |> Array.reduce (fun x y -> x + "\n" + y))

let genInstructionMemberVersionCase (instr: SpirvSpec.Instruction) =
    let underscore () =
        if Array.isEmpty instr.Operands then
            String.Empty
        else
            " _"
    "       | " + instr.Opname + underscore () + " -> " + (match instr.Version.Number with None -> "1.0m" | Some version -> string version + "m")

let genInstructionMemberVersionMember () =
    "    member x.Version =
       match x with\n" +
    (instructions
     |> Array.map genInstructionMemberVersionCase
     |> Array.reduce (fun x y -> x + "\n" + y))

let genCaseArgsMatch argName count =
    "(" + (Array.init count (fun i -> argName + string i) |> Array.reduce (fun x y -> x + ", " + y)) + ")"

let rec genSerializeType arg (ty: OperandType) =
    match ty with
    | OperandType.UInt16 -> "stream.WriteUInt16(" + arg + ")"
    | OperandType.UInt32 -> "stream.WriteUInt32(" + arg + ")"
    | OperandType.String -> "stream.WriteString(" + arg + ")"
    | OperandType.Enum _ -> "stream.WriteEnum(" + arg + ")"
    | OperandType.Composite (name, bases) ->
        "match " + arg + " with " + name + genCaseArgsMatch (arg + "_") bases.Length + " -> " +
        (bases |> List.mapi (fun i x -> genSerializeType (arg + "_" + string i) x) |> List.reduce (fun x y -> x + ";" + y))
    | OperandType.Option ty ->
        "stream.WriteOption(" + arg + ", fun v -> " + genSerializeType "v" ty + ")"
    | OperandType.List ty ->
        "stream.WriteList(" + arg + ", fun v -> " + genSerializeType "v" ty + ")"
        
let rec genDeserializeType (ty: OperandType) =
    match ty with
    | OperandType.UInt16 -> "stream.ReadUInt16()"
    | OperandType.UInt32 -> "stream.ReadUInt32()"
    | OperandType.String -> "stream.ReadString()"
    | OperandType.Enum _ -> "stream.ReadEnum()"
    | OperandType.Composite (name, bases) ->
        name + "(" + (bases |> List.map (fun x -> genDeserializeType x) |> List.reduce (fun x y -> x + ", " + y)) + ")"
    | OperandType.Option ty ->
        "stream.ReadOption(fun () -> " + genDeserializeType ty + ")"
    | OperandType.List ty ->
        "stream.ReadList(fun () -> " + genDeserializeType ty + ")" 

let genSerializeInstruction (instr: SpirvSpec.Instruction) =
    "    | " + instr.Opname + 
    (match instr.Operands with [||] -> String.Empty | operands -> genCaseArgsMatch "arg" operands.Length) +
    " ->\n" +

    match instr.Operands with
    | [||] -> "            ()"
    | operands ->
        operands
        |> Array.mapi (fun i x -> "            " + genSerializeType ("arg" + string i) (getOperandType x))
        |> Array.reduce (fun x y -> x + "\n" + y)

let genDeserializeInstruction (instr: SpirvSpec.Instruction) =
    "    | " + string instr.Opcode + "us" + " ->\n" +

    "            " +
    match instr.Operands with
    | [||] -> instr.Opname
    | operands ->
        instr.Opname + "(" +
        (operands
         |> Array.map (fun x -> genDeserializeType (getOperandType x))
         |> Array.reduce (fun x y -> x + ", " + y)) + ")"

let genSerializeInstructions () =
    "    static member internal Serialize(instr: Instruction, stream: SpirvStream) =
        match instr with\n" +
        (instructions
         |> Array.map (fun x ->
         "    " + genSerializeInstruction x
         )
         |> Array.reduce (fun x y -> x + "\n" + y))

let genDeserializeInstructions () =
    "    static member internal Deserialize(opcode: uint16, stream: SpirvStream) =
        match opcode with\n" +
        (instructions
         |> Array.map (fun x ->
         "    " + genDeserializeInstruction x
         )
         |> Array.reduce (fun x y -> x + "\n" + y)) +
         """        | _ -> failwith "invalid opcode" """

let genInstructions () =
    """type Instruction =
""" +
    (instructions
     |> Array.map genInstruction
     |> Array.reduce (fun x y -> x + "\n" + y)) + "\n\n" +
    genInstructionMemberOpcodeMember () + "\n\n" +
    genInstructionMemberVersionMember () + "\n\n" +
    genSerializeInstructions () + "\n\n" +
    genDeserializeInstructions ()

let genSource () =
    """// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System
open System.IO
open InternalHelpers

// https://github.com/KhronosGroup/SPIRV-Headers/blob/master/include/spirv/unified1/spirv.core.grammar.json
""" + "\n" + 
    "[<Literal>] 
let MagicNumber = " + string spec.MagicNumber + "u\n\n" +
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
