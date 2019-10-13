[<AutoOpen>]
module Falkan.SwapChain

open System
open FSharp.Vulkan.Interop

[<Sealed>]
type FalSwapChain =
    interface IDisposable
