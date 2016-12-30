using System;
using System.Collections.Generic;
using System.Linq;
using TsModelGen.Core;
using TsModelGen.Core.TypeTranslationContext;

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
            var translationTargetTypes = RootTargetTypes.LocateFrom(targetNamespaces).ToList();
            
            var translationContext = new TranslationContextBuilder().Build(translationTargetTypes);

            // TODO Target types to use for iteration instead of RegularTypeTranslationContext

            string generatedCode;
            generatedCode = translationTargetTypes
                .Select(targetType =>
                        translationContext
                            .First(typeTranslationContext => typeTranslationContext.CanProcess(targetType.AsType()))
                            .Process(targetType.AsType())
                            .Definition
                )
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .Aggregate((accumulated, typeDefinition) => accumulated + "\n" + typeDefinition);

            //generatedCode = translationContext
            //    .OfType<RegularTypeTranslationContext>()
            //    .Select(typeContext => typeContext.Process(typeContext.TypeInfo.AsType()).Definition)
            //    .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
            //    .Aggregate((accumulated, typeDefinition) => accumulated + "\n" + typeDefinition);

            Console.WriteLine(generatedCode);
            Console.ReadKey();
        }
    }
}