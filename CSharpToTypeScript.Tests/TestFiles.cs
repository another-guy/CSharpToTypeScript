using System;
using System.IO;
using System.Reflection;
using CSharpToTypeScript.Tests.Integration;

namespace CSharpToTypeScript.Tests
{
    public sealed class TestFiles
    {
        public string GetSampleFile(string fileName)
        {
            return new FileInfo(GetAssemblyLocation())
                .Directory
                .Parent
                .Parent
                .Parent
                .UseAsArgFor(directory => Path.Combine(directory.FullName, "SampleFiles", fileName));
        }

        public string GetAssemblyLocation()
        {
            return typeof(TargetTypesLocatorTest)
                .GetTypeInfo()
                .Assembly
                .Location;
        }
    }
}