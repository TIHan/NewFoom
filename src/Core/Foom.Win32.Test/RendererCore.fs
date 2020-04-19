module FsGame.Renderer.Vulkan

open Foom.Game
open Foom.Win32
open FsGame.Graphics.Vulkan
open FSharp.Vulkan.Interop
open FSharp.Window
open FSharp.Spirv
open FSharp.Spirv.Specification
open FSharp.Spirv.Quotations
open System
open System.Numerics
open System.Drawing
open System.Drawing.Imaging
open FSharp.NativeInterop
open Foom.Wad
open FsGame.Graphics.FreeType
open System.Collections.Concurrent
open FsGame.Core.Collections

#nowarn "9"
#nowarn "51"

type IVulkanWindowEvents =

    abstract OnWindowClosing: FalGraphics -> unit

    abstract OnInputEvents: FalGraphics * InputEvent list -> unit

    abstract OnUpdateFrame: FalGraphics * time: float * interval: float -> bool

    abstract OnRenderFrame: FalGraphics * time: float * delta: float * width: int * height: int -> unit

let createVulkanWin32Window title engineName updateInterval width height (windowEvents: IVulkanWindowEvents) =
    let windowState = Win32WindowState (title, width, height)
    windowState.Show () // Must show to actually instantiate the state. Maybe we need a better API.
    let hwnd = windowState.Hwnd
    let hinstance = windowState.Hinstance

    let device = VulkanDevice.CreateWin32(hwnd, hinstance, title, engineName, [VulkanDeviceLayer.LunarGStandardValidation], [])
    let subpasses =
        [RenderSubpass ColorDepthStencilSubpass]

    let instance = FalGraphics.Create (device, windowState.WindowResized, subpasses)
    instance.SetupCommands() // We need to get rid of this thing.

    let windowEvents2 = 
        let gate = obj ()
        let mutable quit = false

        { new IWindowEvents with

            member __.OnWindowClosing () =
                lock gate (fun () ->
                    quit <- true
                    instance.WaitIdle()
                    windowEvents.OnWindowClosing instance
                )

            member __.OnInputEvents events = 
                windowEvents.OnInputEvents(instance, events)

            member __.OnUpdateFrame (time, interval) =
                quit <- windowEvents.OnUpdateFrame(instance, time, interval) || quit
                quit

            member __.OnRenderFrame (time, delta, width, height) =
                lock gate (fun () ->
                    if not quit then
                        windowEvents.OnRenderFrame(instance, time, delta, width, height)
                ) }

    let window = Window (title, updateInterval, width, height, windowEvents2, windowState)
    window, instance

//

[<Struct;Block>]
type Vertex =
    {
        position: Vector3
        uv: Vector2
    }

[<Struct;Block>]
type ModelViewProjection =
    {
        model: Matrix4x4
        view: Matrix4x4
        proj: Matrix4x4
    }
    
    member this.InvertedView =
        let mutable invertedView = Matrix4x4.Identity
        Matrix4x4.Invert(this.view, &invertedView) |> ignore
        { model = this.model; view = invertedView; proj = this.proj }

let radians (degrees) = degrees * (MathF.PI / 180.f)

let normalize (x: single) (min: single) (max: single) =
    (x - min) / (max - min)

let createBitmap (pixels: Pixel [,]) =
    let mutable isTransparent = false

    let width = Array2D.length1 pixels
    let height = Array2D.length2 pixels

    pixels
    |> Array2D.iter (fun p ->
        if p.Equals Pixel.Cyan then
            isTransparent <- true
    )

    let bitmap = new Bitmap (width, height)
    for i = 0 to width - 1 do
        for j = 0 to height - 1 do
            let pixel = pixels.[i, j]
            if pixel = Pixel.Cyan then
                bitmap.SetPixel (i, j, Color.FromArgb (0, 0, 0, 0))
            else
                bitmap.SetPixel (i, j, Color.FromArgb (int pixel.R, int pixel.G, int pixel.B))

    bitmap

type Sampler2d = SampledImage<single, DimKind.Two, ImageDepthKind.NoDepth, ImageArrayedKind.NonArrayed, ImageMultisampleKind.Single, ImageSampleKind.Sampler, ImageFormatKind.Unknown, AccessQualifierKind.None>

