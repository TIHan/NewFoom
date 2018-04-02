namespace Foom.EntityManager

open System
open System.Runtime.InteropServices

#nowarn "9"

/// Construct that is an unique identifier to the world.
[<Struct; StructLayout(LayoutKind.Explicit)>]
type Entity =

    internal new : int * uint32 -> Entity

    /// Internally used to index entity data in the Entity Manager.
    [<FieldOffset (0)>]
    val Index : int

    /// Version of the entity in relation to its index. 
    /// Re-using the index, will increment the version by one. Doing this repeatly, for example, 60 times a second, it will take more than two years to overflow.
    [<FieldOffset (4)>]
    val Version : uint32

    /// A union, combining index and version. Useful for logging entities.
    [<FieldOffset (0); DefaultValue>]
    val Id : uint64

    /// Checks to see if this Entity is zero-ed out.
    member IsZero : bool

/// A marker for component data.
type IComponent = interface end
