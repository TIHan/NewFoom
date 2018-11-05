module NativeCollectionsTests

open System
open System.Collections.Generic
open Xunit
open Foom.NativeCollections

[<Fact>]
let ``NativeQueue`` () =
    let queue = new NativeQueue<int>(10)
    queue.Enqueue(1)

    let x = queue.Dequeue()
    Assert.Equal(1, x)