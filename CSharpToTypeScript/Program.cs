using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Translation;

namespace CSharpToTypeScript
{
    public class Program
    {
        public static void Main(string[] rawArgs)
        {
            var args = ArgumentParser.ParseArguments(rawArgs);

            var path = Path.GetFullPath(args.ConfigLocation);
            Cli.WriteLine($"Using configuration file from: {path}", ConsoleColor.Green);

            var configuration = File
                .ReadAllText(path)
                .UseAsArgFor(JsonConvert.DeserializeObject<CompleteConfiguration>);

            var rootTargetTypes = RootTargetTypes
                .LocateUsingInputConfiguration(configuration);

            var nonemptyGenerationResults = TranslationContext
                .BuildFor(rootTargetTypes, configuration)
                .TranslateTargets()
                .Where(translationResult => string.IsNullOrWhiteSpace(translationResult.Definition) == false);

            Cli.WriteLine($"Writing results to {configuration.Output.Location}", ConsoleColor.Green);
            Console.ReadKey();
        }
    }
}
