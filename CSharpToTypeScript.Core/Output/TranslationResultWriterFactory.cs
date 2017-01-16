using System;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Output
{
    public sealed class TranslationResultWriterFactory : ITranslationResultWriterFactory
    {
        private OutputConfiguration OutputConfiguration { get; }

        public TranslationResultWriterFactory(OutputConfiguration outputConfiguration)
        {
            OutputConfiguration = outputConfiguration.NullToException(new ArgumentNullException(nameof(outputConfiguration)));
        }

        public ITranslationResultWriter GetWriterFor(OutputMode outputMode)
        {
            switch (outputMode)
            {
                case OutputMode.SingleFile:
                    return new SingleFileResultWriter(OutputConfiguration);
                case OutputMode.FlatDirectory:
                    return new FlatDirectoryResultWriter(OutputConfiguration);
                case OutputMode.NamespaceHierarchyDirecory:
                    return new NamespaceHierarchyDirecoryResultWriter(OutputConfiguration);
                default:
                    throw new InvalidOperationException($"Output mode {outputMode} is not recognized.");
            }
        }
    }
}