[<Struct>]
type WallRenderInfo =
    {
        FrontSideFloorHeight: single
        FrontSideCeilingHeight: single
        BackSideFloorHeight: single
        BackSideCeilingHeight: single
        LightLevel: single
    }

[<Struct>]
type TextureInfo =
    {
        Width: single
        Height: single
    }

let textureCache = Collections.Generic.Dictionary<string, FalkanImage * TextureInfo * VulkanBuffer<TextureInfo>>()
let getImage (graphics: FalGraphics) (wad: Wad) name =
    match textureCache.TryGetValue name with
    | true, res -> res
    | _ ->
        let texture = 
            match wad.TryFindFlatTexture name with
            | None ->
                (wad.TryFindTexture name).Value
            | Some texture -> texture
   
        let data = texture.Data
        use bmp = createBitmap data
        let res = graphics.CreateImage bmp, { Width = single bmp.Width; Height = single bmp.Height }, (graphics.CreateBuffer(UniformBuffer, VulkanBufferFlags.None, [|{ Width = single bmp.Width; Height = single bmp.Height }|]))
        textureCache.[name] <- res
        res

[<Struct;NoComparison;NoEquality>]
type VulkanVar<'T when 'T : unmanaged> = VulkanVar of VulkanBuffer<'T> with

    member this.Set(value: 'T) =
        match this with
        | VulkanVar buffer -> buffer.SetData(ReadOnlySpan [|value|])

    member this.Set(offset, value: 'T) =
        match this with
        | VulkanVar buffer -> buffer.SetData(offset, ReadOnlySpan [|value|])

    member this.Buffer =
        match this with
        | VulkanVar buffer -> buffer

[<Struct;NoComparison;NoEquality>]
type VulkanVarList<'T when 'T : unmanaged> = VulkanVarList of VulkanBuffer<'T> with

    member this.Set(value: ReadOnlySpan<'T>) =
        match this with
        | VulkanVarList buffer -> buffer.SetData(value)

    member this.Set(offset, value: ReadOnlySpan<'T>) =
        match this with
        | VulkanVarList buffer -> buffer.SetData(offset, value)

    member this.Set(offset, value: 'T) =
        this.Set(offset, ReadOnlySpan [|value|])

    member this.Buffer =
        match this with
        | VulkanVarList buffer -> buffer

[<Struct;NoComparison;NoEquality>]
type VulkanVarListSegment<'T when 'T : unmanaged> = VulkanVarListSegment of VulkanVarList<'T> * offset: int * count: int with

    member this.Set(value: ReadOnlySpan<'T>) =
        match this with
        | VulkanVarListSegment(var, offset, count) -> 
            if value.Length > count then
                invalidArg "value" "Value length is greater than the segment's length."
            var.Set(offset, value)

    member this.Set(offset, value: ReadOnlySpan<'T>) =
        match this with
        | VulkanVarListSegment(var, offset2, count) -> 
            if value.Length > (count - offset) then
                invalidArg "value" "Value length is greater than the remaining segment's length."
            var.Set(offset + offset2, value)

    member this.Set(offset, value: 'T) =
        this.Set(offset, ReadOnlySpan [|value|])

    member this.Buffer =
        match this with
        | VulkanVarListSegment(var, _, _) -> var.Buffer

[<Struct;NoComparison;NoEquality>]
type Heights =
    {
        FloorHeight: single
        CeilingHeight: single
    }

[<Struct;NoComparison;NoEquality>]
type SidePartRender =
    {
        PositionXY: VulkanVarListSegment<Vector2>
        PositionZ: VulkanVarListSegment<single>
        UV: VulkanVarListSegment<single>
        Image: FalkanImage
    }

[<Struct;NoComparison;NoEquality>]
type SideView =
    {
        SectorId: int

        HasUpper: bool
        HasMiddle: bool
        Upper: SidePartRender
        Middle: SidePartRender
    }

[<Struct;NoComparison;NoEquality>]
type LineView =
    {
        HasFrontSide: bool
        HasBackSide: bool
        FrontSide: SideView
        BackSide: SideView
    }

[<Struct;NoComparison;NoEquality>]
type SectorView =
    {
        OriginalHeights: Heights
        Heights: Heights
        LineViewIds: ResizeArray<int>
    }

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

let tryInitializeUpperFront map linedef =
    match (map, linedef) with
    | FrontBack(Some frontSideSector, Some backSideSector) ->
        let bottom = float32 backSideSector.CeilingHeight
        let top = float32 frontSideSector.CeilingHeight
        [|
            bottom
            bottom
            top

            top
            top
            bottom        
        |]
        |> Some
    | _ ->
        None

let tryInitializeMiddleFront map linedef =
    match (map, linedef) with
    | FrontBack(frontSideSectorOpt, backSideSectorOpt) ->
        match frontSideSectorOpt, backSideSectorOpt with

        | Some frontSideSector, Some backSideSector ->
            let floorHeight =
                if backSideSector.FloorHeight > frontSideSector.FloorHeight then
                    backSideSector.FloorHeight
                else
                    frontSideSector.FloorHeight
                |> float32

            let ceilingHeight =
                if backSideSector.CeilingHeight < frontSideSector.CeilingHeight then
                    backSideSector.CeilingHeight
                else
                    frontSideSector.CeilingHeight
                |> float32

            [|
                floorHeight
                floorHeight
                ceilingHeight

                ceilingHeight
                ceilingHeight
                floorHeight        
            |]
            |> Some

        | Some frontSideSector, _ ->
            let floorHeight = float32 frontSideSector.FloorHeight
            let ceilingHeight = float32 frontSideSector.CeilingHeight

            [|
                floorHeight
                floorHeight
                ceilingHeight

                ceilingHeight
                ceilingHeight
                floorHeight        
            |]
            |> Some

        | _ ->
            None

let updateLineView (sectorViews: Span<SectorView>) (lineView: inref<LineView>) =

    if lineView.HasFrontSide then
        let origFrontSide = sectorViews.[lineView.FrontSide.SectorId].OriginalHeights
        let frontSide = sectorViews.[lineView.FrontSide.SectorId].Heights

        if lineView.FrontSide.HasUpper && lineView.HasBackSide then
            let origBackSide = sectorViews.[lineView.BackSide.SectorId].OriginalHeights
            let backSide = sectorViews.[lineView.BackSide.SectorId].Heights

            let origBottom = origBackSide.CeilingHeight
            let origTop = origFrontSide.CeilingHeight

            let bottom = backSide.CeilingHeight
            let top = frontSide.CeilingHeight

            let height = top - bottom
            let origHeight = origTop - origBottom

            let bottomScale =
                if origHeight = 0.f then
                    height
                    //1.f
                else
                    (height / origHeight)

            let topScale = 1.f

            let positionZ =
                [|
                    bottom
                    bottom
                    top

                    top
                    top
                    bottom   
                |]

            let uv =
                [|
                    topScale
                    topScale
                    bottomScale

                    bottomScale
                    bottomScale
                    topScale
                
                |]

            lineView.FrontSide.Upper.PositionZ.Set(ReadOnlySpan positionZ)
            lineView.FrontSide.Upper.UV.Set(ReadOnlySpan uv)

        if lineView.FrontSide.HasMiddle then
            let floorHeight =
                if lineView.HasBackSide then
                    let backSide = sectorViews.[lineView.BackSide.SectorId].Heights
                    if backSide.FloorHeight > frontSide.FloorHeight then
                        backSide.FloorHeight
                    else
                        frontSide.FloorHeight
                else
                    frontSide.FloorHeight

            let ceilingHeight =
                if lineView.HasBackSide then
                    let backSide = sectorViews.[lineView.BackSide.SectorId].Heights
                    if backSide.CeilingHeight < frontSide.CeilingHeight then
                        backSide.CeilingHeight
                    else
                        frontSide.CeilingHeight
                else
                    frontSide.CeilingHeight
         //   ()
            let positionZ =
                [|
                    floorHeight
                    floorHeight
                    ceilingHeight

                    ceilingHeight
                    ceilingHeight
                    floorHeight        
                |]

            lineView.FrontSide.Middle.PositionZ.Set(ReadOnlySpan positionZ)

let updateLineViews (sectorViews: Span<SectorView>) (lineViews: Span<LineView>) (lineViewIds: ResizeArray<int>) =
    for i = 0 to lineViewIds.Count - 1 do
        updateLineView sectorViews &lineViews.[lineViewIds.[i]]

[<Sealed>]
type MapView(sectors: SectorView [], lineViews: LineView [], sectorRendersBuffer: VulkanBuffer<SectorRender>) =

    member this.GetSectorHeights(sectorId: int) =
        sectors.[sectorId].Heights
    
    member this.SetSectorHeights(sectorId: int, heights: Heights) =
        let sectorView = { sectors.[sectorId]with Heights = heights }
        sectors.[sectorId] <- sectorView
      //  updateLineViews (Span sectors) (Span lineViews) sectorView.LineViewIds
        sectorRendersBuffer.SetData(sectorId, ReadOnlySpan [|{ OriginalCeilingHeight = sectorView.OriginalHeights.CeilingHeight; OriginalFloorHeight = sectorView.OriginalHeights.FloorHeight; FloorHeight = heights.FloorHeight; CeilingHeight = heights.CeilingHeight  }|])

let createVar (graphics: FalGraphics) (data: 'T[]) : VulkanVarListSegment<'T> =
    let buffer = graphics.CreateBuffer(VertexBuffer, VulkanBufferFlags.None, data)
    let var = VulkanVarList buffer
    VulkanVarListSegment(var, 0, data.Length)

let createFrontSidePositionXY (ldef: Linedef) =
    let a = ldef.Segment.A
    let b = ldef.Segment.B
    [|
        a; b; b
        b; a; a
    |]

let mutable mapViewShader = None
let load (graphics: FalGraphics) (wad: Wad) (map: Map) (mvpBuffer: VulkanBuffer<ModelViewProjection>) =
    let shader =
        match mapViewShader with
        | Some shader -> shader
        | _ ->
            let vertex =
                <@
                    let mvp = Variable<ModelViewProjection> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.Uniform []
                    let sectors = Variable<SectorRendersBlock> [Decoration.Binding 2u; Decoration.DescriptorSet 2u] StorageClass.StorageBuffer []

                    let position = Variable<Vector2> [Decoration.Location 0u; Decoration.Binding 0u] StorageClass.Input []
                    let z = Variable<single> [Decoration.Location 1u; Decoration.Binding 1u] StorageClass.Input []
                    let uv = Variable<single> [Decoration.Location 2u; Decoration.Binding 2u] StorageClass.Input []
                    let sectorId = Variable<int> [Decoration.Location 3u; Decoration.Binding 3u] StorageClass.Input []
                    let origUv = Variable<Vector2> [Decoration.Location 4u; Decoration.Binding 4u] StorageClass.Input []
    
                    let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output []
                    let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output []
    
                    fun () ->
                        let beef = sectors.SectorRenders.[sectorId]
                        let z = z + beef.OriginalCeilingHeight
                        //let z = beef.FloorHeight // ((beef.CeilingHeight - beef.FloorHeight) / (beef.OriginalCeilingHeight - beef.OriginalFloorHeight))
                        let ycoord = origUv.Y * uv
                        gl_Position <- Vector4.Transform(Vector4(position, z, 1.f), mvp.model * mvp.view * mvp.proj)
                        fragTexCoord <- Vector2(origUv.X, ycoord)
                @>
    
            let fragment =
                <@ 
                    let sampler = Variable<Sampler2d> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.UniformConstant []
                    let fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input []
    
                    let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output []
    
                    fun () ->
                        let color = sampler.ImplicitLod fragTexCoord
                        if color.W < 0.5f then
                            kill ()
                        outColor <- color //Vector4.Multiply(color, sectorRenderInfo.LightLevel)
                        outColor <- Vector4(outColor.X, outColor.Y, outColor.Z, color.W)
                @>
    
            let shader = graphics.CreateShader(vertex, fragment)
    
            mapViewShader <- Some shader
            shader

    let sectorViews = Array.zeroCreate map.Sectors.Length
    let sectorRenders = Array.zeroCreate map.Sectors.Length
    for i = 0 to map.Sectors.Length - 1 do
        let sector = map.Sectors.[i]
        let heights = { FloorHeight = single sector.FloorHeight; CeilingHeight = single sector.CeilingHeight }
        sectorViews.[i] <- 
            { 
                OriginalHeights = heights
                Heights = heights
                LineViewIds = ResizeArray()
            }
        sectorRenders.[i] <-
            { 
                OriginalFloorHeight = heights.FloorHeight
                OriginalCeilingHeight = heights.CeilingHeight
                FloorHeight = heights.FloorHeight
                CeilingHeight = heights.CeilingHeight     
            }
        
    let sectorRendersBuffer = graphics.CreateBuffer(StorageBuffer, VulkanBufferFlags.None, sectorRenders)

    let addLineViewId (sectorViewId: int) (lineViewId: int) =
        sectorViews.[sectorViewId].LineViewIds.Add lineViewId

    let lineViews =
        map.Linedefs
        |> Seq.mapi (fun lineViewId ldef ->
    
            let positionXY = createFrontSidePositionXY ldef
            let positionXYVar = createVar graphics (createFrontSidePositionXY ldef)
    
            let frontSide =
                ldef.FrontSidedefIndex
                |> Option.map (fun i -> 
                    let sdef = map.Sidedefs.[i]

                    addLineViewId sdef.SectorNumber lineViewId

                    let upper =
                        match sdef.UpperTextureName with
                        | Some texName ->
                            let image, info, infoBuffer = getImage graphics wad texName

                            let positionZ = (tryInitializeUpperFront map ldef).Value
                            let origZ = Array.init positionZ.Length (fun _ -> sdef.SectorNumber)
                            let uvVertices: Vector2 [] = createWallUv2 map ldef sdef (int info.Width) (int info.Height) positionXY positionZ WallSection.Upper
                            let origUv = createVar graphics uvVertices

                            let partRender =
                                {
                                    PositionXY = positionXYVar
                                    PositionZ = createVar graphics positionZ
                                    UV = createVar graphics (Array.init positionZ.Length (fun _ -> 1.f))
                                    Image = image
                                }

                            let draw = shader.CreateDrawBuilder()
                            let draw = draw.AddDescriptorBuffer(mvpBuffer).AddDescriptorImage(image).AddDescriptorBuffer(sectorRendersBuffer).Next
                            let draw = draw.AddVertexBuffer(partRender.PositionXY.Buffer, PerVertex).AddVertexBuffer(partRender.PositionZ.Buffer, PerVertex).AddVertexBuffer(partRender.UV.Buffer, PerVertex).AddVertexBuffer(graphics.CreateBuffer(VertexBuffer, VulkanBufferFlags.None, origZ), PerVertex).AddVertexBuffer(origUv.Buffer, PerVertex)
                            let vertexCount = positionXY.Length                       
                            shader.AddDraw(draw, uint32 vertexCount, 1u) |> ignore

                            Some partRender
                            //let textureAlignment = getTextureAlignment map ldef ldef.FrontSidedefIndex.IsSome WallSection.Middle
                    
                        
                        | _ ->
                            None

                    let middle =
                        match sdef.MiddleTextureName with
                        | Some texName ->
                            let image, info, infoBuffer = getImage graphics wad texName

                            let positionZ = (tryInitializeMiddleFront map ldef).Value
                            let origZ = Array.init positionZ.Length (fun _ -> sdef.SectorNumber)
                            let uvVertices: Vector2 [] = createWallUv2 map ldef sdef (int info.Width) (int info.Height) positionXY positionZ WallSection.Middle
                            let origUv = createVar graphics uvVertices

                            let partRender =
                                {
                                    PositionXY = positionXYVar
                                    PositionZ = createVar graphics positionZ
                                    UV = createVar graphics (Array.init positionZ.Length (fun _ -> 1.f))
                                    Image = image
                                }

                            let draw = shader.CreateDrawBuilder()
                            let draw = draw.AddDescriptorBuffer(mvpBuffer).AddDescriptorImage(image).AddDescriptorBuffer(sectorRendersBuffer).Next
                            let draw = draw.AddVertexBuffer(partRender.PositionXY.Buffer, PerVertex).AddVertexBuffer(partRender.PositionZ.Buffer, PerVertex).AddVertexBuffer(partRender.UV.Buffer, PerVertex).AddVertexBuffer(graphics.CreateBuffer(VertexBuffer, VulkanBufferFlags.None, origZ), PerVertex).AddVertexBuffer(origUv.Buffer, PerVertex)
                            let vertexCount = positionXY.Length                       
                            shader.AddDraw(draw, uint32 vertexCount, 1u) |> ignore

                            Some partRender
                            //let textureAlignment = getTextureAlignment map ldef ldef.FrontSidedefIndex.IsSome WallSection.Middle
                    
                        
                        | _ ->
                            None

                    let hasUpper = upper.IsSome
                    let upper = match upper with Some v -> v | _ -> Unchecked.defaultof<_>
                    let hasMiddle = middle.IsSome
                    let middle = match middle with Some v -> v | _ -> Unchecked.defaultof<_>
                            
                    { SectorId = sdef.SectorNumber; HasUpper = hasUpper; Upper = upper; HasMiddle = hasMiddle; Middle = middle })
                    
            let backSide =
                ldef.BackSidedefIndex
                |> Option.map (fun i -> 
                    let sdef = map.Sidedefs.[i]

                    addLineViewId sdef.SectorNumber lineViewId
                    
                    { SectorId = sdef.SectorNumber; HasUpper = false; Upper = Unchecked.defaultof<_>; HasMiddle = false; Middle = Unchecked.defaultof<_> })
    
            let hasFrontSide = frontSide.IsSome
            let hasBackSide = backSide.IsSome
            let frontSide = match frontSide with Some v -> v | _ -> Unchecked.defaultof<_>
            let backSide = match backSide with Some v -> v | _ -> Unchecked.defaultof<_>
    
            let lineView = { HasFrontSide = hasFrontSide; FrontSide = frontSide; HasBackSide = hasBackSide; BackSide = backSide }
            
          //  updateLineView (Span sectorViews) &lineView

            lineView)
        |> Array.ofSeq

    MapView(sectorViews, lineViews, sectorRendersBuffer)

let loadMap mapName (wad: Wad) (graphics: FalGraphics) =
    let e1m1 = wad.FindMap mapName
    let start = e1m1.TryFindPlayer1Start().Value
    let start = Vector3(float32 start.X, float32 start.Y, 28.f)

    let mutable quat =  Matrix4x4.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))
    quat.Translation <- Vector3(start.X, start.Y, 28.f)
    let mvp =
        {
            model = Matrix4x4.Identity
            view = quat
            proj = Matrix4x4.CreatePerspectiveFieldOfView(radians 45.f, 1280.f / 720.f, 0.1f, 1000000.f)
        }

    let mvpUniform = graphics.CreateBuffer<ModelViewProjection>(UniformBuffer, VulkanBufferFlags.None, [|mvp.InvertedView|])

    let vertex =
        <@
            let mvp = Variable<ModelViewProjection> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.Uniform []
            let vertex = Variable<Vertex> [Decoration.Location 0u; Decoration.Binding 0u] StorageClass.Input []
            let lightLevel = Variable<single> [Decoration.Location 2u; Decoration.Binding 1u] StorageClass.Input [PerInstance]
            let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output []
            let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output []
            let mutable lightLevelOut = Variable<single> [Decoration.Location 1u] StorageClass.Output []

            fun () ->
                gl_Position <- Vector4.Transform(Vector4(vertex.position, 1.f), mvp.model * mvp.view * mvp.proj)
                fragTexCoord <- vertex.uv
                lightLevelOut <- lightLevel
        @>

    let fragment =
        <@ 
            let sampler = Variable<Sampler2d> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.UniformConstant []
            let fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input []
            let lightLevel = Variable<single> [Decoration.Location 1u] StorageClass.Input []
            let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output []

            fun () ->
                let color = sampler.ImplicitLod fragTexCoord
                if color.W < 0.5f then
                    kill ()
                outColor <- Vector4.Multiply(color, lightLevel)
                outColor <- Vector4(outColor.X, outColor.Y, outColor.Z, color.W)
        @>

    let shader = graphics.CreateShader(vertex, fragment)

    let queueDraw (image: FalkanImage) (lightLevel: int) (vertices: Vector3 []) (uv: Vector2 []) =
        let vertices = Array.init vertices.Length (fun i -> { position = vertices.[i]; uv = uv.[i] })
        let vertexBuffer = graphics.CreateBuffer(VertexBuffer, VulkanBufferFlags.None, vertices) 
        let doot = Array.init 1 (fun _ -> normalize (single lightLevel) 0.f 255.f)
        let dootBuffer = graphics.CreateBuffer(VertexBuffer, VulkanBufferFlags.None, doot)
        
        let draw = shader.CreateDrawBuilder()
        let draw = draw.AddDescriptorBuffer(mvpUniform).AddDescriptorImage(image).Next
        let draw = draw.AddVertexBuffer(vertexBuffer, PerVertex).AddVertexBuffer(dootBuffer, PerInstance)
           
        shader.AddDraw(draw, uint32 vertices.Length, 1u) |> ignore

    (e1m1.Sectors, e1m1.ComputeAllSectorGeometry())
    ||> Seq.iteri2 (fun i s geo ->
        let image, info, _ = getImage graphics wad s.FloorTextureName
        let width = int info.Width
        let height = int info.Height
        let uv = Map.CreateSectorUv(width, height, geo.FloorVertices)
        queueDraw image s.LightLevel geo.FloorVertices uv

        let image, info, _ = getImage graphics wad s.CeilingTextureName
        let width = int info.Width
        let height = int info.Height
        let uv = Map.CreateSectorUv(width,height, geo.CeilingVertices)
        queueDraw image s.LightLevel geo.CeilingVertices uv
        )


    let queueLinedef geo (ldef: Linedef) sdefIndex =
        //match geo.Upper, sdefIndex with
        //| Some vertices, Some i ->
        //    let sdef = e1m1.Sidedefs.[i]
        //    match sdef.UpperTextureName with
        //    | Some upperTex ->
        //        let image, info, _ = getImage graphics wad upperTex
        //        let width = int info.Width
        //        let height = int info.Height
        //        let uv = e1m1.CreateUpperWallUv(ldef, sdef, width, height, vertices)
        //        queueDraw image (e1m1.Sectors.[sdef.SectorNumber].LightLevel) vertices uv
        //    | _ -> ()
        //| _ -> ()
        
        //match geo.Middle, sdefIndex with
        //| Some vertices, Some i ->
        //    let sdef = e1m1.Sidedefs.[i]
        //    match sdef.MiddleTextureName with
        //    | Some upperTex ->
        //        let image, width, height = getImage graphics wad upperTex
        //        let uv = e1m1.CreateMiddleWallUv(ldef, sdef, width, height, vertices)
        //        queueDraw image (e1m1.Sectors.[sdef.SectorNumber].LightLevel) vertices uv
        //    | _ -> ()
        //| _ -> ()
        

        match geo.Lower, sdefIndex with
        | Some vertices, Some i ->
            let sdef = e1m1.Sidedefs.[i]
            match sdef.LowerTextureName with
            | Some upperTex ->
                let image, info, _ = getImage graphics wad upperTex
                let width = int info.Width
                let height = int info.Height
                let uv = e1m1.CreateLowerWallUv(ldef, sdef, width, height, vertices)
                queueDraw image (e1m1.Sectors.[sdef.SectorNumber].LightLevel) vertices uv
            | _ -> ()
        | _ -> ()

    e1m1.Linedefs 
    |> Seq.iter (fun ldef ->
        let geo = e1m1.ComputeFrontWallGeometry ldef
        queueLinedef geo ldef ldef.FrontSidedefIndex        
        )

    let mapView = load graphics wad e1m1 mvpUniform

    e1m1.Linedefs
    |> Seq.iter (fun ldef ->
        if ldef.BackSidedefIndex.IsSome && ldef.SpecialType = 1 then
            let sdef = e1m1.Sidedefs.[ldef.BackSidedefIndex.Value]
            let heights = mapView.GetSectorHeights sdef.SectorNumber
            mapView.SetSectorHeights(sdef.SectorNumber, { heights with CeilingHeight = heights.CeilingHeight + 20.f })
    )

    mvpUniform, mvp

let mkSimpleShader desc vertex fragment (instance: FalGraphics) =
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

    instance.CreateShader(desc, ReadOnlySpan vertexBytes, ReadOnlySpan fragmentBytes)