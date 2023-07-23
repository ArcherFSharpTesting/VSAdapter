module Archer.Quiver.TestAdapter.Tests.``File System getPossibleTestFilesByGetter Should``

open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter
open Archer.Quiver.TestAdapter.FileWrappers

let feature = Arrow.NewFeature ()

let ``Call file getter with "*.dll" and "*.exe"`` =
    feature.Test (fun _ ->
        let calls = System.Collections.Generic.List<string> ()
        
        let fileGetter filter =
            calls.Add filter
            Array.empty<IFileInfoWrapper>
            
        FileSystem.getPossibleTestFilesByGetter fileGetter |> ignore
        
        calls
        |> Seq.toList
        |> Should.BeEqualTo [ "*.dll"; "*.exe" ]
    )
    
let ``Return the any dll files found`` =
    feature.Test (fun _ ->
        let expectedDlls = [| "MyFile.dll" |> getFileInfo; "Another.dll" |> getFileInfo |]
        let expectedExes = Array.empty<IFileInfoWrapper>
        let expected = [| expectedDlls; expectedExes |] |> Array.concat
        
        let fileGetter filter =
            if filter = "*.dll" then
                expectedDlls
            else
                expectedExes
                
        fileGetter
        |> FileSystem.getPossibleTestFilesByGetter
        |> Should.BeEqualTo expected
    )
    
let ``Return any exe files`` =
    feature.Test (fun _ ->
        let expectedDlls = Array.empty<IFileInfoWrapper>
        let expectedExes = [| "YourFile.exe" |> getFileInfo; "DoStuff.exe" |> getFileInfo |]
        let expected = [| expectedDlls; expectedExes |] |> Array.concat
        
        let fileGetter filter =
            if filter = "*.dll" then
                expectedDlls
            else
                expectedExes
                
        fileGetter
        |> FileSystem.getPossibleTestFilesByGetter
        |> Should.BeEqualTo expected
    )
    
let ``Return both dll and exe files`` =
    feature.Test (fun _ ->
        let expectedDlls = [| "MyFile.dll" |> getFileInfo; "Another.dll" |> getFileInfo |]
        let expectedExes = [| "YourFile.exe" |> getFileInfo; "DoStuff.exe" |> getFileInfo |]
        let expected = [| expectedDlls; expectedExes |] |> Array.concat
        
        let fileGetter filter =
            if filter = "*.dll" then
                expectedDlls
            else
                expectedExes
                
        fileGetter
        |> FileSystem.getPossibleTestFilesByGetter
        |> Should.BeEqualTo expected
    )