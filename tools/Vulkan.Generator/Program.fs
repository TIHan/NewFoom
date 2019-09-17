open System
open System.Xml
open System.Xml.Linq

open FSharp.Data

[<RequireQualifiedAccess>]
type VkEnumCaseValue =
    | UInt32 of uint32
    | Ext of name: string * value: string

type VkEnumCase = VkEnumCase of name: string * value: VkEnumCaseValue * comment: string option

type VkMember = VkMember of name: string * typeName: string * comment: string option

type VkUnionCase = VkUnionCase of name: string * typeName: string * comment: string option

type VkTypeKind = 
    | VkEnum of cases: VkEnumCase list
    | VkFlags of cases: VkEnumCase list
    | VkAliasOrDefine of targetName: string
    | VkStruct of members: VkMember list * extendedTypes: string list
    | VkFuncPointer of parTypes: string list * returnType: string
    | VkUnion of members: VkUnionCase list

and VkType = | VkType of name: string * comment: string option * VkTypeKind

type VkBinding = 
    | VkBinding of name: string * value: string * comment: string option * obsoleteDescription: string option
    | VkExtension of VkBinding * extends: string

type VkParam = VkParam of name: string * typeName: string

type VkFunction = VkFunction of name: string * pars: VkParam list * returnType: string * comment: string option

type Vk = XmlProvider<"vk.xml">
let vk = Vk.Load("vk.xml")

let maxPhysicalDeviceNameSize =
    let enum =
        vk.Enums
        |> Array.pick (fun x -> x.Enums |> Array.find (fun x -> x.Name = "VK_MAX_PHYSICAL_DEVICE_NAME_SIZE") |> Some)
    enum.XElement.FirstAttribute.Value |> Int32.Parse

let maxExtensionNameSize =
    let enum =
        vk.Enums
        |> Array.pick (fun x -> x.Enums |> Array.find (fun x -> x.Name = "VK_MAX_EXTENSION_NAME_SIZE") |> Some)
    enum.XElement.FirstAttribute.Value |> Int32.Parse

type env =
    {
        types: Map<string, VkType>
        primTypes: Map<string, string>

        extEnums: Map<string, VkEnumCase list>
    }

let isVkFuncPointer env typeName =
    match env.types.TryFind typeName with
    | Some(VkType(_, _, VkFuncPointer _)) -> true
    | _ -> false

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
        | VkType(_, _, VkAliasOrDefine target) -> getFixedSize env target
        | VkType(_, _, VkStruct(members, _)) ->
            members
            |> List.map (function VkMember(typeName=n) -> getFixedSize env n)
            |> List.reduce (+)
        | VkType(_, _, VkUnion cases) ->
            cases
            |> List.map (function VkUnionCase(typeName=n) -> getFixedSize env n)
            |> List.max
        | VkType(_, _, VkFuncPointer _) -> failwith "Function pointers can't get a fixed size."

let filterComment comment =
    match comment with
    | Some comment -> sprintf "/// %s\n" comment
    | _ -> String.Empty

let tryGetVkEnumCase (vkXmlEnum: Vk.Enum) =
    let valueOpt = 
        if vkXmlEnum.XElement.FirstAttribute.Name.LocalName = "value" then
            Some vkXmlEnum.XElement.FirstAttribute.Value
        else
            None

    match valueOpt with
    | Some value ->
        VkEnumCase(vkXmlEnum.Name, VkEnumCaseValue.UInt32(uint32 (int value)), vkXmlEnum.Comment)
        |> Some
    | _ ->

    match vkXmlEnum.Bitpos with
    | Some bitpos ->
        VkEnumCase(vkXmlEnum.Name, VkEnumCaseValue.UInt32(1u <<< bitpos), vkXmlEnum.Comment)
        |> Some
    | _ ->
        None

let getExtendedVkEnumCases env cases name =
    match env.extEnums.TryFind name with
    | Some cases2 -> cases @ cases2
    | _ -> cases
    |> List.sortByDescending (function VkEnumCase(value=value) -> value)
    |> List.distinctBy (function VkEnumCase(name=name) -> name)
    |> List.sortBy (function VkEnumCase(value=value) -> value)

let tryGetVkEnum env (vkXmlEnums: Vk.Enums) =
    if vkXmlEnums.Type.IsNone then None
    else
        
    match vkXmlEnums.Type with
    | Some "enum" ->
        let cases =
            vkXmlEnums.Enums
            |> Seq.choose tryGetVkEnumCase
            |> List.ofSeq

        let cases = getExtendedVkEnumCases env cases vkXmlEnums.Name

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

        let cases = getExtendedVkEnumCases env cases vkXmlEnums.Name

        if not cases.IsEmpty then
            VkType(vkXmlEnums.Name, vkXmlEnums.Comment, VkFlags cases)
            |> Some
        else
            None
    | _ ->
        None

