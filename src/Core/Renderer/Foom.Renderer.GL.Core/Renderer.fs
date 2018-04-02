namespace Foom.Renderer.GL

open System.IO
open System.Numerics
open System.Collections.Generic
open Foom.Renderer

type ShaderId = int
type TextureId = int
type MeshId = uint32

type Manager(gl: IGL) =

    let lookup = Dictionary<ShaderId, Shader * Dictionary<TextureId, Texture * Dictionary<MeshId, MeshImpl>>>()

    member __.TryAdd(shader: Shader, texture: Texture, mesh: MeshImpl) =
        if texture.Id <= 0 || shader.Id <= 0 then
            false
        else

        let textureLookup =
            match lookup.TryGetValue shader.Id with
            | true, (_, textureLookup) -> textureLookup
            | _ ->
                let textureLookup = Dictionary()
                lookup.Add(shader.Id, (shader, textureLookup))
                textureLookup

        let meshes =
            match textureLookup.TryGetValue texture.Id with
            | false, _ -> 
                let meshes = Dictionary()
                textureLookup.Add(texture.Id, (texture, meshes))
                meshes
            | _, (_, meshes) -> meshes

        meshes.Add(mesh.Id, mesh) |> ignore

        true

    // Deletes a shader and texture if they are not referenced anywhere else.
    member __.TryDeleteShader(shader: Shader) =
        if shader.Id <= 0 then
            false
        else

        match lookup.TryGetValue shader.Id with
        | true, (_, textureLookup) ->
            lookup.Remove shader.Id |> ignore
            gl.DeleteProgram shader.ProgramId |> ignore

            true
        | _ -> false

    member __.Draw(deltaTime: float32, camera: CameraImpl) =
        let view = Matrix4x4.Lerp (camera.ViewLerp, camera.View, deltaTime)

        let mutable invertedView = Matrix4x4.Identity
        Matrix4x4.Invert(view, &invertedView) |> ignore

        gl.EnableDepthTest()

        lookup
        |> Seq.iter(fun pair ->
            let shader, textureLookup = pair.Value
            let input = shader.Input

            gl.UseProgram shader.ProgramId

            textureLookup
            |> Seq.iter(fun pair ->

                // TODO: Bring this out at some point.
                input.Time.Set deltaTime
                input.View.Set invertedView
                input.Projection.Set camera.Projection

                let texture, meshes = pair.Value
                let texbuf = texture.Buffer

                input.Texture.Set [|texbuf|]
                input.TextureResolution.Set(Vector2(single texbuf.Width, single texbuf.Height))

                meshes
                |> Seq.iter(fun pair ->
                    let mesh = pair.Value

                    mesh.SetInput input

                    shader.Program.Run()
                )

                shader.Program.Unbind()
            )

            gl.UseProgram 0
        )

        gl.DisableDepthTest()

// ************************************

module ShaderSource =
    let meshVertex =
        """
#version 330 core

in vec3 position;
in vec2 in_uv;
in vec4 in_color;

uniform mat4x4 uni_projection;
uniform mat4x4 uni_view;

out vec2 uv;
out vec4 color;

void main ()
{
    vec4 snapToPixel = uni_projection * uni_view * vec4(position, 1.0);
    vec4 vertex = snapToPixel;

    //vertex.xyz = snapToPixel.xyz / snapToPixel.w;
    //vertex.x = floor(160 * vertex.x) / 160;
    //vertex.y = floor(120 * vertex.y) / 120;
    //vertex.xyz = vertex.xyz * snapToPixel.w;



    gl_Position = vertex;


    uv = in_uv;
    color = in_color;
}
        """

    let meshFragment =
        """
#version 330 core

precision highp float;

uniform sampler2D uni_texture;

in vec2 uv;
in vec4 color;

out vec4 outColor;

void main()
{
    vec4 newColor = texture(uni_texture, uv) * color;
    if(newColor.a < 0.5)
        discard;
    outColor = newColor;
}
        """

    let spriteVertex =
        """
#version 330 core

in vec3 position;
in vec2 in_uv;
in vec4 in_color;


in vec3 instance_position;
in vec4 instance_lightLevel;
in vec4 instance_uvOffset;
in vec2 instance_size;

uniform mat4x4 uni_projection;
uniform mat4x4 uni_view;
uniform vec2 uTextureResolution;

out vec2 uv;
out vec4 color;
out vec4 lightLevel;

void main ()
{
    float offsetX = instance_uvOffset.x;
    float offsetY = instance_uvOffset.y;
    float width = instance_uvOffset.z;
    float height = instance_uvOffset.w;

    if (instance_size != vec2(0))
    {
        width = instance_size.x;
        height = instance_size.y;
    }

    float halfX = width / 2.0;
    vec3 min = vec3 (-halfX, 0, 0);
    vec3 max = vec3 (halfX, 0, height);
    vec3 mid = min + ((max - min) / 2.0);

    vec3 pos0 = position * vec3 (halfX, 0, height);

    vec3 CameraRight_worldspace = vec3 (uni_view[0][0], uni_view[1][0], uni_view[2][0]);
    vec3 CameraUp_worldspace = vec3 (uni_view[0][1], uni_view[1][1], uni_view[2][1]);

    vec3 c = vec3 (mid.x, 0, pos0.z) + instance_position;
    vec3 pos = pos0 - c + instance_position;
    vec3 vertexPosition_worldspace =
        c
        + CameraRight_worldspace * pos.x + CameraUp_worldspace * pos.z;

    //vertexPosition_worldspace.z = position.z;
    //vertexPosition_worldspace.y += in_center.y;
    //vertexPosition_worldspace.x += in_center.x;

    vec4 snapToPixel = uni_projection * uni_view * vec4(vertexPosition_worldspace, 1.0);
    vec4 vertex = snapToPixel;

    //vertex.xyz = snapToPixel.xyz / snapToPixel.w;
    //vertex.x = floor(160 * vertex.x) / 160;
    //vertex.y = floor(120 * vertex.y) / 120;
    //vertex.xyz = vertex.xyz * snapToPixel.w;



    gl_Position = vertex;

    vec2 maxSize = uTextureResolution;

    if (instance_size != vec2(0))
    {
        maxSize = instance_size;
    }

    float uvX = (width / maxSize.x) * in_uv.x + (offsetX / maxSize.x);
    float uvY = (height / maxSize.y) * in_uv.y + (offsetY / maxSize.y);

    uv = vec2 (uvX, uvY - (1.0 - height / maxSize.y));
    color = in_color;
    lightLevel = instance_lightLevel;
}
        """

    let spriteFragment =
        """
#version 330 core

precision highp float;

uniform sampler2D uni_texture;

in vec2 uv;
in vec4 color;
in vec4 lightLevel;

out vec4 outColor;

void main()
{
    vec4 newColor = texture(uni_texture, uv); // TODO: add lightlevel back
    if(newColor.a < 0.5)
        discard;
    outColor = newColor;
}
        """

