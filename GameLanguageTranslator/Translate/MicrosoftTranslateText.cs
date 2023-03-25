using Newtonsoft.Json;
using System.Text;

namespace GameLanguageTranslator.Translate;

public class MicrosoftTranslateText : ITranslator
{
    public async Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr")
    {
        string key = "YOUR_MICROSOFT_TRANSLATE_API_KEY";

        string endpoint = "https://api.cognitive.microsofttranslator.com";

        string location = "YOUR_LOCATION";

        string route = $"/translate?api-version=3.0&from={sourceLanguageCode}&to={targetLanguageCode}";

        object[] body = new object[] { new { Text = text } };

        var requestBody = JsonConvert.SerializeObject(body);

        using var client = new HttpClient();
        using var request = new HttpRequestMessage();

        request.Method = HttpMethod.Post;
        request.RequestUri = new Uri(endpoint + route);
        request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        request.Headers.Add("Ocp-Apim-Subscription-Key", key);
        request.Headers.Add("Ocp-Apim-Subscription-Region", location);

        HttpResponseMessage response = await client.SendAsync(request).ConfigureAwait(false);

        var responseContent = await response.Content.ReadAsStringAsync();
        var translationResult = JsonConvert.DeserializeObject<MicrosoftTranslationResult[]>(responseContent);

        return translationResult != null ? translationResult[0].Translations[0].Text : text;
    }

    public class MicrosoftTranslationResult
    {
        public MicrosoftTranslation[] Translations { get; set; } = Array.Empty<MicrosoftTranslation>();
    }

    public class MicrosoftTranslation
    {
        public string Text { get; set; } = "";
        public string To { get; set; } = "";
    }
}
