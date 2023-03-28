using GameLanguageTranslator.Selenium.Base;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;

namespace GameLanguageTranslator.Selenium
{
    public class YandexTranslateSeleniumBot : BaseSelenium
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

            _driver.Url = $"https://ceviri.yandex.com.tr/?lang={sourceLanguageCode}-{targetLanguageCode}&text={WebUtility.UrlEncode(text)}";

            // Kaynak ve hedef dilleri seç
            var sourceLanguageSelector = new SelectElement(_driver.FindElement(By.CssSelector(".select__source .select__control")));
            sourceLanguageSelector.SelectByValue(sourceLanguageCode);

            var targetLanguageSelector = new SelectElement(_driver.FindElement(By.CssSelector(".select__target .select__control")));
            targetLanguageSelector.SelectByValue(targetLanguageCode);

            // Çevrilecek cümleyi gir
            var inputBox = _driver.FindElement(By.CssSelector("textarea[autofocus]"));
            inputBox.Clear();
            inputBox.SendKeys(text);
            inputBox.SendKeys(Keys.Enter);

            //her seferinde bekleme süresini değiştir
            int randomNumber = rnd.Next(1000, 2001);

            await Task.Delay(randomNumber);

            // Çeviri sonucunu al
            var translationResult = _driver.FindElement(By.CssSelector(".translation__wrapped"));
            var translatedText = translationResult.Text;

            //150 karakter sonrasını cache atma
            if (text.Length <= 150)
            {
                cache.Add(text.ToLower(), translatedText);
            }

            return translatedText;
        }
    }
}
