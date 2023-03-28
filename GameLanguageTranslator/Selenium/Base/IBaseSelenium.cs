namespace GameLanguageTranslator.Selenium.Base;

public interface IBaseSelenium : IDisposable
{
    Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr");
    void Quit();
}
