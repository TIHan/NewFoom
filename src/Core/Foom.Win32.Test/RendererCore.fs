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

[<Struct>]
type Vertex =
    {
        position: Vector3
        uv: Vector2
    }

[<Struct>]
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

let textureCache = Collections.Generic.Dictionary<string, FalkanImage * int * int>()
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
        let res = graphics.CreateImage bmp, bmp.Width, bmp.Height
        textureCache.[name] <- res
        res

let (|FrontBack|) (map: Map, linedef: Linedef) =
    let frontSideSectorOpt =
        linedef.FrontSidedefIndex
        |> Option.map(fun x -> map.Sidedefs.[x])
        |> Option.map(fun x -> map.Sectors.[x.SectorNumber], x)

    let backSideSectorOpt =
        linedef.BackSidedefIndex
        |> Option.map(fun x -> map.Sidedefs.[x])
        |> Option.map(fun x -> map.Sectors.[x.SectorNumber], x)

    (frontSideSectorOpt, backSideSectorOpt)

[<Struct;NoComparison;NoEquality>]
type VulkanVar<'T when 'T : unmanaged> = VulkanVar of FalkanBuffer with

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
type VulkanVarList<'T when 'T : unmanaged> = VulkanVarList of FalkanBuffer with

    member this.Set value =
        match this with
        | VulkanVarList buffer -> buffer.SetData(value)

    member this.Set(offset, value) =
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
type SideView =
    {
        SectorId: int
        PositionZ: VulkanVarListSegment<single>
        MiddleTexture: FalkanImage voption
    }

[<Struct;NoComparison;NoEquality>]
type LineView =
    {
        PositionXY: VulkanVarListSegment<Vector2>
        FrontSide: SideView voption
        BackSide: SideView voption
    }

[<Struct;NoComparison;NoEquality>]
type Heights =
    {
        FloorHeight: single
        CeilingHeight: single
    }

[<Struct;NoComparison;NoEquality>]
type SectorView =
    {
        Heights: Heights
        LineViews: ReadOnlyMemory<LineView>
    }

let middleFrontSideHeights (frontSide: Heights) (backSideOpt: Heights voption) =
    match backSideOpt with
    | ValueSome backSide ->
        let floorHeight =
            if backSide.FloorHeight > frontSide.FloorHeight then
                backSide.FloorHeight
            else
                frontSide.FloorHeight

        let ceilingHeight =
            if backSide.CeilingHeight < frontSide.CeilingHeight then
                backSide.CeilingHeight
            else
                frontSide.CeilingHeight

        { FloorHeight = floorHeight; CeilingHeight = ceilingHeight }

    | _ -> 
        { FloorHeight = frontSide.FloorHeight; CeilingHeight = frontSide.CeilingHeight }

[<Sealed>]
type MapView(sectors: SectorView [], lineViews: LineView []) =

    member this.Sectors = ReadOnlySpan sectors

    member this.Lines = ReadOnlySpan lineViews

    member this.UpdateLines(lineViews: ReadOnlySpan<LineView>) =
        for i = 0 to lineViews.Length - 1 do
            let lineView = lineViews.[i]

            let frontSideOpt = 
                match lineView.FrontSide with
                | ValueSome frontSide ->
                    ValueSome sectors.[frontSide.SectorId].Heights
                | _ ->
                    ValueNone
                
            let backSideOpt = 
                match lineView.BackSide with
                | ValueSome backSide ->
                    ValueSome sectors.[backSide.SectorId].Heights
                | _ ->
                    ValueNone
            
            match frontSideOpt with
            | ValueSome frontSide ->
                let heights = middleFrontSideHeights frontSide backSideOpt

                let floorHeight = heights.FloorHeight
                let ceilingHeight = heights.CeilingHeight

                let positionZ =
                    [|
                        floorHeight
                        floorHeight
                        ceilingHeight

                        ceilingHeight
                        ceilingHeight
                        floorHeight        
                    |]

                match lineView.FrontSide with
                | ValueSome frontSide ->
                    frontSide.PositionZ.Set(ReadOnlySpan positionZ)
                | _ ->
                    ()
            | _ ->
                ()

    member this.GetSectorHeights(sectorId: int) =
        sectors.[sectorId].Heights
    
    member this.SetSectorHeights(sectorId: int, heights: Heights) =
        let data = { sectors.[sectorId] with Heights = heights }
        sectors.[sectorId] <- data
        this.UpdateLines(data.LineViews.Span)

