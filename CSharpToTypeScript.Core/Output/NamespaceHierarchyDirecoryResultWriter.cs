using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CaseExtensions;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Output
{
    // TODO Kill previous compilation directories for 'dir' location outputs

    public sealed class NamespaceHierarchyDirecoryResultWriter : ITranslationResultWriter
    {
        private OutputConfiguration OutputConfiguration { get; }

        public NamespaceHierarchyDirecoryResultWriter(OutputConfiguration outputConfiguration)
        {
            OutputConfiguration = outputConfiguration.NullToException(new ArgumentNullException(nameof(outputConfiguration)));
        }

        public void Write(IEnumerable<ITranslationResult> translationResultsEnumerable)
        {
            var baseLocation = OutputConfiguration.Location;
            var baseAbsoluteLocation = Path.GetFullPath(baseLocation);

            // TODO Test location is directory
            // TODO Ensure location exists

            var translationResults = translationResultsEnumerable.ToList();

            var namespaces = translationResults
                .Select(result => result.TranslatedType.Namespace)
                .ToList();
            var commonNamespaceRoot = GetCommonNamespaceRoot(namespaces);
            var charsToTrim = commonNamespaceRoot.Length == 0 ? 0 : commonNamespaceRoot.Length + 1;

            foreach (var result in translationResults)
            {
                var translatedType = result.TranslatedType;
                var pathParts = translatedType
                    .Namespace
                    .TrimFirst(charsToTrim)
                    .Split('.')
                    .Select(subNamespace => subNamespace.ToKebabCase())
                    .ToList();
                pathParts.Insert(0, baseAbsoluteLocation);
                
                var containingDirectoryInfo = new DirectoryInfo(Path.Combine(pathParts.ToArray()));
                if (containingDirectoryInfo.Exists == false)
                    containingDirectoryInfo.Create();

                var fileName = $"{translatedType.Name.ToKebabCase()}.ts";
                var filePath = Path.Combine(containingDirectoryInfo.FullName, fileName);
                
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                using (var fileWriter = new StreamWriter(fileStream, Encoding.UTF8))
                        fileWriter.WriteLine(result.Definition);
            }
        }

        private string GetCommonNamespaceRoot(List<string> namespaces)
        {
            var namespacesSplitByDot = namespaces
                .Select(@namespace => @namespace.Split('.'))
                .ToList();
            var minLen = namespacesSplitByDot
                .Min(splitNamespace => splitNamespace.Length);

            var firstSplitNamespace = namespacesSplitByDot.First();

            var wordIndex = 0;
            for (; wordIndex <= minLen; wordIndex++)
            {
                var word = firstSplitNamespace[wordIndex];
                var same = namespacesSplitByDot
                    .All(@namespace => @namespace[wordIndex] == word);

                if (same == false)
                    break;
            }

            return string.Join(".", firstSplitNamespace.Take(wordIndex));
        }
    }
}