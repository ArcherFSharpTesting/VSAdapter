namespace Archer.Quiver.TestAdapter

open System
open System.IO
open Archer.Quiver.TestAdapter.FileWrappers
open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging

[<FileExtension ".dll">]
[<FileExtension ".exe">]
[<DefaultExecutorUri (ExecutorUri)>]
type QuiverDiscoverer (forbidden: IDirectoryInfoWrapper list) =
    new () =
        let f =
            [
                Environment.GetEnvironmentVariable "ProgramW6432"
                Environment.GetEnvironmentVariable "ProgramFiles(x86)"
                Environment.GetEnvironmentVariable "windir"
            ]
            |> List.filter (fun path -> String.IsNullOrEmpty path |> not)
            |> List.map (fun d -> d |> DefaultDirectoryInfo :> IDirectoryInfoWrapper)
            
            
        QuiverDiscoverer f
        
    new (forbidden: DirectoryInfo seq) =
        let f =
            forbidden
            |> Seq.toList
            |> List.map (fun d -> d |> DefaultDirectoryInfo :> IDirectoryInfoWrapper)
            |> List.filter (fun d -> d.Exists)
            
        QuiverDiscoverer f
        
        
    new (forbiddenPaths: string seq) =
        let f = 
            forbiddenPaths
            |> Seq.toList
            |> List.filter (String.IsNullOrEmpty >> not)
            |> List.map (fun p -> p |> DefaultDirectoryInfo :> IDirectoryInfoWrapper)
            |> List.filter (fun d -> d.Exists)
            
        QuiverDiscoverer f
        
    interface ITestDiscoverer with
        member _.DiscoverTests (sources: string seq, discoveryContext: IDiscoveryContext, logger: IMessageLogger, discoverySink: ITestCaseDiscoverySink) =
            failwith "Not implemented"