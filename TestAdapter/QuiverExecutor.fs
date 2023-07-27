namespace Archer.Quiver.TestAdapter

open Microsoft.VisualStudio.TestPlatform.ObjectModel
open Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter
            
[<ExtensionUri (ExecutorUri)>]
type QuiverExecutor () =
    interface ITestExecutor with
        member this.RunTests (tests: TestCase seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            failwith "todo"
            
        member this.Cancel () = failwith "todo"
        
        member this.RunTests (sources: string seq, runContext: IRunContext, frameworkHandle: IFrameworkHandle): unit =
            failwith "todo"

