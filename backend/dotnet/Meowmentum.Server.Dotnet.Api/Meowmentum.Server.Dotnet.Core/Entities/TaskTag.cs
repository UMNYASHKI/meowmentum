namespace Meowmentum.Server.Dotnet.Core.Entities;

public class TaskTag
{
    public long TaskId { get; set; }
    public virtual Task Task { get; set; }

    public long TagId { get; set; }
    public virtual Tag Tag { get; set; }
}
