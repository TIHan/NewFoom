namespace Foom.Core

open System
open System.Collections.Concurrent

type IEvent = interface end

[<Sealed>]
type EventAggregator () =

    let lookup = ConcurrentDictionary<Type, obj>()

    member __.Publish (event: 'T when 'T :> IEvent and 'T : not struct) =
        let mutable value = Unchecked.defaultof<obj>
        if lookup.TryGetValue (typeof<'T>, &value) then
            (value :?> Event<'T>).Trigger event

    member __.GetEvent<'T when 'T :> IEvent> () =
       lookup.GetOrAdd (typeof<'T>, valueFactory = (fun _ -> Event<'T> () :> obj)) :?> Event<'T>
