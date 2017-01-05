using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core
{
    public static class RootTargetTypes
    {
        public static IEnumerable<TypeInfo> LocateUsingInputConfiguration(CompleteConfiguration configuration)
        {
            var includeRegexes = CreateRegexesFor(configuration.Input.IncludeTypes);
            var excludeRegexes = CreateRegexesFor(configuration.Input.ExcludeTypes);

            return configuration
                .Input
                .Assemblies
                .Select(assemblyPath => TargetTypesBasedOnIncludeExcludeRegexes(assemblyPath, includeRegexes, excludeRegexes))
                .Aggregate(new List<Type>(), (result, newTypes) => result.Concat(newTypes).ToList())
                .Select(type => type.GetTypeInfo());
        }

        private static List<Regex> CreateRegexesFor(IEnumerable<string> patterns)
        {
            return patterns
                .Select(pattern => new Regex(pattern))
                .ToList();
        }

        private static IEnumerable<Type> TargetTypesBasedOnIncludeExcludeRegexes(
            string assemblyPath,
            IReadOnlyCollection<Regex> includeRegexes,
            IReadOnlyCollection<Regex> excludeRegexes)
        {
            var absoluteAssemblyPath = Path.GetFullPath(assemblyPath);
            return AssemblyLoadContext
                .Default
                .LoadFromAssemblyPath(absoluteAssemblyPath)
                .GetTypes()
                .Where(type => IsTargetType(type, includeRegexes, excludeRegexes));
        }

        private static bool IsTargetType(
            Type type,
            IEnumerable<Regex> includeRegexes,
            IEnumerable<Regex> excludeRegexes)
        {
            var inInclude = RegexTargetsType(includeRegexes, type);
            var notInExclude = RegexTargetsType(excludeRegexes, type) == false;
            return inInclude && notInExclude;
        }

        private static bool RegexTargetsType(IEnumerable<Regex> regexes, Type type)
        {
            return regexes.Any(regex => regex.IsMatch(type.FullName));
        }
    }
}