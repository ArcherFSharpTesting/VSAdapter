module Archer.Quiver.TestAdapter.Tests.``TestLoader Should``

open Archer
open Archer.Arrows
open Archer.Quiver.TestAdapter
open Archer.Quiver.TestAdapter.AssemblySystem

let private feature = Arrow.NewFeature ()

let ``Get tests from assembly`` =
    feature.Test (
        Setup (fun _ ->
            let fakeFeature = Arrow.NewFeature "A Fake Feature"
            
            let test1 = fakeFeature.Ignore (testName="01 First Test")
            let test2 = fakeFeature.Ignore (testName="02 Second Test")
            let test3 = fakeFeature.Ignore (testName="03 Third Test")
            
            let testType1 =
                {new ITypeWrapper with
                    member _.GetPublicStaticPropertiesOf<'outPutType> () =
                        [|
                            test3 :> obj :?> 'outPutType
                            test1 :> obj :?> 'outPutType
                        |]
                }
                
            let testType2 =
                {new ITypeWrapper with
                    member _.GetPublicStaticPropertiesOf<'outPutType> () =
                        [|
                            test2 :> obj :?> 'outPutType
                        |]
                }
                
            let assembly =
                {new IAssemblyWrapper with
                    member _.GetExportedTypes () = [|testType1; testType2|] 
                }
                
            Ok (assembly, [|test1; test2; test3|] |> Array.sortBy getTestFullName)
        ),
        TestBody (fun (assembly: IAssemblyWrapper, tests) ->
            assembly
            |> getTestLoaderFromAssembly
            |> fun tl -> tl.GetTests ()
            |> Array.sortBy getTestFullName
            |> Should.BeEqualTo tests
        )
    )