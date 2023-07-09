module Archer.Quiver.TestAdapter.FileSystem

open System.IO

let getDirectory (source: string) =
    if Path.IsPathRooted source then
        let fi = source |> FileInfo
        fi.Directory
    else
        Directory.GetCurrentDirectory () |> DirectoryInfo