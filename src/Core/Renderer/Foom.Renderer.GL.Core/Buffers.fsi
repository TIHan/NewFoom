namespace Foom.Renderer.GL

open System.Numerics

[<Sealed>]
type Buffer<'T when 'T : struct> =

    member Set : 'T [] -> unit

    member Set : 'T [] * size: int -> unit

    member Id : int

    member Length : int with get, set

    member Item : index: int -> 'T with set

    member Bind : IGL -> unit

    member TryBufferData : IGL -> bool

    member Release : IGL -> unit

[<AbstractClass; Sealed>]
type Buffer =

    static member Create : Vector2 [] * length: int -> Buffer<Vector2>

    static member Create : Vector3 [] * lenght: int -> Buffer<Vector3>

    static member Create : Vector4 [] * length: int -> Buffer<Vector4>

    static member Create : Vector2 [] -> Buffer<Vector2>

    static member Create : Vector3 [] -> Buffer<Vector3>

    static member Create : Vector4 [] -> Buffer<Vector4>

[<Sealed>]
type Texture2DBuffer =

    new : unit -> Texture2DBuffer

    member Id : int

    member Width : int

    member Height : int

    member HasData : bool

    member Set : nativeint * width: int * height: int -> unit

    member Set : nativeint list * width: int * height: int -> unit

    member Bind : IGL -> unit

    member TryBufferData : IGL -> bool

    member Release : IGL -> unit

[<Sealed>]
type RenderTexture =

    new : width: int * height: int -> RenderTexture

    member Width : int

    member Height : int

    member TextureId : int

    member Bind : IGL -> unit

    member Unbind : IGL -> unit

    member BindTexture : IGL -> unit

    member TryBufferData : IGL -> bool

    member Release : IGL -> unit