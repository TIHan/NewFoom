module FSharp.Spirv.Quotations.TypedTree

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

    /// 32-bit boolean.
    | SpirvTypeBool

    | SpirvTypeInt of width: int * sign: bool
    | SpirvTypeFloat of width: int
    | SpirvTypeVector2
    | SpirvTypeVector2Int
    | SpirvTypeVector3
    | SpirvTypeVector4
    | SpirvTypeMatrix4x4
    | SpirvTypeArray of SpirvType * length: int
    | SpirvTypeRuntimeArray of SpirvType
    | SpirvTypeStruct of name: string * fields: SpirvField list * isBlock: bool
    | SpirvTypeImage of SpirvImageType
    | SpirvTypeSampler
    | SpirvTypeSampledImage of SpirvImageType

    member x.Name =
        match x with
        | SpirvTypeVoid -> "void"
        | SpirvTypeBool -> "bool"
        | SpirvTypeInt (width, sign) -> "int" + string width + "(" + string sign + ")"
        | SpirvTypeFloat width -> "float" + string width
        | SpirvTypeVector2 -> "Vector2"
        | SpirvTypeVector2Int -> "Vector2<int>"
        | SpirvTypeVector3 -> "Vector3"
        | SpirvTypeVector4 -> "Vector4"
        | SpirvTypeMatrix4x4 -> "Matrix4x4"
        | SpirvTypeArray (elementTy, length) -> elementTy.Name + "[" + string length + "]"
        | SpirvTypeRuntimeArray elementTy -> elementTy.Name + "[]"
        | SpirvTypeStruct (name=name) -> name
        | SpirvTypeImage _ -> "Image"
        | SpirvTypeSampler -> "Sampler"
        | SpirvTypeSampledImage _ -> "SampledImage"

    member x.Size: int =
        match x with
        | SpirvTypeVoid -> failwith "Unable to get size of void type."
        | SpirvTypeBool -> sizeof<bool>
        | SpirvTypeInt (width, _) -> width / 8
        | SpirvTypeFloat width -> width / 8
        | SpirvTypeVector2
        | SpirvTypeVector2Int -> sizeof<Vector2>
        | SpirvTypeVector3 -> sizeof<Vector3>
        | SpirvTypeVector4 -> sizeof<Vector4>
        | SpirvTypeMatrix4x4 -> sizeof<Matrix4x4>
        | SpirvTypeArray(ty, length) -> ty.Size * length
        | SpirvTypeStruct(_, fields, _) ->
            fields
            |> List.sumBy(fun (field: SpirvField) -> field.Type.Size)
        | SpirvTypeRuntimeArray _
        | SpirvTypeImage _
        | SpirvTypeSampler
        | SpirvTypeSampledImage _ -> failwith "Unable to get size of opaque type."

    member x.IsVoid = match x with SpirvTypeVoid -> true | _ -> false

    member x.IsStruct = match x with SpirvTypeStruct _ -> true | _ -> false

    member x.IsOpaque =
        match x with
        | SpirvTypeRuntimeArray _
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

let SpirvTypeUInt8 = SpirvTypeInt (8, false)
let SpirvTypeInt32 = SpirvTypeInt (32, true)
let SpirvTypeUInt32 = SpirvTypeInt (32, false)
let SpirvTypeFloat32 = SpirvTypeFloat 32

type SpirvConst =
    | SpirvConstBool of bool * decorations: Decorations
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
        | SpirvConstBool (decorations=decorations)
        | SpirvConstInt (decorations=decorations)
        | SpirvConstUInt32 (decorations=decorations)
        | SpirvConstSingle (decorations=decorations)
        | SpirvConstVector2 (decorations=decorations)
        | SpirvConstVector2Int (decorations=decorations)
        | SpirvConstVector3 (decorations=decorations)
        | SpirvConstVector4 (decorations=decorations)
        | SpirvConstMatrix4x4 (decorations=decorations)
        | SpirvConstArray (decorations=decorations) -> decorations

[<RequireQualifiedAccess>]
type SpirvSelectionControl =
    | None
    | Flatten
    | DontFlatten

