namespace Meowmentum.Server.Dotnet.Shared.Requests.Email;

public class ResetPasswordEmailSendingRequest
{
    public string Email { get; set; }
    public string Name { get; set; }
    public string Otp { get; set; }

}
