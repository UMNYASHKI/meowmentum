using Meowmentum.Server.Dotnet.Core.Entities;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;

namespace Meowmentum.Server.Dotnet.Shared.Responses.Task;

public class TaskResponse
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? Deadline { get; set; }
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public List<TagResponse>? Tags { get; set; }
    public List<TimeInterval>? TimeSpent { get; set; }
}

