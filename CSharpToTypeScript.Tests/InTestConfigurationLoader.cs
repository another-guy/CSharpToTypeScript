﻿using System;
using System.IO;
using System.Linq;
using CSharpToTypeScript.Core.Configuration;
using Newtonsoft.Json;

namespace CSharpToTypeScript.Tests
{
    public sealed class InTestConfigurationLoader
    {
        public CompleteConfiguration GetConfiguration()
        {
            var testFiles = new TestFiles();
            var assemblyLocation = testFiles.GetAssemblyLocation();
            var configurationPath = testFiles.GetSampleFile("sample.debug.cfg.json");
            return File
                .ReadAllText(configurationPath)
                .UseAsArgFor(JsonConvert.DeserializeObject<CompleteConfiguration>)
                .WithAllPathsRelativeToFile(configurationPath, s => s.Replace("Debug", GetRunningConfigurationBasedOnAssemblyPath(assemblyLocation)));
        }

        private static string GetRunningConfigurationBasedOnAssemblyPath(string assemblyLocation)
        {
            var parts = assemblyLocation.Split('\\').ToList();
            var indexOfBinDirectory = parts.IndexOf("bin");
            return parts[indexOfBinDirectory + 1];
        }
    }
}
