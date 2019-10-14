[<AutoOpen>]
module Falkan.PublicHelpers

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

#nowarn "9"
#nowarn "51"

[<RequiresExplicitTypeArguments>]
let mkVertexInputBinding<'T when 'T : unmanaged> binding inputRate =
    VkVertexInputBindingDescription (
        binding = binding,
        stride = uint32 sizeof<'T>,
        inputRate = inputRate
    )

let mkVertexAttributeDescription binding location format offset =
    VkVertexInputAttributeDescription (
        binding = binding,
        location = location,
        format = format,
        offset = offset
    )

[<RequiresExplicitTypeArguments>]
let mkVertexAttributeDescriptions<'T when 'T : unmanaged> locationOffset binding =
    let rec mk (ty: Type) location offset = 
        match ty with
        | _ when ty = typeof<single> -> 
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32_SFLOAT offset|]
    
        | _ when ty = typeof<int> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32_SINT offset|]
    
        | _ when ty = typeof<Numerics.Vector2> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32_SFLOAT offset|]
    
        | _ when ty = typeof<Numerics.Vector3> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32_SFLOAT offset|]
    
        | _ when ty = typeof<Numerics.Vector4> ->
            [|mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT offset|]
    
        | _ when ty = typeof<Numerics.Matrix3x2> ->
            failwith "Matrix3x2 not supported yet."
    
        | _ when ty = typeof<Numerics.Matrix4x4> ->
            [|
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT offset
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT (offset + uint32 sizeof<Numerics.Vector4>)
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT (offset + uint32 (sizeof<Numerics.Vector4> * 2))
                mkVertexAttributeDescription binding location VkFormat.VK_FORMAT_R32G32B32A32_SFLOAT (offset + uint32 (sizeof<Numerics.Vector4> * 3))
            |]
    
        | _ when ty.IsPrimitive ->
            failwithf "Primitive type not supported: %A" ty
    
        | _ when ty.IsValueType ->
            ty.GetFields(Reflection.BindingFlags.NonPublic ||| Reflection.BindingFlags.Public ||| Reflection.BindingFlags.Instance)
            |> Array.mapi (fun i field ->
                mk field.FieldType (location + uint32 i) (Marshal.OffsetOf<'T> field.Name |> uint32)
            )
            |> Array.concat
    
        | _ ->
            failwithf "Type not supported: %A" ty
    
    mk typeof<'T> locationOffset 0u