let rec getPointerCount (node: XNode) =
    if node <> null then
        match node.NodeType with
        | XmlNodeType.Element ->
            let node = node.NextNode
            if node <> null && node.NodeType = XmlNodeType.Text then        
                let node = node :> obj :?> XText
                node.Value |> String.filter (fun x -> x = '*') |> String.length
            else
                0
        | XmlNodeType.Text ->
            let node = node :> obj :?> XText
            let count = node.Value |> String.filter (fun x -> x = '*') |> String.length
            if count > 0 then
                count
            else
                getPointerCount node.NextNode
        | _ -> 
            getPointerCount node.NextNode
    else
        0

let getTypeName env typeName (node: XNode) =
    let typeName =
        match env.primTypes.TryFind typeName with
        | Some typeName -> typeName
        | _ -> typeName
        
    let pointerCount = getPointerCount node

    match typeName, pointerCount with
    | "void", 1 -> "nativeint"
    | _, 1 -> "nativeptr<" + typeName + ">"
    | _, 2 -> "nativeptr<nativeptr<" + typeName + ">>"
    | _, 0 -> typeName
    | _ ->
        failwithf "Nested pointer not handled."

let getVkMemberCount (vkXmlMember: Vk.Member) =
    if vkXmlMember.XElement.Value.Contains "VK_MAX_PHYSICAL_DEVICE_NAME_SIZE" then
        maxPhysicalDeviceNameSize
    elif vkXmlMember.XElement.Value.Contains "VK_MAX_EXTENSION_NAME_SIZE" then
        maxExtensionNameSize
    else
        1

let getVkMember env (vkXmlMember: Vk.Member) =
    let typeName =
        let typeName = vkXmlMember.Type
        getTypeName env typeName vkXmlMember.XElement.FirstNode

    let count = getVkMemberCount vkXmlMember

    let memberName = vkXmlMember.Name
    // Special case this, we cannot have a member name of "type" or "module"
    let memberName = if memberName = "type" then "typ" elif memberName = "module" then "modul" else memberName

    let typeName =
        if count = 1 then typeName
        else typeName + "[" + string count + "]"

    VkMember(memberName, typeName, vkXmlMember.Comment)

let getVkUnionCase env (vkXmlMember: Vk.Member) =
    let typeName =
        let typeName = vkXmlMember.Type
        getTypeName env typeName vkXmlMember.XElement

    let memberName = vkXmlMember.Name
    // Special case this, we cannot have a member name of "type" or "module"
    let memberName = if memberName = "type" then "typ" elif memberName = "module" then "modul" else memberName

    VkUnionCase(memberName, typeName, vkXmlMember.Comment)

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
            let vkTy = VkType(name, vkXmlType.Comment, VkAliasOrDefine "nativeint")
            Some vkTy, { env with types = Map.add name vkTy env.types }
        else
            let target = 
                let target = vkXmlType.Types.[0]
                match env.primTypes.TryFind target with
                | Some target -> target
                | _ -> target

            let vkTy = VkType(name, vkXmlType.Comment, VkAliasOrDefine target)
            Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "struct", Some name, None, Some target ->
        let vkTy = VkType(name, vkXmlType.Comment, VkAliasOrDefine target)
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
        let vkTy = VkType(name, vkXmlType.Comment, VkAliasOrDefine "nativeint")
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | Some "define", None, Some name, None when validName name ->
        let vkTy = VkType(name, vkXmlType.Comment, VkAliasOrDefine "nativeint")
        Some vkTy, { env with types = Map.add name vkTy env.types }

    | _ ->
        None, env

let tryMkVkBinding name aliasOpt commentOpt (xel: XElement) =
    let valueOpt = 
        if xel.FirstAttribute.Name.LocalName = "value" then
            Some xel.FirstAttribute.Value
        else
            None
    match valueOpt, aliasOpt with
    | Some value, None -> 
        let value =
            // special cases
            match value with
            | "(~0U)" -> "~~~0u"
            | "(~0ULL)" -> "~~~0UL"
            | "(~0U-1)" -> "~~~0u-1u"
            | "(~0U-2)" -> "~~~0u-2u"
            | _ -> value
        VkBinding(name, value, commentOpt, None) |> Some
    | None, Some alias -> VkBinding(name, alias, commentOpt, None) |> Some
    | _ -> None

