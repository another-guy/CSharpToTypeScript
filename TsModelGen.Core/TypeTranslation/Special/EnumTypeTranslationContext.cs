using System;
using System.Diagnostics;
using System.Reflection;
using System.Linq;
using System.Text;

namespace TsModelGen.Core.TypeTranslation.Special
{
    public class EnumTypeTranslationContext : ITypeTranslationContext
    {
        private TranslationContext GlobalContext { get; }

        public EnumTypeTranslationContext(TranslationContext translationContext)
        {
            GlobalContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type specificEnumType)
        {
            Debug.Assert(CanProcess(specificEnumType));

            var symbol = GlobalContext.SymbolFor(specificEnumType.Name);

            var definition = GetDefinitionForEnum(symbol, specificEnumType);

            return new TranslatedTypeMetadata
            {
                Symbol = symbol,
                Definition = definition
            };
        }

        private string GetDefinitionForEnum(string symbol, Type specificEnumType)
        {
            var sb = new StringBuilder();
            
            sb.Append(GlobalContext.TypeCommentFor(specificEnumType.GetTypeInfo()));
            sb.Append(TypeScriptExpression.NewLine());
            sb.Append(TypeScriptExpression.EnumNameExpression(symbol));
            sb.Append(TypeScriptExpression.BlockBegin());

            var typeInfo = specificEnumType.GetTypeInfo();
            var declarationElements = typeInfo
                .GetEnumValues()
                .OfType<object>()
                .Select(enumValue =>
                {
                    var memberName = typeInfo.GetEnumName(enumValue);
                    var memberValue = Convert.ChangeType(enumValue, typeInfo.GetEnumUnderlyingType());
                    return TypeScriptExpression.EnumMemberExpression(memberName, memberValue);
                })
                .ToArray();

            for (var currentElementIndex = 0; currentElementIndex < declarationElements.Length; currentElementIndex++)
            {
                sb.Append(declarationElements[currentElementIndex]);
                if (currentElementIndex != declarationElements.Length - 1)
                    sb.AppendLine(TypeScriptExpression.CommaSeparator());
                else
                    sb.AppendLine();
            }

            sb.Append(TypeScriptExpression.BlockEnd());

            return sb.ToString();
        }
    }
}
