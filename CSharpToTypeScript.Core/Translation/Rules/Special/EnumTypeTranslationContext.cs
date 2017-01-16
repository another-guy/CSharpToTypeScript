using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class EnumTypeTranslationContext : ITypeTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        private ITranslationContext TranslationContext { get; }
        private ITypeScriptExpression Expression { get; }

        public EnumTypeTranslationContext(
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ITranslationContext translationContext,
            ITypeScriptExpression expression)
        {
            TranslatedTypeMetadata =
                translatedTypeMetadataFactory
                    .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)))
                    .CreateNew();

            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));

            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
        }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        public bool IsProcessed => true;

        public ITranslatedTypeMetadata Process(Type specificEnumType)
        {
            Debug.Assert(CanProcess(specificEnumType));

            var symbol = TranslationContext.SymbolFor(specificEnumType.Name);

            var definition = GetDefinitionForEnum(symbol, specificEnumType);

            TranslatedTypeMetadata.Symbol = symbol;
            TranslatedTypeMetadata.Definition = definition;
            return TranslatedTypeMetadata;
        }

        private string GetDefinitionForEnum(string symbol, Type specificEnumType)
        {
            var sb = new StringBuilder();
            
            sb.Append(TranslationContext.TypeCommentFor(specificEnumType.GetTypeInfo()));
            sb.Append(Expression.NewLine());
            sb.Append(Expression.EnumNameExpression(symbol));
            sb.Append(Expression.BlockBegin());

            var typeInfo = specificEnumType.GetTypeInfo();
            var declarationElements = typeInfo
                .GetEnumValues()
                .OfType<object>()
                .Select(enumValue =>
                {
                    var memberName = typeInfo.GetEnumName(enumValue);
                    var memberValue = Convert.ChangeType(enumValue, typeInfo.GetEnumUnderlyingType());
                    return Expression.EnumMemberExpression(memberName, memberValue);
                })
                .ToArray();

            for (var currentElementIndex = 0; currentElementIndex < declarationElements.Length; currentElementIndex++)
            {
                sb.Append(declarationElements[currentElementIndex]);
                if (currentElementIndex != declarationElements.Length - 1)
                    sb.AppendLine(Expression.CommaSeparator());
                else
                    sb.AppendLine();
            }

            sb.Append(Expression.BlockEnd());

            return sb.ToString();
        }
    }
}
