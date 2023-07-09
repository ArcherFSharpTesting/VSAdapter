module Archer.Quiver.TestAdapter.Tests.``File System getDirectory Should``

open System.IO
open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter

let feature = Arrow.NewFeature ()

let ``Return a directory info from a source string without full path`` =
     feature.Test (fun _ ->
        let expected = (Directory.GetCurrentDirectory () |> DirectoryInfo).FullName
         
        "Archer.TestAdapter.Tests.dll"
        |> FileSystem.getDirectory
        |> fun di -> di.FullName
        |> Should.BeEqualTo expected
    )
    
let ``Return a directory for a source with a path`` =
     feature.Test (fun _ ->
        let expected = @"C:\MyTestPath"
         
        @"C:\MyTestPath\Directory.GetCurrentDirectory () |> DirectoryInfo"
        |> FileSystem.getDirectory
        |> fun di -> di.FullName
        |> Should.BeEqualTo expected
    )