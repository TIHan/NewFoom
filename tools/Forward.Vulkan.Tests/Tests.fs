module Tests

open System
open Xunit
open Xunit.Sdk
open FsGame.Graphics.Vulkan

let createDevice () =
    VulkanDevice.CreateCompute("Vulkan Tests", "forward", [VulkanDeviceLayer.LunarGStandardValidation], [])

let createCompute device =
    FalGraphics.CreateCompute(device, Event<unit>().Publish)

[<Fact>]
let ``My test`` () =
    use device = createDevice ()
    use compute = createCompute device
    Assert.True(true)
