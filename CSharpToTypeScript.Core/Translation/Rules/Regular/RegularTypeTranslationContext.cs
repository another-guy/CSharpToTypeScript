using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class RegularTypeTranslationContext : ITypeTranslationContext
    {
        public RegularTypeTranslationContext(TranslationContext translationContext, TypeInfo typeInfo)
        {
            GlobalContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TypeInfo = typeInfo.NullToException(new ArgumentNullException(nameof(typeInfo)));
        }

        public TypeInfo TypeInfo { get; }
        private TranslationContext GlobalContext { get; }
        private SourceTypeMetadata SourceTypeMetadata { get; } = new SourceTypeMetadata();

        private readonly TranslatedTypeMetadata _translatedTypeMetadata = new TranslatedTypeMetadata();


        public bool AreDependenciesResolved { get; private set; } = false;

        public void ResolveDependencies()
        {
            AreDependenciesResolved = true;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var propertyInfo in TypeInfo.GetProperties(flags))
            {
                SourceTypeMetadata[propertyInfo.Name] = propertyInfo;
                EnsureTypeWillBeResolved(propertyInfo.PropertyType.GetTypeInfo());
            }

            foreach (var fieldInfo in TypeInfo.GetFields(flags))
            {
                SourceTypeMetadata[fieldInfo.Name] = fieldInfo;
                EnsureTypeWillBeResolved(fieldInfo.FieldType.GetTypeInfo());
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
            var noTypeTranslationContextRegistered = GlobalContext.CanProcess(typeInfo) == false;
            if (noTypeTranslationContextRegistered)
                GlobalContext.AddTypeTranslationContextForType(typeInfo);
        }

        public bool CanProcess(Type type)
        {
            return TypeInfo.AsType() == type;
        }

        public bool IsProcessed { get; private set; } = false;

        public TranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(CanProcess(specificTargetType));
            if (IsProcessed) // Prevent from reevaluation on reentry in case of circular type references.
                return _translatedTypeMetadata;

            IsProcessed = true;

            _translatedTypeMetadata.Symbol = GlobalContext.SymbolFor(TypeInfo.Name);

            var sb = new StringBuilder();

            // TODO Class case only now, think of interfaces
            sb.Append(GlobalContext.TypeCommentFor(TypeInfo));
            sb.Append(TypeScriptExpression.NewLine());
            sb.Append(TypeScriptExpression.ClassNameExpression(_translatedTypeMetadata.Symbol));
            if (SourceTypeMetadata.BaseType != null)
            {
                var baseTypeType = SourceTypeMetadata.BaseType.AsType();

                var typeTranslationContext = GlobalContext.GetByType(baseTypeType);
                var translatedBaseTypeMetadata = typeTranslationContext.Process(baseTypeType);

                var baseTypeSymbol = translatedBaseTypeMetadata.Symbol;
                sb.Append(TypeScriptExpression.InheritedClassExpression(baseTypeSymbol));
            }

            sb.Append(TypeScriptExpression.BlockBegin());

            var skipRule = new SkipRule(GlobalContext.InputConfiguration.SkipMembersWithAttribute);
            foreach (var memberName in SourceTypeMetadata.Members)
            {
                var sourceMemberInfo = SourceTypeMetadata[memberName];
                if (skipRule.AppliesTo(sourceMemberInfo))
                    continue;
                
                var type = ((sourceMemberInfo as PropertyInfo)?.PropertyType)
                    .NullTo((sourceMemberInfo as FieldInfo)?.FieldType);
                Debug.Assert(type != null, $"sourceMemberInfo is supposed to be either a PropertyInfo or FieldInfo but was {sourceMemberInfo.GetType()}");

                var memberTypeTranslationContext = GlobalContext.GetByType(type);
                var translatedMemberTypeMetadata = memberTypeTranslationContext.Process(type); // TODO Process is not needed as a part of Interface!!!

                var sourceTypeComment = GlobalContext.TypeCommentFor(type.GetTypeInfo());
                sb.Append(TypeScriptExpression.MemberDefinitionExpression(sourceMemberInfo.Name, translatedMemberTypeMetadata.Symbol, sourceTypeComment));
            }

            sb.Append(TypeScriptExpression.BlockEnd());

            _translatedTypeMetadata.Definition = sb.ToString();

            return _translatedTypeMetadata;
        }
    }
}