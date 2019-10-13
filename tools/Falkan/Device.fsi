[<AutoOpen>]
module Falkan.Device

open System
open FSharp.Vulkan.Interop

[<Sealed>]
type FalDevice =

    interface IDisposable

    static member Create: appName: string * engineName: string * validationLayers: string list * deviceExtensions: string list * ?mkSurface: (VkInstance -> VkSurfaceKHR) -> FalDevice