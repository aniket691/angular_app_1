using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoniteringSystem.Data;
using MoniteringSystem.Models;
using MoniteringSystem.RequestDTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace MoniteringSystem.Service
{
    public class AuthService : IAuthService
    {
        private readonly AuthDbContext _context;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService; // Add this line for email service

        public AuthService(AuthDbContext context, IConfiguration config, IEmailService emailService)
        {
            _context = context;
            _config = config;
            _emailService = emailService; // Initialize the email service
        }

        public async Task<bool> RegisterAsync(RegisterDTO dto)
        {
            // Check if user already exists
            if (await _context.User.AnyAsync(u => u.Email == dto.Email))
                return false; // User already exists

            // Create new user
            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsEmailVerified = false,  // Initially not verified
                VerificationToken = GenerateVerificationToken() // Generate token for email verification
            };

            // Save user
            _context.User.Add(user);
            await _context.SaveChangesAsync();

            // Send verification email
            var verificationLink = $"http://localhost:5221/api/Auth/verify-email?token={user.VerificationToken}";
            await _emailService.SendEmailAsync(user.Email, "Verify Your Email",
                $"Please verify your email by clicking the following link: {verificationLink}");

            return true;
        }

        private string GenerateVerificationToken()
        {
            return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        }

        public async Task<string?> LoginAsync(LoginDTO dto)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == dto.Email);

            // Check if the user exists and if their email is verified
            if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash) || !user.IsEmailVerified)
                return null; // Invalid credentials or email not verified

            return GenerateJwtToken(user); // Continue if verified
        }

        public async Task<bool> RequestPasswordResetAsync(RequestPasswordResetDTO dto)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) return false; // User not found

            // Generate reset token
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            user.ResetPasswordToken = token;
            user.ResetPasswordTokenExpiration = DateTime.UtcNow.AddHours(1); // Token valid for 1 hour

            await _context.SaveChangesAsync();

            // Send reset token via email
            var resetLink = $"http://your-frontend-domain/reset-password?token={token}";
            await _emailService.SendEmailAsync(user.Email, "Reset Password", $"Click here to reset your password: {resetLink}");

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDTO dto)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.ResetPasswordToken == dto.Token && u.ResetPasswordTokenExpiration > DateTime.UtcNow);
            if (user == null) return false; // Invalid or expired token

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.ResetPasswordToken = null; // Clear the reset token
            user.ResetPasswordTokenExpiration = null;

            await _context.SaveChangesAsync();
            return true;
        }

        private string GenerateJwtToken(User user)
        {
            // Create claims for the token
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // Configure JWT settings
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Create the JWT token
            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds);

            // Return the serialized token
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> VerifyEmailAsync(string token)
        {
            var user = await _context.User.SingleOrDefaultAsync(u => u.VerificationToken == token);

            if (user == null || user.IsEmailVerified)
            {
                return false; // Invalid token or email already verified
            }

            // Mark email as verified
            user.IsEmailVerified = true;
            user.VerificationToken = null; // Clear the token
            user.VerificationTokenExpiration = null; // Optional: Clear token expiration if used

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
