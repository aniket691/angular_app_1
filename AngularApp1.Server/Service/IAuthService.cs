using MoniteringSystem.RequestDTO;

namespace MoniteringSystem.Service
{
    public interface IAuthService
    {
        Task<bool> RegisterAsync(RegisterDTO dto);
        Task<string?> LoginAsync(LoginDTO dto);
        Task<bool> RequestPasswordResetAsync(RequestPasswordResetDTO dto);
        Task<bool> ResetPasswordAsync(ResetPasswordDTO dto);
        Task<bool> VerifyEmailAsync(string token);
    }
}
