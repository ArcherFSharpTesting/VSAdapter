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
        addMessage "QuiverDiscoverer created"
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
            logger.SendMessage (TestMessageLevel.Warning, "ITestDiscoverer.DiscoverTests called")
            logger |> logMessages TestMessageLevel.Warning
            
            logger.SendMessage (TestMessageLevel.Warning, "Starting Discovery")
            
            let tests = getTests logger sources
                
            logger.SendMessage (TestMessageLevel.Informational, "End Discovery")
                
            tests
            |> Seq.iter discoverySink.SendTestCase
            
        
[<ExtensionUri (ExecutorUri)>]
type QuiverExecutor () =
    do
        addMessage "QuiverExecutor created"
        
    interface ITestExecutor with
        member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            let itype = typeof<ITest>

            getAllTests ()
            |> List.iter (fun (t, prop) ->
                frameworkHandle.SendMessage (TestMessageLevel.Warning, $"Is ITest ({itype.IsAssignableFrom prop.PropertyType})")

                let a =
                    prop.GetAccessors ()
                    |> Array.head
                    
                let a = a.Invoke (null, null)
                        
                frameworkHandle.SendMessage (TestMessageLevel.Warning, $"{prop.Name}: %A{a}")

                let v = prop.GetValue (null, null)
                let msg =
                    if v = null then $"%s{prop.Name}: (null)"
                    else $"{prop.Name}: %A{v}"
                    
                frameworkHandle.SendMessage (TestMessageLevel.Warning, msg)
            )

            let handleTest (tc: TestCase) =
                frameworkHandle.RecordStart tc
                let testOutcome = TestOutcome.Passed
                let tr = TestResult tc
                tr.Outcome <- testOutcome 
                tr |> frameworkHandle.RecordResult
                frameworkHandle.RecordEnd (tc, testOutcome)
                
            frameworkHandle.SendMessage (TestMessageLevel.Warning, tests |> Seq.length |> sprintf "Running %d Test(s)")                
            tests
            |> Seq.iter handleTest
            
            //failwith "todo"
            
        member this.Cancel () = () // failwith "todo"
        member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            frameworkHandle.SendMessage (TestMessageLevel.Warning, "ITestExecutor.RunTests called")
            frameworkHandle |> logMessages TestMessageLevel.Warning
            let tests = getTests frameworkHandle sources
                
            let executor = this :> ITestExecutor
            
            executor.RunTests (tests, runContext, frameworkHandle)
            
            //failwith "todo"
            
