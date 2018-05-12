namespace Foom.Tasks

open System
open System.Threading
open System.Threading.Tasks

[<Sealed>]
type TaskQueue() =
    let mutable prevTask = Task.FromResult(true) :> Task
    let lockObj = obj ()

    let mutable taskQueueCount = 0

    member __.Count = taskQueueCount

    member __.Enqueue(f) =
        Interlocked.Increment(&taskQueueCount) |> ignore
        lock lockObj
        |> fun _ ->
            prevTask <- prevTask.ContinueWith(fun _ -> 
                f ()
                Interlocked.Decrement(&taskQueueCount)
            )
            prevTask

    interface IDisposable with

        member __.Dispose() = ()
