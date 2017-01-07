using System.Collections.Generic;

namespace CSharpToTypeScript.Core.Output
{
    public interface ITranslationResultWriter
    {
        void Write(IEnumerable<TranslationResult> translationResults);
    }
}
