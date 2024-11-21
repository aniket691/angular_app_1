using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoniteringSystem.RequestDTO;
using MoniteringSystem.Service;
using System.Threading.Tasks;

namespace MoniteringSystem.Controllers
{
    //[ServiceFilter(typeof(AuthLoggingFilter))]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        //[ServiceFilter(typeof(ValidateModelAttribute))]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (!result) return BadRequest("User already exists.");
            return Ok("User registered successfully.");
        }

        //[ServiceFilter(typeof(ValidateModelAttribute))]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var token = await _authService.LoginAsync(dto);
            if (token == null) return Unauthorized("Invalid login credentials or email not verified.");
            return Ok(new { Token = token });
        }

        [HttpPost("request-password-reset")]
        public async Task<IActionResult> RequestPasswordReset(RequestPasswordResetDTO dto)
        {
            var result = await _authService.RequestPasswordResetAsync(dto);
            if (!result) return NotFound("User not found.");
            return Ok("Password reset email sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            if (!result) return BadRequest("Invalid or expired token.");
            return Ok("Password reset successfully.");
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            var result = await _authService.VerifyEmailAsync(token);
            if (!result)
            {
                return BadRequest("Invalid or expired token.");
            }
            return Ok("Email verified successfully. You can now log in.");
        }

    }
}
