module FSharp.Spirv.Quotations.Tast

open System.Numerics
open FSharp.Spirv
open System.Reflection.Metadata

let mutable nextStamp = 0L 
let newStamp () =
    System.Threading.Interlocked.Increment &nextStamp 

type Decorations = Decoration list

type SpirvImageType = SpirvImageType of sampledType: SpirvType * dim: Dim * depth: uint32 * arrayed: uint32 * ms: uint32 * sampled: uint32 * format: ImageFormat * AccessQualifier option

and SpirvType =
    | SpirvTypeVoid
    | SpirvTypeInt
    | SpirvTypeUInt32
    | SpirvTypeSingle
    | SpirvTypeVector2
    | SpirvTypeVector2Int
    | SpirvTypeVector3
    | SpirvTypeVector4
    | SpirvTypeMatrix4x4
    | SpirvTypeArray of SpirvType * length: int
    | SpirvTypeStruct of name: string * fields: SpirvField list
    | SpirvTypeImage of SpirvImageType
    | SpirvTypeSampler
    | SpirvTypeSampledImage of SpirvImageType

    member x.Name =
        match x with
        | SpirvTypeVoid -> "void"
        | SpirvTypeInt -> "int"
        | SpirvTypeUInt32 -> "uint32"
        | SpirvTypeSingle -> "single"
        | SpirvTypeVector2 -> "Vector2"
        | SpirvTypeVector2Int -> "Vector2<int>"
        | SpirvTypeVector3 -> "Vector3"
        | SpirvTypeVector4 -> "Vector4"
        | SpirvTypeMatrix4x4 -> "Matrix4x4"
        | SpirvTypeArray (elementTy, length) -> elementTy.Name + "[" + string length + "]"
        | SpirvTypeStruct (name=name) -> name
        | SpirvTypeImage _ -> "Image"
        | SpirvTypeSampler -> "Sampler"
        | SpirvTypeSampledImage _ -> "SampledImage"

    member x.Size: int =
        match x with
        | SpirvTypeVoid -> failwith "Unable to get size of void type."
        | SpirvTypeInt -> sizeof<int>
        | SpirvTypeUInt32 -> sizeof<uint32>
        | SpirvTypeSingle -> sizeof<single>
        | SpirvTypeVector2
        | SpirvTypeVector2Int -> sizeof<Vector2>
        | SpirvTypeVector3 -> sizeof<Vector3>
        | SpirvTypeVector4 -> sizeof<Vector4>
        | SpirvTypeMatrix4x4 -> sizeof<Matrix4x4>
        | SpirvTypeArray(ty, length) -> ty.Size * length
        | SpirvTypeStruct(_, fields) ->
            fields
            |> List.sumBy(fun (field: SpirvField) -> field.Type.Size)
        | SpirvTypeImage _
        | SpirvTypeSampler
        | SpirvTypeSampledImage _ -> failwith "Unable to get size of opaque type."

    member x.IsVoid = match x with SpirvTypeVoid -> true | _ -> false

    member x.IsStruct = match x with SpirvTypeStruct _ -> true | _ -> false

    member x.IsOpaque =
        match x with
        | SpirvTypeImage _
        | SpirvTypeSampler
        | SpirvTypeSampledImage _ -> true
        | _ -> false

and SpirvField = SpirvField of name: string * fieldType: SpirvType * Decorations with

    member x.Name =
        match x with
        | SpirvField (name=name) -> name

    member x.Type: SpirvType =
        match x with
        | SpirvField (fieldType=typ) -> typ

type SpirvVar = 
    {
        Stamp: int64
        Name: string
        Type: SpirvType
        Decorations: Decorations
        StorageClass: StorageClass
        IsMutable: bool
    }

let mkSpirvVar (name, ty, decorations, storageClass, isMutable) =
    let stamp = newStamp ()
    {
        Stamp = stamp
        Name = name
        Type = ty
        Decorations = decorations
        StorageClass = storageClass
        IsMutable = isMutable
    }

type SpirvConst =
    | SpirvConstInt of int * decorations: Decorations
    | SpirvConstUInt32 of uint32 * decorations: Decorations
    | SpirvConstSingle of single * decorations: Decorations
    | SpirvConstVector2 of single * single * decorations: Decorations
    | SpirvConstVector2Int of int * int * decorations: Decorations
    | SpirvConstVector3 of single * single * single * decorations: Decorations
    | SpirvConstVector4 of single * single * single * single * decorations: Decorations
    | SpirvConstMatrix4x4 of single * single * single * single *
                             single * single * single * single *
                             single * single * single * single *
                             single * single * single * single *
                             decorations: Decorations                            
    | SpirvConstArray of elementTy: SpirvType * constants: SpirvConst list * decorations: Decorations

    member x.Decorations =
        match x with
        | SpirvConstInt (decorations=decorations)
        | SpirvConstUInt32 (decorations=decorations)
        | SpirvConstSingle (decorations=decorations)
        | SpirvConstVector2 (decorations=decorations)
        | SpirvConstVector2Int (decorations=decorations)
        | SpirvConstVector3 (decorations=decorations)
        | SpirvConstVector4 (decorations=decorations)
        | SpirvConstMatrix4x4 (decorations=decorations)
        | SpirvConstArray (decorations=decorations) -> decorations

