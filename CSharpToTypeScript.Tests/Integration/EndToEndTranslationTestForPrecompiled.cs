using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Output;
using CSharpToTypeScript.Core.Translation;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.SimpleInjector;
using Xunit;

namespace CSharpToTypeScript.Tests.Integration
{
    public sealed class EndToEndTranslationTestForPrecompiled
    {
        [Fact]
        public void CorrectlyTranslatesDemoTargetAssemblies()
        {
            var completeConfiguration = new InTestConfigurationLoader().GetConfiguration("precompiled.sample.debug.cfg.json");
            var expectedResult = new TestFilesAccessor()
                .GetSampleFile("precompiled.sample.debug.cfg.expected.result.txt")
                .UseAsArgFor(File.ReadAllText);

            using (var container = new ContainerBuilder().With(completeConfiguration).Validated().Build())
            {
                var translationRootTargetTypes = container
                    .GetInstance<ITargetTypesLocator>()
                    .LocateRootTargets()
                    .ToList();

                var translationContext = container.GetInstance<ITranslationContext>();
                
                container
                    .GetInstance<TypeTranslationChain>()
                    .BuildDefault()
                    .ForEach(typeTranslationContext => translationContext.AddTypeTranslationContext(typeTranslationContext, false));

                var skipTypeRule = container.GetInstance<ISkipTypeRule>();
                var typeTranslationContextFactory = container.GetInstance<ITypeTranslationContextFactory>();
                // TODO Move to class
                foreach (var sourceType in translationRootTargetTypes)
                    if (skipTypeRule.AppliesTo(sourceType) == false)
                        translationContext.AddTypeTranslationContext(typeTranslationContextFactory.Regular(sourceType), true);
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
