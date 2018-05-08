module Foom.Renderer.GL.ShaderQuotation.GlslGen

open GlslAst

let testMeshVertex =

    let multiplyOp_mat4x4_mat4x4 = 
        GlslFunction("*", 
            [
                GlslParameter("m0", GlslType.Matrix4x4)
                GlslParameter("m1", GlslType.Matrix4x4)
            ], GlslType.Matrix4x4, GlslExpr.Internal)

    let multiplyOp_mat4x4_vec4 = 
        GlslFunction("*", 
            [
                GlslParameter("m0", GlslType.Matrix4x4)
                GlslParameter("v0", GlslType.Vector4 GlslVectorType.Float)
            ], GlslType.Vector4 GlslVectorType.Float, GlslExpr.Internal)

    let projection = mkVar "projection" GlslType.Matrix4x4
    let view = mkVar "view" GlslType.Matrix4x4
    let position = mkVar "position" (GlslType.Vector3 GlslVectorType.Float)
    let vec4_ctor = 
        GlslFunction("vec4.ctor", 
            [
                GlslParameter("xyz", GlslType.Vector3 GlslVectorType.Float)
                GlslParameter("w", GlslType.Float)
            ], GlslType.Float, GlslExpr.Internal
        )
    {
        uniforms = 
            [
            projection
            view
        ]
        ins = 
            [
            position
            mkVar "uv" (GlslType.Vector2 GlslVectorType.Float)
            mkVar "color" (GlslType.Vector4 GlslVectorType.Float)
        ]
        outs = 
            [
            mkVar "out_uv" (GlslType.Vector2 GlslVectorType.Float)
            mkVar "out_color" (GlslType.Vector4 GlslVectorType.Float)
        ]
        funcs = 
            [
            GlslFunction("main", [], GlslType.Void,
                GlslExpr.DeclareVar("snapToPixel", GlslType.Vector4 GlslVectorType.Float,
                    GlslExpr.Call(multiplyOp_mat4x4_vec4,
                        [
                            GlslExpr.Call(multiplyOp_mat4x4_vec4,
                                [
                                    GlslExpr.Var projection
                                    GlslExpr.Var view
                                ]
                            )
                            GlslExpr.Call(vec4_ctor,
                                [
                                    GlslExpr.Var position
                                    GlslExpr.Literal(GlslLiteral.Float 1.f)
                                ]
                            )
                        ]
                    ), 
                    GlslExpr.DeclareVar("vertex", GlslType.Vector4 GlslVectorType.Float,
                        GlslExpr.Var(mkMutableVar "snapToPixel" (GlslType.Vector4 GlslVectorType.Float)),
                        GlslExpr.NoOp
                    )
                )
            )
        ]
    }