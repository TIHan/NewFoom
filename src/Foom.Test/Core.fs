﻿[<AutoOpen>]
module Foom.Test.Core

// 28 - e1m1 doom
// 933 - map10 sunder
// 20 - map10 sunder
// 151 - map10 sunder
// 439 - map08 sunder
// 271 - map03 sunder
// 663 - map05 sunder
// 506 - map04 sunder
// 3 - map02 sunder
// 3450 - map11 sunder
// 1558 - map11 sunder
// 240 - map07 sunder
// 2021 - map11 sunder

open System
open System.IO
open System.IO.Compression
open System.Diagnostics
open System.Numerics
open SkiaSharp
open Foom.Wad
open Foom.IO.Message
open Foom.Wad.Extensions
open Foom.Geometry
open Foom.Renderer
open Foom.Renderer.GL
open Foom.Renderer.GL.Desktop
open Foom.Input
open Foom.Input.Desktop
open Foom.Net
open Foom.EntityManager
open Foom.NativeCollections

let createBitmap (pixels: Pixel [,]) =
    let mutable isTransparent = false

    let width = Array2D.length1 pixels
    let height = Array2D.length2 pixels

    pixels
    |> Array2D.iter (fun p ->
        if p.Equals Pixel.Cyan then
            isTransparent <- true
    )

    let alphaType = if not isTransparent then SKAlphaType.Opaque else SKAlphaType.Premul
    let bitmap = new SKBitmap (width, height, SKColorType.Rgba8888, alphaType)
    for i = 0 to width - 1 do
        for j = 0 to height - 1 do
            let pixel = pixels.[i, j]
            if pixel = Pixel.Cyan then
                bitmap.SetPixel (i, j, SKColor (0uy, 0uy, 0uy, 0uy))
            else
                bitmap.SetPixel (i, j, SKColor (pixel.R, pixel.G, pixel.B))

    bitmap

let savePng name pixels =
    use image = SKImage.FromBitmap(createBitmap pixels)
    use data = image.Encode ()
#if __IOS__ || __ANDROID__
    use fs = File.OpenWrite (Path.Combine (documents, name))
#else
    use fs = File.OpenWrite (name)
#endif

    data.SaveTo (fs)

let tryCreateFloor (wad: Wad) (sector: Sector) (geo: SectorGeometry) (renderer: IRenderer) =
    match wad.TryFindFlatTexture sector.FloorTextureName with
    | Some wadtex ->
        use bitmap = createBitmap wadtex.Data
        let material = renderer.CreateMaterial(bitmap.GetPixels(), wadtex.Width, wadtex.Height, MaterialType.Mesh)
        let uvs = Map.CreateSectorUv(wadtex.Width, wadtex.Height, geo.FloorVertices)
        let colors = Map.CreateColors(sector.LightLevel, geo.FloorVertices.Length)
        let mesh = renderer.CreateMesh(geo.FloorVertices, uvs, colors, material, 0)
        Some mesh
    | _ -> 
        printfn "Couldn't find texture: %s" sector.FloorTextureName
        None

let tryCreateCeiling (wad: Wad) (sector: Sector) (geo: SectorGeometry) (renderer: IRenderer) =
    match wad.TryFindFlatTexture sector.CeilingTextureName with
    | Some wadtex ->
        use bitmap = createBitmap wadtex.Data
        let material = renderer.CreateMaterial(bitmap.GetPixels(), wadtex.Width, wadtex.Height, MaterialType.Mesh)
        let uvs = Map.CreateSectorUv(wadtex.Width, wadtex.Height, geo.CeilingVertices)
        let colors = Map.CreateColors(sector.LightLevel, geo.CeilingVertices.Length)
        let mesh = renderer.CreateMesh(geo.CeilingVertices, uvs, colors, material, 0)
        Some mesh
    | _ -> 
        printfn "Couldn't find texture: %s" sector.CeilingTextureName
        None

let tryCreateUpperWall (wad: Wad) (map: Map) (sector: Sector) (linedef: Linedef) (sidedef: Sidedef) (geo: WallGeometry) (renderer: IRenderer) =
    match geo.Upper, sidedef.UpperTextureName with
    | Some upper, Some upperTex ->
        match wad.TryFindTexture upperTex with
        | Some wadtex ->
            use bitmap = createBitmap wadtex.Data
            let material = renderer.CreateMaterial(bitmap.GetPixels(), wadtex.Width, wadtex.Height, MaterialType.Mesh)
            let uvs = map.CreateUpperWallUv(linedef, sidedef, wadtex.Width, wadtex.Height, upper)
            let colors = Map.CreateColors(sector.LightLevel, upper.Length)
            let mesh = renderer.CreateMesh(upper, uvs, colors, material, 0)
            Some mesh
        | _ ->
            printfn "Couldn't find texture: %s" upperTex
            None
    | _ -> 
        None

