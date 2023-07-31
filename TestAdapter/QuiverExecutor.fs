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
        
        let runner = TestRunner (archerFramework, (fun id -> testCases[id]), frameworkHandle)
        let selectedTests = 
            testCases.Keys
            |> Seq.map getFromCache
            
        let run (runner: TestRunner) = runner.Run () |> ignore
        
        runner.AddTests selectedTests
        |> run
        
            
    member this.Cancel () = () //failwith "todo"
    
    member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
        let getter (source: string) =
            let f = FileLoader source
            f
            :> IAssemblyLocator
            |> (fun t -> t.GetPossibleTestFile ())
            |> TestLoader
            
        let tests =
            sources
            |> Seq.map (getter >> buildTestCasesWithPath pathHelper)
            |> Seq.toArray
            |> Array.concat
            
        this.RunTests (tests, runContext, frameworkHandle)
        
    interface ITestExecutor with
        member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            this.RunTests (tests, runContext, frameworkHandle)
            
        member this.Cancel () = this.Cancel ()
        
        member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            this.RunTests (sources, runContext, frameworkHandle)

