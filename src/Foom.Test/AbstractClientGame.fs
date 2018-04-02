namespace Foom.Game

open System

[<AbstractClass>]
type AbstractClientGame() =

    abstract PreUpdate : time: TimeSpan * interval: TimeSpan -> unit

    abstract Update : time: TimeSpan * interval: TimeSpan -> bool

    abstract Render : time: TimeSpan * deltaTime: float32 -> unit