let createVar (graphics: FalGraphics) (data: 'T[]) =
    let buffer = graphics.CreateBuffer(data.Length, FalkanBufferFlags.None, VertexBuffer, ReadOnlySpan(data))
    let var = VulkanVarList buffer
    VulkanVarListSegment(var, 0, data.Length)

let createFrontSidePositionXY (ldef: Linedef) =
    let a = ldef.Segment.A
    let b = ldef.Segment.B
    [|
        a; b; b
        b; a; a
    |]

let loadMapView (graphics: FalGraphics) (wad: Wad) (map: Map) =
    let lineViews =
        map.Linedefs
        |> Seq.map (fun ldef ->

            let positionXY = createVar graphics (createFrontSidePositionXY ldef)

            let frontSide =
                ldef.FrontSidedefIndex
                |> Option.map (fun i -> 
                    let sdef = map.Sidedefs.[i]
                    let middleTexture =
                        match sdef.MiddleTextureName with
                        | Some texName ->
                            let image, _, _ = getImage graphics wad texName
                            ValueSome image
                        | _ ->
                            ValueNone
                    { SectorId = sdef.SectorNumber; PositionZ = createVar graphics (Array.zeroCreate<single> 6); MiddleTexture = middleTexture })
                
            let backSide =
                ldef.FrontSidedefIndex
                |> Option.map (fun i -> 
                    let sdef = map.Sidedefs.[i]
                    let middleTexture =
                        match sdef.MiddleTextureName with
                        | Some texName ->
                            let image, _, _ = getImage graphics wad texName
                            ValueSome image
                        | _ ->
                            ValueNone
                    { SectorId = sdef.SectorNumber; PositionZ = createVar graphics (Array.zeroCreate<single> 6); MiddleTexture = middleTexture })

            let frontSide = match frontSide with Some v -> ValueSome v | _ -> ValueNone
            let backSide = match backSide with Some v -> ValueSome v | _ -> ValueNone

            { PositionXY = positionXY; FrontSide = frontSide; BackSide = backSide })
        |> Array.ofSeq

    let sectorViews =
        map.Sectors
        |> Seq.mapi (fun i sector ->
            {
                Heights = { FloorHeight = single sector.FloorHeight; CeilingHeight = single sector.CeilingHeight }
                LineViews = 
                    lineViews
                    |> Array.filter (fun x -> 
                        x.FrontSide.IsSome && x.FrontSide.Value.SectorId = i ||
                        x.BackSide.IsSome && x.BackSide.Value.SectorId = i)
                    |> ReadOnlyMemory
            }
        )
        |> Array.ofSeq

    MapView(sectorViews, lineViews)

let mutable mapViewShader = None
let renderMapView (graphics: FalGraphics) (mapView: MapView) (mvpBuffer: FalkanBuffer) =
    let shader =
        match mapViewShader with
        | Some shader -> shader
        | _ ->
            let vertex =
                <@
                    let mvp = Variable<ModelViewProjection> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.Uniform

                    let position = Variable<Vector2> [Decoration.Location 0u; Decoration.Binding 0u] StorageClass.Input
                    let z = Variable<single> [Decoration.Location 1u; Decoration.Binding 1u] StorageClass.Input

                    let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
                    let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output

                    fun () ->
                        gl_Position <- Vector4.Transform(Vector4(position, z, 1.f), mvp.model * mvp.view * mvp.proj)
                        fragTexCoord <- Vector2(0.f, 0.f)
                @>

            let fragment =
                <@ 
                    let sampler = Variable<Sampler2d> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.UniformConstant

                    let fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input

                    let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output

                    fun () ->
                        let color = sampler.ImplicitLod fragTexCoord
                        if color.W < 0.5f then
                            kill ()
                        outColor <- color //Vector4.Multiply(color, sectorRenderInfo.LightLevel)
                        outColor <- Vector4(outColor.X, outColor.Y, outColor.Z, color.W)
                @>

            let shader = 
                graphics.CreateShader(vertex, fragment,
                    Shader(0, true, 
                        [ ShaderDescriptorLayout(UniformBufferDescriptor, VertexStage, 0u)
                          ShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 1u) ],
                        [ ShaderVertexInput(PerVertex, typeof<Vector2>, 0u)
                          ShaderVertexInput(PerVertex, typeof<single>, 1u) ]))

            mapViewShader <- Some shader
            shader

    for lineView in mapView.Lines do
        match lineView.FrontSide with
        | ValueSome frontSide ->
            match frontSide.MiddleTexture with
            | ValueSome image ->
                let draw = shader.CreateDrawBuilder()
                let draw = draw.AddDescriptorBuffer(mvpBuffer, sizeof<ModelViewProjection>).AddDescriptorImage(image).Next
                let draw = draw.AddVertexBuffer(lineView.PositionXY.Buffer, PerVertex).AddVertexBuffer(frontSide.PositionZ.Buffer, PerVertex)
                   
                let vertexCount = lineView.PositionXY.Buffer.Size / sizeof<Vector2>

                shader.AddDraw(draw, uint32 vertexCount, 1u) |> ignore
            | _ ->
                ()
        | _ ->
            ()

    for i = 0 to mapView.Sectors.Length - 1 do
        mapView.SetSectorHeights(i, mapView.GetSectorHeights i)

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

    let mvpUniform = graphics.CreateBuffer<ModelViewProjection>(1, FalkanBufferFlags.None, UniformBuffer, ReadOnlySpan [|mvp.InvertedView|])

    let vertex =
        <@
            let mvp = Variable<ModelViewProjection> [Decoration.Binding 0u; Decoration.DescriptorSet 0u] StorageClass.Uniform
            let position = Variable<Vector3> [Decoration.Location 0u; Decoration.Binding 0u] StorageClass.Input
            let texCoord = Variable<Vector2> [Decoration.Location 1u; Decoration.Binding 0u] StorageClass.Input
            let lightLevel = Variable<single> [Decoration.Location 2u; Decoration.Binding 1u] StorageClass.Input
            let mutable gl_Position  = Variable<Vector4> [Decoration.BuiltIn BuiltIn.Position] StorageClass.Output
            let mutable fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Output
            let mutable lightLevelOut = Variable<single> [Decoration.Location 1u] StorageClass.Output

            fun () ->
                gl_Position <- Vector4.Transform(Vector4(position, 1.f), mvp.model * mvp.view * mvp.proj)
                fragTexCoord <- texCoord
                lightLevelOut <- lightLevel
        @>

    let fragment =
        <@ 
            let sampler = Variable<Sampler2d> [Decoration.Binding 1u; Decoration.DescriptorSet 1u] StorageClass.UniformConstant
            let fragTexCoord = Variable<Vector2> [Decoration.Location 0u] StorageClass.Input
            let lightLevel = Variable<single> [Decoration.Location 1u] StorageClass.Input
            let mutable outColor = Variable<Vector4> [Decoration.Location 0u] StorageClass.Output

            fun () ->
                let color = sampler.ImplicitLod fragTexCoord
                if color.W < 0.5f then
                    kill ()
                outColor <- Vector4.Multiply(color, lightLevel)
                outColor <- Vector4(outColor.X, outColor.Y, outColor.Z, color.W)
        @>

    let shader = 
        graphics.CreateShader(vertex, fragment,
            Shader(0, true, 
                [ ShaderDescriptorLayout(UniformBufferDescriptor, VertexStage, 0u)
                  ShaderDescriptorLayout(CombinedImageSamplerDescriptor, FragmentStage, 1u) ],
                [ ShaderVertexInput(PerVertex, typeof<Vertex>, 0u) 
                  ShaderVertexInput(PerInstance, typeof<single>, 1u) ]))

    let queueDraw (image: FalkanImage) (lightLevel: int) (vertices: Vector3 []) (uv: Vector2 []) =
        let vertices = Array.init vertices.Length (fun i -> { position = vertices.[i]; uv = uv.[i] })
        let vertexBuffer = graphics.CreateBuffer(vertices.Length, FalkanBufferFlags.None, VertexBuffer, ReadOnlySpan vertices) 
        let doot = Array.init 1 (fun _ -> normalize (single lightLevel) 0.f 255.f)
        let dootBuffer = graphics.CreateBuffer(doot.Length, FalkanBufferFlags.None, VertexBuffer, ReadOnlySpan doot)
        
        let draw = shader.CreateDrawBuilder()
        let draw = draw.AddDescriptorBuffer(mvpUniform, sizeof<ModelViewProjection>).AddDescriptorImage(image).Next
        let draw = draw.AddVertexBuffer(vertexBuffer, PerVertex).AddVertexBuffer(dootBuffer, PerInstance)
           
        shader.AddDraw(draw, uint32 vertices.Length, 1u) |> ignore

    (e1m1.Sectors, e1m1.ComputeAllSectorGeometry())
    ||> Seq.iteri2 (fun i s geo ->
        let image, width, height = getImage graphics wad s.FloorTextureName
        let uv = Map.CreateSectorUv(width, height, geo.FloorVertices)
        queueDraw image s.LightLevel geo.FloorVertices uv

        let image, width, height = getImage graphics wad s.CeilingTextureName
        let uv = Map.CreateSectorUv(width,height, geo.CeilingVertices)
        queueDraw image s.LightLevel geo.CeilingVertices uv
        )


    let queueLinedef geo (ldef: Linedef) sdefIndex =
        match geo.Upper, sdefIndex with
        | Some vertices, Some i ->
            let sdef = e1m1.Sidedefs.[i]
            match sdef.UpperTextureName with
            | Some upperTex ->
                let image, width, height = getImage graphics wad upperTex
                let uv = e1m1.CreateUpperWallUv(ldef, sdef, width, height, vertices)
                queueDraw image (e1m1.Sectors.[sdef.SectorNumber].LightLevel) vertices uv
            | _ -> ()
        | _ -> ()
        
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
                let image, width, height = getImage graphics wad upperTex
                let uv = e1m1.CreateLowerWallUv(ldef, sdef, width, height, vertices)
                queueDraw image (e1m1.Sectors.[sdef.SectorNumber].LightLevel) vertices uv
            | _ -> ()
        | _ -> ()

    e1m1.Linedefs
    |> Seq.iter (fun ldef ->
        let geo = e1m1.ComputeFrontWallGeometry ldef
        queueLinedef geo ldef ldef.FrontSidedefIndex        
        )

    let mapView = loadMapView graphics wad e1m1
    renderMapView graphics mapView mvpUniform

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