namespace Meowmentum.Server.Dotnet.Shared.Requests.Email;

public class UpcomingTaskSendingRequest
{
    public string Email { get; set; }
    public string UserName { get; set; }
    public string TaskName { get; set; }
    public string TaskUrl { get; set; }
}
