module Archer.VSAdapter.Lib.TestGetter

open System
open System.IO
open Archer
open Microsoft.VisualStudio.TestPlatform.ObjectModel

open Archer.VSAdapter.Lib.Globals

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
            |> List.map (fun test ->
                let t = TestCase (test |> getTestFullName, ExecutorUri |> Uri, source)
                test.Tags
                |> Seq.iter (fun tag ->
                        let testTrait =
                            match tag with
                            | Category s -> Trait ("Category", s)
                            | Only -> Trait ("Only", "true")
                            | Serial -> Trait ("Serial", "true")
                            
                        t.Traits.Add testTrait
                    )
                
                t.LineNumber <- test.Location.LineNumber
                t.CodeFilePath <- Path.Combine (test.Location.FilePath, test.Location.FileName)
                t
            )
        )
        |> List.concat