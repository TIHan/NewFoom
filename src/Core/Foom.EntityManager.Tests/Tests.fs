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

    let ent = em.Spawn()
    //let test = em.Add<TestComponent>(ent)

    //test.x <- 1
    //test.y <- 2
    //test.z <- 3

    let mutable happenedOnce = false

    em.ForEach<TestComponent>(fun _ comp ->
        happenedOnce <- true
        Assert.Equal(1, comp.x)
        Assert.Equal(2, comp.y)
        Assert.Equal(3, comp.z)
    )

    Assert.True(happenedOnce)
