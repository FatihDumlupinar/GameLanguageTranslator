using GameLanguageTranslator.Selenium.Base;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using SeleniumExtras.WaitHelpers;
using System.Net;

namespace GameLanguageTranslator.Selenium
{
    public class GoogleTranslateSeleniumBot : BaseSelenium
    {
        public override ChromeOptions GetChromeOptions()
        {
            var options = base.GetChromeOptions();

            //tarayıcının sandbox özelliğini devre dışı bırakır. Sandbox, tarayıcının web sayfalarından gelen zararlı kodları tespit edip engellemesine yardımcı olan bir güvenlik özelliğidir. Ancak, bazı durumlarda sandbox, tarayıcının performansını düşürebilir.
            options.AddArgument("--no-sandbox");

            //Chrome'un sayfaları güvenli olup olmadığı konusunda uyarılar göstermesini sağlar. Eğer senaryonun güvenlik uyarılarına ihtiyacı yoksa bu özellik gereksiz olabilir.
            options.AddArgument("--safebrowsing-enabled");

            //Bu seçenek, popup engelleyicisinin devre dışı bırakılmasını sağlar.
            options.AddArgument("--disable-popup-blocking");

            return options;
        }

        public override async Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr")
        {
            if (text.Length <= 150 && cache.TryGet(text.ToLower(), out string? cachedTranslation))
            {
                if (!string.IsNullOrEmpty(cachedTranslation))
                {
                    return cachedTranslation;
                }
            }

            _driver.Url = $"https://translate.google.com/?sl={sourceLanguageCode}&tl={targetLanguageCode}&text={WebUtility.UrlEncode(text)}";

            // JavaScript kodunu kullanarak sayfanın yüklendiğini kontrol edin.
            var javascriptExecutor = (IJavaScriptExecutor)_driver;

            //sayfa yüklenene kadar sonsuz döngü
            bool isNotPageLoaded = true;
            while (isNotPageLoaded)
            {
                isNotPageLoaded = !(bool)javascriptExecutor.ExecuteScript("return document.readyState === 'complete'");
            }

            //her seferinde bekleme süresini değiştir
            int randomNumber = rnd.Next(1000, 2001);

            await Task.Delay(randomNumber);

            var result = _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[starts-with(@jsname,'W')]")));

            var translatedText = result.Text;

            //150 karakter sonrasını cache atma
            if (text.Length <= 150)
            {
                cache.Add(text.ToLower(), translatedText);
            }

            return translatedText;
        }
    }
}
