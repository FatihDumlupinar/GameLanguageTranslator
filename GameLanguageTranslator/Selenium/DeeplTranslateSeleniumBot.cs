using GameLanguageTranslator.Selenium.Base;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;

namespace GameLanguageTranslator.Selenium
{
    public class DeeplTranslateSeleniumBot : BaseSelenium
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

            _driver.Url = $"https://www.deepl.com/translator/{sourceLanguageCode}-{targetLanguageCode}";

            // Sayfa yüklenene kadar bekle
            _wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

            // Çerezleri kabul etmek için gerekli elementi bul ve tıkla
            try
            {
                var acceptCookiesButton = _wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//button[@class='dl_cookieBanner--buttonClose']")));
                acceptCookiesButton.Click();
            }
            catch (NoSuchElementException)
            {
                // Çerez kabul düğmesi bulunamadıysa, devam et
            }

            //await Task.Delay(1500);
            
            var inputTextArea = _wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("div#myTextarea")));

            inputTextArea.Clear();
            inputTextArea.SendKeys(text);

            //her seferinde bekleme süresini değiştir
            int randomNumber = rnd.Next(1000, 2001);

            await Task.Delay(randomNumber);

            var translationResult = _wait.Until(ExpectedConditions.ElementToBeClickable(By.CssSelector("div.result__src-wrap > div.lmt__translations_as_text > p.lmt__translations_as_text__text")));

            string translatedText = translationResult.Text;

            //150 karakter sonrasını cache atma
            if (text.Length <= 150)
            {
                cache.Add(text.ToLower(), translatedText);
            }

            return translatedText;
        }
    }
}
