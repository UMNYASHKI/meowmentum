namespace Meowmentum.Server.Dotnet.Shared.Requests.Email;

public class OtpEmailSendingRequest
{
    public string Email { get; set; }
    public string Otp { get; set; }
    public string Name { get; set; }
}
