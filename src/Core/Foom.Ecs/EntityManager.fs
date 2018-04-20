namespace Foom.EntityManager

open System
open System.Diagnostics
open System.Collections.Generic
open System.Runtime.InteropServices
open FSharp.NativeInterop
open Foom.Core

#nowarn "9"

[<AllowNullLiteral>]
type IEntityLookupData =

    abstract Entities : Entity UnmanagedResizeArray with get

    abstract GetIndex : int -> int

    abstract TryRemoveComponent : Entity -> bool

[<ReferenceEquality>]
type EntityLookupData<'T when 'T : unmanaged and 'T :> IComponent> =
    {
        IndexLookup: int UnmanagedArray
        Entities: Entity UnmanagedResizeArray
        Components: 'T UnmanagedResizeArray

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
type TryGetDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of byref<'T> -> unit

[<Struct>]
[<StructLayout(LayoutKind.Sequential, Size = 510)>]
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
            NativePtrExtension.toByref (NativePtr.add ptr 0)
        else
            let ptr = &&this.values.fixedValues |> NativePtr.toNativeInt |> NativePtr.ofNativeInt<uint16>
            NativePtrExtension.toByref (NativePtr.add ptr i)

    member this.AddComponentId(compId: uint16) =
        let v = this.Get(this.count)
        v <- compId
        this.count <- this.count + 1

    member this.RemoveComponentId(compId: uint16) =
        if this.count > 0 then
            // TODO: We could probably optimize this by providing more book keeping.
            for i = 0 to this.count - 1 do
                let v = this.Get(i)
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
    let lookupType = Array.zeroCreate 256
    let activeVersions = UnmanagedArray<uint32>.Create(maxEntityAmount, fun _ -> 1u) //Array.init maxEntityAmount (fun _ -> 1u)
    let entComps = UnmanagedArray<EntityComponents>.Create(maxEntityAmount, fun _ -> { count = 0 })

    let mutable nextEntityIndex = 0
    let mutable nextCompBit = 0
    let removedEntityQueue = Queue<Entity> ()

    let mutable currentIterations = 0

    member inline __.IsValidEntity(entity: Entity) =
        let ref = activeVersions.[entity.Index]
        ref.Equals entity.Version

    member __.RegisterComponent<'T when 'T : unmanaged and 'T :> IComponent>() =
        let data =
            {
                IndexLookup = UnmanagedArray<int>.Create(maxEntityAmount, fun _ -> -1) // -1 means that no component exists for that entity
                Entities = new UnmanagedResizeArray<Entity>(1)
                Components = new UnmanagedResizeArray<'T>(1)

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

            let count = data.Entities.Count
            let entities = data.Entities.ToSpan()
            let components = data.Components.ToSpan()

            for i = 0 to count - 1 do
                let ent = entities.[i]
                let comp = components.[i]
                f.Invoke(ent, &comp)

    member inline this.Iterate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> (f: ForEachDelegate<'T1, 'T2>) : unit =
        let mutable bit1 = 0
        let mutable bit2 = 0
        if lookup.TryGetValue (typeof<'T1>, &bit1) && lookup.TryGetValue (typeof<'T2>, &bit2) then
            let data1 = lookupType.[bit1]
            let data2 = lookupType.[bit2]
            let data = [|data1;data2|] |> Array.minBy (fun x -> x.Entities.Count)
            let data1 = data1 :?> EntityLookupData<'T1>
            let data2 = data2 :?> EntityLookupData<'T2>

            let count = data.Entities.Count
            let entities = data.Entities.ToSpan()
            let components1 = data1.Components.ToSpan()
            let components2 = data2.Components.ToSpan()
            let lookup1 = data1.IndexLookup
            let lookup2 = data2.IndexLookup
    
            for i = 0 to count - 1 do
                let ent = entities.[i]
    
                let comp1Index = lookup1.[ent.Index]
                let comp2Index = lookup2.[ent.Index]

                if comp1Index >= 0 && comp2Index >= 0 then
                    let comp1 = components1.[comp1Index]
                    let comp2 = components2.[comp2Index]
                    f.Invoke(ent, &comp1, &comp2)

    //member inline this.Iterate<'T1, 'T2, 'T3 when 'T1 :> Component and 'T2 :> Component and 'T3 :> Component> (f) : unit =
    //    let mutable data1 = Unchecked.defaultof<IEntityLookupData>
    //    let mutable data2 = Unchecked.defaultof<IEntityLookupData>
    //    let mutable data3 = Unchecked.defaultof<IEntityLookupData>
    //    if this.Lookup.TryGetValue (typeof<'T1>, &data1) && this.Lookup.TryGetValue (typeof<'T2>, &data2) && 
    //       this.Lookup.TryGetValue (typeof<'T3>, &data3) then
    //        let data = [|data1;data2;data3|] |> Array.minBy (fun x -> x.Entities.Count)
    //        let data1 = data1 :?> EntityLookupData<'T1>
    //        let data2 = data2 :?> EntityLookupData<'T2>
    //        let data3 = data3 :?> EntityLookupData<'T3>

    //        let entities = data.Entities.Buffer
    //        let components1 = data1.Components.Buffer
    //        let components2 = data2.Components.Buffer
    //        let components3 = data3.Components.Buffer
    //        let lookup1 = data1.IndexLookup
    //        let lookup2 = data2.IndexLookup
    //        let lookup3 = data3.IndexLookup

    //        let inline iter i =
    //            let entity = entities.[i]
    
    //            let comp1Index = lookup1.[entity.Index]
    //            let comp2Index = lookup2.[entity.Index]
    //            let comp3Index = lookup3.[entity.Index]

    //            if comp1Index >= 0 && comp2Index >= 0 && comp3Index >= 0 then
    //                f entity components1.[comp1Index] components2.[comp2Index] components3.[comp3Index]
    
    //        for i = 0 to data.Entities.Count - 1 do iter i

    //member inline this.Iterate<'T1, 'T2, 'T3, 'T4 when 'T1 :> Component and 'T2 :> Component and 'T3 :> Component and 'T4 :> Component> (f) : unit =
    //    let mutable data1 = Unchecked.defaultof<IEntityLookupData>
    //    let mutable data2 = Unchecked.defaultof<IEntityLookupData>
    //    let mutable data3 = Unchecked.defaultof<IEntityLookupData>
    //    let mutable data4 = Unchecked.defaultof<IEntityLookupData>
    //    if this.Lookup.TryGetValue (typeof<'T1>, &data1) && this.Lookup.TryGetValue (typeof<'T2>, &data2) && 
    //       this.Lookup.TryGetValue (typeof<'T3>, &data3) && this.Lookup.TryGetValue (typeof<'T4>, &data4) then
    //        let data = [|data1;data2;data3;data4|] |> Array.minBy (fun x -> x.Entities.Count)
    //        let data1 = data1 :?> EntityLookupData<'T1>
    //        let data2 = data2 :?> EntityLookupData<'T2>
    //        let data3 = data3 :?> EntityLookupData<'T3>
    //        let data4 = data4 :?> EntityLookupData<'T4>

    //        let entities = data.Entities.Buffer
    //        let components1 = data1.Components.Buffer
    //        let components2 = data2.Components.Buffer
    //        let components3 = data3.Components.Buffer
    //        let components4 = data4.Components.Buffer
    //        let lookup1 = data1.IndexLookup
    //        let lookup2 = data2.IndexLookup
    //        let lookup3 = data3.IndexLookup
    //        let lookup4 = data4.IndexLookup

    //        let inline iter i =
    //            let entity = entities.[i]
    
    //            let comp1Index = lookup1.[entity.Index]
    //            let comp2Index = lookup2.[entity.Index]
    //            let comp3Index = lookup3.[entity.Index]
    //            let comp4Index = lookup4.[entity.Index]

    //            if comp1Index >= 0 && comp2Index >= 0 && comp3Index >= 0 && comp4Index >= 0 then
    //                f entity components1.[comp1Index] components2.[comp2Index] components3.[comp3Index] components4.[comp4Index]
    
    //        for i = 0 to data.Entities.Count - 1 do iter i
            //Parallel.For (0, data.Entities.Count - 1, fun i _ -> iter i) |> ignore

    // Components

    member this.Add<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity) =
        let struct(bit, data) = this.GetEntityLookupData<'T>()
        if currentIterations > 0 then
            failwith "Can't add while iterating"
            &data.dummy
        else
            if this.IsValidEntity entity then
                let indexLookup = data.IndexLookup
                let entities = data.Entities
                let components = data.Components

                let index = indexLookup.[entity.Index]
                if index >= 0 then
                    Debug.WriteLine (String.Format ("ECS WARNING: Component, {0}, already added to {1}.", typeof<'T>.Name, entity))
                    components.[index]
                else
                    let mutable index = entities.Count

                    indexLookup.[entity.Index] <- index

                    components.AddDefault()
                    entities.Add(entity)

                    let ec = entComps.[entity.Index]
                    ec.AddComponentId(uint16 bit)

                    components.[index]
            else
                &data.dummy

    member this.Remove<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity) =
        let struct(bit, data) = this.GetEntityLookupData<'T> ()
        if currentIterations > 0 then
            failwith "Can't remove while iterating."
        else
            if this.IsValidEntity entity then
                if (data :> IEntityLookupData).TryRemoveComponent(entity) then
                    let ec = entComps.[entity.Index]
                    ec.RemoveComponentId(uint16 bit)
                else
                    Debug.WriteLine (String.Format ("ECS WARNING: Component, {0}, does not exist on {1}", typeof<'T>.Name, entity))
            else
                Debug.WriteLine (String.Format ("ECS WARNING: {0} is invalid. Cannot remove component, {1}", entity, typeof<'T>.Name))

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

    member this.Destroy(ent: Entity) =
        if currentIterations > 0 then
            failwith "Can't destroy while iterating."
        else
            if this.IsValidEntity ent then

                let ec = entComps.[ent.Index]

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

    member this.TryGetComponent<'T when 'T : unmanaged and 'T :> IComponent>(ent: Entity, tryGetF: TryGetDelegate<'T>) =
        let struct(bit, data) = this.GetEntityLookupData<'T>()

        let index = data.IndexLookup.[ent.Index]
        if index <> -1 then
            let compRef = data.Components.[index]
            tryGetF.Invoke(&compRef)

    //member this.ForEach<'T1, 'T2, 'T3 when 'T1 :> Component and 'T2 :> Component and 'T3 :> Component> f : unit =
    //    this.CurrentIterations <- this.CurrentIterations + 1

    //    this.Iterate<'T1, 'T2, 'T3> (f)

    //    this.CurrentIterations <- this.CurrentIterations - 1
    //    this.ResolvePendingQueues ()

    //member this.ForEach<'T1, 'T2, 'T3, 'T4 when 'T1 :> Component and 'T2 :> Component and 'T3 :> Component and 'T4 :> Component> f : unit =
    //    this.CurrentIterations <- this.CurrentIterations + 1

    //    this.Iterate<'T1, 'T2, 'T3, 'T4> (f)

    //    this.CurrentIterations <- this.CurrentIterations - 1
    //    this.ResolvePendingQueues ()

    member this.MaxNumberOfEntities = maxEntityAmount - 1

    //member this.DestroyAll () =
    //    activeVersions
    //    |> Array.iteri (fun index version ->
    //        if version > 0u then
    //            this.Destroy (Entity (index, version))
    //    )
