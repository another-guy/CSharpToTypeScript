using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Input
{
    public class TargetTypesLocator : ITargetTypesLocator
    {
        private InputConfiguration InputConfiguration { get; }

        public TargetTypesLocator(InputConfiguration inputConfiguration)
        {
            InputConfiguration = inputConfiguration.NullToException(new ArgumentNullException(nameof(inputConfiguration)));
        }

        [Pure]
        public IEnumerable<TypeInfo> LocateRootTargets()
        {
            return InputConfiguration
                .Assemblies
                .Select(assemblyPath =>
                    TargetTypesBasedOnIncludeExcludeRegexes(
                        assemblyPath,
                        CreateRegexesFor(InputConfiguration.IncludeTypes),
                        CreateRegexesFor(InputConfiguration.ExcludeTypes))
                )
                .Aggregate(new List<Type>(), (result, newTypes) => result.Concat(newTypes).ToList())
                .Select(type => type.GetTypeInfo());
        }

        private static List<Regex> CreateRegexesFor(IEnumerable<string> patterns)
        {
            return patterns
                .Select(pattern => new Regex(pattern))
                .ToList();
        }

        private IEnumerable<Type> TargetTypesBasedOnIncludeExcludeRegexes(
            string assemblyPath,
            IReadOnlyCollection<Regex> includeRegexes,
            IReadOnlyCollection<Regex> excludeRegexes)
        {
            return AssemblyLoadContext
                .Default
                .LoadFromAssemblyPath(Path.GetFullPath(assemblyPath))
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