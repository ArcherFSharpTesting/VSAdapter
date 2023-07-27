namespace Archer.Quiver.TestAdapter

open Archer.Bow
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger
open Archer.Quiver.TestAdapter.AssemblySystem
open Archer.Quiver.TestAdapter.FileSystem
open Archer.Quiver.TestAdapter.FileWrappers
open Archer.Quiver.TestAdapter.TestCaseCache
open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter

open Archer
open Archer.Logger.Detail
open Microsoft.FSharp.Collections

            
[<ExtensionUri (ExecutorUri)>]
type QuiverExecutor () =
    member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
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
            |> Seq.map getFromCache
        
        let runTests (runner: IRunner) = runner.Run () |> ignore
            
        selectedTests
        |> archerFramework.AddTests
        |> runTests
            
    member this.Cancel () = () //failwith "todo"
    
    member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
        let tests =
            sources
            |> Seq.map (getTestLoadersThroughAssembly AssemblyLocator >> buildTestCasesWithPath pathHelper)
            |> Seq.toArray
            |> Array.concat
            
        this.RunTests (tests, runContext, frameworkHandle)
        
    interface ITestExecutor with
        member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            this.RunTests (tests, runContext, frameworkHandle)
            
        member this.Cancel () = this.Cancel ()
        
        member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            this.RunTests (sources, runContext, frameworkHandle)

