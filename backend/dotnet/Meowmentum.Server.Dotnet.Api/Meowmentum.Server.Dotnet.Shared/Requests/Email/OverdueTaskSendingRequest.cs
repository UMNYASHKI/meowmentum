namespace Meowmentum.Server.Dotnet.Shared.Requests.Email;

public class OverdueTaskSendingRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public int TaskCount { get; set; }
}
