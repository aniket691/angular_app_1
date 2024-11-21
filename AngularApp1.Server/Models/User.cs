namespace MoniteringSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        // Properties for password reset
        public string? ResetPasswordToken { get; set; }
        public DateTime? ResetPasswordTokenExpiration { get; set; }

        // Properties for email verification
        public bool IsEmailVerified { get; set; } = false; // Defaults to false (not verified)
        public string? VerificationToken { get; set; } // Stores the email verification token
        public DateTime? VerificationTokenExpiration { get; set; } // Optional: Token expiration
    }
}
    