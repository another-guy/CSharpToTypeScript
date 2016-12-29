using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TsModelGen.Core.Targets
{
    public interface ITypeRegistry
    {
        bool CanProcess(TypeInfo typeInfo);
        void AddTypeTranslationContextForType(TypeInfo typeInfo);
        void AddTypeTranslationContext(ITypeTranslationContext specialTypeProcessor);
    }

    public interface ITypeTranslationEnumerable : IEnumerable<ITypeTranslationContext> { }

    public sealed class TranslationContextBuilder
    {
        private static readonly Func<ITypeTranslationContext, bool> WithUnresolvedDependencies =
            typeContext => typeContext.AreDependenciesResolved == false;

        public TranslationContext Build(IEnumerable<TypeInfo> translationTargetTypes)
        {
            var translationContext = CreateTranslationContext(translationTargetTypes);

            ITypeTranslationContext unprocessed;
            while ((unprocessed = translationContext.FirstOrDefault(WithUnresolvedDependencies)) != null)
                unprocessed.ResolveDependencies();

            return translationContext;
        }

        private TranslationContext CreateTranslationContext(IEnumerable<TypeInfo> translationTargetTypes)
        {
            var translationContext = new TranslationContext();
            foreach (var sourceType in translationTargetTypes)
                translationContext.AddTypeTranslationContextForType(sourceType);
            return translationContext;
        }

    }

    public sealed class TranslationContext : ITypeRegistry, ITypeTranslationEnumerable
    {
        // TODO Make this dynamic -- let clients alter the chain to fit their need
        private IList<ITypeTranslationContext> TranslationChain { get; } =
            new List<ITypeTranslationContext>();

        public TranslationContext()
        {
            foreach (var rule in DirectMapping.DirectTranslationRules)
                AddTypeTranslationContext(new DirectTypeTranslationContext(rule.SourceType.GetTypeInfo(), rule.DestinationType));

            foreach (var specialTypeProcessor in SpecialTypes.AllProcessors)
                AddTypeTranslationContext(specialTypeProcessor);
        }

        public bool CanProcess(TypeInfo typeInfo)
        {
            return TranslationChain
                .Any(typeTranslationContext => typeTranslationContext.CanProcess(typeInfo.AsType()));
        }

        public void AddTypeTranslationContextForType(TypeInfo typeInfo)
        {
            AddTypeTranslationContext(new RegularTypeTranslationContext(this, typeInfo));
        }

        public void AddTypeTranslationContext(ITypeTranslationContext typeTranslationContext)
        {
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
    }

    public interface ITypeTranslationContext
    {
        bool AreDependenciesResolved { get; }
        void ResolveDependencies();

        bool CanProcess(Type type);
        bool IsProcessed { get; }
        TranslatedTypeMetadata Process(Type specificTargetType);
    }

    public sealed class SourceTypeMetadata
    {
        private IDictionary<string, SourceMemberInfo> MembersSourceInfo { get; } =
            new Dictionary<string, SourceMemberInfo>();

        public SourceParentInfo BaseType { get; set; }

        // TODO INTERFACES ???

        public IEnumerable<string> Members => MembersSourceInfo.Keys;

        public SourceMemberInfo this[string memberName]
        {
            get { return MembersSourceInfo[memberName]; }
            set { MembersSourceInfo[memberName] = value; }
        }
    }

    public sealed class SourceMemberInfo
    {
        public MemberInfo MemberInfo { get; }

        public SourceMemberInfo(MemberInfo memberInfo)
        {
            MemberInfo = memberInfo.NullToException(new ArgumentNullException(nameof(memberInfo)));
        }
    }

    public sealed class SourceParentInfo
    {
        public TypeInfo ParentInfo { get; }

        public SourceParentInfo(TypeInfo parentInfo)
        {
            ParentInfo = parentInfo.NullToException(new ArgumentNullException(nameof(parentInfo)));
        }
    }

    public sealed class TranslatedTypeMetadata
    {
        public string Symbol { get; set; }
        public string Definition { get; set; }
    }



    // TODO Clean this up
    public static class DirectMapping
    {
        public static readonly DirectTranslationRule[] DirectTranslationRules =
            // Simple cases:
            // * object -> any
            // * primitive types to their TS direct translations
            {
                new DirectTranslationRule(typeof(object), "any"),
                new DirectTranslationRule(typeof(short), "number"),
                new DirectTranslationRule(typeof(int), "number"),
                new DirectTranslationRule(typeof(long), "number"),
                new DirectTranslationRule(typeof(ushort), "number"),
                new DirectTranslationRule(typeof(uint), "number"),
                new DirectTranslationRule(typeof(ulong), "number"),
                new DirectTranslationRule(typeof(byte), "number"),
                new DirectTranslationRule(typeof(sbyte), "number"),
                new DirectTranslationRule(typeof(float), "number"),
                new DirectTranslationRule(typeof(double), "number"),
                new DirectTranslationRule(typeof(decimal), "number"),
                new DirectTranslationRule(typeof(bool), "boolean"),
                new DirectTranslationRule(typeof(string), "string")

                // {TypeInfoOf<DateTime>(), "boolean"}
                // { char -> ??? },
                // { TimeSpan -> ??? },
            };
    }

    public sealed class DirectTranslationRule
    {
        public Type SourceType { get; }
        public string DestinationType { get; }

        public DirectTranslationRule(Type sourceType, string destinationType)
        {
            SourceType = sourceType.NullToException(new ArgumentNullException(nameof(sourceType)));
            DestinationType = destinationType.NullToException(new ArgumentNullException(nameof(destinationType)));
        }
    }

    public sealed class SpecialTypes
    {
        public static readonly List<ITypeTranslationContext> AllProcessors = new List<ITypeTranslationContext>
        {
            // TODO Replace DummySpecialTranslationType with specific entity type translation object
            new DummySpecialTranslationType(typeof(Enum), "?ENUM?"), // Not ok
            new DummySpecialTranslationType(typeof(ValueType), "?valuetype?"), // Not ok

            new DummySpecialTranslationType(typeof(IDictionary), "any"), // Ok
            new DummySpecialTranslationType(typeof(IDictionary<,>), "any"), // Can be better, if we discover types
            new DummySpecialTranslationType(typeof(IEnumerable), "any[]"), // Not ok, make strongly typed
            new DummySpecialTranslationType(typeof(IEnumerable<>), "any[]"), // Not ok, make strongly typed
            new DummySpecialTranslationType(typeof(Nullable<>), "any") // Not ok, make strongly typed
        };
    }

    public sealed class DirectTypeTranslationContext : ITypeTranslationContext
    {
        public DirectTypeTranslationContext(TypeInfo type, string symbol)
        {
            Type = type.NullToException(new ArgumentNullException(nameof(type)));
            Symbol = symbol.NullToException(new ArgumentNullException(nameof(symbol)));
        }

        public TypeInfo Type { get; }
        private string Symbol { get; }

        public bool AreDependenciesResolved => true;
        public void ResolveDependencies() { }

        public bool CanProcess(Type type)
        {
            return Type.AsType() == type;
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type _)
        {
            return new TranslatedTypeMetadata { Symbol = Symbol };
        }
    }

    public sealed class DummySpecialTranslationType : ITypeTranslationContext
    {
        public DummySpecialTranslationType(Type type, string symbol)
        {
            Type = type.NullToException(new ArgumentNullException(nameof(type))).GetTypeInfo();
            Symbol = symbol.NullToException(new ArgumentNullException(nameof(symbol)));
        }

        public TypeInfo Type { get; }
        private string Symbol { get; }

        public bool AreDependenciesResolved => true;

        public void ResolveDependencies() { }
        public bool CanProcess(Type type)
        {
            return type.IsChildTypeOfPossiblyOpenGeneric(Type.AsType());
        }
        public bool IsProcessed => true;

        public TranslatedTypeMetadata Process(Type _)
        {
            return new TranslatedTypeMetadata { Symbol = Symbol };
        }
    }

    public sealed class RegularTypeTranslationContext : ITypeTranslationContext
    {
        public RegularTypeTranslationContext(TranslationContext translationContext, TypeInfo typeInfo)
        {
            GlobalContext = translationContext.NullToException(new ArgumentNullException(nameof(translationContext)));
            TypeInfo = typeInfo.NullToException(new ArgumentNullException(nameof(typeInfo)));
        }

        public TypeInfo TypeInfo { get; }
        private TranslationContext GlobalContext { get; }
        public string Id => TypeInfo.AssemblyQualifiedName;
        private SourceTypeMetadata SourceTypeMetadata { get; } = new SourceTypeMetadata();

        private readonly TranslatedTypeMetadata _translatedTypeMetadata = new TranslatedTypeMetadata();


        public bool AreDependenciesResolved { get; private set; } = false;

        public void ResolveDependencies()
        {
            AreDependenciesResolved = true;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            foreach (var propertyInfo in TypeInfo.GetProperties(flags))
            {
                SourceTypeMetadata[propertyInfo.Name] = new SourceMemberInfo(propertyInfo);
                EnsureTypeWillBeResolved(propertyInfo.PropertyType.GetTypeInfo());
            }

            foreach (var fieldInfo in TypeInfo.GetFields(flags))
            {
                SourceTypeMetadata[fieldInfo.Name] = new SourceMemberInfo(fieldInfo);
                EnsureTypeWillBeResolved(fieldInfo.FieldType.GetTypeInfo());
            }

            {
                var baseType = TypeInfo.BaseType;
                if (baseType != typeof(object) &&
                    baseType != typeof(ValueType) &&
                    baseType != typeof(Enum))
                {
                    var parentTypeInfo = baseType.GetTypeInfo();
                    SourceTypeMetadata.BaseType = new SourceParentInfo(parentTypeInfo);
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
            if (IsProcessed) // Prevent from reevaluation on reentry in case of circular type references.
                return _translatedTypeMetadata;

            IsProcessed = true;

            _translatedTypeMetadata.Symbol = $"{TypeInfo.Name}Generated"; // TODO Replace with symbol generation rule (from global context)

            var sb = new StringBuilder();

            // TODO Class case only now, think of interfaces
            sb.Append(TypeScriptExpression.ClassNameExpression(_translatedTypeMetadata.Symbol));
            if (SourceTypeMetadata.BaseType != null)
            {
                var baseTypeType = SourceTypeMetadata.BaseType.ParentInfo.AsType();

                var typeTranslationContext = GlobalContext.GetByType(baseTypeType);
                var translatedBaseTypeMetadata = typeTranslationContext.Process(baseTypeType);

                var baseTypeSymbol = translatedBaseTypeMetadata.Symbol;
                sb.Append(TypeScriptExpression.InheritedClassExpression(baseTypeSymbol));
            }

            sb.Append(TypeScriptExpression.StartClassBodyExpression());

            foreach (var memberName in SourceTypeMetadata.Members)
            {
                var sourceMemberInfo = SourceTypeMetadata[memberName];
                
                // TODO only process serializable members unless explicitly requested oterwise

                var name = sourceMemberInfo.MemberInfo.Name;

                var type = ((sourceMemberInfo.MemberInfo as PropertyInfo)?.PropertyType)
                    .NullTo((sourceMemberInfo.MemberInfo as FieldInfo)?.FieldType)
                    .NullToException(new InvalidOperationException("Oooops!!!"));

                var memberTypeTranslationContext = GlobalContext.GetByType(type);
                var translatedMemberTypeMetadata = memberTypeTranslationContext.Process(type); // TODO Process is not needed as a part of Interface!!!

                sb.Append(TypeScriptExpression.MemberDefinitionExpression(name, translatedMemberTypeMetadata.Symbol, type.FullName)); // TODO Addembly qualified name?
            }

            sb.Append(TypeScriptExpression.EndClassBodyExpression());

            _translatedTypeMetadata.Definition = sb.ToString();

            return _translatedTypeMetadata;
        }
    }

    // TODO Use this
    public static class TypeScriptExpression
    {
        public static string ClassNameExpression(string generatedTypeName)
        {
            return $"export class {generatedTypeName} ";
        }

        public static string InheritedClassExpression(string parentClassName)
        {
            return $"extends {parentClassName} ";
        }

        public static string StartClassBodyExpression()
        {
            return "{\n";
        }

        public static string MemberDefinitionExpression(string memberName, string memberType, string sourceType)
        {
            return $"  public {memberName}: {memberType}; // {sourceType}\n";
        }

        public static string EndClassBodyExpression()
        {
            return "}";
        }
    }
}