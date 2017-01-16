using System.Collections.Generic;
using System.Text;
using CSharpToTypeScript.Core.Common;

namespace CSharpToTypeScript.Core.Output
{
    public sealed class InMemoryWriter : ITranslationResultWriter
    {
        private StringBuilder StringBuilder { get; } = new StringBuilder();

        public string GeneratedText => StringBuilder.ToString();

        public void Write(IEnumerable<ITranslationResult> translationResults)
        {
            translationResults
                .ForEach(result => StringBuilder.AppendLine(result.Definition));
        }
    }
}
