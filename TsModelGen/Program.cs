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
            var rootTranslationTargetTypes = RootTargetTypes.LocateFrom(targetNamespaces).ToList();
            
            var translationContext = new TranslationContextBuilder().Build(rootTranslationTargetTypes);

            // TODO Remove this dictionary
            var dictionary = rootTranslationTargetTypes
                .Union(
                    translationContext
                        .OfType<RegularTypeTranslationContext>()
                        .Select(typeTranslationContext => typeTranslationContext.TypeInfo)
                )
                .ToDictionary(
                    targetType => targetType.FullName,
                    targetType => translationContext
                            .First(typeTranslationContext => typeTranslationContext.CanProcess(targetType.AsType())));

            var generatedCode = rootTranslationTargetTypes
                .Union(
                    translationContext
                        .OfType<RegularTypeTranslationContext>()
                        .Select(typeTranslationContext => typeTranslationContext.TypeInfo)
                )
                .Select(targetType =>
                        translationContext
                            .First(typeTranslationContext => typeTranslationContext.CanProcess(targetType.AsType()))
                            .Process(targetType.AsType())
                            .Definition
                )
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .Aggregate((accumulated, typeDefinition) => accumulated + "\n\n" + typeDefinition);

            Console.WriteLine(generatedCode);
            Console.ReadKey();
        }
    }
}