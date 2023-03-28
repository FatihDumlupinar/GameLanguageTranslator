using GameLanguageTranslator.Utilities;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace GameLanguageTranslator.Selenium.Base;

public abstract class BaseSelenium : IBaseSelenium
{
    protected static IWebDriver? _driver;
    protected static WebDriverWait? _wait;

    //aynı cümleleri tekrar çevirmemesi için
    protected static LRUCache<string, string> cache = new(100);

    protected static readonly Random rnd = new();

    public BaseSelenium()
    {
        if (_driver is null)
        {
            _driver = new ChromeDriver(GetChromeOptions());
        }

        if (_wait is null && _driver is not null)
        {
            _wait = new(_driver, TimeSpan.FromSeconds(10));
        }
    }

    public abstract Task<string> TranslateTextAsync(string text, string sourceLanguageCode = "en", string targetLanguageCode = "tr");

    public virtual ChromeOptions GetChromeOptions()
    {
        var options = new ChromeOptions();

        // tarayıcının headless modda çalışmasını sağlar, bu sayede tarayıcının arayüzü olmadan çalışır ve performans artırılır.
        //options.AddArgument("--headless");

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
        cache = null!;
    }
}
