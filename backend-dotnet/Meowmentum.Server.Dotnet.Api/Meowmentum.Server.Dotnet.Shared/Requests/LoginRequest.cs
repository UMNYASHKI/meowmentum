using System.ComponentModel.DataAnnotations;

namespace Meowmentum.Server.Dotnet.Shared.Requests
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters long.")]
        public string Password { get; set; }
    }
}
