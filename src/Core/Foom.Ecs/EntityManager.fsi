namespace Foom.EntityManager

open System
open System.Runtime.InteropServices

#nowarn "9"

type ForEachDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of Entity * byref<'T> -> unit
type ForEachDelegate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> = delegate of Entity * byref<'T1> * byref<'T2> -> unit

/// Responsible for querying/adding/removing components and spawning/destroying entities.
[<Sealed>]
type EntityManager =

    static member Create : maxEntityCount: int -> EntityManager

    //************************************************************************************************************************

    /// Attempts to find a component of type 'T based on the specified Entity.
    member TryGet<'T when 'T : unmanaged and 'T :> IComponent> : Entity * [<Out>] comp : byref<'T> -> bool

    /// Checks to see if the Entity is valid.
    member IsValid : Entity -> bool

    /// Checks to see if the Entity is valid and has a component of type 'T.
    member Has<'T when 'T : unmanaged and 'T :> IComponent> : Entity -> bool

    //************************************************************************************************************************

    /// Iterate entities that have a component of type 'T.
    member ForEach<'T when 'T : unmanaged and 'T :> IComponent> : ForEachDelegate<'T> -> unit

    ///// Iterate entities that have components of type 'T1 and 'T2.
    //member ForEach<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> : (Entity -> 'T1 -> 'T2 -> unit) -> unit

    ///// Iterate entities that have components of type 'T1, 'T2, and 'T3.
    //member ForEach<'T1, 'T2, 'T3 when 'T1 :> Component and 'T2 :> Component and 'T3 :> Component> : (Entity -> 'T1 -> 'T2 -> 'T3 -> unit) -> unit

    ///// Iterate entities that have components of type 'T1, 'T2, 'T3, and 'T4.
    //member ForEach<'T1, 'T2, 'T3, 'T4 when 'T1 :> Component and 'T2 :> Component and 'T3 :> Component and 'T4 :> Component> : (Entity -> 'T1 -> 'T2 -> 'T3 -> 'T4 -> unit) -> unit

    // Components

    member Add<'T when 'T : unmanaged and 'T :> IComponent> : Entity * 'T -> unit

    // Entites

    member Spawn : unit -> Entity

    /// Defers to destroy the specified Entity.
    member Destroy : Entity -> unit

    member MaxNumberOfEntities : int

    member internal DestroyAll : unit -> unit
