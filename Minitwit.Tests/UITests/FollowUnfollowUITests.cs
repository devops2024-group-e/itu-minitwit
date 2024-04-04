using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
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
    public async Task User1CanFollowUser2_ReturnsTrue()
    {

    }

    [Fact]
    public async Task User1CanUnfollowUser2_ReturnsTrue()
    {

    }

    [Fact]
    public async Task User1HasUser2MessagesOnMyTimelineWhenFollowingUser2_ReturnsTrue()
    {

    }

    [Fact]
    public async Task User2MessagesDissapearsFromUser1TimelineWhenUser1UnfollowsUser2_ReturnsTrue()
    {

    }

    public void Dispose()
    {
        _driver.Quit();
        _driver.Dispose();
    }
}
