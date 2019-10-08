open System
open System.IO
open System.Reflection
open System.Collections.Generic
open FSharp.Data

type SpirvSpec = JsonProvider<"spirv.core.grammar.json">
let spec = SpirvSpec.Load "spirv.core.grammar.json"

[<RequireQualifiedAccess>]
type OperandType =
    | UInt32
    | String
    | Composite of name: string * OperandType list
    | Enum of name: string
    | Option of OperandType
    | List of OperandType

    member x.Name =
        match x with
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
    "       | " + instr.Opname + underscore () + " -> " + string instr.Opcode + "u"

let genInstructionMemberOpcodeMember () =
    "    member x.Opcode =
       match x with\n" +
    (spec.Instructions
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
    (spec.Instructions
     |> Array.map genInstructionMemberVersionCase
     |> Array.reduce (fun x y -> x + "\n" + y))

let genCaseArgsMatch argName count =
    "(" + (Array.init count (fun i -> argName + string i) |> Array.reduce (fun x y -> x + ", " + y)) + ")"

let rec genSerailizeType arg (ty: OperandType) =
    match ty with
    | OperandType.UInt32 -> "stream.WriteUInt32(" + arg + ")"
    | OperandType.String -> "stream.WriteString(" + arg + ")"
    | OperandType.Enum _ -> "stream.WriteEnum(" + arg + ")"
    | OperandType.Composite (name, bases) ->
        "match " + arg + " with " + name + genCaseArgsMatch (arg + "_") bases.Length + " -> " +
        (bases |> List.mapi (fun i x -> genSerailizeType (arg + "_" + string i) x) |> List.reduce (fun x y -> x + ";" + y))
    | OperandType.Option ty ->
        "stream.WriteOption(" + arg + ", fun v -> " + genSerailizeType "v" ty + ")"
    | OperandType.List ty ->
        "stream.WriteList(" + arg + ", fun v -> " + genSerailizeType "v" ty + ")" 

let genSerializeInstruction (instr: SpirvSpec.Instruction) =
    "    | " + instr.Opname + 
    (match instr.Operands with [||] -> String.Empty | operands -> genCaseArgsMatch "arg" operands.Length) +
    " ->\n" +

    match instr.Operands with
    | [||] -> "            ()"
    | operands ->
        operands
        |> Array.mapi (fun i x -> "            " + genSerailizeType ("arg" + string i) (getOperandType x))
        |> Array.reduce (fun x y -> x + "\n" + y)

let genSerializeInstructions () =
    "    static member Serialize(instr: Instruction, stream: SpirvStream) =
        stream.WriteUInt32(instr.Opcode)
        match instr with\n" +
        (spec.Instructions
         |> Array.map (fun x ->
         "    " + genSerializeInstruction x
         )
         |> Array.reduce (fun x y -> x + "\n" + y))

let genInstructions () =
    """type Instruction =
""" +
    (spec.Instructions
     |> Array.map genInstruction
     |> Array.reduce (fun x y -> x + "\n" + y)) + "\n\n" +
    genInstructionMemberOpcodeMember () + "\n\n" +
    genInstructionMemberVersionMember () + "\n\n" +
    genSerializeInstructions ()

let genSource () =
    """// File is generated. Do not modify.
module FSharp.Spirv.GeneratedSpec

open System
open System.IO

type SpirvStream =
    {
        stream: Stream
        mutable remaining: int
        buffer128: byte []
    }

    member x.WriteUInt32 (v: uint32) = ()

    member x.WriteString (v: string) = ()

    member x.WriteEnum<'T when 'T : enum<uint32>> (v: 'T) = ()

    member x.WriteOption (v: 'T option, f: 'T -> unit) = ()

    member x.WriteList (v: 'T list, f: 'T -> unit) = ()
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
