using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Registration;

public record OtpSendRequest([EmailAddress]string Email);
