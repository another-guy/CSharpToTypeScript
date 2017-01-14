using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CaseExtensions;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Output
{
    public sealed class FlatDirectoryResultWriter : ITranslationResultWriter
    {
        private OutputConfiguration OutputConfiguration { get; }

        public FlatDirectoryResultWriter(OutputConfiguration outputConfiguration)
        {
            OutputConfiguration = outputConfiguration.NullToException(new ArgumentNullException(nameof(outputConfiguration)));
        }

        public void Write(IEnumerable<ITranslationResult> translationResults)
        {
            var location = OutputConfiguration.Location;
            var absoluteLocation = Path.GetFullPath(location);
            var directoryInfo = new DirectoryInfo(absoluteLocation);
            var looksLikeDirectory = directoryInfo
                .Extension
                .IsNullOrWhiteSpace();

            if (looksLikeDirectory == false)
                throw new InvalidOperationException($"{location} ({absoluteLocation}) in not a path of a directory.");

            if (directoryInfo.Exists == false)
                directoryInfo.Create();

            foreach (var result in translationResults)
            {
                var fileName = $"{result.TranslatedType.Name.ToKebabCase()}.ts";
                var path = Path.Combine(absoluteLocation, fileName);

                using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
                using (var fileWriter = new StreamWriter(fileStream, Encoding.UTF8))
                    foreach (var definition in result.Definition)
                        fileWriter.WriteLine(definition);
            }
        }
    }
}