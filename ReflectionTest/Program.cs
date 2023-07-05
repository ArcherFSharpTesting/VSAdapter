// See https://aka.ms/new-console-template for more information

using System.Reflection;
using System.Linq;

Console.WriteLine("Hello, World!");

var binPath = new DirectoryInfo(@".\");
var bins = binPath.GetFiles("Archer*.dll");

foreach (var fileInfo in bins)
{
    Console.WriteLine($"\t --> {fileInfo.Name}");
    Assembly.LoadFile(fileInfo.FullName);
}

var testPath = new FileInfo (@"..\..\..\..\Dummy.Tests\bin\Debug\net7.0\Dummy.Tests.dll");
var assembly = Assembly.LoadFile(testPath.FullName);
var types = assembly.GetExportedTypes();

var properties = 
    types.SelectMany(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static));

var tests =
    from prop in properties
    select new { PropertyValue = prop.GetValue(null, null), PropretyName = prop.Name };
    
foreach (var test in tests)
{
    Console.WriteLine(test.PropertyValue == null
        ? $"{test.PropretyName}: (Null)"
        : $"{test.PropretyName}: {test.PropertyValue}");
}