type SpirvExpr =
    | SpirvNop
    | SpirvConst of SpirvConst
    | SpirvLet of SpirvVar * rhs: SpirvExpr * body: SpirvExpr
    | SpirvSequential of SpirvExpr * SpirvExpr
    | SpirvNewVector2 of args: SpirvExpr list
    | SpirvNewVector3 of args: SpirvExpr list
    | SpirvNewVector4 of args: SpirvExpr list
    | SpirvArrayIndexerGet of receiver: SpirvExpr * arg: SpirvExpr
    | SpirvVar of SpirvVar
    | SpirvVarSet of SpirvVar * SpirvExpr
    | SpirvIntrinsicCall of SpirvIntrinsicCall
    | SpirvIntrinsicFieldGet of SpirvIntrinsicFieldGet
    | SpirvFieldGet of receiver: SpirvExpr * index: int

    member x.Type =
        let rec getType expr =
            match expr with
            | SpirvNop -> 
                SpirvTypeVoid
            | SpirvConst spvConst ->
                match spvConst with
                | SpirvConstInt _ -> SpirvTypeInt
                | SpirvConstUInt32 _ -> SpirvTypeUInt32
                | SpirvConstSingle _ -> SpirvTypeSingle
                | SpirvConstVector2 _ -> SpirvTypeVector2
                | SpirvConstVector2Int _ -> SpirvTypeVector2Int
                | SpirvConstVector3 _ -> SpirvTypeVector3
                | SpirvConstVector4 _ -> SpirvTypeVector4
                | SpirvConstMatrix4x4 _ -> SpirvTypeMatrix4x4
                | SpirvConstArray (elementTy, constants, _) ->
                    SpirvTypeArray (elementTy, constants.Length)
            | SpirvLet(_, _, body) ->
                getType body
            | SpirvSequential (_, expr2) ->
                getType expr2
            | SpirvNewVector2 _ ->
                SpirvTypeVector2
            | SpirvNewVector3 _ ->
                SpirvTypeVector3
            | SpirvNewVector4 _ ->
                SpirvTypeVector4
            | SpirvArrayIndexerGet _ ->
                SpirvTypeVoid
            | SpirvVar spvVar ->
                spvVar.Type
            | SpirvVarSet _ ->
                SpirvTypeVoid
            | SpirvIntrinsicCall call ->
                call.ReturnType
            | SpirvIntrinsicFieldGet fieldGet ->
                fieldGet.Type
            | SpirvFieldGet (receiver, index) ->
                match receiver.Type with
                | SpirvTypeStruct (_, fields) -> fields.[index].Type
                | _ -> failwith "Invalid field get."
        getType x

and SpirvIntrinsicCall =
    // TODO: Fix transform and multiply names
    | Transform__Vector4_Matrix4x4__Vector4 of vector4: SpirvExpr * matrix4x4: SpirvExpr
    | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 of matrix4x4_1: SpirvExpr * matrix4x4_2: SpirvExpr
    | SampledImage_T__Single_T of tyArg: SpirvType * receiver: SpirvExpr * arg: SpirvExpr

    member x.ReturnType =
        match x with
        | Transform__Vector4_Matrix4x4__Vector4 _ -> SpirvTypeVector4
        | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 _ -> SpirvTypeMatrix4x4
        | SampledImage_T__Single_T (tyArg, _, _) -> tyArg

    member x.Arguments =
        match x with
        | Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2)
        | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) -> [arg1;arg2]
        | SampledImage_T__Single_T (_, receiver, arg) -> [receiver;arg]

and SpirvIntrinsicFieldGet =
    | Vector2_Get_X of receiver: SpirvExpr * typ: SpirvType
    | Vector2_Get_Y of receiver: SpirvExpr * typ: SpirvType

    | Vector3_Get_X of receiver: SpirvExpr * typ: SpirvType
    | Vector3_Get_Y of receiver: SpirvExpr * typ: SpirvType
    | Vector3_Get_Z of receiver: SpirvExpr * typ: SpirvType

    | Vector4_Get_X of receiver: SpirvExpr * typ: SpirvType
    | Vector4_Get_Y of receiver: SpirvExpr * typ: SpirvType
    | Vector4_Get_Z of receiver: SpirvExpr * typ: SpirvType
    | Vector4_Get_W of receiver: SpirvExpr * typ: SpirvType

    member x.Type =
        match x with
        | Vector2_Get_X(typ=typ)
        | Vector2_Get_Y(typ=typ)

        | Vector3_Get_X(typ=typ)
        | Vector3_Get_Y(typ=typ)
        | Vector3_Get_Z(typ=typ)

        | Vector4_Get_X(typ=typ)
        | Vector4_Get_Y(typ=typ)
        | Vector4_Get_Z(typ=typ)
        | Vector4_Get_W(typ=typ) -> typ

type SpirvDecl =
    | SpirvDeclConst of SpirvVar * SpirvConst
    | SpirvDeclVar of SpirvVar

type SpirvTopLevelExpr =
    | SpirvTopLevelSequential of SpirvTopLevelExpr * SpirvTopLevelExpr
    | SpirvTopLevelDecl of SpirvDecl
    | SpirvTopLevelLambda of SpirvVar * SpirvTopLevelExpr
    | SpirvTopLevelLambdaBody of SpirvVar * SpirvExpr

type SpirvEntryPoint =
    {
        ExecutionModel: ExecutionModel
        FunctionName: string
    }

type SpirvDefnModule =
    {
        Capabilities: Capability list
        Extensions: string list
        ExtendedInstructionSets: string list
        MemoryModel: MemoryModel
        EntryPoints: SpirvEntryPoint list
        ExecutionModes: ExecutionMode list
        Body: SpirvTopLevelExpr
    }
