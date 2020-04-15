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
    inherit AbstractRenderer<struct(FalkanBuffer * int), FalkanShader, struct(FalkanBuffer * int), FalkanImage>()

    override _.CreateUniformBufferCore<'T when 'T : unmanaged>(value: 'T) =
        let buffer = instance.CreateBuffer<'T>(1, FalkanBufferFlags.None, UniformBuffer)
        buffer.SetData(ReadOnlySpan [|value|])
        struct(buffer, sizeof<'T>)

    override _.SetUniformBufferCore<'T when 'T : unmanaged>(struct(internalUniformBuffer, _), value: 'T) =
        internalUniformBuffer.SetData(ReadOnlySpan [|value|])

    override _.CreateSpirvShaderCore<'T when 'T : unmanaged>(vertex, fragment) =
        let input = FalkanShaderVertexInput(PerVertex, 0u, 0u, typeof<'T>)

        let layout = 
            Shader(0,true,
                [
                    FalkanShaderDescriptorLayout(UniformBufferDescriptor, VertexStage, 0u)
                    FalkanShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 1u)
                ],
                [input])
        instance.CreateShader(layout, vertex, fragment)

    override _.CreateMeshCore<'T when 'T : unmanaged>(struct(cameraUniform, cameraUniformSize), shader, vertices: ReadOnlySpan<'T>, texture) =
        let verticesBuffer = instance.CreateBuffer<'T> (vertices.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(verticesBuffer, vertices)

        let draw = shader.CreateDrawBuilder()
        let draw = draw.AddDescriptorBuffer(cameraUniform, cameraUniformSize)
        let draw = draw.AddDescriptorImage(texture)
        let draw = draw.Next.AddVertexBuffer(verticesBuffer)
        let drawId = shader.AddDraw(draw, uint32 vertices.Length, 1u)
        struct(verticesBuffer, drawId)

    override _.CreateTextureCore(bmp) =
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let image = instance.CreateImage(bmp.Width, bmp.Height)
        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image

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