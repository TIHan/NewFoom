module FsGame.Core.Collections

open System
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9"

[<Sealed>]
type SparseResizeArray<'T>(capacity: int) =

    let mutable indexLookupCount = 0
    let mutable indexLookup = Array.init capacity (fun _ -> -1)

    let mutable itemCount = 0
    let mutable itemsLookup = Array.init capacity (fun _ -> -1)
    let mutable items = Array.init capacity (fun _ -> Unchecked.defaultof<_>)

    let lookupQueue = Queue()

    let ensureIndexLookupCapacity () =
        if indexLookup.Length < indexLookupCount then
            let newIndexLookup = Array.zeroCreate (indexLookup.Length * 2)
            indexLookup.CopyTo(newIndexLookup, 0)
            indexLookup <- newIndexLookup

    let ensureItemCapacity () =
        if items.Length < itemCount then
            let newItemsLookup = Array.zeroCreate (itemsLookup.Length * 2)
            itemsLookup.CopyTo(newItemsLookup, 0)
            itemsLookup <- newItemsLookup

            let newItems = Array.zeroCreate (items.Length * 2)
            items.CopyTo(newItems, 0)
            items <- newItems

    let ensureCapacity () =
        ensureIndexLookupCapacity ()
        ensureItemCapacity ()

    let incrementIndexLookupCount () =
        indexLookupCount <- indexLookupCount + 1

    let incrementItemCount () =
        itemCount <- itemCount + 1

    let decrementItemCount () =
        itemCount <- itemCount - 1

    member _.Count = itemCount

    member _.Item with get lookup = items.[indexLookup.[lookup]] and set lookup item = items.[indexLookup.[lookup]] <- item

    member _.Add(item: inref<'T>) =
        incrementItemCount ()
        match lookupQueue.TryDequeue () with
        | true, lookup ->
            let index = indexLookup.[lookup]
            itemsLookup.[index] <- lookup
            items.[index] <- item
            lookup
        | _ ->
            let lookup = indexLookupCount
            let index = itemCount - 1
            incrementIndexLookupCount ()
            ensureCapacity ()
            indexLookup.[lookup] <- index
            itemsLookup.[index] <- lookup
            items.[index] <- item
            lookup

    member inline this.Add(item: 'T) =
        this.Add(&item)

    member _.Remove(lookup: int) =
        match indexLookup.[lookup] with
        | -1 -> invalidArg "lookup" "Item does not exist."
        | index ->
            let lastIndex = itemCount - 1
            if index = lastIndex then
                itemsLookup.[index] <- -1
                items.[index] <- Unchecked.defaultof<_>
                indexLookup.[lookup] <- -1
            else
                let lastLookup = itemsLookup.[lastIndex]             
                itemsLookup.[index] <- itemsLookup.[lastIndex]
                items.[index] <- items.[lastIndex]
                indexLookup.[lastLookup] <- lastIndex
                indexLookup.[lookup] <- -1

            decrementItemCount ()
            lookupQueue.Enqueue lookup

    member _.AsSpan() = Span(items, 0, itemCount)

[<Struct>]
type ItemId = private ItemId of lookup: int * version: uint32

[<Sealed>]
type ItemManager<'T>(capacity: int) =

    let mutable versions = Array.init capacity (fun _ -> 1u)
    let items = SparseResizeArray<'T> capacity

    member _.Count = items.Count

    member _.Add item =
        let lookup = items.Add item
        if lookup >= versions.Length then
            let newVersions = Array.zeroCreate (versions.Length * 2)
            versions.CopyTo(newVersions, 0)
            versions <- newVersions
        ItemId(lookup, versions.[lookup])

    member _.Remove itemId =
        match itemId with
        | ItemId(lookup, version) ->
            if lookup >= versions.Length || versions.[lookup] <> version then
                invalidArg "itemId" "Item does not exist."
            versions.[lookup] <- 
                let newVersion = version + 1u
                if newVersion = 0u then
                    invalidOp "Removing an item has overflowed."
                newVersion

    member _.TryGet(itemId, item: outref<'T>) =
        match itemId with
        | ItemId(lookup, version) ->
            if lookup >= versions.Length || versions.[lookup] <> version then
                false
            else
                item <- items.[lookup]
                true

    member _.AsSpan() = items.AsSpan()

// ECS

[<Struct; StructLayout(LayoutKind.Explicit)>]
type Entity =

    [<FieldOffset (0)>]
    val Index : int

    [<FieldOffset (4)>]
    val Version : uint32

    [<FieldOffset (0); DefaultValue>]
    val Id : uint64

    new (index, version) = { Index = index; Version = version }

    member this.IsNone = this.Id = 0UL

    override this.ToString () = String.Format ("(Entity #{0}.{1})", this.Index, this.Version)

let inline ensureArrayCapacity (arr: byref<_[]>) capacity initValue =
    if arr.Length <= capacity then
        let newArr = Array.init (capacity * 2) (fun _ -> initValue)
        arr.CopyTo(newArr, 0)
        arr <- newArr

[<Struct;System.Runtime.CompilerServices.IsByRefLike>]
type Components<'T when 'T : struct> =

    val mutable internal Indices : ReadOnlySpan<int>
    val mutable internal Entites : ReadOnlySpan<Entity>
    val mutable internal Components : Span<'T>

[<Sealed>]
type private ComponentManager<'T when 'T : struct>() =

    let mutable isLocked = false
    let mutable indices = [||]

    let mutable count = 0
    let mutable entities = [||]
    let mutable items = [||]


    member inline internal _.Set(ent: Entity, com: inref<'T>) =
        if isLocked then
            invalidOp "Unable to set component. This component type is locked."

        ensureArrayCapacity &indices ent.Index -1
        
        let index = 
            let index = indices.[ent.Index]
            if index = -1 then
                let index = count
                count <- count + 1
                ensureArrayCapacity &entities count Unchecked.defaultof<Entity>
                ensureArrayCapacity &items count Unchecked.defaultof<'T>
                index
            else
                index

        items.[index] <- com

    member _.Lock() =
        isLocked <- true
        let mutable components = Components()
        components.Indices <- ReadOnlySpan indices
        components.Entites <- ReadOnlySpan(entities, 0, count)
        components.Components <- Span(items, 0, count)
        components

    member _.Unlock() =
        isLocked <- false
    
[<Sealed>]
type EntityManager() =

    let mutable count = 0
    let mutable versions = [||]

    let mutable nextIndex = 0

    let indexQueue = Queue()

    let componentManagers = Dictionary<Type, obj>()

    member inline private _.GetComponentManager<'T when 'T : struct>() =
        componentManagers.[typeof<'T>] :?> ComponentManager<'T>

    member this.AddComponent<'T when 'T : struct>(ent: Entity, com: inref<'T>) =
        let comManager = this.GetComponentManager<'T>()
        comManager.Set(ent, &com)

    member this.LockComponents<'T when 'T : struct>() =
        this.GetComponentManager<'T>().Lock()

    member this.UnlockComponents<'T when 'T : struct>() =
        this.GetComponentManager<'T>().Unlock()

    member this.CreateEntity<'T when 'T : struct>(com: inref<'T>) =
        let index = 
            match indexQueue.TryDequeue() with
            | true, index -> index
            | _ ->
                count <- count + 1
            
                ensureArrayCapacity &versions count 0u

                let index = nextIndex
                nextIndex <- nextIndex + 1
                index

        let version = versions.[index] + 1u
        versions.[index] <- version

        let ent = Entity(index, version)
        this.AddComponent(ent, &com)
        ent
