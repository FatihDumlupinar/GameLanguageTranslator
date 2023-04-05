using GameLanguageTranslator.Selenium.Base;
using GameLanguageTranslator.Utilities;
using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using System.Net;
using System.Text.RegularExpressions;

namespace GameLanguageTranslator.Selenium;

public class GoogleTranslateSeleniumBot : BaseSelenium
{
    private readonly TranslationFromDatabase translationFromDatabase = TranslationFromDatabase.Instance;

    public override async Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr")
    {
        try
        {
            text = text.TrimStart().TrimEnd();

            //veritabanında varsa direkt geriye döndür
            var translatedText = await translationFromDatabase.GetTranslationFromDatabaseAsync(text, sourceLanguageCode, targetLanguageCode);
            if (!string.IsNullOrEmpty(translatedText))
            {
                return translatedText;
            }

            //tam tersine bak(çevrilmiş bir kelime ise)
            var searchInTranslatedWords = await translationFromDatabase.SearchInTranslatedWordsFromDatabaseAsync(text, sourceLanguageCode, targetLanguageCode);
            if (!string.IsNullOrEmpty(searchInTranslatedWords))
            {
                return searchInTranslatedWords;
            }

            //cümlelere ayır
            int sentencesCount = Regex.Split(text, @"(?<=[\.!\?])\s+").Length;
            //var sentences = Regex.Split(text, @"(?<=[\.!\?])\s+");

            int randomNumber = rnd.Next(1000, 2001);

            await Task.Delay(randomNumber);

            var dictionary = new Dictionary<string, string>();

            // Özel kelimeleri bul ve değiştir
            text = Regex.Replace(text, @"({.*?}|\[.*?\])", match =>
            {
                string key = match.Value;
                if (!dictionary.ContainsKey(key))
                {
                    // Rastgele bir 6 karakterli dize oluşturun
                    string randomString = GenerateRandomString();
                    dictionary.Add(key, randomString);
                }

                // Özel kelimeyi rastgele dizeyle değiştirin
                return dictionary[key];
            });


            _driver.Url = $"https://translate.google.com/?sl={sourceLanguageCode}&tl={targetLanguageCode}&text={WebUtility.UrlEncode(text)}";

            // JavaScript kodunu kullanarak sayfanın yüklendiğini kontrol edin.
            var javascriptExecutor = (IJavaScriptExecutor)_driver;

            //sayfa yüklenene kadar sonsuz döngü
            bool isNotPageLoaded = true;
            while (isNotPageLoaded)
            {
                isNotPageLoaded = !(bool)javascriptExecutor.ExecuteScript("return document.readyState === 'complete'");
            }

            randomNumber = rnd.Next(1000, 2001);

            await Task.Delay(randomNumber);

            // Sayfayı en altına kaydırın, böylece tüm span'lar yüklenir.
            javascriptExecutor.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");

            // Tüm span'ları XPath ifadesiyle seçin.
            var elements = _wait.Until(ExpectedConditions.VisibilityOfAllElementsLocatedBy(By.XPath("//span[starts-with(@jsname,'W') and contains(@class, 'ryNqvb')]")));

            // Tüm span'ların içeriğini birleştirin.
            foreach (var element in elements)
            {
                translatedText = translatedText + " " + element.Text;
            }

            translatedText = translatedText.TrimStart().TrimEnd();

            int translatedTextSentencesCount = Regex.Split(translatedText, @"(?<=[\.!\?])\s+").Length;
            //var translatedTextSentences = Regex.Split(translatedText, @"(?<=[\.!\?])\s+");

            if (sentencesCount > translatedTextSentencesCount)
            {
                throw new Exception("Çeviriler aynı değil!");
            }

            //özel kelimleri yerine geri koyuyor
            foreach (var item in dictionary)
            {
                text = text.Replace(item.Value, item.Key);
            }

            foreach (var item in dictionary)
            {
                translatedText = translatedText.Replace(item.Value, item.Key);
            }

            //veritabanına ekle çeviriyi
            await translationFromDatabase.AddTranslationToDatabaseAsync(text, sourceLanguageCode, targetLanguageCode, translatedText);

            return translatedText;
        }
        catch (Exception e)
        {

            throw;
        }
    }

    public static string GenerateRandomString()
    {
        var random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

}