let getVkBindings () =
    vk.Enums
    |> Seq.map (fun x ->
        match x.Name, x.Type with
        | _, None ->
            x.Enums
            |> Seq.choose (fun x ->
                tryMkVkBinding x.Name x.Alias x.Comment x.XElement
            )
            |> List.ofSeq
        | _ ->
            []
    )
    |> List.ofSeq
    |> List.concat

let getVkFunctions env =
    vk.Commands.Commands
    |> Seq.choose (fun x ->
        match x.Proto with
        | Some proto ->
            let parTypes =
                x.Params
                |> Seq.map (fun p ->
                    let typeName = getTypeName env p.Type p.XElement.FirstNode
                    let name = p.Name
                    // Special case this, we cannot have a param name of "type" or "module"
                    let name = if name = "type" then "typ" elif name = "module" then "modul" else name
                    VkParam(name, typeName)
                )
                |> List.ofSeq
            let returnType = getTypeName env proto.Type x.XElement.FirstNode
            VkFunction(proto.Name, parTypes, returnType, x.Comment) |> Some
        | _ ->
            None
    )
    |> List.ofSeq

// TODO: Clean this up.
let getVkBindingsForExtensions env =
    let vkBindings =
        vk.Features
        |> Seq.map (fun x ->
            x.Requires
            |> Seq.map (fun y ->
                y.Enums
                |> Seq.choose (fun z ->
                    match z.Offset, z.Extnumber with
                    | Some offset, Some extnumber ->
                        let dir =
                            match z.Dir with
                            | Some "-" -> -1
                            | _ -> 1
                        let value = dir * (1000000000 + ((extnumber - 1) * 1000) + offset)
                        VkBinding(z.Name, string (uint32 value), z.Comment, None)
                        |> Some
                    | _ ->
                        match z.Bitpos with
                        | Some bitpos ->
                            let value = 1u <<< bitpos
                            VkBinding(z.Name, string value, z.Comment, None)
                            |> Some
                        | _ ->
                            tryMkVkBinding z.Name z.Alias z.Comment z.XElement
                    |> Option.map (fun vkBinding ->
                        match z.Extends with
                        | Some extends -> VkExtension(vkBinding, extends)
                        | _ -> vkBinding
                    )
                )
                |> List.ofSeq
            )
            |> List.ofSeq
            |> List.concat
        )
        |> List.ofSeq
        |> List.concat

    let vkBindings2 =
        vk.Extensions.Extensions
        |> Seq.map (fun x ->
            let obsoleteDescription =
                match x.Obsoletedby with
                | Some name -> sprintf "Use %s instead." name |> Some
                | _ ->
                    match x.Deprecatedby with
                    | Some name -> sprintf "Use %s instead." name |> Some
                    | _ -> None

            x.Requires2
            |> Seq.map (fun y ->
                y.Enums
                |> Seq.choose (fun z ->
                    match z.Offset with
                    | Some offset ->
                        let dir =
                            match z.Dir with
                            | Some "-" -> -1
                            | _ -> 1
                        let value = dir * (1000000000 + ((x.Number - 1) * 1000) + offset)
                        VkBinding(z.Name, string (uint32 value), z.Comment, obsoleteDescription)
                        |> Some
                    | _ ->
                        match z.Bitpos with
                        | Some bitpos ->
                            let value = 1u <<< bitpos
                            VkBinding(z.Name, string value, z.Comment, obsoleteDescription)
                            |> Some
                        | _ ->
                            tryMkVkBinding z.Name z.Alias z.Comment z.XElement
                    |> Option.map (fun vkBinding ->
                        match z.Extends with
                        | Some extends -> VkExtension(vkBinding, extends)
                        | _ -> vkBinding
                    )
                )
                |> List.ofSeq
            )
            |> List.ofSeq
            |> List.concat
        )
        |> List.concat

    let env =
        (env, vkBindings @ vkBindings2)
        ||> List.fold (fun env vkBinding ->
            match vkBinding with
            | VkExtension(VkBinding(name, value, comment, _), extends) -> 
                let caseValue =
                    match UInt32.TryParse value with
                    | true, value -> VkEnumCaseValue.UInt32 value
                    | _ -> VkEnumCaseValue.Ext (extends, value)
                let case = VkEnumCase(name, caseValue, comment)
                let extEnums =
                    let cases =
                        match env.extEnums.TryFind extends with
                        | Some cases -> cases @ [case]
                        | _ -> [case]
                    env.extEnums
                    |> Map.add extends cases
                { env with extEnums = extEnums }
            | _ -> 
                env
        )
    vkBindings, env

