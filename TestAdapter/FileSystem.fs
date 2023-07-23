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

type IAssemblyLocator =
    abstract member GetPossibleTestFiles : unit -> IFileInfoWrapper array

type AssemblyLocator (dir: IDirectoryInfoWrapper) =
    new (exampleFileName: string, path: IPathWrapper, directory: IDirectoryWrapper) =
        if path.IsPathRooted exampleFileName then
            let fi = exampleFileName |> getFileInfo
            
            AssemblyLocator fi.Directory
            
        else
            AssemblyLocator (directory.GetCurrentDirectory ())
            
    new (exampleFileName: string) =
        AssemblyLocator (exampleFileName, path, directory)
        
    member _.Directory with get () = dir
    
    abstract member GetFiles : searchPattern:string -> IFileInfoWrapper array
    default this.GetFiles searchPattern =
        dir.GetFiles searchPattern
        
    abstract member GetAllLibraries : unit -> IFileInfoWrapper array
    default this.GetAllLibraries () =
        [|
            this.GetFiles "*.dll"
            this.GetFiles "*.exe"
        |] |> Array.concat
        
    interface IAssemblyLocator with
        member this.GetPossibleTestFiles () =
            this.GetAllLibraries ()
    