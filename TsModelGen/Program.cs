using System;
using System.Linq;
using clipr;
using clipr.Usage;
using TsModelGen.Core;

namespace TsModelGen
{
    public class Program
    {
        public static void Main(string[] rawArgs)
        {
            var args = ParseArguments(rawArgs);
            if (args == null) return;

            // TODO Move this to input parameters
            // TODO Translate into a list of namespaces, types, rules on types (such as 
            var targetNameSpace = "TsModelGen.TargetNamespace";
            
            var targetNamespaces = new[] { targetNameSpace }; // TODO Make a parameter
            var rootTargetTypes = RootTargetTypes.LocateFrom(targetNamespaces).ToList();

            var generatedDefinitions = TranslationContext.BuildFor(rootTargetTypes).TranslateTargets();
            
            var generatedCode = generatedDefinitions
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .Aggregate((accumulated, typeDefinition) => accumulated + "\n\n" + typeDefinition);

            Console.WriteLine(generatedCode);
            Console.ReadKey();
        }

        private static Arguments ParseArguments(string[] rawArgs)
        {
            var args = new Arguments();
            var parser = new CliParser<Arguments>(args);
            var help = new AutomaticHelpGenerator<Arguments>();
            try
            {
                parser.Parse(rawArgs);
            }
            catch (Exception caught)
            {
                var originalFgColor = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(caught);
                Console.ForegroundColor = originalFgColor;

                Console.WriteLine(help.GetHelp(parser.Config));

                return null;
            }
            return args;
        }
    }
}