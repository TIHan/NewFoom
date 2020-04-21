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
open FSharp.Spirv.Quotations.TypedTree
open FSharp.Spirv.Quotations.Intrinsics

#nowarn "9"

let checkShaderInputs stage (expr: Expr<unit -> unit>) = 
    let layouts = ResizeArray()
    let inputs = ResizeArray()

    let getDescriptorKind (ty: SpirvType) (storageClass: StorageClass) =
        match ty, storageClass with
        | _, StorageClass.Uniform -> UniformBufferDescriptor
        | SpirvTypeSampledImage _, StorageClass.UniformConstant -> CombinedImageSamplerDescriptor
        | _, StorageClass.StorageBuffer -> StorageBufferDescriptor
        | _ -> failwithf "Storage class, %A, not supported yet." storageClass

    // TODO: Maybe expose descriptor set number?
    // TODO: Expose 'layout' number instead of the internals determining it, then we can determine the 'layout' number here.

    let rec loop expr =
        match expr with
        | Sequential(expr1, expr2) ->
            loop expr1
            loop expr2
        | Let(var, rhs, body) ->
            match rhs with
            | SpecificCall <@ Variable<_> @> (None, [_], args) ->
                let spvVar, customAnnoations = Checker.CheckVariable var args
                if not spvVar.IsMutable then
                    match spvVar.Decorations |> List.tryFind (function Decoration.Binding _ -> true | _ -> false) with
                    | Some(Decoration.Binding n) ->
                        match spvVar.Decorations |> List.tryFind (function Decoration.DescriptorSet _ -> true | _ -> false) with
                        | Some _ -> 
                            let descriptorKind = getDescriptorKind spvVar.Type spvVar.StorageClass
                            layouts.Add(ShaderDescriptorLayout(descriptorKind, stage, n))
                        | _ ->
                            if stage = VertexStage then
                                let rate =
                                    customAnnoations
                                    |> List.tryFind (fun x ->
                                        match x with
                                        | Coerce(NewUnionCase(caseInfo, _), _) when caseInfo.DeclaringType = typeof<VulkanShaderVertexInputRate> && caseInfo.Name = "PerInstance" -> true
                                        | _ -> false)
                                    |> Option.map (fun _ -> PerInstance)
                                    |> Option.defaultValue PerVertex
                                inputs.Add(ShaderVertexInput(rate, var.Type, n))
                    | _ -> ()
            | _ -> ()

            loop body
        | _ ->
            ()

    loop expr

    expr, (layouts |> List.ofSeq), (inputs |> List.ofSeq)

type FalGraphics with

    member this.CreateShader(vertex: Expr<unit -> unit>, fragment: Expr<unit -> unit>) =

        let vertex, vertexLayouts, vertexInputs = checkShaderInputs VertexStage vertex
        let fragment, fragmentLayouts, _ = checkShaderInputs FragmentStage fragment

        // TODO: Maybe the spirv gen info can be placed in the shader itself? Would require changes to quotation translator.
        let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Vertex, [Capability.Shader], [])
        let spvVertex =
            Checker.Check vertex
            |> SpirvGen.GenModule spvVertexInfo

        let spvFragmentInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.Fragment, [Capability.Shader], ["GLSL.std.450"], ExecutionMode.OriginUpperLeft)
        let spvFragment = 
            Checker.Check fragment
            |> SpirvGen.GenModule spvFragmentInfo

        let vertexBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvVertex)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes
        let fragmentBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvFragment)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes


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

            Shader(0, true, layouts, vertexInputs)

        this.CreateShader(shaderDesc, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)

    member this.CreateImage(bmp: Bitmap) =
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        let image = this.CreateImage(bmp.Width, bmp.Height, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image
