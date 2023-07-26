module Archer.Quiver.TestAdapter.Tests.``AssemblyLocator GetPossibleTestFiles Should``

open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter
open Archer.Quiver.TestAdapter.FileSystem
open Archer.Quiver.TestAdapter.FileWrappers

let feature = Arrow.NewFeature ()

type private TestDir (fileGetter: string -> IFileInfoWrapper array, fullName) =
    interface IDirectoryInfoWrapper with
        member _.GetFiles pattern = fileGetter pattern
        member _.FullName with get () = fullName
        
let private getLocator (dir: IDirectoryInfoWrapper) =
    AssemblyLocator dir :> IAssemblyLocator
    
let private getTestDir fileGetter =
    TestDir (fileGetter, @"S:\ome\Path")

let ``Call directory info wrapper get files with "*.dll" and "*.exe"`` =
    feature.Test (fun _ ->
        let calls = System.Collections.Generic.List<string> ()
        
        let fileGetter filter =
            calls.Add filter
            Array.empty<IFileInfoWrapper>
            
        let dir = TestDir (fileGetter, @"S:\ome\dive")
        let locator = getLocator dir
        locator.GetPossibleTestFiles () |> ignore
        
        
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
                
        let locator = fileGetter |> getTestDir |> getLocator
                
        locator
        |> getPossibleTestFiles
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
                
        let dir = fileGetter |> getTestDir
        let locator = getLocator dir
                
        locator
        |> getPossibleTestFiles
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
                
        let locator =
            fileGetter
            |> getTestDir
            |> getLocator
                
        locator
        |> getPossibleTestFiles
        |> Should.BeEqualTo expected
    )