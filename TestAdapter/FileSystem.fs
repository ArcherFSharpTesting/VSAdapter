module Archer.Quiver.TestAdapter.FileSystem

open System.IO
open Archer.Quiver.TestAdapter.FileWrappers

let getDirectory (fileName: string) =
    if path.IsPathRooted fileName then
        let fi = fileName |> getFileInfo
        fi.Directory
    else
        Directory.GetCurrentDirectory () |> getDirectoryInfo
    
let getPossibleTestFilesByGetter (fileGetter: string -> IFileInfoWrapper array): IFileInfoWrapper [] =
    [|
        fileGetter "*.dll"
        fileGetter "*.exe"
    |] |> Array.concat
        
let getFiles (dir: IDirectoryInfoWrapper) (searchPattern: string) =
    dir.GetFiles searchPattern
    
let getPossibleTestFiles (dir: IDirectoryInfoWrapper) = dir |> getFiles |> getPossibleTestFilesByGetter