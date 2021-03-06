﻿namespace Foom.EntityManager

open System
open System.Diagnostics
open System.Collections.Generic
open System.Runtime.InteropServices
open System.Runtime.CompilerServices
open FSharp.NativeInterop
open Foom.NativeCollections

#nowarn "9"

type EntityArchetype<'T1, 'T2, 'T3, 'T4, 'T5> = 
    {
        bit1: byte
        bit2: byte
        bit3: byte
        bit4: byte
        bit5: byte
    }

[<AllowNullLiteral>]
type IEntityLookupData =

    abstract Entities : Entity NativeResizeArray with get

    abstract GetIndex : int -> int

    abstract TryRemoveComponent : Entity -> bool

[<ReferenceEquality>]
type EntityLookupData<'T when 'T : unmanaged and 'T :> IComponent> =
    {
        IndexLookup: int NativeArray
        Entities: Entity NativeResizeArray
        Components: 'T NativeResizeArray

        mutable dummy: 'T
    }

    interface IEntityLookupData with

        member this.Entities = this.Entities

        member this.GetIndex id = 
            let res = this.IndexLookup.[id]
            res

        member this.TryRemoveComponent(ent) =
            let indexLookup = this.IndexLookup
            let entities = this.Entities
            let components = this.Components

            let index = indexLookup.[ent.Index]
            if index >= 0 then
                let swappingEntity = entities.LastItem

                entities.SwapRemoveAt index
                components.SwapRemoveAt index

                if not (ent.Index.Equals swappingEntity.Index) then
                    indexLookup.[swappingEntity.Index] <- index

                indexLookup.[ent.Index] <- -1

                true
            else
                false
    
type ForEachDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of Entity * byref<'T> -> unit
type ForEachDelegate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> = delegate of Entity * byref<'T1> * byref<'T2> -> unit
type ForEachDelegate<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> = delegate of Entity * byref<'T1> * byref<'T2> * byref<'T3> * byref<'T4> * byref<'T5> -> unit
type TryGetDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of byref<'T> -> unit

[<Struct>]
[<StructLayout(LayoutKind.Sequential, Size = 508)>]
type EntityComponentsInternal =

    val mutable fixedValues : uint16

[<Struct>]
type EntityComponents =
    {
        [<DefaultValue>] mutable values : EntityComponentsInternal

        mutable count: int
    }

    member inline this.Get(i) =
        if i >= 252 then
            failwith "Invalid index to find component."
            let ptr = &&this.values.fixedValues |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<uint16>
            NativePtr.toByRef (NativePtr.add ptr 0)
        else
            let ptr = &&this.values.fixedValues |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<uint16>
            NativePtr.toByRef (NativePtr.add ptr i)

    member this.AddComponentId(compId: uint16) =
        let v = &this.Get(this.count)
        v <- compId
        this.count <- this.count + 1

    member this.RemoveComponentId(compId: uint16) =
        if this.count > 0 then
            // TODO: We could probably optimize this by providing more book keeping.
            for i = 0 to this.count - 1 do
                let v = &this.Get(i)
                if v = compId then
                    let lastV = this.Get(this.count - 1)
                    v <- lastV
            this.count <- this.count - 1

