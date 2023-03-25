using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace GameLanguageTranslator.Translate;

public class SeleniumBotTranslate : IDisposable
{

    private IWebDriver driver;

    public SeleniumBotTranslate(string sourceLanguageCode = "en", string targetLanguageCode = "tr")
    {
        if (driver is null)
        {
            ChromeOptions options = new ChromeOptions();

            //tam ekranda başlatır
            options.AddArgument("start-maximized");

            //GPU (Grafik İşlemci Birimi) kullanımını devre dışı bırakır. Bu, tarayıcının daha az bellek kullanmasına ve daha hızlı çalışmasına yardımcı olabilir.
            options.AddArgument("--disable-gpu");

            //tarayıcı uzantılarını devre dışı bırakır. Bu, tarayıcının daha hızlı başlamasına ve daha az bellek kullanmasına yardımcı olabilir.
            //NOT: options.AddArgument("disable-extensions"); bu şekilde de olabilir
            options.AddArgument("--disable-extensions");

            //tarayıcının sandbox özelliğini devre dışı bırakır. Sandbox, tarayıcının web sayfalarından gelen zararlı kodları tespit edip engellemesine yardımcı olan bir güvenlik özelliğidir. Ancak, bazı durumlarda sandbox, tarayıcının performansını düşürebilir.
            options.AddArgument("--no-sandbox");

            //ChromeDriver'ın konsol çıktısını devre dışı bırakarak performansı artırabilir.
            options.AddArgument("disable-logging");

            //ChromeDriver'ın bildirim çubuğunu devre dışı bırakarak performansı artırabilir.
            options.AddArgument("disable-infobars");

            // ChromeDriver'ın /dev/shm klasörünü kullanmayı devre dışı bırakarak performansı artırabilir.
            //NOT: Windows'ta kullanmak fark yaratmaz. Ancak, diğer işletim sistemlerinde bu argüman RAM kullanımını azaltabilir ve performansı artırabilir.
            options.AddArgument("disable-dev-shm-usage");

            //Chrome'un özel modda başlamasını sağlar, bu sayede tarayıcı geçmişi ve çerezleri temizlenir ve performans artırılır.
            options.AddArgument("--incognito");

            driver = new ChromeDriver(options);
            driver.Url = $"https://translate.google.com/?sl={sourceLanguageCode}&tl={targetLanguageCode}";
        }
    }

    public async Task<string> TranslateTextAsync(string text)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

        await Task.Delay(500);
        var textArea = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("textarea[aria-label='Kaynak metin']")));

        textArea.Clear();
        textArea.SendKeys(text);

        textArea.SendKeys(Keys.Enter);

        await Task.Delay(2000);

        var result = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//span[starts-with(@jsname,'W')]")));

        return result.Text;
    }

    public void Quit()
    {
        driver.Quit();
    }

    public void Dispose()
    {
        driver.Quit();
        driver = null!;
    }
}
