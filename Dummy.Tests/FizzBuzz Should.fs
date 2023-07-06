module Dummy.Tests.``FizzBuzz Should``

open System.Collections.Generic
open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.Arrows.Internal.Types

let private feature = Arrow.NewFeature ()
    
let FizzBuzz value = $"%d{value}"

type IAge =
    abstract member Age: int with get
    abstract member ToString: unit -> string

let ``Convert 1 to "1"`` =
    feature.Test (fun _ ->
        1
        |> FizzBuzz
        |> Should.BeEqualTo "1"
    )

let ``Convert 2 to "2"`` =
     feature.Test (fun _ ->
         2
         |> FizzBuzz
         |> Should.BeEqualTo "2"
     )


// let ``Test Cases`` = feature.GetTests ()
