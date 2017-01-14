using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Output
{
    public sealed class SingleFileResultWriter : ITranslationResultWriter
    {
        private OutputConfiguration OutputConfiguration { get; }

        public SingleFileResultWriter(OutputConfiguration outputConfiguration)
        {
            OutputConfiguration = outputConfiguration.NullToException(new ArgumentNullException(nameof(outputConfiguration)));
        }

        public void Write(IEnumerable<ITranslationResult> translationResults)
        {
            var location = OutputConfiguration.Location;
            var absoluteLocation = Path.GetFullPath(location);
            var fileInfo = new FileInfo(absoluteLocation);
            var doesNotLookLikeFile = fileInfo
                .Extension
                .IsNullOrWhiteSpace();

            if (doesNotLookLikeFile)
                throw new InvalidOperationException($"{location} ({absoluteLocation}) in not a path of a file with extension.");

            fileInfo.EnsureContainingDirectoryExists();

            using (var fileStream = new FileStream(absoluteLocation, FileMode.Create, FileAccess.Write))
            using (var fileWriter = new StreamWriter(fileStream, Encoding.UTF8))
                foreach (var definition in translationResults.Select(result => result.Definition))
                    fileWriter.WriteLine(definition);
        }
    }
}