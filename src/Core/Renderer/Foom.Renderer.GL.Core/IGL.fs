namespace Foom.Renderer.GL

open System.Numerics

type IGL =

    abstract BindBuffer : int -> unit

    abstract CreateBuffer : unit -> int

    abstract DeleteBuffer : int -> unit

    abstract BufferData : Vector2 [] * count: int * bufferId: int -> unit

    abstract BufferData : Vector3 [] * count: int * bufferId: int -> unit

    abstract BufferData : Vector4 [] * count: int * bufferId: int -> unit

    abstract BufferSubData : Vector2 [] * offset: int * count: int * bufferId: int -> unit

    abstract BufferSubData : Vector3 [] * offset: int * count: int * bufferId: int -> unit

    abstract BufferSubData : Vector4 [] * offset: int * count: int * bufferId: int -> unit


    abstract BindTexture : int -> unit

    abstract ActiveTexture : number: int -> unit

    abstract CreateTexture : width: int * height: int * data: nativeint -> int

    abstract SetSubTexture : xOffset: int * yOffset: int * width: int * height: int * data: nativeint * textureId: int -> unit

    abstract DeleteTexture : int -> unit


    abstract BindFramebuffer : int -> unit

    abstract CreateFramebuffer : unit -> int

    abstract CreateFramebufferTexture : width: int * height: int * data: nativeint -> int

    abstract SetFramebufferTexture : int -> unit

    abstract CreateRenderbuffer : width: int * height: int -> int


    abstract GetUniformLocation : programId: int * name: string -> int

    abstract BindUniform : locationId: int * int -> unit

    abstract BindUniform : locationId: int * float32 -> unit

    abstract BindUniform : locationId: int * Vector2 -> unit

    abstract BindUniform : locationId: int * Vector4 -> unit

    abstract BindUniform : locationId: int * Matrix4x4 -> unit

    abstract BindUniform : locationId: int * int * int [] -> unit


    abstract GetAttributeLocation : programId: int * name: string -> int

    abstract BindAttributePointerFloat32 : locationId: int * size: int -> unit

    abstract EnableAttribute : locationId: int -> unit

    abstract AttributeDivisor : locationId: int * divisor: int -> unit


    abstract DrawTriangles : first: int * count: int -> unit

    abstract DrawTrianglesInstanced : count: int * primcount: int -> unit


    abstract EnableDepthMask : unit -> unit

    abstract DisableDepthMask : unit -> unit

    abstract EnableColorMask : unit -> unit

    abstract DisableColorMask : unit -> unit



    abstract EnableStencilTest : unit -> unit

    abstract DisableStencilTest : unit -> unit

    abstract Stencil1 : unit -> unit // this will change

    abstract Stencil2 : unit -> unit // this will change


    abstract LoadProgram : vertexSource: string * fragmentSource: string -> int

    abstract UseProgram : programId: int -> unit

    abstract DeleteProgram : programId: int -> unit


    abstract EnableDepthTest : unit -> unit

    abstract DisableDepthTest : unit -> unit

    abstract Clear : unit -> unit

    abstract Swap : unit -> unit
