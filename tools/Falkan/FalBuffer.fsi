[<AutoOpen>]
module Falkan.Buffer

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.NativeInterop
open FSharp.Vulkan.Interop

[<AbstractClass>]
type FalBuffer =

    internal new: unit -> FalBuffer

    abstract internal Buffer: VkBuffer
    abstract internal Memory: VkDeviceMemory

    /// Shared memory have fast write times from CPU but slower read times from GPU.
    /// Non-shared (local) memory have slow write times from CPU but fast read times from GPU.
    abstract IsShared: bool

[<Sealed;Class>]
type VertexBuffer<'T when 'T : unmanaged> =
    inherit FalBuffer

    member Fill: data: ReadOnlySpan<'T> -> unit

    static member internal Create : VkPhysicalDevice * VkDevice * VkCommandPool * transferQueue: VkQueue * count: int * usage: VkBufferUsageFlags -> VertexBuffer<'T>