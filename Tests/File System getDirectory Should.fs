module Archer.Quiver.TestAdapter.Tests.``AssemblyLocator Should``

open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter.FileSystem
open Archer.Quiver.TestAdapter.FileWrappers

let feature = Arrow.NewFeature ()
     
let ``Have a directory equal to the current directory when constructed with a file name that has no path`` =
    feature.Test (fun _ ->
        let expected = directoryHelper.GetCurrentDirectory().FullName
        
        "Archer.TestAdapter.Tests.dll"
        |> AssemblyLocator
        |> fun di -> di.Directory.FullName
        |> Should.BeEqualTo expected
    )
     
let ``Have a directory equal to the path of a file passed to the constructor`` =
    feature.Test (fun _ ->
        let expected = @"C:\MyTestPath"
         
        @"C:\MyTestPath\Archer.TestAdapter.Tests.dll"
        |> AssemblyLocator
        |> fun di -> di.Directory.FullName
        |> Should.BeEqualTo expected
    )
    
let ``Have a the directory passed to the constructor`` =
    feature.Test (fun _ ->
        let expected = @"C:\MyTestPath" |> DefaultDirectoryInfo
        
        expected
        |> AssemblyLocator
        |> fun di -> di.Directory
        |> Should.BeEqualTo expected
    )