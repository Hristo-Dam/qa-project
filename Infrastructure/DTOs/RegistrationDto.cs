namespace Infrastructure.DTOs
{
    /// <summary>
    /// Data Transfer Object representing registration data for a new eMAG user.
    /// </summary>
    public class RegistrationDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is not RegistrationDto other) return false;
            return FirstName == other.FirstName
                && LastName == other.LastName
                && Email == other.Email
                && Password == other.Password
                && ConfirmPassword == other.ConfirmPassword
                && Phone == other.Phone;
        }

        public override int GetHashCode() =>
            HashCode.Combine(FirstName, LastName, Email, Password, ConfirmPassword, Phone);

        public override string ToString() =>
            $"RegistrationDto {{ Email: {Email}, Name: {FirstName} {LastName} }}";
    }
}