let genVkEnumCase env cases vkEnumCase =
    match vkEnumCase with
    | VkEnumCase(name, value, comment) ->
        let comment = if comment.IsSome then "    " + filterComment comment else String.Empty
        let value =
            match value with
            | VkEnumCaseValue.UInt32 value -> string value + "u"
            | VkEnumCaseValue.Ext (extends, value) ->
                let rec findDeepExt (extends: string) (value: string) = 
                    let extCases = env.extEnums.[extends]
                    extCases @ cases
                    |> List.pick (fun x -> 
                        match x with
                        | VkEnumCase(name2, VkEnumCaseValue.Ext (extends2, value2), _) when value = name2 ->
                            findDeepExt extends2 value2 |> Some
                        | VkEnumCase(name2, VkEnumCaseValue.UInt32 value2, _) when value = name2 -> 
                            string value2 + "u" |> Some
                        | _ ->
                            None
                    )
                findDeepExt extends value
        comment + sprintf "    | %s = %s" name value

let genVkMember vkMember =
    match vkMember with
    | VkMember(name, typeName, comment) ->
        let comment = if comment.IsSome then "    " + filterComment comment else String.Empty
        comment + sprintf "    [<DefaultValue(false)>]\n    val mutable %s: %s" name typeName

let genVkUnionCase env vkUnionCase =
    match vkUnionCase with
    | VkUnionCase(name, typeName, Some comment) ->
        sprintf "    /// %s\n    [<FieldOffset(0);DefaultValue(false)>]\n    val mutable %s: %s" comment name typeName

    | VkUnionCase(name, typeName, _) ->
        sprintf "    [<FieldOffset(0);DefaultValue(false)>]\n    val mutable %s: %s" name typeName

let tryGenVkType env vkType =
    match vkType with
    | VkType(name, comment, kind) ->
        let attribs =
            match kind with
            | VkFlags _ -> ["[<Flags>]\n"]
            | VkStruct _ -> ["[<Struct;NoEquality;NoComparison>]\n"]
            | VkUnion _ -> ["[<Struct;StructLayout(LayoutKind.Explicit);NoEquality;NoComparison>]\n"]
            //| VkFuncPointer _ -> ["[<UnmanagedFunctionPointer(CallingConvention.Winapi)>]\n"]
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
                |> List.map (genVkEnumCase env cases)
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

            | VkFlags cases ->
                cases
                |> List.map (genVkEnumCase env cases)
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

            | VkAliasOrDefine targetName ->
                " " + targetName

            | VkStruct(members, _) ->
                members
                |> List.map genVkMember
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

            | VkFuncPointer (_parTypes, _returnType) ->
                " " + "nativeint"
                //" delegate of " +
                //(parTypes |> List.reduce (fun x y -> x + " * " + y)) +
                //" -> " + returnType

            | VkUnion cases ->
                cases
                |> List.map (genVkUnionCase env)
                |> List.reduce (fun x y -> x + "\n" + y)
                |> (+) "\n"

        // Define heuristics
        match name with
        | "VK_MAKE_VERSION" -> 
            "let inline VK_MAKE_VERSION(major: uint32, minor: uint32, patch: uint32) = ((major <<< 22) ||| (minor <<< 12) ||| patch)"
            |> Some
        | "VK_VERSION_MAJOR" ->
            "let inline VK_VERSION_MAJOR(version: uint32) = version >>> 22"
            |> Some
        | "VK_VERSION_MINOR" ->
            "let inline VK_VERSION_MINOR(version: uint32) = (version >>> 12) &&& 0x3ffu"
            |> Some
        | "VK_VERSION_PATCH" ->
            "let inline VK_VERSION_PATCH(version: uint32) = (version &&& 0xfffu)"
            |> Some
        | "VK_API_VERSION" ->
            None // Deprecated
        | "VK_API_VERSION_1_0" ->
            "let VK_API_VERSION_1_0 = VK_MAKE_VERSION(1u, 0u, 0u)"
            |> Some
        | "VK_API_VERSION_1_1" ->
            "let VK_API_VERSION_1_1 = VK_MAKE_VERSION(1u, 1u, 0u)"
            |> Some
        | "VK_HEADER_VERSION" ->
            None // Not relevant
        | "VK_DEFINE_HANDLE" ->
            None // Not relevant
        | "VK_NULL_HANDLE" ->
            "let VK_NULL_HANDLE = nativeint 0"
            |> Some
        | _ ->
            if name.Contains "VK_" then
                failwithf "%s not handled." name
            else
                decl + body
                |> Some

