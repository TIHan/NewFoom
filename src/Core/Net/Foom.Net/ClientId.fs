namespace Foom.Net

open Foom.Core

[<Struct>]
type ClientId =

    val Id : Id

    new (id) = { Id = id }

    member this.IsLocal = this.Id.IsZero

    static member Local = Unchecked.defaultof<ClientId>