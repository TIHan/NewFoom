module FsGame.Core.Tests

open System
open Xunit
open FsGame.Core.Collections

[<Fact>]
let ``Initialized SparseResizeArray has zero count`` () =
    let sparse = SparseResizeArray<obj>(1024)
    Assert.Equal(0, sparse.Count)

[<Fact>]
let ``Initialized SparseResizeArray adds one item and has one count`` () =
    let sparse = SparseResizeArray<obj>(1024)
    sparse.Add(obj ()) |> ignore
    Assert.Equal(1, sparse.Count)

[<Fact>]
let ``Initialized SparseResizeArray adds one item and removes it and has zero count`` () =
    let sparse = SparseResizeArray<obj>(1024)
    let lookup = sparse.Add(obj ())
    sparse.Remove lookup
    Assert.Equal(0, sparse.Count)

[<Fact>]
let ``Initialized SparseResizeArray adds two items and removes first and has one count`` () =
    let sparse = SparseResizeArray<obj>(1024)
    let lookup1 = sparse.Add(obj ())
    let _lookup2 = sparse.Add(obj ())
    sparse.Remove lookup1
    Assert.Equal(1, sparse.Count)

[<Fact>]
let ``Initialized SparseResizeArray adds two items and removes first and has the second item`` () =
    let sparse = SparseResizeArray<obj>(1024)
    let lookup1 = sparse.Add(obj ())
    let item2 = obj ()
    let lookup2 = sparse.Add item2
    sparse.Remove lookup1
    Assert.Equal(item2, sparse.[lookup2])

[<Fact>]
let ``Initialized SparseResizeArray adds two items and removes first and span index zero is the same second item`` () =
    let sparse = SparseResizeArray<obj>(1024)
    let lookup1 = sparse.Add(obj ())
    let item2 = obj ()
    let _lookup2 = sparse.Add item2
    sparse.Remove lookup1
    Assert.Equal(item2, sparse.AsSpan().[0])