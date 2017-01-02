using System;
using System.Collections.Generic;
using System.Linq;
using TsModelGen.Core;

namespace TsModelGen
{
    public class Program
    {
        public static void Main(string[] args)
        {
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
    }
}