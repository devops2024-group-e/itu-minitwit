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

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }

    [Fact]
    public async Task RegisteredUserCanWriteMessage()
    {
        //Arrange
        _driver.Navigate().GoToUrl("http://localhost:5191/public");

        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 6");
        _driver.FindElement(By.Name("email")).SendKeys("example6@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 6");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        //Act
        _driver.Navigate().GoToUrl("http://localhost:5191/");
        _driver.FindElement(By.Name("text")).SendKeys("Hej fra User 6");
        _driver.FindElement(By.XPath("/html/body/div/div[2]/main/div[2]/form/p/input[2]")).Submit();

        //Assert
        var flashMessage = _driver.FindElement(By.ClassName("flashes"));
        Assert.True(flashMessage.Text.Contains("Your message was recorded"));

        var myTimelineBody = _driver.FindElement(By.TagName("body"));
        Assert.True(myTimelineBody.Text.Contains("Hej fra User 6"));

        _driver.Navigate().GoToUrl("http://localhost:5191/public");
        var publicTimelineBody = _driver.FindElement(By.TagName("body"));
        Assert.True(publicTimelineBody.Text.Contains("Hej fra User 6"));
        Assert.True(publicTimelineBody.Text.Contains("Test User 6"));
    }


    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
