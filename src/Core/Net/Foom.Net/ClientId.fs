namespace Foom.Net

open Foom.Core

[<Struct>]
type ClientId =
    internal {
        id: Id
    }

    member this.IsLocal = this.id.IsZero

    static member Local = Unchecked.defaultof<ClientId>