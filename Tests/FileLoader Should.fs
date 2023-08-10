module Archer.Quiver.TestAdapter.Tests.``FileLoader Should``

open System
open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter.FileSystem
open Archer.Quiver.TestAdapter.FileWrappers

type private Builders = {
    PathBuilder: bool -> IPathWrapper
    DirBuilder: string -> IDirectoryWrapper
    FileInfoBuilder: string -> IFileInfoWrapper
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
            
        let fakeDirInfoBuilder (path: string) =
            {new IDirectoryInfoWrapper with
                member _.GetFiles _ = [||]
                member _.FullName with get () = path
                member _.Exists with get () = true
            }
            
        let fakeFileInfoBuilder (path: string) =
            {new IFileInfoWrapper with
                member _.Directory with get () = fakeDirInfoBuilder @"C:\"
                member _.FullName with get () = path
            }
            
        Ok {
            PathBuilder = pathFakeBuilder
            DirBuilder = fakeDirHelperBuilder
            FileInfoBuilder = fakeFileInfoBuilder
        }
    )
)

let ``Return a rooted File`` =
    feature.Test (fun a ->
        let dir = a.DirBuilder @"B:\bad\dir"
        let path = a.PathBuilder true
        
        let loader = FileLoader ("myFile.dll", path, dir, a.FileInfoBuilder) :> IAssemblyLocator
        
        let f = loader.GetPossibleTestFile ()
        f.FullName
        |> Should.BeEqualTo "myFile.dll"
    )
    
let ``Return a file with path for non rooted file`` =
    feature.Test (fun a ->
        let dir = a.DirBuilder @"M:\y\path"
        let path = a.PathBuilder false
        
        let loader = FileLoader ("myFile.dll", path, dir, a.FileInfoBuilder) :> IAssemblyLocator
        
        let f = loader.GetPossibleTestFile ()
        f.FullName
        |> Should.BeEqualTo @"M:\y\path\myFile.dll"
    )
    