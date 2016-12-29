using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            
            var targetNamespaces = new[] { targetNameSpace }; // TODO Make a parameter
            var translationTargetTypes = TargetTypes.From(targetNamespaces);
            
            var translationContext = new TranslationContextBuilder().Build(translationTargetTypes);

            // TODO Target types to use for iteration instead of RegularTypeTranslationContext
            var generatedCode = translationContext
                .OfType<RegularTypeTranslationContext>()
                .Select(typeContext => typeContext.Process(typeContext.TypeInfo.AsType()).Definition)
                .Where(definition => string.IsNullOrWhiteSpace(definition) == false)
                .Aggregate((accumulated, typeDefinition) => accumulated + "\n" + typeDefinition);

            Console.WriteLine(generatedCode);
            Console.ReadKey();
        }
    }

    public static class TargetTypes
    {
        public static IEnumerable<TypeInfo> From(IEnumerable<string> namespaces)
        {
            return RootTypesFromAllAssemblies()
                .Where(TypeIsFromTargetNamespace(namespaces));
        }

        private static Func<TypeInfo, bool> TypeIsFromTargetNamespace(IEnumerable<string> namespaces)
        {
            return typeInfo =>
               namespaces.Any(n => typeInfo.Namespace.StartsWith(n));
        }

        private static IEnumerable<TypeInfo> RootTypesFromAllAssemblies()
        {
            var emptyResult = (IEnumerable<TypeInfo>)new List<TypeInfo>();
            return new[] {Assembly.GetEntryAssembly().DefinedTypes} // TODO More assemblies to load here
                .Aggregate(emptyResult, (result, types) => result.Concat(types));
        }
    }
}