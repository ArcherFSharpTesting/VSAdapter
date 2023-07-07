namespace Archer.Quiver.Lib

open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger
open Archer.Quiver.Lib.TestGetter
open Archer.Quiver.Lib.Literals
open Archer.Quiver.Lib.Globals
open Archer.Logger.Summaries
open Archer.Logger.Detail
open Archer.Bow

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
            
            let archerTests = getTestsByName ()
            
            let testCases =
                tests
                |> Seq.map (fun t -> t.FullyQualifiedName, t)
                |> dict

            let archerFramework = bow.Runner ()
            
            archerFramework.RunnerLifecycleEvent
            |> Event.add (fun args ->
                match args with
                | RunnerStartExecution _ ->
                    printfn ""
                | RunnerTestLifeCycle (test, testEventLifecycle, _) ->
                    let tc = testCases[test |> getTestFullName]
                    
                    match testEventLifecycle with
                    | TestStartExecution _cancelEventArgs ->
                        tc|> frameworkHandle.RecordStart
                    | TestEndExecution testExecutionResult ->
                        let vsTestResult = TestResult tc
                        let indenter = Indent.IndentTransformer 1
                        
                        let errorMessage = defaultDetailedTestExecutionResultTransformer indenter test None testExecutionResult
                        let errorMessage = errorMessage.Trim ()
                        match testExecutionResult with
                        | TestExecutionResult testResult ->
                            match testResult with
                            | TestFailure failure ->
                                let testOutcome = TestOutcome.Failed
                                match failure with
                                | TestIgnored _ ->
                                    vsTestResult.ErrorMessage <- errorMessage
                                    vsTestResult.Outcome <- TestOutcome.Skipped
                                | _ ->
                                    vsTestResult.ErrorMessage <- errorMessage
                                    vsTestResult.Outcome <- testOutcome
                            | TestSuccess ->
                                let testOutcome = TestOutcome.Passed
                                vsTestResult.Outcome <- testOutcome
                                
                        | _ ->
                            let testOutcome = TestOutcome.Failed
                            vsTestResult.Outcome <- testOutcome
                            vsTestResult.ErrorMessage <- errorMessage
                        
                    
                        vsTestResult
                            |> frameworkHandle.RecordResult
                        (tc, vsTestResult.Outcome)
                            |> frameworkHandle.RecordEnd
                            
                    | _ -> ()
                | RunnerEndExecution ->
                    printfn "\n"
            )
            
            let selectedTests = 
                testCases.Keys
                |> Seq.map (fun testName ->
                    archerTests[testName]
                )
            
            let runTests (runner: IRunner) = runner.Run () |> ignore
                
            selectedTests
            |> archerFramework.AddTests
            |> runTests
            
        member this.Cancel () = () // failwith "todo"
        
        member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            setLogger frameworkHandle
            
            let tests = getTests sources
                
            let executor = this :> ITestExecutor
            
            executor.RunTests (tests, runContext, frameworkHandle)
            
            //failwith "todo"
            