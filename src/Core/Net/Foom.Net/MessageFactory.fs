namespace Foom.Net

open System.Net
open System.Collections.Concurrent
open Foom.IO.Message

[<Sealed>]
type MessageRegistration() =
    let lookupTypeId = ConcurrentDictionary()
    let poolCreation = ConcurrentDictionary()

    member __.RegisterMessage<'T when 'T :> Message and 'T : (new : unit -> 'T)>(typeId, poolAmount: int) =
        if lookupTypeId.ContainsKey(typeof<'T>) then
            failwithf "TypeId, %i, already registered." typeId

        lookupTypeId.[typeof<'T>] <- typeId
        poolCreation.[typeId] <- fun poolMultiply -> MessagePool<'T>(typeId, poolMultiply * poolAmount) :> MessagePoolBase

    member __.LookupTypeId = lookupTypeId

    member __.CreatePoolLookup(poolMultiply) =
        let lookup = Array.zeroCreate 65536
        poolCreation
        |> Seq.iter (fun pair ->
            lookup.[int pair.Key] <- pair.Value(poolMultiply)
        )
        lookup

[<Sealed>]
type MessageFactory(msgReg: MessageRegistration, poolMultiply) =

    let poolLookup = msgReg.CreatePoolLookup(poolMultiply)

    member __.PoolLookup = poolLookup

    member __.CreateMessage<'T when 'T :> Message>() =
        poolLookup.[int msgReg.LookupTypeId.[typeof<'T>]].Create() :?> 'T

    member __.RecycleMessage<'T when 'T :> Message>(msg: 'T) =
        if msg.IsRecycled then
            failwithf "Message, %s, has already been recycled." (msg.GetType().Name)
        poolLookup.[int msg.TypeId].Recycle(msg)
