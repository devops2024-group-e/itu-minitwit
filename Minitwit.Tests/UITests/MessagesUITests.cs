using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Minitwit.Tests.UITests;
public class MessagesUITests : IDisposable
{
    private readonly IWebDriver _driver;
    private string appURL;

    public MessagesUITests()
    {
        ChromeOptions option = new ChromeOptions();
        option.AddArguments("--headless");
        new DriverManager().SetUpDriver(new ChromeConfig());
        _driver = new ChromeDriver(option);
    }

    [Fact]
    public async Task RegisteredUserCanWriteMessage_ReturnsTrue()
    {

    }


    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
