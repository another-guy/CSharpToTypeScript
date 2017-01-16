using System;
using System.Reflection;
using CSharpToTypeScript.Core.Configuration;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class Commenter : ICommenter
    {
        private TranslationConfiguration TranslationConfiguration { get; }
        private ITypeScriptExpression Expression { get; }

        public Commenter(TranslationConfiguration translationConfiguration, ITypeScriptExpression expression)
        {
            TranslationConfiguration = translationConfiguration.NullToException(new ArgumentNullException(nameof(translationConfiguration)));
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
        }

        public string TypeCommentFor(TypeInfo typeInfo)
        {
            string typeRef;
            switch (TranslationConfiguration.SourceTypeReferenceKind)
            {
                case SourceTypeReferenceKind.AssemblyQualifiedName:
                    typeRef = typeInfo.AssemblyQualifiedName;
                    break;
                case SourceTypeReferenceKind.FullName:
                    typeRef = typeInfo.FullName;
                    break;
                case SourceTypeReferenceKind.Name:
                    typeRef = typeInfo.Name;
                    break;
                case SourceTypeReferenceKind.None:
                default:
                    return "";
            }
            return Expression.SingleLineComment(typeRef);
        }
    }
}
