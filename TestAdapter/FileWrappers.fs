module Archer.Quiver.TestAdapter.FileWrappers

open System.IO

type IPathWrapper =
    abstract member IsPathRooted : path:string -> bool
    
type IDirectoryInfoWrapper =
    abstract member GetFiles : searchPattern:string -> IFileInfoWrapper array
    abstract member FullName : string with get
    
and IFileInfoWrapper =
    abstract member Directory : IDirectoryInfoWrapper with get
    
type DefaultPath () =
    interface IPathWrapper with
        member _.IsPathRooted (path: string) =
            Path.IsPathRooted path
            
type DefaultDirectoryInfo (dir: DirectoryInfo) =
    new (fullName: string) =
        DefaultDirectoryInfo (DirectoryInfo fullName)
        
    member _.True with get () = true
    interface IDirectoryInfoWrapper with
        member _.GetFiles (searchPattern: string) =
            dir.GetFiles searchPattern
            |> Array.map (fun fi -> fi |> DefaultFileInfo :> IFileInfoWrapper)
            
        member _.FullName with get () = dir.FullName
        
and DefaultFileInfo (file: FileInfo) =
    new (fullName: string) =
        DefaultFileInfo (FileInfo fullName)
    interface IFileInfoWrapper with
        member _.Directory with get () =
            file.Directory
            |> DefaultDirectoryInfo
            :> IDirectoryInfoWrapper

type IDirectoryWrapper =
    abstract member GetCurrentDirectory : unit -> IDirectoryInfoWrapper
    
type DefaultDirectory () =
    interface IDirectoryWrapper with
        member _.GetCurrentDirectory () = Directory.GetCurrentDirectory () |> DefaultDirectoryInfo :> IDirectoryInfoWrapper
        
            
let pathHelper = DefaultPath () :> IPathWrapper
let directoryHelper = DefaultDirectory () : IDirectoryWrapper

let getDirectoryInfo (path: string) =
    path
    |> DefaultDirectoryInfo
    :> IDirectoryInfoWrapper
    
let getFileInfo (fullName: string) =
    fullName
    |> DefaultFileInfo
    :> IFileInfoWrapper