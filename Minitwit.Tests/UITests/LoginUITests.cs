using Microsoft.AspNetCore.Mvc.Testing;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Minitwit.Tests.UITests;
public class LoginUITests : IDisposable
{
    private readonly IWebDriver _driver;
    private string appURL;

    public LoginUITests()
    {
        _driver = new ChromeDriver();
        //appURL = 

    }

    [Fact]
    public async Task FirstTest_ReturnsTrue()
    {
        _driver.Navigate().GoToUrl("http://161.35.78.128/");

        IWebElement body = _driver.FindElement(By.TagName("body"));

        //Assert.IsTrue(body.Text.Contains("MiniTwit"));

        Assert.True(body.Text.Contains("MiniTwit"));
    }


    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}