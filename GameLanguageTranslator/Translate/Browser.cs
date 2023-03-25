using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GameLanguageTranslator.Translate;

public class Browser : IDisposable
{
    private static readonly Lazy<Browser> lazy = new Lazy<Browser>(() => new Browser());

    public static Browser Instance { get { return lazy.Value; } }

    private IWebDriver driver;

    private Browser()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("start-maximized");
        driver = new ChromeDriver(options);
    }

    public IWebDriver Driver { get { return driver; } }

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