type SpirvExpr =
    | SpirvNop
    | SpirvUnreachable
    | SpirvConst of SpirvConst
    | SpirvLet of SpirvVar * rhs: SpirvExpr * body: SpirvExpr
    | SpirvSequential of SpirvExpr * SpirvExpr
    | SpirvNewVector2 of args: SpirvExpr list
    | SpirvNewVector2Int of args: SpirvExpr list
    | SpirvNewVector3 of args: SpirvExpr list
    | SpirvNewVector4 of args: SpirvExpr list
    | SpirvArrayIndexerGet of receiver: SpirvExpr * arg: SpirvExpr * retTy: SpirvType
    | SpirvArrayIndexerSet of receiver: SpirvExpr * indexArg: SpirvExpr * valueArg: SpirvExpr
    | SpirvVar of SpirvVar
    | SpirvVarSet of SpirvVar * SpirvExpr
    | SpirvExprOp of SpirvExprOp
    | SpirvIntrinsicFieldGet of SpirvIntrinsicFieldGet
    | SpirvFieldGet of receiver: SpirvExpr * index: int

    // Control flow

    | SpirvIfThenElse of condition: SpirvExpr * truePath: SpirvExpr * falsePath: SpirvExpr

    member x.Type =
        let rec getType expr =
            match expr with
            | SpirvNop -> 
                SpirvTypeVoid
            | SpirvUnreachable ->
                SpirvTypeVoid
            | SpirvConst spvConst ->
                match spvConst with
                | SpirvConstBool _ -> SpirvTypeBool
                | SpirvConstInt _ -> SpirvTypeInt32
                | SpirvConstUInt32 _ -> SpirvTypeUInt32
                | SpirvConstSingle _ -> SpirvTypeFloat 32
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
            | SpirvNewVector2Int _ ->
                SpirvTypeVector2Int
            | SpirvNewVector3 _ ->
                SpirvTypeVector3
            | SpirvNewVector4 _ ->
                SpirvTypeVector4
            | SpirvArrayIndexerGet(_, _, retTy) ->
                retTy
            | SpirvArrayIndexerSet _ ->
                SpirvTypeVoid
            | SpirvVar spvVar ->
                spvVar.Type
            | SpirvVarSet _ ->
                SpirvTypeVoid
            | SpirvExprOp call ->
                call.ReturnType
            | SpirvIntrinsicFieldGet fieldGet ->
                fieldGet.Type
            | SpirvFieldGet (receiver, index) ->
                match receiver.Type with
                | SpirvTypeStruct (_, fields, _) -> fields.[index].Type
                | _ -> failwith "Invalid field get."
            | SpirvIfThenElse(_, trueExpr, _) -> 
                trueExpr.Type
        getType x

