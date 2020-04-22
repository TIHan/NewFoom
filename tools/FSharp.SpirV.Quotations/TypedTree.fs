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

    | SpirvTypeBool of sizeHint: int

    | SpirvTypeInt of width: int * sign: bool
    | SpirvTypeFloat of width: int
    | SpirvTypeVector2
    | SpirvTypeVector3
    | SpirvTypeVector4
    | SpirvTypeMatrix4x4
    | SpirvTypeArray of SpirvType * length: int
    | SpirvTypeRuntimeArray of SpirvType
    | SpirvTypeStruct of name: string * fields: SpirvField list * isBlock: bool
    | SpirvTypeImage of SpirvImageType
    | SpirvTypeSampler
    | SpirvTypeSampledImage of SpirvImageType
    | SpirvTypeFunction of parameterTypes: SpirvType list * returnType: SpirvType
    | SpirvTypePointer of SpirvType * StorageClass

    member x.Name =
        match x with
        | SpirvTypeVoid -> "Void"
        | SpirvTypeBool _ -> "Bool"
        | SpirvTypeInt (width, sign) -> "Int" + string width + "(" + string sign + ")"
        | SpirvTypeFloat width -> "Float" + string width
        | SpirvTypeVector2 -> "Vector2"
        | SpirvTypeVector3 -> "Vector3"
        | SpirvTypeVector4 -> "Vector4"
        | SpirvTypeMatrix4x4 -> "Matrix4x4"
        | SpirvTypeArray (elementTy, length) -> elementTy.Name + "[" + string length + "]"
        | SpirvTypeRuntimeArray elementTy -> elementTy.Name + "[]"
        | SpirvTypeStruct (name=name) -> name
        | SpirvTypeImage _ -> "Image"
        | SpirvTypeSampler -> "Sampler"
        | SpirvTypeSampledImage _ -> "SampledImage"
        | SpirvTypeFunction(parTys, retTy) when parTys.IsEmpty -> "Function [Void -> " + retTy.Name + "]"
        | SpirvTypeFunction(parTys, retTy) -> "Function" + "[" + (parTys |> List.map (fun x -> x.Name) |> List.reduce (fun x y -> x + " -> " + y)) + " -> " + retTy.Name + "]"
        | SpirvTypePointer(ty, storageClass) -> "Pointer[" + ty.Name + "](" + string storageClass + ")"

    member x.SizeHint: int =
        match x with
        | SpirvTypeVoid -> failwith "Unable to get size of void type."
        | SpirvTypeBool sizeHint -> sizeHint
        | SpirvTypeInt (width, _) -> width / 8
        | SpirvTypeFloat width -> width / 8
        | SpirvTypeVector2 -> 8
        | SpirvTypeVector3 -> 12
        | SpirvTypeVector4 -> 16
        | SpirvTypeMatrix4x4 -> 64
        | SpirvTypeArray(ty, length) -> ty.SizeHint * length
        | SpirvTypeStruct(_, fields, _) ->
            fields
            |> List.sumBy(fun (field: SpirvField) -> field.Type.SizeHint)

        | SpirvTypePointer _
        | SpirvTypeFunction _
        | SpirvTypeRuntimeArray _
        | SpirvTypeImage _
        | SpirvTypeSampler
        | SpirvTypeSampledImage _ -> failwith "Unable to get size hint of type."

    member x.IsVoid = match x with SpirvTypeVoid -> true | _ -> false

    member x.IsStruct = match x with SpirvTypeStruct _ -> true | _ -> false

    member x.IsScalarSInt =
        match x with
        | SpirvTypeInt(_, true) -> true
        | _ -> false

    member x.IsScalarUInt =
        match x with
        | SpirvTypeInt(_, false) -> true
        | _ -> false

    member x.IsScalarFloat =
        match x with
        | SpirvTypeFloat _ -> true
        | _ -> false

    member x.IsVectorInt =
        false

    member x.IsVectorFloat =
        match x with
        | SpirvTypeVector2
        | SpirvTypeVector3
        | SpirvTypeVector4 -> true
        | _ -> false

    member x.IsMatrix4x4 =
        match x with
        | SpirvTypeMatrix4x4 -> true
        | _ -> false

    member x.IsOpaque =
        match x with
        | SpirvTypeImage _
        | SpirvTypeSampler
        | SpirvTypeSampledImage _ -> true
        | _ -> false

    member x.IsRuntimeArray =
        match x with
        | SpirvTypeRuntimeArray _ -> true
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
    | SpirvConstBool of bool * sizeHint: int * decorations: Decorations
    | SpirvConstInt of int * decorations: Decorations
    | SpirvConstUInt32 of uint32 * decorations: Decorations
    | SpirvConstSingle of single * decorations: Decorations
    | SpirvConstVector2 of single * single * decorations: Decorations
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
    | SpirvUnreachable
    | SpirvConst of SpirvConst
    | SpirvLet of SpirvVar * rhs: SpirvExpr * body: SpirvExpr
    | SpirvSequential of SpirvExpr * SpirvExpr
    | SpirvNewVector2 of args: SpirvExpr list
    | SpirvNewVector3 of args: SpirvExpr list
    | SpirvNewVector4 of args: SpirvExpr list
    | SpirvArrayIndexerGet of receiver: SpirvExpr * arg: SpirvExpr * retTy: SpirvType
    | SpirvArrayIndexerSet of receiver: SpirvExpr * indexArg: SpirvExpr * valueArg: SpirvExpr
    | SpirvVar of SpirvVar
    | SpirvVarSet of SpirvVar * SpirvExpr
    | SpirvExprOp of SpirvExprOp
    | SpirvIntrinsicFieldGet of SpirvIntrinsicFieldGet
    | SpirvFieldGet of receiver: SpirvExpr * index: int
    | SpirvCallFunction of SpirvVar * args: SpirvExpr list

    // Control flow

    | SpirvIfThenElse of condition: SpirvExpr * truePath: SpirvExpr * falsePath: SpirvExpr

    member x.Type =
        let rec getType expr =
            match expr with
            | SpirvUnreachable ->
                SpirvTypeVoid
            | SpirvConst spvConst ->
                match spvConst with
                | SpirvConstBool(_, sizeHint, _) -> SpirvTypeBool sizeHint
                | SpirvConstInt _ -> SpirvTypeInt32
                | SpirvConstUInt32 _ -> SpirvTypeUInt32
                | SpirvConstSingle _ -> SpirvTypeFloat 32
                | SpirvConstVector2 _ -> SpirvTypeVector2
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
            | SpirvArrayIndexerGet(_, _, retTy) ->
                retTy
            | SpirvArrayIndexerSet _ ->
                SpirvTypeVoid
            | SpirvVar var ->
                var.Type
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
            | SpirvCallFunction(var, _) -> 
                var.Type
            | SpirvIfThenElse(_, trueExpr, _) -> 
                trueExpr.Type
        getType x

