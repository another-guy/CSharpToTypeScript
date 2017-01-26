using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Output;
using CSharpToTypeScript.Core.Translation;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.SimpleInjector;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSharpToTypeScript
{
    public sealed class Program
    {
        public static void Main(string[] rawArgs)
        {
            var args = ArgumentParser.ParseArguments(rawArgs);

            var configurationPath = Path.GetFullPath(args.ConfigLocation);
            Cli.WriteLine($"Using configuration file from: {configurationPath}", ConsoleColor.Green);

            var configuration = File
                .ReadAllText(configurationPath)
                .UseAsArgFor(JsonConvert.DeserializeObject<CompleteConfiguration>)
                .WithAllPathsRelativeToFile(configurationPath);

            using (var container = new ContainerBuilder().With(configuration).Validated().Build())
            {
                var translationRootTargetTypes = container
                    .GetInstance<ITargetTypesLocator>()
                    .LocateRootTargets()
                    .ToList();

                var translationContext = container.GetInstance<ITranslationContext>();

                container
                    .GetInstance<TypeTranslationChain>()
                    .BuildDefault()
                    .ForEach(typeTranslationContext => translationContext.AddTypeTranslationContext(typeTranslationContext));


                var typeTranslationContextFactory = container.GetInstance<ITypeTranslationContextFactory>();
                var skipTypeRule = container.GetInstance<ISkipTypeRule>();
                // TODO Move to class
                foreach (var sourceType in translationRootTargetTypes)
                    if (skipTypeRule.AppliesTo(sourceType) == false)
                    {
                        // TODO Reuse in other places!!!
                        // TODO Separate class?
                        var typeTranslationContext = sourceType.IsGenericType ?
                            typeTranslationContextFactory.GenericType(sourceType) :
                            typeTranslationContextFactory.Regular(sourceType);
                        translationContext.AddTypeTranslationContext(typeTranslationContext);
                    }
                ITypeTranslationContext unprocessed;
                Func<ITypeTranslationContext, bool> withUnresolvedDependencies =
                    typeContext => typeContext.AreDependenciesResolved == false;
                while ((unprocessed = translationContext.FirstOrDefault(withUnresolvedDependencies)) != null)
                    unprocessed.ResolveDependencies();
                // IoC ^^^^^^^^^^^^^^^^^^^^^^^

                var nonemptyGenerationResults = translationContext
                    .TranslateTargets()
                    .Where(translationResult => string.IsNullOrWhiteSpace(translationResult.Definition) == false);

                Cli.WriteLine($"Writing results to {configuration.Output.Location}", ConsoleColor.Green);
                container
                    .GetInstance<ITranslationResultWriterFactory>()
                    .GetWriterFor(configuration.Output.Mode)
                    .Write(nonemptyGenerationResults);
            }
        }
    }
}
