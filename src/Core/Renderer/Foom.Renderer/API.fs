namespace Foom.Renderer

open System.IO
open System.Numerics

[<AbstractClass>]
type Material internal() = class end

[<AbstractClass>]
type Camera internal() =

    abstract Translation : Vector3 with get, set

    abstract Rotation : Quaternion with get, set

    abstract View : Matrix4x4 with get, set 

    abstract ViewLerp : Matrix4x4 with get, set

    abstract Projection : Matrix4x4 with get, set

type IMesh = interface end

type ISpriteBatch =
    inherit IMesh

    abstract CreateSprite : ?position: Vector3 * ?frame: int * ?size: Vector2 -> int

    abstract DeleteSprite : spriteIndex: int -> unit

    abstract SetSpritePosition : spriteIndex: int * Vector3 -> unit

    abstract SetSpriteFrame : spriteIndex: int * frame: int -> unit

    abstract SetSpriteSize : spriteIndex: int * size: Vector2 -> unit

[<RequireQualifiedAccess>]
type MaterialType =
    | Mesh
    | Sprite

type IRenderer =

    abstract CreateMaterial : pixelData: nativeint * width: int * height: int * MaterialType -> Material

    abstract CreateCamera : view: Matrix4x4 * projection: Matrix4x4 * layerMask: int -> Camera

    abstract CreateMesh : vertices: Vector3 [] * uvs: Vector2 [] * colors: Vector4 [] * Material * layerMask: int -> IMesh

    abstract CreateSpriteBatch : color: Vector4 * Material * layerMask: int -> ISpriteBatch


    abstract DeleteMesh : IMesh -> unit

    abstract DeleteMaterial : Material -> unit

    abstract Draw : time: float32 -> unit

open System.Runtime.CompilerServices

[<assembly:InternalsVisibleTo("Foom.Renderer.GL.Core")>]
()
