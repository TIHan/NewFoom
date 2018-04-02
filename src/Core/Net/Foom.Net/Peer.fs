namespace Foom.Net

open Foom.IO.Message

type Peer internal (msgReg: MessageRegistration, poolMultiply) =
    let msgFactory = MessageFactory(msgReg, poolMultiply)
    let lookupEvent = Array.zeroCreate<Event<struct(int * Message)>> 65536
    let lookupEventLock = obj ()

    member __.MessageFactory = msgFactory

    member this.MessageReceived<'T when 'T :> Message>() =
        let evt = this.GetEvent<'T>()
        evt.Publish

    member private __.GetEvent<'T when 'T :> Message>() : Event<struct(int * Message)> =
        let t = typeof<'T>
        let index = int msgReg.LookupTypeId.[t]
        let evt =
            // It's ok that we lock this, it's not time critical.
            lock lookupEventLock (fun () ->
                match lookupEvent.[index] with
                | evt when obj.ReferenceEquals(evt, null) ->
                    let evt = Event<struct(int * Message)>()
                    lookupEvent.[index] <- evt
                    evt
                | evt -> evt
            )
        evt

    member __.Publish(msg: Message, clientId) =
        match lookupEvent.[int msg.TypeId] with
        | evt when obj.ReferenceEquals(evt, null) -> 
            failwithf "Not listening to message, %s." (msg.GetType().FullName)
        | evt -> 
            evt.Trigger struct(clientId, msg)

    member __.CreateMessage() =
        msgFactory.CreateMessage()

    member __.RecycleMessage(msg) =
        msgFactory.RecycleMessage(msg)
