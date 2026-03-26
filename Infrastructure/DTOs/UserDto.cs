namespace Infrastructure.DTOs
{
    /// <summary>
    /// Data Transfer Object representing login credentials for an eMAG user.
    /// </summary>
    public class UserDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public override bool Equals(object? obj)
        {
            if (obj is not UserDto other) return false;
            return Email == other.Email && Password == other.Password;
        }

        public override int GetHashCode() => HashCode.Combine(Email, Password);

        public override string ToString() => $"UserDto {{ Email: {Email} }}";
    }
}
