namespace Foom.Renderer.GL

open System.Numerics
open System.Collections.Generic
open Foom.Renderer

module SpriteHelpers =

    let createSpriteColor lightLevel =
        let color = Array.init 6 (fun _ -> Vector4 (255.f, lightLevel, lightLevel, lightLevel))
        color
        |> Array.map (fun c ->
            Vector4 (
                c.X / 255.f,
                c.Y / 255.f,
                c.Z / 255.f,
                c.W / 255.f)
        )

    let vertices =
        [|
            Vector3 (-1.f, 0.f, 0.f)
            Vector3 (1.f, 0.f, 0.f)
            Vector3 (1.f, 0.f, 1.f)
            Vector3 (1.f, 0.f, 1.f)
            Vector3 (-1.f, 0.f, 1.f)
            Vector3 (-1.f, 0.f, 0.f)
        |]

    let uv =
        [|
            Vector2 (0.f, 0.f * -1.f)
            Vector2 (1.f, 0.f * -1.f)
            Vector2 (1.f, 1.f * -1.f)
            Vector2 (1.f, 1.f * -1.f)
            Vector2 (0.f, 1.f * -1.f)
            Vector2 (0.f, 0.f * -1.f)
        |]

    let defaultColor =
        let lightLevel = 255.f
        createSpriteColor lightLevel

type SpriteBatchInput (shaderInput) =
    inherit MeshInput (shaderInput)


    member val Sizes = shaderInput.CreateInstanceAttribute<Buffer<Vector2>>("instance_size")

    member val Positions = shaderInput.CreateInstanceAttribute<Buffer<Vector3>>("instance_position")

    member val LightLevels = shaderInput.CreateInstanceAttribute<Buffer<Vector4>>("instance_lightLevel")

    member val UvOffsets = shaderInput.CreateInstanceAttribute<Buffer<Vector4>>("instance_uvOffset")

and SpriteBatch (id, width, height) =
    inherit MeshImpl (id, SpriteHelpers.vertices, SpriteHelpers.uv, SpriteHelpers.defaultColor)

    let mutable nextIndex = 0
    let indexQueue = Queue()
    let indexMap = Array.zeroCreate 10000
    let indexMapReverse = Array.zeroCreate 10000

    let newIndex() =
        if indexQueue.Count > 0 then
            indexQueue.Dequeue()
        else
            let result = nextIndex
            nextIndex <- nextIndex + 1
            result

    let mutable count = 0
    let positions = Array.zeroCreate<Vector3> 10000
    let lightLevels = Array.zeroCreate<Vector4> 10000
    let uvOffsets = Array.zeroCreate<Vector4> 10000
    let sizes = Array.zeroCreate<Vector2> 10000

    let positionBuffer = Buffer.Create(positions, 0)
    let lightLevelBuffer = Buffer.Create(lightLevels, 0)
    let uvOffsetBuffer = Buffer.Create(uvOffsets, 0)
    let sizeBuffer = Buffer.Create(sizes, 0)

    override this.Release(gl) =
        base.Release(gl)
        positionBuffer.Release(gl)
        lightLevelBuffer.Release(gl)
        uvOffsetBuffer.Release(gl)

    override this.SetInput(input) =
        base.SetInput(input)

        positionBuffer.Set(positions, count)
        lightLevelBuffer.Set(lightLevels, count)
        uvOffsetBuffer.Set(uvOffsets, count)
        sizeBuffer.Set(sizes, count)

        match input with
        | :? SpriteBatchInput as input ->
            input.Positions.Set(positionBuffer)
            input.LightLevels.Set(lightLevelBuffer)
            input.UvOffsets.Set(uvOffsetBuffer)
            input.Sizes.Set(sizeBuffer)
        | _ -> ()

    interface ISpriteBatch with

        member this.CreateSprite(?position, ?frame, ?sizee) =
            let position = defaultArg position Vector3.Zero
            let size = defaultArg sizee Vector2.Zero
            let uvOffset = Vector4(0.f, 0.f, width, height)
            let lightLevel = 255.f

            let i = count
            uvOffsets.[i] <- uvOffset
            lightLevels.[i] <- Vector4 (255.f, lightLevel, lightLevel, lightLevel)
            sizes.[i] <- size

            count <- count + 1

            let index = newIndex()
            indexMap.[index] <- i
            indexMapReverse.[i] <- index
            index

        member __.DeleteSprite(spriteIndex) =
            let i = indexMap.[spriteIndex]
            let last = count - 1

            positions.[i] <- positions.[last]
            lightLevels.[i] <- lightLevels.[last]
            uvOffsets.[i] <- uvOffsets.[last]
            sizes.[i] <- sizes.[last]

            count <- count - 1

            let lastIndex = indexMapReverse.[last]
            indexMap.[lastIndex] <- i
            indexMapReverse.[i] <- lastIndex

            indexQueue.Enqueue(spriteIndex)

        member this.SetSpritePosition(spriteIndex, position) =
            positions.[indexMap.[spriteIndex]] <- position

        member this.SetSpriteFrame(sprite, frame) =
            raise <| System.NotImplementedException()

        member this.SetSpriteSize(spriteIndex, size) =
            sizes.[indexMap.[spriteIndex]] <- size
