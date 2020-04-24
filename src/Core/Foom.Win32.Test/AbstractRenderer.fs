[<AutoOpen>]
module FsGame.Renderer.AbstractRenderer

open System
open System.Numerics
open System.Drawing
open System.Collections.Generic
open System.Collections.Concurrent
open FSharp.Quotations
open FSharp.Spirv
open FSharp.Spirv.Quotations
open FsGame.Core.Collections
open FsGame.Graphics.Vulkan
open System.Drawing.Imaging
open Microsoft.FSharp.NativeInterop

open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns
open FSharp.Spirv.Quotations.Intrinsics

#nowarn "9"

let checkShaderInputs stage (variables: FSharpSpirvQuotationVariable list) = 
    let layouts = ResizeArray()
    let inputs = ResizeArray()

    let getDescriptorKind (ty: FSharpSpirvType) (storageClass: StorageClass) =
        match storageClass with
        | StorageClass.Uniform -> UniformBufferDescriptor
        | StorageClass.UniformConstant when ty.IsSampledImage -> CombinedImageSamplerDescriptor
        | StorageClass.StorageBuffer -> StorageBufferDescriptor
        | _ -> failwithf "Storage class, %A, not supported yet." storageClass

    // TODO: Maybe expose descriptor set number?
    // TODO: Expose 'layout' number instead of the internals determining it, then we can determine the 'layout' number here.

    variables
    |> List.iter (fun var ->
        if var.StorageClass <> StorageClass.Output then
            match var.Decorations |> List.tryFind (function Decoration.Binding _ -> true | _ -> false) with
            | Some(Decoration.Binding n) ->
                match var.Decorations |> List.tryFind (function Decoration.DescriptorSet _ -> true | _ -> false) with
                | Some _ -> 
                    let descriptorKind = getDescriptorKind var.Type var.StorageClass
                    layouts.Add(ShaderDescriptorLayout(descriptorKind, stage, n))
                | _ ->
                    if stage = VertexStage then
                        let rate =
                            var.CustomAnnoations
                            |> List.tryFind (fun x ->
                                match x with
                                | Coerce(NewUnionCase(caseInfo, _), _) when caseInfo.DeclaringType = typeof<VulkanShaderVertexInputRate> && caseInfo.Name = "PerInstance" -> true
                                | _ -> false)
                            |> Option.map (fun _ -> PerInstance)
                            |> Option.defaultValue PerVertex
                        inputs.Add(ShaderVertexInput(rate, var.ClrType, n))
            | _ -> ())

    (layouts |> List.ofSeq), (inputs |> List.ofSeq)

type FSharpSpirvQuotationCompilation with

    member this.EmitBytes() =
        use ms = new System.IO.MemoryStream 100
        if not (this.Emit ms) then
            failwithf "%A" (this.GetDiagnostics())
        let bytes = Array.zeroCreate (int ms.Length)
        ms.Position <- 0L
        ms.Read(bytes, 0, bytes.Length) |> ignore
        bytes

type FalGraphics with

    member this.CreateShader(vertex: Expr<unit -> unit>, fragment: Expr<unit -> unit>) =

        let vertexOptions =
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
        let vertexCompilation = FSharpSpirvQuotationCompilation.Create(vertexOptions, vertex)
        let vertexBytes = vertexCompilation.EmitBytes()

        let fragmentOptions =
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
        let fragmentCompilation = FSharpSpirvQuotationCompilation.Create(fragmentOptions, fragment)
        let fragmentBytes = fragmentCompilation.EmitBytes()

        let vertexLayouts, vertexInputs = checkShaderInputs VertexStage (vertexCompilation.GetVariables())
        let fragmentLayouts, _ = checkShaderInputs FragmentStage (fragmentCompilation.GetVariables())

        let shaderDesc =
            let layoutSet = HashSet()
            let layouts = ResizeArray()

            // Combine descriptors into one if they are in both vertex and fragment shaders.
            vertexLayouts
            |> List.iter (fun l1 ->
                fragmentLayouts
                |> List.iter (fun l2 ->
                    if l1.Kind = l2.Kind && l1.Binding = l2.Binding then
                        layoutSet.Add(l1.Binding) |> ignore
                        layouts.Add(ShaderDescriptorLayout(l1.Kind, AllGraphicsStage, l1.Binding))))

            vertexLayouts
            |> List.iter (fun l ->
                if not (layoutSet.Contains l.Binding) then
                    layoutSet.Add l.Binding |> ignore
                    layouts.Add l)

            fragmentLayouts
            |> List.iter (fun l ->
                if not (layoutSet.Contains l.Binding) then
                    layoutSet.Add l.Binding |> ignore
                    layouts.Add l)

            // TODO: We have to order the layouts by binding.
            //       Instead, we should be explicit on which number the descriptor is in - requires changes internally.
            let layouts =
                layouts
                |> List.ofSeq
                |> List.sortBy (fun l -> l.Binding)

            Shader(layouts, vertexInputs)

        this.CreateShader(shaderDesc, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)

    member this.CreateImage(bmp: Bitmap) =
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        let image = this.CreateImage(bmp.Width, bmp.Height, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image
