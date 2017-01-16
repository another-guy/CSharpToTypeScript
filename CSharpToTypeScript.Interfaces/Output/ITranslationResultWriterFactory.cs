using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Output
{
    public interface ITranslationResultWriterFactory
    {
        ITranslationResultWriter GetWriterFor(OutputMode outputMode);
    }
}