let tryCreateMiddleWall (wad: Wad) (map: Map) (sector: Sector) (linedef: Linedef) (sidedef: Sidedef) (geo: WallGeometry) (renderer: IRenderer) =
    match geo.Middle, sidedef.MiddleTextureName with
    | Some middle, Some middleTex ->
        match wad.TryFindTexture middleTex with
        | Some wadtex ->
            use bitmap = createBitmap wadtex.Data
            let material = renderer.CreateMaterial(bitmap.GetPixels(), wadtex.Width, wadtex.Height, MaterialType.Mesh)
            let uvs = map.CreateMiddleWallUv(linedef, sidedef, wadtex.Width, wadtex.Height, middle)
            let colors = Map.CreateColors(sector.LightLevel, middle.Length)
            let mesh = renderer.CreateMesh(middle, uvs, colors, material, 0)
            Some mesh
        | _ ->
            printfn "Couldn't find texture: %s" middleTex
            None
    | _ -> 
        None

let tryCreateLowerWall (wad: Wad) (map: Map) (sector: Sector) (linedef: Linedef) (sidedef: Sidedef) (geo: WallGeometry) (renderer: IRenderer) =
    match geo.Lower, sidedef.LowerTextureName with
    | Some lower, Some lowerTex ->
        match wad.TryFindTexture lowerTex with
        | Some wadtex ->
            use bitmap = createBitmap wadtex.Data
            let material = renderer.CreateMaterial(bitmap.GetPixels(), wadtex.Width, wadtex.Height, MaterialType.Mesh)
            let uvs = map.CreateMiddleWallUv(linedef, sidedef, wadtex.Width, wadtex.Height, lower)
            let colors = Map.CreateColors(sector.LightLevel, lower.Length)
            let mesh = renderer.CreateMesh(lower, uvs, colors, material, 0)
            Some mesh
        | _ ->
            printfn "Couldn't find texture: %s" lowerTex
            None
    | _ -> 
        None

let loadMap mapName (camera: Camera) (renderer: IRenderer) =
    let wad = Wad.FromFile("../../../../../Foom-deps/testwads/doom1.wad")
    let map = wad.FindMap mapName

    let music = wad.FindMusic "d_e1m1"

    let player1Start = map.TryFindPlayer1Start()
    player1Start |> Option.iter(fun doomThing ->
        let position = Vector3 (single doomThing.X, single doomThing.Y, 28.f)

        camera.Translation <- position

        printfn "Position: %A" camera.Translation
        printfn "Player 1 Start Found"
    )

    (map.ComputeAllSectorGeometry(), map.Sectors)
    ||> Seq.iter2(fun geo sector ->
        tryCreateFloor wad sector geo renderer |> ignore
        tryCreateCeiling wad sector geo renderer |> ignore
    )

    map.Linedefs
    |> Seq.iter(fun linedef ->
        let geo = map.ComputeFrontWallGeometry(linedef)
        match linedef.FrontSidedefIndex with
        | Some i -> 
            let sidedef = map.Sidedefs.[i]
            let sector = map.Sectors.[sidedef.SectorNumber]
            tryCreateUpperWall wad map sector linedef sidedef geo renderer |> ignore
            tryCreateMiddleWall wad map sector linedef sidedef geo renderer |> ignore
            tryCreateLowerWall wad map sector linedef sidedef geo renderer |> ignore
        | _ -> ()
    )

    let zombiemanSpriteBatch =
        match wad.TryFindSpriteTexture("POSSA1") with
        | Some(wadtex) ->
            use bitmap = createBitmap wadtex.Data
            let material = renderer.CreateMaterial(bitmap.GetPixels(), bitmap.Width, bitmap.Height, MaterialType.Sprite)
            renderer.CreateSpriteBatch(Vector4.Zero, material, 0)
        | _ -> failwith "did not find zombieman texture"

    map.Things
    |> Seq.iter(function
        | Thing.Doom thing ->
            match thing.Type with
            | ThingType.Zombieman ->
                let sprite = zombiemanSpriteBatch.CreateSprite()
                zombiemanSpriteBatch.SetSpritePosition(sprite, Vector3(single thing.X, single thing.Y, 0.f))
            | _ -> ()
        | _ -> ()
      //  match x with
    )

    zombiemanSpriteBatch

[<Struct>]
type Movement =
    {
        Yaw: float32
        Pitch: float32
        IsMovingForward: bool
        IsMovingLeft: bool
        IsMovingBackward: bool
        IsMovingRight: bool
    }

    static member Default =
        {
            Yaw = 0.f
            Pitch = 0.f
            IsMovingForward = false
            IsMovingLeft = false
            IsMovingBackward = false
            IsMovingRight = false
        }

