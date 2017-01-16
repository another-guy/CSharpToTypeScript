using System;
using System.Collections.Generic;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Translation.Rules;

namespace CSharpToTypeScript.Core.Translation
{
    // TODO This is a huge interface. Consider splitting!
    public interface ITranslationContext : IEnumerable<ITypeTranslationContext>
    {
        // TODO mark here AND in implementations [Pure]
        string TypeCommentFor(TypeInfo typeInfo);
        
        void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext, bool inOrdered);
        bool CanProcess(TypeInfo typeInfo);
        ITypeTranslationContext GetByType(Type type);


        InputConfiguration InputConfiguration { get; }
        OutputConfiguration OutputConfiguration { get; }
        TranslationConfiguration TranslationConfiguration { get; }

        IEnumerable<ITranslationResult> TranslateTargets();

        IList<TypeInfo> OrderedTargetTypes { get; }

        IList<ITypeTranslationContext> TranslationChain { get; }
    }
}
