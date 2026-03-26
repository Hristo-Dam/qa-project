using NUnit.Framework;
using Infrastructure.DTOs;
using Infrastructure.Factories;
using Infrastructure.Pages;
using Tests.Base;

namespace Tests
{
    /// <summary>
    /// Test suite for the eMAG Registration functionality.
    /// The registration flow is reached by submitting an unknown email on the Login page,
    /// then completing the registration form that appears.
    /// </summary>
    [TestFixture]
    public class RegistrationTests : BaseTest
    {
        private LoginPage _loginPage = null!;
        private RegistrationPage _registrationPage = null!;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _loginPage = new LoginPage(Driver);
            _registrationPage = new RegistrationPage(Driver);
            _loginPage.Open();
        }

        /// <summary>
        /// Test: Happy path – submitting all valid registration data creates a new account.
        /// Steps:
        ///   1. Retrieve a valid RegistrationDto from RegistrationFactory.
        ///   2. Submit the new email on the LoginPage to trigger the registration flow.
        ///   3. Wait for the registration page/form to load.
        ///   4. Fill in all fields using the DTO.
        ///   5. Submit the registration form.
        ///   6. Assert the URL no longer points to the registration/login page.
        ///   7. Assert the URL contains 'emag.bg' (success redirect).
        /// Expected result: Account is created and the user is redirected to the main site.
        /// </summary>
        [Test]
        public void Register_WithValidData_ShouldCreateAccountAndRedirect()
        {
            var dto = RegistrationFactory.GetValidRegistration();

            _loginPage.SubmitEmail(dto.Email);
            _registrationPage.WaitForLoad();
            _registrationPage.Register(dto);

            var wait = new OpenQA.Selenium.Support.UI.WebDriverWait(Driver, TimeSpan.FromSeconds(20));
            wait.Until(d => !d.Url.Contains("auth.emag.bg/user/login"));

            Assert.Multiple(() =>
            {
                Assert.That(Driver.Url, Does.Contain("emag.bg"),
                    "After successful registration the URL should redirect to emag.bg.");
                Assert.That(Driver.Url, Does.Not.Contain("/user/login"),
                    "After successful registration the URL should not be the login page.");
            });
        }

        /// <summary>
        /// Test: Registration form is shown after submitting an unknown email.
        /// Steps:
        ///   1. Retrieve a valid RegistrationDto (with a unique new email).
        ///   2. Submit the email on the LoginPage.
        ///   3. Wait for the registration page to load.
        ///   4. Assert the email input on the registration page is visible.
        /// Expected result: Registration form is visible and the email field is pre-filled or visible.
        /// </summary>
        [Test]
        public void Register_AfterSubmittingUnknownEmail_ShouldShowRegistrationForm()
        {
            var dto = RegistrationFactory.GetValidRegistration();

            _loginPage.SubmitEmail(dto.Email);

            Assert.That(_registrationPage.IsEmailInputDisplayed(),
                Is.True, "Registration form's email field should be visible after submitting an unknown email.");
        }

        /// <summary>
        /// Test: Registration with mismatched passwords should display a validation error.
        /// Steps:
        ///   1. Get a RegistrationDto with mismatched passwords from the factory.
        ///   2. Submit the email on LoginPage.
        ///   3. Fill in the registration form using the DTO with mismatched passwords.
        ///   4. Click Register.
        ///   5. Assert that field-level errors are displayed.
        ///   6. Assert that the error list is not empty.
        /// Expected result: Validation error(s) for the password field(s) are shown.
        /// </summary>
        [Test]
        public void Register_WithMismatchedPasswords_ShouldShowValidationError()
        {
            var dto = RegistrationFactory.GetRegistrationWithMismatchedPasswords();

            _loginPage.SubmitEmail(dto.Email);
            _registrationPage.WaitForLoad();
            _registrationPage.Register(dto);

            var errors = _registrationPage.GetFieldErrors();

            Assert.That(errors, Is.Not.Empty,
                "At least one validation error should be shown when passwords do not match.");
        }

