using System.ComponentModel.DataAnnotations;

namespace MoniteringSystem.RequestDTO
{
    public class RegisterDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }


}
