using System;

namespace CSharpToTypeScript.Core.Translation.Rules
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