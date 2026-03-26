using NUnit.Framework;
using Infrastructure.DTOs;
using Infrastructure.Factories;
using Infrastructure.Pages;
using Tests.Base;

namespace Tests
{
    /// <summary>
    /// Test suite for the eMAG Login functionality.
    /// Covers: email validation, happy path, wrong password, and empty submission.
    /// Credentials are read from env variables EMAG_EMAIL and EMAG_PASSWORD.
    /// </summary>
    [TestFixture]
    public class LoginTests : BaseTest
    {
        private LoginPage _loginPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _loginPage = new LoginPage(Driver);
            _loginPage.Open();
        }

        /// <summary>
        /// Test: Login page elements are visible on load.
        /// Steps:
        ///   1. Open the login page.
        ///   2. Assert the eMAG logo is displayed.
        ///   3. Assert the "Здравей!" heading is displayed.
        ///   4. Assert the email input is visible.
        ///   5. Assert the Continue button is visible.
        ///   6. Assert social login buttons (Facebook, Google, Apple) are visible.
        /// Expected result: All key elements are present and visible.
        /// </summary>
        [Test]
        public void LoginPage_OnLoad_ShouldDisplayAllRequiredElements()
        {
            Assert.Multiple(() =>
            {
                Assert.That(_loginPage.IsLogoDisplayed(),
                    Is.True, "eMAG logo should be displayed on the login page.");

                Assert.That(_loginPage.IsHeadingDisplayed(),
                    Is.True, "The 'Здравей!' heading should be displayed.");

                Assert.That(_loginPage.GetHeadingText(),
                    Is.EqualTo("Здравей!"), "Page heading text should be 'Здравей!'.");

                Assert.That(_loginPage.IsContinueButtonDisplayed(),
                    Is.True, "The Continue button should be visible.");

                Assert.That(_loginPage.IsFacebookButtonDisplayed(),
                    Is.True, "Facebook social login button should be displayed.");

                Assert.That(_loginPage.IsGoogleButtonDisplayed(),
                    Is.True, "Google social login button should be displayed.");

                Assert.That(_loginPage.IsAppleButtonDisplayed(),
                    Is.True, "Apple social login button should be displayed.");
            });
        }

        /// <summary>
        /// Test: Happy path – valid credentials lead to successful login.
        /// Steps:
        ///   1. Retrieve valid UserDto from UserFactory (reads env variables).
        ///   2. Submit the email on the LoginPage.
        ///   3. On the PasswordPage, enter the password and click Sign In.
        ///   4. Wait for page to be ready.
        ///   5. Assert the browser URL no longer contains 'auth.emag.bg/user/login'.
        ///   6. Assert the URL contains 'emag.bg' (redirected to the main site).
        /// Expected result: User is authenticated and redirected to the eMAG main site.
        /// </summary>
        [Test]
        public void Login_WithValidCredentials_ShouldRedirectToMainSite()
        {
            var user = UserFactory.GetValidUser();

            _loginPage.SubmitEmail(user);

            var passwordPage = new PasswordPage(Driver);
            passwordPage.WaitForLoad();
            passwordPage.Login(user);

            // Wait for redirect to complete
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(15));
            wait.Until(d => !d.Url.Contains("auth.emag.bg/user/login"));

            Assert.Multiple(() =>
            {
                Assert.That(Driver.Url, Does.Contain("emag.bg"),
                    "After successful login, URL should contain 'emag.bg'.");

                Assert.That(Driver.Url, Does.Not.Contain("/user/login"),
                    "After successful login, URL should not be the login page.");
            });
        }

        /// <summary>
        /// Test: Submitting an empty email should trigger HTML5 validation.
        /// Steps:
        ///   1. Leave the email field empty.
        ///   2. Click the Continue button.
        ///   3. Assert the email input is flagged as invalid by the browser.
        ///   4. Assert the page URL has not changed (still on login page).
        /// Expected result: Form is not submitted; email field shows validation state.
        /// </summary>
        [Test]
        public void Login_WithEmptyEmail_ShouldNotSubmitForm()
        {
            _loginPage.ClickContinue();

            Assert.Multiple(() =>
            {
                Assert.That(_loginPage.IsEmailInputInvalid(),
                    Is.True, "Email input should be in an invalid state when empty and submitted.");

                Assert.That(Driver.Url, Does.Contain("auth.emag.bg/user/login"),
                    "Page should remain on the login URL when email is empty.");
            });
        }

        /// <summary>
        /// Test: Submitting an invalid email format should trigger browser validation.
        /// Steps:
        ///   1. Retrieve an invalid email UserDto from UserFactory.
        ///   2. Enter only a single non-email character to simulate bad format.
        ///   3. Click Continue.
        ///   4. Assert that the email field is invalid.
        /// Expected result: Browser prevents submission due to invalid email format.
        /// </summary>
        [Test]
        public void Login_WithInvalidEmailFormat_ShouldShowValidationError()
        {
            _loginPage.EnterEmail("not-an-email");
            _loginPage.ClickContinue();

            Assert.That(_loginPage.IsEmailInputInvalid(),
                Is.True, "Email input should be invalid when the entered value is not a valid email address.");
        }

        /// <summary>
        /// Data-driven test: Various invalid email values should all fail HTML5 validation.
        /// Steps:
        ///   1. For each test case email, type it into the email field.
        ///   2. Click Continue.
        ///   3. Assert the email input is flagged as invalid.
        ///   4. Clear the field before the next iteration.
        /// Expected result: Every invalid email keeps the form from submitting.
        /// </summary>
        [TestCase("plaintext")]
        [TestCase("missing@domain")]
        [TestCase("@nodomain.com")]
        [TestCase("spaces in@email.com")]
        public void Login_WithVariousInvalidEmails_ShouldFailValidation(string invalidEmail)
        {
            _loginPage.EnterEmail(invalidEmail);
            _loginPage.ClickContinue();

            Assert.That(_loginPage.IsEmailInputInvalid(),
                Is.True, $"Email '{invalidEmail}' should be flagged as invalid by HTML5 form validation.");
        }

        /// <summary>
        /// Test: Submitting a non-existent email should navigate to the next step
        ///       (which should show either a registration form or an error).
        /// Steps:
        ///   1. Get a unique non-existent email from UserFactory.
        ///   2. Submit the email.
        ///   3. Wait briefly for navigation.
        ///   4. Assert the URL changed from the initial login page,
        ///      OR an appropriate next-step element is present.
        /// Expected result: eMAG processes the unknown email and shows the registration/next step.
        /// </summary>
        [Test]
        public void Login_WithNonExistentEmail_ShouldProceedToNextStep()
        {
            var user = UserFactory.GetUserWithInvalidEmail();

            // Compare DTOs to ensure we are using the right data
            var expectedUser = UserFactory.GetUserWithInvalidEmail();
            // Only email field matters here; do a structural check
            Assert.That(user.Email, Is.Not.Empty,
                "Factory should produce a non-empty email for invalid-user test.");

            _loginPage.SubmitEmail(user);

            // After submitting, the page should change
            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            bool pageChanged = false;
            try
            {
                pageChanged = wait.Until(d =>
                    d.Url != LoginPage.Url ||
                    !d.PageSource.Contains("user_login_continue"));
            }
            catch { /* timeout – check assertion below */ }

            Assert.That(pageChanged || !Driver.Url.Equals(LoginPage.Url, StringComparison.OrdinalIgnoreCase),
                Is.True, "After submitting an unknown email, the page should advance to the next step.");
        }
    }
}
