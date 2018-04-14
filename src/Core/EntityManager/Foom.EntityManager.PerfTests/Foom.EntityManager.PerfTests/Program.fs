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
    
    let em = new EntityManager(10000)
    em.RegisterComponent<TestComponent>()
    em.RegisterComponent<TestComponent2>()
    em.RegisterComponent<TestComponent3>()
    em.RegisterComponent<TestComponent4>()
    em.RegisterComponent<TestComponent5>()

    let ent = em.Spawn()
    em.Add<TestComponent>(ent)

    for i = 1 to 15 do
        let queue = Queue()
        let spawnEntityTime = Stopwatch.StartNew()
        for i = 1 to 1000 do
            let ent = em.Spawn()
            em.Add<TestComponent>(ent)
            em.Add<TestComponent2>(ent)
            em.Add<TestComponent3>(ent)
            em.Add<TestComponent4>(ent)
            em.Add<TestComponent5>(ent)
            queue.Enqueue(ent)
        spawnEntityTime.Stop()

        let destroyEntityTime = Stopwatch.StartNew()
        while queue.Count > 0 do
            let ent = queue.Dequeue()
           // em.Remove<TestComponent5>(ent)
            em.Destroy(ent)
        destroyEntityTime.Stop()

        printfn "Spawn 1000: %A" spawnEntityTime.Elapsed.TotalMilliseconds
        printfn "Destroy 1000: %A" destroyEntityTime.Elapsed.TotalMilliseconds

    0
