module Archer.Quiver.TestAdapter.FileSystem

open Archer.Quiver.TestAdapter.FileWrappers

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
        AssemblyLocator (exampleFileName, pathHelper, directoryHelper)
        
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
        
    member this.Locator with get () = this :> IAssemblyLocator
        
    interface IAssemblyLocator with
        member this.GetPossibleTestFiles () =
            this.GetAllLibraries ()
    
let getPossibleTestFiles (locator: #IAssemblyLocator) =
    locator.GetPossibleTestFiles ()
    
let getAssemblyLocatorFromExample (examplePath: string) =
    examplePath
    |> AssemblyLocator
    :> IAssemblyLocator
    