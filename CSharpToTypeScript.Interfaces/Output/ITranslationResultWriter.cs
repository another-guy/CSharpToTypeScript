using System.Collections.Generic;
using CSharpToTypeScript.Core.Common;

namespace CSharpToTypeScript.Core.Output
{
    public interface ITranslationResultWriter
    {
        void Write(IEnumerable<ITranslationResult> translationResults);
    }
}
