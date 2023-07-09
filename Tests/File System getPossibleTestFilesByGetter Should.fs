module Archer.Quiver.TestAdapter.Tests.``File System getPossibleTestFilesByGetter Should``

open System.IO
open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter

let feature = Arrow.NewFeature ()

let ``Call file getter with "*.dll" and "*.exe"`` =
    feature.Test (fun _ ->
        let calls = System.Collections.Generic.List<string> ()
        
        let fileGetter filter =
            calls.Add filter
            Array.empty<FileInfo>
            
        FileSystem.getPossibleTestFilesByGetter fileGetter |> ignore
        
        calls
        |> Seq.toList
        |> Should.BeEqualTo [ "*.dll"; "*.exe" ]
    )
    
let ``Return the any dll files found`` =
    feature.Test (fun _ ->
        let expected = [| "MyFile.dll" |> FileInfo; "YourFile.dll" |> FileInfo |]
        
        let fileGetter filter =
            if filter = "*.dll" then
                expected
            else
                Array.empty
                
        fileGetter
        |> FileSystem.getPossibleTestFilesByGetter
        |> Should.BeEqualTo expected
    )