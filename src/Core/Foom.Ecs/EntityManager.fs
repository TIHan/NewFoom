namespace Foom.EntityManager

open System
open System.Diagnostics
open System.Reflection
open System.Collections.Generic
open System.Collections.Concurrent
open System.Threading.Tasks
open System.Runtime.InteropServices
open System.Runtime.Serialization
open System.Linq
open System.Runtime.InteropServices
open Foom.Core

#nowarn "9"

type IEntityLookupData =

    abstract Entities : Entity UnsafeResizeArray with get

    abstract GetIndex : int -> int

    abstract RemoveComponent : Entity -> unit

[<ReferenceEquality>]
type EntityLookupData<'T when 'T : unmanaged and 'T :> IComponent> =
    {
        RemoveComponent: Entity -> unit

        IndexLookup: int []
        Entities: Entity UnsafeResizeArray
        Components: 'T UnsafeResizeArray
    }

    interface IEntityLookupData with

        member this.Entities = this.Entities

        member this.GetIndex id = this.IndexLookup.[id]

        member this.RemoveComponent ent = this.RemoveComponent ent
    
type ForEachDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of Entity * byref<'T> -> unit
type ForEachDelegate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> = delegate of Entity * byref<'T1> * byref<'T2> -> unit

and [<ReferenceEquality>] EntityManager =
    {
        MaxEntityAmount: int
        Lookup: ConcurrentDictionary<Type, IEntityLookupData>

        ActiveVersions: uint32 []
        EntityComponents : ResizeArray<struct (obj * (obj -> obj) * (Entity -> unit))> []

        mutable nextEntityIndex: int
        RemovedEntityQueue: Queue<Entity>

        mutable CurrentIterations: int
    }

    static member Create(maxEntityAmount) =
        if maxEntityAmount <= 0 then
            failwith "Max entity amount must be greater than 0."

        let maxEntityAmount = maxEntityAmount + 1
        let lookup = ConcurrentDictionary<Type, IEntityLookupData> ()

        let activeVersions = Array.init maxEntityAmount (fun _ -> 0u)

        let mutable nextEntityIndex = 1
        let removedEntityQueue = Queue<Entity> () 

        {
            MaxEntityAmount = maxEntityAmount
            Lookup = lookup
            ActiveVersions = activeVersions
            EntityComponents = Array.init maxEntityAmount (fun _ -> ResizeArray ())
            nextEntityIndex = nextEntityIndex
            RemovedEntityQueue = removedEntityQueue
            CurrentIterations = 0
        }

    member inline this.IsValidEntity (entity: Entity) =
        not (entity.Index.Equals 0u) && this.ActiveVersions.[entity.Index].Equals entity.Version

    member this.GetEntityLookupData<'T when 'T : unmanaged and 'T :> IComponent> () : EntityLookupData<'T> =
        let t = typeof<'T>
        let mutable data = Unchecked.defaultof<IEntityLookupData>
        match this.Lookup.TryGetValue(t, &data) with
        | true -> data :?> EntityLookupData<'T>
        | _ ->
            let data =
                {
                    RemoveComponent = fun entity -> this.Remove<'T>(entity)

                    IndexLookup = Array.init this.MaxEntityAmount (fun _ -> -1) // -1 means that no component exists for that entity
                    Entities = UnsafeResizeArray.Create this.MaxEntityAmount
                    Components = UnsafeResizeArray<'T>.Create(this.MaxEntityAmount)
                }

            this.Lookup.GetOrAdd(t, data :> IEntityLookupData) :?> EntityLookupData<'T>

    member inline this.Iterate<'T when 'T : unmanaged and 'T :> IComponent> (f: ForEachDelegate<'T>) : unit =
        let mutable data = Unchecked.defaultof<IEntityLookupData>
        if this.Lookup.TryGetValue (typeof<'T>, &data) then
            let data = data :?> EntityLookupData<'T>

            let entities = data.Entities.Buffer
            let components = data.Components.Buffer

            let inline iter i =
                f.Invoke(entities.[i], &components.[i])

            for i = 0 to data.Entities.Count - 1 do iter i

    member inline this.Iterate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> (f: ForEachDelegate<'T1, 'T2>) : unit =
        let mutable data1 = Unchecked.defaultof<IEntityLookupData>
        let mutable data2 = Unchecked.defaultof<IEntityLookupData>
        if this.Lookup.TryGetValue (typeof<'T1>, &data1) && this.Lookup.TryGetValue (typeof<'T2>, &data2) then
            let data = [|data1;data2|] |> Array.minBy (fun x -> x.Entities.Count)
            let data1 = data1 :?> EntityLookupData<'T1>
            let data2 = data2 :?> EntityLookupData<'T2>

            let entities = data.Entities.Buffer
            let components1 = data1.Components.Buffer
            let components2 = data2.Components.Buffer
            let lookup1 = data1.IndexLookup
            let lookup2 = data2.IndexLookup

            let inline iter i =
                let entity = entities.[i]
    
                let comp1Index = lookup1.[entity.Index]
                let comp2Index = lookup2.[entity.Index]

                if comp1Index >= 0 && comp2Index >= 0 then
                    f.Invoke(entity, &components1.[comp1Index], &components2.[comp2Index])
    
            for i = 0 to data.Entities.Count - 1 do iter i

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


    member inline this.AddInline2<'T when 'T : unmanaged and 'T :> IComponent> 
            (
                entity: Entity, 
                comp : byref<'T>, 
                indexLookup : int [], 
                components : UnsafeResizeArray<'T>,
                entities : UnsafeResizeArray<Entity>
            ) =
        if indexLookup.[entity.Index] >= 0 then
            Debug.WriteLine (String.Format ("ECS WARNING: Component, {0}, already added to {1}.", typeof<'T>.Name, entity))
        else
            indexLookup.[entity.Index] <- entities.Count

            components.Add comp
            entities.Add entity

    member inline this.AddInline<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity, comp: byref<'T>) =
        let data = this.GetEntityLookupData<'T> ()

        this.AddInline2<'T> (entity, &comp, data.IndexLookup, data.Components, data.Entities)

    member this.Add<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity, comp: 'T) =
        if this.CurrentIterations > 0 then
            failwith "Can't add while iterating"
        else
            if this.IsValidEntity entity then
                let mutable comp = comp
                this.AddInline<'T>(entity, &comp)
            else
                printfn "%s" (String.Format ("ECS WARNING: {0} is invalid. Cannot add component, {1}", entity, typeof<'T>.Name))

    member this.Remove<'T when 'T : unmanaged and 'T :> IComponent>(entity: Entity) =
        if this.CurrentIterations > 0 then
            failwith "Can't remove while iterating."
        else
            if this.IsValidEntity entity then
                let data = this.GetEntityLookupData<'T> ()

                if data.IndexLookup.[entity.Index] >= 0 then
                    let index = data.IndexLookup.[entity.Index]
                    let swappingEntity = data.Entities.LastItem

                    data.Entities.SwapRemoveAt index
                    data.Components.SwapRemoveAt index

                    data.IndexLookup.[entity.Index] <- -1

                    if not (entity.Index.Equals swappingEntity.Index) then
                        data.IndexLookup.[swappingEntity.Index] <- index
                else
                    Debug.WriteLine (String.Format ("ECS WARNING: Component, {0}, does not exist on {1}", typeof<'T>.Name, entity))

            else
                Debug.WriteLine (String.Format ("ECS WARNING: {0} is invalid. Cannot remove component, {1}", entity, typeof<'T>.Name))

    member this.Spawn () =                         
        if this.RemovedEntityQueue.Count = 0 && this.nextEntityIndex >= this.MaxEntityAmount then
            Debug.WriteLine (String.Format ("ECS WARNING: Unable to spawn entity. Max entity amount hit: {0}", (this.MaxEntityAmount - 1)))
            Entity ()
        else
            let entity =
                if this.RemovedEntityQueue.Count > 0 then
                    let entity = this.RemovedEntityQueue.Dequeue ()
                    Entity (entity.Index, entity.Version + 1u)
                else
                    let index = this.nextEntityIndex
                    this.nextEntityIndex <- index + 1
                    Entity (index, 1u)

            this.ActiveVersions.[entity.Index] <- entity.Version

            entity

    member this.Destroy (entity: Entity) =
        if this.CurrentIterations > 0 then
            failwith "Can't destroy while iterating."
        else
            if this.IsValidEntity entity then
                let entComps = this.EntityComponents.[entity.Index]
                entComps |> Seq.iter (fun struct (_, _, remove) -> remove entity)
                entComps.Clear ()
                entComps.TrimExcess ()

                this.RemovedEntityQueue.Enqueue entity  

                this.ActiveVersions.[entity.Index] <- 0u

            else
                Debug.WriteLine (String.Format ("ECS WARNING: {0} is invalid. Cannot destroy.", entity))

    // Component Query

    //************************************************************************************************************************

    member this.TryGet<'T when 'T : unmanaged and 'T :> IComponent> (entity: Entity, [<Out>] comp : byref<'T>) : bool =
        let mutable data = Unchecked.defaultof<IEntityLookupData>
        if this.Lookup.TryGetValue (typeof<'T>, &data) then
            let data = data :?> EntityLookupData<'T>
            if this.IsValidEntity entity then
                let index = data.IndexLookup.[entity.Index]
                if index >= 0 then
                    comp <- data.Components.Buffer.[index]
                    true
                else
                    false
            else
                false
        else
            false

    member this.IsValid entity =
        this.IsValidEntity entity

    member this.Has<'T when 'T : unmanaged and 'T :> IComponent> (entity: Entity) =
        let mutable data = Unchecked.defaultof<IEntityLookupData>
        if this.Lookup.TryGetValue (typeof<'T>, &data) then
            let data = data :?> EntityLookupData<'T>
            data.IndexLookup.[entity.Index] >= 0
        else
            false   

    //************************************************************************************************************************

    member this.ForEach<'T when 'T : unmanaged and 'T :> IComponent>(f) : unit =
        this.CurrentIterations <- this.CurrentIterations + 1

        this.Iterate<'T>(f)

        this.CurrentIterations <- this.CurrentIterations - 1

    //member this.ForEach<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> (f : Entity -> 'T1 -> 'T2 -> unit) : unit =
    //    this.CurrentIterations <- this.CurrentIterations + 1

    //    this.Iterate<'T1, 'T2> (f)

    //    this.CurrentIterations <- this.CurrentIterations - 1

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

    member this.MaxNumberOfEntities = this.MaxEntityAmount - 1

    member this.DestroyAll () =
        this.ActiveVersions
        |> Array.iteri (fun index version ->
            if version > 0u then
                this.Destroy (Entity (index, version))
        )
