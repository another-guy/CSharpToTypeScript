using System;
using System.IO;
using System.Linq;
using CSharpToTypeScript.Core.Common;
using Newtonsoft.Json;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Output;
using CSharpToTypeScript.Core.Translation;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.SimpleInjector;

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
                    .Where(translationResult => string.IsNullOrWhiteSpace(translationResult.Definition) == false)
                    .ToList() // TODO ToList() is a temporary solution
                    ;

                Cli.WriteLine($"Writing results to {configuration.Output.Location}", ConsoleColor.Green);
                Writers
                    .GetFor(configuration.Output)
                    .Write(nonemptyGenerationResults);
            }
        }
    }
}
