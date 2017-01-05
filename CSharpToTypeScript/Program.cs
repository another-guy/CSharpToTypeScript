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
            if (args == null) return;

            var path = Path.GetFullPath(args.ConfigLocation);
            Cli.WriteLine($"Using configuration file from: {path}", ConsoleColor.Green);

            var configuration = File
                .ReadAllText(path)
                .UseAsArgFor(JsonConvert.DeserializeObject<CompleteConfiguration>);

            var rootTargetTypes = RootTargetTypes
                .LocateUsingInputConfiguration(configuration)
                .ToList();

            var generatedDefinitions = TranslationContext
                .BuildFor(rootTargetTypes, configuration)
                .TranslateTargets();

            var generatedCode = generatedDefinitions
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .Aggregate((accumulated, typeDefinition) => accumulated + "\n" + typeDefinition);

            Cli.WriteLine(generatedCode, ConsoleColor.Blue);
            Console.ReadKey();
        }
    }
}
