namespace Foom.Core

open System
open System.Threading.Tasks

[<Sealed>]
type TaskQueue() =
    let mutable prevTask = Task.FromResult(true) :> Task
    let lockObj = obj ()

    member __.Enqueue(f) =
        lock lockObj
        |> fun _ ->
            prevTask <- prevTask.ContinueWith(fun _ -> f ())
            prevTask
