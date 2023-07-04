namespace Archer.Quiver.Lib

open Archer.Quiver.Lib.TestGetter
open Archer.Quiver.Lib.Literals

open System
open System.IO
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
            logger.SendMessage (TestMessageLevel.Warning, "Starting Discovery")
            
            let getPath (source: string) =
                if Path.IsPathRooted source then source
                else
                    (Directory.GetCurrentDirectory (), source)
                    |> Path.Combine
            let tests = getTests logger sources
                
            logger.SendMessage (TestMessageLevel.Informational, "End Discovery")
                
            tests
            |> Seq.iter discoverySink.SendTestCase
            
        
[<ExtensionUri (ExecutorUri)>]
type QuiverExecutor () =
    interface ITestExecutor with
        member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
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
            let tests = getTests frameworkHandle sources
                
            let executor = this :> ITestExecutor
            
            executor.RunTests (tests, runContext, frameworkHandle)
            
            //failwith "todo"
