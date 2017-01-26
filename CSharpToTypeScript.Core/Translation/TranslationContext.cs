using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Translation.Rules;
using Z.Graphs;

namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TranslationContext : ITranslationContext
    {
        private readonly OrGraph<TypeInfo, bool> _typeInfoDependencyGraph = new OrGraph<TypeInfo, bool>();
        public IList<TypeInfo> OrderedTargetTypes
        {
            get
            {
                var orderedTargetTypes = new TopSort()
                    .Run(_typeInfoDependencyGraph)
                    .Select(vertex => vertex.Key)
                    .ToList();
                return orderedTargetTypes;
            }
        }

        public void RegisterDependency(TypeInfo dependentType, TypeInfo dependency)
        {
            var dependentTypeVertex = _typeInfoDependencyGraph.Vertices.SingleOrDefault(v => v.Key == dependentType);
            if (dependentTypeVertex == null)
                dependentTypeVertex = _typeInfoDependencyGraph.AddVertex(dependentType);

            if (dependency != null)
            {
                var dependencyVertex = _typeInfoDependencyGraph.Vertices.SingleOrDefault(v => v.Key == dependency);
                if (dependencyVertex == null)
                    dependencyVertex = _typeInfoDependencyGraph.AddVertex(dependency);

                if (_typeInfoDependencyGraph.Edges.Any() == false)
                    _typeInfoDependencyGraph.AddEdge(dependencyVertex, dependentTypeVertex, true);
            }
        }

        // TODO EXPOSE TO CLIENTS AS AN OBJECT -- Make this dynamic -- let clients alter the chain to fit their need
        public IList<ITypeTranslationContext> TranslationChain { get; } = new List<ITypeTranslationContext>();

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }
        
        public void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext)
        {
            TranslationChain.Add(typeTranslationContext);
        }

        public ITypeTranslationContext GetTranslationContextFor(Type type)
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