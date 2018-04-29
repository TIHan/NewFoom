namespace Foom.EntityManager

open System
open System.Runtime.InteropServices

#nowarn "9"

type EntityArchetype<'T1, 'T2, 'T3, 'T4, 'T5> = 
    private {
        bit1: byte
        bit2: byte
        bit3: byte
        bit4: byte
        bit5: byte
    }

type ForEachDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of Entity * byref<'T> -> unit
type ForEachDelegate<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> = delegate of Entity * byref<'T1> * byref<'T2> -> unit
type ForEachDelegate<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> = delegate of Entity * byref<'T1> * byref<'T2> * byref<'T3> * byref<'T4> * byref<'T5> -> unit
type TryGetDelegate<'T when 'T : unmanaged and 'T :> IComponent> = delegate of byref<'T> -> unit

/// Responsible for querying/adding/removing components and spawning/destroying entities.
[<Sealed>]
type EntityManager =

    new : maxEntityCount: int -> EntityManager

    //************************************************************************************************************************

    member RegisterComponent<'T when 'T : unmanaged and 'T :> IComponent> : unit -> unit

    member GetComponentId<'T when 'T : unmanaged and 'T :> IComponent> : unit -> int

    /// Checks to see if the Entity is valid.
    member IsValid : Entity -> bool

    /// Checks to see if the Entity is valid and has a component of type 'T.
    member Has<'T when 'T : unmanaged and 'T :> IComponent> : Entity -> bool

    member CopyComponentsTo<'T when 'T : unmanaged and 'T :> IComponent> : copyTo: Span<'T> -> unit

    //************************************************************************************************************************

    /// Iterate entities that have a component of type 'T.
    member ForEach<'T when 'T : unmanaged and 'T :> IComponent> : ForEachDelegate<'T> -> unit

    /// Iterate entities that have components of type 'T1 and 'T2.
    member ForEach<'T1, 'T2 when 'T1 : unmanaged and 'T2 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent> : ForEachDelegate<'T1, 'T2> -> unit

    member ForEach<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> : ForEachDelegate<'T1, 'T2, 'T3, 'T4, 'T5> -> unit

    // Components

    member TryGetComponent<'T when 'T : unmanaged and 'T :> IComponent> : Entity * TryGetDelegate<'T> -> unit //(ent: Entity, [<Out>] didGet: byref<bool>) =

    member Add<'T when 'T : unmanaged and 'T :> IComponent> : Entity * byref<'T> -> unit

    member Add<'T when 'T : unmanaged and 'T :> IComponent> : Entity * 'T -> unit

    member Remove<'T when 'T : unmanaged and 'T :> IComponent> : Entity -> unit

    // Entites

    member Spawn : unit -> Entity

    member CreateArchetype<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> : unit -> EntityArchetype<'T1, 'T2, 'T3, 'T4, 'T5>

    member SpawnArchetype<'T1, 'T2, 'T3, 'T4, 'T5 when 'T1 : unmanaged and 'T2 : unmanaged and 'T3 : unmanaged and 'T4 : unmanaged and 'T5 : unmanaged and 'T1 :> IComponent and 'T2 :> IComponent and 'T3 :> IComponent and 'T4 :> IComponent and 'T5 :> IComponent> : EntityArchetype<'T1, 'T2, 'T3, 'T4, 'T5> * 'T1 * 'T2 * 'T3 * 'T4 * 'T5 -> Entity

    /// Defers to destroy the specified Entity.
    member Destroy : Entity -> unit

    member MaxNumberOfEntities : int

   // member internal DestroyAll : unit -> unit
