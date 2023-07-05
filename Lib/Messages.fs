[<Microsoft.FSharp.Core.AutoOpen>]
module Archer.Quiver.Lib.Messages

open System
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging
open Archer.Quiver.Lib.Globals

let private messages = System.Collections.Generic.List<string * DateTime> ()
let private mapper: (string * DateTime) seq -> string seq = Seq.map (fun (msg: string, date: DateTime) -> $"%s{msg} @ %s{date.ToShortDateString ()} - %s{date.ToShortTimeString ()}")

let addMessage msg = messages.Add (msg, DateTime.Now)

let getMessages () = messages |> mapper |> Seq.toList

let iterMessages f =
    messages
    |> mapper
    |> Seq.iter f
    
let logMessages (logLevel: TestMessageLevel) (logger: IMessageLogger) =
    logger
    |> getLogFunction logLevel
    |> iterMessages
    