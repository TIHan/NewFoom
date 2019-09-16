open System
//open System.Xml.Linq

open FSharp.Data

type VkEnumCase = VkEnumCase of name: string * value: int * comment: string option

type VkTypeKind = 
    | VkEnum of cases: VkEnumCase list
    | VkFlags of cases: VkEnumCase list
    | VkAlias of targetName: string

type VkType = VkType of name: string * comment: string option * VkTypeKind

type Vk = XmlProvider<"vk.xml">
let vk = Vk.Load("vk.xml")

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

let genVkEnumCase vkEnumCase =
    match vkEnumCase with
    | VkEnumCase(name, value, Some comment) ->
        sprintf "    /// %s\n    | %s = %i" comment name value
     | VkEnumCase(name, value, _) ->
        sprintf "    | %s = %i" name value

let genVkType vkType =
    match vkType with
    | VkType(name, comment, kind) ->
        let attribs =
            match kind with
            | VkFlags _ -> ["[<Flags>]\n"]
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

        decl + body

let genVkTypes (vkTypes: VkType list) =
    vkTypes
    |> List.map genVkType
    |> List.reduce (fun x y -> x + "\n\n" + y)

let genSource () =
    let vkEnumTypes =
        vk.Enums
        |> Seq.choose tryGetVkEnum
        |> List.ofSeq

    "module FSharp.Vulkan\n\n" +
    "open System\n\n" +
    genVkTypes vkEnumTypes

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
