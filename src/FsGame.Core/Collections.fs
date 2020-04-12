module FsGame.Core.Collections

open System
open System.Collections.Generic

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
