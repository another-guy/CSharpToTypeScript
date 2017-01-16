using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Translation.Rules;
using CSharpToTypeScript.Core.Translation.Rules.Regular;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TranslationContext : ITranslationContext
    {
        // TODO The right way of doing that is using a Graph data structure.
        // Naive list consumption can't guarantee precedence of parent types.
        public IList<TypeInfo> OrderedTargetTypes { get; } = // TODO Make it immutable for clients
            new List<TypeInfo>();

        // TODO EXPOSE TO CLIENTS AS AN OBJECT -- Make this dynamic -- let clients alter the chain to fit their need
        public IList<ITypeTranslationContext> TranslationChain { get; } = new List<ITypeTranslationContext>();

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }


        // TODO Get rid of addToOrderedTargets eventually, when closure builder understands the correct order of type declarations based on dependencies
        public void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext, bool addToOrderedTargets)
        {
            if (addToOrderedTargets)
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
    }
}