using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Infrastructure.Pages
{
    /// <summary>
    /// Abstract base class for all Page Object Models.
    /// Provides shared driver access, explicit waits, and JS execution helpers.
    /// </summary>
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
        private const int DefaultTimeoutSeconds = 15;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(DefaultTimeoutSeconds));
        }

        /// <summary>
        /// Waits until the DOM readyState is 'complete' and no pending jQuery/fetch AJAX requests remain.
        /// Steps:
        ///   1. Poll document.readyState until it equals 'complete'.
        ///   2. Poll jQuery.active (if jQuery exists) until it equals 0.
        /// Expected result: Page is fully loaded and all AJAX calls have finished.
        /// </summary>
        protected void WaitForPageReady()
        {
            Wait.Until(d =>
            {
                var js = (IJavaScriptExecutor)d;
                var domReady = (bool)js.ExecuteScript("return document.readyState === 'complete'");
                if (!domReady) return false;

                var jqueryDone = (bool)js.ExecuteScript(
                    "return typeof jQuery === 'undefined' || jQuery.active === 0");
                return jqueryDone;
            });
        }

        /// <summary>
        /// Waits for an element matching the given XPath to be visible and returns it.
        /// </summary>
        protected IWebElement WaitForVisible(By locator) =>
            Wait.Until(ExpectedConditions.ElementIsVisible(locator));

        /// <summary>
        /// Waits for an element matching the given XPath to be clickable and returns it.
        /// </summary>
        protected IWebElement WaitForClickable(By locator) =>
            Wait.Until(ExpectedConditions.ElementToBeClickable(locator));

        /// <summary>
        /// Scrolls the given element into view using JavaScript.
        /// </summary>
        protected void ScrollIntoView(IWebElement element)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", element);
        }

        /// <summary>
        /// Clicks an element using JavaScript (useful when normal click is intercepted).
        /// </summary>
        protected void JsClick(IWebElement element)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
        }

        /// <summary>
        /// Returns the current page URL.
        /// </summary>
        public string CurrentUrl => Driver.Url;

        /// <summary>
        /// Returns the current page title.
        /// </summary>
        public string PageTitle => Driver.Title;
    }
}
