using System.Linq;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using Xunit;

namespace CSharpToTypeScript.Tests.Integration
{
    public sealed class TargetTypesLocatorTest
    {
        private InputConfiguration Configuration { get; }
        private TargetTypesLocator TargetTypesLocator { get; }

        public TargetTypesLocatorTest()
        {
            var completeConfiguration = new InTestConfigurationLoader()
                .GetConfiguration("sample.debug.cfg.json");
            Configuration = completeConfiguration.Input;
            TargetTypesLocator = new TargetTypesLocator(Configuration);
        }

        [Fact]
        public void CanLocateTypesWhenNoExcludesSpecified()
        {
            // Arrange
            // Act
            var rootTargets = TargetTypesLocator
                .LocateRootTargets()
                .ToList();

            // Assert
            Assert.Equal(7, rootTargets.Count);
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Employee")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Money")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Currency")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Address")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".KnownButIgnored")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".OkayishClassWithBadProperty")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".MyGenericType`1")));
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
                .LocateRootTargets()
                .ToList();

            // Assert
            Assert.Equal(4, rootTargets.Count);
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".Address")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".KnownButIgnored")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".OkayishClassWithBadProperty")));
            Assert.True(rootTargets.Any(t => t.FullName.EndsWith(".MyGenericType`1")));
        }
    }
}
