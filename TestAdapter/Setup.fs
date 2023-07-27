[<Microsoft.FSharp.Core.AutoOpen>]
module Archer.Quiver.TestAdapter.Setup

open System
open System.IO
open Archer.CoreTypes.InternalTypes

[<Literal>]
let ExecutorUri = "executor://ArcherQuiverExecutor"

let forbiddenDirectories =
    [
        Environment.GetEnvironmentVariable "ProgramW6432"
        Environment.GetEnvironmentVariable "ProgramFiles(x86)"
        Environment.GetEnvironmentVariable "ProgramFiles"
        Environment.GetEnvironmentVariable "windir"
    ]
    |> List.filter (fun path -> String.IsNullOrEmpty path |> not)
    |> List.map DirectoryInfo
    
let getTestFullName (test: ITest) = $"%s{test.ContainerPath}.%s{test.ContainerName}.%s{test.TestName}"
