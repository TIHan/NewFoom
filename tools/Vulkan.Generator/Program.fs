open System
open System.Xml
open System.Xml.Linq

open FSharp.Data

type VkEnumCase = VkEnumCase of name: string * value: int * comment: string option

type VkMember = VkMember of name: string * typeName: string * isMutable: bool * comment: string option

type VkUnionCase = VkUnionCase of name: string * typeName: string * count: int * isMutable: bool * comment: string option

type VkTypeKind = 
    | VkEnum of cases: VkEnumCase list
    | VkFlags of cases: VkEnumCase list
    | VkAlias of targetName: string
    | VkStruct of members: VkMember list * extendedTypes: string list
    | VkFuncPointer of parTypes: string list * returnType: string
    | VkUnion of members: VkUnionCase list

and VkType = VkType of name: string * comment: string option * VkTypeKind

type VkBinding = VkBinding of name: string * value: string * comment: string option 

type VkCommand = VkCommand of name: string * parTypes: string list * returnType: string

type Vk = XmlProvider<"vk.xml">
let vk = Vk.Load("vk.xml")

type env =
    {
        types: Map<string, VkType>
        primTypes: Map<string, string>
    }

let rec getFixedSize env = function
    | "byte" -> sizeof<byte>
    | "sbyte" -> sizeof<sbyte>
    | "uint16" -> sizeof<uint16>
    | "int16" -> sizeof<int16>
    | "uint32" -> sizeof<uint32>
    | "int" -> sizeof<int>
    | "uint64" -> sizeof<uint64>
    | "int64" -> sizeof<int64>
    | "char" -> sizeof<char>
    | "float32" -> sizeof<float32>
    | "float" -> sizeof<float>
    | "nativeint" -> failwith "Can't get fixed size of nativeint."
    | x -> 
        let vkTy = env.types.[x]
        match vkTy with
        | VkType(_, _, VkEnum _) -> sizeof<int>
        | VkType(_, _, VkFlags _) -> sizeof<int>
        | VkType(_, _, VkAlias target) -> getFixedSize env target
        | VkType(_, _, VkStruct(members, _)) ->
            members
            |> List.map (function VkMember(typeName=n) -> getFixedSize env n)
            |> List.reduce (+)
        | VkType(_, _, VkUnion cases) ->
            cases
            |> List.map (function VkUnionCase(typeName=n) -> getFixedSize env n)
            |> List.max
        | VkType(_, _, VkFuncPointer _) -> failwith "Function pointers can't get a fixed size."

let tryGetVkEnumCase (vkXmlEnum: Vk.Enum) =
    match vkXmlEnum.Value.Number with
    | Some value ->
        VkEnumCase(vkXmlEnum.Name, value, vkXmlEnum.Comment)
        |> Some
    | _ ->

    match vkXmlEnum.Bitpos with
    | Some bitpos ->
        VkEnumCase(vkXmlEnum.Name, 1 <<< bitpos, vkXmlEnum.Comment)
        |> Some
    | _ ->
        None

let tryGetVkEnum (vkXmlEnums: Vk.Enums) =
    if vkXmlEnums.Type.IsNone then None
    else
        
    match vkXmlEnums.Type with
    | Some "enum" ->
        let cases =
            vkXmlEnums.Enums
            |> Seq.choose tryGetVkEnumCase
            |> List.ofSeq

        if not cases.IsEmpty then
            VkType(vkXmlEnums.Name, vkXmlEnums.Comment, VkEnum cases)
            |> Some
        else
            None
    | Some "bitmask" ->
        let cases =
            vkXmlEnums.Enums
            |> Seq.choose tryGetVkEnumCase
            |> List.ofSeq

        if not cases.IsEmpty then
            VkType(vkXmlEnums.Name, vkXmlEnums.Comment, VkFlags cases)
            |> Some
        else
            None
    | _ ->
        None

let getTypeName env typeName (node: XNode) =
    let typeName =
        match env.primTypes.TryFind typeName with
        | Some typeName -> typeName
        | _ -> typeName
        
    let isPointer = 
        match node.NodeType with
        | XmlNodeType.Text -> 
            (node :> obj :?> XText).Value.Contains "*"
        | XmlNodeType.Element ->
            (node :> obj :?> XElement).Value.Contains "*"
        | _ ->
            false

    match typeName, isPointer with
    | "void", true -> "nativeint"
    | _, true -> "nativeptr<" + typeName + ">"
    | _ -> typeName

let getVkMember env (vkXmlMember: Vk.Member) =
    let typeName =
        let typeName = vkXmlMember.Type
        getTypeName env typeName vkXmlMember.XElement

    let isMutable = not (vkXmlMember.XElement.Value.Contains "const")

    let memberName = vkXmlMember.Name
    // Special case this, we cannot have a member name of "type" or "module"
    let memberName = if memberName = "type" then "typ" elif memberName = "module" then "modul" else memberName
    VkMember(memberName, typeName, isMutable, vkXmlMember.Comment)

