﻿module FSharp.Spirv.Quotations.Intrinsics

open System
open System.Numerics
open System.Collections.Generic
open FSharp.NativeInterop
open FSharp.Quotations
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns
open FSharp.Spirv

[<Sealed;AttributeUsage(AttributeTargets.Struct)>]
type BlockAttribute() =
    inherit Attribute()

[<Literal>]
let private ErrorMessage = "Do not call outside the code quotation."

[<RequiresExplicitTypeArguments>]
let Variable<'T> (_decorations: Decoration list) (_storageClass: StorageClass) (_customAnnoations: obj list) : 'T = failwith ErrorMessage

[<Sealed>]
type Sampler = class end

[<AbstractClass>]
type DimKind = class end

[<RequireQualifiedAccess>]
module DimKind =

    [<Sealed;AbstractClass>]
    type One =
        inherit DimKind

    [<Sealed;AbstractClass>]
    type Two =
        inherit DimKind

    [<Sealed;AbstractClass>]
    type Three =
        inherit DimKind

    [<Sealed;AbstractClass>]
    type Cube =
        inherit DimKind

    [<Sealed;AbstractClass>]
    type Rect =
        inherit DimKind

    [<Sealed;AbstractClass>]
    type Buffer =
        inherit DimKind

    [<Sealed;AbstractClass>]
    type SubpassData =
        inherit DimKind

[<AbstractClass>]
type ImageDepthKind = class end

[<RequireQualifiedAccess>]
module ImageDepthKind =

    /// Indicates not a depth image, 0
    [<Sealed;AbstractClass>]
    type NoDepth =
        inherit ImageDepthKind

    /// Indicates a depth image, 1
    [<Sealed;AbstractClass>]
    type Depth =
        inherit ImageDepthKind

    /// Means no indication as to whether this is a depth or non-depth image, 2
    [<Sealed;AbstractClass>]
    type Unknown =
        inherit ImageDepthKind

[<AbstractClass>]
type ImageArrayedKind = class end

[<RequireQualifiedAccess>]
module ImageArrayedKind =

    /// Indicates non-arrayed content, 0
    [<Sealed;AbstractClass>]
    type NonArrayed =
        inherit ImageArrayedKind

    /// Indicates arrayed content, 1
    [<Sealed;AbstractClass>]
    type Arrayed =
        inherit ImageArrayedKind

[<AbstractClass>]
type ImageMultisampleKind = class end

[<RequireQualifiedAccess>]
module ImageMultisampleKind =

    /// Indicates single-sampled content, 0
    [<Sealed;AbstractClass>]
    type Single =
        inherit ImageMultisampleKind

    /// Indicates multisampled content, 1
    [<Sealed;AbstractClass>]
    type Multi =
        inherit ImageMultisampleKind

[<AbstractClass>]
type ImageSampleKind = class end

[<RequireQualifiedAccess>]
module ImageSampleKind =

    /// Indicates this is only known at run time, not at compile time, 0
    [<Sealed;AbstractClass>]
    type RuntimeOnly =
        inherit ImageSampleKind

    /// Indicates will be used with sampler, 1
    [<Sealed;AbstractClass>]
    type Sampler =
        inherit ImageSampleKind

    /// Indicates will be used without a sampler (a storage image), 2
    [<Sealed;AbstractClass>]
    type NoSampler =
        inherit ImageSampleKind

[<AbstractClass>]
type ImageFormatKind = class end

[<RequireQualifiedAccess>]
module ImageFormatKind =

    [<Sealed;AbstractClass>]
    type Unknown =
        inherit ImageFormatKind

    [<Sealed;AbstractClass>]
    type Rgba32f =
        inherit ImageFormatKind

    [<Sealed;AbstractClass>]
    type Rgba16f =
        inherit ImageFormatKind

    [<Sealed;AbstractClass>]
    type R32f =
        inherit ImageFormatKind

    [<Sealed;AbstractClass>]
    type Rgba8 =
        inherit ImageFormatKind

    [<Sealed;AbstractClass>]
    type Rgba8Snorm =
        inherit ImageFormatKind

    // TODO: Add more formats

[<AbstractClass>]
type AccessQualifierKind = class end

[<RequireQualifiedAccess>]
module AccessQualifierKind =

    [<Sealed;AbstractClass>]
    type None =
        inherit AccessQualifierKind

    [<Sealed;AbstractClass>]
    type ReadOnly =
        inherit AccessQualifierKind

    [<Sealed;AbstractClass>]
    type WriteOnly =
        inherit AccessQualifierKind

    [<Sealed;AbstractClass>]
    type ReadWrite =
        inherit AccessQualifierKind

[<Sealed>]
type Image<'SampledType, 'Dim, 'Depth, 'Arrayed, 'Multisampled, 'Sampled, 'Format, 'AccessQualifier
    when 'Dim :> DimKind
    and  'Depth :> ImageDepthKind
    and  'Arrayed :> ImageArrayedKind
    and  'Multisampled :> ImageMultisampleKind
    and  'Sampled :> ImageSampleKind
    and  'Format :> ImageFormatKind
    and  'AccessQualifier :> AccessQualifierKind
    > = class end

[<Sealed>]
type SampledImage<'SampledType, 'Dim, 'Depth, 'Arrayed, 'Multisampled, 'Sampled, 'Format, 'AccessQualifier
    when 'Dim :> DimKind
    and  'Depth :> ImageDepthKind
    and  'Arrayed :> ImageArrayedKind
    and  'Multisampled :> ImageMultisampleKind
    and  'Sampled :> ImageSampleKind
    and  'Format :> ImageFormatKind
    and  'AccessQualifier :> AccessQualifierKind
    > = class end

/// Only valid in the Fragment Execution Model.
/// Analogous to 'OpKill'.
let kill () : unit = failwith ErrorMessage

/// Analogous to 'OpImageSampleImplicitLod'.
let imageSampleImplicitLod (_coordinate: Vector2) (_sampledImage: SampledImage<_, _, _, _, _, _, _, _>) : Vector4 = failwith ErrorMessage

type SampledImage<'SampledType, 'Dim, 'Depth, 'Arrayed, 'Multisampled, 'Sampled, 'Format, 'AccessQualifier
    when 'Dim :> DimKind
    and  'Depth :> ImageDepthKind
    and  'Arrayed :> ImageArrayedKind
    and  'Multisampled :> ImageMultisampleKind
    and  'Sampled :> ImageSampleKind
    and  'Format :> ImageFormatKind
    and  'AccessQualifier :> AccessQualifierKind
    > with

    [<ReflectedDefinition>]
    member this.ImplicitLod coordinate =
        imageSampleImplicitLod coordinate this