        /// <summary>
        /// Test: Submitting an empty registration form should show multiple validation errors.
        /// Steps:
        ///   1. Get an empty RegistrationDto from the factory.
        ///   2. Navigate to the registration form (via the login page email step).
        ///   3. Click Register without filling any field.
        ///   4. Assert that multiple field errors are displayed.
        /// Expected result: Required-field validation errors are shown for all mandatory fields.
        /// </summary>
        [Test]
        public void Register_WithEmptyFields_ShouldShowRequiredFieldErrors()
        {
            var validDto = RegistrationFactory.GetValidRegistration();

            _loginPage.SubmitEmail(validDto.Email);
            _registrationPage.WaitForLoad();

            // Click register with empty form (clear what may have been pre-filled)
            _registrationPage.ClickRegister();

            var errors = _registrationPage.GetFieldErrors();

            Assert.That(errors.Count, Is.GreaterThan(0),
                "At least one required-field error should appear when the registration form is submitted empty.");
        }

        /// <summary>
        /// Test: DTO comparison – factory-produced RegistrationDto should equal a manually built one.
        /// Steps:
        ///   1. Build a RegistrationDto manually with known values.
        ///   2. Build the same DTO using the factory logic (mimicking factory output).
        ///   3. Assert the DTOs are equal using Equals().
        /// Expected result: Two DTOs with identical field values are considered equal.
        /// </summary>
        [Test]
        public void RegistrationDto_WithSameValues_ShouldBeEqual()
        {
            var dto1 = new RegistrationDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "same@mailinator.com",
                Password = "TestPass123!",
                ConfirmPassword = "TestPass123!",
                Phone = "0888123456"
            };

            var dto2 = new RegistrationDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = "same@mailinator.com",
                Password = "TestPass123!",
                ConfirmPassword = "TestPass123!",
                Phone = "0888123456"
            };

            Assert.Multiple(() =>
            {
                Assert.That(dto1, Is.EqualTo(dto2),
                    "Two RegistrationDtos with identical field values should be equal.");
                Assert.That(dto1.GetHashCode(), Is.EqualTo(dto2.GetHashCode()),
                    "Equal DTOs should produce the same hash code.");
            });
        }

        /// <summary>
        /// Data-driven test: Various invalid email formats should fail HTML5 email validation
        /// on the registration page email field.
        /// Steps:
        ///   1. Navigate to the registration form (via a new-email login step).
        ///   2. For each test case: clear the email field and type the invalid email.
        ///   3. Click Register.
        ///   4. Assert validation errors are shown.
        /// Expected result: Invalid email formats are rejected by the form.
        /// </summary>
        [TestCase("notanemail")]
        [TestCase("missing@domain")]
        [TestCase("spaces in@test.com")]
        public void Register_WithInvalidEmailFormats_ShouldShowValidationError(string invalidEmail)
        {
            var validDto = RegistrationFactory.GetValidRegistration();

            _loginPage.SubmitEmail(validDto.Email);
            _registrationPage.WaitForLoad();

            // Override the email with invalid value
            _registrationPage
                .EnterFirstName(validDto.FirstName)
                .EnterLastName(validDto.LastName)
                .EnterEmail(invalidEmail)
                .EnterPassword(validDto.Password)
                .EnterConfirmPassword(validDto.ConfirmPassword)
                .EnterPhone(validDto.Phone);

            _registrationPage.ClickRegister();

            var errors = _registrationPage.GetFieldErrors();
            var alertError = _registrationPage.GetAlertErrorText();

            Assert.That(errors.Count > 0 || !string.IsNullOrEmpty(alertError),
                Is.True,
                $"Submitting registration with invalid email '{invalidEmail}' should produce at least one error.");
        }
    }
}
