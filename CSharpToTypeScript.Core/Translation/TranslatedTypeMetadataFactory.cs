namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TranslatedTypeMetadataFactory : ITranslatedTypeMetadataFactory
    {
        public ITranslatedTypeMetadata CreateNew()
        {
            return new TranslatedTypeMetadata();
        }
    }
}
