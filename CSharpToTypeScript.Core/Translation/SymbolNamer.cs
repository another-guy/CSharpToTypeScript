using System;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class SymbolNamer : ISymbolNamer
    {
        private TranslationConfiguration TranslationConfiguration { get; }
        private ITranslationContext TranslationContext { get; }

        public SymbolNamer(
            TranslationConfiguration translationConfiguration,
            ITranslationContext translationContext)
        {
            TranslationConfiguration = translationConfiguration.NullToException(new ArgumentNullException(nameof(translationConfiguration)));
            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public string GetNameFor(TypeInfo typeInfo, Type[] genericTypeArguments)
        {
            var genericSymbolParts = typeInfo.Name.Split('`');
            var clearedSymbolBase = genericSymbolParts[0];
            var genericParameterNumber = genericSymbolParts.Length <= 1 ? 0 : int.Parse(genericSymbolParts[1]);
            if (genericParameterNumber != genericTypeArguments.Length)
                throw new InvalidOperationException();

            var genericSuffix =
                genericParameterNumber <= 0
                    ? ""
                    : "<" +
                      genericTypeArguments
                          .Select(targetType =>
                              targetType.IsGenericParameter
                                  ? targetType.Name
                                  : TranslationContext.GetTranslationContextFor(targetType).Process(targetType).Symbol)
                          .Aggregate((result, genericTypeName) => result + ", " + genericTypeName) +
                      ">";

            var symbolRule = TranslationConfiguration.GeneratedSymbols;
            return $"{symbolRule.Prefix}{clearedSymbolBase}{symbolRule.Suffix}{genericSuffix}";
        }
    }
}
