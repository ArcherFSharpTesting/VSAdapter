module Archer.VSAdapter.Lib.Globals

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open Archer.Types.InternalTypes
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging

let getTestFullName (test: ITest) = $"%s{test.ContainerPath}.%s{test.ContainerName}.%s{test.TestName}"

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
    
let private assemblies = Dictionary<string, Assembly> ()
let private sourceTests = Dictionary<string, ITest list> ()

let getTestsByName () =
    sourceTests.Keys
    |> Seq.map (fun key ->
        sourceTests[key]
    )
    |> Seq.concat
    |> Seq.map (fun test ->
        test |> getTestFullName, test
    )
    |> dict

let private loadTests (sourceName: string) (assembly: Assembly) =
    let types =
        assembly.GetExportedTypes ()
    
    let testTypes =
        types
        |> Array.map (fun t ->
            let properties =
                t.GetProperties (BindingFlags.Public ||| BindingFlags.Static ||| BindingFlags.FlattenHierarchy)
                |> Array.filter (fun prop ->
                    prop.PropertyType.IsAssignableFrom typeof<ITest>
                )
                |> Array.map (fun prop ->
                    null |> prop.GetValue :?> ITest
                )
            properties
        )
        |> Array.concat
        |> Array.toList
        
    if (sourceTests.ContainsKey sourceName) then
        sourceTests[sourceName] <-
            [testTypes; sourceTests[sourceName]]
            |> List.concat
            |> List.distinctBy (fun test -> test |> getTestFullName)
    else
        sourceTests[sourceName] <- testTypes
        
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
   sourceTests[sourceName]
   
let getAllTests () =
    sourceTests.Keys
    |> Seq.toList
    |> List.map (fun key ->
        sourceTests[key]
    )
    |> List.concat