and [<Sealed>] EntityManager(maxEntityAmount) =
    let maxEntityAmount =
        if maxEntityAmount <= 0 then
            failwith "Max entity amount must be greater than 0."
        maxEntityAmount

    let lookup = Dictionary<Type, int> ()
    let lookupType = Array.zeroCreate 252
    let activeVersions = new NativeArray<uint32>(maxEntityAmount)
    let entComps = new NativeArray<EntityComponents>(maxEntityAmount)

    let mutable nextEntityIndex = 0
    let mutable nextCompBit = 0
    let removedEntityQueue = Queue<Entity> ()

    let mutable currentIterations = 0

    do
        for i = 0 to activeVersions.Length - 1 do
            activeVersions.[i] <- 1u

    member inline __.IsValidEntity(entity: Entity) =
        let ref = activeVersions.[entity.Index]
        ref.Equals entity.Version

    member __.RegisterComponent<'T when 'T : unmanaged and 'T :> IComponent>() =
        let indexLookup = new NativeArray<int>(maxEntityAmount)
        for i = 0 to indexLookup.Length - 1 do
            indexLookup.[i] <- -1 // -1 means that no component exists for that entity
        let data =
            {
                IndexLookup = indexLookup
                Entities = new NativeResizeArray<Entity>(1)
                Components = new NativeResizeArray<'T>(1)

                dummy = Unchecked.defaultof<'T>
            }

        let compId = nextCompBit
        lookup.Add(typeof<'T>, compId) |> ignore
        lookupType.[compId] <- data :> IEntityLookupData
        nextCompBit <- nextCompBit + 1

    member __.GetComponentId<'T when 'T : unmanaged and 'T :> IComponent>() =
        lookup.[typeof<'T>]

    member this.GetEntityLookupData<'T when 'T : unmanaged and 'T :> IComponent> () =
        let t = typeof<'T>
        let mutable bit = 0
        match lookup.TryGetValue(t, &bit) with
        | true -> struct(bit, lookupType.[bit] :?> EntityLookupData<'T>)
        | _ -> failwithf "Component, %s, not registered." typeof<'T>.Name

    member this.Iterate<'T when 'T : unmanaged and 'T :> IComponent> (f: ForEachDelegate<'T>) : unit =
        let mutable bit = 0
        if lookup.TryGetValue (typeof<'T>, &bit) then
            let data = lookupType.[bit] :?> EntityLookupData<'T>

            let entities = data.Entities
            let count = entities.Count
            let components = data.Components

            for i = 0 to count - 1 do
                f.Invoke(entities.[i], &components.[i])

    member inline this.Iterate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> (f: ForEachDelegate<'T1, 'T2>) : unit =
        let mutable bit1 = 0
        let mutable bit2 = 0
        if lookup.TryGetValue (typeof<'T1>, &bit1) && lookup.TryGetValue (typeof<'T2>, &bit2) then
            let data1 = lookupType.[bit1]
            let data2 = lookupType.[bit2]
            let data = [|data1;data2|] |> Array.minBy (fun x -> x.Entities.Count)
            let data1 = data1 :?> EntityLookupData<'T1>
            let data2 = data2 :?> EntityLookupData<'T2>

            let entities = data.Entities
            let count = entities.Count
            let components1 = data1.Components
            let components2 = data2.Components
            let lookup1 = data1.IndexLookup
            let lookup2 = data2.IndexLookup
    
            for i = 0 to count - 1 do
                let ent = &entities.[i]
                let index = ent.Index
                let comp1Index = lookup1.[index]
                let comp2Index = lookup2.[index]

                if comp1Index >= 0 && comp2Index >= 0 then
                    f.Invoke(ent, &components1.[comp1Index], &components2.[comp2Index])

    member inline this.Iterate<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> (f: ForEachDelegate<'T1, 'T2, 'T3, 'T4, 'T5>) : unit =
        let mutable bit1 = 0
        let mutable bit2 = 0
        let mutable bit3 = 0
        let mutable bit4 = 0
        let mutable bit5 = 0
        if lookup.TryGetValue (typeof<'T1>, &bit1) && lookup.TryGetValue (typeof<'T2>, &bit2) && lookup.TryGetValue (typeof<'T3>, &bit3) && lookup.TryGetValue (typeof<'T4>, &bit4) && lookup.TryGetValue (typeof<'T5>, &bit5) then
            let data1 = lookupType.[bit1]
            let data2 = lookupType.[bit2]
            let data3 = lookupType.[bit3]
            let data4 = lookupType.[bit4]
            let data5 = lookupType.[bit5]
            let data = [|data1;data2;data3;data4;data5|] |> Array.minBy (fun x -> x.Entities.Count)
            let data1 = data1 :?> EntityLookupData<'T1>
            let data2 = data2 :?> EntityLookupData<'T2>
            let data3 = data3 :?> EntityLookupData<'T3>
            let data4 = data4 :?> EntityLookupData<'T4>
            let data5 = data5 :?> EntityLookupData<'T5>

            let entities = data.Entities
            let count = entities.Count
            let components1 = data1.Components
            let components2 = data2.Components
            let components3 = data3.Components
            let components4 = data4.Components
            let components5 = data5.Components
            let lookup1 = data1.IndexLookup
            let lookup2 = data2.IndexLookup
            let lookup3 = data3.IndexLookup
            let lookup4 = data4.IndexLookup
            let lookup5 = data5.IndexLookup
    
            for i = 0 to count - 1 do
                let ent = &entities.[i]
                let index = ent.Index
                let comp1Index = lookup1.[index]
                let comp2Index = lookup2.[index]
                let comp3Index = lookup3.[index]
                let comp4Index = lookup4.[index]
                let comp5Index = lookup5.[index]

                if comp1Index >= 0 && comp2Index >= 0 && comp3Index >= 0 && comp4Index >= 0 && comp5Index >= 0 then
                    f.Invoke(ent, &components1.[comp1Index], &components2.[comp2Index], &components3.[comp3Index], &components4.[comp4Index], &components5.[comp5Index])

    // Components

    member inline this.AddDataInline(entity: Entity, comp: byref<'T>, bit, data: EntityLookupData<'T>) =
        let index = data.Entities.Count

        data.IndexLookup.[entity.Index] <- index

        data.Components.Add(&comp)
        data.Entities.Add(entity)

        let ec = &entComps.[entity.Index]
        ec.AddComponentId(uint16 bit)

    member inline this.AddInline<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity, comp: byref<'T>) =
        let struct(bit, data) = this.GetEntityLookupData<'T>()
        if currentIterations > 0 then
            failwith "Can't add while iterating"
        else
            if this.IsValidEntity entity then
                let indexLookup = data.IndexLookup
                let entities = data.Entities
                let components = data.Components

                let index = indexLookup.[entity.Index]
                if index >= 0 then
                    Console.WriteLine (String.Format ("ECS WARNING: Component, {0}, already added to {1}.", typeof<'T>.Name, entity))
                else
                    let mutable index = entities.Count

                    indexLookup.[entity.Index] <- index

                    components.Add(&comp)
                    entities.Add(entity)

                    let ec = &entComps.[entity.Index]
                    ec.AddComponentId(uint16 bit)

    member this.Add<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity, comp: byref<'T>) =
        this.AddInline(entity, &comp)

    member this.Add<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity, comp: 'T) =
        let mutable comp = comp
        this.AddInline(entity, &comp)

    member this.Remove<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity) =
        let struct(bit, data) = this.GetEntityLookupData<'T> ()
        if currentIterations > 0 then
            failwith "Can't remove while iterating."
        else
            if this.IsValidEntity entity then
                if (data :> IEntityLookupData).TryRemoveComponent(entity) then
                    let ec = &entComps.[entity.Index]
                    ec.RemoveComponentId(uint16 bit)
                else
                    Console.WriteLine (String.Format ("ECS WARNING: Component, {0}, does not exist on {1}", typeof<'T>.Name, entity))
            else
                Console.WriteLine (String.Format ("ECS WARNING: {0} is invalid. Cannot remove component, {1}", entity, typeof<'T>.Name))

    member this.Spawn () =                         
        if removedEntityQueue.Count = 0 && nextEntityIndex >= maxEntityAmount then
            Debug.WriteLine (String.Format ("ECS WARNING: Unable to spawn entity. Max entity amount hit: {0}", (maxEntityAmount)))
            Entity()
        else
            let entity =
                if removedEntityQueue.Count > 0 then
                    let entity = removedEntityQueue.Dequeue ()
                    Entity (entity.Index, entity.Version + 1u)
                else
                    let index = nextEntityIndex
                    nextEntityIndex <- index + 1
                    Entity (index, 1u)

            activeVersions.[entity.Index] <- entity.Version

            entity

    member this.CreateArchetype<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent>() : EntityArchetype<'T1, 'T2, 'T3, 'T4, 'T5> =
        let mutable bit1 = 0
        let mutable bit2 = 0
        let mutable bit3 = 0
        let mutable bit4 = 0
        let mutable bit5 = 0
        if lookup.TryGetValue (typeof<'T1>, &bit1) && lookup.TryGetValue (typeof<'T2>, &bit2) && lookup.TryGetValue (typeof<'T3>, &bit3) && lookup.TryGetValue (typeof<'T4>, &bit4) && lookup.TryGetValue (typeof<'T5>, &bit5) then
            { bit1 = byte bit1; bit2 = byte bit2; bit3 = byte bit3; bit4 = byte bit4; bit5 = byte bit5 }
        else
            failwith "Can't find one or more components for archetype."

    member this.SpawnArchetype<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent>(archetype: EntityArchetype<'T1, 'T2, 'T3, 'T4, 'T5>, comp1: 'T1, comp2: 'T2, comp3: 'T3, comp4: 'T4, comp5: 'T5) =
        if removedEntityQueue.Count = 0 && nextEntityIndex >= maxEntityAmount then
            Debug.WriteLine (String.Format ("ECS WARNING: Unable to spawn entity. Max entity amount hit: {0}", (maxEntityAmount)))
            Entity()
        else
            let entity =
                if removedEntityQueue.Count > 0 then
                    let entity = removedEntityQueue.Dequeue ()
                    Entity (entity.Index, entity.Version + 1u)
                else
                    let index = nextEntityIndex
                    nextEntityIndex <- index + 1
                    Entity (index, 1u)

            activeVersions.[entity.Index] <- entity.Version

            let bit1 = int archetype.bit1
            let bit2 = int archetype.bit2
            let bit3 = int archetype.bit3
            let bit4 = int archetype.bit4
            let bit5 = int archetype.bit5

            let data1 = lookupType.[bit1] :?> EntityLookupData<'T1>
            let data2 = lookupType.[bit2] :?> EntityLookupData<'T2>
            let data3 = lookupType.[bit3] :?> EntityLookupData<'T3>
            let data4 = lookupType.[bit4] :?> EntityLookupData<'T4>
            let data5 = lookupType.[bit5] :?> EntityLookupData<'T5>

            let mutable comp1 = comp1
            let mutable comp2 = comp2
            let mutable comp3 = comp3
            let mutable comp4 = comp4
            let mutable comp5 = comp5
            this.AddDataInline(entity, &comp1, bit1, data1)
            this.AddDataInline(entity, &comp2, bit2, data2)
            this.AddDataInline(entity, &comp3, bit3, data3)
            this.AddDataInline(entity, &comp4, bit4, data4)
            this.AddDataInline(entity, &comp5, bit5, data5)

            entity

    member this.Destroy(ent: Entity) =
        if currentIterations > 0 then
            failwith "Can't destroy while iterating."
        else
            if this.IsValidEntity ent then

                let ec = &entComps.[ent.Index]

                for i = 0 to ec.count - 1 do
                    let v = ec.Get(i)
                    let data = lookupType.[int v]
                    data.TryRemoveComponent(ent) |> ignore
                    ec.RemoveComponentId(v)

                removedEntityQueue.Enqueue ent  
                activeVersions.[ent.Index] <- 0u

            else
                Debug.WriteLine (String.Format ("ECS WARNING: {0} is invalid. Cannot destroy.", ent))

    // Component Query

    //************************************************************************************************************************

    member this.IsValid entity =
        this.IsValidEntity entity

    member this.Has<'T when 'T : unmanaged and 'T :> IComponent> (entity: Entity) =
        let mutable bit = 0
        if lookup.TryGetValue (typeof<'T>, &bit) then
            let data = lookupType.[bit]
            data.GetIndex(entity.Index) >= 0
        else
            false

    //************************************************************************************************************************

    member this.ForEach<'T when 'T : unmanaged and 'T :> IComponent>(f) : unit =
        currentIterations <- currentIterations + 1

        this.Iterate<'T>(f)

        currentIterations <- currentIterations - 1

    member this.ForEach<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> (f) : unit =
        currentIterations <- currentIterations + 1

        this.Iterate<'T1, 'T2>(f)

        currentIterations <- currentIterations - 1

    member this.ForEach<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> (f) : unit =
        currentIterations <- currentIterations + 1

        this.Iterate<'T1, 'T2, 'T3, 'T4, 'T5>(f)

        currentIterations <- currentIterations - 1

    member this.TryGetComponent<'T when 'T : unmanaged and 'T :> IComponent>(ent: Entity, tryGetF: TryGetDelegate<'T>) =
        let struct(bit, data) = this.GetEntityLookupData<'T>()

        let index = data.IndexLookup.[ent.Index]
        if index <> -1 then
            let compRef = &data.Components.[index]
            tryGetF.Invoke(&compRef)

    member this.MaxNumberOfEntities = maxEntityAmount