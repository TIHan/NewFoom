module Tests

open System.IO
open System.Numerics
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open FSharp.Spirv.Quotations.Intrinsics
open Xunit

[<Fact>]
let ``Compiler Vertex`` () =
    let vertex =
        <@
            let positions =
                [|
                    Vector2 (0.f, -0.5f)
                    Vector2 (0.5f, 0.5f)
                    Vector2 (-0.5f, 0.5f)
                |]

            let colors =
                [|
                    Vector3 (1.f, 0.f, 0.f)
                    Vector3 (0.f, 1.f, 0.f)
                    Vector3 (0.f, 0.f, 1.f)
                |]
       
            let gl_VertexIndex = Variable<int> [Decoration.BuiltIn BuiltIn.VertexIndex] StorageClass.Input []
            let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output []
            let mutable fragColor = Variable<Vector3> [Decoration.Location 0u] StorageClass.Output []

            fun () ->
                gl_Position <- Vector4(positions.[gl_VertexIndex], 0.f, 1.f)
                fragColor <- colors.[gl_VertexIndex]
        @>

    let options =
        { 
            DebugEnabled = true
            OptimizationsEnabled = false
            Capabilities = [Capability.Shader]
            ExtendedInstructionSets = ["GLSL.std.450"]
            AddressingModel = AddressingModel.Logical
            MemoryModel = MemoryModel.GLSL450
            ExecutionModel = ExecutionModel.Vertex
            ExecutionMode = None
        }
    let compilation = FSharpSpirvQuotationCompilation.Create(options, vertex)
    use ms = new MemoryStream()
    Assert.True(compilation.Emit ms)

[<Fact>]
let ``Compiler Fragment`` () =
    let fragment = 
        <@ 
            let fragColor = Variable<Vector3> [Decoration.Location 0u] StorageClass.Input []
            let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output []

            fun () -> outColor <- Vector4(fragColor, 1.f)
        @>

    let options =
        { 
            DebugEnabled = true
            OptimizationsEnabled = false
            Capabilities = [Capability.Shader]
            ExtendedInstructionSets = ["GLSL.std.450"]
            AddressingModel = AddressingModel.Logical
            MemoryModel = MemoryModel.GLSL450
            ExecutionModel = ExecutionModel.Fragment
            ExecutionMode = Some(ExecutionMode.OriginUpperLeft)
        }
    let compilation = FSharpSpirvQuotationCompilation.Create(options, fragment)
    use ms = new MemoryStream()
    Assert.True(compilation.Emit ms)

[<Fact>]
let ``Compiler Vertex - 2`` () =
    let vertex =
        <@
            let uni_projection = Variable<Matrix4x4> [Decoration.Uniform] StorageClass.Uniform []
            let uni_view = Variable<Matrix4x4> [Decoration.Uniform] StorageClass.Uniform []

            let position = Variable<Vector3> [Decoration.Location 0u] StorageClass.Input []
            let in_uv = Variable<Vector2> [Decoration.Location 1u] StorageClass.Input []
            let in_color = Variable<Vector4> [Decoration.Location 2u] StorageClass.Input []

            let mutable gl_Position = Variable<Vector3> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output []
            let mutable uv = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output []
            let mutable color = Variable<Vector4> [Decoration.Location 1u] StorageClass.Output []

            fun () ->
                let snapToPixel = Vector4.Transform(Vector4(position, 1.f), uni_view * uni_projection)
                let vertex = snapToPixel

                //vertex.xyz = snapToPixel.xyz / snapToPixel.w;
                //vertex.x = floor(160 * vertex.x) / 160;
                //vertex.y = floor(120 * vertex.y) / 120;
                //vertex.xyz = vertex.xyz * snapToPixel.w;

                gl_Position <- Vector3(vertex.X, vertex.Y, vertex.Z)
                uv <- in_uv
                color <- in_color
        @>

    let options =
        { 
            DebugEnabled = true
            OptimizationsEnabled = false
            Capabilities = [Capability.Shader]
            ExtendedInstructionSets = ["GLSL.std.450"]
            AddressingModel = AddressingModel.Logical
            MemoryModel = MemoryModel.GLSL450
            ExecutionModel = ExecutionModel.Vertex
            ExecutionMode = None
        }
    let compilation = FSharpSpirvQuotationCompilation.Create(options, vertex)
    use ms = new MemoryStream()
    Assert.True(compilation.Emit ms)
