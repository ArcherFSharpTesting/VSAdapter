module Dummy.Tests.``Word Counter Should``

open System
open Archer
open Archer.Arrows

let feature = Arrow.NewFeature (
    TestTags [
        Category "Word Counter"
    ]
)

let countWords (value: string) =
    let split = value.Split ([|"_"|], StringSplitOptions.RemoveEmptyEntries)
    let words =
        split
        |> Array.map (fun item ->
            System.Text.RegularExpressions.Regex.Split(item, @"(?<!^)(?=[A-Z])")
        )
        |> Array.concat
        
    words.Length
    
    
let ``count "hello" as one word`` =
    feature.Test (fun _ ->
        "hello"
        |> countWords
        |> Should.BeEqualTo 1
    )
    
let ``count "" as zero words`` =
    feature.Test (fun _ ->
        ""
        |> countWords
        |> Should.BeEqualTo 0
    )
    
let ``count "hello_world" as 2 words`` =
    feature.Test (fun _ ->
        "hello_world"
        |> countWords
        |> Should.BeEqualTo 2
    )
    
let ``count "helloWorld_rocks" as 3 words`` =
    feature.Test (fun _ ->
        "helloWorld_rocks"
        |> countWords
        |> Should.BeEqualTo 3
    )