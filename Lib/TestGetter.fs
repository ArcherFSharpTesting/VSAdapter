module Archer.Quiver.Lib.TestGetter

open System
open System.IO
open System.Reflection
open Archer.CoreTypes.InternalTypes
open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging

let getPath (source: string) =
    if Path.IsPathRooted source then source |> FileInfo
    else
        (Directory.GetCurrentDirectory (), source)
        |> Path.Combine
        |> FileInfo
        
let getTests (logger: IMessageLogger) (sources: string seq) =
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

                    properties
                    |> Array.iter (fun prop ->
                        //let tmp = prop.GetValue (null)
                        //let msg = 
                        //    if tmp = null then $"{prop.Name}: (null)"
                        //    else $"{prop.Name}: '{tmp}'"
                        //logger.SendMessage (TestMessageLevel.Warning, msg)
                        logger.SendMessage (TestMessageLevel.Warning, prop.ToString ())
                    )

                    properties
                )
                |> Array.concat
            
            // let fns =
            //     sourceName.Split ([|"."; "\\"|], StringSplitOptions.RemoveEmptyEntries)
                
            // fns
            // |> Seq.map (fun fn -> TestCase ( $"%s{fn}..Test%d{i}", ExecutorUri |> Uri, sourceName ))
        
            testTypes
            |> Array.map (fun tt ->
                let tc = TestCase ($"%s{tt.DeclaringType.FullName}.%s{tt.Name}", ExecutorUri |> Uri, sourceName)
                tc
            )
            
        )
        |> Seq.concat