namespace Foom.Net

open Foom.IO.Message

type Peer internal (msgReg: MessageRegistration, poolMultiply) =
    let msgFactory = MessageFactory(msgReg, poolMultiply)

    member __.MessageFactory = msgFactory

    member __.CreateMessage() =
        msgFactory.CreateMessage()

    member __.RecycleMessage(msg) =
        msgFactory.RecycleMessage(msg)
