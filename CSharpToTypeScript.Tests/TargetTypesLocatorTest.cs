using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace CSharpToTypeScript.Tests
{
    public class TargetTypesLocatorTest
    {
        private TargetTypesLocator TargetTypesLocator { get; } = new TargetTypesLocator();
        private InputConfiguration Configuration { get; }

        public TargetTypesLocatorTest(ITestOutputHelper testOutputHelper)
        {
            var assemblyLocation = typeof(TargetTypesLocatorTest)
                .GetTypeInfo()
                .Assembly
                .Location;
            
            var configurationPath = new FileInfo(assemblyLocation)
                .Directory
                .Parent
                .Parent
                .Parent
                .UseAsArgFor(directory => Path.Combine(directory.FullName, "SampleFiles", "sample.debug.cfg.json"));

            var completeConfiguration = File
                .ReadAllText(configurationPath)
                .UseAsArgFor(JsonConvert.DeserializeObject<CompleteConfiguration>)
                .WithAllPathsRelativeToFile(configurationPath, s => s.Replace("Debug", GetRunningConfigurationBasedOnAssemblyPath(assemblyLocation)));

            Configuration = completeConfiguration.Input;
        }

        private static string GetRunningConfigurationBasedOnAssemblyPath(string assemblyLocation)
        {
            var parts = assemblyLocation.Split('\\').ToList();
            var indexOfBinDirectory = parts.IndexOf("bin");
            return parts[indexOfBinDirectory + 1];
        }

        [Fact]
        public void CanLocateTypesWhenNoExcludesSpecified()
        {
            // Arrange
            // Act
            var rootTargets = TargetTypesLocator
                .LocateRootTargetsUsing(Configuration)
                .ToList();

            // Assert
            Assert.Equal(6, rootTargets.Count);
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Employee")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Money")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Currency")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Address")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".KnownButIgnored")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".OkayishClassWithBadProperty")));
        }

        [Fact]
        public void CanLocateTypesAndAcknowledgeExcludes()
        {
            // Arrange
            Configuration
                .ExcludeTypes
                .AddRange(new[] { "Employee", "Money", "Currency" });

            // Act
            var rootTargets = TargetTypesLocator
                .LocateRootTargetsUsing(Configuration)
                .ToList();

            // Assert
            Assert.Equal(3, rootTargets.Count);
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Address")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".KnownButIgnored")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".OkayishClassWithBadProperty")));
        }
    }
}
