using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            Assert.Equal(9, rootTargets.Count);
            Assert.True(TargetMentioned(rootTargets, ".Employee"));
            Assert.True(TargetMentioned(rootTargets, ".Money"));
            Assert.True(TargetMentioned(rootTargets, ".Currency"));
            Assert.True(TargetMentioned(rootTargets, ".Address"));
            Assert.True(TargetMentioned(rootTargets, ".KnownButIgnored"));
            Assert.True(TargetMentioned(rootTargets, ".OkayishClassWithBadProperty"));
            Assert.True(TargetMentioned(rootTargets, ".MyGenericType`2"));
            Assert.True(TargetMentioned(rootTargets, ".GenericTypeWithClosedTypes"));
            Assert.True(TargetMentioned(rootTargets, ".GenericTypeWithMixedTypes`1"));

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
            Assert.Equal(6, rootTargets.Count);
            Assert.True(TargetMentioned(rootTargets, ".Address"));
            Assert.True(TargetMentioned(rootTargets, ".KnownButIgnored"));
            Assert.True(TargetMentioned(rootTargets, ".OkayishClassWithBadProperty"));
            Assert.True(TargetMentioned(rootTargets, ".MyGenericType`2"));
            Assert.True(TargetMentioned(rootTargets, ".GenericTypeWithClosedTypes"));
            Assert.True(TargetMentioned(rootTargets, ".GenericTypeWithMixedTypes`1"));
        }

        private static bool TargetMentioned(IEnumerable<TypeInfo> rootTargets, string typeName)
        {
            return rootTargets.Any(t => t.FullName.EndsWith(typeName));
        }
    }
}
