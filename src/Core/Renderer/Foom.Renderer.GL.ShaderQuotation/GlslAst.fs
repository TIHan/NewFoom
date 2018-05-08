module internal rec Foom.Renderer.GL.ShaderQuotation.GlslAst

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
type GlslVar = GlslVar of name: string * GlslType
type GlslVal = GlslVal of name: string * GlslType

[<RequireQualifiedAccess>]
type GlslExpr =
    | NoOp
    | Internal
    | Call of func: GlslFunction * exprList: GlslExpr list
    | Literal of GlslLiteral
    | Var of GlslVar
    | Val of GlslVal
    | DeclareVar of name: string * GlslType * body: GlslExpr * next: GlslExpr

type GlslModule = GlslModule of uniforms: GlslVal list * ins: GlslVal list * outs: GlslVar list * GlslFunction list