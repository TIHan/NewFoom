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
    inherit AbstractRenderer<FalkanShader, struct(FalkanBuffer * FalkanBuffer), FalkanImage>()

    let mvpUniform = instance.CreateBuffer<ModelViewProjection>(1, FalkanBufferFlags.None, UniformBuffer)

    override _.CreateSpirvShaderCore(vertex, fragment) =
        let input1 = FalkanShaderVertexInput(PerVertex, 0u, 0u, typeof<Vector3>)
        let input2 = FalkanShaderVertexInput(PerVertex, 1u, 1u, typeof<Vector2>)

        let layout = 
            Shader(0,true,
                [
                    FalkanShaderDescriptorLayout(UniformBufferDescriptor, VertexStage, 0u)
                    FalkanShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 1u)
                ],
                [input1; input2])
        instance.CreateShader(layout, vertex, fragment)

    override _.CreateMeshCore(shader, vertices, uv, texture) =
        let verticesBuffer = instance.CreateBuffer<Vector3> (vertices.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(verticesBuffer, vertices)

        let uvBuffer = instance.CreateBuffer<Vector2> (uv.Length, FalkanBufferFlags.None, VertexBuffer)
        instance.FillBuffer(uvBuffer, uv)

        let draw = shader.CreateDrawBuilder()
        let draw = draw.AddDescriptorBuffer(mvpUniform, sizeof<ModelViewProjection>)
        let draw = draw.AddDescriptorImage(texture)
        let draw = draw.Next.AddVertexBuffer(verticesBuffer)
        let draw = draw.AddVertexBuffer(uvBuffer)
        shader.AddDraw(draw, uint32 vertices.Length, 1u)
        struct(verticesBuffer, uvBuffer)

    override _.CreateTextureCore(bmp) =
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let image = instance.CreateImage(bmp.Width, bmp.Height)
        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        instance.FillImage(image, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image

    override _.Dispose() =
        (instance :> IDisposable).Dispose()
        (device :> IDisposable).Dispose()

    static member CreateWin32(hwnd, hinstance, windowResized) =
        let device = VulkanDevice.CreateWin32(hwnd, hinstance, "App", "Engine", [VulkanDeviceLayer.LunarGStandardValidation], [])
        let subpasses =
            [RenderSubpass ColorDepthStencilSubpass]

        let instance = FalGraphics.Create (device, windowResized, subpasses)
        new VulkanRenderer(device, instance)