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
    if Path.IsPathRooted source then
        let fi = source |> FileInfo
        fi.Directory
    else
        Directory.GetCurrentDirectory ()
        |> DirectoryInfo
    
let private assemblies = System.Collections.Generic.Dictionary<string, Assembly> ()
let private tests = System.Collections.Generic.Dictionary<string, (Type * PropertyInfo) list> ()

let private loadTests (sourceName: string) (assembly: Assembly) =
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
        |> List.filter (fun (_t, prop) -> (prop.PropertyType.ContainsGenericParameters || prop.PropertyType.IsGenericType || prop.PropertyType.IsGenericParameter) |> not)
        
    if (tests.ContainsKey sourceName) then
        tests[sourceName] <-
            [testTypes; tests[sourceName]]
            |> List.concat
            |> List.distinctBy (fun (t, prop) -> (t.FullName, prop.Name))
    else
        tests[sourceName] <- testTypes


let getAssembly sourceName =
    let path = getPath sourceName
    
    path.GetFiles "*.dll"
    |> Array.filter ((fun fi -> fi.FullName) >> assemblies.ContainsKey >> not)
    |> Array.iter (fun fi ->
        let assembly =  fi.FullName |> Assembly.LoadFile
        
        assemblies[fi.FullName] <- assembly
    )
    
    let key = Path.Combine (path.FullName, sourceName)
    
    let assembly = assemblies[key]
    
    assembly |> loadTests sourceName
    assembly
    
let getAssemblyFiles () =
    assemblies.Keys
    |> Seq.map FileInfo
    |> Seq.toList
    
        
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