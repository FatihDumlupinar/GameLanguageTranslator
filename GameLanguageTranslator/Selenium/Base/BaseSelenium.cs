using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GameLanguageTranslator.Selenium.Base;

public abstract class BaseSelenium : IBaseSelenium
{
    protected static IWebDriver? _driver = new ChromeDriver(GetChromeOptions());
    protected static WebDriverWait? _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(10));

    protected static Random rnd = new();

    public abstract Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr");

    public static ChromeOptions GetChromeOptions()
    {
        var options = new ChromeOptions();

        // tarayıcının headless modda çalışmasını sağlar, bu sayede tarayıcının arayüzü olmadan çalışır ve performans artırılır.
        options.AddArgument("--headless");

        //tarayıcının sandbox özelliğini devre dışı bırakır. Sandbox, tarayıcının web sayfalarından gelen zararlı kodları tespit edip engellemesine yardımcı olan bir güvenlik özelliğidir. Ancak, bazı durumlarda sandbox, tarayıcının performansını düşürebilir.
        options.AddArgument("--no-sandbox");

        //Chrome'un sayfaları güvenli olup olmadığı konusunda uyarılar göstermesini sağlar. Eğer senaryonun güvenlik uyarılarına ihtiyacı yoksa bu özellik gereksiz olabilir.
        options.AddArgument("--safebrowsing-enabled");

        //Bu seçenek, popup engelleyicisinin devre dışı bırakılmasını sağlar.
        options.AddArgument("--disable-popup-blocking");

        //bot olmadığımızı göstermek için :)
        options.AddArgument("user-agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.5563.111 Safari/537.36");

        //tam ekranda başlatır
        options.AddArgument("start-maximized");

        //GPU (Grafik İşlemci Birimi) kullanımını devre dışı bırakır. Bu, tarayıcının daha az bellek kullanmasına ve daha hızlı çalışmasına yardımcı olabilir.
        options.AddArgument("--disable-gpu");

        //tarayıcı uzantılarını devre dışı bırakır. Bu, tarayıcının daha hızlı başlamasına ve daha az bellek kullanmasına yardımcı olabilir.
        //NOT: options.AddArgument("disable-extensions"); bu şekilde de olabilir
        options.AddArgument("--disable-extensions");

        // Tarayıcı eklentilerini devre dışı bırakır. Bu özellik gereksiz olabilir, çünkü kullanıcının zaten istediği eklentileri yükleyebilir.
        options.AddArgument("--disable-plugins");

        //ChromeDriver'ın konsol çıktısını devre dışı bırakarak performansı artırabilir.
        options.AddArgument("disable-logging");

        //ChromeDriver'ın bildirim çubuğunu devre dışı bırakarak performansı artırabilir.
        options.AddArgument("disable-infobars");

        // ChromeDriver'ın konsol çıktısını devre dışı bırakarak performansı artırabilir.
        options.AddArgument("--log-level=3");

        // ChromeDriver'ın bildirim çubuğunu devre dışı bırakarak performansı artırabilir.
        options.AddArgument("--disable-notifications");

        // ChromeDriver'ın /dev/shm klasörünü kullanmayı devre dışı bırakarak performansı artırabilir.
        //NOT: Windows'ta kullanmak fark yaratmaz. Ancak, diğer işletim sistemlerinde bu argüman RAM kullanımını azaltabilir ve performansı artırabilir.
        options.AddArgument("disable-dev-shm-usage");

        //Chrome'un özel modda başlamasını sağlar, bu sayede tarayıcı geçmişi ve çerezleri temizlenir ve performans artırılır.
        options.AddArgument("--incognito");

        options.AddArgument("--disable-translate");

        return options;
    }

    public virtual void Quit()
    {
        _driver.Quit();
    }

    public virtual void Dispose()
    {
        Quit();
        _driver = null!;
        _wait = null!;
        rnd = null!;
    }
}
