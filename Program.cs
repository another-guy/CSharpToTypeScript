using System;
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

            var generatedText = new DotNetToTypeScript().Translate(new[] {targetNameSpace});

            Console.WriteLine(generatedText);
            Console.ReadKey();
        }
    }
}