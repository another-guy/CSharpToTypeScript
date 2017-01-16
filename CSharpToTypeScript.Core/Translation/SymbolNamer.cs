using System;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class SymbolNamer : ISymbolNamer
    {
        private TranslationConfiguration TranslationConfiguration { get; }

        public SymbolNamer(TranslationConfiguration translationConfiguration)
        {
            TranslationConfiguration = translationConfiguration.NullToException(new ArgumentNullException(nameof(translationConfiguration)));
        }

        public string GetNameFor(string symbolBase)
        {
            var symbolRule = TranslationConfiguration.GeneratedSymbols;
            return $"{symbolRule.Prefix}{symbolBase}{symbolRule.Suffix}";
        }
    }
}
