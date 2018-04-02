namespace Foom.Game

open System

[<AbstractClass>]
type AbstractServerGame() =

    abstract Update : time: TimeSpan * interval: TimeSpan -> bool
