using CSharpToTypeScript.Core.Common;
using CSharpToTypeScript.Core.Configuration;
using CSharpToTypeScript.Core.Input;
using CSharpToTypeScript.Core.Output;
using CSharpToTypeScript.Core.Translation;
using CSharpToTypeScript.Core.Translation.Rules;
using SimpleInjector;

namespace CSharpToTypeScript.SimpleInjector
{
    public sealed class ContainerBuilder
    {
        private Container Container { get; }
        private CompleteConfiguration Configuration { get; set; }

        public ContainerBuilder()
        {
            Container = new Container();
        }

        public ContainerBuilder With(CompleteConfiguration configuration)
        {
            Configuration = configuration;

            // Common
            Container.RegisterSingleton<ISkipTypeRule, SkipTypeRule>();

            // Configuration
            Container.RegisterSingleton(configuration);
            Container.RegisterSingleton(() => configuration.Input);
            Container.RegisterSingleton(() => configuration.Translation);
            Container.RegisterSingleton(() => configuration.Output);

            // Input
            Container.RegisterSingleton<ITargetTypesLocator, TargetTypesLocator>();

            // Translation
            Container.RegisterSingleton<ITypeScriptExpression, TypeScriptExpression>();
            Container.RegisterSingleton<ISourceTypeMetadataFactory, SourceTypeMetadataFactory>();
            Container.RegisterSingleton<ITranslatedTypeMetadataFactory, TranslatedTypeMetadataFactory>();
            Container.RegisterSingleton<ITranslationContext, TranslationContext>();
            Container.RegisterSingleton<ITypeTranslationContextFactory, TypeTranslationContextFactory>();
            Container.RegisterSingleton<TypeTranslationChain>(); // This is the cornerstone class, this is why it's registered directly and does not implement an interface.

            // Output
            Container.RegisterSingleton<ITranslationResultWriterFactory, TranslationResultWriterFactory>();

            return this;
        }

        public ContainerBuilder Validated()
        {
            Container.Verify();

            return this;
        }

        public Container Build()
        {
            return Container;
        }
    }
}
