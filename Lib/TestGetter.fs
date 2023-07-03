module Archer.Quiver.Lib.TestGetter

open System
open System.IO
open System.Reflection
open Archer.CoreTypes.InternalTypes
open Microsoft.VisualStudio.TestPlatform.ObjectModel

let getPath (source: string) =
    if Path.IsPathRooted source then source |> FileInfo
    else
        (Directory.GetCurrentDirectory (), source)
        |> Path.Combine
        |> FileInfo
        
let getTests (sources: string seq) =
    sources
        |> Seq.mapi (fun i sourceName ->
            let path = getPath sourceName
            
            let application = Assembly.LoadFile path.FullName
            
            let types = application.GetExportedTypes ()
            
            let testTypes =
                types
                |> Array.map (fun t ->
                    let properties =
                        t.GetProperties (BindingFlags.Public ||| BindingFlags.Static)
                    //     |> Array.filter (fun p ->
                    //         let propInterfaces =
                    //             p.PropertyType.GetInterfaces ()
                    //         
                    //         propInterfaces
                    //         |> Array.contains typeof<ITest>
                    //     )
                    //     
                    // 0 < properties.Length
                    properties
                )
                |> Array.concat
            
            // let fns =
            //     sourceName.Split ([|"."; "\\"|], StringSplitOptions.RemoveEmptyEntries)
                
            // fns
            // |> Seq.map (fun fn -> TestCase ( $"%s{fn}..Test%d{i}", ExecutorUri |> Uri, sourceName ))
        
            testTypes
            |> Array.map (fun tt ->
                TestCase ($"%s{tt.DeclaringType.FullName}.%s{tt.Name}", ExecutorUri |> Uri, sourceName)
            )
            
        )
        |> Seq.concat