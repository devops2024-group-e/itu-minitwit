using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Mvc.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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

        Assert.True(body.Text.Contains("You were successfully registered and can login now"));
    }

    [Fact]
    public async Task RegisteredUserCanLogin_ReturnsTrue()
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

        Assert.True(body.Text.Contains("You were logged in"));
        Assert.True(body.Text.Contains("My Timeline"));
        Assert.True(body.Text.Contains("This is you!"));
    }

    [Fact]
    public async Task LoggedInUserCanLogout_ReturnsTrue()
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
        _driver.FindElement(By.LinkText("sign out")).Click();

        //Assert
        IWebElement body = _driver.FindElement(By.TagName("body"));

        Assert.True(body.Text.Contains("You were logged out"));
        Assert.True(body.Text.Contains("MiniTwit"));
        Assert.True(body.Text.Contains("sign up"));
        Assert.True(body.Text.Contains("sign in"));
        Assert.True(_driver.Url.Contains("/public"));
    }



    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
