using System;
using System.Diagnostics;
using System.Reflection;

namespace CSharpToTypeScript.Core.Translation.Rules.Special
{
    public sealed class GenericParameterTranslationContext : ITypeTranslationContext, ITypeBoundTranslationContext
    {
        private ITranslatedTypeMetadata TranslatedTypeMetadata { get; }
        public TypeInfo TypeInfo { get; }

        public GenericParameterTranslationContext(
            ITranslatedTypeMetadataFactory translatedTypeMetadataFactory,
            TypeInfo typeInfo)
        {
            TranslatedTypeMetadata = translatedTypeMetadataFactory
                .NullToException(new ArgumentNullException(nameof(translatedTypeMetadataFactory)))
                .CreateNew();

            TypeInfo = typeInfo.NullToException(new ArgumentNullException(nameof(typeInfo)));
        }

        // TODO Naive implementation until generic type constraints are supported
        public bool AreDependenciesResolved => true;

        public void ResolveDependencies()
        {
            throw new NotImplementedException();
        }

        public bool CanProcess(Type type)
        {
            return type == TypeInfo.AsType();
        }

        public bool IsProcessed { get; set; } = false;

        public ITranslatedTypeMetadata Process(Type specificTargetType)
        {
            Debug.Assert(CanProcess(specificTargetType));
            TranslatedTypeMetadata.Symbol = specificTargetType.Name;
            return TranslatedTypeMetadata;
        }
    }
}
