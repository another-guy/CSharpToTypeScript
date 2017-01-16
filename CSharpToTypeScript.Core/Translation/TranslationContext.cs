using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.Core.Translation.Rules.Regular;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TranslationContext : ITranslationContext
    {
        private ITypeScriptExpression Expression { get; }
        private CompleteConfiguration Configuration { get; }
        public InputConfiguration InputConfiguration => Configuration.Input;
        public OutputConfiguration OutputConfiguration => Configuration.Output;
        public TranslationConfiguration TranslationConfiguration => Configuration.Translation;

        // TODO The right way of doing that is using a Graph data structure.
        // Naive list consumption can't guarantee precedence of parent types.
        public IList<TypeInfo> OrderedTargetTypes { get; } = // TODO Make it immutable for clients
            new List<TypeInfo>();

        // TODO EXPOSE TO CLIENTS AS AN OBJECT -- Make this dynamic -- let clients alter the chain to fit their need
        public IList<ITypeTranslationContext> TranslationChain { get; } = new List<ITypeTranslationContext>();
        
        public TranslationContext(ITypeScriptExpression expression, CompleteConfiguration configuration)
        {
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            Configuration = configuration.NullToException(new ArgumentNullException(nameof(configuration)));
        }

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }
        
        public void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext, bool inOrdered)
        {
            if (inOrdered)
                OrderedTargetTypes.Insert(0, (typeTranslationContext as RegularTypeTranslationContext).TypeInfo);

            TranslationChain.Add(typeTranslationContext);
        }

        public ITypeTranslationContext GetByType(Type type)
        {
            return this.First(typeTranslationContext => typeTranslationContext.CanProcess(type));
        }

        public IEnumerator<ITypeTranslationContext> GetEnumerator()
        {
            return TranslationChain.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerable<ITranslationResult> TranslateTargets()
        {
            return OrderedTargetTypes
                .Select(targetType =>
                {
                    var definition =
                        this.First(typeTranslationContext => typeTranslationContext.CanProcess(targetType.AsType()))
                            .Process(targetType.AsType())
                            .Definition;
                    return new TranslationResult(targetType, definition);
                });
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