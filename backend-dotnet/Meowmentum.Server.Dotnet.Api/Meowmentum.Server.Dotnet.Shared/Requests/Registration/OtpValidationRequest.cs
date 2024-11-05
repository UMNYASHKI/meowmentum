using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Registration;

public class OtpValidationRequest
{
    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public string Email { get; set; }
    [Required]
    [JsonProperty("otpCode")]
    public string OtpCode { get; set; }
}
