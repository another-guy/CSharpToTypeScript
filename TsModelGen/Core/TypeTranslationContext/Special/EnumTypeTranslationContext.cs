using System;
using System.Reflection;
using System.Linq;
using System.Text;

namespace TsModelGen.Core.TypeTranslationContext.Special
{
    public class EnumTypeTranslationContext : ITypeTranslationContext
    {
        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type specificEnumType)
        {
            var symbol = $"{specificEnumType.Name}Generated"; // TODO Replace with symbol generation rule (from global context)

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

            sb.Append(TypeScriptExpression.EnumNameExpression(symbol));
            sb.Append(TypeScriptExpression.BlockBegin());

            var typeInfo = specificEnumType.GetTypeInfo();
            typeInfo
                .GetEnumValues()
                .OfType<object>()
                .Select(enumValue =>
                {
                    var memberName = typeInfo.GetEnumName(enumValue);
                    var memberValue = Convert.ChangeType(enumValue, typeInfo.GetEnumUnderlyingType());
                    return TypeScriptExpression.EnumMemberExpression(memberName, memberValue);
                })
                .Aggregate(TypeScriptExpression.CommaSeparatedLines)
                .UseAsArgFor(body => sb.Append(body));

            sb.Append(TypeScriptExpression.BlockEnd());

            return sb.ToString();
        }
    }
}
