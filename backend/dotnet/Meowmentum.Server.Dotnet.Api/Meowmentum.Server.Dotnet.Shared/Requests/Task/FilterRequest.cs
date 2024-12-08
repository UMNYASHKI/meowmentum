using Meowmentum.Server.Dotnet.Core.Entities;
using TaskStatus = Meowmentum.Server.Dotnet.Core.Entities.TaskStatus;

namespace Meowmentum.Server.Dotnet.Shared.Requests.Task;

public class TaskFilterRequest
{
    public long? TaskId { get; set; }
    public List<TaskStatus?> Status { get; set; } = new List<TaskStatus?>();
    public List<TaskPriority?> Priorities { get; set; } = new List<TaskPriority?>();
    public List<long?> TagIds { get; set; } = new List<long?>();
}
