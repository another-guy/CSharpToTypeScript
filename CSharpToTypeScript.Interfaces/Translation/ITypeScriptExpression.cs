namespace CSharpToTypeScript.Core.Translation
{
    public interface ITypeScriptExpression
    {
        string ClassNameExpression(string generatedTypeName);

        string EnumNameExpression(string generatedTypeName);

        string InheritedClassExpression(string parentClassName);

        string BlockBegin();

        string MemberDefinitionExpression(string memberName, string memberType, string sourceTypeComment);

        string BlockEnd();

        string EnumMemberExpression(string memberName, object memberValue);

        string UntypedArray();

        string GenericArrayOf(string genericArgumentTranslatesSymbol);

        string UntypedDictionary();

        string GenericDictionaryOf(string translatedKeySymbol, string translatedValueSymbol);

        string Any();

        string String();

        string Date();

        string Number();

        string Bool();

        string CommaSeparator();

        string Tab(string text);

        string NewLine(string text = "");

        string SingleLineComment(string text);
    }
}
