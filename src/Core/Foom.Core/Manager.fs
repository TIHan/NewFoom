namespace Foom.Core

open System
open System.Collections.Generic
open System.Runtime.InteropServices

#nowarn "9"

[<Struct; StructLayout(LayoutKind.Explicit)>]
type Id =

    [<FieldOffset(0)>]
    val Index : int

    [<FieldOffset(4)>]
    val Version : uint32

    [<FieldOffset(0); DefaultValue>]
    val Id : uint64

    new (index, version) = { Index = index; Version = version }

    member this.IsZero = this.Id = 0UL

    override this.ToString () = String.Format ("(Id #{0}.{1})", this.Index, this.Version)

[<Sealed>]
type Manager<'T>(max: int) =

    let versions = Array.init max (fun _ -> 1u)

    let mutable nextIndex = 0
    let indexQueue = Queue()

    let getNextId() =
        if indexQueue.Count > 0 then
            let index = indexQueue.Dequeue()
            let version = versions.[index] + 1u
            versions.[index] <- version
            Id(index, version)
        else
            let index = nextIndex + 1
            let version = versions.[index] + 1u
            nextIndex <- index
            versions.[index] <- version
            Id(index, version)

    let dataLookup = Array.init max (fun _ -> -1)
    let data = ResizeArray<'T>(max)
    let dataIds = ResizeArray<Id>(max)

    member this.Count = data.Count

    member this.Add(item) =  
        let dataIndex = data.Count
        let id = getNextId()  
        data.Add(item)
        dataIds.Add(id)
        dataLookup.[id.Index] <- dataIndex
        id

    member this.Remove(id: Id) =
        if this.IsValid id then
            let lastDataIndex = data.Count - 1
            let dataIndex = dataLookup.[id.Index]

            data.[dataIndex] <- data.[lastDataIndex]
            dataIds.[dataIndex] <- dataIds.[lastDataIndex]

            data.RemoveAt(lastDataIndex)
            dataIds.RemoveAt(lastDataIndex)

            dataLookup.[id.Index] <- -1

            if lastDataIndex <> dataIndex then
                let swappedId = dataIds.[dataIndex]
                dataLookup.[swappedId.Index] <- dataIndex
        else
            failwithf "Not a valid Id, %A." id

    member this.IsValid(id: Id) =
        versions.[id.Index] = id.Version && dataLookup.[id.Index] <> -1

    member this.ForEach f =
        for i = 0 to data.Count - 1 do
            f dataIds.[i] data.[i]