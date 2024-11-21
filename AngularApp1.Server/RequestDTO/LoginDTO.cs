
using System.ComponentModel.DataAnnotations;

namespace MoniteringSystem.RequestDTO
{
    public class LoginDTO
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}