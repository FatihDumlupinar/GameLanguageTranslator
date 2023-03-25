using Google.Cloud.Translation.V2;
using System.Net;

namespace GameLanguageTranslator.Translate;

public class GoogleTranslateText : ITranslator
{
    public async Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr")
    {
        //bu hesap açarsanız diye

        //string apiKey = "YOUR_GOOGLE_TRANSLATE_API_KEY";

        //using var client = TranslationClient.CreateFromApiKey(apiKey);
        //var response = await client.TranslateTextAsync(text, targetLanguageCode, sourceLanguageCode).ConfigureAwait(false);

        //return response.TranslatedText;

        //bu da ücretsiz apisi 
        string url = $"https://translate.googleapis.com/translate_a/single?client=gtx&sl=en&tl={targetLanguageCode}&dt=t&q={WebUtility.UrlEncode(text)}";

        using var client = new WebClient();

        string response = client.DownloadString(url);
        string translatedText = response.Split('"')[1];

        return translatedText;
    }
}
