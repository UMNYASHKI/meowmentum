using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Requests
{
    public class PasswordUpdateRequest
    {
        [Required]
        [JsonProperty("resetToken")]
        public string ResetToken { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("email")]
        public string Email { get; set; }

        [Required]
        [StringLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [JsonProperty("new_password")]
        public string NewPassword { get; set; }
    }
}
