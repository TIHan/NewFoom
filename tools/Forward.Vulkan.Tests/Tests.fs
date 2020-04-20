module Tests

open System
open Xunit
open Xunit.Sdk
open FsGame.Graphics.Vulkan

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
open Microsoft.FSharp.NativeInterop

open FSharp.Quotations
open FSharp.Quotations.DerivedPatterns
open FSharp.Quotations.ExprShape
open FSharp.Quotations.Patterns
open FSharp.Spirv.Quotations.TypedTree

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

    member this.CreateShader(compute: Expr<unit -> unit>) =

        let vertex, vertexLayouts, vertexInputs = checkShaderInputs ComputeStage compute

        let spvVertexInfo = SpirvGenInfo.Create(AddressingModel.Logical, MemoryModel.GLSL450, ExecutionModel.GLCompute, [Capability.Shader], [], ExecutionMode.LocalSize(1u, 1u, 1u))
        let spvVertex =
            Checker.Check vertex
            |> SpirvGen.GenModule spvVertexInfo

        let vertexBytes =
            use ms = new System.IO.MemoryStream 100
            SpirvModule.Serialize (ms, spvVertex)
            let bytes = Array.zeroCreate (int ms.Length)
            ms.Position <- 0L
            ms.Read(bytes, 0, bytes.Length) |> ignore
            bytes


        let shaderDesc =
            let layoutSet = HashSet()
            let layouts = ResizeArray()

            vertexLayouts
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

        this.CreateComputeShader(shaderDesc, ReadOnlySpan vertexBytes)

let createDevice () =
    let evt = Event<string>()
    let device = VulkanDevice.CreateCompute("Vulkan Tests", "forward", [VulkanDeviceLayer.LunarGStandardValidation], [], evt.Trigger)
    evt.Publish.Add(fun str -> failwithf "%s" str)
    device

let createCompute device =
    FalGraphics.CreateCompute(device, Event<unit>().Publish)






[<Struct;Block>]
type TestBlock =
    {
        x: int[]
    }

[<Fact>]
let ``My test`` () =
    use device = createDevice ()
    use compute = createCompute device

    let computeShader =
        <@
            let test = Variable<TestBlock> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.StorageBuffer []
    
            fun () ->
                test.x.[16] <- 5
        @>

    let testBuffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, Array.init 10000 (fun _ -> 0))

    let computeShader = compute.CreateShader computeShader

    let draw = computeShader.CreateDrawBuilder()
    let draw = draw.AddDescriptorBuffer testBuffer
    let draw = draw.Next

    computeShader.AddDraw(draw, 0u, 1u) |> ignore
    compute.SetupCommands()

    compute.DrawFrame()

    let doot = testBuffer.Memory.MapAsSpan<int>(20)


    Assert.True(true)
