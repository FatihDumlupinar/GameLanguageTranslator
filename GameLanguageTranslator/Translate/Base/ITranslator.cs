namespace GameLanguageTranslator.Translate;

public interface ITranslator
{
    Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr");
}
