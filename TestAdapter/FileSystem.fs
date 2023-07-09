module Archer.Quiver.TestAdapter.FileSystem

open System.IO

let getDirectory (fileName: string) =
    if Path.IsPathRooted fileName then
        let fi = fileName |> FileInfo
        fi.Directory
    else
        Directory.GetCurrentDirectory () |> DirectoryInfo
        
let getFiles (dir: DirectoryInfo) (searchPattern: string) =
    dir.GetFiles searchPattern
    
let getPossibleTestFilesByGetter (fileGetter: string -> FileInfo array): FileInfo [] =
    let o = fileGetter "*.dll"
    fileGetter "*.exe" |> ignore
    o
    
let getPossibleTestFiles (dir: DirectoryInfo) = dir |> getFiles |> getPossibleTestFilesByGetter