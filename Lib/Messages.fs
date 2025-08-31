[<Microsoft.FSharp.Core.AutoOpen>]
module Archer.VSAdapter.Lib.Messages

open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging

let mutable private gLogger: IMessageLogger option = None

let setLogger (logger: IMessageLogger) =
    match gLogger with
    | None -> gLogger <- Some logger
    | Some _ -> ()
    
let private log f msg =
    match gLogger with
    | None -> ()
    | Some l ->
        let t = l |> f
        t msg
        
let sendWarning =
    let sendMsg (logger: IMessageLogger) =
        let send msg =
            logger.SendMessage (TestMessageLevel.Warning, msg)
            
        send
        
    log sendMsg
    
let sendError = 
    let sendMsg (logger: IMessageLogger) =
        let send msg =
            logger.SendMessage (TestMessageLevel.Error, msg)
            
        send
        
    log sendMsg