using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Minitwit.Tests.UITests;
public class LoginRegisterLogoutUITests : IDisposable
{
    private readonly IWebDriver _driver;
    private string appURL;

    public LoginRegisterLogoutUITests()
    {
        ChromeOptions option = new ChromeOptions();
        option.AddArguments("--headless");
        new DriverManager().SetUpDriver(new ChromeConfig());
        _driver = new ChromeDriver(option);
    }

    [Fact]
    public async Task NonRegisteredUserCanAccessTheSite()
    {
        _driver.Navigate().GoToUrl("http://localhost:5191/");

        IWebElement body = _driver.FindElement(By.TagName("body"));

        var currentUrl = _driver.Url;

        Assert.Contains("MiniTwit", body.Text);
        Assert.Contains("sign up", body.Text);
        Assert.Contains("sign in", body.Text);
        Assert.Contains("/public", currentUrl);
    }

    [Fact]
    public async Task NonRegisteredUserCanRegister()
    {
        //Arrange
        _driver.Navigate().GoToUrl("http://localhost:5191/");

        //Act
        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 1");
        _driver.FindElement(By.Name("email")).SendKeys("example1@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        //Assert
        IWebElement body = _driver.FindElement(By.TagName("body"));

        Assert.Contains("You were successfully registered and can login now", body.Text);
    }

    [Fact]
    public async Task RegisteredUserCanLogin()
    {
        //Arrange
        _driver.Navigate().GoToUrl("http://localhost:5191/");

        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 2");
        _driver.FindElement(By.Name("email")).SendKeys("example2@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        //Act
        _driver.FindElement(By.Name("username")).SendKeys("Test User 2");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        //Assert
        IWebElement body = _driver.FindElement(By.TagName("body"));

        Assert.Contains("You were logged in", body.Text);
        Assert.Contains("My Timeline", body.Text);
        Assert.Contains("This is you!", body.Text);
    }

    [Fact]
    public async Task LoggedInUserCanLogout()
    {
        //Arrange
        _driver.Navigate().GoToUrl("http://localhost:5191/Register");

        _driver.FindElement(By.Name("username")).SendKeys("Test User 3");
        _driver.FindElement(By.Name("email")).SendKeys("example3@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.Navigate().GoToUrl("http://localhost:5191/Login");

        _driver.FindElement(By.Name("username")).SendKeys("Test User 3");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

        //Act
        _driver.FindElement(By.XPath("/html/body/div/div[1]/a[3]")).Click();

        var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));
        wait.Until(ExpectedConditions.UrlContains("/public"));


        //Assert
        IWebElement body = _driver.FindElement(By.TagName("body"));

        Assert.Contains("You were logged out", body.Text);
        Assert.Contains("MiniTwit", body.Text);
        Assert.Contains("sign up", body.Text);
        Assert.Contains("sign in", body.Text);
        Assert.Contains("/public", _driver.Url);
    }



    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
