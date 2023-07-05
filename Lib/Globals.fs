module Archer.Quiver.Lib.Globals

open System
open System.IO
open System.Reflection
open Archer.CoreTypes.InternalTypes
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging

let getLogFunction (logLevel: TestMessageLevel) (logger: IMessageLogger) =
    let log msg =
        logger.SendMessage (logLevel, msg)
        
    log
    
let getPath (source: string) =
    if Path.IsPathRooted source then source |> FileInfo
    else
        (Directory.GetCurrentDirectory (), source)
        |> Path.Combine
        |> FileInfo
    
let private assemblies = System.Collections.Generic.Dictionary<string, Assembly> ()
let private tests = System.Collections.Generic.Dictionary<string, (Type * PropertyInfo) list> ()

let private loadTests (sourceName: string) (assembly: Assembly) =
    if tests.ContainsKey sourceName then
        ()
    else
            
        let types =
            assembly.GetExportedTypes ()
        
        let testTypes =
            types
            |> Array.map (fun t ->
                let properties =
                    t.GetProperties (BindingFlags.Public ||| BindingFlags.Static ||| BindingFlags.FlattenHierarchy)
                    |> Array.map (fun p ->
                        t, p
                    )
                properties
            )
            |> Array.concat
            |> Array.toList
            
        tests[sourceName] <- testTypes


let getAssembly sourceName =
    let path = getPath sourceName
    
    if assemblies.ContainsKey path.FullName then
        assemblies[path.FullName]
    else
        let application =  path.FullName |> File.ReadAllBytes |> Assembly.Load
        
        assemblies[path.FullName] <- application
        
        application
        |> loadTests sourceName
        
        application
        
let findTests sourceName =
   getAssembly sourceName |> ignore
   tests[sourceName]
   
let getAllTests () =
    tests.Keys
    |> Seq.toList
    |> List.map (fun key ->
        tests[key]
    )
    |> List.concat