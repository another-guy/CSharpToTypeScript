using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using CSharpToTypeScript.Core.Common;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public class GenericTypeTranslationContext : ITypeTranslationContext, ITypeBoundTranslationContext
    {
        private ITranslatedTypeMetadataFactory TranslatedTypeMetadataFactory { get; }
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        private ISourceTypeMetadataFactory SourceTypeMetadataFactory { get; }
        private ISourceTypeMetadata SourceTypeMetadata { get; }
        private ITranslationContext TranslationContext { get; }
        private ISkipTypeRule SkipTypeRule { get; }
        private ITypeScriptExpression Expression { get; }
        private ISymbolNamer SymbolNamer { get; }
        private ICommenter Commenter { get; }
        public TypeInfo TypeInfo { get; }
        private IDiscoveredTypeRegistrator DiscoveredTypeRegistrator { get; }

        public GenericTypeTranslationContext(
            IDiscoveredTypeRegistrator discoveredTypeRegistrator,
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            ISourceTypeMetadataFactory sourceTypeMetadataFactory,
            ITranslationContext translationContext,
            ISkipTypeRule skipTypeRule,
            ITypeScriptExpression expression,
            ISymbolNamer symbolNamer,
            ICommenter commenter,
            TypeInfo typeInfo)
        {
            DiscoveredTypeRegistrator = discoveredTypeRegistrator.NullToException(new ArgumentNullException(nameof(discoveredTypeRegistrator)));

            TranslatedTypeMetadataFactory = translatedTypeMetadataFactory
                .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)));

            TranslatedTypeMetadata = TranslatedTypeMetadataFactory.CreateNew();

            SourceTypeMetadataFactory = sourceTypeMetadataFactory
                    .NullToException(new ArgumentNullException(nameof(sourceTypeMetadataFactory)));

            SourceTypeMetadata = sourceTypeMetadataFactory.CreateNew();

            TranslationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            SkipTypeRule = skipTypeRule.NullToException(new ArgumentNullException(nameof(skipTypeRule)));
            Expression = expression.NullToException(new ArgumentNullException(nameof(expression)));
            SymbolNamer = symbolNamer.NullToException(new ArgumentNullException(nameof(symbolNamer)));
            Commenter = commenter.NullToException(new ArgumentNullException(nameof(commenter)));
            TypeInfo = typeInfo.NullToException(new ArgumentNullException(nameof(typeInfo)));
        }

        public bool AreDependenciesResolved { get; private set; } = false;

        // TODO Copy-paste in RegularTypeTranslationContext
        public void ResolveDependencies()
        {
            AreDependenciesResolved = true;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var propertyInfo in TypeInfo.GetProperties(flags))
            {
                SourceTypeMetadata[propertyInfo.Name] = propertyInfo;
                EnsureTypeWillBeResolved(propertyInfo.GetPropertyTypeInfoSafe());
            }

            foreach (var fieldInfo in TypeInfo.GetFields(flags))
            {
                SourceTypeMetadata[fieldInfo.Name] = fieldInfo;
                EnsureTypeWillBeResolved(fieldInfo.GetFieldTypeInfoSafe());
            }

            { 
                // TODO Think if this code can be rewritten so that is does not rely on specific types
                var baseType = TypeInfo.BaseType;
                if (baseType != null &&
                    baseType != typeof(object) &&
                    baseType != typeof(ValueType) &&
                    baseType != typeof(Enum))
                {
                    var parentTypeInfo = baseType.GetTypeInfo();
                    SourceTypeMetadata.BaseType = parentTypeInfo;
                    EnsureTypeWillBeResolved(parentTypeInfo);
                }
            }
        }

        private void EnsureTypeWillBeResolved(TypeInfo typeInfo)
        {
            DiscoveredTypeRegistrator.RegisterType(typeInfo);
        }

        public bool CanProcess(Type type)
        {
            return TypeInfo.AsType() == type;
        }

        public bool IsProcessed { get; private set; } = true;

        public ITranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(CanProcess(specificTargetType));
            var genericArgumentTypes = specificTargetType.GetGenericArguments();

            IsProcessed = true;

            TranslatedTypeMetadata.Symbol = SymbolNamer.GetNameFor(TypeInfo, genericArgumentTypes);

            var sb = new StringBuilder();

            // TODO Class case only now, think of interfaces
            sb.Append(Commenter.TypeCommentFor(TypeInfo));
            sb.Append(Expression.NewLine());
            sb.Append(Expression.ClassNameExpression(TranslatedTypeMetadata.Symbol));
            if (SourceTypeMetadata.BaseType != null)
            {
                var baseTypeType = SourceTypeMetadata.BaseType.AsType();

                var typeTranslationContext = TranslationContext.GetByType(baseTypeType);
                var translatedBaseTypeMetadata = typeTranslationContext.Process(baseTypeType);

                var baseTypeSymbol = translatedBaseTypeMetadata.Symbol;
                sb.Append(Expression.InheritedClassExpression(baseTypeSymbol));
            }

            sb.Append(Expression.BlockBegin());

            foreach (var memberName in SourceTypeMetadata.Members)
            {
                var sourceMemberInfo = SourceTypeMetadata[memberName];
                if (SkipTypeRule.AppliesTo(sourceMemberInfo))
                    continue;

                var type = ((sourceMemberInfo as PropertyInfo)?.PropertyType)
                    .NullTo((sourceMemberInfo as FieldInfo)?.FieldType);
                Debug.Assert(type != null, $"sourceMemberInfo is supposed to be either a PropertyInfo or FieldInfo but was {sourceMemberInfo.GetType()}");

                var memberTypeTranslationContext = TranslationContext.GetByType(type);
                var translatedMemberTypeMetadata = memberTypeTranslationContext.Process(type); // TODO Process is not needed as a part of Interface!!!

                var sourceTypeComment = Commenter.TypeCommentFor(type.GetTypeInfo());
                sb.Append(Expression.MemberDefinitionExpression(sourceMemberInfo.Name, translatedMemberTypeMetadata.Symbol, sourceTypeComment));
            }

            sb.Append(Expression.BlockEnd());

            TranslatedTypeMetadata.Definition = sb.ToString();

            return TranslatedTypeMetadata;
        }
    }
}