and SpirvExprOp =
    // TODO: Fix transform and multiply names
    | Transform__Vector4_Matrix4x4__Vector4 of vector4: SpirvExpr * matrix4x4: SpirvExpr
    | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 of matrix4x4_1: SpirvExpr * matrix4x4_2: SpirvExpr
    | ConvertAnyFloatToAnySInt of arg: SpirvExpr
    | ConvertSIntToFloat of arg: SpirvExpr
    | GetImage of arg: SpirvExpr
    | ImageFetch of image: SpirvExpr * coordinate: SpirvExpr * retTy: SpirvType
    | ImageGather of sampledImage: SpirvExpr * coordinate: SpirvExpr * comp: SpirvExpr * retTy: SpirvType
    | VectorShuffle of arg1: SpirvExpr * arg2: SpirvExpr * arg3: uint32 list * retTy: SpirvType
    | ImplicitLod of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | Kill
    | FloatUnorderedLessThan of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | FloatAdd of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | FloatSubtract of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | FloatMultiply of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | FloatDivide of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | VectorTimesScalar of arg1: SpirvExpr * arg2: SpirvExpr * retTy: SpirvType
    | SpirvOp of (IdResultType -> IdResult -> IdRef list -> Instruction) * args: SpirvExpr list * retTy: SpirvType

    static member Create(op, retTy) =
        let op = fun idResTy idRes _ -> op(idResTy, idRes)
        SpirvOp(op, [], retTy)

    static member Create(op, arg, retTy) =
        let op = 
            fun idResTy idRes args ->
                match args with [arg] -> op(idResTy, idRes, [arg]) | _ -> failwith "should not happen"
        SpirvOp(op, [arg], retTy)

    static member Create(op, arg1, arg2, retTy) =
        let op = 
            fun idResTy idRes args ->
                match args with [arg1;arg2] -> op(idResTy, idRes, arg1, arg2) | _ -> failwith "should not happen"
        SpirvOp(op, [arg1;arg2], retTy)

    member x.ReturnType =
        match x with
        | Transform__Vector4_Matrix4x4__Vector4 _ -> SpirvTypeVector4
        | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 _ -> SpirvTypeMatrix4x4
        | ConvertAnyFloatToAnySInt arg ->
            match arg.Type with
            | SpirvTypeFloat width -> 
                if width = 32 then
                    SpirvTypeInt32
                else
                    SpirvTypeInt (width, true)
            | SpirvTypeVector2 -> SpirvTypeVector2Int
            | _ -> failwith "ConvertAnyFloatToAnySInt: Expected SpirvTypeFloat."
        | ConvertSIntToFloat arg ->
            match arg.Type with
            | SpirvTypeInt (width, true) -> 
                if width = 32 then
                    SpirvTypeFloat32
                else
                    SpirvTypeFloat width
            | SpirvTypeVector2Int -> SpirvTypeVector2
            | _ -> failwith "ConvertAnyFloatToAnySInt: Expected SpirvTypeFloat."
        | GetImage arg ->
            match arg.Type with
            | SpirvTypeSampledImage imageTy -> SpirvTypeImage imageTy
            | _ -> failwith "GetImage: Expected SpirvTypeSampledImage."
        | ImageFetch (_, _, retTy) ->
            match retTy with
            | SpirvTypeVector4 _ -> retTy
            | _ -> failwith "ImageFetch: Expected SpirvTypeVector4."
        | ImageGather (_, _, _, retTy) -> retTy
        | VectorShuffle (_, _, _, retTy) -> retTy
        | ImplicitLod (_, _, retTy) -> retTy
        | Kill -> SpirvType.SpirvTypeVoid
        | FloatUnorderedLessThan(_, _, retTy) -> retTy
        | FloatAdd(_, _, retTy) -> retTy
        | FloatSubtract(_, _, retTy) -> retTy
        | FloatMultiply(_, _, retTy) -> retTy
        | FloatDivide(_, _, retTy) -> retTy
        | VectorTimesScalar(_, _, retTy) -> retTy
        | SpirvOp(_, _, retTy) -> retTy

    member x.Arguments =
        match x with
        | Transform__Vector4_Matrix4x4__Vector4 (arg1, arg2)
        | Multiply__Matrix4x4_Matrix4x4__Matrix4x4 (arg1, arg2) -> [arg1;arg2]
        | ConvertAnyFloatToAnySInt arg -> [arg]
        | ConvertSIntToFloat arg -> [arg]
        | GetImage arg -> [arg]
        | ImageFetch (arg1, arg2, _) -> [arg1;arg2]
        | ImageGather (arg1, arg2, arg3, _) -> [arg1;arg2;arg3]
        | VectorShuffle (arg1, arg2, _, _) -> [arg1;arg2]
        | ImplicitLod (arg1, arg2, _) -> [arg1;arg2]
        | Kill -> []
        | FloatUnorderedLessThan(arg1, arg2, _) -> [arg1;arg2]
        | FloatAdd(arg1, arg2, _) -> [arg1;arg2]
        | FloatSubtract(arg1, arg2, _) -> [arg1;arg2]
        | FloatMultiply(arg1, arg2, _) -> [arg1;arg2]
        | FloatDivide(arg1, arg2, _) -> [arg1;arg2]
        | VectorTimesScalar(arg1, arg2, _) -> [arg1;arg2]
        | SpirvOp(_, args, _) -> args


and SpirvIntrinsicFieldGet =
    | Vector2_Get_X of receiver: SpirvExpr * typ: SpirvType
    | Vector2_Get_Y of receiver: SpirvExpr * typ: SpirvType

    | Vector2Int_Get_X of receiver: SpirvExpr * typ: SpirvType
    | Vector2Int_Get_Y of receiver: SpirvExpr * typ: SpirvType

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

        | Vector2Int_Get_X(typ=typ)
        | Vector2Int_Get_Y(typ=typ)

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