module Archer.Quiver.Lib.TestGetter

open System
open System.IO
open Microsoft.VisualStudio.TestPlatform.ObjectModel

let getPath (source: string) =
    if Path.IsPathRooted source then source
    else
        (Directory.GetCurrentDirectory (), source)
        |> Path.Combine
        
let getTests (sources: string seq) =
    sources
        |> Seq.mapi (fun i name ->
            let path = getPath name
            let fns =
                name.Split ([|"."; "\\"|], StringSplitOptions.RemoveEmptyEntries)
                
            fns
            |> Seq.map (fun fn -> TestCase ( $"%s{fn}..Test%d{i}", ExecutorUri |> Uri, $"%s{fn}..Test%d{i}")) 
            
        )
        |> Seq.concat