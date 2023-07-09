module Archer.Quiver.TestAdapter.FileSystem

open System.IO

let getDirectory (fileName: string) =
    if Path.IsPathRooted fileName then
        let fi = fileName |> FileInfo
        fi.Directory
    else
        Directory.GetCurrentDirectory () |> DirectoryInfo
    
let getPossibleTestFilesByGetter (fileGetter: string -> FileInfo array): FileInfo [] =
    [|
        fileGetter "*.dll"
        fileGetter "*.exe"
    |] |> Array.concat
        
let getFiles (dir: DirectoryInfo) (searchPattern: string) =
    dir.GetFiles searchPattern
    
let getPossibleTestFiles (dir: DirectoryInfo) = dir |> getFiles |> getPossibleTestFilesByGetter