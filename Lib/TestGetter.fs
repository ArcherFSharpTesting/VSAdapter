module Archer.Quiver.Lib.TestGetter

open System
open System.IO
open Microsoft.VisualStudio.TestPlatform.ObjectModel

open Archer.Quiver.Lib.Globals

let getPath (source: string) =
    if Path.IsPathRooted source then source |> FileInfo
    else
        (Directory.GetCurrentDirectory (), source)
        |> Path.Combine
        |> FileInfo
        
let getTests (sources: string seq) =
    sources
        |> Seq.toList
        |> List.map (fun source ->
            source
            |> findTests
            |> List.map (fun (t, p) -> TestCase ($"{t.FullName}.{p.Name}", ExecutorUri |> Uri, source))
        )
        |> List.concat