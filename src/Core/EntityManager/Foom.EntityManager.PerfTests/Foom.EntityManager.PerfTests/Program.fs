open System
open System.Collections.Generic
open System.Diagnostics
open Foom.EntityManager

[<Struct>]
type TestComponent =
    {
        x: int64
        y: int64
        z: int64
        w: int64
    }

    interface IComponent

[<Struct>]
type TestComponent2 =
    {
        x: int64
        y: int64
        z: int64
        w: int64
    }

    interface IComponent

[<Struct>]
type TestComponent3 =
    {
        x: int64
        y: int64
        z: int64
        w: int64
    }

    interface IComponent

[<Struct>]
type TestComponent4 =
    {
        x: int64
        y: int64
        z: int64
        w: int64
    }

    interface IComponent

[<Struct>]
type TestComponent5 =
    {
        x: int64
        y: int64
        z: int64
        w: int64
    }

    interface IComponent

[<EntryPoint>]
let main argv =
    
    let em = new EntityManager(10000000)
    em.RegisterComponent<TestComponent>()
    em.RegisterComponent<TestComponent2>()
    em.RegisterComponent<TestComponent3>()
    em.RegisterComponent<TestComponent4>()
    em.RegisterComponent<TestComponent5>()

    for i = 1 to 10 do
        let queue = Queue()
        let spawnEntityTime = Stopwatch.StartNew()
        for i = 1 to 10000000 do
            let ent = em.Spawn()
            queue.Enqueue(ent)
        spawnEntityTime.Stop()

        let destroyEntityTime = Stopwatch.StartNew()
        while queue.Count > 0 do
            let ent = queue.Dequeue()
            em.Destroy(ent)
        destroyEntityTime.Stop()

        printfn "Spawn 10M: %A" spawnEntityTime.Elapsed.TotalMilliseconds
        printfn "Destroy 10M: %A" destroyEntityTime.Elapsed.TotalMilliseconds

    0
