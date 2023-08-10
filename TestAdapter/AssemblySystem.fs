module Archer.Quiver.TestAdapter.AssemblySystem

open System
open System.Reflection
open Archer
open Archer.CoreTypes.InternalTypes
open Archer.Quiver.TestAdapter.FileSystem
open Archer.Quiver.TestAdapter.FileWrappers
open Archer.Quiver.TestAdapter.TestCaseCache
open Microsoft.VisualStudio.TestPlatform.ObjectModel

type IPropertyWrapper =
    abstract member PropertyType : ITypeWrapper with get
    abstract member IsAssignableFrom<'desiredType> : unit -> bool
    abstract member GetStaticValue<'outPutType> : unit -> 'outPutType
    abstract member IsAssignableTo<'desiredType> : unit -> bool
    
and ITypeWrapper =
    abstract member GetPublicStaticPropertiesOf<'outPutType> : unit -> 'outPutType array
    abstract member IsGenericType : bool with get
    abstract member GetGenericTypeArguments : unit -> ITypeWrapper array
    abstract member IsAssignableFrom<'desiredType> : unit -> bool
    abstract member IsAssignableTo<'desiredType> : unit -> bool
    
type IAssemblyWrapper =
    abstract member GetExportedTypes: unit -> ITypeWrapper array
    
type ITestLoader =
    abstract member GetTests: unit -> ITest array

type PropertyWrapper (prop: PropertyInfo) =
    interface IPropertyWrapper with
        member _.PropertyType with get () =
            prop.PropertyType
            |> TypeWrapper
            :> ITypeWrapper
            
        member _.IsAssignableFrom<'desiredType> () =
            prop.PropertyType.IsAssignableFrom typeof<'desiredType>
            
        member _.IsAssignableTo<'desiredType> () =
            prop.PropertyType.IsAssignableTo typeof<'desiredType>
            
        member _.GetStaticValue<'outPutType>() =
            null |> prop.GetValue :?> 'outPutType
            
and TypeWrapper (t: Type) =
    member _.GetProperties (bindingFlags: BindingFlags) =
        t.GetProperties bindingFlags
        |> Array.map (fun p -> p |> PropertyWrapper :> IPropertyWrapper)
        
    interface ITypeWrapper with
        member _.IsAssignableTo<'desiredType> () =
            t.IsAssignableTo typeof<'desiredType>
            
        member _.IsAssignableFrom<'desiredType> () =
            t.IsAssignableFrom typeof<'desiredType>
            
        member _.GetGenericTypeArguments () =
            t.GenericTypeArguments
            |> Array.map (fun t -> t |> TypeWrapper :> ITypeWrapper)
            
        member _.IsGenericType with get () = t.IsGenericType
        
        member this.GetPublicStaticPropertiesOf<'outPutType> () =
            let props = this.GetProperties (BindingFlags.Public ||| BindingFlags.Static ||| BindingFlags.FlattenHierarchy)
            
            let a =
                props
                |> Array.filter (fun prop ->
                    prop.IsAssignableFrom<'outPutType> ()
                    || prop.IsAssignableTo<'outPutType> ()
                )
                |> Array.map (fun p -> p.GetStaticValue<'outPutType> ())
                
            let b =
                props
                |> Array.filter (fun prop ->
                    prop.IsAssignableFrom<'outPutType seq> ()
                    || prop.IsAssignableTo<'outPutType seq> ()
                )
                |> Array.map (fun p ->
                    p.GetStaticValue<'outPutType seq> ()
                    |> Seq.toArray
                )
                |> Array.concat
                
                
            [|a; b|]
            |> Array.concat
            
type AssemblyWrapper (assembly: Assembly) =
    new (file: IFileInfoWrapper) =
        let assembly =
            try
                Assembly.LoadFile file.FullName
            with
            | _ -> Assembly.GetCallingAssembly ()
            
        AssemblyWrapper assembly
        
    interface IAssemblyWrapper with
        member _.GetExportedTypes () =
            try
                assembly.GetExportedTypes ()
                |> Array.map (fun t -> t |> TypeWrapper :> ITypeWrapper)
            with
            | _ -> [||]
            
type TestLoader (assembly: IAssemblyWrapper) =
    new (file: IFileInfoWrapper) =
        TestLoader (AssemblyWrapper file)
        
    interface ITestLoader with
        member _.GetTests () =
            assembly.GetExportedTypes ()
            |> Array.map (fun t ->
                t.GetPublicStaticPropertiesOf<ITest>()
            )
            |> Array.concat
            
let getTestLoaderFromAssembly (assembly: IAssemblyWrapper) =
    TestLoader assembly :> ITestLoader
    
let getTestLoaderFromFile (file: IFileInfoWrapper) =
    TestLoader file :> ITestLoader
            
let getTestLoadersThroughAssembly (getLocator: string -> #IAssemblyLocator) (exampleFile: string) =
    let locator = getLocator exampleFile
    locator.GetPossibleTestFile ()
    |> TestLoader
    
let getTests (loaders: #ITestLoader) =
    loaders.GetTests ()
    
let getTestCase (pathHelper: IPathWrapper) (test: ITest) =
    let tc = TestCase (test |> getTestFullName, ExecutorUri |> Uri, pathHelper.Combine (test.Location.FilePath, test.Location.FileName))
    
    test.Tags
    |> Seq.iter (fun tag ->
        let testTrait =
            match tag with
            | Category s -> Trait ("Category", s)
            | Only -> Trait ("Only", "true")
            | Serial -> Trait ("Serial", "true")
            
        tc.Traits.Add testTrait
    )
    
    tc.LineNumber <- test.Location.LineNumber
    tc
    
let getTestCases (pathHelper: IPathWrapper) (tests: ITest array) =
    tests
    |> Array.map (getTestCase pathHelper)
    
let buildTestCasesWithPath (pathHelper: IPathWrapper) (loaders: #ITestLoader) =
    loaders
    |> getTests
    |> Array.map addCache
    |> getTestCases pathHelper