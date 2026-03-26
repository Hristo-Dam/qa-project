using Infrastructure.DTOs;

namespace Infrastructure.Factories
{
    /// <summary>
    /// Factory for creating <see cref="UserDto"/> instances used in login tests.
    /// Credentials are read from environment variables: EMAG_EMAIL and EMAG_PASSWORD.
    /// </summary>
    public static class UserFactory
    {
        /// <summary>
        /// Creates a valid UserDto from environment variables.
        /// Steps:
        ///   1. Read EMAG_EMAIL and EMAG_PASSWORD env variables.
        /// Expected result: A UserDto with valid credentials.
        /// </summary>
        public static UserDto GetValidUser() => new UserDto
        {
            Email = Environment.GetEnvironmentVariable("EMAG_EMAIL")
                    ?? throw new InvalidOperationException("Environment variable EMAG_EMAIL is not set."),
            Password = Environment.GetEnvironmentVariable("EMAG_PASSWORD")
                       ?? throw new InvalidOperationException("Environment variable EMAG_PASSWORD is not set.")
        };

        /// <summary>
        /// Creates a UserDto with an invalid (non-existent) email.
        /// Steps:
        ///   1. Return a DTO with a random invalid email and arbitrary password.
        /// Expected result: A UserDto that should fail authentication.
        /// </summary>
        public static UserDto GetUserWithInvalidEmail() => new UserDto
        {
            Email = $"notexisting_{Guid.NewGuid():N}@invalid-domain-test.bg",
            Password = "SomePassword123!"
        };

        /// <summary>
        /// Creates a UserDto with a valid email but wrong password.
        /// Steps:
        ///   1. Read EMAG_EMAIL from env, use a wrong password.
        /// Expected result: A UserDto that should fail authentication.
        /// </summary>
        public static UserDto GetUserWithWrongPassword() => new UserDto
        {
            Email = Environment.GetEnvironmentVariable("EMAG_EMAIL")
                    ?? throw new InvalidOperationException("Environment variable EMAG_EMAIL is not set."),
            Password = "WrongPassword_XYZ_999!"
        };

        /// <summary>
        /// Returns a UserDto with empty email and empty password (boundary test).
        /// </summary>
        public static UserDto GetEmptyUser() => new UserDto
        {
            Email = string.Empty,
            Password = string.Empty
        };
    }
}
