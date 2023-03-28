using GameLanguageTranslator.Selenium.Base;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace GameLanguageTranslator.Selenium;

public class BingTranslateSeleniumBot : BaseSelenium
{
    public override async Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr")
    {
        if (text.Length <= 150 && cache.TryGet(text.ToLower(), out string? cachedTranslation))
        {
            if (!string.IsNullOrEmpty(cachedTranslation))
            {
                return cachedTranslation;
            }
        }

        _driver.Url = "https://www.bing.com/translator";

        var fromLanguage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tta_srcsl")));
        fromLanguage.SendKeys(sourceLanguageCode); 

        var toLanguage = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tta_tgtsl")));
        toLanguage.SendKeys(targetLanguageCode); 

        var inputBox = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tta_input_ta")));

        inputBox.SendKeys(text);
        inputBox.SendKeys(Keys.Enter);

        //her seferinde bekleme süresini değiştir
        int randomNumber = rnd.Next(1000, 2001);

        await Task.Delay(randomNumber);

        var result = _wait.Until(ExpectedConditions.ElementIsVisible(By.Id("tta_output_ta")));

        var translatedText = result.Text;

        //150 karakter sonrasını cache atma
        if (text.Length <= 150)
        {
            cache.Add(text.ToLower(), translatedText);
        }

        return translatedText;
    }
}
