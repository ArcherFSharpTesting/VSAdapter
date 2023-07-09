module Archer.Quiver.TestAdapter.FileSystem

open System.IO

let getDirectory (fileName: string) =
    if Path.IsPathRooted fileName then
        let fi = fileName |> FileInfo
        fi.Directory
    else
        Directory.GetCurrentDirectory () |> DirectoryInfo