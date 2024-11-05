using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Registration;

public class RegisterUserRequest
{
    [Required]
    [JsonProperty("username")]
    public string UserName { get; set; }
    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public string Email { get; set; }

    [Required]
    [JsonProperty("password")]
    public string Password { get; set; }
}
