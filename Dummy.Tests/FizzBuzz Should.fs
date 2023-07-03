module Dummy.Tests.``FizzBuzz Should``

open Archer
open Archer.Arrows
open Archer.Arrows.Internals
open Archer.Arrows.Internal.Types

let private feature = Arrow.NewFeature ()

type private Thing = {
    UnitProp: unit
}

let private names =
    let t = typeof<Thing>
    t.Namespace, t.DeclaringType.Name
    
let FizzBuzz value = ""

let ``Convert 1 to "1"`` =
    feature.Test (fun _ ->
        1
        |> FizzBuzz
        |> Should.BeEqualTo "1"
    )


// let ``Test Cases`` = feature.GetTests ()