let genVkTypes env vkTypes =
    vkTypes
    |> List.choose (tryGenVkType env)
    |> List.reduce (fun x y -> x + "\n\n" + y)

let tryGenVkBinding vkBinding =
    match vkBinding with
    | VkBinding(name, value, comment, obsoleteDescription) ->
        let attribs =
            match obsoleteDescription with
            | Some x -> sprintf """[<Obsolete("%s")>]""" x + "\n"
            | _ -> String.Empty
        let comment = filterComment comment
        sprintf "%s%slet %s = %s" comment attribs name value
        |> Some
    | _ ->
        None

let genVkBindings vkBindings =
    let xs =
        vkBindings
        |> List.choose tryGenVkBinding
    if xs.IsEmpty then
        String.Empty
    else
        xs
        |> List.reduce (fun x y -> x + "\n" + y)

let filterParamAndReturnType env typeName =
    match typeName with
    | "unit" -> "void"
    | "nativeint" -> "void*"
    | _ when typeName.StartsWith("nativeptr") ->
        let typeName = typeName.Replace("nativeptr<", String.Empty).Replace(">", String.Empty)
        if isVkFuncPointer env typeName then
            typeName
        else
            typeName + "*"
    | _ when typeName.StartsWith("nativeptr<nativeptr<") ->
        let typeName = typeName.Replace("nativeptr<nativeptr<", String.Empty).Replace(">>", String.Empty)
        if isVkFuncPointer env typeName then
            typeName
        else
            typeName + "**"
    | _ -> typeName

let genVkParams env vkParams =
    if List.isEmpty vkParams then
        String.Empty
    else
        vkParams
        |> List.map (function VkParam(name, typeName) -> filterParamAndReturnType env typeName + " " + name)
        |> List.reduce (fun x y -> x + ", " + y)

let genVkFunction env vkFunction =
    match vkFunction with
    | VkFunction(name, vkParams, returnType, comment) ->
        filterComment comment + 
        """[<DllImport("vulkan-1.dll", CallingConvention = CallingConvention.Winapi)>]""" + "\n" +
        sprintf "extern %s %s" (filterParamAndReturnType env returnType) name +
        "(" + genVkParams env vkParams + ")"

let genVkFunctions env vkFunctions =
    vkFunctions
    |> List.map (genVkFunction env)
    |> List.reduce (fun x y -> x + "\n\n" + y)

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
            ("size_t", "unativeint")          
        ]

    let env = { types = Map.empty; primTypes = Map.ofList primTypes; extEnums = Map.empty }

    let vkBindingsForExtensions, env = getVkBindingsForExtensions env

    let vkEnumTypes =
        vk.Enums
        |> Seq.choose (tryGetVkEnum env)
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
    let vkFunctions = getVkFunctions env

    "// File is generated. Do not modify.\n" +
    "module rec FSharp.Vulkan.Interop\n\n" +
    "open System\n" +
    "open System.Runtime.InteropServices\n" +
    "open FSharp.NativeInterop\n\n" +
    """#nowarn "9" """ + "\n\n" +
    genVkTypes env vkEnumTypes + "\n\n" +
    genVkTypes env vkTypes + "\n\n" +
    genVkBindings vkBindings + "\n\n" +
    genVkFunctions env vkFunctions + "\n\n" +
    genVkBindings vkBindingsForExtensions +
    "let inline vkMarshalString str = Marshal.StringToHGlobalAnsi str |> NativePtr.ofNativeInt<char>" + "\n" +
    "let inline vkMarshal(o: 'T when 'T : unmanaged) : nativeptr<'T> =
    let p = Marshal.AllocHGlobal(sizeof<'T>)
    Marshal.StructureToPtr(o, p, false)
    p |> NativePtr.ofNativeInt<'T>" + "\n" +
    "let inline vkMarshalArray(xs: 'T [] when 'T : unmanaged) : nativeptr<'T> =
    let size = sizeof<'T> * xs.Length
    let p = Marshal.AllocHGlobal size |> NativePtr.ofNativeInt
    use p2 = fixed xs
    Buffer.MemoryCopy(p2 |> NativePtr.toVoidPtr, p |> NativePtr.toVoidPtr, uint64 size, uint64 size)
    p" + "\n" +
    "let inline vkNull<'T when 'T : unmanaged> = nativeint 0 |> NativePtr.ofNativeInt<'T>" + "\n" +
    "let inline vkFree o = Marshal.FreeHGlobal o"

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
