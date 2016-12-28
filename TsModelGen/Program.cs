using System;
using TsModelGen.Core;
using TsModelGen.Core.Targets;

namespace TsModelGen
{
    public class Program
    {

        public static void Main(string[] args)
        {
            // TODO Move this to input parameters
            // TODO Translate into a list of namespaces, types, rules on types (such as 
            var targetNameSpace = "TsModelGen.TargetNamespace";

            var translationContext = new TranslationContextBuilder().Build(targetNameSpace);

            foreach (var typeTranslationContext in translationContext)
                if (typeTranslationContext.IsProcessed == false)
                    typeTranslationContext.Process();

            // var generatedText = new DotNetToTypeScript(new[] { targetNameSpace }).Translate();
            // Console.WriteLine(generatedText);
            Console.ReadKey();
        }
    }
}