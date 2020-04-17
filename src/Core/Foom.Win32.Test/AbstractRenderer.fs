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

#nowarn "9"

type FalGraphics with

    member this.CreateShader(vertex: Expr<unit -> unit>, fragment: Expr<unit -> unit>, shaderDesc) =
        
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

        this.CreateShader(shaderDesc, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)

    member this.CreateImage(bmp: Bitmap) =
        let rect = Rectangle(0, 0, bmp.Width, bmp.Height)
        let data = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb)

        let ptr = data.Scan0 |> NativePtr.ofNativeInt<byte> |> NativePtr.toVoidPtr
        let image = this.CreateImage(bmp.Width, bmp.Height, ReadOnlySpan(ptr, data.Width * data.Height * 4))
        bmp.UnlockBits(data)
        image

[<Struct>]
type UniformBufferId<'T> = private UniformBufferId of ItemId with

    member this.ItemId = match this with UniformBufferId itemId -> itemId

[<Struct>]
type VertexBufferId<'T> = private VertexBufferId of ItemId with

    member this.ItemId = match this with VertexBufferId itemId -> itemId

[<Struct>]
type TextureId = private TextureId of ItemId with

    member this.ItemId = match this with TextureId itemId -> itemId

[<Struct>]
type ShaderId = private ShaderId of ItemId with

    member this.ItemId = match this with ShaderId itemId -> itemId

[<Struct>]
type DrawCallId = private DrawCallId of ItemId with

    member this.ItemId = match this with DrawCallId itemId -> itemId
