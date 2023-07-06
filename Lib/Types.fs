namespace Archer.Quiver.Lib

open System.Reflection
open Archer.CoreTypes.InternalTypes
open Archer.Quiver.Lib.TestGetter
open Archer.Quiver.Lib.Literals
open Archer.Quiver.Lib.Globals

open System
open Microsoft.FSharp.Collections
open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging

[<FileExtension ".dll">]
[<FileExtension ".exe">]
[<DefaultExecutorUri (ExecutorUri)>]
type QuiverDiscoverer (forbidden: string list) =
    new () =
        let f =
            [
                Environment.GetEnvironmentVariable "ProgramW6432"
                Environment.GetEnvironmentVariable "ProgramFiles(x86)"
                Environment.GetEnvironmentVariable "windir"
            ]
            |> List.filter (fun path -> String.IsNullOrEmpty path |> not)
            |> List.map (fun path -> $"%s{path.ToLower ()}\\")
            
            
        QuiverDiscoverer f
        
    interface ITestDiscoverer with
        member _.DiscoverTests (sources: string seq, discoveryContext: IDiscoveryContext, logger: IMessageLogger, discoverySink: ITestCaseDiscoverySink) =
            setLogger logger
            sendWarning "ITestDiscoverer.DiscoverTests called"
            
            sendWarning "Starting Discovery"
            
            let tests = getTests sources
                
            sendWarning "End Discovery"
                
            tests
            |> Seq.iter discoverySink.SendTestCase
            
        
[<ExtensionUri (ExecutorUri)>]
type QuiverExecutor () =
    interface ITestExecutor with
        member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            setLogger frameworkHandle
            let iType = typeof<ITest>
            
            getAllTests ()
            |> List.iter (fun (t, prop) ->
                sendWarning $"processing: {t.Name}"
                sendWarning $"\tIs ITest ({iType.IsAssignableFrom prop.PropertyType})"

                let v = prop.GetValue (null, null)
                let msg =
                    if v = null then $"\t%s{prop.Name}: (null)"
                    else $"\t{prop.Name}: %A{v}"
                    
                sendWarning msg
            )

            let handleTest (tc: TestCase) =
                frameworkHandle.RecordStart tc
                let testOutcome = TestOutcome.Passed
                let tr = TestResult tc
                tr.Outcome <- testOutcome 
                tr |> frameworkHandle.RecordResult
                frameworkHandle.RecordEnd (tc, testOutcome)
                
            tests
                |> Seq.length
                |> sprintf "Running %d Test(s)"
                |> sendWarning
                
            tests
                |> Seq.iter handleTest
            
        member this.Cancel () = () // failwith "todo"
        
        member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            setLogger frameworkHandle
            sendWarning "ITestExecutor.RunTests called"
            
            let tests = getTests sources
                
            let executor = this :> ITestExecutor
            
            executor.RunTests (tests, runContext, frameworkHandle)
            
            //failwith "todo"
            