let getMovement (input: InputState) mov =
    (mov, input.Events)
    ||> List.fold (fun mov -> function
        | MouseMoved (x, y, xrel, yrel) ->
            { mov with
                Yaw = mov.Yaw + (single xrel * -0.25f) * (float32 Math.PI / 180.f)
                Pitch = mov.Pitch + (single yrel * -0.25f) * (float32 Math.PI / 180.f)
            }

        | KeyPressed x when x = 'w' -> { mov with IsMovingForward = true }
        | KeyReleased x when x = 'w' -> { mov with IsMovingForward = false }

        | KeyPressed x when x = 'a' -> { mov with IsMovingLeft = true }
        | KeyReleased x when x = 'a' -> { mov with IsMovingLeft = false }

        | KeyPressed x when x = 's' -> { mov with IsMovingBackward = true }
        | KeyReleased x when x = 's' -> { mov with IsMovingBackward = false }

        | KeyPressed x when x = 'd' -> { mov with IsMovingRight = true }
        | KeyReleased x when x = 'd' -> { mov with IsMovingRight = false }
        | _ -> mov
    )

[<Struct>]
type Transform =
    val mutable position : Vector3
    val mutable rotation : Quaternion

    interface IComponent

[<Struct>]
type Direction =
    val mutable value : byte

    interface IComponent

[<Struct>]
type Render =
    val mutable index : int
    val mutable frame : uint16

    interface IComponent

[<Struct>]
type UserControlled =
    val mutable clientId : ClientId
    val mutable movement : Movement

    interface IComponent

type SpectatorTag = struct interface IComponent end

[<Struct>]
type PlayerSnapshot =
    val mutable entity : Entity
    val mutable position : Vector3
    val mutable rotation : Quaternion
    val mutable renderIndex : int
    val mutable renderFrame : uint16
    val mutable direction : byte
    val mutable clientId : ClientId

[<Struct>]
type RenderableSnapshot =
    val mutable entity : Entity
    val mutable position : Vector3
    val mutable renderIndex : int
    val mutable renderFrame : uint16
    val mutable direction : byte

let updatePlayer (mov: Movement) (player: byref<Transform>) =
    let isMovingForward = mov.IsMovingForward
    let isMovingLeft = mov.IsMovingLeft
    let isMovingBackward = mov.IsMovingBackward
    let isMovingRight = mov.IsMovingRight

    let mutable acc = Vector3.Zero
                            
    if isMovingForward then
        let v = Vector3.Transform (-Vector3.UnitZ, player.rotation)
        acc <- (Vector3 (v.X, v.Y, v.Z))

    if isMovingLeft then
        let v = Vector3.Transform (-Vector3.UnitX, player.rotation)
        acc <- acc + (Vector3 (v.X, v.Y, v.Z))

    if isMovingBackward then
        let v = Vector3.Transform (Vector3.UnitZ, player.rotation)
        acc <- acc + (Vector3 (v.X, v.Y, v.Z))

    if isMovingRight then
        let v = Vector3.Transform (Vector3.UnitX, player.rotation)
        acc <- acc + (Vector3 (v.X, v.Y, v.Z))
   
    acc <- 
        if acc <> Vector3.Zero then
            (acc |> Vector3.Normalize) * 10.f
        else
            acc

    player.rotation <- Quaternion.CreateFromAxisAngle (Vector3.UnitX, 90.f * (float32 Math.PI / 180.f))

    player.rotation <- player.rotation *
        Quaternion.CreateFromYawPitchRoll (
            mov.Yaw * 0.25f,
            mov.Pitch * 0.25f,
            0.f
        )

    player.position <- player.position + acc

type UserInfo() =
    inherit NetMessage()

    let mutable movement = Unchecked.defaultof<Movement>

    member __.Movement
        with get () = movement
        and set value = movement <- value

    override this.NetSerialize(writer, stream) =
        writer.Write(stream, &movement)

type Snapshot =
    inherit NetMessage

    val mutable snapshotId : int64
    val mutable serverTime : TimeSpan
    val mutable playerCount : int
    val mutable playerSnapshots : NativeArray<PlayerSnapshot>

    new () = { snapshotId = 0L; serverTime = TimeSpan.Zero; playerCount = 0; playerSnapshots = new NativeArray<PlayerSnapshot>(0) }

    override this.NetSerialize(writer, stream) =
        writer.WriteInt64(stream, &this.snapshotId)
        writer.Write(stream, &this.serverTime)
        writer.WriteInt(stream, &this.playerCount)
        for i = 0 to this.playerCount - 1 do
            let r = &this.playerSnapshots.[i]
            writer.Write(stream, &r)

    override this.NetReset() =
      //  (this.playerSnapshots :> IDisposable).Dispose() // TODO: Fix this.
        this.playerSnapshots <- new NativeArray<PlayerSnapshot>(0)
