module Dummy.Tests.``FizzBuzz Convert Should``

open Archer
open Archer.Arrows

let feature = Arrow.NewFeature (
    TestTags [
        Category "FizzBuzz"
        Category "Converter"
        Serial
    ]
)

let fizzBuzz = function
    | v when v % 15 = 0 -> "FizzBuzz"
    | v when v % 5 = 0 -> "Buzz"
    | v when v % 3 = 0 -> "Fizz"
    | v -> $"%d{v}"

let ``Convert 1 to "1"`` =
    feature.Test (fun _ ->
        1
        |> fizzBuzz
        |> Should.BeEqualTo "1."
    )
    
let ``Convert 2 to "2"`` =
    feature.Test (fun _ ->
        2
        |> fizzBuzz
        |> Should.BeEqualTo "2"
    )
    
let ``Convert 3 to "Fizz"`` =
    feature.Test (fun _ ->
        3
        |> fizzBuzz
        |> Should.BeEqualTo "Fizz"
    )
    
let ``Convert 6 to "Fizz"`` =
    feature.Test (fun _ ->
        6
        |> fizzBuzz
        |> Should.BeEqualTo "Fizz"
    )
    
let ``Convert 5 to "Buzz"`` =
    feature.Test (fun _ ->
        5
        |> fizzBuzz
        |> Should.BeEqualTo "Buzz"
    )
    
let ``Convert 10 to "Buzz"`` =
    feature.Test (fun _ ->
        10
        |> fizzBuzz
        |> Should.BeEqualTo "Buzz"
    )
    
let ``Convert 15 to "FizzBuzz"`` =
    feature.Test (fun _ ->
        15
        |> fizzBuzz
        |> Should.BeEqualTo "FizzBuzz"
    )
    
let ``Convert 30 to "FizzBuzz"`` =
    feature.Test (fun _ ->
        30
        |> fizzBuzz
        |> Should.BeEqualTo "FizzBuzz"
    )