let getVkUnionCase env (vkXmlMember: Vk.Member) =
    let typeName =
        let typeName = vkXmlMember.Type
        getTypeName env typeName vkXmlMember.XElement

    let isMutable = not (vkXmlMember.XElement.Value.Contains "const")

    let memberName = vkXmlMember.Name
    // Special case this, we cannot have a member name of "type" or "module"
    let memberName = if memberName = "type" then "typ" elif memberName = "module" then "modul" else memberName

    VkUnionCase(memberName, typeName, 0, isMutable, vkXmlMember.Comment)

let validName = function
    | "void"
    | "char"
    | "float"
    | "double"
    | "uint8_t"
    | "uint16_t"
    | "uint32_t"
    | "uint64_t"
    | "int32_t"
    | "int64_t"
    | "size_t" 
    | "int" -> false
    | _ -> true

let tryGetVkType env (vkXmlType: Vk.Type) =
    match vkXmlType.Category, vkXmlType.Name, vkXmlType.Name2, vkXmlType.Alias with
    | Some catName, None, Some name, None when (catName = "basetype" || catName = "bitmask" || catName = "handle") && vkXmlType.Types.Length = 1 ->
        if catName = "handle" then
            let vkTy = VkType(name, vkXmlType.Comment, VkAlias "nativeint")
            Some vkTy, { env with types = Map.add name vkTy env.types }
        else
            let target = 
                let target = vkXmlType.Types.[0]
                match env.primTypes.TryFind target with
                | Some target -> target
                | _ -> target

            let vkTy = VkType(name, vkXmlType.Comment, VkAlias target)
            Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "struct", Some name, None, Some target ->
        let vkTy = VkType(name, vkXmlType.Comment, VkAlias target)
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "struct", Some name, None, None ->
        let members =
            vkXmlType.Members
            |> Seq.map (getVkMember env)
            |> List.ofSeq

        let extendedTypes =
            match vkXmlType.Structextends with
            | Some extNames -> 
                extNames.Split ','
                |> List.ofSeq
            | _ -> []

        let vkTy = VkType(name, vkXmlType.Comment, VkStruct(members, extendedTypes))
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "funcpointer", None, Some name, None ->
        let firstTypeNode = vkXmlType.XElement.FirstNode
        let parTypes =
            let mutable node = firstTypeNode
            [
                while node <> null do
                    if node.NodeType = XmlNodeType.Element then
                        let element = node :> obj :?> XElement
                        if element.Name.LocalName = "type" then
                            let typeName = element.Value
                            node <- node.NextNode
                            if element <> null then
                                yield getTypeName env typeName node
                            else
                                yield typeName
                    node <- node.NextNode
            ]

        let parTypes =
            if parTypes.IsEmpty then
                ["unit"]
            else
                parTypes

        let returnType =
            // special case
            if vkXmlType.XElement.Value.Contains "typedef void*" then
                "nativeint"
            elif vkXmlType.XElement.Value.Contains "typedef void" then
                "unit"
            elif vkXmlType.XElement.Value.Contains "typedef VkBool32" then
                "VkBool32"
            else
                failwith "Special case funcpointer return type not handled."
                
        let vkTy = VkType(name, vkXmlType.Comment, VkFuncPointer(parTypes, returnType))
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "union", Some name, None, None ->
        let cases =
            vkXmlType.Members
            |> Seq.map (getVkUnionCase env)
            |> List.ofSeq

        let vkTy = VkType(name, vkXmlType.Comment, VkUnion cases)
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | None, Some name, None, None when validName name ->
        let vkTy = VkType(name, vkXmlType.Comment, VkAlias "nativeint")
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "define", None, Some name, None when validName name ->
        let vkTy = VkType(name, vkXmlType.Comment, VkAlias "nativeint")
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | _ ->
        None, env

let getVkBindings () =
    vk.Enums
    |> Seq.map (fun x ->
        match x.Name, x.Type with
        | _, None ->
            x.Enums
            |> Seq.choose (fun x ->
                let valueOpt = 
                    if x.XElement.FirstAttribute.Name.LocalName = "value" then
                        Some x.XElement.FirstAttribute.Value
                    else
                        None
                match valueOpt, x.Alias with
                | Some value, None -> 
                    let value =
                        // special cases
                        match value with
                        | "(~0U)" -> "~~~0u"
                        | "(~0ULL)" -> "~~~0UL"
                        | "(~0U-1)" -> "~~~0u-1u"
                        | "(~0U-2)" -> "~~~0u-2u"
                        | _ -> value
                    VkBinding(x.Name, value, x.Comment) |> Some
                | None, Some alias -> VkBinding(x.Name, alias, x.Comment) |> Some
                | _ -> None
            )
            |> List.ofSeq
        | _ ->
            []
    )
    |> List.ofSeq
    |> List.concat

let genVkEnumCase vkEnumCase =
    match vkEnumCase with
    | VkEnumCase(name, value, Some comment) ->
        sprintf "    /// %s\n    | %s = %i" comment name value
    | VkEnumCase(name, value, _) ->
        sprintf "    | %s = %i" name value

