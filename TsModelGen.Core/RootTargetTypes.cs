using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TsModelGen
{
    public static class RootTargetTypes
    {
        public static IEnumerable<TypeInfo> LocateFrom(IEnumerable<string> namespaces)
        {
            return RootTypesFromAllAssemblies()
                .Where(TypeIsFromTargetNamespace(namespaces));
        }

        private static IEnumerable<TypeInfo> RootTypesFromAllAssemblies()
        {
            var emptyResult = (IEnumerable<TypeInfo>)new List<TypeInfo>();
            return new[] {Assembly.GetEntryAssembly().DefinedTypes} // TODO More assemblies to load here
                .Aggregate(emptyResult, (result, types) => result.Concat(types));
        }

        private static Func<TypeInfo, bool> TypeIsFromTargetNamespace(IEnumerable<string> namespaces)
        {
            return typeInfo =>
                    namespaces.Any(n => typeInfo.Namespace.StartsWith(n));
        }
    }
}