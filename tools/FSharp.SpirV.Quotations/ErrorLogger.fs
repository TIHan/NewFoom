module internal FSharp.Spirv.Quotations.ErrorLogger

open System
open FSharp.Spirv.Quotations.TypedTree

[<RequireQualifiedAccess>]
type ErrorKind =
    | Error = 0
    | Warning = 1

type Error = 
    | Error of ErrorKind * message: string * textSpan: TextSpan

[<Sealed>]
type ErrorLogger private () =

    [<DefaultValue;ThreadStatic>]
    static val mutable private threadStaticInstance : ErrorLogger

    let mutable errors = []

    member _.Log error =
        errors <- error :: errors

    member _.Errors = errors |> List.rev

    member _.Reset() =
        errors <- []

    static member Instance = 
        if ErrorLogger.threadStaticInstance = Unchecked.defaultof<_> then
            ErrorLogger.threadStaticInstance <- ErrorLogger()
        ErrorLogger.threadStaticInstance

[<Sealed>]
type NonRecoverableErrorException() =
    inherit Exception()

let errorR(message, textSpan) =
    ErrorLogger.Instance.Log(Error(ErrorKind.Error, message, textSpan))

let error(message, textSpan) =
    errorR(message, textSpan)
    raise (NonRecoverableErrorException())

let warn(message, textSpan) =
    ErrorLogger.Instance.Log(Error(ErrorKind.Warning, message, textSpan))
