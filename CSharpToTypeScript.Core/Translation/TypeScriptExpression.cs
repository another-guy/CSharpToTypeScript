namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TypeScriptExpression : ITypeScriptExpression
    {
        public string ClassNameExpression(string generatedTypeName)
        {
            return $"export class {generatedTypeName} ";
        }

        public string EnumNameExpression(string generatedTypeName)
        {
            return $"export enum {generatedTypeName} ";
        }

        public string InheritedClassExpression(string parentClassName)
        {
            return $"extends {parentClassName} ";
        }

        public string BlockBegin()
        {
            return NewLine("{");
        }

        public string MemberDefinitionExpression(string memberName, string memberType, string sourceTypeComment)
        {
            var commentSuffix = string.IsNullOrWhiteSpace(sourceTypeComment) ? "" : $" {sourceTypeComment}";
            return NewLine(Tab($"public {memberName}: {memberType};{commentSuffix}"));
        }

        public string BlockEnd()
        {
            return NewLine("}");
        }

        public string EnumMemberExpression(string memberName, object memberValue)
        {
            return Tab($"{memberName} = {memberValue}");
        }

        public string UntypedArray()
        {
            return GenericArrayOf(Any());
        }

        public string GenericArrayOf(string genericArgumentTranslatesSymbol)
        {
            return $"{genericArgumentTranslatesSymbol}[]";
        }

        public string UntypedDictionary()
        {
            return GenericDictionaryOf(String(), Any());
        }

        public string GenericDictionaryOf(string translatedKeySymbol, string translatedValueSymbol)
        {
            return $"{{ [id: {translatedKeySymbol}]: {translatedValueSymbol}; }}";
        }

        public string Any()
        {
            return "any";
        }

        public string String()
        {
            return "string";
        }

        public string Date()
        {
            return "Date";
        }

        public string Number()
        {
            return "number";
        }

        public string Bool()
        {
            return "boolean";
        }

        public string CommaSeparator()
        {
            return ",";
        }

        public string Tab(string text)
        {
            return $"  {text}";
        }

        public string NewLine(string text = "")
        {
            return $"{text}\n";
        }

        public string SingleLineComment(string text)
        {
            return $"// {text}";
        }
    }
}