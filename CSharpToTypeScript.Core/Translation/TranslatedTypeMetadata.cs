namespace CSharpToTypeScript.Core.Translation
{
    public sealed class TranslatedTypeMetadata : ITranslatedTypeMetadata
    {
        public string Symbol { get; set; }
        public string Definition { get; set; }
    }
}