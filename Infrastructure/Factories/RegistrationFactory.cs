using Infrastructure.DTOs;

namespace Infrastructure.Factories
{
    /// <summary>
    /// Factory for creating <see cref="RegistrationDto"/> instances used in registration tests.
    /// </summary>
    public static class RegistrationFactory
    {
        /// <summary>
        /// Creates a valid RegistrationDto with a unique email for a new account.
        /// Steps:
        ///   1. Generate a unique email using a GUID suffix to avoid conflicts.
        ///   2. Populate all required fields with valid data.
        /// Expected result: A RegistrationDto that should pass registration validation.
        /// </summary>
        public static RegistrationDto GetValidRegistration()
        {
            var unique = Guid.NewGuid().ToString("N")[..8];
            return new RegistrationDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"testuser_{unique}@mailinator.com",
                Password = "TestPass123!",
                ConfirmPassword = "TestPass123!",
                Phone = "0888123456"
            };
        }

        /// <summary>
        /// Creates a RegistrationDto with an already-registered email (from env variable).
        /// Steps:
        ///   1. Read EMAG_EMAIL env variable as the existing email.
        /// Expected result: Registration should fail with "email already exists" error.
        /// </summary>
        public static RegistrationDto GetRegistrationWithExistingEmail() => new RegistrationDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = Environment.GetEnvironmentVariable("EMAG_EMAIL")
                    ?? throw new InvalidOperationException("Environment variable EMAG_EMAIL is not set."),
            Password = "TestPass123!",
            ConfirmPassword = "TestPass123!",
            Phone = "0888123456"
        };

        /// <summary>
        /// Creates a RegistrationDto with mismatched passwords.
        /// Steps:
        ///   1. Set Password and ConfirmPassword to different values.
        /// Expected result: Validation error about password mismatch.
        /// </summary>
        public static RegistrationDto GetRegistrationWithMismatchedPasswords()
        {
            var unique = Guid.NewGuid().ToString("N")[..8];
            return new RegistrationDto
            {
                FirstName = "Test",
                LastName = "User",
                Email = $"testuser_{unique}@mailinator.com",
                Password = "TestPass123!",
                ConfirmPassword = "DifferentPass456!",
                Phone = "0888123456"
            };
        }

        /// <summary>
        /// Creates a RegistrationDto with an invalid email format.
        /// Steps:
        ///   1. Set email to a malformed string without @ symbol.
        /// Expected result: Validation error about invalid email.
        /// </summary>
        public static RegistrationDto GetRegistrationWithInvalidEmail() => new RegistrationDto
        {
            FirstName = "Test",
            LastName = "User",
            Email = "not-a-valid-email",
            Password = "TestPass123!",
            ConfirmPassword = "TestPass123!",
            Phone = "0888123456"
        };

        /// <summary>
        /// Creates a RegistrationDto with empty required fields.
        /// Steps:
        ///   1. Leave all fields as empty strings.
        /// Expected result: Multiple validation errors for required fields.
        /// </summary>
        public static RegistrationDto GetEmptyRegistration() => new RegistrationDto
        {
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            Password = string.Empty,
            ConfirmPassword = string.Empty,
            Phone = string.Empty
        };
    }
}