type Renderer(gl) =

    let shaders = ShaderManager()
    let textures = TextureManager()
    let meshes = MeshManager()

    let manager = Manager(gl)

    let mutable camera = CameraImpl(0, Matrix4x4.Identity, Matrix4x4.Identity)

    let defaultMeshShaderProgram = ShaderProgram.Create(gl, ShaderSource.meshVertex, ShaderSource.meshFragment)
    let defaultMeshShader = shaders.Create(defaultMeshShaderProgram, MeshInput defaultMeshShaderProgram)

    let spriteBatchShaderProgram = ShaderProgram.Create(gl, ShaderSource.spriteVertex, ShaderSource.spriteFragment)
    let spriteBatchShader = shaders.Create(spriteBatchShaderProgram, SpriteBatchInput spriteBatchShaderProgram)

    member this.InternalCreateMaterial(pixelData: nativeint, width, height, shader) = 
        let texbuf = Texture2DBuffer()
        texbuf.Set(pixelData, width, height)
        texbuf.TryBufferData gl |> ignore

        let tex = textures.Create texbuf

        MaterialImpl(shader, tex) :> Material

    interface IRenderer with

        member this.CreateMaterial(pixelData, width, height, materialType) =
            match materialType with
            | MaterialType.Mesh -> 
                this.InternalCreateMaterial(pixelData, width, height, defaultMeshShader)
            | MaterialType.Sprite ->
                this.InternalCreateMaterial(pixelData, width, height, spriteBatchShader)

        member this.CreateCamera (view, projection, layerMask) = 
            camera <- CameraImpl(0, view, projection)
            camera :> Camera

        member this.CreateMesh(vertices, uvs, colors, material, layerMask) =
            match material with
            | :? MaterialImpl as material ->
                let mesh = meshes.Create(Array.copy vertices, Array.copy uvs, Array.copy colors)
                manager.TryAdd(material.Shader, material.Texture, mesh) |> ignore
                mesh :> IMesh
            | _ -> failwith "Wrong Material Type"

        member this.CreateSpriteBatch(_color, material, layerMask) =
            match material with
            | :? MaterialImpl as material ->
                let spriteBatch = SpriteBatch(0u, single material.Texture.Buffer.Width, single material.Texture.Buffer.Height)
                meshes.AssignId(spriteBatch)
                manager.TryAdd(material.Shader, material.Texture, spriteBatch) |> ignore
                spriteBatch :> ISpriteBatch
            | _ -> failwith "Wrong Material Type"

        member this.DeleteMesh mesh =
            match mesh with
            | :? SpriteBatch as spriteBatch -> ()
            | :? MeshImpl as mesh -> ()
            | _ -> failwith "Wrong Mesh Type"

        member this.DeleteMaterial material =
            match material with
            | :? MaterialImpl as material -> ()
            | _ -> failwith "Wrong Material Type"

        member this.Draw time =
            gl.Clear()
            manager.Draw(time, camera)
            gl.Swap()




