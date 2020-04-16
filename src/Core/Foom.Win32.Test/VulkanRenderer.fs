[<AutoOpen>]
module FsGame.Renderer.VulkanRenderer

open System
open System.Numerics
open System.Drawing
open System.Collections.Generic
open FSharp.Quotations
open FSharp.Spirv
open FSharp.Spirv.Quotations
open FsGame.Core.Collections
open FsGame.Graphics.Vulkan
open System.Drawing.Imaging
open Microsoft.FSharp.NativeInterop

#nowarn "9"

[<Sealed>]
type VulkanRenderer private (device: VulkanDevice, instance: FalGraphics) =
    inherit AbstractRenderer<struct(FalkanBuffer * int), struct(FalkanBuffer * int), FalkanImage, FalkanShader, int>()

    override _.CreateUniformBufferCore<'T when 'T : unmanaged>(value: 'T) =
        let buffer = instance.CreateBuffer<'T>(1, FalkanBufferFlags.None, UniformBuffer)
        buffer.SetData(ReadOnlySpan [|value|])
        struct(buffer, sizeof<'T>)

    override _.SetUniformBufferCore<'T when 'T : unmanaged>(struct(internalUniformBuffer, _), value: 'T) =
        internalUniformBuffer.SetData(ReadOnlySpan [|value|])


    override _.CreateVertexBufferCore<'T when 'T : unmanaged>(value: ReadOnlySpan<'T>) =
        let buffer = instance.CreateBuffer<'T>(value.Length, FalkanBufferFlags.None, VertexBuffer)
        buffer.SetData(value)
        struct(buffer, value.Length)

    override _.SetVertexBufferCore<'T when 'T : unmanaged>(struct(internalVertexBuffer, count), value: ReadOnlySpan<'T>) =
        if count <> value.Length then
            invalidArg "value" "Value span must be the same length as the buffer."
        internalVertexBuffer.SetData(value)


    override _.CreateTextureCore(bmp) =
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let image = instance.CreateImage(bmp.Width, bmp.Height)
        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image


    override _.CreateSpirvShaderCore(vertex, fragment, vertexType, instanceTypeOpt) =
        let inputs =
            [
                yield FalkanShaderVertexInput(PerVertex, 0u, vertexType)
                if instanceTypeOpt.IsSome then
                    yield FalkanShaderVertexInput(PerInstance, 1u, instanceTypeOpt.Value)
            ]

        let layout = 
            Shader(0,true,
                [
                    FalkanShaderDescriptorLayout(UniformBufferDescriptor, VertexStage, 0u)
                    FalkanShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 1u)
                ],
                inputs)
        instance.CreateShader(layout, vertex, fragment)

    override _.CreateDrawCallCore(struct(uniformBuffer, uniformBufferSize), struct(vertexBuffer, vertexCount), internalInstanceBufferOpt, texture, shader, instanceCount) =
        let draw = shader.CreateDrawBuilder()
        let draw = draw.AddDescriptorBuffer(uniformBuffer, uniformBufferSize)
        let draw = draw.AddDescriptorImage(texture)
        let draw = draw.Next.AddVertexBuffer(vertexBuffer)

        let draw =
            match internalInstanceBufferOpt with
            | Some struct(instanceBuffer, _) ->
                draw.AddInstanceBuffer instanceBuffer
            | _ ->
                draw
        shader.AddDraw(draw, uint32 vertexCount, uint32 instanceCount)

    override _.Refresh() =
        instance.SetupCommands()

    override _.Draw() =
        instance.DrawFrame()

    override _.WaitIdle() =
        instance.WaitIdle()

    override _.Dispose() =
        (instance :> IDisposable).Dispose()
        (device :> IDisposable).Dispose()

    static member CreateWin32(hwnd, hinstance, windowResized) =
        let device = VulkanDevice.CreateWin32(hwnd, hinstance, "App", "Engine", [VulkanDeviceLayer.LunarGStandardValidation], [])
        let subpasses =
            [RenderSubpass ColorDepthStencilSubpass]

        let instance = FalGraphics.Create (device, windowResized, subpasses)
        instance.SetupCommands()
        new VulkanRenderer(device, instance)