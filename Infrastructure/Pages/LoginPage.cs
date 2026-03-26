using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Infrastructure.DTOs;

namespace Infrastructure.Pages
{
    /// <summary>
    /// Page Object Model for the eMAG login page (Step 1 – email entry).
    /// URL: https://auth.emag.bg/user/login
    /// </summary>
    public class LoginPage : BasePage
    {
        public const string Url = "https://auth.emag.bg/user/login";

        // XPaths
        private static readonly By LogoImage         = By.XPath("//a[contains(@class,'d-inline-block')]//img[@alt='eMAG']");
        private static readonly By PageHeading        = By.XPath("//h1[normalize-space()='Здравей!']");
        private static readonly By EmailInput         = By.XPath("//input[@id='user_login_email']");
        private static readonly By ContinueButton     = By.XPath("//button[@id='user_login_continue']");
        private static readonly By InfoText           = By.XPath("//*[contains(normalize-space(),'Нямаш акаунт')]");
        private static readonly By FacebookButton     = By.XPath("//button[contains(@class,'facebook')]");
        private static readonly By GoogleButton       = By.XPath("//button[contains(@class,'google')]");
        private static readonly By AppleButton        = By.XPath("//button[contains(@class,'apple')]");
        private static readonly By HelpLink           = By.XPath("//a[contains(@href,'vpisvane-registratsiya')]");
        private static readonly By PrivacyLink        = By.XPath("//a[contains(@href,'lichni-danni')]");
        private static readonly By ErrorAlert         = By.XPath("//*[contains(@class,'alert') and contains(@class,'alert-danger')]");
        private static readonly By ValidationError    = By.XPath("//*[contains(@class,'invalid-feedback') or contains(@class,'form-error')]");

        public LoginPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Navigates to the login page and waits for it to be fully loaded.
        /// Steps:
        ///   1. Navigate to the login URL.
        ///   2. Wait for the DOM and AJAX to complete.
        ///   3. Wait for the email input to be visible.
        /// Expected result: Login page is displayed with the email input field.
        /// </summary>
        public LoginPage Open()
        {
            Driver.Navigate().GoToUrl(Url);
            WaitForPageReady();
            WaitForVisible(EmailInput);
            return this;
        }

        /// <summary>
        /// Enters an email address into the email field.
        /// Steps:
        ///   1. Wait for email input to be clickable.
        ///   2. Clear any existing value.
        ///   3. Type the provided email.
        /// Expected result: Email field contains the entered value.
        /// </summary>
        public LoginPage EnterEmail(string email)
        {
            var field = WaitForClickable(EmailInput);
            field.Clear();
            field.SendKeys(email);
            return this;
        }

        /// <summary>
        /// Clicks the "Продължи" (Continue) button to proceed to the next step.
        /// Steps:
        ///   1. Wait for the Continue button to be clickable.
        ///   2. Click it.
        /// Expected result: Form is submitted; browser navigates to password or registration step.
        /// </summary>
        public void ClickContinue()
        {
            WaitForClickable(ContinueButton).Click();
        }

        /// <summary>
        /// Enters the email and clicks Continue – shortcut for the full email-step flow.
        /// Steps:
        ///   1. Call EnterEmail.
        ///   2. Call ClickContinue.
        /// Expected result: Email step completed; next page begins loading.
        /// </summary>
        public void SubmitEmail(string email)
        {
            EnterEmail(email);
            ClickContinue();
        }

        /// <summary>
        /// Submits the email from a UserDto (DTO-based login entry point).
        /// Steps:
        ///   1. Extract email from the DTO.
        ///   2. Call SubmitEmail with that value.
        /// Expected result: Email step completed using DTO data.
        /// </summary>
        public void SubmitEmail(UserDto user) => SubmitEmail(user.Email);

        // ── Element state queries ──────────────────────────────────────────────

        public bool IsLogoDisplayed()       => WaitForVisible(LogoImage).Displayed;
        public bool IsHeadingDisplayed()    => WaitForVisible(PageHeading).Displayed;
        public bool IsContinueButtonDisplayed() => WaitForVisible(ContinueButton).Displayed;
        public bool IsFacebookButtonDisplayed() => WaitForVisible(FacebookButton).Displayed;
        public bool IsGoogleButtonDisplayed()   => WaitForVisible(GoogleButton).Displayed;
        public bool IsAppleButtonDisplayed()    => WaitForVisible(AppleButton).Displayed;
        public bool IsHelpLinkDisplayed()       => WaitForVisible(HelpLink).Displayed;

        /// <summary>
        /// Returns the text of the page heading element.
        /// </summary>
        public string GetHeadingText() => WaitForVisible(PageHeading).Text;

        /// <summary>
        /// Returns the "no account" informational text shown below the email form.
        /// </summary>
        public string GetInfoText() => WaitForVisible(InfoText).Text;

        /// <summary>
        /// Returns the error alert text when the server rejects the email step.
        /// </summary>
        public string GetErrorText()
        {
            try { return Wait.Until(ExpectedConditions.ElementIsVisible(ErrorAlert)).Text; }
            catch { return string.Empty; }
        }

        /// <summary>
        /// Returns the inline validation error text (e.g. "Please enter a valid email").
        /// </summary>
        public string GetValidationErrorText()
        {
            try { return Wait.Until(ExpectedConditions.ElementIsVisible(ValidationError)).Text; }
            catch { return string.Empty; }
        }

        /// <summary>
        /// Checks whether the email input HTML5 validation is invalid (empty submit attempt).
        /// Uses JS to query the validity state.
        /// </summary>
        public bool IsEmailInputInvalid()
        {
            var input = Driver.FindElement(EmailInput);
            var valid = (bool)((IJavaScriptExecutor)Driver)
                .ExecuteScript("return arguments[0].validity.valid", input);
            return !valid;
        }
    }
}