and SpirvExprOp =
    | SpirvNop
    | SpirvOp of (IdResultType -> IdResult -> IdRef list -> Instruction) * args: SpirvExpr list * retTy: SpirvType

    static member Create(op, retTy) =
        let op = fun idResTy idRes _ -> op(idResTy, idRes)
        SpirvOp(op, [], retTy)

    static member Create(op, arg, retTy) =
        let op = 
            fun idResTy idRes args ->
                match args with 
                | [arg] -> op(idResTy, idRes, arg) 
                | _ -> invalidArg "args" "Argument count is not one."
        SpirvOp(op, [arg], retTy)

    static member Create(op, arg1, arg2, retTy) =
        let op = 
            fun idResTy idRes args ->
                match args with 
                | [arg1;arg2] -> op(idResTy, idRes, arg1, arg2) 
                | _ -> invalidArg "args" "Argument count is not two."
        SpirvOp(op, [arg1;arg2], retTy)

    member x.ReturnType =
        match x with
        | SpirvNop -> SpirvTypeVoid
        | SpirvOp(_, _, retTy) -> retTy

    member x.Arguments =
        match x with
        | SpirvNop -> []
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
    | SpirvTopLevelLet of SpirvVar * rhs: SpirvTopLevelExpr * body: SpirvTopLevelExpr
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