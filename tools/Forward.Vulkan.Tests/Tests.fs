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
    for i = 0 to 10 do
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


[<Struct;Block>]
type TestBlock2 =
    {
        x: single[]
    }


[<Fact>]
let ``My test 2`` () =
    use device = createDevice ()
    use compute = createCompute device

    let computeShader =
        <@
            let test = Variable<TestBlock2> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.StorageBuffer []
    
            fun () ->
                if test.x.[0] = 1.f then
                    if test.x.[1] = 2.f then
                        test.x.[4] <- 100.f
        @>

    let testBuffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, [|1.f;2.f;3.f;4.f;5.f|])

    let computeShader = compute.CreateShader computeShader

    let draw = computeShader.CreateDrawBuilder()
    let draw = draw.AddDescriptorBuffer testBuffer
    let draw = draw.Next

    computeShader.AddDraw(draw, 0u, 1u) |> ignore
    compute.SetupCommands()

    compute.DrawFrame()
    compute.WaitIdle()
    let data = testBuffer.Memory.MapAsSpan<single>(5)

    Assert.Equal(100.f, data.[4])


[<Fact>]
let ``My test 3`` () =
    use device = createDevice ()
    use compute = createCompute device

    let computeShader =
        <@
            let test = Variable<TestBlock2> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.StorageBuffer []
    
            fun () ->
                let x =
                    if test.x.[0] = 1.f then
                        let y =
                            if test.x.[1] = 2.f then
                                if test.x.[2] = 3.f then

                                    let mutable z = 1.f
                                    if test.x.[3] = 4.f then
                                        z <- 167.f
                                    z
                                else
                                    99.f
                            else
                                88.f
                        y
                    else
                        1.f
                test.x.[4] <- x
        @>

    let testBuffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, [|1.f;2.f;3.f;4.f;5.f|])

    let computeShader = compute.CreateShader computeShader

    let draw = computeShader.CreateDrawBuilder()
    let draw = draw.AddDescriptorBuffer testBuffer
    let draw = draw.Next

    computeShader.AddDraw(draw, 0u, 1u) |> ignore
    compute.SetupCommands()

    compute.DrawFrame()
    compute.WaitIdle()
    let data = testBuffer.Memory.MapAsSpan<single>(5)

    Assert.Equal(167.f, data.[4])


[<Struct;NoComparison;NoEquality>]
type SectorRender =
    {
        OriginalFloorHeight: single
        OriginalCeilingHeight: single
        FloorHeight: single
        CeilingHeight: single
    }

[<Struct;NoComparison;NoEquality;Block>]
type SectorRendersBlock =
    {
        SectorRenders: SectorRender[]
    }

    
[<Fact>]
let ``My test 4`` () =
    use device = createDevice ()
    use compute = createCompute device

    let computeShader =
        <@
            let test1 = Variable<SectorRendersBlock> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.StorageBuffer []
            let test2 = Variable<TestBlock2> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.StorageBuffer []
    
            fun () ->
                test2.x.[1] <- test1.SectorRenders.[200].OriginalCeilingHeight + test1.SectorRenders.[1].OriginalFloorHeight + test1.SectorRenders.[1].FloorHeight + test1.SectorRenders.[1].CeilingHeight
        @>

    let test1Buffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, Array.init 1600 (fun i -> { OriginalCeilingHeight = single i; OriginalFloorHeight = single i; FloorHeight = single i; CeilingHeight = single i }))
    let test2Buffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, Array.init 1600 (fun i -> single i))

    let computeShader = compute.CreateShader computeShader

    let draw = computeShader.CreateDrawBuilder()
    let draw = draw.AddDescriptorBuffer test1Buffer
    let draw = draw.AddDescriptorBuffer test2Buffer
    let draw = draw.Next

    computeShader.AddDraw(draw, 0u, 1u) |> ignore
    compute.SetupCommands()

    compute.DrawFrame()
    compute.WaitIdle()
    let data = test2Buffer.Memory.MapAsSpan<single>(16)

    Assert.Equal(203.f, data.[1])


[<Fact>]
let ``My test 5`` () =
    use device = createDevice ()
    use compute = createCompute device

    let computeShader =
        <@
            let test1 = Variable<SectorRendersBlock> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.StorageBuffer []
            let test2 = Variable<TestBlock2> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.StorageBuffer []
            let test3 = Variable<TestBlock> [Decoration.Binding 2u; Decoration.DescriptorSet 2u] StorageClass.StorageBuffer []
    
            fun () ->
                test2.x.[1] <- test1.SectorRenders.[test3.x.[200]].OriginalCeilingHeight + test1.SectorRenders.[1].OriginalFloorHeight + test1.SectorRenders.[1].FloorHeight + test1.SectorRenders.[1].CeilingHeight
        @>

    let test1Buffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, Array.init 1600 (fun i -> { OriginalCeilingHeight = single i; OriginalFloorHeight = single i; FloorHeight = single i; CeilingHeight = single i }))
    let test2Buffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, Array.init 1600 (fun i -> single i))
    let test3Buffer = compute.CreateBuffer(StorageBuffer, VulkanBufferFlags.SharedMemory, Array.init 1600 (fun i -> i))

    let computeShader = compute.CreateShader computeShader

    let draw = computeShader.CreateDrawBuilder()
    let draw = draw.AddDescriptorBuffer test1Buffer
    let draw = draw.AddDescriptorBuffer test2Buffer
    let draw = draw.AddDescriptorBuffer test3Buffer
    let draw = draw.Next

    computeShader.AddDraw(draw, 0u, 1u) |> ignore
    compute.SetupCommands()

    compute.DrawFrame()
    compute.WaitIdle()
    let data = test2Buffer.Memory.MapAsSpan<single>(16)

    Assert.Equal(203.f, data.[1])