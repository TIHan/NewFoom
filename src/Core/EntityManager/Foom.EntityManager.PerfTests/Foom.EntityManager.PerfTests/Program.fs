open System
open System.Collections.Generic
open System.Diagnostics
open System.Runtime.CompilerServices
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

let test (em: EntityManager) (ent: Entity) =
    em.Add<TestComponent>(ent, Unchecked.defaultof<TestComponent>)

let inline test2 (comp: TestComponent) = comp.w

let test3 (comp: TestComponent) =
    test2 comp

let inline test4 (comp: byref<TestComponent>) = comp.w

let test5 (comp: TestComponent) =
    let mutable comp = comp
    test4 (&comp)

[<MethodImpl(MethodImplOptions.NoInlining)>]
let test6 (comp: byref<TestComponent>) = comp.w

let inline test7 (comp: TestComponent) =
    let mutable comp = comp
    test6 (&comp)

let test8 () =
    test7 (Unchecked.defaultof<TestComponent>)

[<EntryPoint>]
let main argv =
    
    let em = new EntityManager(1000000)
    em.RegisterComponent<TestComponent>()
    em.RegisterComponent<TestComponent2>()
    em.RegisterComponent<TestComponent3>()
    em.RegisterComponent<TestComponent4>()
    em.RegisterComponent<TestComponent5>()

    let archetype = em.CreateArchetype<TestComponent, TestComponent2, TestComponent3, TestComponent4, TestComponent5>()

    for i = 1 to 250 do
        let queue = Queue()
        let spawnEntityTime = Stopwatch.StartNew()
        for i = 1 to 100000 do
            let ent = em.SpawnArchetype(archetype, Unchecked.defaultof<TestComponent>, Unchecked.defaultof<TestComponent2>, Unchecked.defaultof<TestComponent3>, Unchecked.defaultof<TestComponent4>, Unchecked.defaultof<TestComponent5>)
            //let ent = em.Spawn()
            //let mutable t = Unchecked.defaultof<TestComponent>
            //em.Add<TestComponent>(ent, t)
            //let mutable t2 = Unchecked.defaultof<TestComponent2>
            //em.Add<TestComponent2>(ent, t2)
            //let mutable t3 = Unchecked.defaultof<TestComponent3>
            //em.Add<TestComponent3>(ent, t3)
            //let mutable t4 = Unchecked.defaultof<TestComponent4>
            //em.Add<TestComponent4>(ent, t4)
            //let mutable t5 = Unchecked.defaultof<TestComponent5>
            //em.Add<TestComponent5>(ent, t5)
            queue.Enqueue(ent)
        spawnEntityTime.Stop()

        let forEachEntityTime = Stopwatch.StartNew()
        em.ForEach<TestComponent, TestComponent2, TestComponent3, TestComponent4, TestComponent5>(fun _ _ _ _ _ _ -> ())
        forEachEntityTime.Stop()

        let destroyEntityTime = Stopwatch.StartNew()
        while queue.Count > 0 do
            let ent = queue.Dequeue()
           // em.Remove<TestComponent5>(ent)
            em.Destroy(ent)
        destroyEntityTime.Stop()

        printfn "Spawn 1000: %A" spawnEntityTime.Elapsed.TotalMilliseconds
        printfn "Iterate 1000 (5 comps): %A" forEachEntityTime.Elapsed.TotalMilliseconds
        printfn "Destroy 1000: %A" destroyEntityTime.Elapsed.TotalMilliseconds

    0
