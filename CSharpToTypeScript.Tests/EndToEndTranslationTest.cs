using System;
using System.IO;
using System.Linq;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Output;
using CSharpToTypeScript.Core.Translation;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.SimpleInjector;
using Xunit;

namespace CSharpToTypeScript.Tests
{
    public sealed class EndToEndTranslationTest
    {
        [Fact]
        public void CorrectlyTranslatesDemoTargetAssembly()
        {
            var completeConfiguration = new InTestConfigurationLoader().GetConfiguration();
            var expectedResult = new TestFiles().GetSampleFile("sample.debug.cfg.expected.result.txt")
                .UseAsArgFor(File.ReadAllText);

            using (var container = new ContainerBuilder().With(completeConfiguration).Validated().Build())
            {
                var translationRootTargetTypes = container
                    .GetInstance<ITargetTypesLocator>()
                    .LocateRootTargets()
                    .ToList();

                var translationContext = container.GetInstance<ITranslationContext>();

                var skipRule = container.GetInstance<ISkipRule>();

                // IoC vvvvvvvvvvvvvvvvvvvvvv
                foreach (var sourceType in translationRootTargetTypes)
                    if (skipRule.AppliesTo(sourceType) == false)
                        translationContext.AddTypeTranslationContextForType(sourceType);
                ITypeTranslationContext unprocessed;
                Func<ITypeTranslationContext, bool> withUnresolvedDependencies =
                    typeContext => typeContext.AreDependenciesResolved == false;
                while ((unprocessed = translationContext.FirstOrDefault(withUnresolvedDependencies)) != null)
                    unprocessed.ResolveDependencies();
                // IoC ^^^^^^^^^^^^^^^^^^^^^^^

                var nonemptyGenerationResults = translationContext
                    .TranslateTargets()
                    .Where(translationResult => string.IsNullOrWhiteSpace(translationResult.Definition) == false);

                var writer = new InMemoryWriter();
                writer.Write(nonemptyGenerationResults);
                Assert.Equal(Canonic(expectedResult), Canonic(writer.GeneratedText));
            }
        }

        public string Canonic(string text)
        {
            return text
                .Replace("\n", "")
                .Replace("\r", "");
        }
    }
}
