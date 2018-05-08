module internal Foom.Renderer.GL.ShaderQuotation.GlslGen

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

    let projection = GlslVal("projection", GlslType.Matrix4x4)
    let view = GlslVal("view", GlslType.Matrix4x4)
    let position = GlslVal("position", GlslType.Vector3 GlslVectorType.Float)
    let vec4_ctor = 
        GlslFunction("vec4.ctor", 
            [
                GlslParameter("xyz", GlslType.Vector3 GlslVectorType.Float)
                GlslParameter("w", GlslType.Float)
            ], GlslType.Float, GlslExpr.Internal
        )
    GlslModule(
        [
            projection
            view
        ],
        [
            position
            GlslVal("uv", GlslType.Vector2 GlslVectorType.Float)
            GlslVal("color", GlslType.Vector4 GlslVectorType.Float)
        ],
        [
            GlslVar("out_uv", GlslType.Vector2 GlslVectorType.Float)
            GlslVar("out_color", GlslType.Vector4 GlslVectorType.Float)
        ],
        [
            GlslFunction("main", [], GlslType.Void,
                GlslExpr.DeclareVar("snapToPixel", GlslType.Vector4 GlslVectorType.Float,
                    GlslExpr.Call(multiplyOp_mat4x4_vec4,
                        [
                            GlslExpr.Call(multiplyOp_mat4x4_vec4,
                                [
                                    GlslExpr.Val projection
                                    GlslExpr.Val view
                                ]
                            )
                            GlslExpr.Call(vec4_ctor,
                                [
                                    GlslExpr.Val position
                                    GlslExpr.Literal(GlslLiteral.Float 1.f)
                                ]
                            )
                        ]
                    ), 
                    GlslExpr.DeclareVar("vertex", GlslType.Vector4 GlslVectorType.Float,
                        GlslExpr.Val(GlslVal("snapToPixel", GlslType.Vector4 GlslVectorType.Float)),
                        GlslExpr.NoOp
                    )
                )
            )
        ]
    )