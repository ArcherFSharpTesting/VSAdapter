module Archer.Quiver.TestAdapter.FileSystem

open Archer.Quiver.TestAdapter.FileWrappers

type IAssemblyLocator =
    abstract member GetPossibleTestFile : unit -> IFileInfoWrapper
    
type FileLoader (sourceFile: string, pathHelper: IPathWrapper, directoryHelper: IDirectoryWrapper, fileGetter: string -> IFileInfoWrapper) =
    let filePath =
        if pathHelper.IsPathRooted sourceFile then
            sourceFile
        else
            let path = directoryHelper.GetCurrentDirectory().FullName
            pathHelper.Combine (path, sourceFile)
            
    new (sourceFile: string) =
        FileLoader (sourceFile, pathHelper, directoryHelper, fun s -> DefaultFileInfo s)
            
    interface IAssemblyLocator with
        member _.GetPossibleTestFile () = filePath |> fileGetter
        
    
let getPossibleTestFiles (locator: #IAssemblyLocator) =
    locator.GetPossibleTestFile ()
        