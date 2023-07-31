module Archer.Quiver.TestAdapter.Tests.``FileLoader Should``

open System
open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter.FileSystem
open Archer.Quiver.TestAdapter.FileWrappers

type private Builders = {
    PathBuilder: bool -> IPathWrapper
    DirBuilder: string -> IDirectoryWrapper
}

let private feature = Arrow.NewFeature (
    Setup (fun _ ->
        let pathFakeBuilder rooted =
            {new IPathWrapper with
                member _.IsPathRooted _ = rooted
                member _.Combine ([<ParamArray>]paths: string array) = pathHelper.Combine paths
            }
            
        let fakeDirHelperBuilder (currentDirectory: string) =
            {new IDirectoryWrapper with
                member _.GetCurrentDirectory () = currentDirectory |> DefaultDirectoryInfo :> IDirectoryInfoWrapper
            }
            
        Ok { PathBuilder = pathFakeBuilder; DirBuilder = fakeDirHelperBuilder }
    )
)

let ``Return a rooted File`` =
    feature.Test (
        fun a ->
            let dir = a.DirBuilder @"B:\bad\dir"
            let path = a.PathBuilder true
            
            let loader = FileLoader ("myFile.dll", path, dir, fun s -> DefaultFileInfo s) :> IAssemblyLocator
            
            let f = loader.GetPossibleTestFile ()
            f.FullName
            |> Should.BeEqualTo "myFile.dll"
    )