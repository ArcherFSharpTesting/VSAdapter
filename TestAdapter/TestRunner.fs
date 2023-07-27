namespace Archer.Quiver.TestAdapter

open Archer
open Archer.CoreTypes.InternalTypes
open Archer.CoreTypes.InternalTypes.RunnerTypes
open Archer.Logger
open Archer.Logger.Detail
open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter

type TestRunner (archerFramework: IRunner, getter: string -> TestCase, frameworkHandle: IFrameworkHandle) =
    do
        archerFramework.RunnerLifecycleEvent
        |> Event.add (fun args ->
            match args with
            | RunnerStartExecution _ ->
                printfn ""
            | RunnerTestLifeCycle (test, testEventLifecycle, _) ->
                let tc = getter (test |> getTestFullName)
                
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
        
    member this.Run () =
        let runTests (runner: IRunner) = runner.Run () |> ignore
        
        archerFramework
        |> runTests
        
        this
            
    member this.AddTests (tests: ITest seq) =
        archerFramework.AddTests tests
        |> ignore
        
        this

