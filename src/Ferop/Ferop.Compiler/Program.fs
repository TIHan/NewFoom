open Ferop

[<EntryPoint>]
let main argv = 
    let result =
        argv
        |> Array.forall(fun x -> Ferop.run x)
    if result then 0 else 1
