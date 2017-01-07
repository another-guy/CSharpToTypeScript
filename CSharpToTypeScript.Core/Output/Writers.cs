using System;
using System.Collections.Generic;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Output
{
    // TODO Use IOC instead?
    public static class Writers
    {
        private static readonly IDictionary<OutputMode, Func<OutputConfiguration, ITranslationResultWriter>> Registry =
            new Dictionary<OutputMode, Func<OutputConfiguration, ITranslationResultWriter>>
            {
                { OutputMode.SingleFile, outputConfiguration => new SingleFileResultWriter(outputConfiguration) },
                { OutputMode.FlatDirectory, outputConfiguration => new SingleFileResultWriter(outputConfiguration) },
                { OutputMode.NamespaceHierarchyDirecory, outputConfiguration => new NamespaceHierarchyDirecoryResultWriter(outputConfiguration) }
            };

        public static ITranslationResultWriter GetFor(OutputConfiguration configurationOutput)
        {
            return Registry[configurationOutput.Mode](configurationOutput);
        }
    }
}
