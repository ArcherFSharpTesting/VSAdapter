module Archer.Quiver.TestAdapter.Tests.``Source Should``

open System.IO
open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter

let private feature = Arrow.NewFeature ()

let ``Return a directory info from a source string without full path`` =
    feature.Test (
        TestTags [
            Category "Source Path"
        ],
        TestBody (fun _ ->
            let expected = (Directory.GetCurrentDirectory () |> DirectoryInfo).FullName
            
            "Archer.TestAdapter.Tests.dll"
            |> Source.getDirectory
            |> fun di -> di.FullName
            |> Should.BeEqualTo expected
        )
    )
    
let ``Return a directory for a source with a path`` =
    feature.Test (
        TestTags [
            Category "Source Path"
        ],
        TestBody (fun _ ->
            let expected = @"C:\MyTestPath"
            
            @"C:\MyTestPath\Directory.GetCurrentDirectory () |> DirectoryInfo"
            |> Source.getDirectory
            |> fun di -> di.FullName
            |> Should.BeEqualTo expected
        )
    )
    
let Feature = feature