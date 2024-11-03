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
    public string Email { get; set; }
    [Required]
    public string OtpCode { get; set; }
}