let genVkMember vkMember =
    match vkMember with
    | VkMember(name, typeName, isMutable, Some comment) ->
        let mutableStr = if isMutable then "mutable " else ""
        sprintf "    /// %s\n    val %s%s: %s" comment mutableStr name typeName
    | VkMember(name, typeName, isMutable, _) ->
        let mutableStr = if isMutable then "mutable " else ""
        sprintf "    val %s%s: %s" mutableStr name typeName

let genVkUnionCase env vkUnionCase =
    match vkUnionCase with
    | VkUnionCase(name, typeName, count, isMutable, Some comment) ->
        let mutableStr = if isMutable then "mutable " else ""
        if count = 0 then
            sprintf "    /// %s\n    [<FieldOffset(0)>] val %s%s: %s" comment mutableStr name typeName
        else
            let typeSize = getFixedSize env typeName
            List.init count (fun i -> "name_" + string i)
            |> List.mapi (fun i name ->
                let offset = i * typeSize
                sprintf "    /// %s\n    [<FieldOffset(%i)>] val %s%s: %s" comment offset mutableStr name typeName
            )
            |> List.reduce (fun x y -> x + "\n" + y)
    | VkUnionCase(name, typeName, count, isMutable, _) ->
        let mutableStr = if isMutable then "mutable " else ""
        if count = 0 then
            sprintf "    [<FieldOffset(0)>] val %s%s: %s" mutableStr name typeName
        else
            let typeSize = getFixedSize env typeName
            List.init count (fun i -> "name_" + string i)
            |> List.mapi (fun i name ->
                let offset = i * typeSize
                sprintf "    [<FieldOffset(%i)>] val %s%s: %s" offset mutableStr name typeName
            )
            |> List.reduce (fun x y -> x + "\n" + y)

let genVkType env vkType =
    match vkType with
    | VkType(name, comment, kind) ->
        let attribs =
            match kind with
            | VkFlags _ -> ["[<Flags>]\n"]
            | VkStruct _ -> ["[<Struct>]\n"]
            | VkUnion _ -> ["[<Struct;StructLayout(LayoutKind.Explicit)>]\n"]
            | _ -> [""]
            |> List.reduce(fun x y -> x + y)

        let decl =
            match comment with
            | Some comment -> 
                sprintf "/// %s\n%stype %s =" comment attribs name
            | _ ->
                sprintf "%stype %s =" attribs name

        let body =
            match kind with
            | VkEnum cases ->
                cases
                |> List.map genVkEnumCase
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

            | VkFlags cases ->
                cases
                |> List.map genVkEnumCase
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

            | VkAlias targetName ->
                " " + targetName

            | VkStruct(members, _) ->
                members
                |> List.map genVkMember
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

            | VkFuncPointer (parTypes, returnType) ->
                " delegate of " +
                (parTypes |> List.reduce (fun x y -> x + " * " + y)) +
                " -> " + returnType

            | VkUnion cases ->
                cases
                |> List.map (genVkUnionCase env)
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

        decl + body

let genVkTypes env vkTypes =
    vkTypes
    |> List.map (genVkType env)
    |> List.reduce (fun x y -> x + "\n\n" + y)

let genVkBinding vkBinding =
    match vkBinding with
    | VkBinding(name, value, comment) ->
        let comment =
            match comment with
            | Some comment -> sprintf "/// %s\n" comment
            | _ -> String.Empty

        sprintf "%slet %s = %s" comment name value

let genVkBindings vkBindings =
    vkBindings
    |> List.map genVkBinding
    |> List.reduce (fun x y -> x + "\n" + y)

let genSource () =
    let primTypes =
        [
            ("float", "float32")
            ("double", "float")
            ("uint8_t", "byte")
            ("uint16_t", "uint16")
            ("uint32_t", "uint32")
            ("uint64_t", "uint64")
            ("int32_t", "int")
            ("int64_t", "int64")
            ("size_t", "nativeint")          
        ]

    let env = { types = Map.empty; primTypes = Map.ofList primTypes }

    let vkEnumTypes =
        vk.Enums
        |> Seq.choose tryGetVkEnum
        |> List.ofSeq

    let vkTypes, env =
        (([], env), vk.Types.Types)
        ||> Seq.fold (fun (vkTys, env) vkXmlType ->
            match tryGetVkType env vkXmlType with
            | Some vkTy, env -> (vkTy :: vkTys, env)
            | _ -> (vkTys, env)
        )
    let vkTypes =
        vkTypes
        |> List.rev

    let vkBindings = getVkBindings()

    "// File is generated. Do not modify.\n" +
    "module rec FSharp.Vulkan\n\n" +
    "open System\n" +
    "open System.Runtime.InteropServices\n\n" +
    """#nowarn "9" """ + "\n\n" +
    genVkTypes env vkEnumTypes + "\n\n" +
    genVkTypes env vkTypes + "\n\n" +
    genVkBindings vkBindings

open System.IO
open System.Reflection

[<EntryPoint>]
let main argv =
    let src = genSource()

    let asm = Assembly.GetExecutingAssembly()
    let path1 = (Path.GetDirectoryName asm.Location) + """\..\..\..\..\FSharp.Vulkan\vk.fs"""
    if File.Exists path1 then
        File.WriteAllText(path1, src)
        printfn "Success"
    else
        failwith "Path not found."
    0
