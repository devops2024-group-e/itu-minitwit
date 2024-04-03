using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Minitwit.Tests.UITests;
public class LoginAndLogoutUITests : IDisposable
{
    private readonly IWebDriver _driver;
    private string appURL;

    public LoginAndLogoutUITests()
    {
        ChromeOptions option = new ChromeOptions();
        option.AddArguments("--headless");
        new DriverManager().SetUpDriver(new ChromeConfig());
        _driver = new ChromeDriver(option);
        //appURL = 

    }

    [Fact]
    public async Task NonRegisteredUserCanAccessTheSite_ReturnsTrue()
    {
        _driver.Navigate().GoToUrl("http://localhost:5191/");

        IWebElement body = _driver.FindElement(By.TagName("body"));

        String currentUrl = _driver.Url; 

        Assert.True(body.Text.Contains("MiniTwit"));
        Assert.True(body.Text.Contains("sign up"));
        Assert.True(body.Text.Contains("sign in"));
        Assert.True(currentUrl.Contains("/public"));
    }

    [Fact]
    public async Task NonRegisteredUserCanRegister_ReturnsTrue()
    {
        _driver.Navigate().GoToUrl("http://localhost:5191/");

        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 1");
        _driver.FindElement(By.Name("email")).SendKeys("example@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        IWebElement body = _driver.FindElement(By.TagName("body"));

        Assert.True(body.Text.Contains("You were successfully registered and can login now"));
    }

    public async Task RegisteredUserCanLogin_ReturnsTrue()
    {
        _driver.Navigate().GoToUrl("http://localhost:5191/");

        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 1");
        _driver.FindElement(By.Name("email")).SendKeys("example@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 1");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        IWebElement body = _driver.FindElement(By.TagName("body"));

        Assert.True(body.Text.Contains("You were logged in"));
        Assert.True(body.Text.Contains("My Timeline"));
        Assert.True(body.Text.Contains("This is you!"));
    }



    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
