module rec Foom.Renderer.GL.ShaderQuotation.GlslAst 

[<RequireQualifiedAccess>]
type GlslVectorType =
    | Bool
    | Int
    | UInt
    | Float
    | Double

[<RequireQualifiedAccess>]
type GlslType =
    | Void
    | Bool
    | Int
    | UInt
    | Float
    | Double
    | Vector2 of GlslVectorType
    | Vector3 of GlslVectorType
    | Vector4 of GlslVectorType
    | Matrix4x4
    | Struct of fields: (string * GlslType) list

[<RequireQualifiedAccess>]
type GlslLiteral =
    | Bool of bool
    | Int of int
    | UInt of uint32
    | Float of float32
    | Double of float

type GlslParameter = GlslParameter of name: string * GlslType
type GlslFunction = GlslFunction of name: string * parms: GlslParameter list * ret: GlslType * body: GlslExpr
type GlslVar = GlslVar of name: string * typ: GlslType * isMutable: bool

let mkVar name typ = GlslVar(name, typ, false)
let mkMutableVar name typ = GlslVar(name, typ, true)
let mkParameter name typ = GlslParameter(name, typ)
let mkFunction name parms ret body = GlslFunction(name, parms, ret, body)
let mkLiteralBool value = GlslLiteral.Bool(value)
let mkLiteralInt value =  GlslLiteral.Int(value)
let mkLiteralUInt value = GlslLiteral.UInt(value)
let mkLiteralFloat value = GlslLiteral.Float(value)
let mkLiteralDouble value = GlslLiteral.Double(value)

[<RequireQualifiedAccess>]
type GlslExpr =
    | NoOp
    | Internal
    | Call of func: GlslFunction * exprList: GlslExpr list
    | Literal of GlslLiteral
    | Var of GlslVar
    | VarSet of GlslVar * GlslExpr
    | Sequential of GlslExpr * GlslExpr
    | DeclareVar of GlslVar * body: GlslExpr * next: GlslExpr

type GlslModule = 
    {
        uniforms: GlslVar list
        ins: GlslVar list 
        outs: GlslVar list 
        funcs: GlslFunction list
    }

module GlslModule =

    let empty =
        {
            uniforms = []
            ins = []
            outs = []
            funcs = []
        }