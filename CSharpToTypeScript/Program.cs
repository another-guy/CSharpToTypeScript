using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Output;
using CSharpToTypeScript.Core.Translation;

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
                .WithAllPathsRelativeToFile(configurationPath, untouched => untouched);

            var rootTargetTypes = new TargetTypesLocator()
                .LocateRootTargetsUsing(configuration.Input);

            var nonemptyGenerationResults = TranslationContext
                .BuildFor(new TypeScriptExpression(), rootTargetTypes, configuration)
                .TranslateTargets()
                .Where(translationResult => string.IsNullOrWhiteSpace(translationResult.Definition) == false);

            Cli.WriteLine($"Writing results to {configuration.Output.Location}", ConsoleColor.Green);
            Writers
                .GetFor(configuration.Output)
                .Write(nonemptyGenerationResults);
        }
    }
}
