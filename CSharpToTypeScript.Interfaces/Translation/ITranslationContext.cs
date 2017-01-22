using System;
using System.Collections.Generic;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Translation.Rules;

namespace CSharpToTypeScript.Core.Translation
{
    // TODO This is a huge interface. Consider splitting!
    public interface ITranslationContext : IEnumerable<ITypeTranslationContext>
    {
        // TODO mark here AND in implementations [Pure]
        
        void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext, bool inOrdered);
        bool CanProcess(TypeInfo typeInfo);
        ITypeTranslationContext GetByType(Type type);

        

        IEnumerable<ITranslationResult> TranslateTargets();

        IList<TypeInfo> OrderedTargetTypes { get; }

        IList<ITypeTranslationContext> TranslationChain { get; }
    }
}
