using OpenQA.Selenium;
using SeleniumExtras.WaitHelpers;
using Infrastructure.DTOs;

namespace Infrastructure.Pages
{
    /// <summary>
    /// Page Object Model for the eMAG registration page.
    /// Shown after submitting an unknown email on the LoginPage,
    /// or navigated to directly via: https://auth.emag.bg/user/register
    /// </summary>
    public class RegistrationPage : BasePage
    {
        public const string Url = "https://auth.emag.bg/user/register";

        // XPaths – using flexible attribute-based selectors
        private static readonly By PageHeading         = By.XPath("//h1[contains(normalize-space(),'Creează') or contains(normalize-space(),'Създай') or contains(normalize-space(),'Регистрация') or contains(normalize-space(),'Register')]");
        private static readonly By FirstNameInput      = By.XPath("//input[@id='user_registration_firstName' or @name='user_registration[firstName]']");
        private static readonly By LastNameInput       = By.XPath("//input[@id='user_registration_lastName' or @name='user_registration[lastName]']");
        private static readonly By EmailInput          = By.XPath("//input[@id='user_registration_email' or @name='user_registration[email]' or (@type='email' and not(@id='user_login_email'))]");
        private static readonly By PasswordInput       = By.XPath("//input[@id='user_registration_password' or @name='user_registration[password][first]' or (@type='password' and contains(@id,'password') and not(contains(@id,'confirm')))]");
        private static readonly By ConfirmPasswordInput = By.XPath("//input[@id='user_registration_passwordConfirm' or @name='user_registration[password][second]' or (contains(@id,'confirm') and @type='password')]");
        private static readonly By PhoneInput          = By.XPath("//input[@id='user_registration_phone' or @name='user_registration[phone]' or @type='tel']");
        private static readonly By RegisterButton      = By.XPath("//button[@id='user_registration_submit' or (@type='submit' and (contains(normalize-space(),'Регистрирай') or contains(normalize-space(),'Register') or contains(normalize-space(),'Създай')))]");
        private static readonly By ErrorAlerts         = By.XPath("//*[contains(@class,'alert') and contains(@class,'alert-danger')]");
        private static readonly By FieldErrors         = By.XPath("//*[contains(@class,'invalid-feedback') or contains(@class,'form-error')]");
        private static readonly By TermsCheckbox       = By.XPath("//input[@type='checkbox' and (contains(@id,'terms') or contains(@name,'terms') or contains(@id,'gdpr'))]");
        private static readonly By BackToLoginLink     = By.XPath("//a[contains(@href,'login') and (contains(normalize-space(),'Влез') or contains(normalize-space(),'вход') or contains(normalize-space(),'Login'))]");

        public RegistrationPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Waits for the registration page/form to be fully loaded.
        /// Steps:
        ///   1. Wait for DOM and AJAX to finish.
        ///   2. Wait for the email input to become visible.
        /// Expected result: Registration form is visible and interactive.
        /// </summary>
        public RegistrationPage WaitForLoad()
        {
            WaitForPageReady();
            WaitForVisible(EmailInput);
            return this;
        }

        /// <summary>
        /// Fills in the first name field.
        /// Steps:
        ///   1. Wait for the first name input to be clickable.
        ///   2. Clear and type the provided value.
        /// Expected result: First name field is populated.
        /// </summary>
        public RegistrationPage EnterFirstName(string firstName)
        {
            var field = WaitForClickable(FirstNameInput);
            field.Clear();
            field.SendKeys(firstName);
            return this;
        }

        /// <summary>
        /// Fills in the last name field.
        /// </summary>
        public RegistrationPage EnterLastName(string lastName)
        {
            var field = WaitForClickable(LastNameInput);
            field.Clear();
            field.SendKeys(lastName);
            return this;
        }

        /// <summary>
        /// Fills in the email field.
        /// </summary>
        public RegistrationPage EnterEmail(string email)
        {
            var field = WaitForClickable(EmailInput);
            field.Clear();
            field.SendKeys(email);
            return this;
        }

        /// <summary>
        /// Fills in the password field.
        /// </summary>
        public RegistrationPage EnterPassword(string password)
        {
            var field = WaitForClickable(PasswordInput);
            field.Clear();
            field.SendKeys(password);
            return this;
        }

        /// <summary>
        /// Fills in the confirm password field.
        /// </summary>
        public RegistrationPage EnterConfirmPassword(string confirmPassword)
        {
            var field = WaitForClickable(ConfirmPasswordInput);
            field.Clear();
            field.SendKeys(confirmPassword);
            return this;
        }

        /// <summary>
        /// Fills in the phone number field.
        /// </summary>
        public RegistrationPage EnterPhone(string phone)
        {
            var field = WaitForClickable(PhoneInput);
            field.Clear();
            field.SendKeys(phone);
            return this;
        }

        /// <summary>
        /// Accepts the terms and conditions checkbox if present.
        /// Steps:
        ///   1. Find the terms checkbox.
        ///   2. Click it only if it is not already checked.
        /// Expected result: Terms checkbox is in a checked state.
        /// </summary>
        public RegistrationPage AcceptTermsIfPresent()
        {
            try
            {
                var checkbox = Driver.FindElement(TermsCheckbox);
                if (!checkbox.Selected) checkbox.Click();
            }
            catch (NoSuchElementException) { /* Terms checkbox may not be present */ }
            return this;
        }

        /// <summary>
        /// Clicks the Register / Submit button.
        /// Steps:
        ///   1. Scroll the button into view.
        ///   2. Wait for it to be clickable and click.
        /// Expected result: Registration form is submitted.
        /// </summary>
        public void ClickRegister()
        {
            var btn = WaitForClickable(RegisterButton);
            ScrollIntoView(btn);
            btn.Click();
        }

        /// <summary>
        /// Fills the entire registration form from a RegistrationDto and submits it.
        /// Steps:
        ///   1. Enter first name, last name, email, password, confirm password, phone from DTO.
        ///   2. Accept terms if present.
        ///   3. Click Register.
        /// Expected result: Full registration form submitted.
        /// </summary>
        public void Register(RegistrationDto dto)
        {
            WaitForLoad();
            EnterFirstName(dto.FirstName);
            EnterLastName(dto.LastName);
            EnterEmail(dto.Email);
            EnterPassword(dto.Password);
            EnterConfirmPassword(dto.ConfirmPassword);
            EnterPhone(dto.Phone);
            AcceptTermsIfPresent();
            ClickRegister();
        }

        /// <summary>
        /// Returns all field-level validation error texts after a failed submit.
        /// </summary>
        public IList<string> GetFieldErrors()
        {
            try
            {
                Wait.Until(ExpectedConditions.PresenceOfAllElementsLocatedBy(FieldErrors));
                return Driver.FindElements(FieldErrors)
                             .Select(e => e.Text.Trim())
                             .Where(t => t.Length > 0)
                             .ToList();
            }
            catch { return new List<string>(); }
        }

        /// <summary>
        /// Returns the server-level alert error text (e.g. "Email already in use").
        /// </summary>
        public string GetAlertErrorText()
        {
            try { return Wait.Until(ExpectedConditions.ElementIsVisible(ErrorAlerts)).Text; }
            catch { return string.Empty; }
        }

        public bool IsEmailInputDisplayed()
        {
            try { return WaitForVisible(EmailInput).Displayed; }
            catch { return false; }
        }
    }
}
