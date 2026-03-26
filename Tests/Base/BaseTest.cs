using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Tests.Base
{
    /// <summary>
    /// Abstract base class for all NUnit test fixtures.
    /// Handles ChromeDriver setup and teardown; all test classes inherit from this.
    /// </summary>
    [TestFixture]
    public abstract class BaseTest
    {
        protected IWebDriver Driver = null!;

        /// <summary>
        /// Initialises a new ChromeDriver instance before each test.
        /// Steps:
        ///   1. Configure ChromeOptions (no sandbox, disable GPU for CI, set window size).
        ///   2. Create a new ChromeDriver instance.
        ///   3. Maximise the browser window.
        ///   4. Set implicit wait to 0 (we rely on explicit waits via WebDriverWait).
        /// Expected result: Driver is ready; browser window is open and maximised.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            var options = new ChromeOptions();
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--window-size=1920,1080");
            // Uncomment to run headless in CI:
            // options.AddArgument("--headless=new");

            Driver = new ChromeDriver(options);
            Driver.Manage().Window.Maximize();
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
        }

        /// <summary>
        /// Quits the ChromeDriver instance after each test.
        /// Steps:
        ///   1. Call Driver.Quit() to close the browser and end the WebDriver session.
        /// Expected result: Browser is closed; no orphaned chromedriver.exe processes.
        /// </summary>
        [TearDown]
        public void TearDown()
        {
            Driver?.Quit();
            Driver?.Dispose();
        }
    }
}
