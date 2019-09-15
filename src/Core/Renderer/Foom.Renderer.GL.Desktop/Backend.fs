namespace Foom.Renderer.GL.Desktop

open System
open System.IO
open System.Drawing
open System.Diagnostics
open System.Runtime.InteropServices

open FSharp.NativeInterop

open OpenTK
open OpenTK.Graphics.OpenGL4

open System.Numerics

#nowarn "9"

[<Struct>]
type Application =
    private {
        window: NativeWindow
        glContext: Graphics.GraphicsContext
    }

    member this.Window = this.window

[<Struct>]
type RenderColor =
    val R : single
    val G : single
    val B : single
    val A : single

    new (r, g, b, a) = { R = r; G = g; B = b; A = a }

    static member OfColor (color: Color) =
        RenderColor (
            single color.R / 255.f,
            single color.G / 255.f,
            single color.B / 255.f,
            single color.A / 255.f)

module Backend =

    let init () : Application =
        let graphicsMode = new Graphics.GraphicsMode(Graphics.ColorFormat(32), 24, 8)
        let window = new NativeWindow(1280, 720, "Foom", GameWindowFlags.FixedWindow, graphicsMode, DisplayDevice.Default)

        let glContext = new Graphics.GraphicsContext(graphicsMode, window.WindowInfo, 3, 2, Graphics.GraphicsContextFlags.Default)
        glContext.LoadAll()

        let mutable vao = 0
        GL.GenVertexArrays(1, &vao)

        GL.BindVertexArray vao

        window.Visible <- true
        window.ProcessEvents()
        {
            window = window
            glContext = glContext
        }

    let exit (app: Application) : int =
        app.glContext.Dispose()
        app.window.Close()
        app.window.Dispose()
        0
    
    let clear () : unit = 
        GL.Clear(ClearBufferMask.ColorBufferBit ||| ClearBufferMask.DepthBufferBit ||| ClearBufferMask.StencilBufferBit)

    let draw (app: Application) : unit = 
        app.window.ProcessEvents()
        app.glContext.SwapBuffers()

    let makeVbo () : int =
        let mutable vbo = 0
        GL.GenBuffers(1, &vbo)
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo)
        GL.BufferData(BufferTarget.ArrayBuffer, 0, nativeint 0, BufferUsageHint.DynamicDraw)
        vbo

    let bufferData (data: nativeint) (size: int) (vbo: int) : unit =
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo)
        GL.BufferData(BufferTarget.ArrayBuffer, size, data, BufferUsageHint.DynamicDraw)

    let bufferSubData (data: nativeint) (offset: int) (size: int) (vbo: int) : unit =
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo)
        GL.BufferSubData(BufferTarget.ArrayBuffer, nativeint offset, size, data)

    let getMaxTextureSize () : int =
        GL.GetInteger(GetPName.MaxTextureSize)

    let bindArrayBuffer (bufferId: int) : unit =
        GL.BindBuffer(BufferTarget.ArrayBuffer, bufferId)

    let bindVbo (vbo: int) : unit =
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo)

    let drawArrays (first: int) (count: int) : unit =
        GL.DrawArrays(PrimitiveType.Lines, first, count)

    let drawArraysLoop (first: int) (count : int) : unit =
        GL.DrawArrays(PrimitiveType.LineLoop, first, count)

    let drawTriangles (first: int) (count : int) : unit =
        GL.DrawArrays(PrimitiveType.Triangles, first, count)

    let loadShaders (vertexSource: string) (fragmentSource: string) : int =
        let vertexShaderId = GL.CreateShader(ShaderType.VertexShader)
        let fragmentShaderId = GL.CreateShader(ShaderType.FragmentShader)

        GL.ShaderSource(vertexShaderId, 1, [|vertexSource|], [|vertexSource.Length|])
        GL.CompileShader vertexShaderId

        match GL.GetShaderInfoLog vertexShaderId with
        | str when not (String.IsNullOrWhiteSpace str) -> failwithf "%A" str
        | _ ->

        use fragmentP = fixed fragmentSource
        GL.ShaderSource(fragmentShaderId, 1, [|fragmentSource|], [|fragmentSource.Length|])
        GL.CompileShader fragmentShaderId

        match GL.GetShaderInfoLog fragmentShaderId with
        | str when not (String.IsNullOrWhiteSpace str) -> failwithf "%A" str
        | _ ->

        let programId = GL.CreateProgram()
        GL.AttachShader(programId, vertexShaderId)
        GL.AttachShader(programId, fragmentShaderId)
        GL.LinkProgram(programId)

        match GL.GetProgramInfoLog programId with
        | str when not (String.IsNullOrWhiteSpace str) -> failwithf "%A" str
        | _ ->

        programId

    let useProgram (programId: int) : unit =
        GL.UseProgram programId

    let getAttributeLocation (programId: int) (name: string) =
        GL.GetAttribLocation(programId, name)
         
    let bindVertexAttributePointer_Float (location: int) (size: int) : unit =
        GL.VertexAttribPointer(location, size, VertexAttribPointerType.Float, false, 0, 0)

    let enableVertexAttribute (location: int) : unit =
        GL.EnableVertexAttribArray location

    let disableVertexAttribute (location: int) : unit =
        GL.DisableVertexAttribArray location

    let getUniformLocation (programId: int) (name: string) =
        GL.GetUniformLocation(programId, name)

    //

    let bindVertexAttributeVector3 (id: int) : unit =
        GL.VertexAttribPointer(id, 3, VertexAttribPointerType.Float, false, 0, 0)
        GL.EnableVertexAttribArray id

    let bindVertexAttributeVector2 (id: int) : unit =
        GL.VertexAttribPointer(id, 2, VertexAttribPointerType.Float, false, 0, 0)
        GL.EnableVertexAttribArray id

    //

    let bindUniformMatrix4x4 (id: int) (value: Matrix4x4) : unit =
        let mutable value = value
        GL.UniformMatrix4(id, 1, false, &&value |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<float32>)

    let bindUniformVector4 (id: int) (value: Vector4) : unit =
        GL.Uniform4 (id, value.X, value.Y, value.Z, value.W)

    let bindUniformInt (id: int) (value: int) : unit =
        GL.Uniform1(id, value)

    let bindUniformIntVarying (id: int) (size: int) (values: nativeint) : unit =
        GL.Uniform1(id, size, values |> NativePtr.ofNativeInt<int>)

    let bindUniform_float (id: int) (value: float32) : unit =
        GL.Uniform1(id, value)

    let bindUniformVector2 (id: int) (value: Vector2) : unit =
        GL.Uniform2(id, value.X, value.Y)

    let activeTexture (number: int) : unit =
        GL.ActiveTexture(TextureUnit.Texture0 + LanguagePrimitives.EnumOfValue number)

    let bindTexture2D (textureId: int) : unit =
        GL.BindTexture(TextureTarget.Texture2D, textureId)

    let setUniformMatrix4x4 (uni: int) (m: Matrix4x4)  : unit =
        let mutable value = m
        GL.UniformMatrix4(uni, 1, false, &&value |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<float32>)

    let getUniformColor (program: int) : int =
        GL.GetUniformLocation(program, "uni_color")

    let setUniformColor (uniformColor: int) (color: RenderColor) : unit =
        GL.Uniform4(uniformColor, color.R, color.G, color.B, color.A)

    let setUniformVector4 (v: Vector4) (uniformId: int) : unit =
        GL.Uniform4(uniformId, v.X, v.Y, v.Z, v.W)

    let setTexture (shaderProgram: int) (textureId: int) : unit =
        GL.GetUniformLocation(shaderProgram, "uni_texture") |> ignore
        GL.Uniform1(textureId, 0)

    let createTexture (width: int) (height: int) (data: nativeint) : int =
        let textureId = GL.GenTexture()

        GL.BindTexture(TextureTarget.Texture2D, textureId)

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, int TextureWrapMode.Repeat)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, int TextureWrapMode.Repeat)

        textureId

    let enableDepth () : unit =
        GL.Enable EnableCap.DepthTest
        GL.DepthFunc DepthFunction.Less
        GL.Enable EnableCap.CullFace

    let disableDepth () : unit =
        GL.Disable EnableCap.CullFace
        GL.Disable EnableCap.DepthTest

    let enableStencilTest () : unit =
        GL.Enable EnableCap.StencilTest

    let disableStencilTest () : unit =
        GL.Disable EnableCap.StencilTest

    let enableBlend () : unit =
        GL.Enable EnableCap.Blend
        GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha)

    let disableBlend () : unit =
        GL.Disable EnableCap.Blend

    let createFramebuffer () : int =
        GL.GenFramebuffer()

    let bindFramebuffer (id: int) : unit =
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, id)

    let createFramebufferTexture (width: int) (height: int) (data: nativeint) : int =
        let textureId = GL.GenTexture()

        GL.BindTexture(TextureTarget.Texture2D, textureId)

        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, data)

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, int TextureMagFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, int TextureMinFilter.Nearest)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, int TextureWrapMode.ClampToEdge)
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, int TextureWrapMode.ClampToEdge)

        textureId

    let createRenderbuffer (width: int) (height: int) : int =
        let depthrenderbuffer = GL.GenRenderbuffer()
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, depthrenderbuffer)
        GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, RenderbufferStorage.Depth32fStencil8, width, height)
        GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthStencilAttachment, RenderbufferTarget.Renderbuffer, depthrenderbuffer)
        GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, 0)
        depthrenderbuffer

    let setFramebufferTexture  (textureId: int) : unit =
        GL.FramebufferTexture(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, textureId, 0)
        let mutable drawBuffers = DrawBuffersEnum.ColorAttachment0
        GL.DrawBuffers(1, &drawBuffers)

    let framebufferRender (width: int) (height: int) (id: int) : unit =
        GL.BindFramebuffer(FramebufferTarget.Framebuffer, id)
        GL.Viewport(0, 0, width, height)

    let enableCullFace () : unit =
        GL.Enable EnableCap.CullFace

    let disableCullFace () : unit =
        GL.Disable EnableCap.CullFace

    let clearDepth () : unit =
        GL.Clear ClearBufferMask.DepthBufferBit

    let clearColor () : unit =
        GL.Clear ClearBufferMask.ColorBufferBit

    let clearStencil () : unit =
        GL.Enable EnableCap.StencilTest
        GL.StencilMask 0xFF
        GL.Clear ClearBufferMask.StencilBufferBit
        GL.StencilMask 0x00
        GL.Disable EnableCap.StencilTest

    let deleteBuffer (id: int) : unit =
        GL.DeleteBuffer id

    let deleteTexture (id: int) : unit =
        GL.DeleteTexture id

    let deleteProgram(programId: int) : unit =
        GL.DeleteProgram programId

    let colorMaskFalse () : unit =
        GL.ColorMask(false, false, false, false)

    let colorMaskTrue () : unit =
        GL.ColorMask(true, true, true, true)

    let stencil1 () : unit =
        GL.StencilFunc(StencilFunction.Always, 1, 0xFF)
        GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace)
        GL.StencilMask 0xFF
        GL.Clear ClearBufferMask.StencilBufferBit

    let stencil2 () : unit =
        GL.StencilFunc(StencilFunction.Equal, 1, 0xFF)
        GL.StencilMask 0x00

    let depthMaskFalse () : unit =
        GL.DepthMask false

    let depthMaskTrue () : unit =
        GL.DepthMask true

    let glVertexAttribDivisor (location: int) (divisor: int) : unit =
        GL.VertexAttribDivisor(location, divisor)

    let drawTrianglesInstanced (count: int) (primcount: int) : unit =
        GL.DrawArraysInstanced(PrimitiveType.Triangles, 0, count, primcount)

    let setSubTexture (xOffset: int) (yOffset: int) (width: int) (height: int) (data: nativeint) (textureId: int) : unit =
        GL.BindTexture(TextureTarget.Texture2D, textureId)
        GL.TexSubImage2D(TextureTarget.Texture2D, 0, xOffset, yOffset, width, height, PixelFormat.Rgba, PixelType.UnsignedByte, data)

    let checkError () : int =
        GL.GetError()
        |> int