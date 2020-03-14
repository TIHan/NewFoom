[<AutoOpen>]
module FsGame.Graphics.Vulkan.VulkanDevice

open System
open System.Threading
open System.Runtime.InteropServices
open FSharp.Vulkan.Interop
open InternalDeviceHelpers

type VulkanDeviceLayer =
    | LunarGStandardValidation

    override this.ToString() =
        match this with
        | LunarGStandardValidation -> "VK_LAYER_LUNARG_standard_validation"

type VulkanDeviceExtension =
    | ShaderFloat16Int8

    override this.ToString() =
        match this with
        | ShaderFloat16Int8 -> VK_KHR_SHADER_FLOAT16_INT8_EXTENSION_NAME

[<Sealed>]
type VulkanDevice private 
    (
        instance: VkInstance,
        surfaceOpt: VkSurfaceKHR option,
        debugMessenger: VkDebugUtilsMessengerEXT,
        physicalDevice: VkPhysicalDevice,
        indices: QueueFamilyIndices,
        device: VkDevice,
        vkCommandPool: VkCommandPool,
        vkTransferQueue: VkQueue,
        handles: GCHandle []
    ) =

    let mutable isDisposed = 0

    let checkDispose () =
        if isDisposed <> 0 then
            failwith "FalDevice is disposed."

    member _.PhysicalDevice =
        checkDispose ()
        physicalDevice

    member _.Device =
        checkDispose ()
        device

    member _.Indices =
        checkDispose ()
        indices

    member _.Surface =
        checkDispose ()
        surfaceOpt

    member _.VkCommandPool =
        checkDispose ()
        vkCommandPool

    member _.VkTransferQueue =
        checkDispose ()
        vkTransferQueue

    interface IDisposable with
        member x.Dispose () =
            if Interlocked.CompareExchange(&isDisposed, 1, 0) = 1 then
                checkDispose ()
            else
                GC.SuppressFinalize x

                vkDestroyCommandPool(device, vkCommandPool, vkNullPtr)
                vkDestroyDevice(device, vkNullPtr)

                if debugMessenger <> IntPtr.Zero then
                    let destroyDebugUtilsMessenger = getInstanceExtension<vkDestroyDebugUtilsMessengerEXT> instance
                    destroyDebugUtilsMessenger.Invoke(instance, debugMessenger, vkNullPtr)

                surfaceOpt |> Option.iter (fun surface -> vkDestroySurfaceKHR(instance, surface, vkNullPtr))
                vkDestroyInstance(instance, vkNullPtr)

                handles
                |> Array.iter (fun x -> x.Free())

    static member Create (appName, engineName, deviceLayers: VulkanDeviceLayer list, deviceExtensions: VulkanDeviceExtension list, ?createVkSurface) =
        let deviceLayers = deviceLayers |> List.map (fun x -> x.ToString()) |> Array.ofList
        let deviceExtensions = (VK_KHR_SWAPCHAIN_EXTENSION_NAME :: (deviceExtensions |> List.map (fun x -> x.ToString()))) |> Array.ofList
        let debugCallbackHandle, debugCallback =
            createVkDebugUtilsMessengerCallback (fun str -> printfn "validation layer: %s" str)

        let instance = mkInstance appName engineName deviceLayers
        // must create surface right after instance - influences device calls
        let surfaceOpt = match createVkSurface with Some mkSurface -> Some(mkSurface instance) | _ -> None
        let debugMessenger = mkDebugMessenger instance debugCallback
        let physicalDevice = getSuitablePhysicalDevice instance
        let indices = getPhysicalDeviceQueueFamilies physicalDevice surfaceOpt
        let device = mkLogicalDevice physicalDevice indices deviceLayers deviceExtensions

        let _transferFamily =
            match indices.transferFamily with
            | Some transferQueueFamily -> transferQueueFamily
            | _ -> failwith "Unable to create FalkanGraphicsDevice: Transfer queue not available on physical device."

        let graphicsFamily =
            match indices.graphicsFamily with
            | Some graphicsQueueFamily -> graphicsQueueFamily
            | _ -> failwith "Unable to create FalkanGraphicsDevice: Graphics queue not available on physical device."

        let commandPool = mkCommandPool device graphicsFamily
        // TODO: We should try to use a transfer queue instead of a graphics queue. This works for now.
        let transferQueue = mkQueue device graphicsFamily

        new VulkanDevice (instance, surfaceOpt, debugMessenger, physicalDevice, indices, device, commandPool, transferQueue, [|debugCallbackHandle|])

    static member CreateWin32 (hwnd, hinstance, appName, engineName, deviceLayers, deviceExtensions) =
        VulkanDevice.Create (appName, engineName, deviceLayers, deviceExtensions, createVkSurface = createWin32Surface hwnd hinstance)