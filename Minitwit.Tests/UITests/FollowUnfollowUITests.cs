using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;

namespace Minitwit.Tests.UITests;
public class FollowUnfollowUITests : IDisposable
{
    private readonly IWebDriver _driver;
    private string appURL;

    public FollowUnfollowUITests()
    {
        ChromeOptions option = new ChromeOptions();
        option.AddArguments("--headless");
        new DriverManager().SetUpDriver(new ChromeConfig());
        _driver = new ChromeDriver(option);
    }

    [Fact]
    public async Task User1CanFollowAndUnfollowUser2()
    {
        //Arrange
        _driver.Navigate().GoToUrl("http://localhost:5191/public");

        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 4");
        _driver.FindElement(By.Name("email")).SendKeys("example4@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.FindElement(By.LinkText("sign up |")).Click();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 5");
        _driver.FindElement(By.Name("email")).SendKeys("example5@email.com");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.Name("password2")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.FindElement(By.Name("username")).SendKeys("Test User 5");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        _driver.Navigate().GoToUrl("http://localhost:5191/");
        _driver.FindElement(By.Name("text")).SendKeys("Hej fra User 5");
        _driver.FindElement(By.XPath("/html/body/div/div[2]/main/div[2]/form/p/input[2]")).Submit();
        _driver.FindElement(By.XPath("/html/body/div/div[1]/a[3]")).Click();

        _driver.Navigate().GoToUrl("http://localhost:5191/Login");
        _driver.FindElement(By.Name("username")).SendKeys("Test User 4");
        _driver.FindElement(By.Name("password")).SendKeys("12345");
        _driver.FindElement(By.ClassName("actions")).Submit();

        //Act Follow
        _driver.Navigate().GoToUrl("http://localhost:5191/public");
        _driver.FindElement(By.LinkText("Test User 5")).Click();
        _driver.FindElement(By.LinkText("Follow user")).Click();

        //Assert Follow
        var flashMessage = _driver.FindElement(By.ClassName("flashes"));
        Assert.Contains("You are now following", flashMessage.Text);

        _driver.Navigate().GoToUrl("http://localhost:5191");
        var body = _driver.FindElement(By.TagName("body"));

        Assert.Contains("Hej fra User 5", body.Text);

        //Act Unfollow
        _driver.Navigate().GoToUrl("http://localhost:5191/public");
        _driver.FindElement(By.LinkText("Test User 5")).Click();
        _driver.FindElement(By.LinkText("Unfollow user")).Click();

        //Assert Unfollow
        flashMessage = _driver.FindElement(By.ClassName("flashes"));

        Assert.Contains("You are no longer following", flashMessage.Text);

        _driver.Navigate().GoToUrl("http://localhost:5191");
        body = _driver.FindElement(By.TagName("body"));

        Assert.DoesNotContain("Hej fra User 5", body.Text);
    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
