using System;

namespace TsModelGen.Core.TypeTranslationContext
{
    public interface ITypeTranslationContext
    {
        bool AreDependenciesResolved { get; }
        void ResolveDependencies();

        bool CanProcess(Type type);
        bool IsProcessed { get; }
        TranslatedTypeMetadata Process(Type specificTargetType);
    }
}