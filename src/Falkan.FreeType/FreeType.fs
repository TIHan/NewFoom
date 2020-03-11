module Falkan.FreeType

open Falkan.FreeTypeInterop

#nowarn "9"
#nowarn "51"

[<AutoOpen>]
module internal Helpers =

    let internal checkResult n =
        if n <> 0 then
            failwith "Failed FreeType call"

[<Sealed>]
type Face(lib: FT_Library, face: FT_Face) =
    class end

[<Sealed>]
type FreeType private (lib: FT_Library) =

    member _.Load(filePath: string) =
        let mutable face = Unchecked.defaultof<_>
        FT_New_Face(lib, filePath, 0, &&face) |> checkResult

    static member Create() =
        let mutable lib = Unchecked.defaultof<_>
        FT_Init_FreeType &&lib |> checkResult
        FreeType lib