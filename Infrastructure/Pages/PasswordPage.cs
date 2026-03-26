using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Infrastructure.DTOs;

namespace Infrastructure.Pages
{
    /// <summary>
    /// Page Object Model for the eMAG login page Step 2 – password entry.
    /// Shown after submitting a known email on the LoginPage.
    /// URL: https://auth.emag.bg/user/login (same URL, different form state)
    /// </summary>
    public class PasswordPage : BasePage
    {
        // XPaths
        private static readonly By PasswordInput      = By.XPath("//input[@id='user_login_password' or @name='user_login[password]']");
        private static readonly By SignInButton        = By.XPath("//button[@id='user_login_submit' or (@type='submit' and not(@id='user_login_continue'))]");
        private static readonly By ForgotPasswordLink  = By.XPath("//a[contains(@href,'forgot') or contains(normalize-space(),'забравена') or contains(normalize-space(),'Forgot')]");
        private static readonly By BackLink            = By.XPath("//a[contains(@href,'login') and (contains(normalize-space(),'Назад') or contains(normalize-space(),'Back'))]");
        private static readonly By EmailDisplay        = By.XPath("//*[contains(@class,'user-email') or contains(@class,'login-email')]");
        private static readonly By ErrorAlert          = By.XPath("//*[contains(@class,'alert') and contains(@class,'alert-danger')]");
        private static readonly By ErrorList           = By.XPath("//*[contains(@class,'invalid-feedback') or contains(@class,'form-error') or contains(@class,'has-error')]");

        public PasswordPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Waits for the password step to be fully loaded.
        /// Steps:
        ///   1. Wait for DOM and AJAX to finish.
        ///   2. Wait for the password input to become visible.
        /// Expected result: Password input field is visible and ready for interaction.
        /// </summary>
        public PasswordPage WaitForLoad()
        {
            WaitForPageReady();
            WaitForVisible(PasswordInput);
            return this;
        }

        /// <summary>
        /// Enters a password into the password field.
        /// Steps:
        ///   1. Wait for the password input to be clickable.
        ///   2. Clear any existing value.
        ///   3. Type the provided password.
        /// Expected result: Password field contains the entered value.
        /// </summary>
        public PasswordPage EnterPassword(string password)
        {
            var field = WaitForClickable(PasswordInput);
            field.Clear();
            field.SendKeys(password);
            return this;
        }

        /// <summary>
        /// Clicks the Sign In / Login submit button.
        /// Steps:
        ///   1. Wait for the button to be clickable.
        ///   2. Click it.
        /// Expected result: Login form submitted; navigates to dashboard on success.
        /// </summary>
        public void ClickSignIn()
        {
            WaitForClickable(SignInButton).Click();
        }

        /// <summary>
        /// Enters password and clicks Sign In – shortcut for the full password-step flow.
        /// Steps:
        ///   1. Call EnterPassword with the given password.
        ///   2. Call ClickSignIn.
        /// Expected result: Authentication attempted; redirects on success or shows error on failure.
        /// </summary>
        public void Login(string password)
        {
            EnterPassword(password);
            ClickSignIn();
        }

        /// <summary>
        /// Completes login using a UserDto.
        /// Steps:
        ///   1. Extract password from the DTO.
        ///   2. Call Login with that value.
        /// Expected result: Authentication attempted using DTO credentials.
        /// </summary>
        public void Login(UserDto user) => Login(user.Password);

        /// <summary>
        /// Returns the error message text shown after a failed login attempt.
        /// </summary>
        public string GetErrorText()
        {
            try { return Wait.Until(ExpectedConditions.ElementIsVisible(ErrorAlert)).Text; }
            catch { return string.Empty; }
        }

        public bool IsPasswordInputDisplayed() =>
            WaitForVisible(PasswordInput).Displayed;

        public bool IsForgotPasswordLinkDisplayed()
        {
            try { return Driver.FindElement(ForgotPasswordLink).Displayed; }
            catch { return false; }
        }
    }
}
