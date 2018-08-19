module Tests

open Xunit
open Foom.EntityManager

[<Struct>]
type TestComponent =
    {
        mutable x: int
        mutable y: int
        mutable z: int
    }

    interface IComponent

let beef<'T when 'T : (new : unit -> 'T)> () = ()

[<Fact>]
let ``EntityManager - Simple Iteration`` () =
    let em = EntityManager(1024)
    em.RegisterComponent<TestComponent>()

    for i = 1 to 1024 do
        let ent = em.Spawn()
        em.Add<TestComponent>(ent, { x = 1; y = 2; z = 3 })

    let mutable happenedOnce = false
    let mutable count = 0

    em.ForEach<TestComponent>(fun _ comp ->
        happenedOnce <- true
        Assert.Equal(1, comp.x)
        Assert.Equal(2, comp.y)
        Assert.Equal(3, comp.z)
        count <- count + 1
    )

    Assert.True(happenedOnce)
    Assert.Equal(1024, count)
