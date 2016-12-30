using System;
using System.Linq;
using System.Reflection;

namespace TsModelGen.Core.TypeTranslationContext.Special
{
    public class NullableTypeTranslationContext : ITypeTranslationContext
    {
        private readonly TranslationContext _translationContext;

        public NullableTypeTranslationContext(TranslationContext translationContext)
        {
            _translationContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
        }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(typeof(Nullable<>));
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type specificEnumType)
        {
            return new TranslatedTypeMetadata
            {
                Symbol = specificEnumType
                    .GetGenericArguments()
                    .Single()
                    .UseAsArgFor(argumentType => _translationContext.GetByType(argumentType).Process(argumentType))
                    .Symbol
            };
        }
    }
}
